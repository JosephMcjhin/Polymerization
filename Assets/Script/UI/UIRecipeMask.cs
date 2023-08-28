using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIRecipeMask : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public GameObject recipe_item;
    bool in_ui;

    void Start(){
        CraftManager.instance.recipe_action += GenRecipe;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        in_ui = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        in_ui = false;
    }

    void GenRecipe(string s, Item res){
        GameObject temp1 = Instantiate(recipe_item,transform.position,Quaternion.identity);
        temp1.transform.SetParent(transform);
        temp1.transform.localScale = new Vector3(1,1,1);
        temp1.GetComponent<UIRecipe>().GenRecipe(s, res);
    }

    void Update(){
        if(CameraManager.instance.Onmaincamera)return;
        if (in_ui){
            transform.GetComponent<GridLayoutGroup>().padding.top += (int)(Input.GetAxis("Mouse ScrollWheel") * 50);
            transform.GetComponent<GridLayoutGroup>().padding.top = Mathf.Clamp(transform.GetComponent<GridLayoutGroup>().padding.top, -500, 8);
            transform.GetComponent<GridLayoutGroup>().SetLayoutVertical();  //更新立即生效
        }
    }
}
