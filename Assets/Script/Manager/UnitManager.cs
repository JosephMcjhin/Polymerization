using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    GameObject nowDragObj; //当前正在拖动的物品
    Vector3 preMousePos;   //之前鼠标的位置，用于跟随拖动的物品
    bool onDrag;    //判断是否正在拖动
    public float ClickGap; //双击判断时间
    float lastClick;   //上次点击的时间
    Hexcell curPos;  //当前高亮的格子

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        Instance = this;
    }

    void GetCurrentCell(out Hexcell curCell, out GameObject curObj)
    {
        Ray ray = CameraManager.Instance.MainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject temp = hit.collider.gameObject;
            if (temp.GetComponent<Hexcell>()) { curCell = temp.GetComponent<Hexcell>(); curObj = temp; return; }
        }
        curCell = null;
        curObj = null;
        return;
    }

    bool CanMove()
    {
        return CameraManager.Instance.OnMainCamera && !TurnManager.Instance.AnimeBusy() && !TurnManager.Instance.NowTurn;
    }
    
    void Update()
    {
        if(!CanMove())return;
        if(!curPos)return;
        Hexcell curCell;
        GameObject curObj;
        GetCurrentCell(out curCell, out curObj);    //获取当前鼠标所指的格子
        if(Input.GetMouseButtonDown(1) && !onDrag)    //删除格子中的物品
        {
            if(curCell && curPos.Index == curCell.Index)
            {
                BasicData.Instance.Money += curCell.CellItem.ItemValue/20;
                curCell.ResetCell();
                return;
            }
        }
        else if(Input.GetMouseButtonDown(0) && !onDrag)     //对于空格，双击合成，对于有物品的格子，实现拖动。
        {
            //判断双击
            if(curCell && curPos.Index == curCell.Index)
            {
                if(Time.time - lastClick < ClickGap){Craft();return;}
                lastClick = Time.time;
            }
            //拖动
            if(curCell && curPos.Index == curCell.Index && !curCell.CellItem.IsEmpty() && curCell.CellItem.ItemBelong == 1)
            {
                onDrag = true;
                nowDragObj = curObj;
                nowDragObj.GetComponent<MeshCollider>().enabled = false;
                preMousePos = CameraManager.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
                return;
            }
        }
        else if(Input.GetMouseButtonUp(0) && onDrag)   //结束拖动
        {
            //结束拖动时上面没有对象，或者有对象但不是格子，或者是原来的位置
            if(!curObj || !curCell || curPos.Index == curCell.Index)
            {
                onDrag = false;
                nowDragObj.transform.position = nowDragObj.transform.parent.position;
                nowDragObj.GetComponent<MeshCollider>().enabled = true;
                nowDragObj = null;
                return;
            }
            //交换父亲，将位置设置为父亲的位置，并更新CellItem信息。
            if(curCell.CellItem.IsEmpty() && BasicData.Instance.TestMoney(1)){}
            else if(!curCell.CellItem.IsEmpty() && BasicData.Instance.TestMoney(2)){}
            else
            {
                onDrag = false;
                nowDragObj.transform.position = nowDragObj.transform.parent.position;
                nowDragObj.GetComponent<MeshCollider>().enabled = true;
                nowDragObj = null;
                return;
            }
            GameObject swapObj = curObj;
            BasicData.Instance.SwapParent(swapObj.transform, nowDragObj.transform);
            BasicData.Instance.SwapCell(nowDragObj.GetComponent<Hexcell>(), swapObj.GetComponent<Hexcell>());

            onDrag = false;
            nowDragObj.GetComponent<MeshCollider>().enabled = true;
            nowDragObj = null;
            return;
        }
        else if(onDrag)   //正在拖动，跟着鼠标走。
        {
            Vector3 delta = CameraManager.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition) - preMousePos;
            nowDragObj.transform.position += delta;
            preMousePos = CameraManager.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
    }
    void LateUpdate() //获取鼠标点击的格子位置，在操作事件结束后进行高亮操作，保证操作的格子必须是高亮状态的。
    {
        if(!CanMove())return;
        if(onDrag)return;
        Hexcell curCell;
        GameObject curObj;
        GetCurrentCell(out curCell, out curObj);    //获取当前鼠标所指的格子
        if(Input.GetMouseButtonDown(0))
        {
            if(curCell)
            {
                if(curPos != null)curPos.HighlightMat.material.SetFloat("_Highlighted",0);
                curPos = curCell;
                curPos.HighlightMat.material.SetFloat("_Highlighted",1);
            }
        }
    }

    public void Craft(){
        if(curPos.CellItem.IsEmpty())   //有空位，进行合成操作。
        {
            string curIDStr = "";   //当前组合出的ID组合
            string resIDStr = "";   //得到合成结果ID
            string curIndexStr = "";    //当前组合的序号组合
            string[] aroundID = new string[6]; //收集周围六个格子的物品id。
            List<Hexcell> cellAround = BasicData.Instance.GetCellAround(curPos.Index);
            for(int i=0; i<6; i++){aroundID[i] = "0000" + i.ToString();}
            for(int i=0; i<cellAround.Count; i++){aroundID[i] = cellAround[i].CellItem.ItemID + i.ToString();}  //追踪id+位置，排序后仍然能看的出来。
            Array.Sort(aroundID);  //将id进行排序，以方便合成公式的判断。
            for(int i=63;i>=0;i--)  //然后用二进制状态i，表示每个物品id是否放入合成公式中进行判断。
            {
                curIDStr = "";
                curIndexStr = "";
                for(int j=0;j<6;j++)
                {
                    if(((i & 1<<j) != 0) && aroundID[j].Substring(0,4) != "0000")
                    {
                        curIDStr += aroundID[j].Substring(0,4);
                        curIndexStr += aroundID[j][4];
                    }
                }
                if(curIDStr == "")continue;
                if(BasicData.Instance.DicRecipe.ContainsKey(curIDStr) && ((resIDStr = BasicData.Instance.DicRecipe[curIDStr]) != null))break; //取第一个合成成功的公式作为最后合成出的物品。
            }

            Item resItem = (resIDStr == "" || !BasicData.Instance.DicID2Item.ContainsKey(resIDStr))?null: BasicData.Instance.DicID2Item[resIDStr];
            if(resItem == null)return;
            curPos.CellItem = resItem;
            curPos.UpdateCell(resItem);
            //ui_action(resItem);
            for(int i=0; i<curIndexStr.Length; i++)     //合成用的我方棋子删除
            {
                if(cellAround[(int)(curIndexStr[i]-'0')].CellItem.ItemBelong == 1){
                    cellAround[(int)(curIndexStr[i]-'0')].ResetCell();
                }
            }
            if(!BasicData.Instance.DicRecipeUnlock[curIDStr])
            {
                //recipe_action(curIDStr, resItem);
                BasicData.Instance.DicRecipeUnlock[curIDStr] = true;
                BasicData.Instance.OnRecipeChanged(curIDStr, resItem);
            }
            
            TurnManager.Instance.ChangeTurn();
        }
        else if(curPos.CellItem.ItemBelong == 2)   //无空位，攻击敌人
        {
            List<Hexcell> res = BasicData.Instance.GetCellAround(curPos.Index);
            int dmg = 0;
            for(int i=0; i<res.Count; i++)
            {
                if(res[i].CellItem.ItemBelong == 1) {
                    dmg += res[i].CellItem.ItemAtk;
                    if(res[i].CellItem.ItemAtk > 0)StartCoroutine(res[i].AScript.Attack());
                }
            }
            if(dmg == 0)return;
            curPos.EScript.HpChange(-dmg);
            TurnManager.Instance.ChangeTurn();
        }
    }
}
