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
    public Material cell_m;
    public Material outline_m;
    public int belong;
    public int value;
    public int hp;
    public int atk;
}