using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//合成公式图鉴
public class UIRecipeMask : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public GameObject recipeItem;
    bool mouseOnUI;

    void Awake()
    {
        BasicData.Instance.OnRecipeChanged += GenRecipe;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOnUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOnUI = false;
    }

    void GenRecipe(string s, Item res)
    {
        GameObject temp1 = Instantiate(recipeItem,transform.position,Quaternion.identity);
        temp1.transform.SetParent(transform);
        temp1.transform.localScale = new Vector3(1,1,1);
        temp1.GetComponent<UIRecipe>().GenRecipe(s, res);
    }

    void Update()
    {
        if(CameraManager.Instance.OnMainCamera)return;
        if (mouseOnUI)
        {
            transform.GetComponent<GridLayoutGroup>().padding.top += (int)(Input.GetAxis("Mouse ScrollWheel") * 50);
            transform.GetComponent<GridLayoutGroup>().padding.top = Mathf.Clamp(transform.GetComponent<GridLayoutGroup>().padding.top, -500, 8);
            transform.GetComponent<GridLayoutGroup>().SetLayoutVertical();  //更新立即生效
        }
    }
}
