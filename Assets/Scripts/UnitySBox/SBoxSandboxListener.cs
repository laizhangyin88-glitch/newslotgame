using BlizzEvent;
using SlotMaker;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using static SBoxApi.SBoxSandbox;
using static SBoxSandboxListener;

public class CoinInData
{
    public int id;
    public int value;
}

public class SBoxSanboxEventHandle
{
    public const string COIN_IN = "COIN_IN";
    public const string COIN_OUT = "COIN_OUT";
    public const string COIN_OUT_TIMEOUT = "COIN_OUT_TIMEOUT";
    public const string BILL_IN = "BILL_IN";
    public const string BILL_STACKED = "BILL_STACKED";
}
public class SwitchClass
{
    public bool triggerPointerDown;
    public bool startPress;
    public bool longPressTrigger;
    public float curPointDownTime;
    public ButtonPointerDownEvent onPointerDown = new ButtonPointerDownEvent();
    public ButtonPointerUpEvent onPointerUp = new ButtonPointerUpEvent();
    public ButtonClickEvent onClick = new ButtonClickEvent();
    public ButtonLongPressEvent onLongPress = new ButtonLongPressEvent();
}
public class SBoxSandboxListener : MonoSingleton<SBoxSandboxListener>
{
    private bool isInit;

    private const float longPressTime = 0.6f;

    private readonly Dictionary<SBOX_SWITCH, SwitchClass> switchClassDic = new Dictionary<SBOX_SWITCH, SwitchClass>();

    public void Init()
    {
        if (isInit)
            return;
        Debug.LogError("SBoxSandboxListener Init");
        switchClassDic.Add(SBOX_SWITCH.SWITCH_UP, new SwitchClass());
        switchClassDic.Add(SBOX_SWITCH.SWITCH_DOWN, new SwitchClass());
        switchClassDic.Add(SBOX_SWITCH.SWITCH_LEFT, new SwitchClass());
        switchClassDic.Add(SBOX_SWITCH.SWITCH_RIGHT, new SwitchClass());
        switchClassDic.Add(SBOX_SWITCH.SWITCH_ROOT_SET, new SwitchClass());
        switchClassDic.Add(SBOX_SWITCH.SWITCH_SET, new SwitchClass());
        switchClassDic.Add(SBOX_SWITCH.SWITCH_DOOR_SWITCH, new SwitchClass());
        switchClassDic.Add(SBOX_SWITCH.SWITCH_PAYOUT, new SwitchClass());
        switchClassDic.Add(SBOX_SWITCH.SWITCH_ENTER, new SwitchClass());
        switchClassDic.Add(SBOX_SWITCH.SWITCH_ESC, new SwitchClass());
        switchClassDic.Add(SBOX_SWITCH.SWITCH_SWITCH, new SwitchClass());
        switchClassDic.Add(SBOX_SWITCH.SWITCH_SCORE_UP, new SwitchClass());
        switchClassDic.Add(SBOX_SWITCH.SWITCH_SCORE_DOWN, new SwitchClass());
        switchClassDic.Add(SBOX_SWITCH.SWITCH_RED, new SwitchClass());
        switchClassDic.Add(SBOX_SWITCH.SWITCH_GREEN, new SwitchClass());
        switchClassDic.Add(SBOX_SWITCH.SWITCH_YELLOW, new SwitchClass());
        switchClassDic.Add(SBOX_SWITCH.SWITCH_BET4, new SwitchClass());
        switchClassDic.Add(SBOX_SWITCH.SWITCH_BET5, new SwitchClass());
        switchClassDic.Add(SBOX_SWITCH.SWITCH_AUTO, new SwitchClass());
        isInit = true;

    }

    private void Update()
    {
        CheckNumberOfCointIn();
        CheckNumberOfCoinOut();
        CheckCoinOutTimeOut();
        CheckBillIn();
        CheckBillStacked();
        CheckButtonState();
    }

    private void CheckNumberOfCointIn()
    {
        for (int i = 0; i < 4; i++)
        {
            int value = NumberOfCoinIn(i);
            if (value > 0)
            {
                CoinInData coinInData = new CoinInData
                {
                    id = i,
                    value = value,
                };
                EventCenter.Instance.EventTrigger(SBoxSanboxEventHandle.COIN_IN, coinInData);
            }
        }
    }

    private void CheckBillIn()
    {
        int data = BillCredit();
        if (data > 0)
            EventCenter.Instance.EventTrigger(SBoxSanboxEventHandle.BILL_IN, data);
    }

    private void CheckBillStacked()
    {
        bool data = IsBillStacked();
        if (data)
            EventCenter.Instance.EventTrigger(SBoxSanboxEventHandle.BILL_STACKED);
    }

    private void CheckNumberOfCoinOut()
    {
        for (int i = 0; i < 2; i++)
        {
            int data = NumberOfCoinOut(i);
            if (data > 0)
                EventCenter.Instance.EventTrigger(SBoxSanboxEventHandle.COIN_OUT, data);
        }
    }

    private void CheckCoinOutTimeOut()
    {
        for (int i = 0; i < 2; i++)
            if (IsCoinOutTimeout(i))
                EventCenter.Instance.EventTrigger(SBoxSanboxEventHandle.COIN_OUT_TIMEOUT, i);
    }

    private void CheckButtonState()
    {
        ulong data = SwitchInState();
        foreach (SBOX_SWITCH switchKey in switchClassDic.Keys)
        {
            if (!switchClassDic[switchKey].startPress && (data & (ulong)switchKey) == (ulong)switchKey)
            {
                switchClassDic[switchKey].curPointDownTime = Time.time;
                switchClassDic[switchKey].startPress = true;
                switchClassDic[switchKey].longPressTrigger = false;
                if (!switchClassDic[switchKey].triggerPointerDown)
                {
                    switchClassDic[switchKey]?.onPointerDown.Invoke();
                    switchClassDic[switchKey].triggerPointerDown = true;
                    MatchDebugManager.Instance.SendUdpMessage(EventHandle.HARDWARE_KEY_DOWN, ((ulong)switchKey).ToString());
                }
            }

            if (switchClassDic[switchKey].startPress && (data & (ulong)switchKey) == 0)
            {
                switchClassDic[switchKey].startPress = false;
                switchClassDic[switchKey]?.onPointerUp.Invoke();
                switchClassDic[switchKey].triggerPointerDown = false;
                MatchDebugManager.Instance.SendUdpMessage(EventHandle.HARDWARE_KEY_UP, ((ulong)switchKey).ToString());
                CheckClick(switchKey);
            }
        }
        CheckIsLongPress();
    }

    private void CheckIsLongPress()
    {
        foreach (SBOX_SWITCH switchKey in switchClassDic.Keys)
        {
            if (switchClassDic[switchKey].startPress && !switchClassDic[switchKey].longPressTrigger)
            {
                if (Time.time > switchClassDic[switchKey].curPointDownTime + longPressTime)
                {
                    switchClassDic[switchKey].longPressTrigger = true;
                    switchClassDic[switchKey].startPress = false;
                    switchClassDic[switchKey].onLongPress?.Invoke();
                    MatchDebugManager.Instance.SendUdpMessage(EventHandle.HARDWARE_KEY_LONG_PRESS, ((ulong)switchKey).ToString());
                }
            }
        }
    }

    private void CheckClick(SBOX_SWITCH switchKey)
    {
        if (!switchClassDic[switchKey].longPressTrigger
            && Time.time <= switchClassDic[switchKey].curPointDownTime + longPressTime)
        {
            switchClassDic[switchKey].onClick?.Invoke();
            MatchDebugManager.Instance.SendUdpMessage(EventHandle.HARDWARE_KEY_CLICK, ((ulong)switchKey).ToString());
        }
    }

    public void AddButtonDown(SBOX_SWITCH sboxSwtich, UnityAction unityAction)
    {
        switchClassDic[sboxSwtich].onPointerDown.AddListener(unityAction);

    }

    public void RemoveButtonDown(SBOX_SWITCH sboxSwtich, UnityAction unityAction)
    {
        switchClassDic[sboxSwtich].onPointerDown.RemoveListener(unityAction);

    }

    public void AddButtonUp(SBOX_SWITCH sboxSwtich, UnityAction unityAction)
    {
        switchClassDic[sboxSwtich].onPointerUp.AddListener(unityAction);
    }
    public void RemoveButtonUp(SBOX_SWITCH sboxSwtich, UnityAction unityAction)
    {
        switchClassDic[sboxSwtich].onPointerUp.RemoveListener(unityAction);
    }


    public void AddButtonClick(SBOX_SWITCH sboxSwtich, UnityAction unityAction)
    {
        switchClassDic[sboxSwtich].onClick.AddListener(unityAction);
    }

    public void AddButtonLongPress(SBOX_SWITCH sboxSwtich, UnityAction unityAction)
    {
        switchClassDic[sboxSwtich].onLongPress.AddListener(unityAction);
    }

    public void RemoveButtonLongPress(SBOX_SWITCH sboxSwtich, UnityAction unityAction)
    {
        switchClassDic[sboxSwtich].onLongPress.RemoveListener(unityAction);
    }

    public class ButtonPointerDownEvent : UnityEvent { }

    public class ButtonPointerUpEvent : UnityEvent { }

    public class ButtonClickEvent : UnityEvent { }

    public class ButtonLongPressEvent : UnityEvent { }
}
