using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [System.Serializable]
    public class InventorySlot
    {
        public ItemData item;
        public int count;
    }

    public static InventoryManager Instance { get; private set; }

    [Header("背包UI父物体")]
    public Transform contentWeapon;
    public Transform contentFood;
    public GameObject cellUIPrefab;

    [Header("背包容量")]
    public int weaponSlotCount = 15;
    public int foodSlotCount = 15;

    private List<InventorySlot> weaponSlots = new List<InventorySlot>();
    private List<InventorySlot> foodSlots = new List<InventorySlot>();
    private bool initialized;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        Instance = this;
    }

    private void Start()
    {
        InitInventory();
    }

    public void InitInventory()
    {
        if (initialized)
        {
            return;
        }

        InitSlots();
        CreateCells(contentWeapon, weaponSlotCount);
        CreateCells(contentFood, foodSlotCount);
        RefreshAllUI();
        initialized = true;
    }

    // 初始化背包空格子
    private void InitSlots()
    {
        weaponSlots.Clear();
        foodSlots.Clear();

        for (int i = 0; i < weaponSlotCount; i++)
        {
            weaponSlots.Add(new InventorySlot());
        }

        for (int i = 0; i < foodSlotCount; i++)
        {
            foodSlots.Add(new InventorySlot());
        }
    }

    // 如果Content下面格子不够，就自动生成格子
    private void CreateCells(Transform content, int slotCount)
    {
        if (content == null || cellUIPrefab == null)
        {
            return;
        }

        while (content.childCount < slotCount)
        {
            Instantiate(cellUIPrefab, content);
        }
    }

    // 添加物品到背包
    public bool AddItem(ItemData item, int count = 1)
    {
        InitInventory();

        if (item == null || count <= 0)
        {
            return false;
        }

        List<InventorySlot> slots = GetSlots(item.itemType);

        // 先找相同物品，能堆叠就加数量
        for (int i = 0; i < slots.Count; i++)
        {
            InventorySlot slot = slots[i];
            if (slot.item == item && slot.count < item.maxStack)
            {
                int canAdd = item.maxStack - slot.count;
                int addCount = Mathf.Min(canAdd, count);
                slot.count += addCount;
                count -= addCount;

                if (count <= 0)
                {
                    RefreshUIByType(item.itemType);
                    Debug.Log("物品已堆叠到背包：" + item.itemName + "，格子：" + i);
                    return true;
                }
            }
        }

        // 再找空格子
        for (int i = 0; i < slots.Count; i++)
        {
            InventorySlot slot = slots[i];
            if (slot.item == null)
            {
                int addCount = Mathf.Min(item.maxStack, count);
                slot.item = item;
                slot.count = addCount;
                count -= addCount;

                if (count <= 0)
                {
                    RefreshUIByType(item.itemType);
                    Debug.Log("物品已放入背包：" + item.itemName + "，类型：" + item.itemType + "，格子：" + i);
                    return true;
                }
            }
        }

        RefreshAllUI();
        Debug.Log("背包已满，无法放入物品：" + item.itemName);
        return false;
    }

    private List<InventorySlot> GetSlots(ItemType itemType)
    {
        if (itemType == ItemType.Weapon)
        {
            return weaponSlots;
        }

        return foodSlots;
    }

    private void RefreshUIByType(ItemType itemType)
    {
        if (itemType == ItemType.Weapon)
        {
            RefreshUI(contentWeapon, weaponSlots);
        }
        else
        {
            RefreshUI(contentFood, foodSlots);
        }
    }

    private void RefreshAllUI()
    {
        RefreshUI(contentWeapon, weaponSlots);
        RefreshUI(contentFood, foodSlots);
    }

    private void RefreshUI(Transform content, List<InventorySlot> slots)
    {
        if (content == null)
        {
            return;
        }

        for (int i = 0; i < content.childCount; i++)
        {
            CellUI cell = content.GetChild(i).GetComponentInChildren<CellUI>(true);
            if (cell == null)
            {
                continue;
            }

            if (i < slots.Count)
            {
                cell.RefreshCell(slots[i].item, slots[i].count);

                if (slots[i].item != null)
                {
                    Debug.Log("刷新格子：" + content.name + " / " + cell.name + " / " + slots[i].item.itemName);
                }
            }
            else
            {
                cell.RefreshCell(null, 0);
            }
        }
    }
}
