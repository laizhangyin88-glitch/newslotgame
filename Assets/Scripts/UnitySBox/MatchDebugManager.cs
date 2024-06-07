using BlizzEvent;
using Newtonsoft.Json;
using SBoxApi;
using SlotMaker;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using static SBoxApi.SBoxSandbox;

public class MatchDebugManager : MonoSingleton<MatchDebugManager>
{
    class UdpData
    {
        private readonly UdpClient udpClient;
        public UdpClient UdpClient => udpClient;
        private readonly IPEndPoint endPoint;
        public IPEndPoint EndPoint => endPoint;

        public UdpData(IPEndPoint endPoint, UdpClient udpClient)
        {
            this.endPoint = endPoint;
            this.udpClient = udpClient;
        }
    }

    class MatchDebugMsg
    {
        public string handle;
        public string data;
    }

    private Thread reciveThread;
    private Queue<string> reciveQueue = new Queue<string>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ThreadRecive();
    }

    private void Update()
    {
        if (reciveQueue.Count > 0)
        {
            string msg = reciveQueue.Dequeue();
            var matchDebugMsg = JsonConvert.DeserializeObject<MatchDebugMsg>(msg);
            DealWithMsg(matchDebugMsg);
        }
    }

    private void DealWithMsg(MatchDebugMsg msg)
    {
        List<string> strList;
        switch (msg.handle)
        {
            case EventHandle.CHECK_SBOX_SANBOX_READY:
                EventCenter.Instance.EventTrigger(EventHandle.CHECK_SBOX_SANBOX_READY, int.Parse(msg.data));
                break;
            case SBoxEventHandle.SBOX_SADNBOX_RESET:
                EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SADNBOX_RESET, int.Parse(msg.data));
                break;
            case SBoxSanboxEventHandle.COIN_IN:
                CoinInData condata = JsonConvert.DeserializeObject<CoinInData>(msg.data);
                EventCenter.Instance.EventTrigger(SBoxSanboxEventHandle.COIN_IN, condata);
                break;
            case SBoxSanboxEventHandle.COIN_OUT:
                EventCenter.Instance.EventTrigger(SBoxSanboxEventHandle.COIN_OUT, int.Parse(msg.data));
                break;
            case SBoxEventHandle.SBOX_SADNBOX_COIN_OUT_START:
                EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SADNBOX_COIN_OUT_START, int.Parse(msg.data));
                break;
            case SBoxEventHandle.SBOX_SADNBOX_COIN_OUT_STOP:
                EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SADNBOX_COIN_OUT_STOP, int.Parse(msg.data));
                break;
            case SBoxSanboxEventHandle.COIN_OUT_TIMEOUT:
                EventCenter.Instance.EventTrigger(SBoxSanboxEventHandle.COIN_OUT_TIMEOUT, int.Parse(msg.data));
                break;
            case EventHandle.HARDWARE_KEY_DOWN:
                EventCenter.Instance.EventTrigger(EventHandle.HARDWARE_KEY_DOWN, (SBOX_SWITCH)((ulong.Parse(msg.data))));
                break;
            case EventHandle.HARDWARE_KEY_UP:
                EventCenter.Instance.EventTrigger(EventHandle.HARDWARE_KEY_UP, (SBOX_SWITCH)((ulong.Parse(msg.data))));
                break;
            case EventHandle.HARDWARE_KEY_CLICK:
                EventCenter.Instance.EventTrigger(EventHandle.HARDWARE_KEY_CLICK, (SBOX_SWITCH)((ulong.Parse(msg.data))));
                break;
            case EventHandle.HARDWARE_KEY_LONG_PRESS:
                EventCenter.Instance.EventTrigger(EventHandle.HARDWARE_KEY_LONG_PRESS, (SBOX_SWITCH)((ulong.Parse(msg.data))));
                break;
            case SBoxEventHandle.SBOX_SADNBOX_MOTOR_TOUCH:
                EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SADNBOX_MOTOR_TOUCH, int.Parse(msg.data));
                break;
            case EventHandle.SBOX_SADNBOX_IS_MOTOR_BUSY:
                EventCenter.Instance.EventTrigger(EventHandle.SBOX_SADNBOX_IS_MOTOR_BUSY, bool.Parse(msg.data));
                break;
            case SBoxEventHandle.SBOX_SADNBOX_BILL_LIST_GET:
                strList = JsonConvert.DeserializeObject<List<string>>(msg.data);
                EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SADNBOX_BILL_LIST_GET, strList);
                break;
            case SBoxEventHandle.SBOX_SADNBOX_BILL_SELECT:
                EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SADNBOX_BILL_SELECT, int.Parse(msg.data));
                break;
            case SBoxEventHandle.SBOX_SADNBOX_BILL_APPROVE:
                EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SADNBOX_BILL_APPROVE, int.Parse(msg.data));
                break;
            case SBoxEventHandle.SBOX_SADNBOX_BILL_REJECT:
                EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SADNBOX_BILL_REJECT, int.Parse(msg.data));
                break;
            case SBoxSanboxEventHandle.BILL_STACKED:
                EventCenter.Instance.EventTrigger(SBoxSanboxEventHandle.BILL_STACKED);
                break;
            case SBoxSanboxEventHandle.BILL_IN:
                EventCenter.Instance.EventTrigger(SBoxSanboxEventHandle.BILL_IN, int.Parse(msg.data));
                break;
            case EventHandle.SBOX_SADNBOX_BILL_STATE:
                EventCenter.Instance.EventTrigger(EventHandle.SBOX_SADNBOX_BILL_STATE, int.Parse(msg.data));
                break;
            case SBoxEventHandle.SBOX_SADNBOX_PRINTER_LIST_GET:
                strList = JsonConvert.DeserializeObject<List<string>>(msg.data);
                EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SADNBOX_PRINTER_LIST_GET, strList);
                break;
            case SBoxEventHandle.SBOX_SADNBOX_PRINTER_SELECT:
                EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SADNBOX_PRINTER_SELECT, int.Parse(msg.data));
                break;
            case SBoxEventHandle.SBOX_SADNBOX_PRINTER_RESET:
                EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SADNBOX_PRINTER_RESET, int.Parse(msg.data));
                break;
            case SBoxEventHandle.SBOX_SADNBOX_PRINTER_FONTSIZE:
                EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SADNBOX_PRINTER_FONTSIZE, int.Parse(msg.data));
                break;
            case SBoxEventHandle.SBOX_SADNBOX_PRINTER_MESSAGE:
                EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SADNBOX_PRINTER_MESSAGE, int.Parse(msg.data));
                break;
            case SBoxEventHandle.SBOX_SADNBOX_PRINTER_DATESET:
                EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SADNBOX_PRINTER_DATESET, int.Parse(msg.data));
                break;
            case SBoxEventHandle.SBOX_SADNBOX_PRINTER_DATEGET:
                SBoxDate sBoxDate = JsonConvert.DeserializeObject<SBoxDate>(msg.data);
                EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SADNBOX_PRINTER_DATEGET, sBoxDate);
                break;
            case SBoxEventHandle.SBOX_SADNBOX_PRINTER_PAPERCUT:
                EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SADNBOX_PRINTER_PAPERCUT, int.Parse(msg.data));
                break;

        }
    }

    private void ThreadRecive()
    {
        reciveThread = new Thread(() =>
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 8091);
            UdpClient udpReceive = new UdpClient(endPoint);
            UdpData data = new UdpData(endPoint, udpReceive);
            udpReceive.BeginReceive(CallBackRecive, data);
        })
        {
            IsBackground = true
        };
        reciveThread.Start();
    }

    private void CallBackRecive(IAsyncResult ar)
    {
        try
        {
            UdpData state = ar.AsyncState as UdpData;
            IPEndPoint iPEndPoint = state.EndPoint;
            byte[] bytes = state.UdpClient.EndReceive(ar, ref iPEndPoint);
            reciveQueue.Enqueue(System.Text.Encoding.UTF8.GetString(bytes));
            state.UdpClient.BeginReceive(CallBackRecive, state);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            throw;
        }
    }

    public void SendUdpMessage(string handle, string data = null)
    {
        MatchDebugMsg matchDebugMsg = new MatchDebugMsg()
        {
            handle = handle,
            data = data,
        };
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(matchDebugMsg));
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(SBoxModel.Instance.matchIp), 8092);
        UdpClient udpClient = new UdpClient();
        udpClient.Send(bytes, bytes.Length, endPoint);
        udpClient.Close();
    }
}

class CointOutData
{
    public int id;
    public int count;
    public int type;
}
