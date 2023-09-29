using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : BaseUnit
{
    int stepSize;
    public int StepGap;

    public int NowGap;
    void Awake()
    {
        Cell = gameObject.GetComponent<Hexcell>();
        CellItem = Cell.CellItem;
        TurnManager.Instance.EnemyDic[Cell.Index] = this;
        Init();
    }

    protected override void Init()
    {
        stepSize = CellItem.ItemStepSize;
        StepGap = CellItem.ItemStepGap;
        NowGap = StepGap;
        MaxHp = CellItem.ItemHp;
        NowHp = MaxHp;
        Atk = CellItem.ItemAtk;
        AttackObj = Cell.AttackObj;
    }

    public int ValueCal(Hexcell curCell, bool flag)       //计算该格子周围格子的最大估值
    {
        if(!curCell.CellItem.IsEmpty() && !flag)return -1;
        List<Hexcell> res = BasicData.Instance.GetCellAround(curCell.Index);
        int value = 0;
        for(int i=0; i<res.Count; i++)
        {
            value += (res[i].CellItem.ItemBelong == 1)?res[i].CellItem.ItemValue:0;
        }
        return Mathf.Max(value, 1);
    }

    public Hexcell DirCal()  //计算每步走的方向
    {
        List<Hexcell> res = BasicData.Instance.GetCellAround(Cell.Index);
        Hexcell dirRes = null;
        int maxVal = -1;
        maxVal = ValueCal(Cell, true);
        for(int i=0; i<res.Count; i++)
        {
            int temp = ValueCal(res[i], false);
            if(maxVal < temp)
            {
                maxVal = temp;
                dirRes = res[i];
            }
        }
        return dirRes;
    }

    public override IEnumerator Attack()
    {
        TurnManager.Instance.AttackBusy ++;
        bool hasAim = false;
        List<Hexcell> res = BasicData.Instance.GetCellAround(Cell.Index, out hasAim);
        if(!hasAim)
        {
            TurnManager.Instance.AttackBusy --;
            yield break;
        }

        AttackObj.SetActive(true);
        yield return new WaitForSeconds(AttackObj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        AttackObj.SetActive(false);
        for(int i=0; i<res.Count; i++)
        {
            if(res[i].CellItem.ItemBelong == 1)
            {
                res[i].AScript.HpChange(-Atk);
            }
        }
        TurnManager.Instance.AttackBusy --;
    }

    public IEnumerator Move()
    {
        TurnManager.Instance.MoveBusy ++;
        for(int i=0; i<stepSize; i++)
        {
            Hexcell dir = DirCal();
            if(dir == null)break;
            Vector3 posDiff = dir.transform.position - transform.position;
            for(int j=0; j<5; j++)
            {
                transform.position += posDiff/100;
                yield return new WaitForSeconds(.02f);
            }
            for(int j=0; j<10; j++)
            {
                transform.position += posDiff * 0.09f;
                yield return new WaitForSeconds(.02f);
            }
            for(int j=0; j<5; j++)
            {
                transform.position += posDiff/100;
                yield return new WaitForSeconds(.02f);
            }
            BasicData.Instance.SwapParent(dir.transform, transform);
            BasicData.Instance.SwapCell(dir, Cell);
        }

        StartCoroutine(Attack());
        TurnManager.Instance.MoveBusy --;
    }

    private void OnDestroy() {
        TurnManager.Instance.EnemyDic[Cell.Index] = null;
    }


}
