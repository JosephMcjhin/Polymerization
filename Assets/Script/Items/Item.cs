using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item")]
public class Item : ScriptableObject
{
    public string ItemID;
    public Sprite ItemImage;
    [TextArea]
    public string ItemInfo;
    public Material ItemCellMat;
    public Material ItemOutlineMat;
    public int ItemBelong;
    public int ItemValue;
    public int ItemHp;
    public int ItemAtk;
    public int ItemStepSize;
    public int ItemStepGap;
    public bool IsEmpty()
    {
        return ItemID == "0000";
    }
}