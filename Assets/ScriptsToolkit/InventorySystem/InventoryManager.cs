using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    //唯一数据源
    public List<InventoryItemData> items = new List<InventoryItemData>();

    void Awake()
    {
        Instance = this;
    }

    //添加道具
    public void AddItem(InventoryItemData item)
    {
        items.Add(item);
        InventoryUI.Instance.Refresh();
    }

    //移除道具
    public void RemoveItem(int index)
    {
        if (index < 0 || index >= items.Count) return;
        items.RemoveAt(index);
        //通知 UI 修正选中索引并刷新
        InventoryUI.Instance.OnItemRemoved(index);
    }

    //插入排序：把 from 位置的道具插入到 target 前面
    public void MoveItem(int from, int target)
    {
        if (from == target) return;
        InventoryItemData item = items[from];
        items.RemoveAt(from);

        //关键：如果 from 在 target 前面，删除后 target 索引会前移一格
        int insertIndex = from < target ? target - 1 : target;
        items.Insert(insertIndex, item);

        int sel = InventoryUI.Instance.selectedIndex;
        if (sel == from)
        {
            //选中的就是被拖动的道具 → 跟到新位置
            InventoryUI.Instance.selectedIndex = insertIndex;
        }
        else if (from < sel && sel <= target)
        {
            //选中的道具在被移区间内 → 整体前移一格
            InventoryUI.Instance.selectedIndex = sel - 1;
        }
        else if (target <= sel && sel < from)
        {
            //选中的道具在被移区间内 → 整体后移一格
            InventoryUI.Instance.selectedIndex = sel + 1;
        }
        InventoryUI.Instance.Refresh();
    }

    //使用道具：点击“使用”按钮
    public void ConsumeItem(int index)
    {
        if(index < 0 || index >= items.Count) return;

        //先拿到该食物的回血量百分比:
        //注意要在RemoveItem之前读取healPercent，因为移除后索引会变
        int heal = items[index].healPercent;
        items[index].count--;
        if(items[index].count <= 0)
        {
            RemoveItem(index);
        }
        else
        {
            InventoryUI.Instance.Refresh();
        }
        //吃食物回血10%，交给血量系统处理
        HealthSystem.Instance.Heal(heal);
    }
}