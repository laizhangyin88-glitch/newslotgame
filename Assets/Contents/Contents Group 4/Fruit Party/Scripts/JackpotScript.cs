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

    public TextMeshProUGUI compText;
     
    void Start()
    {
        nowData = minData + Random.Range(0, 100);
        tempGap = gap;
    }

    void Update()
    {
        tempGap--;

        if (tempGap <= 0)
        {
            tempGap = gap;

            if (!BlackboardQueryUtils.IsSpin())
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
