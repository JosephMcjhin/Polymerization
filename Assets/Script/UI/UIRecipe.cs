using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//合成公式对象
public class UIRecipe : MonoBehaviour
{
    public GameObject Img;
    public Sprite Arrow;
    void GenImg(Sprite ii)
    {
        GameObject temp1 = Instantiate(Img,transform.position,Quaternion.identity);
        temp1.GetComponent<Image>().sprite = ii;
        temp1.transform.SetParent(transform);
        temp1.transform.localScale = new Vector3(1,1,1);
    }
    public void GenRecipe(string s, Item res)
    {
        for(int i=0; i<s.Length/4; i++)
        {
            //print(i);
            Item temp = BasicData.Instance.DicID2Item[s.Substring(i*4, 4)];
            GenImg(temp.ItemImage);
        }

        GenImg(Arrow);
        GenImg(res.ItemImage);
    }
}
