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
        switch (msg.handle)
        {
            case EventHandle.CHECK_SBOX_READY:
                BlizzEvent.EventCenter.Instance.EventTrigger(EventHandle.SBOX_READY, int.Parse(msg.data));
                break;
            case SBoxEventHandle.SBOX_SADNBOX_RESET:
                BlizzEvent.EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SADNBOX_RESET, int.Parse(msg.data));
                break;
            case SBoxSanboxEventHandle.COIN_IN:
                BlizzEvent.EventCenter.Instance.EventTrigger(SBoxSanboxEventHandle.COIN_IN, int.Parse(msg.data));
                break;
            case SBoxSanboxEventHandle.COIN_OUT:
                BlizzEvent.EventCenter.Instance.EventTrigger(SBoxSanboxEventHandle.COIN_OUT, int.Parse(msg.data));
                break;
            case SBoxEventHandle.SBOX_SADNBOX_COIN_OUT_START:
                BlizzEvent.EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SADNBOX_COIN_OUT_START, int.Parse(msg.data));
                break;
            case SBoxEventHandle.SBOX_SADNBOX_COIN_OUT_STOP:
                BlizzEvent.EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SADNBOX_COIN_OUT_STOP, int.Parse(msg.data));
                break;
            case SBoxSanboxEventHandle.COIN_OUT_TIMEOUT:
                BlizzEvent.EventCenter.Instance.EventTrigger(SBoxSanboxEventHandle.COIN_OUT_TIMEOUT, int.Parse(msg.data));
                break;
            case EventHandle.HARDWARE_KEY_DOWN:
                BlizzEvent.EventCenter.Instance.EventTrigger(EventHandle.HARDWARE_KEY_DOWN, (SBOX_SWITCH)((ulong.Parse(msg.data))));
                break;
            case EventHandle.HARDWARE_KEY_UP:
                BlizzEvent.EventCenter.Instance.EventTrigger(EventHandle.HARDWARE_KEY_UP, (SBOX_SWITCH)((ulong.Parse(msg.data))));
                break;
            case EventHandle.HARDWARE_KEY_CLICK:
                BlizzEvent.EventCenter.Instance.EventTrigger(EventHandle.HARDWARE_KEY_CLICK, (SBOX_SWITCH)((ulong.Parse(msg.data))));
                break;
            case EventHandle.HARDWARE_KEY_LONG_PRESS:
                BlizzEvent.EventCenter.Instance.EventTrigger(EventHandle.HARDWARE_KEY_LONG_PRESS, (SBOX_SWITCH)((ulong.Parse(msg.data))));
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
