using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    public Hexcell cell;
    public int step_size;
    int hp;
    public TextMeshProUGUI hp_text;
    public GameObject hp_bar;

    public GameObject attack_obj;

    public void Init(){
        if(cell.cell_item.belong == 1){
            CraftManager.instance.enemy_action += EnemyWalkAndAttack;
            CraftManager.instance.enemy_num ++;
            //CraftManager.instance.enemy_action += EnemyAttack;
        }
        
        hp = cell.cell_item.hp;
        Hpchange(0);
    }

    public void StartAttack(){
        attack_obj.SetActive(true);
    }

    IEnumerator Shake(){
        Vector3 pre_pos = transform.position;
        Vector3 shake_pos = new Vector3(Random.Range(0f, 0.5f), Random.Range(0f, 0.5f), 0);
        for(int i=0; i<20; i++){
            transform.position = pre_pos + ((i%2 == 1)?-1:1) *  shake_pos;
            yield return new WaitForSeconds(.02f);
        }
        transform.position = pre_pos;

        if(hp == 0){
            if(cell.cell_item.belong == 1){
                CraftManager.instance.enemy_action -= EnemyWalkAndAttack;
                CraftManager.instance.enemy_num --;
                CraftManager.instance.money += cell.cell_item.value/5;
                //CraftManager.instance.enemy_action -= EnemyAttack;
            }
            if(cell.cell_item.ItemID == "0015")CraftManager.instance.tips_action(3);
            CraftManager.instance.Resetcell(cell);
        }
    }

    public void Hpchange(int x){
        if(x==0){
            HpUpdate();
            return;
        }
        //print(cell.cell_item.hp);
        //print(cell.cell_item.ItemID);
        hp += x;
        if(cell.cell_item.ItemID == "0015"){
            hp = Mathf.Max(hp, 0);
            HpUpdate();
            if(x < 0){
                StartCoroutine(Shake());
            }
            return;
        }
        hp = Mathf.Clamp(hp, 0, cell.cell_item.hp);
        HpUpdate();
        StartCoroutine(Shake());
    }

    void HpUpdate(){
        if(hp == 0){
            hp_bar.SetActive(false);
        }
        else{
            hp_bar.SetActive(true);
            hp_text.text = hp.ToString();
        }
    }

    void EnemyWalkAndAttack(){
        //print("我是"+ cell.index_x);
        //print("我是"+ cell.index_y);
        CraftManager.instance.WalkAndAttack(cell, step_size);
    }
    
}
