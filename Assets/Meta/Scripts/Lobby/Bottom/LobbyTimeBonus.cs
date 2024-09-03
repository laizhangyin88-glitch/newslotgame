using BagelCode.ClientModels;
using BagelCode;
using Newtonsoft.Json;
using ParadoxNotion;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class LobbyTimeBonus : MonoBehaviour
{
    public List<Sprite> images = new List<Sprite>();
    private Text text;
    private Image icon;
    private List<Jackpot> jackpots;
    private int index;
    private Coroutine changeCoroutine;
    private float timers = 5;
    private ContextButton contextButton => GetComponent<ContextButton>();

    void Start()
    {
        text = transform.Find("Anchors/Text").GetComponent<Text>();
        icon = transform.Find("Anchors/Icon").GetComponent<Image>();
        MessageDispatcher.Register("UpdateJackpot", OnUpdateJackpot);
        InitJackpot();

        contextButton.UpdateContext();

        contextButton.AddListenerOnClick((context) => EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_ENTER_JACKPOT_DIALOG));
    }

    private void InitJackpot()
    {
        jackpots = MainBlackboard.Get().GetValue<List<Jackpot>>("LobbyJackpot");
        icon.sprite = images[index];
        text.text = GetNumStr(jackpots[index].total_bonus_count);
        changeCoroutine = StartCoroutine(JackpotChangeTimers());
    }

    private void OnUpdateJackpot(EventData data)
    {
        var jsonData = data.value as JSONNode;
        List<int> jacks = new List<int>();
        for (int i = 0; i < jsonData["remain_jackpot_list"].Count; i++)
        {
            var temp = (float)jsonData["remain_jackpot_list"][i];
            jacks.Add(int.Parse((temp * 100).ToString()));
        }

        jackpots = new List<Jackpot>();
        jacks.ForEach(j =>
        {
            jackpots.Add(new Jackpot { total_bonus_count = j });
        });
        jackpots.Reverse();
    }

    private IEnumerator JackpotChangeTimers()
    {
        while (true)
        {
            yield return new WaitForSeconds(timers);
            index = index + 1 > 3 ? 0 : index + 1;
            icon.sprite = images[index];
            text.text = GetNumStr(jackpots[index].total_bonus_count);
        }
    }

    private string GetNumStr(int num)
    {
        string str = "$";
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

    private void OnDestroy()
    {
        MessageDispatcher.UnRegister("UpdateJackpot", OnUpdateJackpot);
        if (changeCoroutine != null)
            StopCoroutine(changeCoroutine);
    }
}
