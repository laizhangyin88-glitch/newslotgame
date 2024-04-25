using BagelCode.ClientModels;
using BagelCode.Protobuf;
using NodeCanvas.Framework;
using ParadoxNotion;
using SboxSpace;
using SlotMaker;
using SlotMaker.Tasks.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class IOController : EventMonoSingleton<IOController>
    {
        private int coinOutNum;

        private enum LightStatus
        {
            OFF = 0,
            ON = 1,
        }

        void Start()
        {
            AddEventListener();
        }

        private void AddEventListener()
        {
            IOEventCenter.AddListener(IOCenterEvent.EVENT_CoinIn, OnCoinIn);
            IOEventCenter.AddListener(IOCenterEvent.EVENT_CoinOut, OnCoinOut);
            IOEventCenter.AddListener(IOCenterEvent.EVENT_KeyStatus, OnKeyStatus);
            IOEventCenter.AddListener(IOCenterEvent.EVENT_CashIn, OnCashIn);
            //IOEventCenter.AddListener(IOCenterEvent.EVENT_PrintStatus, OnCoinIn);
            //IOEventCenter.AddListener(IOCenterEvent.EVENT_CashStatus, OnCoinIn);
            //IOEventCenter.AddListener(IOCenterEvent.EVENT_OneLightStatus, OnCoinIn);
            //IOEventCenter.AddListener(IOCenterEvent.EVENT_AllLightStatus, OnCoinIn);
            //IOEventCenter.AddListener(IOCenterEvent.EVENT_CahserList, OnCoinIn);
            //IOEventCenter.AddListener(IOCenterEvent.EVENT_PrinterList, OnCoinIn);
            //IOEventCenter.AddListener(IOCenterEvent.EVENT_PrintOK, OnCoinIn);

            //MessageDispatcher.Register(MetaEventDefine.ON_MACHINE, Dispatch);
            Register(MachineEventDefine.ON_LIGHT_CHANGE, OnLightChange);

        }

        public void OnLightChange(EventData data)
        {
            switch (data.value)
            {
                case "lobby":
                    //ChangeLightStatus(Light3288.Light_Bet1, LightStatus.OFF);
                    //ChangeLightStatus(Light3288.Light_Bet2, LightStatus.OFF);
                    //ChangeLightStatus(Light3288.Light_Bet3, LightStatus.ON);
                    //ChangeLightStatus(Light3288.Light_Bet4, LightStatus.ON);
                    //ChangeLightStatus(Light3288.Light_Help, LightStatus.ON);
                    //ChangeLightStatus(Light3288.Light_Start, LightStatus.ON);

                    ChangeLightStatus(Light3288.Light_Bet1, LightStatus.ON);
                    ChangeLightStatus(Light3288.Light_Bet2, LightStatus.ON);
                    ChangeLightStatus(Light3288.Light_Bet3, LightStatus.ON);
                    ChangeLightStatus(Light3288.Light_Bet4, LightStatus.ON);
                    ChangeLightStatus(Light3288.Light_Bet5, LightStatus.ON);
                    ChangeLightStatus(Light3288.Light_DownSco, LightStatus.ON);
                    ChangeLightStatus(Light3288.Light_Start, LightStatus.ON);
                    ChangeLightStatus(Light3288.Light_Auto, LightStatus.ON);
                    ChangeLightStatus(Light3288.Light_Select, LightStatus.ON);
                    ChangeLightStatus(Light3288.Light_Help, LightStatus.ON);
                    break;
                case "slots":
                    ChangeLightStatus(Light3288.Light_Bet1, LightStatus.ON);
                    ChangeLightStatus(Light3288.Light_Bet3, LightStatus.ON);
                    ChangeLightStatus(Light3288.Light_Start, LightStatus.ON);
                    ChangeLightStatus(Light3288.Light_Help, LightStatus.ON);
                    ChangeLightStatus(Light3288.Light_Auto, LightStatus.ON);
                    ChangeLightStatus(Light3288.Light_Select, LightStatus.ON);
                    ChangeLightStatus(Light3288.Light_DownSco, LightStatus.ON);
                    break;
                default:
                    break;
            }
        }

        private void OnCoinIn(object[] args)
        {
            int coinCount = (int)args[0];
            //StartCoroutine(PurchaseCreditRequest(1, coinCount));
            MachineSelectManager.Instance.PurchaseCreditRequest(1, coinCount);
        }

        private void OnCashIn(object[] args)
        {
            //todo
        }

        private void OnCoinOut(object[] args)
        {
            int coinCount = (int)args[0];
            if (coinOutNum - coinCount < 0)
                coinCount = coinOutNum;
            coinOutNum -= coinCount;
            //StartCoroutine(PurchaseCreditRequest(2, coinCount * 100));
            MachineSelectManager.Instance.PurchaseCreditRequest(2, coinCount * 100);
            if (coinOutNum == 0)
                IOEventCenter.SendEvent(IOCenterEvent.EVENT_StopCoinOut, args);
        }


        //加钱
        private void OnAddCredit(int data)
        {
            if (data == 0) {
                //StartCoroutine(PurchaseCreditRequest(1, 100));
                MachineSelectManager.Instance.PurchaseCreditRequest(1, 100);
            }
        }

        //扣钱
        private void OnSubCredit(int data)
        {
            if (data == 0)
            {
                //StartCoroutine(PurchaseCreditRequest(2, 100));
                MachineSelectManager.Instance.PurchaseCreditRequest(2, 100);
            }
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

        /// <summary>
        /// 按键事件响应
        /// </summary>
        /// <param name="args">按键状态 args[0]:按键序号 ,args[1]:按键状态</param>
        /// <param name="args[1]">0:按键抬起 ,1:按键按下</param>
        private void OnKeyStatus(object[] args)
        {
            bdKeyCode bdKeyCode = (bdKeyCode)args[0];
            //(int)args[1] 0: 抬起
            //(int)args[1] 1: 按下
            switch (bdKeyCode)
            {
                case global::bdKeyCode.Key_Up:
                    OnKeyUp((int)args[1]);
                    break;
                case global::bdKeyCode.Key_Down:
                    OnKeyDown((int)args[1]);
                    break;
                case global::bdKeyCode.Key_Left:
                    OnKeyLeft((int)args[1]);
                    break;
                case global::bdKeyCode.Key_Right:
                    OnKeyRight((int)args[1]);
                    break;
                case global::bdKeyCode.Key_KeyOut:
                    break;
                case global::bdKeyCode.Key_CoinOut:
                    OnKeyCointOut((int)args[1]);
                    break;
                case global::bdKeyCode.Key_Cancle:
                    break;
                case global::bdKeyCode.Key_Confirm:
                    break;
                case global::bdKeyCode.Key_Bet1:
                    //OnKeyUp((int)args[1]);
                    OnAddCredit((int)args[1]);
                    break;
                case global::bdKeyCode.Key_Bet2:
                    //OnKeyDown((int)args[1]);
                    OnSubCredit((int)args[1]);
                    break;
                case global::bdKeyCode.Key_Bet3:
                    //OnKeyLeft((int)args[1]);
                    OnKeySwitch((int)args[1]);
                    break;
                case global::bdKeyCode.Key_Bet4:
                    //OnKeyRight((int)args[1]);
                    OnKeyMenu((int)args[1]);
                    break;
                case global::bdKeyCode.Key_Bet5:
                    OnKeyExit((int)args[1]);
                    break;
                case global::bdKeyCode.Key_Undefine:
                    //OnKeyCointOut((int)args[1]);
                    break;
                case global::bdKeyCode.Key_Rules:
                    OnKeyRule((int)args[1]);
                    //OnKeyCointIn((int)args[1]);
                    break;
                case global::bdKeyCode.Key_Auto:
                    break;
                case global::bdKeyCode.Key_Tab:
                    OnMaxBet((int)args[1]);
                    break;
                case global::bdKeyCode.Key_KeyUnkown1:
                    OnKeyCointIn((int)args[1]);
                    break;
                case global::bdKeyCode.Key_KeyUnkown2:
                    Discharge((int)args[1]);
                    break;
                case global::bdKeyCode.Key_KeyCoinIn:
                    break;
                case global::bdKeyCode.Key_Set:
                    OnKeySet((int)args[1]);
                    break;
                case global::bdKeyCode.Key_Start:
                    OnKeyStart((int)args[1]);
                    break;
                case global::bdKeyCode.Key_Account:
                    OnKeyAccount((int)args[1]);
                    break;
                default:
                    break;
            }
        }

        private void OnKeyExit(int data)
        {
            /*if (data == 0)
            {
                //退出
                EventSender.SendGlobalEvent("OnLobby");
                //鱼房间退出
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_BET5));
                //鱼机 退出
                MessageDispatcher.Dispatch(MetaEventDefine.ON_MACHINE, new EventData(MachineEventDefine.ON_KEY_BET5));
            }*/

            if (data == 0)
            {
                MachineSelectManager.Instance.BtnReturn();
            }
        }


        private void OnKeyMenu(int data)
        {
            if (data == 0 && PopupManager.Instance.popupCount == 0)
            {
                MachineSelectManager.Instance.BtnMenu();
            }
        }


        private void OnKeyRule(int data)
        {
            if (data == 0 && PopupManager.Instance.popupCount == 0)
            {
                //EventSender.SendGlobalEvent("OpenPaytable");
                //MachineSelectManager.Instance.BtnMenu();
                MachineSelectManager.Instance.BtnHelp();
            }
        }

        private void OnMaxBet(int data)
        {
            /* if (data == 0)
             {
                 //最大下注
                 MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetMax"));
                 //炮升级
                 EventSender.SendGlobalEvent(MetaEventDefine.ON_MACHINE, new EventData<int>(MachineEventDefine.ON_KEY_MAX_BET, data));
             }*/

            if (data == 0)
            {
                MachineSelectManager.Instance.BtnBetMax();
            }
        }

        private void OnKeyCointOut(int data)
        {
            if (data == 0)
            {
                object[] arg = new object[1] {10};
                coinOutNum = 10;
                IOEventCenter.SendEvent(IOCenterEvent.Event_StartCoinOut, arg);


                ////下分
                //StartCoroutine(PurchaseCreditRequest(2, 100));
                MachineSelectManager.Instance.PurchaseCreditRequest(2, 100);
            }
        }

        private void OnKeyCointIn(int data)
        {
            if (data == 0)
            {
                //上分
                //StartCoroutine(PurchaseCreditRequest(1, 100));
                MachineSelectManager.Instance.PurchaseCreditRequest(1, 100);
            }
        }

        private void Discharge(int data)
        {
            if (data == 0)
            {
                //StartCoroutine(PurchaseCreditRequest(2, 100));
                MachineSelectManager.Instance.PurchaseCreditRequest(2, 100);
            }
        }

        private void OnKeyUp(int data)
        {
            if (data == 0)
            {
                EventSender.SendGlobalEvent(MetaEventDefine.ON_MACHINE, new EventData<int>(MachineEventDefine.ON_KEY_UP, data));
            }
        }

        private void OnKeyDown(int data)
        {
            if (data == 0)
            {
                EventSender.SendGlobalEvent(MetaEventDefine.ON_MACHINE, new EventData<int>(MachineEventDefine.ON_KEY_DOWN, data));

            }
        }


        private void OnKeySwitch(int data)
        {
            if (data == 0)
            {
                MachineSelectManager.Instance.BtnSwitch();
            }

        }



        private void OnKeyLeft(int data)
        {
            /*
            if (data == 0)
            {
                //选择框左移
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_LEFT));
                //降低押注
                MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetDown"));
            }
            //炮左移
            EventSender.SendGlobalEvent(MetaEventDefine.ON_MACHINE, new EventData<int>(MachineEventDefine.ON_KEY_LEFT, data));
            */

            if (data == 0)
            {
                MachineSelectManager.Instance.BtnPre();
            }

        }

        private void OnKeyRight(int data)
        {
            /*
            if (data == 0)
            {
                //选择框右移
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_RIGHT));
                //提高押注
                MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetUp"));
            }
            //炮右移
            EventSender.SendGlobalEvent(MetaEventDefine.ON_MACHINE, new EventData<int>(MachineEventDefine.ON_KEY_RIGHT, data));
            */
            if (data == 0)
            {
                MachineSelectManager.Instance.BtnNext();
            }
        }

        private void OnKeyStart(int data)
        {
            /*
            //Slot开始
            if (PopupManager.Instance.popupCount > 0 && data == 1)
            {
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Return"));
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Collect"));
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnClose"));
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnCollect"));
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnPointerClick"));
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnStartFreeSpin"));
            }
            else
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData<int>(MachineEventDefine.ON_KEY_START, data));
            ////开炮
            //EventSender.SendGlobalEvent(MetaEventDefine.ON_MACHINE, new EventData<int>(MachineEventDefine.ON_KEY_START, data));
            */



            //(int)args[1] 0: 抬起
            //(int)args[1] 1: 按下
            if (data == 1)
            {
                MachineSelectManager.Instance.BtnSpinDown();
            }
            else
            {
                MachineSelectManager.Instance.BtnSpinUp();
            }
             
        }

        private void OnKeySet(int data)
        {

            if (data == 0)
            {
                //提高押注
                //MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetUp"));
                MachineSelectManager.Instance.BtnBetUp();
            }
        }

        private void OnKeyAccount(int data)
        {
            if (data == 0)
            {
                //降低押注
                //MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetDown"));
                MachineSelectManager.Instance.BtnBetDown();
            }
        }

        private void ChangeLightStatus(Light3288 light3288, LightStatus status)
        {
            object[] args = new object[2]
            {
                (int)light3288,
                (int)status
            };
            IOEventCenter.SendEvent(IOCenterEvent.EVENT_OneLightStatus, args);
        }
    }
}
