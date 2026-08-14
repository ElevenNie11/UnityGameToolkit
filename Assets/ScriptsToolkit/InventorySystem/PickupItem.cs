using UnityEngine;
using UnityEngine.EventSystems;

public class PickupItem : MonoBehaviour, IPointerClickHandler
{
    [Header("拾取物品")]
    public ItemData itemData;
    public int count = 1;
    public bool destroyAfterPick = true;

    private bool pickedUp;

    // 点击UI图片时调用
    public void OnPointerClick(PointerEventData eventData)
    {
        PickUp();
    }

    // 点击带Collider的场景物体时调用
    private void OnMouseDown()
    {
        PickUp();
    }

    private void PickUp()
    {
        if (pickedUp)
        {
            return;
        }

        InventoryManager manager = GetInventoryManager();
        if (manager == null)
        {
            Debug.Log("场景中没有找到 InventoryManager");
            return;
        }

        bool success = manager.AddItem(itemData, count);
        if (!success)
        {
            return;
        }

        pickedUp = true;

        if (destroyAfterPick)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private InventoryManager GetInventoryManager()
    {
        if (InventoryManager.Instance != null)
        {
            return InventoryManager.Instance;
        }

        InventoryManager manager = FindObjectOfType<InventoryManager>(true);
        if (manager != null)
        {
            manager.InitInventory();
        }

        return manager;
    }
}
