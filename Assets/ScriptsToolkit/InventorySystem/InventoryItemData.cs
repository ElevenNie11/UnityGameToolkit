using UnityEngine;
public enum ItemCategory
{
    Food,   //食物
    Weapon  //武器
}
[System.Serializable]
public class InventoryItemData
{
    public int itemId;          //道具ID
    public string itemName;     //道具名
    public Sprite icon;         //图标
    public int count;           //数量
    public string description;  //道具详情描述
    public int healPercent = 10;     //回血百分比（0~100）默认10%
    public int price = 0;        //商品价格
    public ItemCategory category; //商品分类：食物和武器
}