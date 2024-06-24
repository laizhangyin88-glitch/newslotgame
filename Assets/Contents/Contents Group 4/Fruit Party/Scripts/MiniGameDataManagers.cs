using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Game1Data
{
    public int card_index;
    public int[] middle_list;
    public int round_mutiple;
}

public class Game2Data
{
    public int card_index;
    public int step_mutiple;
    public int extern_mutiple;
}

public class MiniGameDataManagers
{
    private static MiniGameDataManagers instance;
    public static MiniGameDataManagers Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new MiniGameDataManagers();
            }
            return instance;
        }
    }

    public List<Game1Data> game1Datas = new List<Game1Data>();
    public List<Game2Data> game2Datas = new List<Game2Data>();
    public void FillGame1Data(JSONNode node)
    {
        game1Datas.Clear();
        Debug.LogError("免费游戏的数据..................." + node.ToString());
        for (int i = 0; i < node.Count; i++)
        {
            var temp = node[i];
            Game1Data data = new Game1Data();
            data.card_index = temp["card_index"];
            if(data.card_index == 100)
            {
                data.card_index = 10;//退出图标转义为 10
            }
            data.round_mutiple = temp["round_mutiple"];
            var ttt = temp["middle_list"];
            data.middle_list = new int[ttt.Count];
            for (int j = 0; j < ttt.Count; j++)
            {
                data.middle_list[j] = ttt[j];
            }
            Debug.LogError("转灯结果......" + data.card_index);
            if(data.round_mutiple > 0)
            {
                Debug.LogError("击中............................................." + data.card_index);
            }
            game1Datas.Add(data);
        }
        Debug.LogError(game1Datas.Count); 
    }

    public void FillGame2Data(JSONNode node) 
    {
        game2Datas.Clear();
        Debug.LogError("免费游戏的数据..................." + node.ToString());
        for (int i = 0; i < node.Count; i++)
        {
            var temp = node[i];
            for (int j = 0; j < temp.Count; j++)
            {
                var t = temp[j];
                Game2Data game2Data = new Game2Data();
                game2Data.card_index = t["card_index"];
                game2Data.step_mutiple = t["step_mutiple"];
                if (t["extern_mutiple"] != null)
                {
                    game2Data.extern_mutiple = t["extern_mutiple"];
                }
                else
                {
                    game2Data.extern_mutiple = 0;
                }
                game2Datas.Add(game2Data);
            }
        }
        Debug.LogError (game2Datas.Count);
    }
}
