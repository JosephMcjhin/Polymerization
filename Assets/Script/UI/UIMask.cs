using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIMask : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public GameObject uiitem;
    bool in_ui;
    void Start(){
        for(int i=1; i<CraftManager.instance.item_list.Count; i++){
            GameObject temp = Instantiate(uiitem, transform.position, Quaternion.identity);
            temp.transform.SetParent(transform);
            UIItem temp1 = temp.GetComponent<UIItem>();
            temp1.img.sprite = CraftManager.instance.item_list[i].ItemImage;
            temp1.item = CraftManager.instance.item_list[i];
            temp.transform.localScale = new Vector3(1,1,1);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        in_ui = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        in_ui = false;
    }

    private void Update(){
        if(CameraManager.instance.Onmaincamera)return;
        if(in_ui){
            transform.GetComponent<GridLayoutGroup>().padding.top += (int)(Input.GetAxis("Mouse ScrollWheel") * 50);
            transform.GetComponent<GridLayoutGroup>().padding.top = Mathf.Clamp(transform.GetComponent<GridLayoutGroup>().padding.top, -500, 8);
            transform.GetComponent<GridLayoutGroup>().SetLayoutVertical();  //更新立即生效
        }
    }
}
