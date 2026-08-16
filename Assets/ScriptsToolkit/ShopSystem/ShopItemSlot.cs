using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemSlot : MonoBehaviour
{
    [Header("子节点引用")]
    public Image iconImage;
    public TextMeshProUGUI priceText;
    [HideInInspector] public InventoryItemData itemData;

    //绑定商品数据
    public void Bind(InventoryItemData data)
    {
        itemData = data;
        iconImage.sprite = data.icon;
        priceText.text = data.price + "铜币";
    }

    //点击格子（在编辑器里给预制体根节点的 Button 绑定这个方法）
    public void OnClicked()
    {
        ShopUI.Instance.SelectItem(itemData);
    }
}
