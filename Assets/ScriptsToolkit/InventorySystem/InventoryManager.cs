using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Serializable]
    public class InventorySlot
    {
        public ItemData item;
        public int count;

        public bool IsEmpty => item == null || count <= 0;

        public void Clear()
        {
            item = null;
            count = 0;
        }
    }

    public static InventoryManager Instance { get; private set; }

    [Header("分类界面")]
    public GameObject weaponPanel;
    public GameObject foodPanel;
    public Button weaponButton;
    public Button foodButton;
    public Button closeButton;

    [Header("格子父物体")]
    [FormerlySerializedAs("contentWeapon")]
    public Transform weaponContent;
    [FormerlySerializedAs("contentFood")]
    public Transform foodContent;
    public CellUI cellPrefab;
    [FormerlySerializedAs("cellUIPrefab")]
    public GameObject cellUIPrefab;

    [Header("背包容量")]
    [Min(1)]
    [FormerlySerializedAs("weaponSlotCount")]
    public int weaponCapacity = 15;
    [Min(1)]
    [FormerlySerializedAs("foodSlotCount")]
    public int foodCapacity = 15;

    [Header("测试数据")]
    public List<ItemData> startItems = new List<ItemData>();
    public ItemData testAddItem;
    [Min(1)]
    public int testAddCount = 1;

    private readonly List<InventorySlot> weaponSlots = new List<InventorySlot>();
    private readonly List<InventorySlot> foodSlots = new List<InventorySlot>();
    private readonly List<CellUI> weaponCells = new List<CellUI>();
    private readonly List<CellUI> foodCells = new List<CellUI>();

    private ItemType currentType = ItemType.Weapon;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        AutoBindFromScene();
        PrepareSlots();
        PrepareCells();
        BindButtons();
    }

    private void Start()
    {
        foreach (ItemData item in startItems)
        {
            AddItem(item, 1);
        }

        ShowWeapon();
        RefreshAll();
    }

    public bool AddItem(ItemData item)
    {
        return AddItem(item, 1);
    }

    public bool AddItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0)
        {
            return false;
        }

        List<InventorySlot> slots = GetSlots(item.itemType);
        int remaining = amount;

        remaining = AddToSameItemSlots(slots, item, remaining);
        remaining = AddToEmptySlots(slots, item, remaining);

        RefreshByType(item.itemType);
        return remaining == 0;
    }

    public bool RemoveItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0)
        {
            return false;
        }

        List<InventorySlot> slots = GetSlots(item.itemType);
        if (GetItemCount(item) < amount)
        {
            return false;
        }

        int remaining = amount;
        for (int i = slots.Count - 1; i >= 0 && remaining > 0; i--)
        {
            InventorySlot slot = slots[i];
            if (slot.item != item)
            {
                continue;
            }

            int removeCount = Mathf.Min(slot.count, remaining);
            slot.count -= removeCount;
            remaining -= removeCount;

            if (slot.count <= 0)
            {
                slot.Clear();
            }
        }

        RefreshByType(item.itemType);
        return true;
    }

    public int GetItemCount(ItemData item)
    {
        if (item == null)
        {
            return 0;
        }

        int total = 0;
        foreach (InventorySlot slot in GetSlots(item.itemType))
        {
            if (slot.item == item)
            {
                total += slot.count;
            }
        }

        return total;
    }

    public void ShowWeapon()
    {
        currentType = ItemType.Weapon;
        SetCategoryVisible(ItemType.Weapon);
    }

    public void ShowFood()
    {
        currentType = ItemType.Food;
        SetCategoryVisible(ItemType.Food);
    }

    public void ToggleInventory()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void OpenInventory()
    {
        gameObject.SetActive(true);
        SetCategoryVisible(currentType);
        RefreshAll();
    }

    public void CloseInventory()
    {
        gameObject.SetActive(false);
    }

    // 可以把测试按钮的 OnClick 绑到这里。
    public void AddTestItem()
    {
        AddItem(testAddItem, testAddCount);
    }

    public void RefreshAll()
    {
        RefreshWeapon();
        RefreshFood();
    }

    private int AddToSameItemSlots(List<InventorySlot> slots, ItemData item, int amount)
    {
        int maxStack = Mathf.Max(1, item.maxStack);
        for (int i = 0; i < slots.Count && amount > 0; i++)
        {
            InventorySlot slot = slots[i];
            if (slot.item != item || slot.count >= maxStack)
            {
                continue;
            }

            int addCount = Mathf.Min(maxStack - slot.count, amount);
            slot.count += addCount;
            amount -= addCount;
        }

        return amount;
    }

    private int AddToEmptySlots(List<InventorySlot> slots, ItemData item, int amount)
    {
        int maxStack = Mathf.Max(1, item.maxStack);
        for (int i = 0; i < slots.Count && amount > 0; i++)
        {
            InventorySlot slot = slots[i];
            if (!slot.IsEmpty)
            {
                continue;
            }

            int addCount = Mathf.Min(maxStack, amount);
            slot.item = item;
            slot.count = addCount;
            amount -= addCount;
        }

        return amount;
    }

    private void RefreshByType(ItemType type)
    {
        if (type == ItemType.Weapon)
        {
            RefreshWeapon();
        }
        else
        {
            RefreshFood();
        }
    }

    private void RefreshWeapon()
    {
        RefreshCells(weaponSlots, weaponCells);
    }

    private void RefreshFood()
    {
        RefreshCells(foodSlots, foodCells);
    }

    private void RefreshCells(List<InventorySlot> slots, List<CellUI> cells)
    {
        for (int i = 0; i < cells.Count; i++)
        {
            if (i < slots.Count && !slots[i].IsEmpty)
            {
                cells[i].RefreshCell(slots[i].item, slots[i].count);
            }
            else
            {
                cells[i].Clear();
            }
        }
    }

    private void SetCategoryVisible(ItemType type)
    {
        if (weaponPanel != null)
        {
            weaponPanel.SetActive(type == ItemType.Weapon);
        }

        if (foodPanel != null)
        {
            foodPanel.SetActive(type == ItemType.Food);
        }
    }

    private List<InventorySlot> GetSlots(ItemType type)
    {
        return type == ItemType.Weapon ? weaponSlots : foodSlots;
    }

    private void PrepareSlots()
    {
        EnsureSlotCount(weaponSlots, weaponCapacity);
        EnsureSlotCount(foodSlots, foodCapacity);
    }

    private void EnsureSlotCount(List<InventorySlot> slots, int capacity)
    {
        while (slots.Count < capacity)
        {
            slots.Add(new InventorySlot());
        }

        while (slots.Count > capacity)
        {
            slots.RemoveAt(slots.Count - 1);
        }
    }

    private void PrepareCells()
    {
        CollectCells(weaponContent, weaponCells, weaponCapacity);
        CollectCells(foodContent, foodCells, foodCapacity);
    }

    private void CollectCells(Transform content, List<CellUI> cells, int capacity)
    {
        cells.Clear();
        if (content == null)
        {
            return;
        }

        for (int i = 0; i < content.childCount; i++)
        {
            CellUI cell = content.GetChild(i).GetComponent<CellUI>();
            if (cell != null)
            {
                cells.Add(cell);
            }
        }

        if (cellPrefab == null)
        {
            cellPrefab = cellUIPrefab != null ? cellUIPrefab.GetComponent<CellUI>() : null;
        }

        if (cellPrefab == null)
        {
            return;
        }

        while (cells.Count < capacity)
        {
            CellUI cell = Instantiate(cellPrefab, content);
            cell.name = $"Cell_UI ({cells.Count})";
            cells.Add(cell);
        }
    }

    private void BindButtons()
    {
        if (weaponButton != null)
        {
            weaponButton.onClick.RemoveListener(ShowWeapon);
            weaponButton.onClick.AddListener(ShowWeapon);
        }

        if (foodButton != null)
        {
            foodButton.onClick.RemoveListener(ShowFood);
            foodButton.onClick.AddListener(ShowFood);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(CloseInventory);
            closeButton.onClick.AddListener(CloseInventory);
        }
    }

    private void AutoBindFromScene()
    {
        if (weaponContent == null)
        {
            weaponContent = FindChildByName(transform.root, "Content_Weapon");
        }

        if (foodContent == null)
        {
            foodContent = FindChildByName(transform.root, "Content_Food");
        }

        if (weaponPanel == null)
        {
            Transform weaponScroll = FindChildByName(transform.root, "Scroll View_Weapon");
            weaponPanel = weaponScroll != null ? weaponScroll.gameObject : null;
        }

        if (foodPanel == null)
        {
            Transform foodScroll = FindChildByName(transform.root, "Scroll View_Food");
            foodPanel = foodScroll != null ? foodScroll.gameObject : null;
        }
    }

    private Transform FindChildByName(Transform root, string childName)
    {
        if (root == null)
        {
            return null;
        }

        if (root.name == childName)
        {
            return root;
        }

        for (int i = 0; i < root.childCount; i++)
        {
            Transform result = FindChildByName(root.GetChild(i), childName);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
}
