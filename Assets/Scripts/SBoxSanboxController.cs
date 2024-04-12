using BagelCode;
using BlizzEvent;
using ParadoxNotion;
using SBoxApi;
using SboxSpace;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SBoxApi.SBoxSandbox;

public class SBoxSanboxController : EventMonoSingleton<SBoxSanboxController>
{
    private int coinOutNum;

    private enum LightStatus
    {
        OFF = 0,
        ON = 1,
    }

    void Start()
    {
        AddButtonEvent();
        AddEventListener();
    }

    private void AddButtonEvent()
    {

#if UNITY_EDITOR
        EventCenter.Instance.AddEventListener<SBOX_SWITCH>(EventHandle.HARDWARE_KEY_DOWN, OnKeyDown);
        EventCenter.Instance.AddEventListener<SBOX_SWITCH>(EventHandle.HARDWARE_KEY_UP, OnKeyUp);
#else
        if (ApplicationSettings.Instance.isMachine){
            //SBoxSandboxListener.Instance.AddButtonDown(SBOX_SWITCH.SWITCH_UP, () => { EventSender.SendGlobalEvent(MetaEventDefine.ON_MACHINE, new EventData<int>(MachineEventDefine.ON_KEY_UP, 0)); });
            //SBoxSandboxListener.Instance.AddButtonDown(SBOX_SWITCH.SWITCH_DOWN, () => { EventSender.SendGlobalEvent(MetaEventDefine.ON_MACHINE, new EventData<int>(MachineEventDefine.ON_KEY_DOWN, 0)); });
            //SBoxSandboxListener.Instance.AddButtonDown(SBOX_SWITCH.SWITCH_LEFT, () => {
            //    //选择框左移
            //    MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_LEFT));
            //    //降低押注
            //    MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetDown"));
            //});
            //SBoxSandboxListener.Instance.AddButtonDown(SBOX_SWITCH.SWITCH_RIGHT, () => {
            //    //选择框右移
            //    MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_RIGHT));
            //    //提高押注
            //    MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetUp"));
            //});
            SBoxSandboxListener.Instance.AddButtonDown(SBOX_SWITCH.SWITCH_ENTER, () => {
                if (PopupManager.Instance.popupCount > 0)
                    {
                        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Return"));
                        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Collect"));
                        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnClose"));
                        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnCollect"));
                        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnPointerClick"));
                        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnStartFreeSpin"));
                    }
                    else
                        MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData<int>(MachineEventDefine.ON_KEY_START, 1));
            });
            SBoxSandboxListener.Instance.AddButtonDown(SBOX_SWITCH.SWITCH_ESC, () => {
                if (PopupManager.Instance.popupCount == 0)
                    EventSender.SendGlobalEvent("OpenPaytable");
            });
            SBoxSandboxListener.Instance.AddButtonDown(SBOX_SWITCH.SWITCH_SWITCH, () => {
                //最大下注
                MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetMax"));
                //炮升级
                //EventSender.SendGlobalEvent(MetaEventDefine.ON_MACHINE, new EventData<int>(MachineEventDefine.ON_KEY_MAX_BET, data));
            });
            SBoxSandboxListener.Instance.AddButtonDown(SBOX_SWITCH.SWITCH_RED, () => {
                StartCoroutine(PurchaseCreditRequest(1, 100));
            });
            SBoxSandboxListener.Instance.AddButtonDown(SBOX_SWITCH.SWITCH_GREEN, () => {
                StartCoroutine(PurchaseCreditRequest(2, 100));
            });
            SBoxSandboxListener.Instance.AddButtonDown(SBOX_SWITCH.SWITCH_YELLOW, () => {
                //选择框左移
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_LEFT));
                //降低押注
                MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetDown"));
            });
            SBoxSandboxListener.Instance.AddButtonDown(SBOX_SWITCH.SWITCH_BET4, () => {
                //选择框右移
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_RIGHT));
                //提高押注
                MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetUp"));
            });
            SBoxSandboxListener.Instance.AddButtonDown(SBOX_SWITCH.SWITCH_BET5, () => {
                if (BlackboardQueryUtils.IsSpin()
                        || BlackboardQueryUtils.IsAutoSpin())
                    return;
                //退出
                EventSender.SendGlobalEvent("OnLobby");
                //鱼房间退出
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_BET5));
                //鱼机 退出
                MessageDispatcher.Dispatch(MetaEventDefine.ON_MACHINE, new EventData(MachineEventDefine.ON_KEY_BET5));
            });
            SBoxSandboxListener.Instance.AddButtonUp(SBOX_SWITCH.SWITCH_ENTER, () => {
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData<int>(MachineEventDefine.ON_KEY_START, 0));
            });
        }
#endif
    }

    private void OnKeyDown(SBOX_SWITCH sBOX_SWITCH)
    {
#if UNITY_EDITOR
        Debug.LogError("KeyDown " + sBOX_SWITCH);
#endif
        switch (sBOX_SWITCH)
        {
            case SBOX_SWITCH.SWITCH_UP:
                EventSender.SendGlobalEvent(MetaEventDefine.ON_MACHINE, new EventData<int>(MachineEventDefine.ON_KEY_UP, 0));
                break;
            case SBOX_SWITCH.SWITCH_DOWN:
                EventSender.SendGlobalEvent(MetaEventDefine.ON_MACHINE, new EventData<int>(MachineEventDefine.ON_KEY_DOWN, 0));
                break;
            case SBOX_SWITCH.SWITCH_LEFT:
                //选择框左移
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_LEFT));
                //降低押注
                MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetDown"));
                break;
            case SBOX_SWITCH.SWITCH_RIGHT:
                //选择框右移
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_RIGHT));
                //提高押注
                MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetUp"));
                break;
            case SBOX_SWITCH.SWITCH_ROOT_SET:
                break;
            case SBOX_SWITCH.SWITCH_SET:
                break;
            case SBOX_SWITCH.SWITCH_DOOR_SWITCH:
                break;
            case SBOX_SWITCH.SWITCH_PAYOUT:
                break;
            case SBOX_SWITCH.SWITCH_ENTER: 
                if (PopupManager.Instance.popupCount > 0)
                {
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Return"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Collect"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnClose"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnCollect"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnPointerClick"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnStartFreeSpin"));
                }
                else
                    MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData<int>(MachineEventDefine.ON_KEY_START, 1));
                break;
            case SBOX_SWITCH.SWITCH_ESC:
                if (PopupManager.Instance.popupCount == 0)
                    EventSender.SendGlobalEvent("OpenPaytable");
                break;
            case SBOX_SWITCH.SWITCH_SWITCH:
                //最大下注
                MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetMax"));
                //炮升级
                //EventSender.SendGlobalEvent(MetaEventDefine.ON_MACHINE, new EventData<int>(MachineEventDefine.ON_KEY_MAX_BET, data));
                break;
            case SBOX_SWITCH.SWITCH_SCORE_UP:
                break;
            case SBOX_SWITCH.SWITCH_SCORE_DOWN:
                break;
            case SBOX_SWITCH.SWITCH_RED:
                StartCoroutine(PurchaseCreditRequest(1, 100));
                break;
            case SBOX_SWITCH.SWITCH_GREEN:
                StartCoroutine(PurchaseCreditRequest(2, 100));
                break;
            case SBOX_SWITCH.SWITCH_YELLOW:
                //选择框左移
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_LEFT));
                //降低押注
                MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetDown"));
                break;
            case SBOX_SWITCH.SWITCH_BET4:
                //选择框右移
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_RIGHT));
                //提高押注
                MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetUp"));
                break;
            case SBOX_SWITCH.SWITCH_BET5:
                if (BlackboardQueryUtils.IsSpin()
                    || BlackboardQueryUtils.IsAutoSpin())
                    return;
                //退出
                EventSender.SendGlobalEvent("OnLobby");
                //鱼房间退出
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_BET5));
                //鱼机 退出
                MessageDispatcher.Dispatch(MetaEventDefine.ON_MACHINE, new EventData(MachineEventDefine.ON_KEY_BET5));
                break;
            case SBOX_SWITCH.SWITCH_AUTO:
                break;
            default:
                break;
        }
    }

    private void OnKeyUp(SBOX_SWITCH sBOX_SWITCH)
    {
        Debug.LogError("KeyUp " + sBOX_SWITCH);
        switch (sBOX_SWITCH)
        {
            case SBOX_SWITCH.SWITCH_UP:
                break;
            case SBOX_SWITCH.SWITCH_DOWN:
                break;
            case SBOX_SWITCH.SWITCH_LEFT:
                break;
            case SBOX_SWITCH.SWITCH_RIGHT:
                break;
            case SBOX_SWITCH.SWITCH_ROOT_SET:
                break;
            case SBOX_SWITCH.SWITCH_SET:
                break;
            case SBOX_SWITCH.SWITCH_DOOR_SWITCH:
                break;
            case SBOX_SWITCH.SWITCH_PAYOUT:
                break;
            case SBOX_SWITCH.SWITCH_ENTER:
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData<int>(MachineEventDefine.ON_KEY_START, 0));
                break;
            case SBOX_SWITCH.SWITCH_ESC:
                break;
            case SBOX_SWITCH.SWITCH_SWITCH:
                break;
            case SBOX_SWITCH.SWITCH_SCORE_UP:
                break;
            case SBOX_SWITCH.SWITCH_SCORE_DOWN:
                break;
            case SBOX_SWITCH.SWITCH_RED:
                break;
            case SBOX_SWITCH.SWITCH_GREEN:
                break;
            case SBOX_SWITCH.SWITCH_YELLOW:
                break;
            case SBOX_SWITCH.SWITCH_BET4:
                break;
            case SBOX_SWITCH.SWITCH_BET5:
                break;
            case SBOX_SWITCH.SWITCH_AUTO:
                break;
            default:
                break;
        }
    }

    private void AddEventListener()
    {
        BlizzEvent.EventCenter.Instance.AddEventListener<int>(SBoxSanboxEventHandle.COIN_IN, OnCoinIn);
        BlizzEvent.EventCenter.Instance.AddEventListener<int>(SBoxSanboxEventHandle.COIN_OUT, OnCoinOut);
        Register(MachineEventDefine.ON_LIGHT_CHANGE, OnLightChange);
    }

    private void OnCoinIn(int coinCount)
    {
        Debug.LogError("CoinIn");
        StartCoroutine(PurchaseCreditRequest(1, coinCount));
    }

    public IEnumerator PurchaseCreditRequest(UInt32 operateType, long purchase)
    {
        bool requestSuccess = false;
        bool requestFail = false;

#if NEW_NET

        string rpcName = operateType == 1 ? RPCName.agentRechargeToDeviceUser : RPCName.decreaseDeviceCredit;
        //operateType == 1 ? "/v0/purchase/add_credit" : "/v0/purchase/sub_credit";

        Dictionary<string, object> req = new Dictionary<string, object>
            {
                {"balance",purchase},
            };

        NetManager.Instance.Post(rpcName, req,
        (res) =>
        {

            requestSuccess = true;
            globalStore.newCredit = res["balance"].AsLong;
            BlackboardQueryUtils.SetMyCredit(globalStore.newCredit);
            MessageDispatcher.Dispatch("OnCreditEvent", new EventData<bool>("UpdateNaviCredit", true));
        },
        (error) =>
        {
            switch (error.errorCode)
            {
                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }

            requestFail = true;
        });
#else
            BagelCodeClientAPI.PurchaseCreditRequest(operateType, purchase,
                (response) =>
                {
                    requestSuccess = true;
                    if (response.userSyncInfo != null)
                    {
                        BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                        BlackboardQueryUtils.ApplyUserSyncInfo();
                    }
                },
                (error) =>
                {
                    switch (error.errorCode)
                    {
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }

                    requestFail = true;
                });
#endif

        yield return new WaitUntil(() => requestSuccess || requestFail);
    }

    private void OnCoinOut(int coinCount)
    {
        if (coinOutNum - coinCount < 0)
            coinCount = coinOutNum;
        coinOutNum -= coinCount;
        StartCoroutine(PurchaseCreditRequest(2, coinCount * 100));
        if (coinOutNum == 0)
            CoinOutStop(0);
    }

    private void OnLightChange(EventData data)
    {
        switch (data.value)
        {
            case "lobby":
                //开灯
                SwitchOutStateOn((ulong)SBOX_SWITCH.SWITCH_UP);
                SwitchOutStateOn((ulong)SBOX_SWITCH.SWITCH_RED);
                SwitchOutStateOn((ulong)SBOX_SWITCH.SWITCH_GREEN);
                SwitchOutStateOn((ulong)SBOX_SWITCH.SWITCH_YELLOW);
                SwitchOutStateOn((ulong)SBOX_SWITCH.SWITCH_BET4);
                SwitchOutStateOn((ulong)SBOX_SWITCH.SWITCH_BET5);
                SwitchOutStateOn((ulong)SBOX_SWITCH.SWITCH_ENTER);
                SwitchOutStateOn((ulong)SBOX_SWITCH.SWITCH_AUTO);
                break;
            case "slots":
                //关灯
                SwitchOutStateOff((ulong)SBOX_SWITCH.SWITCH_UP);
                break;
            default:
                break;
        }
    }

}
