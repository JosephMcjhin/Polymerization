using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CraftManager : MonoBehaviour
{
    int now_x;
    int now_y;  //当前鼠标所在的格子
    Vector2Int[,] dir = new Vector2Int[2,6]; //一个格子周围格子的行列位置，奇偶行的位置不同
    public int size_x;
    public int size_y;
    public Recipe recipe;   //合成公式
    public List<Item> item_list = new List<Item>(); //所有物品的列表，用于构造字典
    public List<Item> level1_wish = new List<Item>();
    public List<Item> level2_wish = new List<Item>();
    public List<Item> enemy_list = new List<Item>();
    public List<Item> devil_list = new List<Item>();
    public int devil_level;
    public List<Item> map_item = new List<Item>();  //预设的棋盘上每个格子里的物品
    public List<List<Hexcell>> cell_list = new List<List<Hexcell>>();   //棋盘格子的列表
    Dictionary<string,string> dic = new Dictionary<string,string>();    //合成公式
    public Dictionary<string,Item> dic1 = new Dictionary<string,Item>();   //编号转物品
    Dictionary<string, bool> dic2 = new Dictionary<string, bool>();     //合成公式是否创建过UI
    public Camera camera1;
    public static CraftManager instance;
    GameObject pre_clicked;
    void Awake(){
        if(instance != null && instance != this){
            Destroy(this);
        }
        instance = this;
    }

    public Action enemy_action;
    public Action<Item> ui_action;
    public Action<string,Item> recipe_action;
    public Action money_action;
    public Action<int> tips_action;
    int now_turn;
    private int Money;
    public int money{
        get{
            return Money;
        }
        set{
            Money = value;
            money_action();
        }
    }

    void Start(){
        for(int i=0;i<recipe.recipe.Count;i++){
            dic.Add(recipe.recipe[i].Substring(0,recipe.recipe[i].Length-4),recipe.recipe[i].Substring(recipe.recipe[i].Length-4,4));   //分解，合成公式
            dic2.Add(recipe.recipe[i].Substring(0,recipe.recipe[i].Length-4), false);
        }
        for(int i=0;i<item_list.Count;i++){
            dic1.Add(string.Format("{0:D4}",i),item_list[i]);     //编号转成字符串并填充4位
        }

        dir = new Vector2Int[,]{
            {    
                new Vector2Int(1,-1),
                new Vector2Int(1,0),
                new Vector2Int(0,1),
                new Vector2Int(0,-1),
                new Vector2Int(-1,-1),
                new Vector2Int(-1,0),
            },

            {
                new Vector2Int(1,1),
                new Vector2Int(1,0),
                new Vector2Int(0,1),
                new Vector2Int(0,-1),
                new Vector2Int(-1,1),
                new Vector2Int(-1,0),
            },
        };

    }

    GameObject nowdrag_obj; //当前正在拖动的物品
    Vector3 pre_mousepos;   //之前鼠标的位置，用于跟随拖动的物品
    bool ondrag;    //判断是否正在拖动
    public float click_gap; //双击判断时间
    float last_click;   //上次点击的时间

    
    void Update(){
        if(!CameraManager.instance.Onmaincamera || enemy_moving > 0)return;
        if(Input.GetMouseButtonDown(1) && !ondrag){     //删除格子中的物品
            Ray ray = camera1.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit)){
                GameObject temp = hit.collider.gameObject;
                if(temp.GetComponent<Hexcell>() && now_x == temp.GetComponent<Hexcell>().index_x && now_y == temp.GetComponent<Hexcell>().index_y){ //必须当前格子高亮
                    money += temp.GetComponent<Hexcell>().cell_item.value/20;
                    temp.GetComponent<Hexcell>().cell_item = item_list[0];
                    temp.GetComponent<Hexcell>().Update_cell();
                    return;
                }
            }
        }

        if(Input.GetMouseButtonDown(0) && !ondrag){     //对于空格，双击合成，对于有物品的格子，实现拖动。
            Ray ray = camera1.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit)){
                GameObject temp = hit.collider.gameObject;
                //判断双击
                if(temp.GetComponent<Hexcell>() && now_x == temp.GetComponent<Hexcell>().index_x && now_y == temp.GetComponent<Hexcell>().index_y){
                    if(Time.time - last_click < click_gap){
                        Craft();
                        return;
                    }
                    last_click = Time.time;
                }
                //拖动
                if(temp.GetComponent<Hexcell>() && now_x == temp.GetComponent<Hexcell>().index_x && now_y == temp.GetComponent<Hexcell>().index_y && temp.GetComponent<Hexcell>().cell_item.ItemID != "0000" && temp.GetComponent<Hexcell>().cell_item.belong == 0){
                    ondrag = true;
                    nowdrag_obj = temp;
                    nowdrag_obj.GetComponent<MeshCollider>().enabled = false;
                    pre_mousepos = camera1.ScreenToWorldPoint(Input.mousePosition);
                    return;
                }
                
            }
        }
        else if(Input.GetMouseButtonUp(0) && ondrag){   //结束拖动
            Ray ray = camera1.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            //结束拖动时上面没有格子
            if(!Physics.Raycast(ray,out hit)){
                ondrag = false;
                nowdrag_obj.transform.position = nowdrag_obj.transform.parent.position;
                nowdrag_obj.GetComponent<MeshCollider>().enabled = true;
                nowdrag_obj = null;
                return;
            }
            //原来的位置
            GameObject temp = hit.collider.gameObject;
            print(temp.GetComponent<Hexcell>().index_x);
            if(!temp.GetComponent<Hexcell>() || (now_x == temp.GetComponent<Hexcell>().index_x && now_y == temp.GetComponent<Hexcell>().index_y)){
                ondrag = false;
                nowdrag_obj.transform.position = nowdrag_obj.transform.parent.position;
                nowdrag_obj.GetComponent<MeshCollider>().enabled = true;
                nowdrag_obj = null;
                return;
            }
            //交换父亲，将位置设置为父亲的位置，并更新cell_item信息。
            if(temp.GetComponent<Hexcell>().cell_item.ItemID == "0000" && money >= 1){
                money -= 1;
            }
            else if(temp.GetComponent<Hexcell>().cell_item.ItemID != "0000" && money >= 2){
                money -= 2;
            }
            else{
                ondrag = false;
                nowdrag_obj.transform.position = nowdrag_obj.transform.parent.position;
                nowdrag_obj.GetComponent<MeshCollider>().enabled = true;
                nowdrag_obj = null;
                return;
            }
            Transform pre_parent = nowdrag_obj.transform.parent;
            GameObject swap_obj = temp;
            nowdrag_obj.transform.SetParent(swap_obj.transform.parent);
            nowdrag_obj.transform.position = nowdrag_obj.transform.parent.position;
            swap_obj.transform.SetParent(pre_parent);
            swap_obj.transform.position = swap_obj.transform.parent.position;
            Swapcell(nowdrag_obj.GetComponent<Hexcell>(), swap_obj.GetComponent<Hexcell>());

            ondrag = false;
            nowdrag_obj.GetComponent<MeshCollider>().enabled = true;
            nowdrag_obj = null;
            return;
        }
        else if(ondrag){    //正在拖动，跟着鼠标走。
            Vector3 delta = camera1.ScreenToWorldPoint(Input.mousePosition) - pre_mousepos;
            nowdrag_obj.transform.position += delta;
            pre_mousepos = camera1.ScreenToWorldPoint(Input.mousePosition);
        }

        //print(ondrag);
    }
    void LateUpdate(){  //获取鼠标点击的格子位置，在操作事件结束后进行高亮操作，保证操作的格子必须是高亮状态的。
        if(!CameraManager.instance.Onmaincamera || enemy_moving > 0)return;
        if(ondrag)return;
        if(Input.GetMouseButtonDown(0)){
            Ray ray = camera1.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit) && hit.collider.gameObject.GetComponent<Hexcell>()){
                now_x = hit.collider.gameObject.GetComponent<Hexcell>().index_x;
                now_y = hit.collider.gameObject.GetComponent<Hexcell>().index_y;
                if(pre_clicked){
                    pre_clicked.GetComponent<Hexcell>().highlight_m.material.SetFloat("_Highlighted",0);
                }
                pre_clicked = hit.collider.gameObject;
                pre_clicked.GetComponent<Hexcell>().highlight_m.material.SetFloat("_Highlighted",1);
            }
        }
        // var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        // RaycastHit hit;
        // if (Physics.Raycast(ray, hit, 100)) {
        //     GameObject target = hit.collider.gameObject;//获得点击的物体
        //     if(Input.getMouseButtonDown("0") && target.GetComponent<Hexcell>()){
        //         now_x = target.GetComponent<Hexcell>().index_x;
        //         now_y = target.GetComponent<Hexcell>().index_y;
        //     }
        // }
    }

    public List<Hexcell> GetCellAround(int x,int y){   //获取周围的格子
        int sign = x % 2;
        List<Hexcell> res = new List<Hexcell>();
        for(int i=0; i<6; i++){
            int temp_x = x + dir[sign,i].x;
            int temp_y = y + dir[sign,i].y;
            if(temp_x < 0 || temp_x >= size_x || temp_y < 0 || temp_y >= size_y){
                continue;
            }
            res.Add(cell_list[temp_x][temp_y]);
        }
        return res;
    }

    public int ValueCal(int x,int y){       //计算估值
        if(cell_list[x][y].cell_item.ItemID != "0000")return -1;
        List<Hexcell> res = GetCellAround(x, y);
        int value = 0;
        for(int i=0; i<res.Count; i++){
            value += (res[i].cell_item.belong == 0)?res[i].cell_item.value:0;
        }
        return value;
    }

    public Vector2Int DirCal(int x,int y, ref int now_v){
        List<Hexcell> res = GetCellAround(x, y);
        Vector2Int dir_res = new Vector2Int(-1, -1);
        for(int i=0; i<res.Count; i++){
            int temp = ValueCal(res[i].index_x, res[i].index_y);
            if(now_v < temp){
                now_v = temp;
                dir_res = new Vector2Int(res[i].index_x, res[i].index_y);
            }
            if(dir_res.x == -1 && dir_res.y == -1 && temp != -1 && now_v == 0){
                dir_res = new Vector2Int(res[i].index_x, res[i].index_y);
            }
        }
        return dir_res;
    }



    public float walk_time;
    public float attack_time;
    public float wait_time;
    float enemy_moving;
    int now_id;
    int pre_id;

    public int enemy_num;

    IEnumerator WalkAnime(Hexcell enemy, int step, int id) {
        //print(enemy_moving);
        while(id != now_id){
            yield return null;
        }
        // if(enemy_moving > 0){
        //     yield return new WaitForSeconds(enemy_moving + now_id * wait_time);
        // }
        int temp_x = enemy.index_x;
        int temp_y = enemy.index_y;
        int now_v = 0;
        List<Hexcell> res1 = GetCellAround(temp_x, temp_y);
        for(int i=0; i<res1.Count; i++){
            now_v += (res1[i].cell_item.belong == 0)?res1[i].cell_item.value:0;
        }
        List<Vector2Int> trail = new List<Vector2Int>();
        for(int i=0; i<step; i++){
            Vector2Int temp = DirCal(temp_x, temp_y, ref now_v);
            if(temp.x == -1 && temp.y == -1){
                break;
            }
            temp_x = temp.x;
            temp_y = temp.y;
            trail.Add(temp);
            enemy_moving += walk_time;
        }
        List<Hexcell> res = GetCellAround(temp_x, temp_y);
        bool has_aim = false;
        for(int i=0; i<res.Count; i++){
            if(res[i].cell_item.ItemID != "0000" && res[i].cell_item.belong == 0){
                has_aim = true;
                enemy_moving += attack_time;
                break;
            }
        }
        //yield return null;
        for(int i=0; i<trail.Count; i++){
            Vector3 pos_diff = cell_list[trail[i].x][trail[i].y].transform.position - enemy.transform.position;
            var temp1 = enemy.transform.parent;
            var temp2 = enemy.transform.position;
            enemy.transform.parent = cell_list[trail[i].x][trail[i].y].transform.parent;
            cell_list[trail[i].x][trail[i].y].transform.parent = temp1;
            cell_list[trail[i].x][trail[i].y].transform.position = temp2;
            yield return new WaitForSeconds(walk_time/2 - 0.2f);
            for(int j=0; j<5; j++){
                enemy.transform.position += pos_diff/100;
                yield return new WaitForSeconds(.02f);
            }
            for(int j=0; j<10; j++){
                enemy.transform.position += pos_diff * 0.09f;
                yield return new WaitForSeconds(.02f);
            }
            for(int j=0; j<5; j++){
                enemy.transform.position += pos_diff/100;
                yield return new WaitForSeconds(.02f);
            }
            yield return new WaitForSeconds(walk_time/2 - 0.2f);
            enemy.transform.position = enemy.transform.parent.position;
            Swapcell(enemy, cell_list[trail[i].x][trail[i].y]);
        }
        if(has_aim){
            enemy.GetComponent<Enemy>().StartAttack();
            yield return new WaitForSeconds(attack_time-.4f);
            for(int i=0; i<res.Count; i++){
                if(res[i].cell_item.ItemID != "0000" && res[i].cell_item.belong == 0){
                    res[i].transform.GetComponent<Enemy>().Hpchange(-enemy.cell_item.atk);
                }
            }
            yield return new WaitForSeconds(.4f);
        }
        now_id ++;
        //print(now_id);
        //print(enemy_num);
        if(now_id == enemy_num) enemy_moving = 0;
    }
    public void WalkAndAttack(Hexcell enemy, int step){
        int temp = pre_id++;
        enemy_moving = 1;
        StartCoroutine(WalkAnime(enemy, step, temp));
    }

    IEnumerator AllyAttack(int dmg){
        enemy_moving = 1;
        yield return new WaitForSeconds(.3f);
        cell_list[now_x][now_y].transform.GetComponent<Enemy>().Hpchange(-dmg);
        yield return new WaitForSeconds(.5f);
        now_id = 0;
        pre_id = 0;
        enemy_moving = 0;
        now_turn ++;
        GlobalAction();
        if(enemy_action!=null){
            enemy_action();
        }
    }

    public void Craft(){
        if(cell_list[now_x][now_y].cell_item.ItemID == "0000"){   //有空位，进行合成操作。
            string temp_str = "";
            string temp3_str = "";
            string[] temp1_str = new string[6]; //收集周围六个格子的物品id。
            List<Hexcell> cell_around = GetCellAround(now_x, now_y);
            for(int i=0; i<6; i++){
                temp1_str[i] = "0000" + i.ToString();
            }
            for(int i=0; i<cell_around.Count; i++){
                temp1_str[i] = cell_around[i].cell_item.ItemID + i.ToString();
            }
            Array.Sort(temp1_str);  //将id进行排序，以方便合成公式的判断。
            string temp2_str = "";
            for(int i=63;i>=0;i--){  //然后用二进制状态i，表示每个物品id是否放入合成公式中进行判断。
                temp_str = "";
                temp3_str = "";
                for(int j=0;j<6;j++){
                    if(((i & 1<<j) != 0) && temp1_str[j].Substring(0,4) != "0000"){
                        temp_str += temp1_str[j].Substring(0,4);
                        temp3_str += temp1_str[j][4];
                    }
                }
                if(temp_str == "")continue;
                if(dic.ContainsKey(temp_str) && ((temp2_str = dic[temp_str]) != null))break; //取第一个合成成功的公式作为最后合成出的物品。
            }

            Item tempp = (temp2_str == "" || !dic1.ContainsKey(temp2_str))?null:dic1[temp2_str];
            if(tempp == null)return;
            cell_list[now_x][now_y].cell_item = tempp;
            cell_list[now_x][now_y].Update_cell();
            ui_action(tempp);
            for(int i=0; i<temp3_str.Length; i++){      //合成用的我方棋子删除
                if(cell_around[(int)(temp3_str[i]-'0')].cell_item.belong == 0){
                    Resetcell(cell_around[(int)(temp3_str[i]-'0')]);
                }
            }
            if(!dic2[temp_str]){
                recipe_action(temp_str, tempp);
                dic2[temp_str] = true;
            }
            now_turn ++;
            GlobalAction();
            now_id = 0;
            pre_id = 0;
            enemy_moving = 0;
            if(enemy_action!=null){
                enemy_action();
            }
            //cell_list[now_x * size_y + now_y].sr.sprite = tempp.ItemImage;  //取出物品并进行赋值。
        }
        else if(cell_list[now_x][now_y].cell_item.belong == 1){
            List<Hexcell> res = GetCellAround(now_x, now_y);
            int dmg = 0;
            for(int i=0; i<res.Count; i++){
                if(res[i].cell_item.belong == 0 && res[i].cell_item.ItemID != "0000") {
                    dmg += res[i].cell_item.atk;
                    if(res[i].cell_item.atk > 0)res[i].GetComponent<Enemy>().StartAttack();
                }
            }
            if(dmg == 0){
                now_id = 0;
                pre_id = 0;
                enemy_moving = 0;
                now_turn ++;
                GlobalAction();
                if(enemy_action!=null){
                    enemy_action();
                }
                return;
            }
            StartCoroutine(AllyAttack(dmg));
        }
        else if(cell_list[now_x][now_y].cell_item.ItemID == "0015"){
            List<Hexcell> res = GetCellAround(now_x, now_y);
            for(int i=0; i<res.Count; i++){
                if(res[i].cell_item.belong == 0 && res[i].cell_item.ItemID != "0000"){
                    cell_list[now_x][now_y].GetComponent<Enemy>().Hpchange(res[i].cell_item.hp/3);
                    cell_list[now_x][now_y].cell_item.atk += res[i].cell_item.atk/10;
                    Resetcell(res[i]);
                }
            }
            now_id = 0;
            pre_id = 0;
            enemy_moving = 0;
            now_turn ++;
            GlobalAction();
            if(enemy_action!=null){
                enemy_action();
            }
        }
        else{
            now_id = 0;
            pre_id = 0;
            enemy_moving = 0;
            now_turn ++;
            GlobalAction();
            if(enemy_action!=null){
                enemy_action();
            }
        }
    }
    void swap(ref int x,ref int y){
        int temp=x;
        x=y;
        y=temp;
    }
    public void Swapcell(Hexcell x1, Hexcell x2){
        Vector2Int v1,v2;
        v1 = new Vector2Int(x1.index_x, x1.index_y);
        v2 = new Vector2Int(x2.index_x, x2.index_y);
        var temp = cell_list[v1.x][v1.y];
        cell_list[v1.x][v1.y] = cell_list[v2.x][v2.y];
        cell_list[v2.x][v2.y] = temp;
        
        swap(ref x1.index_x, ref x2.index_x);
        swap(ref x1.index_y, ref x2.index_y);

        //x1.sr.sprite = x2.sr.sprite;
        //x2.sr.sprite = temp2;
    }

    public void Resetcell(Hexcell x1){
        x1.cell_item = item_list[0];
        x1.Update_cell();
    }

    Vector2Int GenPos(){
        Vector2Int res = new Vector2Int(UnityEngine.Random.Range(0,size_x), UnityEngine.Random.Range(0,size_y));
        while(cell_list[res.x][res.y].cell_item.ItemID != "0000"){
            res.y ++;
            if(res.y == size_y){
                res.y = 0;
                res.x ++;
                if(res.x == size_x) res.x = 0;
            }
        }
        return res;
    }

    void GenItem(List<Item> gen_list, Vector2Int pos){
        //if(enemy_moving > 0)return;
        int temp_z = UnityEngine.Random.Range(0,gen_list.Count);
        cell_list[pos.x][pos.y].cell_item = gen_list[temp_z];
        cell_list[pos.x][pos.y].Update_cell();
        ui_action(gen_list[temp_z]);
    }
    public void Wish(int level){
        if(enemy_moving > 0)return;
        if(level == 1 && money < 3)return;
        if(level == 2 && money < 8)return;
        for(int i=0; i<3; i++){
            Vector2Int pos = GenPos();
            if(level == 1){GenItem(level1_wish, pos);}
            else if(level == 2){GenItem(level2_wish, pos);}
        }
        if(level == 1)money -= 3;
        if(level == 2)money -= 8;
        now_turn ++;
        GlobalAction();
        now_id = 0;
        pre_id = 0;
        enemy_moving = 0;
        if(enemy_action!=null){
            enemy_action();
        }
    }

    public void EnemyGen(){
        Vector2Int pos = GenPos();
        GenItem(enemy_list, pos);
    }

    public void DevilGen(){
        Vector2Int pos = GenPos();
        GenItem(devil_list, pos);
    }
    
    public void GlobalAction(){
        //print(now_turn);
        if((now_turn)%15 == 0){
            if(now_turn == 15)tips_action(2);
            devil_level ++;
            devil_list[0].hp = (int)(devil_list[0].hp *1.4f);
            devil_list[0].atk = (int)(devil_list[0].atk *1.15f);
            for(int i=0; i<Mathf.Min(Mathf.Ceil(now_turn/15),5); i++){
                DevilGen();
            }
        }
        if(now_turn % 5 == 0 && now_turn != 0){
            if(now_turn == 5)tips_action(1);
            EnemyGen();
        }
    }
}
