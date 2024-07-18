using BagelCode;
using BagelCode.Tasks.Actions.Contents;
using GameUtil;
using NodeCanvas.Framework;
using ParadoxNotion;
using SimpleJSON;
using Sirenix.OdinInspector;
using SlotMaker;
using SlotMaker.Slots.Tasks.Actions.Win;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public enum BoyStealHarpState
{
    None = -1,
    ChoseGift,
    Stealing,
    StealFinish,
    TimerUp,
}
public class MiniGame2 : MonoBehaviour
{
    BoyStealHarpState boyState = BoyStealHarpState.None;
    Transform root;
    Animator ani_Man, ani_Boy, ani_Gift1, ani_Gift2, ani_Gift3, ani_Gift4, ani_Gift5, ani_Gift6, ani_Gift7;
    Button btn_Gift1, btn_Gift2, btn_Gift3, btn_Gift4, btn_Gift5, btn_Gift6, btn_Gift7;
    TextMeshProUGUI betNum, winNum,countDownText; 
    Dictionary<int, Vector2> _dicPos = new Dictionary<int, Vector2>()
    {
        [1] = new Vector2(-224, 333),
        [2] = new Vector2(-196, 183),
        [3] = new Vector2(-136, 54),
        [4] = new Vector2(0, 54),
        [5] = new Vector2(400, 265),
        [6] = new Vector2(270, 196),
        [7] = new Vector2(295, 17),
    };


    Dictionary<int, Transform> giftContents = new Dictionary<int, Transform> {};
    Dictionary<int, Text> giftContentTexts = new Dictionary<int, Text> { };
    Dictionary<int, Animator> giftAtors = new Dictionary<int, Animator> { };

    List<int> RecIndexList = new List<int>();

    List<DelayTimer> _timers = new List<DelayTimer>();
    LoopTimer _countDownTimer;

    long _totalWin = 0;
    void Awake()
    {
        root = transform.Find("Animator/Anchor");
        ani_Man = root.Find("man/Animator").GetComponent<Animator>();
        ani_Boy = root.Find("boy").GetComponent<Animator>();
        ani_Gift1 = root.Find("gifts/gift1").GetComponent<Animator>();
        ani_Gift2 = root.Find("gifts/gift2").GetComponent<Animator>();
        ani_Gift3 = root.Find("gifts/gift3").GetComponent<Animator>();
        ani_Gift4 = root.Find("gifts/gift4").GetComponent<Animator>();
        ani_Gift5 = root.Find("gifts/gift5").GetComponent<Animator>();
        ani_Gift6 = root.Find("gifts/gift6").GetComponent<Animator>();
        ani_Gift7 = root.Find("gifts/gift7").GetComponent<Animator>();
        giftAtors = new Dictionary<int, Animator>
        {
            [1] = ani_Gift1,
            [2] = ani_Gift2,
            [3] = ani_Gift3,
            [4] = ani_Gift4,
            [5] = ani_Gift5,
            [6] = ani_Gift6,
            [7] = ani_Gift7,
        };

        btn_Gift1 = ani_Gift1.GetComponent<Button>();
        btn_Gift2 = ani_Gift2.GetComponent<Button>();
        btn_Gift3 = ani_Gift3.GetComponent<Button>();
        btn_Gift4 = ani_Gift4.GetComponent<Button>();
        btn_Gift5 = ani_Gift5.GetComponent<Button>();
        btn_Gift6 = ani_Gift6.GetComponent<Button>();
        btn_Gift7 = ani_Gift7.GetComponent<Button>();
        betNum = root.Find("betWin/bet/betNum").GetComponent<TextMeshProUGUI>();
        winNum = root.Find("betWin/win/winNum").GetComponent<TextMeshProUGUI>();
        countDownText = root.Find("countDown").GetComponent<TextMeshProUGUI>();

        giftContents = new Dictionary<int, Transform>()
        {
            [1] = ani_Gift1.transform.GetChild(0),
            [2] = ani_Gift2.transform.GetChild(0),
            [3] = ani_Gift3.transform.GetChild(0),
            [4] = ani_Gift4.transform.GetChild(0),
            [5] = ani_Gift5.transform.GetChild(0),
            [6] = ani_Gift6.transform.GetChild(0),
            [7] = ani_Gift7.transform.GetChild(0),

        };

        giftContentTexts = new Dictionary<int, Text>()
        {
            [1] = ani_Gift1.transform.GetChild(0).GetChild(0).GetComponent<Text>(),
            [2] = ani_Gift2.transform.GetChild(0).GetChild(0).GetComponent<Text>(),
            [3] = ani_Gift3.transform.GetChild(0).GetChild(0).GetComponent<Text>(),
            [4] = ani_Gift4.transform.GetChild(0).GetChild(0).GetComponent<Text>(),
            [5] = ani_Gift5.transform.GetChild(0).GetChild(0).GetComponent<Text>(),
            [6] = ani_Gift6.transform.GetChild(0).GetChild(0).GetComponent<Text>(),
            [7] = ani_Gift7.transform.GetChild(0).GetChild(0).GetComponent<Text>(),
        };

        AddEvent();
    }

    List<int> lstScore = new List<int>();
    void OnEnable()
    {
        boyState = BoyStealHarpState.None;
        foreach (var item in giftContents)
        {
            item.Value.gameObject.SetActive(false);
        }
        RecIndexList = new List<int>();
        //RefreshGiftState(false);
        ClearTimer();
        ani_Boy.gameObject.SetActive(false);

        betNum.text = $"{BlackboardUtils.FindVariable<long>("./totalBetCredit").value}";
        _totalWin = 0;
        winNum.text = $"{_totalWin}";

        //ani_Man.Play("Default");


        long betCredit = BlackboardUtils.GetOrCreateVariable<long>(null, "./totalBetCredit").value;
        int selectLine = BlackboardUtils.GetOrCreateVariable<int>(null, "./gameNew/selectLine").value;

        Dictionary<string, object> req = new Dictionary<string, object>
        {
                {"bet",betCredit/selectLine},
                { "win_line_count",selectLine},
                { "jackpot_game_index",2}
        };


        NetManager.Instance.Post(RPCName.newClaimBonus, req,
        (res) =>
        {
            string resStr = res.ToString();
            Debug.Log(resStr);

            lstScore = new List<int>();
            foreach (JSONNode item in res["game_result"]["jackpot_game_result_list"])
            {
                lstScore.Add((int)item);
            }

            Debug.LogWarning($"Count = {lstScore.Count}");

            Blackboard bonusBB = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(ContentBlackboard.Get(), "bonus");
            BlackboardUtils.SetOrCreateValue(bonusBB, "responseNew", resStr);
            BlackboardUtils.SetOrCreateValue(bonusBB, "bonusName", "chili");

            //BeginBonusNew.CreatBonus(resStr, "chili");

            StartSteal();
        },
        (error) =>
        {
            GlobalErrorHandler.GlobalError(error);
        });


    }

    public void ClearTimer()
    {
        if (_timers != null && _timers.Count >0)
        {
            foreach (var timer in _timers)
            {
                timer?.Cancel();
            }
            _timers?.Clear();
        }
    }

    void OnDisable()
    {
        RecIndexList?.Clear();
        ClearTimer();
        _countDownTimer?.Cancel();
        _countDownTimer = null;
    }

    private void OnDestroy()
    {
        RemoveEvent();    
    }

    void AddEvent()
    {
        btn_Gift1.onClick.AddListener(ClickGift1);
        btn_Gift2.onClick.AddListener(ClickGift2);
        btn_Gift3.onClick.AddListener(ClickGift3);
        btn_Gift4.onClick.AddListener(ClickGift4);
        btn_Gift5.onClick.AddListener(ClickGift5);
        btn_Gift6.onClick.AddListener(ClickGift6);
        btn_Gift7.onClick.AddListener(ClickGift7);
    }

    void ClickGift1() { OnClickGift(1); }
    void ClickGift2() { OnClickGift(2); }
    void ClickGift3() { OnClickGift(3); }
    void ClickGift4() { OnClickGift(4); }
    void ClickGift5() { OnClickGift(5); }
    void ClickGift6() { OnClickGift(6); }
    void ClickGift7() { OnClickGift(7); }
    void RemoveEvent()
    {
        btn_Gift1.onClick.RemoveListener(ClickGift1);
        btn_Gift2.onClick.RemoveListener(ClickGift2);
        btn_Gift3.onClick.RemoveListener(ClickGift3);
        btn_Gift4.onClick.RemoveListener(ClickGift4);
        btn_Gift5.onClick.RemoveListener(ClickGift5);
        btn_Gift6.onClick.RemoveListener(ClickGift6);
        btn_Gift7.onClick.RemoveListener(ClickGift7);
    }
    void OnClickGift(int index)
    {
        if (boyState != BoyStealHarpState.ChoseGift)
            return;
        boyState = BoyStealHarpState.Stealing;
        _countDownTimer?.Cancel();
        _countDownTimer = null;

        var score =  0;
        if (lstScore.Count>0)
        {
            score = lstScore[0];
            lstScore.RemoveAt(0);
        }

        if ( _dicPos.TryGetValue(index, out var pos) && !RecIndexList.Contains(index))
        {
            RecIndexList.Add(index);
            ani_Boy.gameObject.SetActive(true);
            ani_Boy.transform.localPosition = pos;
            ani_Boy.Play("ClimbDown");
            RefreshGiftState(false);

            var timer0 = TimerExtensions.DelayAction(this, 1.5f, () =>
            {
                Animator gifAtor = giftAtors[index];  

                gifAtor.Play("Open");

                giftContents[index].gameObject.SetActive(true);
                giftContents[index].GetComponent<Animator>().Play(lstScore.Count == 0 ? "GiftFail" : $"GiftSuccess{Random.Range(1, 6)}");
                giftContentTexts[index].text = score.ToString();

                var timer2 = TimerExtensions.DelayAction(this, 2.2f, () =>
                {
                    giftContents[index].gameObject.SetActive(false);

                    _totalWin += score;
                    winNum.text = $"{_totalWin}";

                    ani_Boy.Play("ClimbUp");
                    boyState = BoyStealHarpState.StealFinish;
                    var timer3 = TimerExtensions.DelayAction(this, 1.6f, () =>
                    {
                        ani_Boy.gameObject.SetActive(false);
                        if (lstScore.Count > 0 && boyState != BoyStealHarpState.TimerUp)
                        {
                            StartSteal();
                        }
                    });
                    _timers.Add(timer3);


                    if (lstScore.Count == 0)
                    {
                        ani_Man.Play("Query");
                        boyState = BoyStealHarpState.TimerUp;
                        var timer4 = TimerExtensions.DelayAction(this, 4f, () =>
                        {
                            MessageDispatcher.Dispatch("OnContentUIEvent", new EventData("BSTMiniGameFinish")); //发给脚本
                            EventSender.SendGlobalEvent(new EventData("BSTMiniGameFinish")); //发给NodeCanvas （类型：OnCustomEvent）
                        });
                        _timers.Add(timer4);
                    }


                });
                _timers.Add(timer2);

            });
            _timers.Add(timer0);
        }
    }
    void RefreshGiftState(bool isHighLight)
    {
        //Debug.LogError($"【TEST】: RefreshGiftState == {isHighLight}");
        foreach (var item  in giftAtors)
        {
            if (!RecIndexList.Contains(item.Key))
            {
                item.Value.Play(isHighLight == true?"HighLight" : "Default");
            }
        }
    }



    void StartSteal()
    {
        RefreshGiftState(true);
        boyState = BoyStealHarpState.ChoseGift;
       
        countDownText.text = "5";
        var timeVal = 5;
        _countDownTimer = TimerExtensions.LoopAction(this, 1, (a) =>
        {
            timeVal--;
            if (timeVal < 0)
            {
                var index = 1;
                for (int i = 1; i <= 7; i++)
                {
                    if (!RecIndexList.Contains(i))
                    {
                        index = i;
                        break;
                    }
                }
                OnClickGift(index);
                return;
            }
            else
            {
                countDownText.text = timeVal.ToString();
            }
        });
    }
}
