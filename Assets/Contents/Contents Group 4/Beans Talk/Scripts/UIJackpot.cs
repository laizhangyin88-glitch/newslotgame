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


    private const string ON_CONTENT_EVENT = "OnContentEvent";

    void Start()
    {
        nowData = minData + Random.Range(0, 100);
        tempGap = gap;

        MessageDispatcher.Register(ON_CONTENT_EVENT, OnJackpotEvent);
    }

    private void OnDestroy()
    {
        MessageDispatcher.UnRegister(ON_CONTENT_EVENT, OnJackpotEvent);
    }


    bool isJackpotRun = true;

    private void OnJackpotEvent(ParadoxNotion.EventData eventData)
    {
        if (eventData.name == "JackpotRun")
        {
            isJackpotRun = true;
        }
        else if (eventData.name == "JackpotStop")
        {
            isJackpotRun = false;
        }
    }



    void Update()
    {
        tempGap--;

        if (tempGap <=0)
        {
            tempGap = gap;

            if (isJackpotRun) //!BlackboardQueryUtils.IsSpin())
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
