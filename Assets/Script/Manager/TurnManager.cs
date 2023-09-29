using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public bool NowTurn;    //false我方行动,true敌方行动
    int nowTurnNum; //当前轮数
    public int AttackBusy;   //当有攻击动画执行时，某些动作无法执行
    public int ShakeBusy;   //当有受击动画执行时，某些动作无法执行
    public int MoveBusy;   //当有移动动画执行时，某些动作无法执行
    public Dictionary<Vector2Int, EnemyUnit> EnemyDic = new Dictionary<Vector2Int, EnemyUnit>();

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        Instance = this;
    }
    
    public void ChangeTurn()
    {
        NowTurn = !NowTurn;
        if(NowTurn)
        {
            nowTurnNum ++;
            if(nowTurnNum % 4 == 0)
            {
                BasicData.Instance.EnemyGen();
            }
            StartCoroutine(EnemyTurn());
        }
    }

    public bool AnimeBusy()
    {
        return AttackBusy > 0 || ShakeBusy > 0 || MoveBusy > 0;
    }
    IEnumerator EnemyTurn()
    {
        for(int i=0; i<BasicData.Instance.MapSize.x; i++)
        {
            for(int j=0; j<BasicData.Instance.MapSize.y; j++)
            {
                while(AnimeBusy())
                {
                    yield return null;
                }
                EnemyUnit temp;
                if(EnemyDic.ContainsKey(new Vector2Int(i,j)) && (temp = EnemyDic[new Vector2Int(i,j)]) != null)
                {
                    temp.NowGap --;
                    if(temp.NowGap == 0)
                    {
                        temp.NowGap = temp.StepGap;
                        StartCoroutine(temp.Move());
                    }
                }
            }
        }

        ChangeTurn();
    }

    public void Wish(int level)
    {
        if(level == 1 && !BasicData.Instance.TestMoney(3))return;
        if(level == 2 && !BasicData.Instance.TestMoney(8))return;
        for(int i=0; i<3; i++)
        {
            Vector2Int pos = BasicData.Instance.GenPos();
            if(level == 1){BasicData.Instance.GenItem(BasicData.Instance.Lvl1Wish, pos);}
            else if(level == 2){BasicData.Instance.GenItem(BasicData.Instance.Lvl2Wish, pos);}
        }
        
        ChangeTurn();
    }

}
