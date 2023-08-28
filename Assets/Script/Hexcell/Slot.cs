using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public int index_x;
    public int index_y;
    public GameObject cell;
    public void Init(){
        GameObject temp = Instantiate(cell, transform.position, Quaternion.identity);
        Hexcell temp1 = temp.GetComponent<Hexcell>();
        temp1.cell_item = CraftManager.instance.map_item[index_x * CraftManager.instance.size_y + index_y];
        temp1.index_x = index_x;
        temp1.index_y = index_y;
        temp1.Update_cell();
        temp.transform.SetParent(transform);
        //temp.GetComponent<Enemy>().Init();
        CraftManager.instance.cell_list[index_x].Add(temp1);
    }
}
