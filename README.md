# Unity脚本工具包[Unity Scripts Toolkit]
# 目录
- 剧情对话：https://github.com/ElevenNie11/UnityDialogSystem
- UI：
- 背包系统：

## UI_Effects
- 打字机效果: Typewriter_Effect
- 音量滑动条: VolumnController
- 技能冷却cd: SkillCooldown（*给按钮绑定On Click()事件）
- 片头动画:   Cutscene_ver1
- 片尾动画:   EndingCutscene_ver1  TestEnding（*给测试按钮绑定On Click()事件）
- 无代码代码实现多个画面切换: Toggle
<img width="1280" height="771" alt="1" src="https://github.com/user-attachments/assets/61e5d832-7e1d-4a7c-990c-2ec2c9332570" />

片头：动画使用*Timeline*编排

<img width="1280" height="766" alt="image" src="https://github.com/user-attachments/assets/d6b0c831-fb25-41ca-b333-c3a108843835" />

## 无代码实现多个画面切换: *Toggle*

Hierarchy结构

<img width="239" height="282" alt="image" src="https://github.com/user-attachments/assets/0e2e3758-d9f4-4f49-9e84-89249e00d0b3" />

---

## 背包系统的测试数据
<img width="934" height="948" alt="image" src="https://github.com/user-attachments/assets/f6cb37f7-4c3e-4a58-903b-daff9a58aaaf" />
<img width="2559" height="1533" alt="image" src="https://github.com/user-attachments/assets/e8b1fde6-b4cb-447e-8397-f8a753327943" />

# Unity背包系统：基于单一数据源 + 插入式拖拽排序的 Unity UI 背包系统

## 设计思路
- **数据与 UI 分离**：所有业务状态只存在一份 `List<InventoryItemData>`，UI 只负责读取和显示
- **统一刷新**：道具获得、消耗、排序本质上都是改数据，然后统一调用 `Refresh()`
- **插入排序**：拖拽 A 到 B 上，含义是把 A 插入到 B 前面（不是交换）
- **Slot 只做显示**：格子不保存业务状态，只负责 Bind 数据和响应事件

## Hierarchy 结构

```
Canvas_Inventory
└── Panel_Inventory
    ├── ScrollView_Bag
    │   ├── Viewport
    │   │   └── Content              ← 挂 GridLayoutGroup + ContentSizeFitter
    │   ├── Scrollbar Horizontal     ← 可删除（横向滚动关闭）
    │   └── Scrollbar Vertical
    ├── Panel_Detail                 ← 右侧详情面板
    │   ├── Text_ItemName
    │   ├── Text_ItemDescription
    │   └── Image_ItemIcon
    ├── DragLayer                    ← 必须排在最后（最上层）
    │   └── Image_DragIcon           ← 拖拽时跟随鼠标的图标
    └── ExitButton

InventoryManager                     ← 场景中的空物体，挂管理脚本
```

### Content 配置

| 组件 | 设置 |
|------|------|
| ScrollRect | Horizontal 关闭，Vertical 开启 |
| Content RectTransform | Anchor 顶部拉伸，Pivot = (0.5, 1) |
| GridLayoutGroup | Cell Size 控格子大小，Spacing 控间距；Constraint = Fixed Column Count（如 5 列） |
| ContentSizeFitter | Vertical Fit = Preferred Size |

### Slot_Prefab 结构

```
Slot_Prefab (Image + CanvasGroup + InventorySlot)
├── Image_Icon        (道具图标)
├── Text_Count        (数量角标)
├── Image_Selected    (选中边框)
└── Image_EmptyMask   (空格遮罩)
```

**Raycast Target 设置：**
- 根节点背景 Image：**开启**（点击、拖拽、Drop 都靠它接事件）
- 所有子节点（Icon / Count / Selected / EmptyMask）：**关闭**（避免截走事件）

---

## 脚本说明

### 1. InventoryItemData.cs —— 道具数据类

**功用**：纯数据容器，描述一个道具的所有属性。

**关键点**：
- 不继承 `MonoBehaviour`，不需要挂载
- 加 `[System.Serializable]` 才能在 Inspector 里编辑

---

### 2. InventoryManager.cs —— 背包管理器

**功用**：持有唯一数据源 `List<InventoryItemData> items`，提供增删移操作。

**关键点**：
- **单例模式**：`Instance` 静态变量，其他脚本直接访问
- **插入排序核心**：
  ```csharp
  InventoryItemData item = items[from];
  items.RemoveAt(from);
  // 如果 from 在 target 前面，删除后 target 索引前移一格，所以 insertIndex 要减 1
  int insertIndex = from < target ? target - 1 : target;
  items.Insert(insertIndex, item);
  ```
- 每次操作后调用 `InventoryUI.Instance.Refresh()` 统一刷新

---

### 3. InventorySlot.cs —— 格子脚本

**功用**：只做显示 + 接事件，不保存业务数据。

实现的接口：
- `IPointerClickHandler` —— 点击选中
- `IBeginDragHandler` / `IDragHandler` / `IEndDragHandler` —— 拖拽
- `IDropHandler` —— 接收放下

**关键点**：
- `Bind(data, index)`：绑定数据，刷新图标、数量、空格遮罩显示
- `SetSelected(bool)`：控制选中边框显示
- **拖拽开始时** `canvasGroup.blocksRaycasts = false`，否则源格子会挡住射线，目标格子收不到 `OnDrop`
- **拖拽结束时** 恢复 `blocksRaycasts = true`
- `OnDrop` 中通过 `eventData.pointerDrag` 拿到拖拽来源的 Slot

---

### 4. InventoryUI.cs —— 背包 UI 管理器

**功用**：统一刷新所有格子、管理选中状态、控制拖拽图标、更新详情面板。

```csharp
public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;
    public Transform content;
    public GameObject slotPrefab;
    public Image dragIcon;
    public TextMeshProUGUI detailNameText;
    public TextMeshProUGUI detailDescText;
    public Image detailIcon;

    private List<InventorySlot> slots = new List<InventorySlot>();
    private int selectedIndex = -1;

    public void Refresh() { ... }
    public void SelectSlot(int index) { ... }
    public void ShowDragIcon(Sprite icon, Vector2 pos) { ... }
    public void UpdateDragIcon(Vector2 pos) { ... }
    public void HideDragIcon() { ... }
}
```

**关键点**：
- **Refresh()**：遍历数据源 `items`，给每个 Slot 做 `Bind`，数据不够的格子隐藏
- **动态生成格子**：`Instantiate(slotPrefab, content)` 运行时创建，不手动摆
- **SelectSlot(index)**：记录选中索引 → 刷新选中边框 → 更新右侧详情面板
- **拖拽图标**：`dragIcon.raycastTarget = false`，否则会挡住目标格子导致 `OnDrop` 不触发

---

## 脚本挂载指南

| 脚本 | 挂在哪里 | 需要配置的引用 |
|------|---------|--------------|
| InventoryItemData | 不挂载 | — |
| InventoryManager | 场景空物体 `InventoryManager` | 无（items 列表可在 Inspector 手动加测试数据） |
| InventorySlot | `Slot_Prefab` 预制体根节点 | Icon Image、Count Text、Selected Image、Empty Mask（拖4个子节点）；需额外加 CanvasGroup 组件 |
| InventoryUI | `Panel_Inventory` | Content、Slot Prefab、Drag Icon、Detail Name Text、Detail Desc Text、Detail Icon |

---

## 运行流程

```
玩家点击格子
  → Slot.OnPointerClick
    → InventoryUI.SelectSlot(index)
      → 记录 selectedIndex
      → Refresh() 刷新选中边框
      → 更新详情面板

玩家拖拽 A 到 B 上
  → OnBeginDrag: 源Slot blocksRaycasts=false, 显示DragIcon
  → OnDrag: DragIcon跟随鼠标
  → OnDrop(B上): eventData.pointerDrag 拿到源Slot
    → InventoryManager.MoveItem(from, target)
      → 插入排序改 items 数据
      → Refresh() 统一刷新
  → OnEndDrag: 源Slot blocksRaycasts=true, 隐藏DragIcon
```

---

### Q: 怎么添加测试数据？

两种方式：
- **手动**：选中 InventoryManager，在 Inspector 的 Items 列表里改 Size，逐个填 itemId、itemName、icon、count、description
- **脚本**：写一个测试脚本，在 Start() 里 `InventoryManager.Instance.items.Add(...)` 批量添加，最后调用 `Refresh()`
