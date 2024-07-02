using BagelCode;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LineController : MonoBehaviour
{
    private Text lineTxt;
    private TextMeshProUGUI totalTxt;
     
    private int totalLineCount = 0;

    private int currentLine = 1;

    private long currentBet;

    private long totalBet;
    private void Start()
    {
        var temp = BlackboardUtils.FindVariable<long>(null, "./game/baseWager");
        totalLineCount = (int)temp.value;
        currentLine = 15;
        lineTxt = transform.Find("line_Text").GetComponent<Text>();
        totalTxt = transform.Find("total_Text").GetComponent<TextMeshProUGUI>();
        currentBet = BlackboardUtils.FindVariable<long>("./betCredit").value;
        totalBet = currentLine * currentBet;

        MessageDispatcher.Register(MetaEventDefine.ON_CREDIT_EVENT, UpdateBet);
        //BlackboardUtils.SetOrCreateValue<int>(ContentBlackboard.Get(), "./game/lineCount", currentLine);
        BlackboardUtils.GetOrCreateVariable<int>(ContentBlackboard.Get(), "./game/lineCount").value = currentLine;
    }

    private void UpdateBet(EventData eventData)
    {
        currentBet = BlackboardUtils.FindVariable<long>("./betCredit").value;
        UpdateInfo();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            currentLine++;
            if(currentLine > totalLineCount)
            {
                currentLine = 1;
            }
            UpdateInfo();
        }
    }

    private void UpdateInfo()
    {
        lineTxt.text = "Line: " + currentLine;
        totalBet = currentBet * currentLine;
        totalTxt.text = "Total Bet:" + totalBet.ToString("N0");
        BlackboardUtils.GetOrCreateVariable<int>(ContentBlackboard.Get(), "./game/lineCount").value = currentLine;
    }

    private void OnDestroy()
    {
        MessageDispatcher.UnRegister(MetaEventDefine.ON_CREDIT_EVENT, UpdateBet);
    }
}
