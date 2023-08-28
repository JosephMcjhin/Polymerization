using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITips : MonoBehaviour
{
    public TextMeshProUGUI tt;
    [TextArea]
    public string[] tips;
    public Camera ca;
    void Start(){
        CraftManager.instance.tips_action += Display;
        Display(0);
    }

    void Display(int now){
        ca.enabled = true;
        tt.text = tips[now];
    }
    void Update(){
        if(Input.GetMouseButtonDown(0)){
            ca.enabled = false;
        }
    }
}
