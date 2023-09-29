using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUnit : MonoBehaviour
{  
    protected int MaxHp{get;set;}
    protected int nowHp;
    protected int NowHp
    {
        get
        {
            return nowHp;
        }
        set
        {
            nowHp = value;
            if(nowHp == 0)Cell.HpBar.SetActive(false);
            else {Cell.HpBar.SetActive(true); Cell.HpText.text = nowHp.ToString();}
        }
    }
    protected int Atk{get;set;}
    protected Hexcell Cell{get; set;}
    protected Item CellItem{get; set;}
    protected GameObject AttackObj{get; set;}

    protected abstract void Init();
    public abstract IEnumerator Attack();
    
    IEnumerator Shake()
    {
        while(TurnManager.Instance.AttackBusy > 0)
        {
            yield return null;
        }
        TurnManager.Instance.ShakeBusy ++;
        Vector3 prePos = transform.position;
        Vector3 shakePos = new Vector3(0.25f, 0.25f, 0);
        for(int i=0; i<10; i++)
        {
            transform.position = prePos + ((i%2 == 1)?-1:1) * shakePos;
            yield return new WaitForSeconds(.04f);
        }
        transform.position = prePos;
        
        if(NowHp == 0)
        {
            if(Cell.CellItem.ItemBelong == 2)
            {
                BasicData.Instance.Money += Cell.CellItem.ItemValue/5;
            }
            //if(Cell.CellItem.ItemID == "0015")CraftManager.instance.tips_action(3);
            Cell.ResetCell();
        }
        TurnManager.Instance.ShakeBusy --;
    }

    public void HpChange(int x)
    {
        if(x == 0)return;
        NowHp += x;
        NowHp = Mathf.Clamp(NowHp, 0, MaxHp);
        StartCoroutine(Shake());
    }

}
