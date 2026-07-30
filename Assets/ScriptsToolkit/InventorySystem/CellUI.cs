using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CellUI : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text countText;
    //刷新格子显示
    public void RefreshCell(ItemData item, int count)
    {
        if(item == null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
            countText.text = "";
        }
        else
        {
            iconImage.enabled = true;
            iconImage.sprite = item.icon;
            //堆叠数为1时不显示文字，大于1才显示
            if(count > 1)
            {
                //...
            }
        }
    }
}
