using BagelCode;
using GameUtil;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniGame1 : MonoBehaviour
{

    Transform root;
    Animator ani_Woman, ani_Boy, ani_Fire, ani_Egg, ani_Goose, ani_Door1, ani_Door2, ani_Door3, ani_Door4, ani_Door5;
    Button btn_Door1, btn_Door2, btn_Door3, btn_Door4, btn_Door5;
    TextMeshProUGUI betNum, winNum, scoreNum, countDownText;
    //List<bool> openList = new List<bool>() { true,false,true,true,false};
    int fireIndex = 2;//着火的门index
    int clickIndex = 0;
    Dictionary<int, Vector2> dicPos = new Dictionary<int, Vector2>();
    List<DelayTimer> _timers;
    bool isAni;
    Vector2 startPos = new Vector2(49, -165);
    Vector2 endPos = new Vector2(-225, -165);
    LoopTimer _countDownTimer;
    Dictionary<int, int> dic_Score = new Dictionary<int, int>
    {
        [1] = 100,
        [2] = 200,
        [3] = 300,
        [4] = 400,
        [5] = 500,
    };
    void Awake()
    {
        root = transform.Find("Animator/Anchor");
        ani_Woman = root.Find("woman").GetComponent<Animator>();
        ani_Boy = root.Find("boy").GetComponent<Animator>();
        ani_Fire = root.Find("fire").GetComponent<Animator>();
        ani_Egg = root.Find("egg").GetComponent<Animator>();
        ani_Goose = root.Find("goose").GetComponent<Animator>();
        ani_Door1 = root.Find("doors/door1").GetComponent<Animator>();
        ani_Door2 = root.Find("doors/door2").GetComponent<Animator>();
        ani_Door3 = root.Find("doors/door3").GetComponent<Animator>();
        ani_Door4 = root.Find("doors/door4").GetComponent<Animator>();
        ani_Door5 = root.Find("doors/door5").GetComponent<Animator>();
        btn_Door1 = ani_Door1.GetComponent<Button>();
        btn_Door2 = ani_Door2.GetComponent<Button>();
        btn_Door3 = ani_Door3.GetComponent<Button>();
        btn_Door4 = ani_Door4.GetComponent<Button>();
        btn_Door5 = ani_Door5.GetComponent<Button>();
        scoreNum = ani_Boy.transform.Find("score").GetComponent<TextMeshProUGUI>();
        betNum = root.Find("betWin/bet/betNum").GetComponent<TextMeshProUGUI>();
        winNum = root.Find("betWin/win/winNum").GetComponent<TextMeshProUGUI>();
        countDownText = root.Find("countDown").GetComponent<TextMeshProUGUI>();

        dicPos.Add(1, ani_Door1.transform.localPosition);
        dicPos.Add(2, ani_Door2.transform.localPosition);
        dicPos.Add(3, ani_Door3.transform.localPosition);
        dicPos.Add(4, ani_Door4.transform.localPosition);
        dicPos.Add(5, ani_Door5.transform.localPosition);


        AddEvent();
    }
    void OnEnable()
    {
        scoreNum.gameObject.SetActive(false);
        countDownText.text = "5";
        isAni = false;
        _timers = new List<DelayTimer>();
        ani_Fire.gameObject.SetActive(false);
        SetDoorCanOpenState(false);
        RefreshBetWin();
        clickIndex = 3;
        PlayEffect();
    }
    void OnDisable()
    {
        dicPos?.Clear();
        RemoveEvent();
        if (_timers != null)
        {
            foreach (var timer in _timers)
            {
                timer?.Cancel();
            }
            _timers?.Clear();
            _timers = null;
        }
        _countDownTimer?.Cancel();
        _countDownTimer = null;
    }
    void AddEvent()
    {
        btn_Door1.onClick.AddListener(ClickDoor1);
        btn_Door2.onClick.AddListener(ClickDoor2);
        btn_Door3.onClick.AddListener(ClickDoor3);
        btn_Door4.onClick.AddListener(ClickDoor4);
        btn_Door5.onClick.AddListener(ClickDoor5);
    }
    void ClickDoor1() { OnClickDoor(1); }
    void ClickDoor2() { OnClickDoor(2); }
    void ClickDoor3() { OnClickDoor(3); }
    void ClickDoor4() { OnClickDoor(4); }
    void ClickDoor5() { OnClickDoor(5); }


    void RemoveEvent()
    {
        btn_Door1.onClick.RemoveListener(ClickDoor1);
        btn_Door2.onClick.RemoveListener(ClickDoor2);
        btn_Door3.onClick.RemoveListener(ClickDoor3);
        btn_Door4.onClick.RemoveListener(ClickDoor4);
        btn_Door5.onClick.RemoveListener(ClickDoor5);
    }
    void OnClickDoor(int index)
    {
        clickIndex = index;
        PlayEffect();
    }
    void PlayEffect()
    {
        _countDownTimer?.Cancel();
        _countDownTimer = null;
        if (isAni) return;
        isAni = true;
        ani_Door1.Play(1 == clickIndex || 1 == fireIndex ? "Open" : "Default");
        ani_Door2.Play(2 == clickIndex || 2 == fireIndex ? "Open" : "Default");
        ani_Door3.Play(3 == clickIndex || 3 == fireIndex ? "Open" : "Default");
        ani_Door4.Play(4 == clickIndex || 4 == fireIndex ? "Open" : "Default");
        ani_Door5.Play(5 == clickIndex || 5 == fireIndex ? "Open" : "Default");

        if (dicPos.TryGetValue(clickIndex, out var pos) && dicPos.TryGetValue(fireIndex,out var firePos))
        {
            ani_Boy.transform.localPosition = new Vector2(pos.x, pos.y);
            ani_Boy.Play("Hide");
            ani_Goose.Play("State1");
            var timer1 = TimerExtensions.DelayAction(this, 1f, () =>
            {
                ani_Egg.gameObject.SetActive(true);
                ani_Egg.Play("Start");
                var timer2 = TimerExtensions.DelayAction(this, 1f, () =>
                {
                    ani_Egg.gameObject.SetActive(false);
                });
                _timers.Add(timer2);
            });
            _timers.Add(timer1);
            ani_Fire.transform.localPosition = new Vector2(firePos.x, firePos.y);
            ani_Fire.gameObject.SetActive(true);
        }


        if (clickIndex != fireIndex)
        {
            ani_Woman.GetComponent<Image>().enabled = true;
            ani_Woman.Play("State1");
            var timer3 = TimerExtensions.DelayAction(this, 2f, () =>
            {
                ani_Fire.gameObject.SetActive(false);
                SetDoorCanOpenState(true);
                ani_Boy.Play("Steal");
                AsyncActionUtils.ApplyLocalMovement(this, ani_Boy.transform, startPos, endPos, 0.3f, TweenUtils.VectorTweenLinear, 0, () =>
                {
                    if (dic_Score.TryGetValue(clickIndex,out var score))
                    {
                        scoreNum.gameObject.SetActive(true);
                        scoreNum.text = score.ToString();
                    }
                    ani_Goose.Play("State2");
                    var timer4 = TimerExtensions.DelayAction(this, 1.2f, () =>
                    {
                        scoreNum.gameObject.SetActive(false);
                        AsyncActionUtils.ApplyLocalMovement(this, ani_Boy.transform, endPos, startPos, 0.4f, TweenUtils.VectorTweenLinear, 0f, () =>
                        {
                            SetDoorCanOpenState(false);
                            isAni = false;
                            clickIndex = 0;
                            StartCountDown();
                        });
                    });
                    _timers.Add(timer4);
                });
            });
            _timers.Add(timer3);
        }
        else
        {
            ani_Woman.GetComponent<Image>().enabled = true;
            ani_Woman.Play("State3");
            ani_Boy.Play("Fire");
            SetDoorCanOpenState(true);
            isAni = false;
            clickIndex = 0;
        }
    }
    void SetDoorCanOpenState(bool isDefault)
    {
        ani_Door1.Play(isDefault ? "Default" : "CanOpen");
        ani_Door2.Play(isDefault ? "Default" : "CanOpen");
        ani_Door3.Play(isDefault ? "Default" : "CanOpen");
        ani_Door4.Play(isDefault ? "Default" : "CanOpen");
        ani_Door5.Play(isDefault ? "Default" : "CanOpen");
    }
    void StartCountDown()
    {
        var timeVal = 5;
        _countDownTimer = TimerExtensions.LoopAction(this, 1, (a) =>
        {
            timeVal--;
            if (timeVal < 0)
            {
                countDownText.text = "5";
                clickIndex = Random.Range(1,5);
                PlayEffect();
                return;
            }
            else
            {
                countDownText.text = timeVal.ToString();
            }
        });
    }
    //刷新底部文字
    void RefreshBetWin()
    {
        betNum.text = "111";
        winNum.text = "222";
    }
}
