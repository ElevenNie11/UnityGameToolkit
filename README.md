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

## 动态加入UI到背包物品栏
### 思路如下：
先理清架构：
- 物品数据区分类型：武器 / 食物
- 两套背包容器：武器背包、食物背包（对应你两个 ScrollView）
- 拾取物品 → 判断类型 → 放入对应背包 → 动态生成 CellUI 显示在对应 Content
- 两套独立 UI：Content_Weapon（武器格子父物体）、Content_Food（食物格子父物体）

### 脚本需求：

#### Script_01: `ItemData.cs`

- 新建脚本`ItemData.cs`，用来定义物品信息: (使用方式：在 Project 窗口右键 → Inventory/Item，创建武器、食物配置文件，挂上对应的图片、类型)
  
  <img width="854" height="511" alt="image" src="https://github.com/user-attachments/assets/cff1af01-a63b-4379-b2f2-59537351b07f" />


  ````
  using UnityEngine;
  //此脚本用来定义物品信息
  //物品类型枚举
  public enum ItemType
  {
      Weapon,    //武器工具
      Food       //食物料理
  }
  // CreateAssetMenu 特性：这是 ScriptableObject 专属特性：作用是在Unity编辑器右键菜单生成配置文件
  //1. fileName = "NewItem"：新建物品资源默认文件名
  //2. menuName = "Inventory/Item"：右键路径
    [CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
  // ScriptableObject 适合存放静态数据：物品属性、技能数据、怪物属性；
    public class ItemData : ScriptableObject
    {
        public string itemName;   //物品名称
        public ItemType itemType; //物品类型（武器/食物 -> 枚举选择）
        public Sprite icon;       //物品图标
        public int maxStack = 1;  //堆叠上限（武器一般不能堆叠，食物可以堆叠）
    }
  ````
  
#### 整体工作流程（背包系统逻辑）

- 右键创建多个ItemData资源：铁剑、苹果、面包
- 在 Inspector 分别设置：
  ```
  铁剑：类型 Weapon，maxStack=1
  苹果：类型 Food，maxStack=20
  ```
- 背包格子只保存：ItemData引用 + 当前数量
- 当拾取物品时，读取 ItemData 里的图标、名字、堆叠上限做逻辑判断
  
---

#### Script_02: `CellUI.cs`

- 此脚本绑定到*Cell_UI*预制体上
- 改造*Cell_UI*预制体的层级（一定要做这件事！！！）
 <img width="1280" height="766" alt="image" src="https://github.com/user-attachments/assets/490162eb-6f89-4156-85d6-af6b50cc434a" />


  ```` 
  using UnityEngine;
  using UnityEngine.UI;
  using TMPro;
  
  // 格子UI刷新，自动查找 iconImage 和 countText (TMP)，数量大于 1 才显示
  public class CellUI : MonoBehaviour
  {
      [Header("UI组件")]
      public Image iconImage;
      public TMP_Text countText;
  
      // 一个背包格子Slot = 存什么物品(CurrentItem) + 物品数量(CurrentCount)
      public ItemData CurrentItem { 
          get;
          private set;
      }
      public int CurrentCount { 
          get;
          private set;
      }
      //以上两个比较安全的写法：
      //get 公开读取：外部任何脚本都可以读取，比如 slot.CurrentItem、slot.CurrentCount
      //private set 私有赋值：只有这个类内部的函数才能修改数值
      //外部脚本不能直接写 slot.CurrentCount = 10，否则会报错
  
      //比较危险的写法是：
      //public ItemData CurrentItem;
      //public int CurrentCount;
      //可以用，但危险：
      //物品数量不能随便乱改，修改数量时需要做一系列校验：
      //1. 判断物品是否为空
      //2. 判断有没有超过堆叠上限 maxStack
      //3. 数量减到 0 的时候，清空格子物品
      //4. 刷新 UI 图标
  
      //物体激活时最先执行的Unity生命周期函数
      private void Awake()  
      {
          AutoBind();       
          RefreshCell(null, 0);
      }
  
      //只在Unity编辑器模式生效，游戏运行时不会调用
      //作用：在Inspector修改脚本参数或者新建格子预设时会[自动执行]AutoBind()，不用每次手动拖拽组件。
      //方便开发，减少拖拽操作
      private void OnValidate(){
          AutoBind();
      }
  
      //自动绑定：懒人功能：不用手动把图标、文本拖进Inspector，脚本会自动在子物体查找UI组件
      private void AutoBind()
      {
          if (iconImage == null)
          {
              Transform icon = transform.Find("iconImage");
              if (icon != null)
              {
                  iconImage = icon.GetComponent<Image>();
              }
          }
  
          if (countText == null)
          {
              countText = GetComponentInChildren<TMP_Text>(true);
          }
      }
  
      //刷新背包格子的显示
      public void RefreshCell(ItemData item, int count)
      {
          CurrentItem = item;
          CurrentCount = Mathf.Max(0, count);
  
          if (item == null || CurrentCount <= 0)
          {
              //空格子逻辑：没有物品/物品数量为0
              if (iconImage != null)
              {
                  iconImage.sprite = null;  //清空图片
                  iconImage.enabled = false;//隐藏图片组件
              }
              if (countText != null)
              {
                  countText.text = string.Empty;//清空数字文字
              }
              return;
          }
          //格子有物品时执行：
          if (iconImage != null)
          {
              iconImage.enabled = true;        //开启图片显示
              iconImage.sprite = item.icon;    //赋值物品图标（ItemData里的Sprite）
              iconImage.preserveAspect = true; //保持图片原始比例，不会拉伸变形
          }
          if (countText != null)
          {
              //三元运算符：数量>1才显示数字；等于1的时候，不显示数量文字
              countText.text = CurrentCount > 1 ? CurrentCount.ToString() : string.Empty;
          }
      }
      
      //清空格子的外部方法
      public void Clear()
      {
          RefreshCell(null, 0);
      }
    }
  ````

*在这份脚本中最主要的思想就是实现了用代码自动化替换了手动拖拽调整Inspector面板*

---

#### Script_03: 背包管理器（InventoryManager.cs，核心）
- 实现两套背包数据：挂载到PackagePanel或者一个管理器空对象上

功能如下：
- 自动区分武器 / 食物，存入对应栏
- 支持物品堆叠
- 自动刷新 UI 格子
  
