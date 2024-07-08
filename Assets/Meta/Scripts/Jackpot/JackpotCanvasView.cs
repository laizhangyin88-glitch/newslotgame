using Newtonsoft.Json;
using ParadoxNotion;
using SlotMaker;
using System.Collections.Generic;
using UnityEngine;

public class JackpotCanvasView : MonoBehaviour
{
    List<JackpotView> jackpotViews = new List<JackpotView>();

    private void Awake()
    {
        var trans = transform.Find("Anchor/BG");
        for (int i = 0; i < trans.childCount; i++)
            jackpotViews.Add(trans.GetChild(i).GetComponent<JackpotView>());
        MessageDispatcher.Register("SetJackpot", OnSetJackpot);
        MessageDispatcher.Register("UpdateJackpot", OnUpdateJackpot);
    }

    private void OnDestroy()
    {
        MessageDispatcher.UnRegister("SetJackpot", OnSetJackpot);
        MessageDispatcher.UnRegister("UpdateJackpot", OnUpdateJackpot);
    }

    private void OnSetJackpot(EventData data)
    {
        List<Jackpot> jackpots = JsonConvert.DeserializeObject<List<Jackpot>>(data.value.ToString());
        for (int i = 0; i < jackpotViews.Count; i++)
            jackpotViews[i].SetJackpot(jackpots[i].total_bonus_count);
    }

    private void OnUpdateJackpot(EventData data)
    {
        List<Jackpot> jackpots = JsonConvert.DeserializeObject<List<Jackpot>>(data.value.ToString());
        for (int i = 0; i < jackpotViews.Count; i++)
            jackpotViews[i].ScrollTo(jackpots[i].total_bonus_count);
    }
}

public class Jackpot
{
    public int total_bonus_count; 
}
