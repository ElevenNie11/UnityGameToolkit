using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
   [Header("背包UI父物体")]
   public Transform contentWeapon;
   public Transform contentFood;
   public GameObject cellUIPrefab;  //Cell_UI预制体
   [Header("背包容量")]
   public int weaponSlotCount = 15;
   public int foodSlotCount = 15;
   //背包槽位数据结构
   [System.Serializable]
   public class InventorySlot
    {
        public ItemData item;
        public int count;
    }
   //背包数据容器
   private List<InventorySlot> weaponSlot = new List<InventorySlot>();
   private List<InventorySlot> foodSlot = new List<InventorySlot>();

   //单例：拾取物体可以直接调用 InventoryManager.Instance
   public static InventoryManager Instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            InitSlots();
        }
    }
    //初始化空格子
    private void InitSlots()
    {
        //初始化武器栏
        for(int i = 0; i < weaponSlotCount; i++)
        {
            weaponSlot.Add(new InventorySlot());
        }
        //初始化食物栏
        for(int i = 0; i < foodSlotCount; i++)
        {
            foodSlot.Add(new InventorySlot());
        }
        RefreshAllUI();
    }
    //刷新两套UI
    void RefreshAllUI()
    {
        RefreshUI(contentWeapon, weaponSlot);
        RefreshUI(contentFood, foodSlot);
    }
    //刷新UI
    void RefreshUI(Transform content, List<InventorySlot> slots)
    {
        
    }
}
