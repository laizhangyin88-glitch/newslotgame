using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandboxApi;
using System;
namespace SboxSpace
{

    public enum IOCenterEvent
    {
        //底层事件
        EVENT_HardCheckOK = 100,
        EVENT_CoinIn,//投币
        EVENT_CoinOut,//退币
        EVENT_StopCoinOut,//停止退币
        EVENT_KeyStatus,//按键状态
        EVENT_CashIn,//进钞
        EVENT_PrintStatus,//打印机状态
        EVENT_CashStatus,//纸钞机状态
        EVENT_OneLightStatus,//灯状态
        EVENT_AllLightStatus,//灯状态
        EVENT_CahserList,//纸钞机列表
        EVENT_PrinterList,//打印机列表
        EVENT_PrintOK,//打印成功


        //普通事件
        Event_StartCoinOut =200,//开始退币
        Event_StartPrint,//开始打印
        Event_RejectCash,//拒收纸钞
        Event_SaveCredit,//存储积分
        Event_ReadCredit,//读取积分
        Event_SetCahserID,//设置纸钞机
        Event_SetPrinterID,//设置打印机
        Event_StartCoinCoder,  //操作码表
        Event_JPAdd,//收到押分累积彩金
        Event_UpdateJP,//更新彩金

        //游戏结果
        Event_SlotRetrun,//拉霸结果回传

        //网络
        Event_InitNetwork = 300,
        Event_RecvNetworkBox,
        Event_SendNetworkBox,
        Event_ServerSendNetwork,
        Event_NetworkErr,
        Event_NetworkJP,//同步彩金
        Event_NetworkPlayerWinJP,//玩家赢得彩金
        //算法卡底板
        Event_ClientSendBaox = 500,
    }
    public static class IOEventCenter
    {
        /// <summary>
        /// 事件委托
        /// </summary>
        public delegate void EventCallback(object[] args);
        static Dictionary<IOCenterEvent, List<EventCallback>> EventDic = new Dictionary<IOCenterEvent, List<EventCallback>>();

        public static bool AddListener(IOCenterEvent Event, EventCallback Callback)
        {
            List<EventCallback> Callbacks = null;
            if (!EventDic.TryGetValue(Event, out Callbacks))
            {
                Callbacks = new List<EventCallback>();
                EventDic.Add(Event, Callbacks);
            }
            Callbacks.Add(Callback);
            return true;
        }
        public static void RemoveListener(IOCenterEvent eventName, EventCallback listener)
        {

            List<EventCallback> listeners = null;
            if (EventDic.TryGetValue(eventName, out listeners))
            {
               // int index = listeners.Find(listener);
                listeners.RemoveAt(0);
                if (listeners.Count <= 0)
                {
                    EventDic.Remove(eventName);

                }
            }
        }
        public static void SendEvent(IOCenterEvent EventName, object[] args)
        {
            if (!EventDic.ContainsKey(EventName)) return;
            List<EventCallback> events = null;
            if (EventDic.TryGetValue(EventName, out events))
            {
                foreach (EventCallback temp in events)
                {
                    temp(args);
                }
            }
        }
    }
}