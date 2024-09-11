using BagelCode;
using SlotMaker;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

public class NewJackpotDialog : MonoBehaviour
{
    private ContextButton backBtn;
    private ContextButton recordBtn;
    private List<Jackpot> jackpots;
    private List<TextMeshProUGUI> jackpotTxtList = new List<TextMeshProUGUI>();

    void Start()
    {
        backBtn = transform.Find("Anchors/Top/BackBtn").GetComponent<ContextButton>();
        recordBtn = transform.Find("Anchors/Top/RecordBtn").GetComponent<ContextButton>();

        jackpotTxtList.Add(transform.Find("Anchors/Grand/Text").GetComponent<TextMeshProUGUI>());
        jackpotTxtList.Add(transform.Find("Anchors/Major/Text").GetComponent<TextMeshProUGUI>());
        jackpotTxtList.Add(transform.Find("Anchors/Minor/Text").GetComponent<TextMeshProUGUI>());
        jackpotTxtList.Add(transform.Find("Anchors/Minin/Text").GetComponent<TextMeshProUGUI>());


        backBtn.UpdateContext();
        recordBtn.UpdateContext();
        backBtn.AddListenerOnClick(OnBackBtnClick);
        recordBtn.AddListenerOnClick(OnRecordBtnClick);

        InitJackpot();
    }

    private void InitJackpot()
    {
        jackpots = MainBlackboard.Get().GetValue<List<Jackpot>>("LobbyJackpot");
        for (int i = 0; i < jackpots.Count; i++)
            jackpotTxtList[i].text = GetNumStr(jackpots[i].total_bonus_count);
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

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void OnBackBtnClick(ContextElement context)
    {

        EventSender.SendGlobalEvent("OnLeaveNewJackpotDialog");
        Destroy(gameObject);
    }

    private void OnRecordBtnClick(ContextElement context)
    {

    }
}
