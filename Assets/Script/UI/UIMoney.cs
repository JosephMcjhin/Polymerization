using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIMoney : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI tt;
    void Awake()
    {
        BasicData.Instance.OnMoneyChanged += UpdateUI;
        BasicData.Instance.Money = 50;
    }

    void UpdateUI()
    {
        tt.text = "X" + BasicData.Instance.Money.ToString();
    }
}
