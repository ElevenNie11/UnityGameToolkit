using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//此脚本用来定义物品信息
//物品信息枚举
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
    public string itemName;   //物品名称
    public ItemType itemType; //物品类型（武器/食物 -> 枚举选择）
    public Sprite icon;       //物品图标：背包里显示的图片
    public int maxStack = 1;  //堆叠上限：武器一般不能堆叠，食物可以堆叠
}
