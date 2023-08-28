using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRecipe : MonoBehaviour
{
    public GameObject img;
    public Sprite arrow;
    void GenImg(Sprite ii){
        GameObject temp1 = Instantiate(img,transform.position,Quaternion.identity);
        temp1.GetComponent<Image>().sprite = ii;
        temp1.transform.SetParent(transform);
        temp1.transform.localScale = new Vector3(1,1,1);
    }
    public void GenRecipe(string s, Item res){
        for(int i=0; i<s.Length/4; i++){
            //print(i);
            Item temp = CraftManager.instance.dic1[s.Substring(i*4, 4)];
            GenImg(temp.ItemImage);
        }

        GenImg(arrow);
        GenImg(res.ItemImage);
    }
}
