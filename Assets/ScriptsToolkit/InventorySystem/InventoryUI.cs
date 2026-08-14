using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("引用")]
    public Transform content;           //ScrollView/Viewport/Content
    public GameObject slotPrefab;       //Slot_Prefab
    public Image dragIcon;              //DragLayer/Image_DragIcon

    [Header("详情面板")]
    public TextMeshProUGUI detailNameText;
    public TextMeshProUGUI detailDescText;
    public Image detailIcon;

    private List<InventorySlot> slots = new List<InventorySlot>();
    public int selectedIndex = -1;

    void Awake()
    {
        Instance = this;
        dragIcon.gameObject.SetActive(false);
        dragIcon.raycastTarget = false;     //确保拖拽图标不参与射线
    }

    void Start()
    {
        Refresh();
    }

    //统一刷新：遍历数据源，给每个Slot做Bind
    public void Refresh()
    {
        var items = InventoryManager.Instance.items;
        while (slots.Count < items.Count)
        {
            GameObject go = Instantiate(slotPrefab, content);
            slots.Add(go.GetComponent<InventorySlot>());
        }
        //绑定数据
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < items.Count)
            {
                slots[i].gameObject.SetActive(true);
                slots[i].Bind(items[i], i);
                slots[i].SetSelected(i == selectedIndex);
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
        }
    }

    //选中格子
    public void SelectSlot(int index)
    {
        selectedIndex = index;
        Refresh();
        //更新详情面板
        if (index >= 0 && index < InventoryManager.Instance.items.Count)
        {
            var item = InventoryManager.Instance.items[index];
            detailNameText.text = item.itemName;
            detailDescText.text = item.description;
            detailIcon.sprite = item.icon;
            detailIcon.enabled = true;
        }
        else
        {
            detailNameText.text = "";
            detailDescText.text = "";
            detailIcon.enabled = false;
        }
    }
    //拖拽图标
    public void ShowDragIcon(Sprite icon, Vector2 pos)
    {
        dragIcon.sprite = icon;
        dragIcon.gameObject.SetActive(true);
        dragIcon.transform.position = pos;
    }

    public void UpdateDragIcon(Vector2 pos)
    {
        dragIcon.transform.position = pos;
    }

    public void HideDragIcon()
    {
        dragIcon.gameObject.SetActive(false);
    }
}