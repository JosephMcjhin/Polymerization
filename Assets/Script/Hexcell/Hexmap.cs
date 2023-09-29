using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Hexmap : MonoBehaviour
{
    public GameObject Slot; //六边形格子,是Hexcell的父对象
    public float Padding;   //格子空隙

    private void Start()
    {
        float cellSize = 2.2f + Padding;
        float radiusSize = cellSize * (float)Math.Sqrt(3)/2;
        for(int i=0; i<BasicData.Instance.MapSize.x; i++)
        {
            List<Hexcell> temp = new();
            BasicData.Instance.CellList.Add(temp);
        }
        for(int i=0; i<BasicData.Instance.MapSize.x; i++)  //在指定位置画出六边形格子，并赋值坐标。
        { 
            for(int j=0; j<BasicData.Instance.MapSize.y; j++)
            {
                GameObject temp = Instantiate(Slot, new Vector3(0,0,0), Quaternion.identity);
                temp.GetComponent<Slot>().Index = new Vector2Int(i,j);
                temp.GetComponent<Slot>().Init();
                //temp.GetComponent<Hexcell>().cell_item = BasicData.Instance.map_item[i* BasicData.Instance.MapSize.y + j];
                //temp.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = temp.GetComponent<Hexcell>().cell_item.ItemImage;
                temp.transform.SetParent(gameObject.transform);
                temp.transform.position = new Vector3(2 * j * radiusSize + (i%2==1?radiusSize:0), 1.5f * i * cellSize, 0);
                //BasicData.Instance.CellList.Add(temp.GetComponent<Hexcell>());
            }
        }
    }
}
