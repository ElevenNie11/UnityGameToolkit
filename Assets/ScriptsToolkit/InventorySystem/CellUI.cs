using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellUI : MonoBehaviour
{
    [Header("UI组件")]
    public Image iconImage;
    public TMP_Text countText;

    public ItemData CurrentItem { get; private set; }
    public int CurrentCount { get; private set; }

    private void Awake()
    {
        AutoBind();
        RefreshCell(null, 0);
    }

    private void OnValidate()
    {
        AutoBind();
    }

    // 刷新格子显示。
    public void RefreshCell(ItemData item, int count)
    {
        CurrentItem = item;
        CurrentCount = Mathf.Max(0, count);

        if (item == null || CurrentCount <= 0)
        {
            if (iconImage != null)
            {
                iconImage.sprite = null;
                iconImage.enabled = false;
            }

            if (countText != null)
            {
                countText.text = string.Empty;
            }

            return;
        }

        if (iconImage != null)
        {
            iconImage.enabled = true;
            iconImage.sprite = item.icon;
            iconImage.preserveAspect = true;
        }

        if (countText != null)
        {
            countText.text = CurrentCount > 1 ? CurrentCount.ToString() : string.Empty;
        }
    }

    public void Clear()
    {
        RefreshCell(null, 0);
    }

    private void AutoBind()
    {
        if (iconImage == null)
        {
            Transform icon = transform.Find("iconImage");
            iconImage = icon != null ? icon.GetComponent<Image>() : GetFirstChildImage();
        }

        if (countText == null)
        {
            countText = GetComponentInChildren<TMP_Text>(true);
        }
    }

    private Image GetFirstChildImage()
    {
        Image[] images = GetComponentsInChildren<Image>(true);
        foreach (Image image in images)
        {
            if (image.gameObject != gameObject)
            {
                return image;
            }
        }

        return null;
    }
}
