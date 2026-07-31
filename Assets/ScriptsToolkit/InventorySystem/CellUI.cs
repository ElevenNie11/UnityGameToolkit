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
