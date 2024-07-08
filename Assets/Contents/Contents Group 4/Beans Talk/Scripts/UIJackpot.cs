using BagelCode;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIJackpot : MonoBehaviour
{

    public long minData;
    public long maxData;

    public int gap = 10;

    private long nowData;
    private int tempGap;

    public ContextElement compText;

    void Start()
    {
        nowData = minData + Random.Range(0, 100);
        tempGap = gap;


    }

    void Update()
    {
        tempGap--;

        if (tempGap <=0)
        {
            tempGap = gap;

            if (!BlackboardQueryUtils.IsSpin())
            {
                nowData += Random.Range(10000, 100);
                if (nowData > maxData)
                {
                    nowData = minData + Random.Range(0, 100);
                }
                ContextUtils.SetGlobalText(compText, "TEXT_BET_CREDIT", nowData);
                //compText.text = nowData.ToString();
            }
        }
    }
}
