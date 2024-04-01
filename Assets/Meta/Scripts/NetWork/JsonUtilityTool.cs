/**/using UnityEngine;
using System;
using System.Collections.Generic;
using BagelCode.ClientModels;
using SlotMaker;
using Dreamteck.Splines.Primitives;

[Serializable]
class Enemy
{
    [SerializeField]
    string name;
    [SerializeField]
    List<string> skills;
    public Enemy(string name, List<string> skills)
    {
        this.name = name;
        this.skills = skills;
    }
}

// List<T>


[Serializable]
public class Serialization<T>
{
    [SerializeField]
    List<T> target;

    public List<T> ToList() { return target; }
    public Serialization(List<T> target)
    {
        this.target = target;
    }
}


// Dictionary<TKey, TValue>
[Serializable]
public class Serialization<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField]
    List<TKey> keys;
    [SerializeField]
    List<TValue> values;

    Dictionary<TKey, TValue> target;
    public Dictionary<TKey, TValue> ToDictionary() { return target; }
    public Serialization(Dictionary<TKey, TValue> target)
    {
        this.target = target;
    }

    public void OnBeforeSerialize()
    {
        keys = new List<TKey>(target.Keys);
        values = new List<TValue>(target.Values);
    }

    public void OnAfterDeserialize()
    {
        var count = Math.Min(keys.Count, values.Count);
        target = new Dictionary<TKey, TValue>(count);
        for (var i = 0; i < count; ++i)
        {
            target.Add(keys[i], values[i]);
        }
    }

    public string ChangeStr(string jsonStr)
    {
        if (jsonStr.StartsWith("{\"target\":["))
        {
            return jsonStr;
        }else if (jsonStr.StartsWith("[") && jsonStr.EndsWith("]"))
        {
            return "{\"target\":" + jsonStr + "}";
        }
        else{
            return null;
        }
    }
}
public class JsonUtilityTest
{
    public static void DoExample()
    {
        string data1 = "{\"earnCredit\":6,\"multiplier\":1,\"symbolIndex\":9,\"cells\":[{\"column\":0,\"row\":0},{\"column\":1,\"row\":0},{\"column\":2,\"row\":0}],\"hitCount\":3,\"direction\":1}";
        SymbolWin temp1 = JsonUtility.FromJson<SymbolWin>(data1);
        Debug.Log($"test1 = {JsonUtility.ToJson(temp1)}");//ok

        string data = "{\"target\":[{\"earnCredit\":6,\"multiplier\":1,\"symbolIndex\":9,\"cells\":[{\"column\":0,\"row\":0},{\"column\":1,\"row\":0},{\"column\":2,\"row\":0}],\"hitCount\":3,\"direction\":1},{\"earnCredit\":6,\"multiplier\":1,\"symbolIndex\":9,\"cells\":[{\"column\":0,\"row\":0},{\"column\":1,\"row\":0},{\"column\":2,\"row\":1}],\"hitCount\":3,\"direction\":1},{\"earnCredit\":6,\"multiplier\":1,\"symbolIndex\":9,\"cells\":[{\"column\":0,\"row\":0},{\"column\":1,\"row\":0},{\"column\":2,\"row\":2}],\"hitCount\":3,\"direction\":1},{\"earnCredit\":6,\"multiplier\":1,\"symbolIndex\":9,\"cells\":[{\"column\":0,\"row\":0},{\"column\":1,\"row\":0},{\"column\":2,\"row\":1}],\"hitCount\":3,\"direction\":1}]}";
        List<SymbolWin> temp = JsonUtility.FromJson<Serialization<SymbolWin>>(data).ToList();
        Debug.Log($"test2 = {JsonUtility.ToJson(temp)}");  // 输出为： {}
        Debug.Log($"test3 = {JsonUtility.ToJson(new Serialization<SymbolWin>(temp))}");  // ok,但是多个target



        string data001 = "[{\"earnCredit\":6,\"multiplier\":1,\"symbolIndex\":9,\"cells\":[{\"column\":0,\"row\":0},{\"column\":1,\"row\":0},{\"column\":2,\"row\":0}],\"hitCount\":3,\"direction\":1},{\"earnCredit\":6,\"multiplier\":1,\"symbolIndex\":9,\"cells\":[{\"column\":0,\"row\":0},{\"column\":1,\"row\":0},{\"column\":2,\"row\":1}],\"hitCount\":3,\"direction\":1},{\"earnCredit\":6,\"multiplier\":1,\"symbolIndex\":9,\"cells\":[{\"column\":0,\"row\":0},{\"column\":1,\"row\":0},{\"column\":2,\"row\":2}],\"hitCount\":3,\"direction\":1},{\"earnCredit\":6,\"multiplier\":1,\"symbolIndex\":9,\"cells\":[{\"column\":0,\"row\":0},{\"column\":1,\"row\":0},{\"column\":2,\"row\":1}],\"hitCount\":3,\"direction\":1}]";
        List<SymbolWin> temp001 = JsonUtility.FromJson<Serialization<SymbolWin>>(data001).ToList();
        Debug.Log($"test001 = {JsonUtility.ToJson(new Serialization<SymbolWin>(temp001))}");  // ok,但是多个target



        string data002 = "{\"1\":{\"earnCredit\":6,\"multiplier\":1,\"symbolIndex\":9,\"cells\":[{\"column\":0,\"row\":0},{\"column\":1,\"row\":0},{\"column\":2,\"row\":0}],\"hitCount\":3,\"direction\":1},\"2\":{\"earnCredit\":6,\"multiplier\":1,\"symbolIndex\":9,\"cells\":[{\"column\":0,\"row\":0},{\"column\":1,\"row\":0},{\"column\":2,\"row\":0}],\"hitCount\":3,\"direction\":1},\"3\":{\"earnCredit\":6,\"multiplier\":1,\"symbolIndex\":9,\"cells\":[{\"column\":0,\"row\":0},{\"column\":1,\"row\":0},{\"column\":2,\"row\":0}],\"hitCount\":3,\"direction\":1}}";
        Dictionary<string,SymbolWin> temp002 = JsonUtility.FromJson<Serialization<string, SymbolWin>>(data002).ToDictionary();
        Debug.Log($"test002 = {JsonUtility.ToJson(new Serialization<string, SymbolWin>(temp002))}");  // ok,但是多个target



        List<Enemy> enemies = new List<Enemy>();
        enemies.Add(new Enemy("怪物1",new List<string>(){"攻击"}));
        enemies.Add(new Enemy("怪物2", new List<string>() { "攻击", "恢复" }));
        Debug.Log($"test4 = {JsonUtility.ToJson(enemies)}");  // 输出为： {}



        List<Enemy> lst0 = new List<Enemy>();
        lst0.Add(new Enemy("怪物1", new List<string>() { "攻击" }));
        lst0.Add(new Enemy("怪物2", new List<string>() { "攻击", "恢复" }));
        string str = JsonUtility.ToJson(new Serialization<Enemy>(lst0));
        Debug.Log($"test5 = {JsonUtility.ToJson(str)}"); 
        List<Enemy> lst2 = JsonUtility.FromJson<Serialization<Enemy>>(str).ToList();


        Dictionary<int, Enemy> dic = new Dictionary<int, Enemy> {
            {3,new Enemy("怪物1", new List<string>() { "攻击" }) },
            {5,new Enemy("怪物2", new List<string>() { "攻击", "恢复" })}
        };

        string str2 = JsonUtility.ToJson(new Serialization<int, Enemy>(dic));
        Debug.Log($"dic test0 = {str2}");
        Debug.Log($"dic test3 = {JsonUtility.ToJson(str2)}");
        Dictionary<int, Enemy> dic2 = JsonUtility.FromJson<Serialization<int, Enemy>>(str2).ToDictionary();

        string str3 = JsonUtility.ToJson(new Serialization<int, Enemy>(dic2));
        Debug.Log($"dic test7 = {str3}");

        Dictionary<int, Enemy> dic3 = JsonUtility.FromJson<Dictionary<int, Enemy>>(str2);

        Debug.Log($"dic test6 = {JsonUtilityTool.DicToJson<int, Enemy>(dic)}");
        Debug.Log($"lst test6 = {JsonUtilityTool.ListToJson<Enemy>(lst0)}");
    }
}

public class JsonUtilityTool{

    public static string ListToJson<T>(List<T> lst)
    {
        return JsonUtility.ToJson(new Serialization<T>(lst));
    }

    public static List<T> ListFromJson<T>(string str)
    {
        return JsonUtility.FromJson<Serialization<T>>(str).ToList();
    }

    public static string DicToJson<TKey, TValue>(Dictionary<TKey, TValue> dic)
    {
        return JsonUtility.ToJson(new Serialization<TKey, TValue>(dic));
    }

    public static Dictionary<TKey, TValue> DicFromJson<TKey, TValue>(string str)
    {
        return JsonUtility.FromJson<Serialization<TKey, TValue>>(str).ToDictionary();   
    }


}
