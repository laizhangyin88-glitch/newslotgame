using BagelCode;
using BagelCode.ClientModels;
using GameUtil;
using NodeCanvas.Framework;
using ParadoxNotion;
using SimpleJSON;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SlotMaker.IoC.Strategy.Tween.TweenEase;

public class DoorController : MonoBehaviour
{
    private Transform listParent;

    public GameObject selectItem;

    public GameObject[] GameList;

    private Text TimeTxt;

    private int currentCountDown;

    private Text totalWinValueTxt;

    private Text BetValueTxt;

    private long currentBet;

    private long rewardValue;

    private LoopTimer _loopTimer;

    private List<DoorSelectItem> doorSelectItems = new List<DoorSelectItem>();

    private bool autoSpinValue = false;

    public bool isSelect = false;
    private void SetAutoSpinFalse()
    {
        var autoSpin = BlackboardUtils.FindVariable<bool>("./autoSpin");
        if (autoSpin != null && autoSpin.value)
        {
            autoSpinValue = autoSpin.value;
            BlackboardUtils.FindVariable<bool>("./autoSpin").value = false;
        }
    }

    public void ResetAutoSpin()
    {
        BlackboardUtils.FindVariable<bool>("./autoSpin").value = autoSpinValue;
    }

    public void OnStart()
    {
        Debug.LogError("显示选中小游戏界面..................");
        SetAutoSpinFalse();
        currentBet = BlackboardUtils.FindVariable<long>("./betCredit").value;
        isSelect = false;

        MiniGameDataManagers.StopBGM();  
        GSManager.Instance.GetHandler("door_A15").Play();

        listParent = transform.Find("list");
        BetValueTxt = transform.Find("bet/BetValue").GetComponent<Text>();
        totalWinValueTxt = transform.Find("Img/totalWinValue").GetComponent<Text>();
        TimeTxt = transform.Find("time/TimeTxt").GetComponent<Text>();

        totalWinValueTxt.text = "0";

        BetValueTxt.text = currentBet.ToString("N0");

        InitSelectItem();

        StartCountDown();
    }

    private void StartCountDown()
    {
        _loopTimer?.Cancel();
        currentCountDown = 5;
        TimeTxt.text = currentCountDown.ToString();
        GSManager.Instance.GetHandler("cound_down_C01").Play();
        _loopTimer = this.LoopAction(1, (time) =>
        {
            currentCountDown--;
            TimeTxt.text = currentCountDown.ToString();
            GSManager.Instance.GetHandler("cound_down_C01").Play();
            if (currentCountDown <= 0)
            {
                TimeTxt.text = "";
                AutoSelect();
                _loopTimer?.Cancel();
            }
        });
    }
     
    private void AutoSelect()
    {
        if (isSelect) return;
        if(doorSelectItems.Count > 0)
        {
            _loopTimer?.Cancel();
            int index = Random.Range(0, doorSelectItems.Count);
            doorSelectItems[index].OnClickBtn();
        }
    }

    private void InitSelectItem()
    {
        for (int i = 0; i < 3; i++)
        {
            if(selectItem != null)
            {
                GameObject go = Instantiate(selectItem);
                go.transform.SetParent(listParent, false);
                var select = go.GetComponent<DoorSelectItem>();
                select.SetIndex(i);
                select._DoorController = this;
                doorSelectItems.Add(select);
            }
        }
    }

    public void SetClickReward()
    {
        _loopTimer?.Cancel();
        //var value = BlackboardUtils.GetOrCreateVariable<long>(ContentBlackboard.Get(), "jackpot_reward3");
        //totalWinValueTxt.text = value.value.ToString("N0");
        //rewardValue = value.value;
        Variable<Blackboard> spinBB = ContentBlackboard.Get().GetVariable<Blackboard>("spin");
        string responseNew = spinBB.value.GetValue<string>("responseNew");
        JSONNode res = JSONNode.Parse(responseNew);
        if (res["game_result"]["jackpot_info"]["jackpot_reward3"] != null)
        {
            long value = res["game_result"]["jackpot_info"]["jackpot_reward3"].AsLong;
            if (value > 0) 
            {
                totalWinValueTxt.text = ((long)value).ToString("N0");
                long temp = BlackboardUtils.FindVariable<long>("/me/credit").value;
                BlackboardQueryUtils.SetMyCredit(temp + value);
                rewardValue = value;
                MessageDispatcher.Dispatch("OnCreditEvent", new EventData<bool>("UpdateNaviCredit", true));

            }
        }
    }
     
    public void PlayMiniGame(int index)
    {
        this.DelayAction(0.5f, () =>
        {
            switch (index)
            {
                case 0:
                    var controller0 = GameList[index].GetComponent<FruitPartyMiniGameController1>();
                    controller0.gameObject.SetActive(true);
                    controller0.OnStart();
                    break;
                case 1:
                    var controller1 = GameList[index].GetComponent<FruitPartyMiniGameController2>();
                    controller1.gameObject.SetActive(true);
                    controller1.OnStart();
                    break;
                case 2:
                    var controller2 = GameList[index].GetComponent<FruitPartyMiniGameController3>();
                    controller2.gameObject.SetActive(true);
                    controller2.OnStart();
                    break;
            }
            Clear();
            GSManager.Instance.GetHandler("door_A15").Clear();
        });
    }

    private void Clear()
    {
        this.gameObject.SetActive(false);
        _loopTimer?.Cancel();
        for (int i = 0; i < doorSelectItems.Count; i++)
        {
            Destroy(doorSelectItems[i].gameObject);
        }
        doorSelectItems.Clear();
    }
}



