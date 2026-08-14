using System.Collections;
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

    [System.Serializable]
    public class SceneItem
    {
        public GameObject sceneObject;  // 场景中可以点击拾取的物体
        public ItemData itemData;       // 点击后存入背包的物品数据
        public int count = 1;           // 拾取数量
        public bool destroyAfterPick = true;
    }

    public static InventoryManager Instance { get; private set; }

    [Header("背包UI父物体")]
    public Transform contentWeapon;
    public Transform contentFood;
    public GameObject cellUIPrefab;

    [Header("背包容量")]
    public int weaponSlotCount = 15;
    public int foodSlotCount = 15;

    [Header("场景可拾取物品")]
    public Camera clickCamera;
    public List<SceneItem> sceneItems = new List<SceneItem>();

    // 背包数据容器
    private List<InventorySlot> weaponSlots = new List<InventorySlot>();
    private List<InventorySlot> foodSlots = new List<InventorySlot>();

    private void Awake()
    {
        Instance = this;

        if (clickCamera == null)
        {
            clickCamera = Camera.main;
        }

        InitSlots();
        CreateCells(contentWeapon, weaponSlotCount);
        CreateCells(contentFood, foodSlotCount);
        RefreshAllUI();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckClickSceneItem();
        }
    }

    // 初始化空格子数据
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

    // 如果Content下面没有足够格子，就自动生成一些格子
    private void CreateCells(Transform content, int count)
    {
        if (content == null || cellUIPrefab == null)
        {
            return;
        }

        while (content.childCount < count)
        {
            Instantiate(cellUIPrefab, content);
        }
    }

    // 点击场景中的素材，拾取进背包
    private void CheckClickSceneItem()
    {
        GameObject clickObject = GetClickedObject();
        if (clickObject == null)
        {
            return;
        }

        for (int i = 0; i < sceneItems.Count; i++)
        {
            SceneItem sceneItem = sceneItems[i];
            if (sceneItem.sceneObject != clickObject)
            {
                continue;
            }

            bool success = AddItem(sceneItem.itemData, sceneItem.count);
            if (success)
            {
                PickSceneItem(sceneItem);
                sceneItems.RemoveAt(i);
            }

            return;
        }
    }

    // 获取鼠标当前点击到的场景物体，需要物体身上有 Collider2D 或 Collider
    private GameObject GetClickedObject()
    {
        if (clickCamera == null)
        {
            return null;
        }

        Vector3 mouseWorldPos = clickCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
        Collider2D hit2D = Physics2D.OverlapPoint(mousePos2D);
        if (hit2D != null)
        {
            return hit2D.gameObject;
        }

        Ray ray = clickCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit3D))
        {
            return hit3D.collider.gameObject;
        }

        return null;
    }

    private void PickSceneItem(SceneItem sceneItem)
    {
        if (sceneItem.sceneObject == null)
        {
            return;
        }

        if (sceneItem.destroyAfterPick)
        {
            Destroy(sceneItem.sceneObject);
        }
        else
        {
            sceneItem.sceneObject.SetActive(false);
        }
    }

    // 添加物品到背包
    public bool AddItem(ItemData item, int count = 1)
    {
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
                    RefreshAllUI();
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
                    RefreshAllUI();
                    return true;
                }
            }
        }

        RefreshAllUI();
        Debug.Log("背包已满，无法完全放入物品：" + item.itemName);
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

    // 刷新两套UI
    private void RefreshAllUI()
    {
        RefreshUI(contentWeapon, weaponSlots);
        RefreshUI(contentFood, foodSlots);
    }

    // 刷新某一类背包UI
    private void RefreshUI(Transform content, List<InventorySlot> slots)
    {
        if (content == null)
        {
            return;
        }

        for (int i = 0; i < content.childCount; i++)
        {
            CellUI cell = content.GetChild(i).GetComponent<CellUI>();
            if (cell == null)
            {
                continue;
            }

            if (i < slots.Count)
            {
                cell.RefreshCell(slots[i].item, slots[i].count);
            }
            else
            {
                cell.RefreshCell(null, 0);
            }
        }
    }
}