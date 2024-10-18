using BagelCode;
using BlizzUtils;
using ParadoxNotion;
using SimpleJSON;
using SlotMaker;
using SlotMaker.Slots;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DailyLottery : MonoBehaviour
{
    private Transform lottery;
    private PIDButton spinBtn;
    private Animator spinBtnAni;
    private PIDButton closeBtn;
    private BigWheel bigWheel;
    private GameObject winObj;
    private TextMeshProUGUI winText;
    private List<GameObject> rewardObjs = new List<GameObject>();
    private GameObject rollingEffect;
    private List<TextMeshProUGUI> jackpotList = new List<TextMeshProUGUI>();
    private List<int> creditList = new List<int>();
    private List<int> showList = new List<int>();
    private Animator animator;
    private bool canSpin;

    private float targetZ;
    private float slowOffset;
    private bool delaySlow;
    private bool hasSlowed;
    private float startSlowRotate;
    private State state;
    private float acceTimer;
    private float slowTimer;
    private float rotateTimer;
    private float rotateSpeed;
    private int balance;
    private int winIdex;

    private readonly float rotateTime = 6;
    private readonly float originSlowOffset = 180f;
    private readonly float slowDuration = 1.5f;
    private readonly float maxRotateSpeed = 12f;
    private readonly float accelerationDuration = 1f;

    private enum State
    {
        Init,
        Ready,
        Rolling
    }

    private void Awake()
    {
        lottery = transform.Find("Lottery/Lottery");
        bigWheel = lottery.GetComponent<BigWheel>();
        spinBtn = transform.Find("Lottery/SpinBtn").GetComponent<PIDButton>();
        spinBtnAni = spinBtn.GetComponent<Animator>();
        closeBtn = transform.Find("CloseBtn").GetComponent<PIDButton>();
        winObj = transform.Find("Win").gameObject;
        winText = transform.Find("Win/Text").GetComponent<TextMeshProUGUI>();
        for (int i = 0; i < lottery.childCount; i++)
            rewardObjs.Add(lottery.GetChild(i).Find("Effect").gameObject);
        for (int i = 0; i < lottery.childCount - 1; i++)
            jackpotList.Add(lottery.GetChild(i).Find("Text").GetComponent<TextMeshProUGUI>());
        rollingEffect = transform.Find("effect").gameObject;
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        spinBtn.interactable = false;
        NetManager.Instance.SendMsg(RPCName.queryDailyLottery, null);
    }

    private void OnEnable()
    {
        state = State.Init;
        MessageDispatcher.Register(RPCName.queryDailyLottery, OnQueryDailyLottery);
        MessageDispatcher.Register(RPCName.tryDailyLottery, OnTryDailyLottery);
    }

    private void OnDisable()
    {
        MessageDispatcher.UnRegister(RPCName.queryDailyLottery, OnQueryDailyLottery);
        MessageDispatcher.UnRegister(RPCName.tryDailyLottery, OnTryDailyLottery);
    }

    private void OnQueryDailyLottery(EventData data)
    {
        var jsonData = data.value as JSONNode;
        canSpin = jsonData["is_daily_sign"] != 1;
        spinBtn.interactable = canSpin;
        spinBtnAni.SetInteger("enable", canSpin ? 1 : 0);
        var tempList = jsonData["credit_list"];
        for (int i = 0; i < tempList.Count; i++)
        {
            creditList.Add(tempList[i]);
            if (i < tempList.Count - 1)
                showList.Add(tempList[i]);
        }
        Shuffle(showList);
        showList.Add(tempList[tempList.Count - 1]);
        for (int i = 0; i < jackpotList.Count; i++)
            jackpotList[i].text = showList[i] > 999 ? GetLotteryNumStr(creditList[i]) : showList[i].ToString();
        state = State.Ready;
    }

    public List<T> Shuffle<T>(List<T> original)
    {
        System.Random randomNum = new System.Random();
        T temp;
        for (int i = 0; i < original.Count; i++)
        {
            int index = randomNum.Next(0, original.Count - 1);
            if (index != i)
            {
                temp = original[i];
                original[i] = original[index];
                original[index] = temp;
            }
        }
        return original;
    }


    private string GetLotteryNumStr(float num)
    {
        num /= 1000;
        return num + "K";
    }

    private string GetJackpotNumStr(int num)
    {
        string str = "";
        while (num >= 1000)
        {
            string temp = (num % 1000).ToString();
            int k = 3 - temp.Length;
            for (int i = 0; i < k; i++)
                temp = '0' + temp;
            str = ',' + temp;
            num /= 1000;
        }
        str = num + str;

        return str;
    }

    private void OnTryDailyLottery(EventData data)
    {
        var jsonData = data.value as JSONNode;
        int index = jsonData["index"];
        int reward = jsonData["reward_credit"];
        balance = jsonData["balance"];
        int trueIndex = showList.IndexOf(creditList[index]);
        targetZ = (trueIndex + 1) * 36f;
        winIdex = trueIndex;
        winText.text = GetJackpotNumStr(reward);
        //StartRoll();
        NewStartRoll();


    }

    private void NewStartRoll()
    {
        animator.SetInteger("roll", 1);
        rollingEffect.SetActive(true);
        bigWheel.Simulation(1, targetZ, 5);
    }

    public void OnStoppedRoll()
    {
        animator.SetInteger("roll", 0);
        rollingEffect.SetActive(false);
        rewardObjs[winIdex].SetActive(true);
        winObj.SetActive(true);
        BlackboardQueryUtils.SetMyCredit(balance);
        MessageDispatcher.Dispatch("OnCreditEvent", new EventData<bool>("UpdateNaviCredit", true));
        closeBtn.interactable = true;
    }

    private void StartRoll()
    {
        slowOffset = originSlowOffset + Utils.GetRandom(30, 61);
        if (targetZ > slowOffset)
        {
            if (targetZ - lottery.rotation.eulerAngles.z <= slowOffset)
                delaySlow = true;
            else
                delaySlow = false;
        }
        else
        {
            if (360 - lottery.rotation.eulerAngles.z + targetZ <= slowOffset
                || lottery.rotation.eulerAngles.z < targetZ
                || 360 - lottery.rotation.eulerAngles.z + targetZ > 359)
                delaySlow = true;
            else
                delaySlow = false;
        }
        state = State.Rolling;

        if (this.targetZ - slowOffset < 0)
            startSlowRotate = 360 + targetZ - slowOffset;
        else
            startSlowRotate = targetZ - slowOffset;
        if (startSlowRotate > 360 - maxRotateSpeed)
            startSlowRotate = 360 - maxRotateSpeed - 1;
        else if (startSlowRotate < maxRotateSpeed)
            startSlowRotate = maxRotateSpeed + 1;
    }

    private void Rotate()
    {
        if (acceTimer < accelerationDuration)
        {
            float t = acceTimer / accelerationDuration;
            rotateSpeed = Mathf.LerpUnclamped(0, maxRotateSpeed, t);
            acceTimer += Time.deltaTime;
        }
        else
        {
            rotateSpeed = maxRotateSpeed;
            rotateTimer += Time.deltaTime;
        }
        lottery.Rotate(Vector3.forward * rotateSpeed);
    }

    private void StopRotate()
    {
        float curZ = lottery.rotation.eulerAngles.z;
        if (targetZ > slowOffset)
        {
            if (!delaySlow && (curZ > startSlowRotate || hasSlowed))
                DoSlow();
            if (delaySlow)
            {
                if (targetZ < slowOffset)
                {
                    if (targetZ - curZ > slowOffset)
                        delaySlow = false;
                }
                else
                {
                    float temp = targetZ - curZ < 0 ? 360 + targetZ - curZ : targetZ - curZ;
                    if (temp > slowOffset)
                        delaySlow = false;
                }
            }
        }
        else
        {
            if (!delaySlow && curZ > startSlowRotate || hasSlowed)
                DoSlow();

            if (delaySlow && targetZ + 360 - curZ > slowOffset)
                delaySlow = false;
        }
        lottery.Rotate(Vector3.forward * rotateSpeed);
    }

    private void DoSlow()
    {
        float curZ = lottery.rotation.eulerAngles.z;
        if (!hasSlowed)
            hasSlowed = true;
        if (slowTimer < slowDuration)
        {
            float t = slowTimer / slowDuration;
            rotateSpeed = Mathf.LerpUnclamped(maxRotateSpeed, 0.2f, t);
            slowTimer += Time.deltaTime;
        }
        else
        {
            if (rotateSpeed > 0.3f)
                rotateSpeed -= Time.deltaTime * 0.05f;
            if (curZ >= targetZ && Mathf.Abs(curZ - targetZ) < 1)
            {
                rotateSpeed = 0;
                state = State.Ready;
                BlackboardQueryUtils.SetMyCredit(balance);
                MessageDispatcher.Dispatch("OnCreditEvent", new EventData<bool>("UpdateNaviCredit", true));
                closeBtn.interactable = true;
            }
        }
    }

    public void OnSpinClick()
    {
        NetManager.Instance.SendMsg(RPCName.tryDailyLottery, null);
        canSpin = false;
        spinBtn.interactable = canSpin;
        spinBtnAni.SetInteger("enable", canSpin ? 1 : 0);
        closeBtn.interactable = false;
    }

    public void OnCloseClick()
    {
        EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_LEAVE_LOTTERY);
        PopupManager.Instance.Close();
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (state == State.Rolling)
        {
            if (rotateTimer < rotateTime)
                Rotate();
            else
                StopRotate();
        }
    }
}
