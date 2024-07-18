using BagelCode;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JackpotScript : MonoBehaviour
{
    public long minData;
    public long maxData;

    public int gap = 10;

    private long nowData;
    private int tempGap;

    private bool isSpin = false;

    public TextMeshProUGUI compText;
     
    void Start()
    {
        nowData = minData + Random.Range(0, 100);
        tempGap = gap;
        isSpin = true;


        MessageDispatcher.Register("OnContentEvent", OnJackpotEvent);
    }

    private void OnDestroy()
    {
        MessageDispatcher.UnRegister("OnContentEvent", OnJackpotEvent);
    }

    private void OnJackpotEvent(ParadoxNotion.EventData eventData)
    {
        if (eventData.name == "JackpotRun")
        {
            isSpin = false;
        }
        else if (eventData.name == "JackpotStop")
        {
            isSpin = true;
        }
    }

    void Update()
    {
        tempGap--;

        if (tempGap <= 0)
        {
            tempGap = gap;

            if (isSpin)
            {
                nowData += Random.Range(10000, 100);
                if (nowData > maxData)
                {
                    nowData = minData + Random.Range(0, 100);
                }
                if(compText!= null)
                {
                    compText.text = nowData.ToString("N0");
                }
                //compText.text = nowData.ToString();
            }
        }
    }
}
