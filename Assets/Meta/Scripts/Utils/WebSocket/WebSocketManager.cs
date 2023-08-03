using fishMsg;
using hall;
using ParadoxNotion;
using rpc;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityWebSocket;

namespace BagelCode
{
    public class WebSocketManager : MonoSingleton<WebSocketManager>
    {
        private IWebSocket socket;
        private string host;
        private bool lockReconnect;
        private Coroutine clientPing, serverPing;
        private float beating = 30;
        private HeartHeat heartHeat;

        private void Awake()
        {
            WebSocketState state = socket == null ? WebSocketState.Closed : socket.ReadyState;
            if (state == WebSocketState.Closed)
            {
                var webSocketBB = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "webSocket");
                host = webSocketBB.GetVariable<string>("wsHost").value;
            }
            
        }

        public void ConnetServer()
        {
            Debug.LogError("正在连接 ==> " + host);
            socket = new WebSocket(host);
            InitHandle();
            socket.ConnectAsync();
        }


        public WebSocketState GetSocketState()
        {
            return socket.ReadyState;
        }

        private void InitHandle()
        {
            RemoveAllHandle();
            socket.OnOpen += OnConnectedServer;
            socket.OnMessage += OnMessageReceived;
            socket.OnClose += OnClosed;
            socket.OnError += OnConnectError;
        }

        private void RemoveAllHandle()
        {
            socket.OnOpen -= OnConnectedServer;
            socket.OnMessage -= OnMessageReceived;
            socket.OnClose += OnClosed;
            socket.OnError -= OnConnectError;
        }

        public void OnConnectedServer(object sender, OpenEventArgs e)
        {
            Debug.LogError("webSocket连接成功  ====> ");
            FirstHeartBeat();
            HeartCheck();
            EventData eventData = new EventData("ConnectedFishServer");
            MessageDispatcher.Dispatch("ConnectedFishServer", eventData);
        }

        private void OnMessageReceived(object sender, MessageEventArgs e)
        {
            if (e.IsBinary)
            {
                rpc.RpcPacket rpcPacket = Deserialize<rpc.RpcPacket>(e.RawData);
                if (rpcPacket.MsgId == (int)HALL_CMD.HALL_CMD_HeartHeat_Rsp)
                    HeartCheck();
                else
                {
                    string msgName;
                    if (rpcPacket.MsgId == (uint)HALL_CMD.HALL_CMD_FishGame_Rsp)
                    {

                        msgName = Enum.GetName(typeof(Proto_Fish_CMD), rpcPacket.SubMsgId);

                        //未退出房间, 重新登录的特殊处理
                        if (!FishGameManager.Instance.IsGameContentLoadComplete())
                            return;
                    }
                    else
                    {
                        msgName = Enum.GetName(typeof(HALL_CMD), rpcPacket.MsgId);
                        if (!FishGameManager.Instance.IsResourceLoadComplete())
                        {
                            if (rpcPacket.MsgId == (uint)HALL_CMD.HALL_CMD_EnterGame_Rsp)
                            {
                                StartCoroutine(ReceiveEnterFishGameRsp(msgName, rpcPacket.RpcBody));
                                return;
                            }
                                
                        }
                    }
                    WebSocketTool.CheckReceiveMsg(msgName, rpcPacket.RpcBody);
                }

            }
            else if (e.IsText)
                Debug.LogError("接到text ==> " + e.Data);
        }

        IEnumerator ReceiveEnterFishGameRsp(string msgName, byte[] rpcBody)
        {
            yield return new WaitUntil(FishGameManager.Instance.IsGameContentLoadComplete);
            WebSocketTool.CheckReceiveMsg(msgName, rpcBody);
        }

        private void OnClosed(object sender, CloseEventArgs e)
        {
            Debug.LogError("Closed: StatusCode: " + e.StatusCode + " , Reason: " + e.Reason);
            socket = null;
            //Reconnect();
        }

        private void OnConnectError(object sender, ErrorEventArgs e)
        {
            Debug.LogError("webSocket连接出错  ====> " + e.Message);
            socket = null;
            //Reconnect();
        }

        private void Reconnect()
        {
            if (lockReconnect)
                return;
            lockReconnect = true;
            StartCoroutine(SetReconnect());
        }

        private IEnumerator SetReconnect()
        {
            Debug.LogError("正在重连webSocket");
            yield return new WaitForSeconds(5);
            ConnetServer();
            lockReconnect = false;
        }

        private void FirstHeartBeat()
        {
            if (heartHeat == null)
            {
                heartHeat = new HeartHeat();
                heartHeat.SendTime = GetCurrentTimestamp();
            }
            SendHallMessage(HALL_CMD.HALL_CMD_HeartHeat_Req, WebSocketTool.Serialize(heartHeat));
            serverPing = StartCoroutine(ServerPing());
        }

        private void HeartCheck()
        {
            if (clientPing != null)
            {
                StopCoroutine(clientPing);
                clientPing = null;
            }

            if (serverPing != null)
            {
                StopCoroutine(serverPing);
                serverPing = null;
            }

            clientPing = StartCoroutine(ClientPing());
        }

        private IEnumerator ClientPing()
        {
            yield return new WaitForSeconds(beating);
            if (heartHeat == null)
                heartHeat = new HeartHeat();
            heartHeat.SendTime = GetCurrentTimestamp();
            SendHallMessage(HALL_CMD.HALL_CMD_HeartHeat_Req, WebSocketTool.Serialize(heartHeat));
            serverPing = StartCoroutine(ServerPing());
        }

        private IEnumerator ServerPing()
        {
            yield return new WaitForSeconds(beating);
            socket.CloseAsync();
        }

        public void CloseConnect()
        {
            socket.CloseAsync();
        }

        public void SendHallMessage(HALL_CMD msgId, byte[] bytes)
        {
            Send((UInt32)msgId, bytes);
        }

        public void SendGameMessage(Proto_Fish_CMD SubMsgId, byte[] bytes)
        {

            Send((UInt32)HALL_CMD.HALL_CMD_FishGame_Req, bytes, (UInt32)SubMsgId);
        }

        private void Send(UInt32 MsgId, byte[] bytes, UInt32 subMsgId = 0)
        {
            if (socket == null || MsgId == 0)
                return;
            if (socket.ReadyState == WebSocketState.Open)
            {
                rpc.RpcPacket rpcPacket = new rpc.RpcPacket();
                rpcPacket.MsgId = MsgId;
                rpcPacket.SubMsgId = subMsgId;
                rpcPacket.ServiceType = rpc.SERVICE.CLIENT;
                rpcPacket.RpcBody = bytes;
                socket.SendAsync(WebSocketTool.Serialize(rpcPacket));
            }
        }

        private T Deserialize<T>(byte[] bytes)
        {
            T obj = WebSocketTool.Deserialize<T>(bytes);
            return obj;
        }

        private ulong GetCurrentTimestamp()
        {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            return (ulong)timeSpan.TotalSeconds;
        }

        protected override void OnDestroy()
        {

        }
    }
}
