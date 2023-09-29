using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultUnit : MonoBehaviour
{
    Hexcell Cell;
    private void Awake()
    {
        Cell = GetComponent<Hexcell>();
        Cell.HpBar.SetActive(false);
    }
}
