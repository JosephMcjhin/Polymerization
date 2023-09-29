using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//棋子图鉴滚动条
public class UIMask : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public GameObject UIItem;
    bool mouseOnUI;
    void Awake()
    {
        for(int i=1; i<BasicData.Instance.ItemList.Count; i++)
        {
            GameObject temp = Instantiate(UIItem, transform.position, Quaternion.identity);
            temp.transform.SetParent(transform);
            UIItem temp1 = temp.GetComponent<UIItem>();
            temp1.Img.sprite = BasicData.Instance.ItemList[i].ItemImage;
            temp1.item = BasicData.Instance.ItemList[i];
            temp.transform.localScale = new Vector3(1,1,1);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOnUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOnUI = false;
    }

    private void Update()
    {
        if(CameraManager.Instance.OnMainCamera) return;
        if(mouseOnUI)
        {
            transform.GetComponent<GridLayoutGroup>().padding.top += (int)(Input.GetAxis("Mouse ScrollWheel") * 50);
            transform.GetComponent<GridLayoutGroup>().padding.top = Mathf.Clamp(transform.GetComponent<GridLayoutGroup>().padding.top, -500, 8);
            transform.GetComponent<GridLayoutGroup>().SetLayoutVertical();  //更新立即生效
        }
    }
}
