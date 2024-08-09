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
using Sirenix.OdinInspector;
using EventData = ParadoxNotion.EventData;
using Newtonsoft.Json;
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


    bool isTestCloseNet = false;
    [Button]
    void test_CloseNet()
    {
        isTestCloseNet = true;
        this._socket.Close();
    }

    [Button]
    void test_RecoveryNet()
    {
        isTestCloseNet = false;
        this.onClosed("test recovery net");
    }

    [Button]
    void test_OpenWinReturn2Login(string msg)
    {
        ReturnToLoginPage("Return to Login");
    }


    [Button]
    void test_ShowSeqID()
    {
        for (int i = 0; i<10;i++)
        {
            JSONNode data = JSONNode.Parse("{}");
            data.Add("cur_time", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            Post(RPCName.ping, data,
            (res) => {
                Debug.Log($"========= seq_id = {res["seq_id"]}  {this._onceEventHandlerLst[RPCName.ping].Count}");
                foreach (var item in this._onceEventHandlerLst[RPCName.ping])
                {
                    Debug.Log($"==seq_id = {item.seqID}");
                }
            },
            (err) =>
            {
                JSONNode res = JSONNode.Parse(err.response);
   
                Debug.Log($"=========Err seq_id = {res["seq_id"]}  {this._onceEventHandlerLst[RPCName.ping].Count}");
                foreach (var item in this._onceEventHandlerLst[RPCName.ping])
                {
                    Debug.Log($"==seq_id = {item.seqID}");
                }
            });
        }

        JSONNode jsonNode = JSONNode.Parse(string.Format("{{\"err\":408,\"msg\":\"请求超时\",\"seq_id\":{0}}}", -1));
        this._Emit(RPCName.ping, jsonNode);

    }


    void ReturnToLoginPage(string msg = "")
    {

        if (msg.Length >= 2 && msg[0] == '"' && msg[msg.Length - 1] == '"')
        {
            msg = msg.Substring(1, msg.Length - 2);
            msg = msg ?? "";
        }



        BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "sessionAlive", false);
        this._autoReconnect = 0;
        this._socket.Close();

        ErrorPopupInfo info = new ErrorPopupInfo();
        info.text = $"<size=32>{msg}</size>";
        info.type = ErrorPopupType.SystemReset;
        info.buttonText1 = "OK";
        info.callback1 = delegate
        {
            //this._autoReconnect = 0;
            //this._socket.Close();
        };
        ErrorPopupHandler.Instance.OpenError(info);
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
                ReturnToLoginPage($"token is error");
                return;
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
        this._requests.Clear();
        if (last != null)
        {
            if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - last.time < this._requestTimeOut)
            {
                lst.RemoveAt(lst.Count - 1);  //删除last
                this._Send(last.rpcName, last.buffer as string, false);  //最后一条重发
            }
        }
        foreach (var item in lst)
        {
            JSONNode jsonNode = JSONNode.Parse(@"{""err"":408,""msg"":""请求超时"",""seq_id"":-1}");

            this._Emit(item.rpcName, jsonNode);
        }
        MessageDispatcher.Dispatch("NetManagerEvent", new EventData("isChecked"));
    }

    private int _Send(string rpcName, string buf, bool force = false)
    {

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
            int seq_id = -1;
            Match match = Regex.Match(buf, "\"seq_id\":\\s*(\\d+)");
            if (match.Success)
            {
                string str = match.Groups[1].Value;// 提取数字  
                seq_id = int.Parse(str);
            }

            /*string.Format(
                          "UpdateCollectingGameLevelLocked failure. " +
                          "eventInfo != null:{0}, isLockedFeature:{1}, IsMetaEventLevelLock:{2}",
                          metaGameEnterInfo != null, isLockedFeature, MetaGameUtils.IsMetaEventLevelLock())*/

            //JSONNode jsonNode = JSONNode.Parse(@"{""err"":408,""msg"":""请求超时""}");
            JSONNode jsonNode = JSONNode.Parse(string.Format("{{\"err\":408,\"msg\":\"请求超时\",\"seq_id\":{0}}}", seq_id));
            this._Emit(rpcName, jsonNode);
        }

        return res;
    }


    public void On(string rpcName, MessageDispatcher.EventDelegate del)
    {
        MessageDispatcher.Register(rpcName, del);
    }
    public void Off(string rpcName, MessageDispatcher.EventDelegate del)
    {
        MessageDispatcher.UnRegister(rpcName, del);
    }
    private void _Emit(string rpcName, JSONNode data)
    {
        // json数据监听
        //this.eventTarget.emit(rpcName, data)
       // (data as JSONNode ).ToString();

        var eventData = new EventData<JSONNode>(rpcName, data as JSONNode);
        MessageDispatcher.Dispatch(rpcName, eventData);

        if (this._onceEventHandlerLst.ContainsKey(rpcName))
        {
            //List<EventHandlerInfo> ehs = new List<EventHandlerInfo>(this._EventHandlerLst[rpcName]);

            EventHandlerInfo[] ehs = new EventHandlerInfo[] { };

            if (data.HasKey("seq_id"))
             {
                 int seqID = data["seq_id"];
                 if (seqID == -1)
                 {
                     RequestType req = this._requests.Find(item => item.rpcName == rpcName);
                     EventHandlerInfo eh = null;
                     if (req != null)
                     {
                         Match match = Regex.Match((string)req.buffer, "\"seq_id\":\\s*(\\d+)");
                         if (match.Success)
                         {
                             string str = match.Groups[1].Value;
                             int seq_id = int.Parse(str);
                             eh = this._onceEventHandlerLst[rpcName].Find(item => item.seqID == seq_id);
                         }
                     }

                     if (eh != null)
                         this._onceEventHandlerLst[rpcName].Remove(eh);

                     ehs = this._onceEventHandlerLst[rpcName].ToArray();
                     this._onceEventHandlerLst[rpcName].Clear();

                     if (eh != null)
                         this._onceEventHandlerLst[rpcName].Add(eh);
                 }
                 else
                 {
                     EventHandlerInfo eh = this._onceEventHandlerLst[rpcName].Find(item => item.seqID == seqID);
                     if (eh != null)
                     {
                         this._onceEventHandlerLst[rpcName].Remove(eh);
                         ehs = new EventHandlerInfo[] { eh };
                     }
                 }
             }
             else
             {
                 ehs = this._onceEventHandlerLst[rpcName].ToArray();
                 this._onceEventHandlerLst[rpcName].Clear();
             }


            if ((int)data["err"] == 0) 
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
                    if (data.HasKey("seq_id"))  //把"seq_id" = -1 替换为对应id
                    {
                        data["seq_id"] = eh.seqID;
                    }
                    errMsg.response = data.ToString();
                    eh.errorCallback(errMsg);
                }
            }
        }
    }


    public void SendMsg(string rpcName, object data)
    {
        SendMsg(rpcName,data,-1);
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="rpcName">协议名称</param>
    /// <param name="data">Dictionary&lt;string,object> / List&lt;object> / 类 如：RPCKenoClassic.ReqKenoSpin </param>
    /// <returns>void</returns>
    private void SendMsg(string rpcName, object data ,int seq_id = -1)
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

        JSONNode nd = JSONNode.Parse(buffer);
        if(seq_id == -1)
            seq_id = CreatSeqID();
        if (!(rpcName == RPCName.login))
        {
            nd["data"].Add("seq_id", seq_id);
        }
        else
        {
            nd["data"].Add(seq_id);
        }
        buffer = nd.ToString();

        this._Send(rpcName, buffer);
    }

    /// <summary>
    /// 强制发送数据
    /// </summary>
    /// <param name="rpcName">协议名称</param>
    /// <param name="data">Dictionary&lt;string,object> / List&lt;object> / 类 如：RPCKenoClassic.ReqKenoSpin </param>
    /// <returns>void</returns>
    private void SendMsgForce(string rpcName, object data, int seq_id = -1)
    {
        string buffer = null;
        if (data is JSONNode)
        {

            //if (!(data as JSONNode).IsArray && !(data as JSONNode).HasKey("seq_id"))
            //    (data as JSONNode).Add("seq_id", CreatSeqID());

            JSONNode node = JSONNode.Parse("{}");
            node.Add("protocol_key", rpcName);
            node.Add("data", (JSONNode)data);
            buffer = node.ToString();
        }
        else
        {
            //if ((data is Dictionary<string, object>) && !(data as Dictionary<string, object>).ContainsKey("seq_id"))
            //    (data as Dictionary<string, object>).Add("seq_id", CreatSeqID());

            Dictionary<string, object> jsonNode = new Dictionary<string, object>
                {
                    { "protocol_key", rpcName},
                    { "data", data},
                };
            buffer = SlotSimpleJson.SerializeObject(jsonNode);
        }

        if (!(rpcName == RPCName.login))
        {
            JSONNode node = JSONNode.Parse(buffer);
            if (seq_id == -1)
                seq_id = CreatSeqID();
            node["data"].Add("seq_id", seq_id);
            buffer = node.ToString();
        }
        this._Send(rpcName, buffer , true);
    }

   

    string test_protocol_key = "";
    [Button]
    void test_SengMsg(string buf)
    {
       
        Match match = Regex.Match(buf, "\"protocol_key\":\\s*\"([a-zA-Z_]+)\"");
        if (match.Success)
        {
            test_protocol_key = match.Groups[1].Value;// 提取数字
            Debug.Log($" test_protocol_key = {test_protocol_key} ");
        }
        Debug.Log($"==@【发送测试数据】：{buf}");
        string aesBuf = AesManager.Instance.TryEncrypt(buf);
        this._socket.Send(aesBuf);
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
                data.Add("cur_time", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
                this.SendMsgForce(RPCName.ping, data);
            });
        };
        //this._keepAliveTimer.Enabled = true; //开始执行
        this._keepAliveTimer.Start();
    }


    private bool isJson(string str)
    {
        try
        {
            SimpleJSON.JSONNode temp = SimpleJSON.JSONNode.Parse(str);
            return true;
        }
        catch (Exception ex)
        {
            return false;
            // 抛出了异常，所以jsonString不是有效的JSON  
        }
    }

    private void OnMessage(object aesEvt)
    {
        //{"protocol_key":"login","data":{"err":11,"msg":"login token is over time"}}
        string evt = ""; //= AesManager.Instance.TryDecrypt(aesEvt as string);
        SimpleJSON.JSONNode dataDict = null;
        try
        {
            evt = AesManager.Instance.TryDecrypt(aesEvt as string);
            dataDict = SimpleJSON.JSONNode.Parse(evt as string);
        }
        catch (Exception ex)
        {
            Debug.LogError($"@ ERROR :{ex}");
            Debug.LogError($"@ 服务器数据没加密 : evt = {aesEvt}");

            if (isJson(aesEvt as string))
            {
                evt = aesEvt as string;  //这包数据服务器没有加密
                dataDict = SimpleJSON.JSONNode.Parse(evt as string);

                string msg = evt;

                if (!dataDict["data"].HasKey("msg"))
                {
                    Debug.LogError($"服务器下行数据，没有data.msg字段 ：{evt}");
                }
                else
                {
                    msg = dataDict["data"]["msg"];
                }
                ReturnToLoginPage(msg);
                return;
                /*if (dataDict["data"].HasKey("err") && dataDict["data"]["err"] != 0){
                    ReturnToLoginPage($"{msg}");
                    return;
                }*/
            }
            else
            {
                ReturnToLoginPage(aesEvt as string);
                return;
                //弹回登录界面
            }
        }


        try
        {

           // SimpleJSON.JSONNode dataDict = SimpleJSON.JSONNode.Parse(evt as string);

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


            if (evt.Contains("Sparkling"))
            {
                Debug.Log($"@【find Sparkling】：{evt}");
            }


            if (test_protocol_key != "")
            {
                test_protocol_key = "";
                return;
            }

            this.OnWebSocketMessage((string)dataDict["protocol_key"], dataDict["data"]);

        }
        catch (Exception ex)
        {
            Debug.LogError($"@【报错】: {ex}\n{evt}");
            //ReturnToLoginPage($"Error {ex}   evt = {evt}");
        }

    }

    private void OnWebSocketMessage(string rpcName, JSONNode data)
    {

        this._requests.RemoveAll(item => item.rpcName == rpcName);

        long err = data["err"].AsLong;



        // 实时刷新金钱
        if (err == 0 && data.HasKey("balance"))
        {
            long credit = data["balance"].AsLong;
            globalStore.newCredit = credit;
            NetData_Login.Instance.SetNetDataValue(NetData_Login.Path_UserCredit, credit);
        }else if (err == 0 && data.HasKey("after_credit"))
        {
            long credit = data["after_credit"].AsLong;
            globalStore.newCredit = credit;
            NetData_Login.Instance.SetNetDataValue(NetData_Login.Path_UserCredit, credit);
        }

        //添加检测code 和msg 的逻辑
        switch (rpcName)
        {
            case RPCName.login:
                if (err == 0) {
                    AesManager.Instance.initAesKey(data["aes_key"]);
                    AesManager.Instance.initAesIv(data["aes_iv"]);
                }

                //彩金
                if (data.HasKey("bonus_result"))
                    MessageDispatcher.Dispatch("SetJackpot", new EventData<string>("SetJackpot", data["bonus_result"].ToString()));

                if (data.HasKey("level"))
                    NetData_Login.Instance.SetNetDataValue(NetData_Login.Path_UserLevel, data["level"].AsInt);

                if (data.HasKey("profile_url"))
                    NetData_Login.Instance.SetNetDataValue(NetData_Login.Path_UserProfileUrl, data["profile_url"].Value);

                if (this._state == NetNodeState.Checking) //断线重连
                {

                    if (err != 0)
                    {
                        //返回到登录界面
                        /*
                        this._autoReconnect = 0;
                        this._socket.Close();
                        */
                        ReturnToLoginPage(data["msg"]??"");
                    }
                    else if ((globalStore.gameState == GameState.Game && globalStore.nowGameID != -1)|| globalStore.gameState == GameState.Hall)//断线重链
                    {
                        this.SendMsgForce(RPCName.lobby, null);
                    }
                    else//踢回登录
                    {
                        //返回到登录界面
                        /*
                        this._autoReconnect = 0;
                        this._socket.Close();
                        */
                        ReturnToLoginPage(data["msg"]??"");
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
                        this._autoReconnect = 0;
                        this._socket.Close();
                        */
                        ReturnToLoginPage(data["msg"]??"");
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
                        /*this._autoReconnect = 0;
                        this._socket.Close();*/

                        ReturnToLoginPage(data["msg"]??"");
                    }
                    return;
                }

                globalStore.gameState = GameState.Hall;
                globalStore.nowGameID = -1;

                //广告
                if (data.HasKey("l_ads"))
                {
                    var adJson = data["l_ads"];
                    List<ADSData> adsDatas = new List<ADSData>();
                    for (int i = 0; i < adJson.Count; i++)
                    {
                        ADSData adsData = new ADSData
                        {
                            imageUrl = adJson[i]["image_url"],
                            linkUrl = adJson[i]["page_url"],
                            showTime = adJson[i]["show_long"],
                            status = adJson[i]["status"],
                            sort = adJson[i]["sort"]
                        };
                        
                        if (adsData.status != 0)
                            adsDatas.Add(adsData);
                    }

                    adsDatas.Sort((a, b) => -a.sort.CompareTo(b.sort));

                    MainBlackboard.Get().SetValue("slotADurls", adsDatas);
                }

                //收藏
                if (data.HasKey("user_cache"))
                {
                    string res = (string)data["user_cache"];
                    var cacheJsonNode = JSONNode.Parse(res);
                    List<int> collectList = new List<int>();
                    if (cacheJsonNode.HasKey("userCollect"))
                    {
                        JSONNode res1 = JSONNode.Parse((string)cacheJsonNode["userCollect"]);
                        for (int i = 0; i < res1.Count; i++)
                            collectList.Add(res1[i]);
                    }
                    else
                    {
                        collectList = new List<int>();
                        cacheJsonNode.Add("userCollect", JsonConvert.SerializeObject(collectList));
                    }

                    MainBlackboard.Get().SetValue("userCache", cacheJsonNode);
                    MainBlackboard.Get().SetValue("collectList", collectList);
                }

                if (data.HasKey("agent_notice"))
                {
                    var noticeNode = data["agent_notice"];
                    if (noticeNode.IsArray)
                    {
                        List<NoticeData> noticeList = new List<NoticeData>();

                        foreach (var item in noticeNode)
                        {
                            var node = item.Value;
                            NoticeData noticeData = new NoticeData()
                            {
                                id = node["id"],
                                agent_id = node["agent_id"],
                                title = node["title"],
                                content = node["content"],
                                start_time = node["start_time"],
                                end_time = node["end_time"],
                                status = node["status"],
                                is_system = node["is_system"],
                                created_at = node["created_at"],
                                updated_at = node["updated_at"],
                                deleted_at = node["deleted_at"]
                            };

                            noticeList.Add(noticeData);
                        }

                        MainBlackboard.Get().SetValue("notices", noticeList);
                    }
                }

                break;
            case RPCName.enterGame://进入子游戏

                //断线重链子游戏
                if (this._state == NetNodeState.Checking && globalStore.gameState == GameState.Game)
                {

                    if (err != 0)
                    {
                        //返回到登录界面
                        /*this._autoReconnect = 0;
                        this._socket.Close();*/
                        ReturnToLoginPage(data["msg"]??"");
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
                /*this._autoReconnect = 0;
                this._socket.Close();
                */

                ReturnToLoginPage(data["msg"]?? "your account has been login on another device,please login again");
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
            case RPCName.gameBonusResult:
                MessageDispatcher.Dispatch("UpdateJackpot", new EventData<string>("UpdateJackpot", data["data"]["bonus_list"].ToString()));
                break;
            case RPCName.winGameBonus:
                if (!data.HasKey("win_result_list")) return;
                bool isWin = false;
                var winResultList = JsonConvert.DeserializeObject<List<WinResult>>(data["win_result_list"].ToString());
                string userId = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "me/userId").value;
                for (int i = 0; i < winResultList.Count; i++)
                {
                    var winResult = winResultList[i];
                    if (winResult.user_id == userId)
                    {
                        isWin = true;
                        
                        BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "winLobbyJackpotResult", winResult);
                        break;
                    }
                }
                if (isWin)
                {
                    if (BlackboardQueryUtils.IsSpin())
                        BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "isWinLobbyJackpot", isWin);
                    else
                    {
                        var sceneInfo = AssetBundleManager.LoadAsset<SceneInfoObject>(MetaStringDefine.LOBBY_BUNDLE_NAME, "Popup Win Lobby Jackpot Scene").GetSceneInfo();
                        var parent = GameObject.Find("Popup Manager/Area");
                        var sceneObj = SceneManager.LoadScene(parent.transform, sceneInfo);
                        PopupManager.Instance.Open(sceneObj);
                        sceneObj.SetActive(true);
                    }
                }
                break;
            case RPCName.ping:
            case RPCName.confirmAddCoinOrder:
            case RPCName.confirmCoinOutOrder:
            case RPCName.confirmPrintOrder:
            case RPCName.confirmAddMoneyOrder:
            case RPCName.addCredit:
            case RPCName.decreaseCredit:

                if(!isChangeCreditAnimation)
                    SetMyCredit();

                if (rpcName == RPCName.ping)
                {
                    if (data.HasKey("cur_time"))
                        MessageDispatcher.Dispatch("OnPing", new EventData<string>("ShowInfo", data.ToString()));
                }       

                break;
            default:
                break;
        }

        this._Emit(rpcName, data as JSONNode);

    }

    public bool isChangeCreditAnimation = false;
    public void SetMyCredit(long credit = -1)
    {
        int updateCreditState = (int)(BlackboardUtils.FindVariable(MainBlackboard.Get(), "updateCreditState")?.value ?? 1);

        if (updateCreditState != 1)return;

        long oldCredit = (long)(BlackboardUtils.FindVariable(MainBlackboard.Get(), "me/credit").value ?? 0);

        long newCredit = credit < 0? globalStore.newCredit : credit;

        Debug.LogWarning($"@ 玩家金币发生改变1  oldCredit = {oldCredit} ，newCredit = {newCredit}");

        if (oldCredit != newCredit)
        {
            //if (globalStore.nowGameID == -1 || globalStore.isPlay == false) // 在大厅 或没有玩游戏
            if (globalStore.nowGameID == -1 || !BlackboardQueryUtils.IsSpin()) // 在大厅 或没有玩游戏
            {
                Debug.LogWarning($"@ 玩家金币发生改变2  oldCredit = {oldCredit} ，newCredit = {newCredit}");
                BlackboardQueryUtils.SetMyCredit(newCredit);
                //Debug.LogError($"Refresh {BlackboardUtils.FindVariable(MainBlackboard.Get(), "me/credit").value}");
                MessageDispatcher.Dispatch("OnCreditEvent", new EventData<bool>("UpdateNaviCredit", true));
                //EventSender.SendGlobalEvent("OnCreditEvent", "UpdateNaviCredit");
            }
        }
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
        if (this._autoReconnect != 0  && !isTestCloseNet) {

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
                JSONNode jsonNode = JSONNode.Parse(@"{""err"":408,""msg"":""请求超时"",""seq_id"":-1}");

                this._Emit(item.rpcName, jsonNode);
            }

            MessageDispatcher.Dispatch("NetManagerEvent", new EventData("isClosed"));
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

        int _seqID = CreatSeqID();

        this._onceEventHandlerLst[url].Add(new EventHandlerInfo(url, data, responseCallback, errorCallback, _seqID));
        this.SendMsg(url, data, _seqID);
        return _seqID;
    }



    private List<EventHandlerInfo> RemoveEventHandlerByID(List<int> seqIDs)
    {
        List<EventHandlerInfo> targets = new List<EventHandlerInfo>();
        int i = 0;
        while(i < this._onceEventHandlerLst.Keys.Count) {

            if (seqIDs.Count == 0)
            {
                break;
            }
            string key = this._onceEventHandlerLst.Keys.ElementAt(i);
            var lst = this._onceEventHandlerLst[key];
            int j = 0;
            while (j < lst.Count && seqIDs.Count >0)
            {
                if (seqIDs.Contains(lst[j].seqID))
                {
                    targets.Add(lst[j]);
                    seqIDs.Remove(lst[j].seqID);
                    lst.RemoveAt(j);
                }
                else
                {
                    j++;
                }
            }
            i++;
        }
        return targets;
    }


    private List<EventHandlerInfo> RemoveEventHandlerByRpcname(string name)
    {

        if (this._onceEventHandlerLst.ContainsKey(name))
        {
            List<EventHandlerInfo> targets = this._onceEventHandlerLst[name];
            this._onceEventHandlerLst.Remove(name);
            return targets;
        }
        else
        {
            return null;
        }
    }

    private int seqID = 0;

    //List<int> existSeqIDs = new List<int>();
    private int CreatSeqID()
    {
        List<int> temp = new List<int>();
        foreach (KeyValuePair<string, List<EventHandlerInfo>> kv in this._onceEventHandlerLst)
        {
            foreach (EventHandlerInfo item in kv.Value)
            {
                temp.Add(item.seqID);
            }
        }
        while (temp.Contains(seqID))
        {
            if (++this.seqID > 10000)
                this.seqID = 1;
        }
        return seqID;
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
                if (this._onceEventHandlerLst[kv.Key][i].seqID == mark)
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


    public bool isHasRequest(string rpcName)
    {
        if (this._requests.Count>0)
        {
            return this._requests.Find(item => item.rpcName == rpcName) != null;
        }
        return false;
    }



    [Button]
    void test_ShowRpc()
    {

        foreach(var item in this._requests)
        {
            Debug.Log($"_requests = {item.rpcName}");
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

    public EventHandlerInfo(string rpcName, object data, Action<JSONNode> responseCallback, HTTPErrorCallback errorCallback, int seqID)
    {
        this.rpcName = rpcName;
        this.data = data;
        this.responseCallback = responseCallback;
        this.errorCallback = errorCallback;
        this.seqID = seqID;
    }

    public int seqID;
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
            Debug.LogWarning($"@【WS】： ws 初始化并开始链接  {url}");
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


