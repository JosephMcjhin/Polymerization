using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
//棋子图鉴更新
public class UIItem : MonoBehaviour
{
    public Image Img;

    //public TextMeshProUGUI Text;

    public Item item;

    void Awake()
    {
        Img.color = new Color(0,0,0,1);
        BasicData.Instance.OnItemCreated += UpdateUI;
    }

    // public void OnPointerClick(PointerEventData pointerEventData){
    //     Text.enabled = !Text.enabled;
    // }

    void UpdateUI(Item x)
    {
        if(x.ItemID == item.ItemID)
        {
            Img.color = new Color(1,1,1,1);
            BasicData.Instance.OnItemCreated -= UpdateUI;
        }
    }
}
