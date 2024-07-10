using Newtonsoft.Json;
using NodeCanvas.Framework;
using ParadoxNotion;
using SimpleJSON;
using SlotMaker;
using System.Collections.Generic;
using UnityEngine;

public class JackpotCanvasView : MonoBehaviour
{
    private List<JackpotView> jackpotViews = new List<JackpotView>();
    private bool canUpdate;

    public void TestSetJackpot()
    {
        jackpotViews[0].SetJackpot(500);
    }

    public void TestUpdateJackpot()
    {
        jackpotViews[0].ScrollTo(1000);
    }

    private void Awake()
    {
        var trans = transform.Find("Anchor/JackpotParent");
        for (int i = 0; i < trans.childCount; i++)
            jackpotViews.Add(trans.GetChild(i).GetComponent<JackpotView>());
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
        var data = eventData.value as JSONNode;
        if(!data.HasKey("win_result_list")) return;
        canUpdate = false;
        var winJackpotData = JsonConvert.DeserializeObject<WinJackpotData>(data["win_result_list"].ToString());

        var meBB = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "me");
        if (meBB == null) return;
        int userId = BlackboardUtils.FindVariable<int>(meBB.value, "userId").value;
        if (int.TryParse(winJackpotData.userId, out int winJackpotId) && userId == winJackpotId)
            ShowWinJackpot(winJackpotData.singleReward);
        else
            DispatchWinJackpot(winJackpotData.singleReward);
    }

    private void ShowWinJackpot(int singleReward)
    {
        
    }

    private void DispatchWinJackpot(int singleReward)
    {

    }
}

public class Jackpot
{
    public int total_bonus_count; 
}

public class WinJackpotData
{
    public int singleReward;
    public string userId;
    public string nickName;
}
