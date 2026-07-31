using UnityEngine;
//此脚本的功用：物品配置，支持武器/食物分类、图标、最大堆叠数
//物品类型：用于决定物品显示在哪一个背包分类里
public enum ItemType
{
    Weapon,
    Food
}

// CreateAssetMenu 特性：这是 ScriptableObject 专属特性：作用是在Unity编辑器右键菜单生成配置文件
//1. fileName = "NewItem"：新建物品资源默认文件名
//2. menuName = "Inventory/Item"：右键路径
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
// ScriptableObject 适合存放静态数据：物品属性、技能数据、怪物属性；
public class ItemData : ScriptableObject
{
    [Header("基础信息")]
    public string itemName;
    public ItemType itemType;
    public Sprite icon;

    [Header("堆叠设置")]
    public int maxStack = 1;
}
