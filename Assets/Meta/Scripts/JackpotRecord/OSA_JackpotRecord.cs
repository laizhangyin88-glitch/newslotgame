using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using ParadoxNotion;
using SimpleJSON;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class OSA_JackpotRecord : OSA<BaseParamsWithPrefab, JackpotRecordItemViewsHolder>
{
    [SerializeField]
    private List<Sprite> bgSprites;
    [SerializeField]
    private List<Sprite> titleSprites;
    private List<JackpotRecordInfo> jackpotRecordInfos = new List<JackpotRecordInfo>();


    protected override void Start()
    {
        base.Start();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        MessageDispatcher.Register(RPCName.queryJackpotRanking, OnQueryJackpotRanking);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        MessageDispatcher.UnRegister(RPCName.queryJackpotRanking, OnQueryJackpotRanking);
    }

    private void OnQueryJackpotRanking(EventData data)
    {
        jackpotRecordInfos.Clear();
        JSONNode json = data.value as JSONNode;
        var recordInfoJson = json["jackpot_ranking_info"];
        int isAll = json["is_all"];
        int recordType = isAll == 1 ? 4 : (int)json["jackpot_id"];
        string bbJackpotRecordStr = $"jackpotRecordType{recordType}";
        MainBlackboard.Get().SetValue(bbJackpotRecordStr, data);
        for (int i = 0; i < recordInfoJson.Count; i++)
        {
            var jackpotRecord = new JackpotRecordInfo
            {
                user_id = recordInfoJson[i]["user_id"],
                earn_credit = recordInfoJson[i]["earn_credit"],
                earn_money = recordInfoJson[i]["earn_money"],
                accept_time = recordInfoJson[i]["accept_time"],
            };
            jackpotRecord.jackpot_id = recordType == 4 ? (int)recordInfoJson[i]["jackpot_id"] : recordType;
            jackpotRecordInfos.Add(jackpotRecord);
        }
        StartCoroutine(DelayReset(recordInfoJson.Count));
        
    }

    private IEnumerator DelayReset(int count)
    {
        yield return new WaitForSeconds(0.15f);
        ResetItems(count);
    }

    protected override JackpotRecordItemViewsHolder CreateViewsHolder(int itemIndex)
    {
        var instance = new JackpotRecordItemViewsHolder();

        instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);

        return instance;
    }


    protected override void UpdateViewsHolder(JackpotRecordItemViewsHolder newOrRecycled)
    {
        var jackpotRecordInfo = jackpotRecordInfos[newOrRecycled.ItemIndex];
        newOrRecycled.BG.sprite = bgSprites[jackpotRecordInfos[newOrRecycled.ItemIndex].jackpot_id];
        newOrRecycled.order.text = (newOrRecycled.ItemIndex + 1).ToString();
        newOrRecycled.id.text = jackpotRecordInfo.user_id;
        newOrRecycled.title.sprite = titleSprites[jackpotRecordInfos[newOrRecycled.ItemIndex].jackpot_id];
        newOrRecycled.time.text = MetaSystem.TimeStampToLocalDateTime(jackpotRecordInfo.accept_time * 1000).ToString("MM/dd HH:mm");
        newOrRecycled.bonus.text = $"{jackpotRecordInfo.earn_money}";
    }


}

public class JackpotRecordItemViewsHolder : BaseItemViewsHolder
{
    public Image BG;
    public TextMeshProUGUI order;
    public TextMeshProUGUI id;
    public Image title;
    public TextMeshProUGUI time;
    public TextMeshProUGUI bonus;

    public override void CollectViews()
    {
        base.CollectViews();
        BG = root.Find("BG").GetComponent<Image>();
        order = root.Find("Order").GetComponent<TextMeshProUGUI>();
        id = root.Find("Id").GetComponent<TextMeshProUGUI>();
        title = root.Find("TitleImg").GetComponent<Image>();
        time = root.Find("Time").GetComponent<TextMeshProUGUI>();
        bonus = root.Find("Bonus").GetComponent<TextMeshProUGUI>();
    }
}

public class JackpotRecordInfo
{
    public string user_id;
    public int earn_credit;
    public int earn_money;
    public long accept_time;
    public int jackpot_id;
}
