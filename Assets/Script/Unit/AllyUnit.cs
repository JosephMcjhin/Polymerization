using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyUnit : BaseUnit
{
    
    void Awake()
    {
        Cell = gameObject.GetComponent<Hexcell>();
        CellItem = Cell.CellItem;
        
        Init();
    }

    protected override void Init()
    {
        MaxHp = CellItem.ItemHp;
        NowHp = MaxHp;
        Atk = CellItem.ItemAtk;
        AttackObj = Cell.AttackObj;
    }

    public override IEnumerator Attack()
    {
        TurnManager.Instance.AttackBusy ++;

        AttackObj.SetActive(true);
        yield return new WaitForSeconds(AttackObj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        AttackObj.SetActive(false);

        TurnManager.Instance.AttackBusy --;
    }

}
