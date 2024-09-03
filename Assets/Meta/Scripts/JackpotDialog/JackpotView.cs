using BagelCode;
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
    public int testJackpot = 0;
    public string flag;
    private RectTransform dotRect;
    private float aniSpeed = 1f;


    private void Awake()
    {
        var trans = transform.Find("Nums");
        dotRect = transform.Find("dotMask/dot").GetComponent<RectTransform>();
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
        testJackpot = jackpot;
        for (int i = 0; i < jackpotNums.Count; i++)
        {
            if (i > this.jackpot.ToString().Length - 1)
                jackpotNums[i].Sleep();
            jackpotNums[i].ScrollTo(jackpot % 10, .5f, .5f);
            datas[i] = jackpot % 10;
            jackpot /= 10;
        }

        dotRect.anchoredPosition = new Vector2(0, this.jackpot.ToString().Length > 5 ? 0 : -65f);
    }

    private void DotScroll()
    {
        AsyncActionUtils.ApplyAnchoredMovement(this, dotRect.transform, new Vector2(0, -65f), new Vector2(0, 0), 0.25f, TweenUtils.VectorTweenLinear);
    }

    public void Test()
    {
        testJackpot += 133;
        ScrollTo(testJackpot);
    }

    public void ScrollTo(int value)
    {
        int data = jackpotNums[0].StopSimulation();
        if (data != -1)
        {
            jackpot = jackpot / 10 * 10;
            jackpot += data;
        }
        else
        {
            jackpot = jackpot / 10 * 10;
            jackpot += jackpotNums[0].GetCurrentItemValue();
        }
        if (value == jackpot)
            return;
        if (jackpot > value || jackpot == 0)
        {
            SetJackpot(value);
            return;
        }
        int tempValue = value - jackpot;
        int single = value % 10;
        int round = single == jackpot % 10 ? tempValue / 10 - 1 : tempValue / 10;

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
            jackpot += (int)Math.Pow(10, index) * (scrollValue - temp);
            if (index == 5 && jackpotNums[index].sleep)
                DotScroll();
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
