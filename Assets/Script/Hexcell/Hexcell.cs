using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEditor;

public class Hexcell : MonoBehaviour
{
    public int index_x; 
    public int index_y; //格子位置序号
    public Item cell_item;  //格子中的物品
    public MeshRenderer cell_m;
    public MeshRenderer outline_m;
    public MeshRenderer highlight_m;
    public SpriteRenderer cell_img;

    //public Action enemy_init;
    // private void Awake() {
    //     var temp_or_member_var = GetComponent<MeshRenderer>().material;
    // }
    public void Update_cell(){
        cell_m.material = cell_item.cell_m;
        outline_m.material = cell_item.outline_m;
        cell_img.sprite = cell_item.ItemImage;
        transform.GetComponent<Enemy>().Init();
    }
}
