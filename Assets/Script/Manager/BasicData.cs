using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BasicData : MonoBehaviour
{
    public static BasicData Instance;
    public Vector2Int[,] Dir = new Vector2Int[2,6]; //一个格子周围格子的行列位置，奇偶行的位置不同
    public Vector2Int MapSize; //棋盘大小
    public Recipe Recipe;   //合成公式
    public List<Item> ItemList = new List<Item>(); //所有物品的列表，用于构造字典
    public List<Item> Lvl1Wish = new List<Item>();  //第一级召唤棋子列表
    public List<Item> Lvl2Wish = new List<Item>();  //第二级召唤棋子列表
    public List<Item> EnemyList = new List<Item>(); 
    //public List<Item> DevilList = new List<Item>();
    public List<Item> InitItem = new List<Item>();  //预设的棋盘上每个格子里的物品
    public List<List<Hexcell>> CellList = new List<List<Hexcell>>();   //棋盘格子的列表
    public Dictionary<string,string> DicRecipe = new Dictionary<string,string>();    //合成公式
    public Dictionary<string,Item> DicID2Item = new Dictionary<string,Item>();   //编号转物品
    public Dictionary<string, bool> DicRecipeUnlock = new Dictionary<string, bool>();     //合成公式是否创建过UI
    public Camera MainCamera;
    //public int DevilLvl;

    public Action<Item> OnItemCreated;
    public Action<string,Item> OnRecipeChanged;
    public Action OnMoneyChanged;
    public Action<int> OnTipsTriggered;
    private int money;
    public int Money
    {
        get
        {
            return money;
        }
        set
        {
            money = value;
            OnMoneyChanged();
        }
    }
    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        Instance = this;
    }

    void Start(){
        for(int i=0;i<Recipe.RecipeList.Count;i++)
        {
            DicRecipe.Add(Recipe.RecipeList[i].Substring(0,Recipe.RecipeList[i].Length-4),Recipe.RecipeList[i].Substring(Recipe.RecipeList[i].Length-4,4));   //分解，合成公式
            DicRecipeUnlock.Add(Recipe.RecipeList[i].Substring(0,Recipe.RecipeList[i].Length-4), false);
        }
        for(int i=0;i<ItemList.Count;i++)
        {
            DicID2Item.Add(string.Format("{0:D4}",i),ItemList[i]);     //编号转成字符串并填充4位
        }

        Dir = new Vector2Int[,]
        {
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

    public bool TestMoney(int testNum)
    {
        if(Money < testNum)return false;
        Money -= testNum;
        return true;
    }

    public List<Hexcell> GetCellAround(Vector2Int pos)//获取周围的格子
    {
        int sign = pos.x % 2;
        List<Hexcell> res = new ();
        for(int i=0; i<6; i++)
        {
            Vector2Int temp = new Vector2Int(pos.x + Dir[sign,i].x, pos.y + Dir[sign,i].y);
            if(temp.x < 0 || temp.x >= MapSize.x || temp.y < 0 || temp.y >= MapSize.y)continue;
            res.Add(CellList[temp.x][temp.y]);
        }
        return res;
    }

    public List<Hexcell> GetCellAround(Vector2Int pos, out bool hasAim)//获取周围的格子并判断是否有友方棋子
    {
        int sign = pos.x % 2;
        List<Hexcell> res = new ();
        hasAim = false;
        for(int i=0; i<6; i++)
        {
            Vector2Int temp = new Vector2Int(pos.x + Dir[sign,i].x, pos.y + Dir[sign,i].y);
            if(temp.x < 0 || temp.x >= MapSize.x || temp.y < 0 || temp.y >= MapSize.y)continue;
            if(CellList[temp.x][temp.y].CellItem.ItemBelong == 1)hasAim = true;
            res.Add(CellList[temp.x][temp.y]);
        }
        return res;
    }

    public void SwapCell(Hexcell x1, Hexcell x2)    //交换两个格子
    {
        (CellList[x1.Index.x][x1.Index.y], CellList[x2.Index.x][x2.Index.y]) = (CellList[x2.Index.x][x2.Index.y], CellList[x1.Index.x][x1.Index.y]);
        (x1.Index, x2.Index) = (x2.Index, x1.Index);
    }

    public void SwapParent(Transform t1, Transform t2)
    {
        (t1.parent, t2.parent) = (t2.parent, t1.parent);
        (t1.position, t2.position) = (t1.parent.position, t2.parent.position);
    }

    public Vector2Int GenPos() //随机指定一个空位
    {
        Vector2Int res = new Vector2Int(UnityEngine.Random.Range(0,MapSize.x), UnityEngine.Random.Range(0,MapSize.y));
        while(CellList[res.x][res.y].CellItem.ItemID != "0000")
        {
            res.y ++;
            if(res.y == MapSize.y)
            {
                res.y = 0;
                res.x ++;
                if(res.x == MapSize.x) res.x = 0;
            }
        }
        return res;
    }

    public void GenItem(List<Item> genList, Vector2Int pos)    //根据列表生成物品
    {
        int temp = UnityEngine.Random.Range(0,genList.Count);
        CellList[pos.x][pos.y].UpdateCell(genList[temp]);
        //ui_action(genList[temp]);
    }

    public void EnemyGen(){
        Vector2Int pos = GenPos();
        GenItem(EnemyList, pos);
    }
}
