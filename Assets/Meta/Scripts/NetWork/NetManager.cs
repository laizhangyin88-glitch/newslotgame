using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Timers;
using System.Linq;
using SlotMaker.Json;
using ParadoxNotion;
using BagelCode;
using SlotMaker;
using JSONNode = SimpleJSON.JSONNode;
using System.Text.RegularExpressions;
using BagelCode.ClientModels;
using Action = System.Action;

public class RequestType {

    public RequestType(object buffer, string rpcName, long time, bool force =false)
    {
        this.buffer = buffer;
        this.rpcName = rpcName;
        this.time = time;
        this.force = force;
    }

    /// <summary>打包后的数据</summary>
    public object buffer;

    /// <summary>rpc名称</summary>
    public string rpcName;

    /// <summary>发送数据时间戳</summary>
    public long time;

    /// <summary>是否在验证阶段也能发送</summary>
    public bool force;
}
public class NetManager:MonoSingleton<NetManager>, IHttp
{
    /*
    private static NetManager instance;
    public static NetManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NetManager();
            }
            return instance;
        }
    }*/

    bool _isSocketInit = false;
    private ISocket _socket = null;
    protected NetNodeState _state  = NetNodeState.Closed;
    protected int _autoReconnect = 0;

    /// <summary>网络参数</summary>
    protected NetConnectOptions _connectOptions = null;

    /// <summary>接收数据定时器</summary>
    protected System.Timers.Timer _receiveMsgTimer = null;

    /// <summary>心跳定时器</summary>
    protected System.Timers.Timer _keepAliveTimer = null;

    /// <summary>重连定时器</summary>
    protected System.Timers.Timer _reconnectTimer = null;

    /// <summary>多久没收到数据断开ms</summary>
    protected readonly int  _receiveTime = 8*1000; //   120 * 1000;
                                          //   
    /// <summary>心跳间隔ms</summary>
    protected readonly int _heartTime  = 5000;

    /// <summary>请求服务超时不重发</summary>
    protected readonly int _requestTimeOut = 150 *1000;

    /// <summary>重连间隔ms</summary>
    protected readonly int _reconnetTimeOut = 5000; //70 * 1000;  //

    protected  List<RequestType>  _requests = new List<RequestType>();

    private Dictionary<string, List<EventHandlerInfo>> _onceEventHandlerLst = new Dictionary<string, List<EventHandlerInfo>>();


    private bool isRun = false;
    private Queue<Action> taskQueue = new Queue<Action>();
    private void Update()
    {
        if (!isRun)
        {
            isRun = true;
            while (taskQueue.Count>0)
            {
                var task = taskQueue.Dequeue();
                task.Invoke();
            }
            isRun = false;
        }
    }

    private const string ON_SYSTEM_EVENT = "OnSystemEvent";
    private void Start()
    {
        MessageDispatcher.Register("OnWinEvent", tmp_GameEnd);
    }

    protected override void OnDestroy()
    {
        MessageDispatcher.UnRegister("OnWinEvent", tmp_GameEnd);
        base.OnDestroy();
    }

    private void tmp_GameEnd(ParadoxNotion.EventData eventData)
    {

        //Debug.Log($"@@i am GameEnd {eventData.name} {eventData.value}");

        // if (!eventData.name.Equals("SkipWin", StringComparison.Ordinal))
        //     return;

        if (eventData.name == "SkipWin" || eventData.name == "NoWin") { 

            globalStore.isPlay = false;

            long oldCredit = (long)(BlackboardUtils.FindVariable(MainBlackboard.Get(), "me/credit").value ?? 0);
            if (oldCredit != globalStore.newCredit)
            {
                Debug.LogWarning($"@ 玩家金币发生改变  oldCredit = {oldCredit} ，newCredit = {globalStore.newCredit}");

                BlackboardQueryUtils.SetMyCredit(globalStore.newCredit);
                //Debug.LogError($"Refresh {BlackboardUtils.FindVariable(MainBlackboard.Get(), "me/credit").value}");
                MessageDispatcher.Dispatch("OnCreditEvent", new EventData<bool>("UpdateNaviCredit", true));
                //EventSender.SendGlobalEvent("OnCreditEvent", "UpdateNaviCredit");
            }
        }
    }




    public void Init(ISocket socket)
    {
        // Debug.Log("【NetManager】i am init~~~~~");

        Debug.Log("@ 初始化 socket!");

        this.Clear();
        this._socket = socket;
    }


    private void Clear()
    {
        if (this._socket != null)
        {
            this._socket.onConnected = null;
            this._socket.onMessage = null;
            this._socket.onError = null;
            this._socket.onClosed = null;
            this._socket.Close();
            this._socket = null;
        }
        this.ClearTimer();
        this._isSocketInit = false;
        this._requests = new List<RequestType>();
        this.isRun = false;
        this.taskQueue = new Queue<Action>();
        this._onceEventHandlerLst = new Dictionary<string, List<EventHandlerInfo>>();


        this._state = NetNodeState.Closed;
        this._autoReconnect = 0;
        this._connectOptions = null;

        globalStore.gameState = GameState.Login;
        globalStore.nowGameID = -1;
        globalStore.gToken = null;
    }




    private void _InitSocket()
    {
        if (this._socket != null)
        {
            this._socket.onConnected = this.OnConnected;
            this._socket.onMessage = this.OnMessage;
            this._socket.onError = this.OnError;
            this._socket.onClosed = this.onClosed;
            this._isSocketInit = true;
        }
    }

    public bool Connect(NetConnectOptions options)
    {
        if (this._socket != null && this._state == NetNodeState.Closed) {
            if (!this._isSocketInit)
            {
                this._InitSocket();
            }
            this._state = NetNodeState.Connecting;

            if (!this._socket.Connect(options))
            {
                return false;
            }
            if (this._connectOptions == null)
            {
                this._autoReconnect = options.autoReconnect; //刷新重连次数
            }
            this._connectOptions = options;
            return true;
        }
        return false;
    }

    public bool isConnect(){
        if(this._socket == null || this._state == NetNodeState.Closed){
            return false;
        }
        return true;
    }

    private void OnConnected(object evt)
    {
        this._state = (globalStore.gameState == GameState.Hall || globalStore.gameState == GameState.Game) ? NetNodeState.Checking : NetNodeState.Working;

        //断线重连
        if (this._state == NetNodeState.Checking)
        {
            if (!string.IsNullOrEmpty(globalStore.gToken))
            { //先链接大厅

                JSONNode data = JSONNode.Parse("[]");
                data.Add(globalStore.gToken);
                this.SendMsgForce(RPCName.login, data);
            }
            else
            {
                //返回到登录界面
               /* BagelCodeHTTPError errMsg = new BagelCodeHTTPError();
                errMsg.url = "";
                errMsg.responseCode = 0;
                errMsg.errorCode = Error.UNKNOWN;
                errMsg.error = "token is null";
                GlobalErrorHandler.GlobalError(errMsg);*/
            }
        }
        else if (this._state == NetNodeState.Working)
        {
            this.OnChecked();
        }

        this._SendHeartbeat();
    }


    private void OnChecked()
    {
        List<RequestType> lst = this._requests.Where(item => item.force == false).ToList();
        RequestType last = lst.Count >0 ? lst[lst.Count - 1] : null;
        /*if (last != null)
        {
            lst.RemoveAt(lst.Count - 1);
        }*/
        this._requests.Clear();
        if (last != null)
        {
            if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - last.time < this._requestTimeOut)
            {
                lst.RemoveAt(lst.Count - 1);  //删除last
                this._Send(last.rpcName, last.buffer as string, false);  //最后一条重发
            }
           /*else
            {
                lst.Add(last);
            }*/
        }
        foreach (var item in lst)
        {
            JSONNode jsonNode = JSONNode.Parse(@"{""err"":408,""msg"":""请求超时""}");

            this._Emit(item.rpcName, jsonNode);
        }  

    }

    private int _Send(string rpcName, string buf , bool  force = false){

        var res = 1;

        if (this._state == NetNodeState.Working || force) {

            if (rpcName == "ping" || rpcName == "meta_info")
            {
                Debug.Log($"== 发送上行数据 ：{buf}");
            }
            else
            {
                Debug.Log($"==@ 发送上行数据 ：{buf}");
            }

            /*string aesBuf = rpcName == RPCName.login ? AesManager.Instance.TryLocalEncrypt(buf) :
                AesManager.Instance.TryEncrypt(buf);*/
            if (rpcName == RPCName.login)
            {
                AesManager.Instance.ResetAesKeyIv();
            }
            string aesBuf = AesManager.Instance.TryEncrypt(buf);
            res = this._socket.Send(aesBuf);
        } else if (this._state == NetNodeState.Checking || this._state == NetNodeState.Connecting) {
            res = 0;
        } else {
            res = -1;
        }

        if(res == 1 && this._receiveMsgTimer != null){

            this._receiveMsgTimer = new System.Timers.Timer(this._receiveTime);
            this._receiveMsgTimer.AutoReset = true; // 是否重复执行
            this._receiveMsgTimer.Elapsed += (object sender, ElapsedEventArgs e) => {
                taskQueue.Enqueue(() =>
                {
                    if (this._requests.Count > 0
                    && DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - this._requests[0].time >= this._receiveTime)
                    {

                        this._receiveMsgTimer.Stop();
                        this._receiveMsgTimer.Dispose();
                        this._receiveMsgTimer = null;

                        Debug.LogWarning("@网络接收数据超时，断开网络");
                        //重启socket
                        this._socket.Close();

                    }
                    else if (this._requests.Count == 0)
                    {
                        this._receiveMsgTimer.Stop();
                        this._receiveMsgTimer.Dispose();
                        this._receiveMsgTimer = null;
                    }
                });
            };
            //this._receiveMsgTimer.Enabled = true; //开始执行
            this._receiveMsgTimer.Start();
        }
    

        if (res == 1 || res == 0)
        {

            this._requests.RemoveAll(item => item.rpcName == rpcName);
            this._requests.Add(new RequestType(
                buf,
                rpcName,
                DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                force
                ));

        }
        else if (res == -1)
        {
            JSONNode jsonNode = JSONNode.Parse(@"{""err"":408,""msg"":""请求超时""}");
            this._Emit(rpcName, jsonNode);
        }

        return res;
    }


    private void _Emit(string rpcName, JSONNode data)
    {
        // json数据监听
        //this.eventTarget.emit(rpcName, data)
       // (data as JSONNode ).ToString();

        var eventData = new EventData<JSONNode>(rpcName, data as JSONNode);
        //EventSender.SendGlobalEvent(eventData);
        //MessageDispatcher
        //EventSender.SendGlobalEvent
        //MessageDispatcher.Dispatch(eventType, new EventData<T>(eventName, eventData));
        MessageDispatcher.Dispatch(rpcName, eventData);

        if (this._onceEventHandlerLst.ContainsKey(rpcName))
        {
            //List<EventHandlerInfo> ehs = new List<EventHandlerInfo>(this._EventHandlerLst[rpcName]);
            EventHandlerInfo[] ehs = this._onceEventHandlerLst[rpcName].ToArray();
            this._onceEventHandlerLst[rpcName].Clear();

            if (data["err"] == 0) //Convert.ToInt32(dat["err"])
            {
                foreach (var eh in ehs)
                {
                    eh.responseCallback(data);
                }
            }
            else
            {
                BagelCodeHTTPError errMsg = new BagelCodeHTTPError();
                errMsg.url = rpcName;
                errMsg.responseCode = data["err"]; // Convert.ToInt64(dat["err"]);
                errMsg.errorCode = Error.UNKNOWN;
                if (data.HasKey("msg"))
                {
                    errMsg.error = data["msg"].ToString();
                    Debug.LogWarning($"rpc name = {rpcName} ,err = {errMsg.responseCode} ,错误信息： {errMsg.error}");
                }
                else
                {
                    Debug.LogWarning($"rpc name = {rpcName} ,err = {errMsg.responseCode} ,服务器没有下发错误信息，对应的字段名：msg");
                }
                foreach (var eh in ehs)
                {
                    eh.errorCallback(errMsg);
                }
            }
        }

    }


    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="rpcName">协议名称</param>
    /// <param name="data">Dictionary&lt;string,object> / List&lt;object> / 类 如：RPCKenoClassic.ReqKenoSpin </param>
    /// <returns>void</returns>
    public void SendMsg(string rpcName,object data)
    {
        string buffer = null;
        if (data is JSONNode)
        {
            JSONNode node = JSONNode.Parse("{}");
            node.Add("protocol_key", rpcName);
            node.Add("data", (JSONNode)data);
            buffer = node.ToString();
        }
        else
        {
            Dictionary<string, object> jsonNode = new Dictionary<string, object>
                {
                    { "protocol_key", rpcName},
                    { "data", data},
                };
            buffer = SlotSimpleJson.SerializeObject(jsonNode);
        }
        this._Send(rpcName, buffer);
    }

    /// <summary>
    /// 强制发送数据
    /// </summary>
    /// <param name="rpcName">协议名称</param>
    /// <param name="data">Dictionary&lt;string,object> / List&lt;object> / 类 如：RPCKenoClassic.ReqKenoSpin </param>
    /// <returns>void</returns>
    private void SendMsgForce(string rpcName, object data)
    {
        string buffer = null;
        if (data is JSONNode)
        {
            JSONNode node = JSONNode.Parse("{}");
            node.Add("protocol_key", rpcName);
            node.Add("data", (JSONNode)data);
            buffer = node.ToString();
        }
        else
        {
            Dictionary<string, object> jsonNode = new Dictionary<string, object>
                {
                    { "protocol_key", rpcName},
                    { "data", data},
                };
            buffer = SlotSimpleJson.SerializeObject(jsonNode);
        }
        this._Send(rpcName, buffer , true);
    }


    private void _SendHeartbeat()
    {

        if (this._keepAliveTimer != null)
        {
            this._keepAliveTimer.Stop();
            this._keepAliveTimer.Dispose();
            this._keepAliveTimer = null;
        }
        this._keepAliveTimer = new System.Timers.Timer(this._heartTime);
        this._keepAliveTimer.AutoReset = true; // 是否重复执行
        this._keepAliveTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
        {
            taskQueue.Enqueue(() =>
            {

                JSONNode data = JSONNode.Parse("{}");
                //this.sendMsgForce("ping", data);
                this.SendMsgForce("ping", data);
            });
        };
        //this._keepAliveTimer.Enabled = true; //开始执行
        this._keepAliveTimer.Start();

    }


    private void OnMessage(object aesEvt)
    {

        string evt = AesManager.Instance.TryDecrypt(aesEvt as string);
        try
        {

            SimpleJSON.JSONNode dataDict = SimpleJSON.JSONNode.Parse(evt as string);



            if (!dataDict.HasKey("protocol_key"))
            {
                Debug.LogError($"服务器下行数据，没有protocol_key字段 ：{evt}");
                return;
            }
            else if (!dataDict.HasKey("data"))
            {
                Debug.LogError($"服务器下行数据，没有data字段 ：{evt}");
                return;
            }
            else if (!dataDict["data"].HasKey("err"))
            {
                Debug.LogError($"服务器下行数据，没有data.err字段 ：{evt}");
                return;
            }

            if (dataDict["protocol_key"] == "ping" || dataDict["protocol_key"] == "meta_info")
            {
                Debug.Log($"== 接受收到下行数据 ：{evt}");
            }
            else
            {
                Debug.Log($"==@ 接受收到下行数据 ：{evt}");
            }


            this.OnWebSocketMessage((string)dataDict["protocol_key"], dataDict["data"]);

        }
        catch (Exception ex)
        {
            Debug.LogError($"@【报错】: {ex}\n{evt}");
        }

    }



 
    private void OnWebSocketMessage(string rpcName, JSONNode data)
    {

        this._requests.RemoveAll(item => item.rpcName == rpcName);

        long err = data["err"].AsLong;


        // 实时刷新金钱
        if (err == 0 && data.HasKey("balance"))
        {
            globalStore.newCredit = data["balance"].AsLong;
        }


        //添加检测code 和msg 的逻辑
        switch (rpcName)
        {
            case RPCName.login:
                if (err == 0) {
                    AesManager.Instance.initAesKey(data["aes_key"]);
                    AesManager.Instance.initAesIv(data["aes_iv"]);
                }

                if (this._state == NetNodeState.Checking) //断线重连
                {

                    if (err != 0)
                    {
                        /*
                        //返回到登录界面
                        BagelCodeHTTPError errMsg = new BagelCodeHTTPError();
                       // errMsg.url = RPCName.login;
                       // errMsg.responseCode = err;
                        errMsg.errorCode = Error.UNKNOWN;
                       // errMsg.error = data["msg"].ToString() ?? "";
                        GlobalErrorHandler.GlobalError(errMsg);
                        */

                        this._autoReconnect = 0;
                        this._socket.Close();
                    }
                    else if ((globalStore.gameState == GameState.Game && globalStore.nowGameID != -1)|| globalStore.gameState == GameState.Hall)//断线重链
                    {
                        this.SendMsgForce(RPCName.lobby, null);
                    }
                    else//踢回登录
                    {
                        //返回到登录界面
                        /*
                        BagelCodeHTTPError errMsg = new BagelCodeHTTPError();
                        errMsg.errorCode = Error.UNKNOWN;
                        GlobalErrorHandler.GlobalError(errMsg);
                        */
                        this._autoReconnect = 0;
                        this._socket.Close();
                    }
                    return;
                }

                break;

            case RPCName.lobby:
                if (this._state == NetNodeState.Checking) //断线断线重连
                {
                    if (err != 0)
                    {
                        //返回到登录界面
                        /*
                        BagelCodeHTTPError errMsg = new BagelCodeHTTPError();
                        errMsg.url = RPCName.lobby;
                        errMsg.responseCode = err;
                        errMsg.errorCode = Error.UNKNOWN;
                        errMsg.error = data["msg"].ToString() ?? "";
                        GlobalErrorHandler.GlobalError(errMsg);
                        */
                        this._autoReconnect = 0;
                        this._socket.Close();
                    }
                    else if (globalStore.gameState == GameState.Hall) //重连大厅
                    {
                        Debug.LogWarning("@【重连】:成功重连大厅 ");
                        //断线重链,提回登录界面
                        globalStore.nowGameID = -1;
                        this._state = NetNodeState.Working;
                        this.OnChecked();
                    }
                    else if (globalStore.gameState == GameState.Game && globalStore.nowGameID != -1)//断线重链子游戏
                    {
                        Dictionary<string, object> req = new Dictionary<string, object> { { "game_id",globalStore.nowGameID} };
                        this.SendMsgForce(RPCName.enterGame, req);
                    }
                    else //踢回登录
                    {
                        //返回到登录界面
                        /*BagelCodeHTTPError errMsg = new BagelCodeHTTPError();
                        errMsg.errorCode = Error.UNKNOWN;
                        GlobalErrorHandler.GlobalError(errMsg);*/
                        this._autoReconnect = 0;
                        this._socket.Close();
                    }
                    return;
                }

                globalStore.gameState = GameState.Hall;
                globalStore.nowGameID = -1;

                break;

            case RPCName.enterGame://进入子游戏

                //断线重链子游戏
                if (this._state == NetNodeState.Checking && globalStore.gameState == GameState.Game)
                {

                    if (err != 0)
                    {
                        //返回到登录界面
                        /*BagelCodeHTTPError errMsg = new BagelCodeHTTPError();
                        errMsg.url = RPCName.enterGame;
                        errMsg.responseCode = err;
                        errMsg.errorCode = Error.UNKNOWN;
                        errMsg.error = data["msg"].ToString() ?? "";
                        GlobalErrorHandler.GlobalError(errMsg);*/
                        this._autoReconnect = 0;
                        this._socket.Close();
                    }
                    else { 

                        Debug.LogWarning("@【重连】:成功重连子游戏 ");
                        this._state = NetNodeState.Working;
                        this.OnChecked();
                    }

                    return;
                }

                globalStore.gameState = GameState.Game;
                globalStore.nowGameID = data["contents"]["game_info"]["game_id"].AsInt;

                Debug.LogWarning("@ 进入游戏 id = "+ globalStore.nowGameID);


                break; //普通进入游戏

            case RPCName.serverClose:  //顶号
                //返回到登录界面
                this._autoReconnect = 0;
                this._socket.Close();
                /* 
                BagelCodeHTTPError errMsg = new BagelCodeHTTPError();
                errMsg.url = RPCName.serverClose;
                errMsg.responseCode = err;
                errMsg.errorCode = Error.REPEAT_LOGIN_ANOTHER_DEVICE;
                errMsg.error = data["msg"].ToString() ?? "";
                GlobalErrorHandler.GlobalError(errMsg);
                */
                return;
            case RPCName.kenoSpin:
            case RPCName.slotSpin:
            case RPCName.jacksDeal:
            case RPCName.jacksDraw:
            case RPCName.jacksGambleStart:
            case RPCName.jacksGambleDeal:
            case RPCName.jacksGambleTake:
                break;
            case RPCName.metaInfo:
                break;
            case RPCName.ping:
                long oldCredit = (long)(BlackboardUtils.FindVariable(MainBlackboard.Get(), "me/credit").value ?? 0);
                //globalStore.newCredit = data["balance"].AsLong;
                if (oldCredit != globalStore.newCredit)
                {
                    if (globalStore.nowGameID == -1 || globalStore.isPlay == false) // 在大厅 或没有玩游戏
                    {
                        Debug.LogWarning($"@ 玩家金币发生改变  oldCredit = {oldCredit} ，newCredit = {globalStore.newCredit}");
                        BlackboardQueryUtils.SetMyCredit(globalStore.newCredit);
                        //Debug.LogError($"Refresh {BlackboardUtils.FindVariable(MainBlackboard.Get(), "me/credit").value}");
                        MessageDispatcher.Dispatch("OnCreditEvent", new EventData<bool>("UpdateNaviCredit", true));
                        //EventSender.SendGlobalEvent("OnCreditEvent", "UpdateNaviCredit");
                    }
                }
                break;
            default:
                break;
        }

        this._Emit(rpcName, data as JSONNode);

    }


    private void OnError(object evt)
    {
        //Debug.Log($"【WARN】:Network err {evt}");
        this.ClearTimer();

    }
    private void onClosed(object evt)
    {
        Debug.Log($"【WARN】:Network close {evt}" );
        this.ClearTimer();

        this._state = NetNodeState.Closed;

        // 自动重连
        if (this._autoReconnect != 0) {

            this._reconnectTimer = new System.Timers.Timer(this._reconnetTimeOut);
            this._reconnectTimer.AutoReset = false; // 是否重复执行
            this._reconnectTimer.Elapsed += (object sender, ElapsedEventArgs e) => {
                taskQueue.Enqueue(() =>
                {
                    Debug.LogWarning($"@【断线重连】:Network reconnection  {this._autoReconnect}");
                    //this._state = NetNodeState.Closed;
                    this.Connect(this._connectOptions);
                    if (this._autoReconnect > 0)
                    {
                        this._autoReconnect -= 1;
                    }
                });
            };
            //this._reconnectTimer.Enabled = true; //开始执行
            this._reconnectTimer.Start();

        } else {

            Debug.Log($"@【禁止】 websocket");
            //this._state = NetNodeState.Closed;

            List<RequestType> lst = new List<RequestType>(this._requests);
            this._requests.Clear();
            foreach (var item in lst)
            {
                JSONNode jsonNode = JSONNode.Parse(@"{""err"":408,""msg"":""请求超时""}");

                this._Emit(item.rpcName, jsonNode);
            }
        }
    }


    /// <summary>
    /// 清除所有定时器
    /// </summary>
    private void ClearTimer()
    {
        if (this._keepAliveTimer != null)
        {
            this._keepAliveTimer.Stop();
            this._keepAliveTimer.Dispose();
            this._keepAliveTimer = null;
        }

        if (this._receiveMsgTimer != null)
        {
            this._receiveMsgTimer.Stop();
            this._receiveMsgTimer.Dispose();
            this._receiveMsgTimer = null;
        }

        if (this._reconnectTimer != null)
        {
            this._reconnectTimer.Stop();
            this._reconnectTimer.Dispose();
            this._reconnectTimer = null;
        }
    }




    void temp_BeforePlay(string url)
    {
        switch (url)
        {
            case RPCName.kenoSpin:
            case RPCName.slotSpin:
            case RPCName.jacksDeal:
            case RPCName.jacksDraw:
            case RPCName.jacksGambleStart:
            case RPCName.jacksGambleDeal:
            case RPCName.jacksGambleTake:
                globalStore.isPlay = true;
               /*long oldCredit = (long)(BlackboardUtils.FindVariable(MainBlackboard.Get(), "me/credit").value ?? 0);
                //globalStore.newCredit = data["balance"].AsLong;
                if (oldCredit != globalStore.newCredit)
                {
                    BlackboardQueryUtils.SetMyCredit(globalStore.newCredit);
                    Debug.LogError($"Refresh {BlackboardUtils.FindVariable(MainBlackboard.Get(), "me/credit").value}");
                    MessageDispatcher.Dispatch("OnCreditEvent", new EventData<bool>("UpdateNaviCredit", true));
                    //EventSender.SendGlobalEvent("OnCreditEvent", "UpdateNaviCredit");
                }*/
                break;
        }
    }

    /// <summary>
    /// post发送数据
    /// </summary>
    /// <param name="url">协议名称</param>
    /// <param name="data">Dictionary&lt;string,object> / List&lt;object> / 类 如：RPCKenoClassic.ReqKenoSpin </param>
    /// <param name="responseCallback">Action<JSONNode> / 成功回调</param>
    /// <param name="errorCallback"> HTTPErrorCallback / 失败回调 </param>
    /// <returns>void</returns>
    public int Post(string url, object data, Action<JSONNode> responseCallback, HTTPErrorCallback errorCallback)
    {
        temp_BeforePlay(url);

        if (!this._onceEventHandlerLst.ContainsKey(url))
        {
            this._onceEventHandlerLst.Add(url, new List<EventHandlerInfo>());
        }
        int mark = CreatMark();
        this._onceEventHandlerLst[url].Add(new EventHandlerInfo(url, data, responseCallback, errorCallback, mark));
        this.SendMsg(url, data);
        return mark;
    }



    private int mark = 0;
    private int CreatMark()
    {
        if (++this.mark > 1000)
        {
            this.mark = 1;
        }
        return this.mark;
    }

    public int Get(string url, Action<JSONNode> responseCallback, HTTPErrorCallback errorCallback)
    {
        string rpcName = url.Split('?')[0];
        string pattern = @"(\?|&)([^=]+)=([^&]*)";
 
        MatchCollection matches = Regex.Matches(url, pattern);

        Dictionary<string, string> parameters = new Dictionary<string, string>();

        foreach (Match match in matches)
        {
            string key = match.Groups[2].Value;// 匹配到的参数名  
            string value = match.Groups[3].Value;// 匹配到的参数值  
            parameters.Add(key, value);
        }

        return this.Post(rpcName, parameters, responseCallback, errorCallback);
    }

    public void ClearRequest(int mark)
    {
        foreach (var kv in this._onceEventHandlerLst)
        {
            int i = 0;
            while (i < this._onceEventHandlerLst[kv.Key].Count)
            {
                if (this._onceEventHandlerLst[kv.Key][i].mark == mark)
                {
                    this._onceEventHandlerLst[kv.Key].RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }




    public static String toCamelCase(string key)
    {
        // 使用正则表达式将下划线及其后面的单词首字母替换为大写  
        string camelCase = Regex.Replace(key, @"_(\w)", (mth) => mth.Value[1].ToString().ToUpper());
        // 将第一个字符转换为小写，以符合小驼峰命名的规则  
        camelCase = char.ToLower(camelCase[0]) + camelCase.Substring(1);
        return camelCase;
    }



    /// <summary>
    /// json键值转为小驼峰写法
    /// </summary>
    /// <param name="str">协议名称</param>
    /// <returns>String</returns>
    public static String ChangeJsonKeyToCameCase(string str)
    {
        string pattern = "\"[a-z_]+\"[\\s]*:"; 
        MatchCollection matches = Regex.Matches(str, pattern);

        string output = str;
        foreach (Match match in matches)
        {
            string camelCase = toCamelCase(match.Value);
            output = output.Replace(match.Value, camelCase);
        }
        return output;
    }

    /*
    public List<object> InterceptHttpPost(string method)
    {
#if MOCK
        return MockManager.Instance.InterceptHttpPost(method);
#endif
        return new List<object> { "Do not intercept requests" };
    }*/
    
}

class EventHandlerInfo
{

    public EventHandlerInfo(string rpcName, object data, Action<JSONNode> responseCallback, HTTPErrorCallback errorCallback,int mark)
    {
        this.rpcName = rpcName;
        this.data = data;
        this.responseCallback = responseCallback;
        this.errorCallback = errorCallback;
        this.mark = mark;
    }

    public int mark;
    public string rpcName;
    public object data;
    public Action<JSONNode> responseCallback;
    public HTTPErrorCallback errorCallback;
}

public enum NetNodeState
{
    /// <summary>已关闭</summary>
    Closed,
    /// <summary>连接中</summary>
    Connecting,
    /// <summary>断线重连验证中</summary>
    Checking,
    /// <summary>可传输数据</summary>
    Working,
}


public class NetConnectOptions
{
    public NetConnectOptions(string url,int autoReconnect = 0)
    {
        this.url = url;
        this.autoReconnect = autoReconnect;
    }

    public string url;

    /// <summary>-1 永久重连，0不自动重连，其他正整数为自动重试次数</summary>
    public int autoReconnect = 0;

}

public enum GameState
{
    None,
    /// <summary>登录</summary>
    Login,
    /// <summary>大厅</summary>
    Hall,
    /// <summary>进入子游戏</summary>
    Game,
}


public interface IHttp
{
    int Post(string rpcName, object data, Action<JSONNode> responseCallback, HTTPErrorCallback errorCallback);

    int Get(string rpcName, Action<JSONNode> responseCallback, HTTPErrorCallback errorCallback);

    void ClearRequest(int mark);
}


public interface ISocket
{

    /// <summary>连接回调</summary>
    Action<object> onConnected { set; }

    /// <summary>消息回调</summary>
    Action<object> onMessage { set; }

    /// <summary>错误回调</summary>
    Action<object> onError { set; }

    /// <summary>关闭回调</summary>
    Action<object> onClosed { set; }


    /// <summary>
    /// 连接网络
    /// </summary>
    /// <param name="options">协议名称</param>
    /// <returns>true 开始链接 / false 链接异常</returns>
    bool Connect(NetConnectOptions options);


    /// <summary>
    /// 数据发送接口
    /// </summary>
    /// <param name="data">数据</param>
    /// <returns>发送成功返回1  / 发送失败返回-1</returns>
    int Send(object data);


    /// <summary>
    /// 关闭接口
    /// </summary>
    /// <returns>void</returns>
    void Close();
}

public class WebSock : ISocket
{
    Action<object> _onConnected = null;
    Action<object> _onMessage = null;
    Action<object> _onError = null;
    Action<object> _onClosed = null;

    public Action<object> onConnected { set { this._onConnected = value; } }         // 连接回调
    public Action<object> onMessage { set { this._onMessage = value; } }         // 消息回调 onMessage: (event: any)
    public Action<object> onError { set { this._onError = value; } }          // 错误回调
    public Action<object> onClosed { set { this._onClosed = value; } }           // 关闭回调

    private CancellationTokenSource cancellationTokenSource;

    private ClientWebSocket webSocket;

    public  bool Connect(NetConnectOptions options){

        if (this.webSocket != null)
        {
            if (this.webSocket.State == WebSocketState.Connecting || this.webSocket.State == WebSocketState.Open )
            {
                //Debug.Log("websocket connecting, wait for a moment...");
                string msg = this.webSocket.State == WebSocketState.Connecting ? "链接中..." : "已链接";
                Debug.LogWarning($"@【WS】： ws {msg}");
                return false;
            }
        }

        string url = options.url;

        this._Connect(url);

        return true;
    }


    async void _Connect(string url)
    {
        this.cancellationTokenSource = new CancellationTokenSource();
        try
        {
            this.webSocket = new ClientWebSocket();
            Debug.LogWarning($"@【WS】： ws 初始化并开始链接");
            await this.webSocket.ConnectAsync(new Uri(url), this.cancellationTokenSource.Token);

            // _StartListeningForMessages();
            //Debug.Log("WebSocket connected");

            Debug.LogWarning($"@【WS】： ws 链接成功  {webSocket.State}");

            if (this._onConnected != null)
            {
                this._onConnected(null);
            }


            byte[] buffer = new byte[65535];
            StringBuilder messageBuilder = new StringBuilder();

            while (webSocket != null && webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationTokenSource.Token);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string receivedData = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    messageBuilder.Append(receivedData);

                    if (result.EndOfMessage)
                    {
                        string completeMessage = messageBuilder.ToString();
                        if (this._onMessage != null)
                        {
                            this._onMessage(completeMessage);
                        }
                        messageBuilder.Clear();
                    }
                }
            }

        }
        catch (Exception ex)
        {
            WebSocketState state = webSocket != null ? webSocket.State : WebSocketState.None;
            Debug.LogWarning($"@【WS】： ws 报错 {state} \n {ex}");
            //Debug.LogError($"WebSocket connection error: {ex.Message}");

            if (webSocket != null)
            {
                this.webSocket.Dispose();
                this.webSocket = null;
            }

            if (this._onError != null)
            {
                this._onError(ex);
            }
        }
        finally
        {
            WebSocketState state = webSocket != null ? webSocket.State : WebSocketState.None;
            Debug.LogWarning($"@【WS】： ws 关闭 {state}");

            if (webSocket != null)
            {
                this.webSocket.Dispose();
                this.webSocket = null;
            }

            if (this._onClosed != null) { 
                this._onClosed(null);
            }
            else{
                Debug.LogWarning("_onClosed is null!");
            }
        }
    }

    public  int Send(object message)
    {
        if (this.webSocket != null && this.webSocket.State == WebSocketState.Open) {

            byte[] buffer = Encoding.UTF8.GetBytes(message as string);
            this.webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, this.cancellationTokenSource.Token);

            return 1;
        }

        return -1;
    }

    public void Close()
    {
        if (this.webSocket != null)
        {

            if (this.webSocket.State != WebSocketState.Closed && this.webSocket.State != WebSocketState.Aborted)
            {
                this.webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by user", this.cancellationTokenSource.Token);
            }
            
            this.webSocket.Dispose();
            this.webSocket = null;
        }
    }
}


