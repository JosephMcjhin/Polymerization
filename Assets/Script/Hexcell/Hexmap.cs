using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Hexmap : MonoBehaviour
{
    public GameObject slot; //六边形格子，棋盘长宽大小，格子之间的空隙。
    public int sizex;
    public int sizey;
    public float padding;

    private void Start() {
        float cell_size = 2.2f + padding;
        float radius_size = cell_size * (float)Math.Sqrt(3)/2;
        for(int i=0; i<sizex; i++){
            List<Hexcell> temp = new List<Hexcell>();
            CraftManager.instance.cell_list.Add(temp);
        }
        for(int i=0;i<sizex;i++){   //在指定位置画出六边形格子，并赋值坐标。
            for(int j=0;j<sizey;j++){
                GameObject temp = Instantiate(slot, new Vector3(0,0,0), Quaternion.identity);
                temp.GetComponent<Slot>().index_x = i;
                temp.GetComponent<Slot>().index_y = j;
                temp.GetComponent<Slot>().Init();
                //temp.GetComponent<Hexcell>().cell_item = CraftManager.instance.map_item[i* sizey + j];
                //temp.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = temp.GetComponent<Hexcell>().cell_item.ItemImage;
                temp.transform.SetParent(gameObject.transform);
                temp.transform.position = new Vector3(2 * j * radius_size + (i%2==1?radius_size:0), 1.5f * i * cell_size, 0);
                //CraftManager.instance.cell_list.Add(temp.GetComponent<Hexcell>());
            }
        }
    }
}
