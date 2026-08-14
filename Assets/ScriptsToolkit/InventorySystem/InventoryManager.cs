using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    // 唯一数据源
    public List<InventoryItemData> items = new List<InventoryItemData>();

    void Awake()
    {
        Instance = this;
    }

    // 添加道具
    public void AddItem(InventoryItemData item)
    {
        items.Add(item);
        InventoryUI.Instance.Refresh();
    }

    // 移除道具
    public void RemoveItem(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            items.RemoveAt(index);
            InventoryUI.Instance.Refresh();
        }
    }

    // 插入排序：把 from 位置的道具插入到 target 前面
    public void MoveItem(int from, int target)
    {
        if (from == target) return;

        InventoryItemData item = items[from];
        items.RemoveAt(from);

        // 关键：如果 from 在 target 前面，删除后 target 索引会前移一格
        int insertIndex = from < target ? target - 1 : target;
        items.Insert(insertIndex, item);

        InventoryUI.Instance.Refresh();
    }
}