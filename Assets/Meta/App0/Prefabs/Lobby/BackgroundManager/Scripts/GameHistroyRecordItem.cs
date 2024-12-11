using BagelCode.ClientModels;
using Com.ForbiddenByte.OSA.Core;
using GameUtil;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHistroyRecordItem : MonoBehaviour
{
    private TextMeshProUGUI ID;
    private TextMeshProUGUI BuildTime;
    //private TextMeshProUGUI Account;
    private TextMeshProUGUI GameName;
    private TextMeshProUGUI StartCredit;
    private TextMeshProUGUI Bet;
    private TextMeshProUGUI Line;
    private TextMeshProUGUI Total_Bet;
    private TextMeshProUGUI Total_Win;
    private TextMeshProUGUI EndCredit;
    private Image imageBg;
    public GameHistroyRecordController controller;
    private Button moreBtn;

    private GameHistroyRecordItemData gameHistroyRecordItemData;

    private void Awake()
    {
        ID = transform.Find("Index").GetComponent<TextMeshProUGUI>();
        BuildTime = transform.Find("BuildTime").GetComponent<TextMeshProUGUI>();
        //Account = transform.Find("Account").GetComponent<TextMeshProUGUI>();
        GameName = transform.Find("GameName").GetComponent<TextMeshProUGUI>();
        StartCredit = transform.Find("StartCredit").GetComponent<TextMeshProUGUI>();
        Bet = transform.Find("Bet").GetComponent<TextMeshProUGUI>();
        Line = transform.Find("Line").GetComponent<TextMeshProUGUI>();
        Total_Bet = transform.Find("Total_Bet").GetComponent<TextMeshProUGUI>();
        Total_Win = transform.Find("Total_Win").GetComponent<TextMeshProUGUI>();
        EndCredit = transform.Find("EndCredit").GetComponent<TextMeshProUGUI>();
        imageBg = GetComponent<Image>();
        moreBtn = transform.GetComponent<Button>();
        moreBtn.onClick.AddListener(OnClickMoreBtn);
    }

    private void OnClickMoreBtn()
    {
        if(gameHistroyRecordItemData != null)
        {
            controller.ShowMoreView(gameHistroyRecordItemData);
        }
    }

    public void UpdateView(GameHistroyRecordItemData data, int index)
    {
        gameHistroyRecordItemData = data;
        ID.text = data.sn.ToString();
        BuildTime.text = FormatTime(data.game_time);
        GameName.text = GetGameName(data.game_id);
        StartCredit.text = data.init_cent.ToString("N0");
        Bet.text = data.bet.ToString("N0");
        Line.text = data.win_line_count.ToString();
        Total_Bet.text = data.total_bet.ToString("N0");
        Total_Win.text = data.total_win.ToString("N0");
        EndCredit.text = data.end_cent.ToString("N0");

        if (index % 2 == 0)
        {
            imageBg.color = Color.white;
        }
        else
        {
            imageBg.color = new Color(0, 1, 1, 1);
        }
    }
    private string FormatTime(long time)
    {
        long unixTimestamp = time; 
        int timeOffset = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "timezone_offset").value;
        DateTimeOffset dateTimeOffsetUtc = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp + timeOffset);
        return dateTimeOffsetUtc.ToString("yyyy-MM-dd HH:mm:ss");
    }

    private string GetGameName(int gameId)
    {
        if (GameHistroyRecordController.gameNameDicti.ContainsKey(gameId))
        {
            return GameHistroyRecordController.gameNameDicti[gameId];
        }
        return "";
    }
}
