using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "New Recipe")]
public class Recipe : ScriptableObject
{
    public List<string> recipe = new List<string>();
}