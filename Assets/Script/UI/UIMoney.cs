using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIMoney : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI tt;
    void Start(){
        CraftManager.instance.money_action += UpdateUI;
        CraftManager.instance.money = 50;
    }

    void UpdateUI(){
        tt.text = "X" + CraftManager.instance.money.ToString();
    }
}
