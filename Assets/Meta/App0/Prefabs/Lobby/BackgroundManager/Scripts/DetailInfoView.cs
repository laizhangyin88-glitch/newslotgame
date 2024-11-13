using Dreamteck.Splines.Primitives;
using GameUtil;
using SlotMaker;
using SlotMaker.Slots.Tasks.Actions.Win;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetailInfoView
{
    public GameObject gameObject;
    private Transform transform;
    private Button bgClose;
    private TextMeshProUGUI _text;

    public DetailInfoView(GameObject gameObject)
    {
        this.gameObject = gameObject;
        transform = gameObject.transform;
        Init();
    }

    private void Init()
    {
        _text = transform.Find("AllInfo/TextInfo").GetComponent<TextMeshProUGUI>();
        bgClose = transform.Find("bgClose").GetComponent<Button>();
        bgClose.onClick.AddListener(() => { gameObject.SetActive(false); });
    }

    public void SetText(string text)
    {
        _text.text = text;
    }

    public void SetText(GameHistroyRecordItemData data)
    {
        string result = "\nID: " + data.sn.ToString() + "\n" +
        "\nTime: " + FormatTime(data.game_time) + "\n" +
        "\nGame Name: " + GetGameName(data.game_id) + "\n" +
        "\nStart Credit: " + data.init_cent.ToString("N0") + "\n" +
        "\nBet: " + data.bet.ToString("N0") + "\n" +
        "\nLine: " + data.win_line_count.ToString() + "\n" +
        "\nTotal Bet: " + data.total_bet.ToString("N0") + "\n" +
        "\nTotal Win: " + data.total_win.ToString("N0") + "\n" +
        "\nEnd Credit: " + data.end_cent.ToString("N0");
        _text.text = result;
        Timer.DelayAction(Time.deltaTime, () =>
        {
            _text.text += "\n  ";
            LayoutRebuilder.ForceRebuildLayoutImmediate(_text.transform as RectTransform);
        });
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
