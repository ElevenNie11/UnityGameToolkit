# Unity-UGUI脚本工具包[Unity Scripts Toolkit]
# 所有脚本均在[Assets/ScriptsToolkit]文件夹里
# 目录 [Content]

## #01. DialogueSystem
- 剧情对话：https://github.com/ElevenNie11/UnityDialogSystem

## #02. UI_Effects
- 打字机效果: Typewriter_Effect
- 音量滑动条: VolumnController
- 技能冷却cd: SkillCooldown（*给按钮绑定On Click()事件）
- 片头动画:   Cutscene_ver1（动画使用*Timeline*编排）
- 片尾动画:   EndingCutscene_ver1  TestEnding（*给测试按钮绑定On Click()事件）
- 无代码代码实现多个画面切换: Toggle

## #03. InventorySystem
- 背包数据源：InventoryItemData.cs
- 背包管理：InventoryManager.cs
- 背包UI刷新: InventoryUI.cs

## #04. HealthSystem
- 血量系统：HealthSystem.cs（血条UI用 **Slider** 实现）

## #05. MissonSystem(Quests)
- 任务管理：QuestManager.cs
- 任务UI：QuestUI.cs

## #06. ShopSystem
- 商店管理：ShopManager.cs
- 商品UI：  ShopUI.cs
- 商品管理： ShopItemSlot.cs

## #07. SaveSystem
#### 游戏存档系统：只存血量、金币、任务进度，不碰道具（因为道具图标是sprite，如果要存档就要修改ItemData的数据结构，改动量太大，所以单独建一个GitHub仓库来做一个完整的存档系统：）
- 数据结构：SaveData.cs
- 存档/读档：SaveSystem.cs

---

## 无代码实现多个画面切换: *Toggle*

Hierarchy结构

<img width="239" height="282" alt="image" src="https://github.com/user-attachments/assets/0e2e3758-d9f4-4f49-9e84-89249e00d0b3" />

---

## 背包系统
### Hierarchy 结构

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

### 脚本挂载指南

| 脚本 | 挂在哪里 | 需要配置的引用 |
|------|---------|--------------|
| InventoryItemData | 不挂载 | — |
| InventoryManager | 场景空物体 `InventoryManager` | 无（items 列表可在 Inspector 手动加测试数据） |
| InventorySlot | `Slot_Prefab` 预制体根节点 | Icon Image、Count Text、Selected Image、Empty Mask（拖4个子节点）；需额外加 CanvasGroup 组件 |
| InventoryUI | `Panel_Inventory` | Content、Slot Prefab、Drag Icon、Detail Name Text、Detail Desc Text、Detail Icon |
