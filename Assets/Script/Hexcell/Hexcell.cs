using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEditor;
using TMPro;

public class Hexcell : MonoBehaviour
{
    public Vector2Int Index;  //格子所在的位置
    public Item CellItem; //格子中的当前物品

    public MeshRenderer CellMat;
    public MeshRenderer OutlineMat;
    public MeshRenderer HighlightMat;
    public SpriteRenderer CellImg;
    public GameObject HpBar;
    public TextMeshProUGUI HpText;

    public GameObject AttackObj;

    public EnemyUnit EScript;
    public AllyUnit AScript;
    DefaultUnit dScript;
    
    public void UpdateCell(Item newItem)
    {
        BasicData.Instance.OnItemCreated(newItem);
        CellItem = newItem;
        CellMat.material = CellItem.ItemCellMat;
        OutlineMat.material = CellItem.ItemOutlineMat;
        CellImg.sprite = CellItem.ItemImage;
        if (EScript != null) { Destroy(EScript); EScript = null; }
        if (AScript != null) { Destroy(AScript); AScript = null; }
        if (dScript != null) { Destroy(dScript); dScript = null; }
        if (CellItem.ItemBelong == 1) AScript = gameObject.AddComponent<AllyUnit>();
        else if (CellItem.ItemBelong == 2) EScript = gameObject.AddComponent<EnemyUnit>();
        else dScript = gameObject.AddComponent<DefaultUnit>();
    }
    public void ResetCell()
    {
        UpdateCell(BasicData.Instance.ItemList[0]);
    }
}
