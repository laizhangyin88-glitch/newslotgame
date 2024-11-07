using Com.ForbiddenByte.OSA.Core;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameHistroyRecordItem : BaseItemViewsHolder
{
    private TextMeshProUGUI ID;
    private TextMeshProUGUI BuildTime;
    private TextMeshProUGUI Account;
    private TextMeshProUGUI GameName;
    private TextMeshProUGUI StartCredit;
    private TextMeshProUGUI Bet;
    private TextMeshProUGUI Line;
    private TextMeshProUGUI Total_Bet;
    private TextMeshProUGUI Total_Win;
    private TextMeshProUGUI EndCredit;

    public override void CollectViews()
    {
        base.CollectViews();
        ID = root.transform.Find("ScrollView/Viewport/Content/Index").GetComponent<TextMeshProUGUI>();
        BuildTime = root.transform.Find("ScrollView/Viewport/Content/BuildTime").GetComponent<TextMeshProUGUI>();
        Account = root.transform.Find("ScrollView/Viewport/Content/Account").GetComponent<TextMeshProUGUI>();
        GameName = root.transform.Find("ScrollView/Viewport/Content/GameName").GetComponent<TextMeshProUGUI>();
        StartCredit = root.transform.Find("ScrollView/Viewport/Content/StartCredit").GetComponent<TextMeshProUGUI>();
        Bet = root.transform.Find("ScrollView/Viewport/Content/Bet").GetComponent<TextMeshProUGUI>();
        Line = root.transform.Find("ScrollView/Viewport/Content/Line").GetComponent<TextMeshProUGUI>();
        Total_Bet = root.transform.Find("ScrollView/Viewport/Content/Total_Bet").GetComponent<TextMeshProUGUI>();
        Total_Win = root.transform.Find("ScrollView/Viewport/Content/Total_Win").GetComponent<TextMeshProUGUI>();
        EndCredit = root.transform.Find("ScrollView/Viewport/Content/EndCredit").GetComponent<TextMeshProUGUI>();

        Account.text = BlackboardUtils.FindValue<string>(MainBlackboard.Get(), "me/name");
    }

    public void UpdateView(GameHistroyRecordItemData data)
    {
        ID.text = data.sn.ToString();
        BuildTime.text = FormatTime(data.game_time);
        GameName.text = GetGameName(data.game_id);
        StartCredit.text = data.init_cent.ToString("N0");
        Bet.text = data.bet.ToString("N0");
        Line.text = data.win_line_count.ToString();
        Total_Bet.text = data.total_bet.ToString("N0");
        Total_Win.text = data.total_win.ToString("N0");
        EndCredit.text = data.end_cent.ToString("N0");
    }

    private string FormatTime(long time)
    {
        DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(time).DateTime;
        return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
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
