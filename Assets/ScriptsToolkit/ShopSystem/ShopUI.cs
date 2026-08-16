using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml.Serialization;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;
    void Awake()
    {
        Instance = this;
    }

    [Header("分类按钮")]
    public Button foodTabButton;
    public Button weaponTabButton;
    [Header("商品列表")]
    public Transform itemContent;    //商品格子的父物体
    public GameObject itemSlotPrefab;//商品格子预制体
    [Header("详情面板")]
    public Image detailIcon;
    public TextMeshProUGUI detailName;
    public TextMeshProUGUI detailDesc;
    [Header("购买")]
    public TextMeshProUGUI coinsText;
    public Button buyButton;

    private List<ShopItemSlot> slots = new List<ShopItemSlot>();
    private ItemCategory currentCategory = ItemCategory.Food;
    private InventoryItemData selectedItem;

    //如果要用代码来绑定按钮的话，就必须在Start()函数里写逻辑
    void Start()
    {
        //初始没有选中商品，购买按钮不可点击
        buyButton.interactable = false;
        //分类按钮绑定：
        foodTabButton.onClick.AddListener(() => SwitchCategory(ItemCategory.Food));
        weaponTabButton.onClick.AddListener(() => SwitchCategory(ItemCategory.Weapon));
        //购买按钮绑定在Unity编辑器中实现
        //buyButton.onClick.AddListener(OnBuyClicked);

        //默认显示食物分类
        SwitchCategory(ItemCategory.Food);
        UpdateCoinsDisplay();
    }

    //切换分类
    public void SwitchCategory(ItemCategory category)
    {
        currentCategory = category;
        selectedItem = null;
        buyButton.interactable = false;
        //清空详情面板
        ClearDetail();
        //刷新UI
        RefreshItemList();
    } 

    //清空详情面板
    void ClearDetail()
    {
        detailIcon.enabled = false;
        detailName.text = "";
        detailDesc.text = "";
    }

    //显示金币UI更新
    public void UpdateCoinsDisplay()
    {
        coinsText.text = "金币: " + ShopManager.Instance.coins;
    }

    //刷新商品列表
    public void RefreshItemList()
    {
        var items = ShopManager.Instance.GetItemsByCategory(currentCategory);
        //生成格子
        while (slots.Count < items.Count)   
        {
            GameObject go = Instantiate(itemSlotPrefab, itemContent);
            slots.Add(go.GetComponent<ShopItemSlot>());
        }
        //绑定数据
        for (int i = 0; i < slots.Count; i++)
        {
            if(i < items.Count)
            {
                slots[i].gameObject.SetActive(true);
                slots[i].Bind(items[i]);
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
        }
    }

    //选中商品: 详情面板刷新
    public void SelectItem(InventoryItemData item)
    {
        selectedItem = item;
        detailIcon.sprite = item.icon;
        detailIcon.enabled = true;
        detailName.text = item.itemName;
        detailDesc.text = item.description;
        buyButton.interactable = true;
    }

    //点击购买
    public void OnBuyClicked()
    {
        if(selectedItem == null) return;
        bool success = ShopManager.Instance.BuyItem(selectedItem);
        if (success)
        {
            Debug.Log("购买成功：" + selectedItem.itemName);
        }
    }
}