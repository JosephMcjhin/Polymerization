using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public Vector2Int Index;
    public GameObject HexcellPrefab;
    
    public void Init()
    {
        GameObject cellObj = Instantiate(HexcellPrefab, transform.position, Quaternion.identity);
        Hexcell cell = cellObj.GetComponent<Hexcell>();
        cell.Index = Index;
        cell.ResetCell();
        cellObj.transform.SetParent(transform);
        BasicData.Instance.CellList[Index.x].Add(cell);
    }
}
