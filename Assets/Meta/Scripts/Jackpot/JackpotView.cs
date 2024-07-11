using ParadoxNotion;
using SlotMaker;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class JackpotView : MonoBehaviour
{

    [HideInInspector]
    public List<int> datas = new List<int>();
    [HideInInspector]
    public List<OSA_JackpotNum> jackpotNums = new List<OSA_JackpotNum>();
    public int jackpot = 0;
    public string flag;

    private float aniSpeed = 2f;

    private void Awake()
    {
        var trans = transform.Find("Nums");
        for (int i = 0; i < trans.childCount; i++)
        {
            jackpotNums.Add(trans.GetChild(i).GetComponent<OSA_JackpotNum>());
            jackpotNums[i].numIndex = i;
            jackpotNums[i].flag = flag;
            datas.Add(0);
        }
        MessageDispatcher.Register("JackpotNumChange", OnJackpotNumChange);
    }

    private void OnDestroy()
    {
        MessageDispatcher.UnRegister("JackpotNumChange", OnJackpotNumChange);
    }

    public void SetJackpot(int jackpot)
    {
        this.jackpot = jackpot;
        for (int i = 0; i < jackpotNums.Count; i++)
        {
            jackpotNums[i].ScrollTo(jackpot % 10, .5f, .5f);
            datas[i] = jackpot % 10;
            jackpot /= 10;
        }
    }

    public void ScrollTo(int value)
    {
        if (value == jackpot)
            return;
        if (jackpot > value || jackpot == 0)
            SetJackpot(value);
        value -= jackpot;
        jackpot = value;
        int single = value % 10;
        int round = single <= jackpotNums[0].curItemIndex ? value / 10 - 1 : value / 10;
        jackpotNums[0].Simulation(single, aniSpeed * (round + 1), round);
    }

    void OnJackpotNumChange(EventData data)
    {
        if (data.name == flag)
        {
            string str = "";
            for (int i = datas.Count - 1; i >= 0; i--)
            {
                str += datas[i];
            }
            int index = (int)data.value + 1;
            int temp = datas[index];
            int scrollValue = temp + 1 > 9 ? 0 : temp + 1;
            datas[index] = scrollValue;
            jackpotNums[index].Simulation(scrollValue, 0.25f, 0);
        }
    }

    public int GetNumberDigit(int n)
    {
        int j = 0;
        for (int i = 0; i < 10; i++)
        {
            if ((n % (Math.Pow(10, i))) == n)
            {
                j = i;
                break;
            }
        }
        return j;
    }
}
