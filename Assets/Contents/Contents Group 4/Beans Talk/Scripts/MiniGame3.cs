using GameUtil;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniGame3 : MonoBehaviour
{
    Transform root;
    Animator ani_Man, ani_Boy, ani_Gift1, ani_Gift2, ani_Gift3, ani_Gift4, ani_Gift5;
    Button btn_Gift1, btn_Gift2, btn_Gift3, btn_Gift4, btn_Gift5;
    TextMeshProUGUI betNum, winNum, scoreNum, countDownText;
    Dictionary<int, Vector2> _dicPos = new Dictionary<int, Vector2>()
    {
        [1] = new Vector2(-304, 333),
        [2] = new Vector2(-276, 183),
        [3] = new Vector2(-216, 54),
        [4] = new Vector2(-79, 54),
        [5] = new Vector2(190, 196),
    };
    Dictionary<int, int> dic_Score = new Dictionary<int, int>
    {
        [1] = 100,
        [2] = 200,
        [3] = 300,
        [4] = 400,
        [5] = 500,
    };
    List<int> RecIndexList;
    bool isAni;
    int timeUpIndex = 3;
    bool isOver = false;
    List<DelayTimer> _timers;
    LoopTimer _countDownTimer;
    void Awake()
    {
        root = transform.Find("Animator/Anchor");
        ani_Man = root.Find("man/Animator").GetComponent<Animator>();
        ani_Boy = root.Find("boy").GetComponent<Animator>();
        scoreNum = ani_Boy.transform.Find("score").GetComponent<TextMeshProUGUI>();
        ani_Gift1 = root.Find("gifts/gift1").GetComponent<Animator>();
        ani_Gift2 = root.Find("gifts/gift2").GetComponent<Animator>();
        ani_Gift3 = root.Find("gifts/gift3").GetComponent<Animator>();
        ani_Gift4 = root.Find("gifts/gift4").GetComponent<Animator>();
        ani_Gift5 = root.Find("gifts/gift5").GetComponent<Animator>();
        btn_Gift1 = ani_Gift1.GetComponent<Button>();
        btn_Gift2 = ani_Gift2.GetComponent<Button>();
        btn_Gift3 = ani_Gift3.GetComponent<Button>();
        btn_Gift4 = ani_Gift4.GetComponent<Button>();
        btn_Gift5 = ani_Gift5.GetComponent<Button>();
        betNum = root.Find("betWin/bet/betNum").GetComponent<TextMeshProUGUI>();
        winNum = root.Find("betWin/win/winNum").GetComponent<TextMeshProUGUI>();
        countDownText = root.Find("countDown").GetComponent<TextMeshProUGUI>();
    }
    void OnEnable()
    {
        _timers = new List<DelayTimer>();
        ani_Boy.gameObject.SetActive(false);
        isAni = false;
        RecIndexList = new List<int>();
        AddEvent();
        RefreshBetWin();
        StartCountDown();
    }
    void OnDisable()
    {
        RemoveEvent();
        RecIndexList?.Clear();
        foreach (var timer in _timers)
        {
            timer?.Cancel();
        }
        _timers?.Clear();
        _timers = null;
    }
    void AddEvent()
    {
        btn_Gift1.onClick.AddListener(ClickGift1);
        btn_Gift2.onClick.AddListener(ClickGift2);
        btn_Gift3.onClick.AddListener(ClickGift3);
        btn_Gift4.onClick.AddListener(ClickGift4);
        btn_Gift5.onClick.AddListener(ClickGift5);
    }

    void ClickGift1() { OnClickGift(1); }
    void ClickGift2() { OnClickGift(2); }
    void ClickGift3() { OnClickGift(3); }
    void ClickGift4() { OnClickGift(4); }
    void ClickGift5() { OnClickGift(5); }

    void RemoveEvent()
    {
        btn_Gift1.onClick.RemoveListener(ClickGift1);
        btn_Gift2.onClick.RemoveListener(ClickGift2);
        btn_Gift3.onClick.RemoveListener(ClickGift3);
        btn_Gift4.onClick.RemoveListener(ClickGift4);
        btn_Gift5.onClick.RemoveListener(ClickGift5);
    }
    void OnClickGift(int index)
    {
        _countDownTimer?.Cancel();
        _countDownTimer = null;
        if (!isOver && !isAni && _dicPos.TryGetValue(index, out var pos) && !RecIndexList.Contains(index))
        {
            isAni = true;
            RecIndexList.Add(index);
            ani_Boy.gameObject.SetActive(true);
            ani_Boy.transform.localPosition = pos;
            scoreNum.gameObject.SetActive(false);
            ani_Boy.Play("Steal");
            RefreshGiftState(true);
            if (scoreNum != null && dic_Score.TryGetValue(index, out var score))
                scoreNum.text = score.ToString();
            var timer1 = TimerExtensions.DelayAction(this, 1.5f, () =>
            {
                scoreNum.gameObject.SetActive(true);
            });
            var timer2 = TimerExtensions.DelayAction(this, 2.2f, () =>
            {
                scoreNum.gameObject.SetActive(false);
                var obj = GetGiftObj(index);
                if (obj != null)
                {
                    obj.SetActive(false);
                }
                ani_Boy.gameObject.SetActive(false);
                RefreshGiftState(false);
                isAni = false;
                if (index == timeUpIndex)
                {
                    ani_Man.Play("Query");
                    isOver = true;
                }
                if (RecIndexList.Count != 5 && !isOver)
                    StartCountDown();
            });
            _timers.Add(timer1);
            _timers.Add(timer2);
        }
    }
    void RefreshGiftState(bool isSteal)
    {
        ani_Gift1.Play((isSteal && RecIndexList.Contains(1) || (!isSteal && !RecIndexList.Contains(1))) ? "HighLight" : "Default");
        ani_Gift2.Play((isSteal && RecIndexList.Contains(2) || (!isSteal && !RecIndexList.Contains(2))) ? "HighLight" : "Default");
        ani_Gift3.Play((isSteal && RecIndexList.Contains(3) || (!isSteal && !RecIndexList.Contains(3))) ? "HighLight" : "Default");
        ani_Gift4.Play((isSteal && RecIndexList.Contains(4) || (!isSteal && !RecIndexList.Contains(4))) ? "HighLight" : "Default");
        ani_Gift5.Play((isSteal && RecIndexList.Contains(5) || (!isSteal && !RecIndexList.Contains(5))) ? "HighLight" : "Default");
    }
    GameObject GetGiftObj(int index)
    {
        GameObject obj = null;
        switch (index)
        {
            case 1:
                obj = ani_Gift1.gameObject;
                break;
            case 2:
                obj = ani_Gift2.gameObject;
                break;
            case 3:
                obj = ani_Gift3.gameObject;
                break;
            case 4:
                obj = ani_Gift4.gameObject;
                break;
            case 5:
                obj = ani_Gift5.gameObject;
                break;
            default:
                break;
        }
        return obj;
    }
    //刷新底部文字
    void RefreshBetWin()
    {
        betNum.text = "111";
        winNum.text = "222";
    }
    void StartCountDown()
    {
        countDownText.text = "5";
        var timeVal = 5;
        _countDownTimer = TimerExtensions.LoopAction(this, 1, (a) =>
        {
            timeVal--;
            if (timeVal < 0)
            {
                var index = 1;
                for (int i = 1; i <= 5; i++)
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
