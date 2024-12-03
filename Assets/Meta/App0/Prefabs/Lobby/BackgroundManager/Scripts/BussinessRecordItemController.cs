using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BussinessRecordItemController : MonoBehaviour
{
    private TextMeshProUGUI ID;
    private TextMeshProUGUI BuildTime;
    private TextMeshProUGUI PlayerId;
    private TextMeshProUGUI Account;
    private TextMeshProUGUI Before;
    private TextMeshProUGUI Up;
    private TextMeshProUGUI Down;
    private TextMeshProUGUI After;

    private Image imageBg;
    public BussinessRecordController controller;
    private Button moreBtn;

    private BussinessItemData bussinessItemData;

    private void Start()
    {
        ID = transform.Find("Index").GetComponent<TextMeshProUGUI>();
        BuildTime = transform.Find("BuildTime").GetComponent<TextMeshProUGUI>();
        PlayerId = transform.Find("PlayerId").GetComponent<TextMeshProUGUI>();
        Account = transform.Find("Account").GetComponent<TextMeshProUGUI>();
        Before = transform.Find("Before").GetComponent<TextMeshProUGUI>();
        Up = transform.Find("Up").GetComponent<TextMeshProUGUI>();
        Down = transform.Find("Down").GetComponent<TextMeshProUGUI>();
        After = transform.Find("After").GetComponent<TextMeshProUGUI>();
        imageBg = GetComponent<Image>();
        moreBtn = transform.GetComponent<Button>();
        moreBtn.onClick.AddListener(OnClickMoreBtn);
    }

    private void OnClickMoreBtn()
    {
        if (bussinessItemData != null)
        {
            controller.ShowMoreView(bussinessItemData);
        }
    }

    public void UpdateView(BussinessItemData data, int index)
    {
        bussinessItemData = data;
        ID.text = data.id.ToString();
        BuildTime.text = data.change_time;
        PlayerId.text = data.user_id;
        Account.text = data.agent_id;
        Before.text = data.before_credit.ToString("N0");
        if(data.change_credit > 0)
        {
            Up.text = data.change_credit.ToString("N0");
            Down.text = "0";
        }
        else
        {
            Down.text = data.change_credit.ToString("N0");
            Up.text = "0";
        }
        After.text = data.after_credit.ToString("N0");
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
