using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour,
                             IPointerClickHandler,
                             IBeginDragHandler,
                             IDragHandler,
                             IEndDragHandler,
                             IDropHandler
{
    [Header("子节点引用")]
    public Image iconImage;
    public Text countText;
    public Image selectedImage;
    public Image emptyMask;

    [HideInInspector] public int index = -1;  //对应数据源中的索引

    private CanvasGroup canvasGroup;
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    //绑定数据，刷新显示
    public void Bind(InventoryItemData data, int idx)
    {
        index = idx;
        if (data != null)
        {
            iconImage.sprite = data.icon;
            iconImage.enabled = true;
            countText.text = data.count > 1 ? data.count.ToString() : "";
            emptyMask.enabled = false;
        }
        else
        {
            iconImage.enabled = false;
            countText.text = "";
            emptyMask.enabled = true;
        }
    }

    //设置选中态
    public void SetSelected(bool selected)
    {
        selectedImage.enabled = selected;
    }

    //点击道具格子
    //PointerEventData是Unity引擎内置的类：它属于UnityEngine.EventSystems，Unity会自动帮我们new一个对象，里面包含了点击的鼠标按键、点击位置等信息
    public void OnPointerClick(PointerEventData eventData)
    {
        InventoryUI.Instance.SelectSlot(index);
    }

    //开始拖拽
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (index < 0 || InventoryManager.Instance.items[index] == null) return;
        //源Slot暂时不阻挡射线，否则 OnDrop 收不到
        canvasGroup.blocksRaycasts = false;
        //显示拖拽图标
        InventoryUI.Instance.ShowDragIcon(InventoryManager.Instance.items[index].icon,eventData.position);
    }

    //拖拽过程中ing
    public void OnDrag(PointerEventData eventData)
    {
        InventoryUI.Instance.UpdateDragIcon(eventData.position);
    }

    //拖拽结束
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        InventoryUI.Instance.HideDragIcon();
    }

    //松开鼠标
    public void OnDrop(PointerEventData eventData)
    {
        //拿到拖拽来源的Slot
        InventorySlot fromSlot = eventData.pointerDrag.GetComponent<InventorySlot>();
        if (fromSlot == null || fromSlot.index == index) return;
        //调用插入排序
        InventoryManager.Instance.MoveItem(fromSlot.index, index);
    }
}