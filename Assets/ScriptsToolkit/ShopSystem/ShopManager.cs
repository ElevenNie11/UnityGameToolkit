using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//管理金币和商品数据
public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;
    [Header("金币")]
    public int coins = 100; //初始金币数量100
    [Header("商品列表")]
    public List<InventoryItemData> foodItems = new List<InventoryItemData>(); //食物商品列表
    public List<InventoryItemData> weaponItems = new List<InventoryItemData>(); //武器商品列表
    void Awake()
    {
        Instance = this;
    }

    //获取当前分类的商品列表
    public List<InventoryItemData> GetItemsByCategory(ItemCategory category)
    {
      return category == ItemCategory.Food ? foodItems : weaponItems;   
    }

    //购买商品
    public bool BuyItem(InventoryItemData item)
    {
        if (coins < item.price)
        {
            Debug.Log("金币不足，无法购买: " + item.itemName);
            return false;
        }
        else
        {
            coins -= item.price; //购买成功：扣除金币
            //购买成功以后加入背包,背包里有了新的newItem,把商品数据一一赋值给它
            InventoryItemData newItem = new InventoryItemData
            {
                itemId = item.itemId,
                itemName = item.itemName,
                icon = item.icon,
                count = 1,                     //购买时数量为1
                description = item.description,
                healPercent = item.healPercent,
                price = item.price,
                category = item.category
            };
            InventoryManager.Instance.AddItem(newItem); //加入背包
            //更新商城UI的金币显示
            ShopUI.Instance.UpdateCoinsDisplay();
            return true;
        }
    }
}
