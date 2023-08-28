using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIItem : MonoBehaviour,IPointerClickHandler
{
    public Image img;

    public TextMeshProUGUI text;

    public Item item;

    void Start(){
        text.enabled = false;
        img.color = new Color(0,0,0,1);
        CraftManager.instance.ui_action += UpdateUI;
    }

    public void OnPointerClick(PointerEventData pointerEventData){
        text.enabled = !text.enabled;
    }

    void UpdateUI(Item x){
        if(x.ItemID == item.ItemID){
            img.color = new Color(1,1,1,1);
        }
    }
}
