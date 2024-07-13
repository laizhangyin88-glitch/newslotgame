using BagelCode;
using BagelCode.Tasks.Actions.Contents;
using GameUtil;
using NodeCanvas.Framework;
using ParadoxNotion;
using SimpleJSON;
using Sirenix.OdinInspector;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public enum BoyStealTeasureState
{
    None = -1,
    ChoseTeasure,
    Stealing,
    StealFinish,
    TimerUp,
}
public class MiniGame1 : MonoBehaviour
{
    BoyStealTeasureState boyState = BoyStealTeasureState.None;
    Transform root;
    Animator ani_ManSleep, ani_Box1, ani_Box2, ani_Box3, ani_TimeUp1, ani_TimeUp2, ani_TimeUp3, ani_Boy;
    Button btn_Box1, btn_Box2, btn_Box3;
    TextMeshProUGUI betNum, winNum, scoreNum, countDownText;
    List<DelayTimer> _timers;
    float endTime = 30;
    bool isTimeUp = false;
    LoopTimer _countDownTimer;
    void Awake()
    {
        root = transform.Find("Animator/Anchor");
        ani_ManSleep = root.Find("manSleep/Animator").GetComponent<Animator>();
        ani_Box1 = root.Find("boxs/box1").GetComponent<Animator>();
        ani_Box2 = root.Find("boxs/box2").GetComponent<Animator>();
        ani_Box3 = root.Find("boxs/box3").GetComponent<Animator>();
        ani_TimeUp1 = ani_Box1.transform.Find("TimeUp").GetComponent<Animator>();
        ani_TimeUp2 = ani_Box2.transform.Find("TimeUp").GetComponent<Animator>();
        ani_TimeUp3 = ani_Box3.transform.Find("TimeUp").GetComponent<Animator>();
        ani_Boy = root.transform.Find("boxs/boy").GetComponent<Animator>();
        scoreNum = ani_Boy.transform.Find("score").GetComponent<TextMeshProUGUI>();
        btn_Box1 = ani_Box1.GetComponent<Button>();
        btn_Box2 = ani_Box2.GetComponent<Button>();
        btn_Box3 = ani_Box3.GetComponent<Button>();
        betNum = root.Find("betWin/bet/betNum").GetComponent<TextMeshProUGUI>();
        winNum = root.Find("betWin/win/winNum").GetComponent<TextMeshProUGUI>();
        countDownText = root.Find("countDown").GetComponent<TextMeshProUGUI>();
    }


    /*
    List<int> typeList = new List<int>() { 1, 2, 3, 4, 5,3,2,4,5,1,2,4,5,3,2,5,1 };
    Dictionary<int, int> dic_Score = new Dictionary<int, int> {
        [1] = 100,
        [2] = 200,
        [3] = 300,
        [4] = 400,
        [5] = 500,
    };
     */
    List<KeyValuePair<int, int>> lst_Score = new List<KeyValuePair<int, int>>();


    readonly float TIME_STEAL_TREASURE = 1f;  //treasure
    readonly float TIME_FIND_TREASURE = 1.3f; //treasure
    readonly float TIME_WARK = 2.2f;

    void OnEnable()
    {
        boyState = BoyStealTeasureState.None;

        long betCredit = BlackboardUtils.GetOrCreateVariable<long>(null, "./totalBetCredit").value;
        int selectLine = BlackboardUtils.GetOrCreateVariable<int>(null, "./gameNew/selectLine").value;

        Dictionary<string, object> req = new Dictionary<string, object>
        {
                {"bet",betCredit/selectLine},
                { "win_line_count",selectLine},
                { "jackpot_game_index",1}
        };


        NetManager.Instance.Post(RPCName.newClaimBonus, req,
        (res) =>
        {
            string resStr = res.ToString();
            Debug.Log(resStr);

            lst_Score = new List<KeyValuePair<int, int>>();

            foreach (JSONNode item in res["game_result"]["jackpot_game_result_list"])
            {
                lst_Score.Add(new KeyValuePair<int,int>(Random.Range(1, 5) , (int)item));
            }

            Debug.LogError($"Count = {lst_Score.Count}");

            Blackboard bonusBB = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(ContentBlackboard.Get(), "bonus");
            BlackboardUtils.SetOrCreateValue(bonusBB, "responseNew", resStr);
            BlackboardUtils.SetOrCreateValue(bonusBB, "bonusName", "treasure");
            //BeginBonusNew.CreatBonus(resStr, "treasure");

            StartSteale();
        },
        (error) =>
        {
            GlobalErrorHandler.GlobalError(error);
        });


    }


    private void StartSteale()
    {
        isTimeUp = false;
        _timers = new List<DelayTimer>();
        AddEvent();
        RefreshBetWin();
        ani_Box1.Play("CanOpen");
        ani_Box2.Play("CanOpen");
        ani_Box3.Play("CanOpen");

        ani_TimeUp1.gameObject.SetActive(false);
        ani_TimeUp2.gameObject.SetActive(false);
        ani_TimeUp3.gameObject.SetActive(false);
        ani_Boy.gameObject.SetActive(false);
        scoreNum.gameObject.SetActive(false);
        winNum.text = "0";
        countDownText.text = "5";
        StartCountDown();
        boyState = BoyStealTeasureState.ChoseTeasure;
    }



    void OnDisable()
    {
        RemoveEvent();
        ClearTimer();
        _countDownTimer?.Cancel();
        _countDownTimer = null;
    }


    void ClearTimer()
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



    void AddEvent()
    {
        btn_Box1.onClick.AddListener(ClickBox1);
        btn_Box2.onClick.AddListener(ClickBox2);
        btn_Box3.onClick.AddListener(ClickBox3);
    }
    int test1;
    public int Test => test1;
    void ClickBox1() { OnClickBox(1); }
    void ClickBox2() { OnClickBox(2); }
    void ClickBox3() { OnClickBox(3); }

    void RemoveEvent()
    {
        btn_Box1.onClick.RemoveListener(ClickBox1);
        btn_Box2.onClick.RemoveListener(ClickBox2);
        btn_Box3.onClick.RemoveListener(ClickBox3);
    }
    void OnClickBox(int index)
    {

        if (boyState != BoyStealTeasureState.ChoseTeasure)
            return;
        boyState = BoyStealTeasureState.Stealing;

        _countDownTimer?.Cancel();
        _countDownTimer = null;
        Animator box = null, timeUp = null;

        endTime = lst_Score.Count * (TIME_STEAL_TREASURE + TIME_FIND_TREASURE) + TIME_WARK;
        float startTime = 1 - endTime / 60;
        Dictionary<int, float> _dicTime = new Dictionary<int, float>();
        _dicTime.Add(index, startTime);
        for (int i = 1; i <= 3; i++)
        {
            if (!_dicTime.ContainsKey(i))
            {
                var randomTime = Random.Range(1, 60);
                _dicTime.Add(i, 1 - randomTime / 60);
            }
        }
        if (index == 1)
        {
            box = ani_Box1;
            timeUp = ani_TimeUp1;
            ani_Box2.Play("Default");
            ani_Box3.Play("Default");
        }
        else if (index == 2)
        {
            box = ani_Box2;
            timeUp = ani_TimeUp2;
            ani_Box1.Play("Default");
            ani_Box3.Play("Default");

        }
        else if (index == 3)
        {
            box = ani_Box3;
            timeUp = ani_TimeUp3;
            ani_Box1.Play("Default");
            ani_Box2.Play("Default");
        }
        ani_TimeUp1.gameObject.SetActive(true);
        ani_TimeUp2.gameObject.SetActive(true);
        ani_TimeUp3.gameObject.SetActive(true);
        if (box != null && timeUp != null)
        {
            ani_Boy.transform.localPosition = new Vector2(box.transform.localPosition.x, ani_Boy.transform.localPosition.y);
            ani_Boy.gameObject.SetActive(true);
            box.Play("Open");
            timeUp.gameObject.SetActive(true);
            ani_TimeUp1.Play("Start", -1, _dicTime[1]);
            ani_TimeUp2.Play("Start", -1, _dicTime[2]);
            ani_TimeUp3.Play("Start", -1, _dicTime[3]);

            ani_Boy.Play("Walk");
            //PlayBoy();
            var timer0 = TimerExtensions.DelayAction(this, TIME_WARK, () =>
            {
                if (lst_Score.Count == 0)
                    return;
                OnPlayBoxEffect(0);
            });
            _timers.Add(timer0);

            var timer1 = TimerExtensions.DelayAction(this, endTime + 1, () =>
            {
                ClearTimer();

                isTimeUp = true;
                timeUp.gameObject.SetActive(false);
                ani_ManSleep.Play("TimeUp");
                ani_Boy.gameObject.SetActive(true);
                ani_Boy.Play("timp_up");

                var timer3 = TimerExtensions.DelayAction(this, 2f, () =>
                {
                    MessageDispatcher.Dispatch("OnContentUIEvent", new EventData("BSTMiniGameFinish")); //发给脚本
                    EventSender.SendGlobalEvent(new EventData("BSTMiniGameFinish")); //发给NodeCanvas （类型：OnCustomEvent）
                });
                _timers.Add(timer3);

            });
            _timers.Add(timer1);
            var timer2 = TimerExtensions.DelayAction(this, 1f, () =>
            {
                ani_TimeUp1.gameObject.SetActive(index == 1);
                ani_TimeUp2.gameObject.SetActive(index == 2);
                ani_TimeUp3.gameObject.SetActive(index == 3);
            });
            _timers.Add(timer2);
        }
    }


    void OnPlayBoxEffect(int index)
    {
        if (isTimeUp) return;
        boyState = BoyStealTeasureState.Stealing;

        //int type = typeList[index];
        //var score = dic_Score[type];
        int type = lst_Score[index].Key;
        var score = lst_Score[index].Value;

        string aniName = "Type" + (type + 1);
        ani_Boy.Play(aniName);
        scoreNum.text = score.ToString();
        scoreNum.gameObject.SetActive(true);
        int totalScore = int.Parse(winNum.text) + score;
        winNum.text = totalScore.ToString();
        var timer1 = TimerExtensions.DelayAction(this, TIME_STEAL_TREASURE, () =>
        {
            scoreNum.gameObject.SetActive(false);
            ani_Boy.Play("Steal");
            var timer2 = TimerExtensions.DelayAction(this, TIME_FIND_TREASURE, () =>
            {
                boyState = BoyStealTeasureState.StealFinish;
                if (index + 1 < lst_Score.Count)
                {
                    OnPlayBoxEffect(index + 1);
                }
            });
            _timers.Add(timer2);
        });
        _timers.Add(timer1);
    }
    //刷新底部文字
    void RefreshBetWin()
    {
        betNum.text = "111";
    }
    void StartCountDown()
    {
        var timeVal = 5;
        _countDownTimer = TimerExtensions.LoopAction(this, 1, (a) =>
        {
            timeVal--;
            if (timeVal < 0)
            {
                OnClickBox(Random.Range(1, 3));
                return;
            }
            else
            {
                countDownText.text = timeVal.ToString();
            }
        });
    }
}
