using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Timers;
using SboxSpace;
using Newtonsoft.Json;
using SandboxApi;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;

public enum _NetCmd
{
    Server_Start = 100,     //游戏开始
    Server_DownTime,        //倒计时
    Server_Result,          //游戏结果
    Server_OnlineCheck,     //在线检测
    Server_Sate,            //游戏状态
    Server_Config,          //游戏参数配置
    Server_PlayerBet,       //押分回传
    Server_NetBet,          //全台押注信息
    Server_CancleBet,       //取消下注
    Server_ClcWon,          //计算赢分
    Server_Wait,            //等待开始
    Server_PlayLucky,       //播放幸运玩家动画
    Server_StopLucky,       //结束播放
    Server_Jack,            //结束播放
    Server_UpdateRoad,      //更新牌路
    Server_CloseBox,        //关箱
    Server_ResultAni,       //结果动画
    Server_FindRecord,      //查
    Server_HeartBeat,      //查
    Server_GetLastBouns,
    Server_StopPayCoins,
    Server_CreditAction,          //积分变化
    Server_CoinGameCreditAction,          //抢金币积分变化

    //加密
    Server_SyncPublicKey = 150,//同步公钥
    Server_SyncDynamicKey,//同步动态秘钥

    Client_PlayerBet = 200, //玩家下注
    Client_CancleBet,       //取消下注
    Client_WinJP,      //分机赢得彩金
    Client_Record,          //游戏记录
    Client_ACK,             //在线回传
                            // Client_LastBouns,          //剩余彩金
    Client_CreditAction,          //积分变化
    Client_CoinGameCreditAction,          //抢金币积分变化
    Client_ReqLucyStart,
}
public class ServerInfo
{
    public string IP { get; set; }
    public int port { get; set; }
}
//自动连接--主机3秒广播一次信息
public class NetworkDis : MonoBehaviour
{
    public static NetworkDis Inst;

    public bool Connect = false;
    private bool FirstStart = false;
    private bool IsHost = false;
    private float dtime = 0;
    //public TCPGameNet tcpTool = null;
    private UdpClient client = null;
    private IPEndPoint endpoint;

    private readonly int port = 7789;
    private Thread RcvThread = null;
    private string localip = "";
    private bool IsStop = false;
    private bool GetHost = false;
    private string HostIp = "";
    private int connect_step = 0;
    private int firstdis = 3;
    private float dismax = 1.0f;
    public int reConnect = 0;

    int CloseAllAniCount = 0;
  
    private int lostcount = 0;
    int pauseCount = 0;
    //通讯包处理
    private string TemRecv = "";
    //private string singlePacket = "";
    int recvHeartBeat = 0;
    private Queue<string> clientUdpDataQueue = new Queue<string>();
    //private int maxcount = 1;
    // MsgInfo cmd = new MsgInfo();
    DateTime beginTime = DateTime.Now;
    DateTime endTime = DateTime.Now;

    ConcurrentQueue<int> ServerSendDataQueue = new ConcurrentQueue<int>();

    ServerInfo serverinfo = new ServerInfo();
    private bool Connectflag = false;
#if UNITY_ANDROID && !UNITY_EDITOR
       //static AndroidJavaClass ajc= new AndroidJavaClass("com.unity3d.player.UnityPlayer");
      // static AndroidJavaObject jo = ajc.GetStatic<AndroidJavaObject>("currentActivity");
       private static AndroidJavaClass m_jc = null;//new AndroidJavaClass("com.unity3d.player.UnityPlayer");
       private static AndroidJavaObject jo = null;//m_jc.GetStatic<AndroidJavaObject>("currentActivity");
#endif
    public object ServJsonConvert { get; private set; }

    private void Awake()
    {

        if (Inst == null)
            Inst = this;
        
        localip = LocalIP();
        serverinfo.IP = localip;
        serverinfo.port = port;

       
    }
    void Start()
    {

        //Start_GameNet(CheckHardware.Inst.macID);
        TCPGameNet.Inst.ProcessRecv += RecvpProc;
        IOEventCenter.AddListener(IOCenterEvent.Event_InitNetwork, InitNetwork);
        IOEventCenter.AddListener(IOCenterEvent.Event_UpdateJP, ServerUpdateNetJP);
        //if (CheckHardware.Inst.macID == 0)
        //{
        //    IsHost = true;

        //}
        //else
        //{


        //}
    }
    void ServerUpdateNetJP(object[] args)
    {
        MsgInfo info = MemManager<MsgInfo>.tryGet(MemType.Mem_MsgInfo);
        info.cmd = (int)_NetCmd.Server_Jack;
        info.id = CheckHardware.Inst.macID;
        for (int i = 0; i < args.Length; i++)
            info.data[i] = (int)args[i];
        TCPGameNet.Inst.cClientTCPSend(JsonConvert.SerializeObject(info));
        MemManager<MsgInfo>.Reset(MemType.Mem_MsgInfo, info);
    }
    void InitNetwork(object[] args)
    {
        CheckHardware.Inst.useGroup = 1;
        if (CheckHardware.Inst.isHost == 1)
            Start_GameNet(0);
        else
        {
            CheckHardware.Inst.isHost = 0;
            Start_GameNet(1);
        }
        Debug.Log("分机初始化网络成功1");
    }
    void ClientSendServer(object[] args)
    {
        MsgInfo info = MemManager<MsgInfo>.tryGet(MemType.Mem_MsgInfo);
        info.cmd = (int)_NetCmd.Client_PlayerBet;
        info.id = CheckHardware.Inst.macID;
        for (int i = 0; i < args.Length; i++)
            info.data[i] = (int)args[i];
        TCPGameNet.Inst.cServerTCPSend(JsonConvert.SerializeObject(info));
        MemManager<MsgInfo>.Reset(MemType.Mem_MsgInfo, info);
    }
    void ClientSendServerWinJP(object[] args)
    {
        MsgInfo info = MemManager<MsgInfo>.tryGet(MemType.Mem_MsgInfo);
        info.cmd = (int)_NetCmd.Client_WinJP;
        info.id = CheckHardware.Inst.macID;
        for (int i = 0; i < args.Length; i++)
            info.data[i] = (int)args[i];
        TCPGameNet.Inst.SendServer(JsonConvert.SerializeObject(info));
        MemManager<MsgInfo>.Reset(MemType.Mem_MsgInfo, info);
    }
    void ServerSendClient(object[] args)
    {
        int id = 0;
        if (ServerSendDataQueue.TryDequeue(out id))
        {
            MsgInfo info = MemManager<MsgInfo>.tryGet(MemType.Mem_MsgInfo);
            info.cmd = (int)_NetCmd.Server_PlayerBet;
            info.id = 0;
            for (int i = 0; i < args.Length; i++)
                info.data[i] = (int)args[i];
            string fo = JsonConvert.SerializeObject(info);
            TCPGameNet.Inst.SendClientSingle(id, fo);
            CheckHardware.Inst.DebugLog("发送: " + fo);
            MemManager<MsgInfo>.Reset(MemType.Mem_MsgInfo, info);
        }
    }
    public void Start_GameNet(int id)
    {
        
        if (id == 0) //主机
        {
            SetNet_AutoConnect(true);
             IOEventCenter.AddListener(IOCenterEvent.Event_ServerSendNetwork, ServerSendClient);
            IsHost = true;
        }
        else
        {
            SetNet_AutoConnect(false);
            IOEventCenter.AddListener(IOCenterEvent.Event_SendNetworkBox, ClientSendServer);
            IOEventCenter.AddListener(IOCenterEvent.Event_NetworkPlayerWinJP, ClientSendServerWinJP);
        }
       
    }
    public void cClientTCPInit(string server_ip, int port)
    {

        TCPGameNet.Inst.InitSocket(server_ip, port);
    }
    public void cClientTCPSend(string cmd_str)
    {
        // tcpTool.clientSDataQueue.Enqueue(cmd_str);
        TCPGameNet.Inst.cClientTCPSend(cmd_str);
    }
    public void cCloseClientSocket()
    {
        TCPGameNet.Inst.CloseClient();
    }
    public bool isConnected()
    {
        return TCPGameNet.Inst.connect_flag;
    }
    public string cClientTCPRecv()
    {
        string str = "";
        str = TCPGameNet.Inst.GetMsg();
        return str;
    }
    public void cServerTCPInit(int port)
    {
        TCPGameNet.Inst.InitSocket(port);
    }
    public string cServerTCPRecv()
    {
        string str = "";
        str = TCPGameNet.Inst.GetClientMsg();
        return str;
    }
    string[] getSinglePacket(string  str )
    {
      return   str.Split('#');
    }
    public  void cServerTCPSend(string cmd_str)
    {
        //tcpTool.clientSDataQueue.Enqueue(cmd_str);
        TCPGameNet.Inst.cServerTCPSend(cmd_str);
    }
    public void cServerTCPSendA(int id, string cmd_str)
    {
        //tcpTool.clientSDataQueueA.Enqueue(cmd_str);
        TCPGameNet.Inst.SendClientSingle(id,cmd_str);
    }
    private void SetNet_AutoConnect(bool Host)
    {
        TCPGameNet.Inst.SetNetAutoConnect(Host);
        //IsHost = Host;
        //FirstStart = true;
        //if (IsHost)
        //{
        //    //NetworkManager.singleton.StartHost();
        //    //本机的ip地址
        //    client = new UdpClient(new IPEndPoint(IPAddress.Parse(localip), 0));
        //    endpoint = new IPEndPoint(IPAddress.Broadcast, 10000);
        //    cServerTCPInit(port);
        //}
        //else
        //{
        //    client = new UdpClient(new IPEndPoint(IPAddress.Any, 10000));
        //    endpoint = new IPEndPoint(IPAddress.Any, 0);

        //    RcvThread = new Thread(new ThreadStart(RcvMsg))
        //    {
        //        IsBackground = true
        //    };
        //    RcvThread.Start();
        //}
    }


    private string LocalIP()
    {
        string AddressIP = string.Empty;
        string IP = "";
        IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());   //Dns.GetHostName()获取本机名Dns.GetHostAddresses()根据本机名获取ip地址组
        foreach (IPAddress ip in ips)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                IP = ip.ToString();  //ipv4
            }
        }
        return IP;
    }

    public void SendMsg(string ctrlMsg)
    {
        byte[] bf = Encoding.UTF8.GetBytes(ctrlMsg);
        client.Send(bf, bf.Length, endpoint);
    }

    private void RcvMsg()
    {
        while (!IsStop)
        {
            byte[] buf = client.Receive(ref endpoint);
            string msg = Encoding.UTF8.GetString(buf);
            if (string.IsNullOrEmpty(msg))
            {

            }
            else
            {
                //recvHeartBeat = 0;
                clientUdpDataQueue.Enqueue(msg);
                if (!GetHost)
                {
                    HostIp = endpoint.Address.ToString();
                    serverinfo = (ServerInfo)JsonConvert.DeserializeObject(msg, typeof(ServerInfo));
                    GetHost = true;
                }

            }
        }
    }
    public void RecvpProc(int clientid, string singlePacket)
    {
        if (singlePacket == null)
            return;
        if (IsHost)
        {
           
            //singlePacket = cmd;
            if (singlePacket != "")
            {
                //Debug.Log("数据 ： " + singlePacket);
                // Debug.Log("长度 ： " + singlePacket.Length);
                 MsgInfo info = MemManager<MsgInfo>.tryGet(MemType.Mem_MsgInfo);
                //  MsgBetInfo info1 = null;
                try
                {

                    // if(singlePacket.Length>100)
                    //MsgInfo info1 = MemManager<MsgInfo>.tryGet(MemType.Mem_MsgInfo);
                    // info = JsonUtility.FromJson<MsgInfo>(singlePacket);
                    //  else
                    //    info1 = (MsgBetInfo)JsonConvert.DeserializeObject(singlePacket, typeof(MsgBetInfo));
                    //JObject item = JObject.Parse(singlePacket);
                    //info.id = item["id"].Value<int>();
                    //info.cmd = item["cmd"].Value<int>();
                    //info.socketid = item["socketid"].Value<int>();
                    //info.info = item["info"].Value<string>();
                    //JArray arrdata2 = JArray.Parse(item["data"].ToString());
                    //for( int i = 0; i < arrdata2.Count; i ++ )
                    //    info.data[i] = arrdata2[i].Value<int>();
                    //arrdata2.ClearItems();
                    //item.ClearItems();
                    info = JsonConvert.DeserializeObject<MsgInfo>(singlePacket);
                }
                catch (System.Exception ex)
                {
                    Debug.Log("JSON : " + ex.Message);
                }

                if (info != null)
                {
                    switch ((_NetCmd)info.cmd)
                    {
                        case _NetCmd.Client_WinJP:
                            {
                                IOEventCenter.SendEvent(IOCenterEvent.Event_NetworkPlayerWinJP, new object[2] { info.data[0], info.data[1] });
                            }
                            break;
                        case _NetCmd.Client_ACK:
                            {
                                MsgInfo cmd = MemManager<MsgInfo>.tryGet(MemType.Mem_MsgInfo);//new MsgInfo();
                                cmd.cmd = (int)_NetCmd.Server_HeartBeat;
                                cmd.id = -1;
                               // cmd.data = new int[2];
                                cmd.data[0] = IO5255.Inst.dataSet.major_curr;
                                cmd.data[1] = IO5255.Inst.dataSet.minor_curr;
                                cmd.data[2] = IO5255.Inst.dataSet.mini_curr;
                                string s = JsonConvert.SerializeObject(cmd);
                                cServerTCPSendA(clientid,s);
                                MemManager<MsgInfo>.Reset(MemType.Mem_MsgInfo, cmd);
                            }
                            break;
                        case _NetCmd.Client_Record:
                            {
                                if (info.id < IO5255.Inst.dataSet.MaxPlayer)
                                {
                                    //HostInfo.Instance.online[info.id] = 1;
                                    //for (int i = 0; i < 6; i++)
                                    //{
                                    //    HostInfo.Instance.playrecord[info.id, i] = info.data[i];
                                    //}
                                }
                            }
                            break;
                        case _NetCmd.Client_PlayerBet:
                            {
                                //netPlayer.ClcBetLimit(info.id, info.data);
                                object[] args = new object[20];
                                for (int i = 0; i < args.Length; i++)
                                    args[i] = info.data[i];
                                IOEventCenter.SendEvent(IOCenterEvent.Event_ClientSendBaox, args);
                                ServerSendDataQueue.Enqueue(clientid);
                            }
                            break;
                        case _NetCmd.Client_CancleBet:
                            {
                                //netPlayer.CancleBet(info.id, info.data);
                            }
                            break;
                        //case _NetCmd.Client_LastBouns:
                        //    {
                        //        // int score = info.data[0];
                              
                        //        if (HostInfo.Instance.playjack[info.id] >= info.data[0])
                        //        {
                        //            HostInfo.Instance.playjack[info.id] -= info.data[0];
                        //        }
                        //    }
                        //    break;
                        case _NetCmd.Client_CreditAction:
                            {
                                if (info.data[0] < IO5255.Inst.dataSet.MaxPlayer && info.data[0] >= 0)
                                {
                                    int id = info.data[0];
                                    //if (info.data[1] > 0)
                                    //    HostInfo.Instance.playercredit[id] += info.data[1];//投币
                                    //if (HostInfo.Instance.playercredit[id] >= info.data[2] && info.data[2] > 0)
                                    //    HostInfo.Instance.playercredit[id] -= info.data[2];//退币
                                    //if (info.data[3] > 0)
                                    //    HostInfo.Instance.playercredit[id] += info.data[3];//上分
                                    //if (HostInfo.Instance.playercredit[id] >= info.data[4] && info.data[4] > 0)
                                    //    HostInfo.Instance.playercredit[id] -= info.data[4];//下分

                                    //if (info.data[1] > 0)
                                    //    HostInfo.Instance.playrecord[id, 2] += info.data[1];//投币
                                    //if (info.data[2] > 0)
                                    //    HostInfo.Instance.playrecord[id, 3] += info.data[2];//退币
                                    //if (info.data[3] > 0)
                                    //    HostInfo.Instance.playrecord[id, 4] += info.data[3];//上分
                                    //if (info.data[4] > 0)
                                    //    HostInfo.Instance.playrecord[id, 5] += info.data[4];//下分

                                    //netPlayer.TargetPlayerCreditAction((_NetCmd)info.cmd, info.data[0], info.data);
                                    ////HostInfo.Instance.Send_savePlayerInfo();
                                    //HostInfo.Instance.saveInfo = 1;

                                    //CheckHardware.Inst.DebugLog(" 分数:  " + HostInfo.Instance.playercredit[id] + ", " + info.data[3] + "," + info.data[4]);
                                }
                            }
                            break;
                        case _NetCmd.Client_CoinGameCreditAction:
                            {
                                if (info.data[0] < IO5255.Inst.dataSet.MaxPlayer && info.data[0] >= 0)
                                {
                                    int id = info.data[0];
                                    //if (HostInfo.Instance.playjack[info.id] >= info.data[1])
                                    //{
                                    //    HostInfo.Instance.playjack[info.id] -= info.data[1];
                                    //    HostInfo.Instance.playercredit[id] += info.data[1];
                                    //    HostInfo.Instance.playrecord[id,1] += info.data[1];
                                    //    netPlayer.TargetPlayerCoinGameCreditAction((_NetCmd)info.cmd, info.data[0], info.data);
                                    //    HostInfo.Instance.saveCoinParty += info.data[1];
                                    //    HostInfo.Instance.saveInfo =1;
                                    //}                                                                      
                                }
                            }
                            break;
                        case _NetCmd.Client_ReqLucyStart:
                            {
                                //if (HostInfo.Instance.lucky_type > 0&& HostInfo.Instance.lucky_type == info.data[1])
                                //{
                                //    //int id = info.data[0];
                                //    NetPlayer.Inst.RpcPlayLuckyA(HostInfo.Instance.lucky_type, IO5255.Inst.dataSet.MaxPlayer, IO5255.Inst.dataSet.jack_type);
                                //}
                            }
                            break;
                    }
                }
                MemManager<MsgInfo>.Reset(MemType.Mem_MsgInfo, info);
            }
            
        }
        else //分机
        {



            //singlePacket = cmd;// cClientTCPRecv();

            if (singlePacket != "")
            {
                MsgInfo info = MemManager<MsgInfo>.tryGet(MemType.Mem_MsgInfo);
                // MsgBetInfo info1 = null;
                try
                {
                    //   if (singlePacket.Length > 100)
                    // info = (MsgInfo)JsonConvert.DeserializeObject(singlePacket, typeof(MsgInfo));
                    //   else
                    //       info1 = (MsgBetInfo)JsonConvert.DeserializeObject(singlePacket, typeof(MsgBetInfo));

                    //JObject item = JObject.Parse(singlePacket);
                    //info.id = item["id"].Value<int>();
                    //info.cmd = item["cmd"].Value<int>();
                    //info.socketid = item["socketid"].Value<int>();
                    //info.info = item["info"].Value<string>();
                    //JArray arrdata2 = JArray.Parse(item["data"].ToString());
                    //for (int i = 0; i < arrdata2.Count; i++)
                    //    info.data[i] = arrdata2[i].Value<int>();
                    //arrdata2.ClearItems();
                    //item.ClearItems();

                    info = JsonConvert.DeserializeObject<MsgInfo>(singlePacket);
                }
                catch (System.Exception ex)
                {
                    Debug.Log("JSON : " + ex.Message);
                }
                if (info != null)
                {
                    if (info.id == -1 || info.id == CheckHardware.Inst.macID)
                    {
                        recvHeartBeat = 0;
                        switch ((_NetCmd)info.cmd)
                        {
                            case _NetCmd.Server_OnlineCheck:
                                {
                                    //PlayerInfo.Inst.onlinecmd = 1;
                                }
                                break;
                            case _NetCmd.Server_Config:
                                {
                                   // //netPlayer.RpcInitConfig(info.data, HostInfo.Instance.result, HostInfo.Instance.lucky);
                                   // //if (PlayerInfo.Inst.net_init == false)
                                   // //{
                                   // //    MainGame.Inst.Player_outline();
                                   // //}

                                   // //if (NetBetInfo.Instance.luckytype > 0)
                                   // //{
                                   // //    NetBetInfo.Instance.stoplucky = 1;
                                   // //}

                                   // NetBetInfo.Instance.mpplay = 1;
                                   // NetBetInfo.Instance.mpid = 0;
                                   // NetBetInfo.Instance.isLucyStart = 0;
                                   // PlayerInfo.Inst.haoyun_id = 0;
                                   // //初始客户端配置
                                   // PlayerInfo.Inst.coin = info.data[0];
                                   // PlayerInfo.Inst.coinbak = PlayerInfo.Inst.coin;
                                   // PlayerInfo.Inst.cm[0] = info.data[1];
                                   // PlayerInfo.Inst.cm[1] = info.data[2];
                                   // PlayerInfo.Inst.cm[2] = info.data[3];
                                   // PlayerInfo.Inst.minbet = info.data[4];
                                   // PlayerInfo.Inst.g_display_mode = info.data[5];
                                   // PlayerInfo.Inst.LOTX = info.data[6];
                                   // PlayerInfo.Inst.LOTX1 = info.data[7];
                                   
                                   // if (NetBetInfo.Instance.ju != info.data[8])
                                   // {
                                   //     //if(info.data[8] == 1)
                                   //        PlayerInfo.Inst.clearShowLuzu = 1;
                                   // }
                                   // NetBetInfo.Instance.ju = info.data[8];

                                   // NetBetInfo.Instance.lun = info.data[9];
                                   //// Debug.Log( "  轮局：" + NetBetInfo.Instance.ju + " " + NetBetInfo.Instance.lun);
                                   // PlayerInfo.Inst.net_init = true;

                                   // PlayerInfo.Inst.ttbet = 0;

                                   // NetBetInfo.Instance.resetinit = 1;
                                   // NetBetInfo.Instance.maxtwin = info.data[10];
                                   // PlayerInfo.Inst.coinValbak = info.data[11];
                                   // PlayerInfo.Inst.maxbet = info.data[12];
                                   // //记录
                                   // for (int i = 0; i < 100; i++)
                                   //     NetBetInfo.Instance.result[i] = info.data[i + 13];
                                   // for (int i = 0; i < 100; i++)
                                   //     NetBetInfo.Instance.lucky[i] = info.data[i + 113];
                                   // PlayerInfo.Inst.credit = info.data[PlayerInfo.Inst.id + 214];
                                   // PlayerInfo.Inst.credit_tmp = PlayerInfo.Inst.credit;

                                   // //for (int i = 0; i < 6; i++)
                                   // //{
                                   // //    PlayerInfo.Inst.playin = info.data[6*PlayerInfo.Inst.id + i];
                                   // //     PlayerInfo.Inst.playout = info.data[6 * PlayerInfo.Inst.id + i+1];
                                   // //    PlayerInfo.Inst.coinin = info.data[6 * PlayerInfo.Inst.id + i+2];
                                   // //    PlayerInfo.Inst.coinout = info.data[6 * PlayerInfo.Inst.id + i+3];
                                   // //    PlayerInfo.Inst.keyin = info.data[6 * PlayerInfo.Inst.id + i+4];
                                   // //    PlayerInfo.Inst.keyout = info.data[6 * PlayerInfo.Inst.id + i+5];
                                   // //}

                                   // for (int i = 0; i < 5; i++)
                                   // {
                                   //     PlayerInfo.Inst.bet[i] = 0;
                                   //     NetBetInfo.Instance.netbet[i] = 0;
                                   // }
                                   // NetBetInfo.Instance.netbetflag = 0;
                                   // NetBetInfo.Instance.netbetflag2 = 0;

                                   // NetBetInfo.Instance.playerbetflag = 0;
                                   // NetBetInfo.Instance.playerbetflag2 = 0;
                                    
                                   // CloseAllAniCount = 0;

                                    //UiData.Inst.HideRing(true);
                                    //
                                    //LuckyGame
                                }
                                break;
                            case _NetCmd.Server_Sate:
                                {
                                    //NetBetInfo.Instance.curstate = info.data[0];
                                }
                                break;
                            case _NetCmd.Server_Jack:
                                {
                                    IOEventCenter.SendEvent(IOCenterEvent.Event_NetworkJP, new object[3] { info.data[0], info.data[1], info.data[2] });
                                    //if (PlayerInfo.Inst.net_init)
                                    //{
                                    //    NetBetInfo.Instance.upbettime = 1;

                                    //    NetBetInfo.Instance.bettime = info.data[0];
                                    //    NetBetInfo.Instance.randjack = info.data[1];
                                    //    if (info.data[2] == 2) //开启
                                    //    {
                                    //        NetBetInfo.Instance.mpplay = 1;
                                    //        NetBetInfo.Instance.mpid = info.data[3];
                                    //    }
                                    //    else if (info.data[2] == 1) //关闭
                                    //    {
                                    //        NetBetInfo.Instance.mpplay = 1;
                                    //        NetBetInfo.Instance.mpid = 0;
                                    //    }
                                    //    if (info.data[4] > 0)
                                    //    {
                                    //        NetBetInfo.Instance.npcpos1[0] = info.data[5];
                                    //        NetBetInfo.Instance.npcpos1[1] = info.data[6];
                                    //        NetBetInfo.Instance.npcpos1[2] = info.data[7];
                                    //        NetBetInfo.Instance.npcupdate1 = 1;
                                    //    }
                                    //    if (info.data[8] > 0)
                                    //    {
                                    //        NetBetInfo.Instance.npcpos2[0] = info.data[9];
                                    //        NetBetInfo.Instance.npcpos2[1] = info.data[10];
                                    //        NetBetInfo.Instance.npcpos2[2] = info.data[11];
                                    //        NetBetInfo.Instance.npcupdate2 = 1;
                                    //    }
                                    //}
                                }
                                break;
                            case _NetCmd.Server_NetBet:
                                {
                                    //for (int i = 0; i < 5; i++)
                                    //{
                                    //    NetBetInfo.Instance.netbet[i] = info.data[i];
                                    //}
                                    //NetBetInfo.Instance.netbetflag++;
                                    //if (NetBetInfo.Instance.netbetflag > 9999)
                                    //    NetBetInfo.Instance.netbetflag = 1;
                                }
                                break;
                            case _NetCmd.Server_PlayerBet:
                                {
                                    //if (info.id == CheckHardware.Inst.macID)
                                    //{
                                        //PlayerInfo.Inst.ttbet = 0;
                                        //for (int i = 0; i < 5; i++)
                                        //{
                                        //    PlayerInfo.Inst.credit_tmp += info.data[i];// (PlayerInfo.Inst.yubet[i] - info.data[i]);
                                        //    PlayerInfo.Inst.bet[i] += (PlayerInfo.Inst.yubet[i] - info.data[i]);
                                        //    PlayerInfo.Inst.ttbet += PlayerInfo.Inst.bet[i];
                                        //    PlayerInfo.Inst.yubet[i] = 0;
                                        //    //   UiData.Inst.UI_bet(i, PlayerInfo.Inst.bet[i]);
                                        //}


                                        //PlayerInfo.Inst.state = 0;

                                        //NetBetInfo.Instance.playerbetflag++;
                                        //if (NetBetInfo.Instance.playerbetflag > 9999)
                                        //    NetBetInfo.Instance.playerbetflag = 1;

                                        object[] args = new object[info.data.Length];
                                        for (int i = 0; i < info.data.Length; i++)
                                        {
                                            args[i] = info.data[i];
                                        }
                                        IOEventCenter.SendEvent(IOCenterEvent.Event_RecvNetworkBox, args);
                                  //  }
                                }
                                break;
                            case _NetCmd.Server_CancleBet:
                                {
                                    if (info.id == CheckHardware.Inst.macID)
                                    {
                                        int esc = info.data[0];
                                    //    if (esc == 1)  //成功
                                    //    {
                                    //        for (int i = 0; i < 5; i++)
                                    //        {
                                    //            if (PlayerInfo.Inst.bet[i] > 0)
                                    //            {
                                    //                PlayerInfo.Inst.credit_tmp += PlayerInfo.Inst.bet[i];
                                    //                PlayerInfo.Inst.bet[i] = 0;
                                    //            }
                                    //        }
                                    //        NetBetInfo.Instance.playerbetflag++;
                                    //        if (NetBetInfo.Instance.playerbetflag > 9999)
                                    //            NetBetInfo.Instance.playerbetflag = 1;

                                    //        PlayerInfo.Inst.state = 0;
                                    //    }
                                    //    else
                                    //    {
                                    //        PlayerInfo.Inst.err = 0x44;
                                    //        PlayerInfo.Inst.dtime = 0;
                                    //        //GameEorroInfo.inst.SetGameEorroInfo("取消失败");
                                    //    }
                                    }
                                }
                                break;
                            case _NetCmd.Server_ClcWon:
                                {
                                    //if (PlayerInfo.Inst.net_init)
                                    //    MainGameClient.Inst.Won_ToCredit();
                                }
                                break;
                            case _NetCmd.Server_UpdateRoad:
                                {
                                    //for (int i = 0; i < 100; i++)
                                    //    NetBetInfo.Instance.result[i] = info.data[i];
                                    //for (int i = 0; i < 100; i++)
                                    //    NetBetInfo.Instance.lucky[i] = info.data[i + 100];
                                    //NetBetInfo.Instance.updateroad = 1;
                                }
                                break;
                            case _NetCmd.Server_Wait:
                                {
                                }
                                break;
                            case _NetCmd.Server_CloseBox:
                                {
                                }
                                break;
                            case _NetCmd.Server_Result:
                                {
                                }
                                break;
                            case _NetCmd.Server_HeartBeat:
                                {
                                    //
                                }
                                break;
                            case _NetCmd.Server_ResultAni:
                                {

                                }
                                break;
                            case _NetCmd.Server_PlayLucky:
                                {

                                }
                                break;
                            case _NetCmd.Server_StopLucky:
                                {
                                }
                                break;
                            case _NetCmd.Server_FindRecord:
                                {
                                }
                                break;
                            case _NetCmd.Server_GetLastBouns:
                                {

                                }
                                break;
                            case _NetCmd.Server_StopPayCoins:
                                {
                                }
                                break;
                            case _NetCmd.Client_CreditAction:
                                {

                                }
                                break;
                            case _NetCmd.Client_CoinGameCreditAction:
                                {                                 
                                }
                                break;
                        }
                    }
                }
                MemManager<MsgInfo>.Reset(MemType.Mem_MsgInfo,info);
            }
        }
    }
    private void Update()
    {
        Connect = isConnected();
        //没有启动，返回
        if (!FirstStart) return;

        if (CloseAllAniCount==0)
        { 
            CloseAllAniCount = 1;
            //UiData.Inst.HideRing(true);
        }
    }
    private void OnApplicationQuit()
    {
        IsStop = true;
        if (RcvThread != null)
        {
            if (RcvThread.IsAlive)
            {
                RcvThread.Abort();
                RcvThread = null;
            }
        }

        if (null != client)
        {
            client.Close();
            client = null;
        }
        if (IsHost)
        {
            //      if (NetworkServer.active)
            //         NetworkManager.singleton.StopHost();
        }
        else
        {
            //        if (NetworkClient.isConnected)
            //        {
            //          NetworkManager.singleton.StopClient();
            //         }
        }
    }
    void Connect_Host()
    {
        cClientTCPInit(serverinfo.IP, serverinfo.port);
    }
    void OnDestroy()
    {
        TCPGameNet.Inst.ProcessRecv -= RecvpProc;
    }
}

