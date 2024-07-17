using Newtonsoft.Json;
using NodeCanvas.Framework;
using ParadoxNotion;
using SimpleJSON;
using SimpleJSON;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.PlayerLoop.PreUpdate;

public class JackpotCanvasView : MonoBehaviour
{
    private List<JackpotView> jackpotViews = new List<JackpotView>();
    private bool canUpdate = true;
    private GameObject effect;
    private Coroutine effectCorountine;

    public void TestSetJackpot()
    {
        jackpotViews[3].SetJackpot(50);
    }

    //public void testupdatejackpot()
    //{
    //}

    public void TestUpdateJackpot()
    {
        jackpotViews[3].ScrollTo(100);
    }

    private void Awake()
    {
        var trans = transform.Find("Anchor/JackpotParent");
        for (int i = 0; i < trans.childCount; i++)
            jackpotViews.Add(trans.GetChild(i).GetComponent<JackpotView>());
        effect = transform.Find("Anchor/Effect").gameObject;
        MessageDispatcher.Register("SetJackpot", OnSetJackpot);
        MessageDispatcher.Register("UpdateJackpot", OnUpdateJackpot);
        MessageDispatcher.Register(RPCName.winGameBonus, OnWinGameBounus);
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
        ShowEffect();
        var jackpotView = jackpotViews[winResult.bonus_id - 1];
        jackpotView.jackpot -= winResult.single_reward;
        jackpotView.SetJackpot(jackpotView.jackpot);
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
    public int bonusId;
}
