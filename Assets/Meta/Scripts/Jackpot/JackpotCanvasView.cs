using Newtonsoft.Json;
using ParadoxNotion;
using SimpleJSON;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JackpotCanvasView : MonoBehaviour
{
    private List<JackpotView> jackpotViews = new List<JackpotView>();
    private bool canUpdate = true;
    private GameObject effect;
    private Coroutine effectCorountine;
    private GameObject winTips;
    private List<GameObject> titleList = new List<GameObject>();
    private TextMeshProUGUI content;

    private void Awake()
    {
        var trans = transform.Find("Anchor/JackpotParent");
        for (int i = 0; i < trans.childCount; i++)
            jackpotViews.Add(trans.GetChild(i).GetComponent<JackpotView>());
        effect = transform.Find("Anchor/Effect").gameObject;
        winTips = transform.Find("Anchor/WinTips").gameObject;
        var titleTrans = winTips.transform.Find("Title");
        for (int i = 0; i < titleTrans.childCount; i++)
            titleList.Add(titleTrans.GetChild(i).gameObject);
        content = winTips.transform.Find("content").GetComponent<TextMeshProUGUI>();
        MessageDispatcher.Register("SetJackpot", OnSetJackpot);
        MessageDispatcher.Register("UpdateJackpot", OnUpdateJackpot);
        MessageDispatcher.Register(RPCName.winGameBonus, OnWinGameBounus);

        content.text = "123";
    }

    private void OnDestroy()
    {
        MessageDispatcher.UnRegister("SetJackpot", OnSetJackpot);
        MessageDispatcher.UnRegister("UpdateJackpot", OnUpdateJackpot);
        MessageDispatcher.UnRegister(RPCName.winGameBonus, OnWinGameBounus);
    }

    private void OnSetJackpot(EventData data)
    {
        List<Jackpot> jackpots = JsonConvert.DeserializeObject<List<Jackpot>>(data.value.ToString());
        for (int i = 0; i < jackpotViews.Count; i++)
            jackpotViews[i].SetJackpot(jackpots[i].total_bonus_count);
    }

    private void OnUpdateJackpot(EventData data)
    {
        if (!canUpdate) return;
        List<Jackpot> jackpots = JsonConvert.DeserializeObject<List<Jackpot>>(data.value.ToString());
        for (int i = 0; i < jackpotViews.Count; i++)
            jackpotViews[i].ScrollTo(jackpots[i].total_bonus_count);
    }

    private void OnWinGameBounus(EventData eventData)
    {
        Debug.LogError("call OnWinGameBounus");
        var data = eventData.value as JSONNode;
        if(!data.HasKey("win_result_list")) return;
        canUpdate = false;
        var winResultList = JsonConvert.DeserializeObject<List<WinResult>>(data["win_result_list"].ToString());

        string userId = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "me/userId").value;
        for (int i = 0; i < winResultList.Count; i++)
        {
            var winResult = winResultList[i];
            if (winResult.user_id == userId)
                ShowWinJackpot(winResult);
            else
                DispatchWinJackpot(winResult);
        }
    }

    private void ShowWinJackpot(WinResult winResult)
    {
        ShowEffect();
        var jackpotView = jackpotViews[winResult.bonus_id - 1];
        jackpotView.jackpot = jackpotView.jackpot - winResult.single_reward > 0 ? jackpotView.jackpot - winResult.single_reward : 0;
        jackpotView.SetJackpot(jackpotView.jackpot);
    }

    private void DispatchWinJackpot(WinResult winResult)
    {
        var jackpotView = jackpotViews[winResult.bonus_id - 1];
        jackpotView.jackpot = jackpotView.jackpot - winResult.single_reward > 0 ? jackpotView.jackpot - winResult.single_reward : 0;
        jackpotView.SetJackpot(jackpotView.jackpot);
        ShowWinTips(winResult);
    }

    private void ShowWinTips(WinResult winResult)
    {
        int index = winResult.bonus_id - 1;
        winTips.SetActive(true);
        titleList[index].SetActive(true);
        string titleStr = "";
        switch (index)
        {
            case 0: titleStr = "grand"; break;
            case 1: titleStr = "mega"; break;
            case 2: titleStr = "minor"; break;
            case 3: titleStr = "mini"; break;
        }
        content.text = $"{winResult.nick_name} win {titleStr} jackpot $";
        content.text += GetNumStr(winResult.single_reward);
    }

    private string GetNumStr(int value)
    {
        string str;
        int tempValue = value % 100;
        string point;
        if (tempValue < 10)
            point = "0" + tempValue;
        else
            point = tempValue.ToString();
        value /= 100;
        if (value > 1000)
        {
            str = $"{(value / 1000)},";
            str += value % 1000;
        }
        else
            str = value.ToString();
        str += $".{point}";
        return str;
    }

    private void ShowEffect()
    {
        if (effectCorountine != null)
            StopCoroutine(effectCorountine);
        effectCorountine = StartCoroutine(ShowEffectEnumerator());
    }

    private IEnumerator ShowEffectEnumerator()
    {
        canUpdate = false;
        effect.SetActive(true);
        yield return new WaitForSeconds(5);
        effect.SetActive(false);
        winTips.SetActive(false);
        titleList.ForEach(t => t.SetActive(false));
        content.text = "";
        canUpdate = true;
    }
}

public class Jackpot
{
    public int total_bonus_count; 
}

public class WinResult
{
    public string user_id;
    public string nick_name;
    public int single_reward;
    public int bonus_id;
}
