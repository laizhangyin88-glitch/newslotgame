using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using ParadoxNotion;
using SimpleJSON;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class OSA_LobbyBonus : OSA<BaseParamsWithPrefab, LobbyBonusItemViewsHolder>
{
    public List<Sprite> images = new List<Sprite>();
    private List<int> jackpots;
    private Coroutine coroutine;
    private int index;

    protected override void Awake()
    {
        base.Awake();
        InitJackpot();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        index = 0;
        MessageDispatcher.Register("UpdateJackpot", OnUpdateJackpot);
        StartCoroutine(DelayReset(jackpots.Count));
    }

    protected override void Start()
    {
        base.Start();
    }

    private IEnumerator DelayReset(int count)
    {
        yield return new WaitForSeconds(0.15f);
        ResetItems(count);
        coroutine = StartCoroutine(ShowAni());
    }

    private IEnumerator ShowAni()
    {
        while (true)
        {
            yield return new WaitForSeconds(3.0f);
            index++;
            if (index >= jackpots.Count)
                index = 0;
            SmoothScrollTo(index, 1f);
        }
    }

    private void InitJackpot()
    {
        var lobbyJackpotScore = MainBlackboard.Get().GetValue<List<int>>("LobbyJackpotScore");
        jackpots = new List<int>(lobbyJackpotScore.Select((v) => v / 100));
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        MessageDispatcher.UnRegister("UpdateJackpot", OnUpdateJackpot);
        if (coroutine != null)
            StopCoroutine(coroutine);
    }

    private void OnUpdateJackpot(EventData data)
    {
        //var jsonData = data.value as JSONNode;
        //List<int> jacks = new List<int>();
        //float offset = MainBlackboard.Get().GetValue<int>("OutCreditRate");
        //for (int i = 0; i < jsonData["remain_jackpot_list"].Count; i++)
        //{
        //    var temp = (float)jsonData["remain_jackpot_list"][i];
        //    temp *= offset;
        //    var str = temp.ToString();
        //    str = str.Split('.')[0];
        //    jacks.Add(int.Parse(str));
        //}

        //jackpots.Reverse();

        List<int> lobbyJackpotScore = BlackboardUtils.FindValue<List<int>>(MainBlackboard.Get(), "LobbyJackpotScore");
        if (lobbyJackpotScore == null)
            return;

        jackpots = new List<int>(lobbyJackpotScore.Select((v) => v / 100));
    }

    protected override LobbyBonusItemViewsHolder CreateViewsHolder(int itemIndex)
    {
        var instance = new LobbyBonusItemViewsHolder();

        instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);

        return instance;
    }

    protected override void UpdateViewsHolder(LobbyBonusItemViewsHolder newOrRecycled)
    {
        var jackpotScore = jackpots[newOrRecycled.ItemIndex];
        newOrRecycled.icon.sprite = images[newOrRecycled.ItemIndex];
        newOrRecycled.text.text = jackpotScore.ToString();
    }

    private string GetNumStr(int num)
    {
        string str = "";
        string temp = (num % 10).ToString();
        num /= 10;
        temp = (num % 10).ToString() + temp;
        num /= 10;
        if (num > 999)
        {
            str += num / 1000;
            str += ",";
            num %= 1000;
            for (int i = 0; i < 3 - num.ToString().Length; i++)
                str += '0';
            str += num;
            str += ".";
            str += temp;
        }
        else
        {
            str += num % 1000;
            str += ".";
            str += temp;
        }
        return str;
    }
}

public class LobbyBonusItemViewsHolder : BaseItemViewsHolder
{
    public Image icon;
    public Text text;

    public override void CollectViews()
    {
        base.CollectViews();
        icon = root.Find("Icon").GetComponent<Image>();
        text = root.Find("Text").GetComponent<Text>();
    }
}
