using Newtonsoft.Json;
using SboxSpace;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
public class MsgInfo
{
    public int cmd { get; set; }
    public int id { get; set; }
    public int socketid { get; set; }
    public string info { get; set; }
    public int[] data= new int[256];
}
public class ClientRecv
{
    public int id { get; set; }
    public bool use { get; set; }
    public Socket Client { get; set; }
    public ConcurrentQueue<string> clientDataQueue;
    public ConcurrentQueue<string> clientSDataQueue = new ConcurrentQueue<string>();
}

public class AESHelper
{
    /// <summary>
    /// 默认密钥-密钥的长度必须是32
    /// </summary>
    private const string PublicKey = "1234567890qwerty";//"1234567890123456";

    /// <summary>
    /// 默认向量
    /// </summary>
    private const string Iv = "abcdefghijklmnop";
    /// <summary>  
    /// AES加密  
    /// </summary>  
    /// <param name="str">需要加密字符串</param>  
    /// <returns>加密后字符串</returns>  
    public static string Encrypt(string str)
    {
        return Encrypt_gb2312(str, PublicKey);
    }

    /// <summary>  
    /// AES解密  
    /// </summary>  
    /// <param name="str">需要解密字符串</param>  
    /// <returns>解密后字符串</returns>  
    public static string Decrypt(string str)
    {
        return Decrypt_gb2312(str, PublicKey);
    }
    /// <summary>
    /// AES加密
    /// </summary>
    /// <param name="str">需要加密的字符串</param>
    /// <param name="key">32位密钥</param>
    /// <returns>加密后的字符串</returns>
    public static string Encrypt(string str, string key)
    {
        Byte[] keyArray = System.Text.Encoding.UTF8.GetBytes(key);
        Byte[] toEncryptArray = System.Text.Encoding.Unicode.GetBytes(str);
        var rijndael = new System.Security.Cryptography.RijndaelManaged();
        rijndael.Key = keyArray;
        rijndael.Mode = System.Security.Cryptography.CipherMode.ECB;
        rijndael.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
        //  rijndael.IV =  System.Text.Encoding.UTF8.GetBytes(Iv);
        System.Security.Cryptography.ICryptoTransform cTransform = rijndael.CreateEncryptor();
        Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        return System.Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }
    public static string Encrypt_gb2312(string str, string key)
    {
        Byte[] keyArray = gb2312_utf8_buff(key); //System.Text.Encoding.UTF8.GetBytes(key);
        Byte[] toEncryptArray = gb2312_utf8_buff(str);// System.Text.Encoding.Unicode.GetBytes(str);
        var rijndael = new System.Security.Cryptography.RijndaelManaged();
        rijndael.Key = keyArray;
        rijndael.Mode = System.Security.Cryptography.CipherMode.ECB;
        rijndael.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
        rijndael.IV = gb2312_utf8_buff(Iv);// System.Text.Encoding.UTF8.GetBytes(Iv);
        System.Security.Cryptography.ICryptoTransform cTransform = rijndael.CreateEncryptor();
        Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        return System.Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }
    /// <summary>
    /// AES解密
    /// </summary>
    /// <param name="str">需要解密的字符串</param>
    /// <param name="key">32位密钥</param>
    /// <returns>解密后的字符串</returns>
    public static string Decrypt_gb2312(string str, string key)
    {

        Byte[] keyArray = gb2312_utf8_buff(key); //System.Text.Encoding.UTF8.GetBytes(key);
        Byte[] toEncryptArray = Convert.FromBase64String(str);
        var rijndael = new System.Security.Cryptography.RijndaelManaged();
        rijndael.Key = keyArray;
        rijndael.Mode = System.Security.Cryptography.CipherMode.ECB;
        rijndael.Padding = System.Security.Cryptography.PaddingMode.None; //System.Security.Cryptography.PaddingMode.PKCS7;
        rijndael.IV = gb2312_utf8_buff(Iv); //System.Text.Encoding.UTF8.GetBytes(Iv);
        System.Security.Cryptography.ICryptoTransform cTransform = rijndael.CreateDecryptor();
        Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        System.Text.Encoding gb2312;
        //gb2312   
        gb2312 = System.Text.Encoding.GetEncoding("gb2312");
        string str1 = gb2312.GetString(resultArray);
        return gb2312_utf8(str1);// System.Text.Encoding.UTF8.GetString(resultArray);
    }
    public static string Decrypt(string str, string key)
    {
        Byte[] keyArray = System.Text.Encoding.UTF8.GetBytes(key);
        Byte[] toEncryptArray = Convert.FromBase64String(str);
        var rijndael = new System.Security.Cryptography.RijndaelManaged();
        rijndael.Key = keyArray;
        rijndael.Mode = System.Security.Cryptography.CipherMode.ECB;
        rijndael.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
        rijndael.IV = System.Text.Encoding.UTF8.GetBytes(Iv);
        System.Security.Cryptography.ICryptoTransform cTransform = rijndael.CreateDecryptor();
        Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        return System.Text.Encoding.UTF8.GetString(resultArray);
    }
    /// <summary>
    /// GB2312转换成UTF8
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string gb2312_utf8(string text)
    {
        //声明字符集   
        System.Text.Encoding utf8, gb2312;
        //gb2312   
        gb2312 = System.Text.Encoding.GetEncoding("gb2312");
        //utf8   
        utf8 = System.Text.Encoding.GetEncoding("utf-8");
        byte[] gb;
        gb = gb2312.GetBytes(text);
        gb = System.Text.Encoding.Convert(gb2312, utf8, gb);
        //返回转换后的字符   
        return utf8.GetString(gb);
    }
    public static byte[] gb2312_utf8_buff(string text)
    {
        //声明字符集   
        System.Text.Encoding utf8, gb2312;
        //gb2312   
        gb2312 = System.Text.Encoding.GetEncoding("gb2312");
        //utf8   
        utf8 = System.Text.Encoding.GetEncoding("utf-8");
        byte[] gb;
        gb = gb2312.GetBytes(text);
        //  gb = System.Text.Encoding.Convert(gb2312, utf8, gb);
        //返回转换后的字符   
        return gb;// utf8.GetString(gb);
    }
    /// <summary>
    /// UTF8转换成GB2312
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string utf8_gb2312(string text)
    {
        //声明字符集   
        System.Text.Encoding utf8, gb2312;
        //utf8   
        utf8 = System.Text.Encoding.GetEncoding("utf-8");
        //gb2312   
        gb2312 = System.Text.Encoding.GetEncoding("gb2312");
        byte[] utf;
        utf = utf8.GetBytes(text);
        utf = System.Text.Encoding.Convert(utf8, gb2312, utf);
        //返回转换后的字符   
        return gb2312.GetString(utf);
    }
}
public class TCPGameNet : MonoBehaviour
{
    //private  int port;
    public static TCPGameNet Inst = null;
    public class ClientConnect
    {
        public Socket Client { get; set; }
        public string cmd_str;
    }
    class ServerInfo
    {
        public string IP { get; set; }
        public int port { get; set; }
    }
    private Socket serverSocket;//服务器Socket
    //private Socket client;//客户端Socket
    private Thread myThread;//启动监听线程
    private Thread myThread1;//启动监听线程
    private Thread receiveThread;//接收数据线程
    private Thread SendThread;//接收数据线程
    private bool exitTheadFlag = false;
    public bool connect_flag = false;
    public bool host_flag = false;
    int lostConnect = 0;
    int exitClientThreadFlag = 0;

    public bool Connect = false;
    private bool FirstStart = false;
    private bool IsHost = false;
    private float dtime = 0;
    private UdpClient client = null;
    private IPEndPoint endpoint;

    private readonly int port = 7789;
    private Thread RcvThread = null;
    private string localip = "";
    private bool IsStop = false;
    private bool GetHost = false;
    private string HostIp = "";
    private int connect_step = 1;
    private int firstdis = 3;
    private float dismax = 1.0f;
    public int reConnect = 0;
    int CloseAllAniCount = 0;
    private int lostcount = 0;
    int pauseCount = 0;
    public int broadcastPort = 10000;
    //通讯包处理
    private string TemRecv = "";
    int recvHeartBeat = 0;
    private Queue<string> clientUdpDataQueue = new Queue<string>();
    DateTime beginTime = DateTime.Now;
    DateTime endTime = DateTime.Now;


    private Socket clientSocket;//服务器Socket
    private List<Socket> clientSocketList = new List<Socket>();
    public List<Thread> clientThreadList = new List<Thread>();
    private ConcurrentQueue<string> clientDataQueue = new ConcurrentQueue<string>();
    public  ConcurrentQueue<string> clientSDataQueue = new ConcurrentQueue<string>();
    public ConcurrentQueue<string> clientSDataQueueA = new ConcurrentQueue<string>();
    

    private List<ClientRecv> clientRecvList = new List<ClientRecv>();

    private object SocketListLock = new object();//创建对象锁
    private object ThreadListLock = new object();//创建对象锁
  //  int dealTim = 0;
    public delegate void ReadDataBackDelegate(int id, string cmd);
    //数据接收代理
    public ReadDataBackDelegate ProcessRecv;

    ServerInfo serverinfo = new ServerInfo();
    private bool Connectflag = false;
    public object ServJsonConvert { get; private set; }
    private string aesKey = "qwe12345qwe12345";
   
    byte[] ClientAllData = new byte[20480];
    byte[] ClientTmpData = new byte[2048];
    byte[] ClientReadBytes = new byte[2048];
    int ClientAllLenght = 0;
    private void Awake()
    {

        if (Inst == null)
            Inst = this;
    }
    void Start()
    {
        for( int i = 0; i < 100; i ++ )
           MemManager<MsgInfo>.Add(MemType.Mem_MsgInfo, new MsgInfo());


    }
    //@key:  16位字符
    //注意:   主分机key要一致否则无法解密
    public void SetEncryptKey(string key)
    {
        aesKey = key;
    }

    //@Host  初始化为主机或分机
    public void SetNetAutoConnect(bool Host)
    {
        IsHost = Host;
        FirstStart = true;
        if (IsHost)
        {
            localip = LocalIP();
            serverinfo.IP = localip;
            serverinfo.port = port;
            //NetworkManager.singleton.StartHost();
            //本机的ip地址
            client = new UdpClient(new IPEndPoint(IPAddress.Parse(localip), 0));
            endpoint = new IPEndPoint(IPAddress.Broadcast, broadcastPort);
            InitSocket(port);
        }
        else
        {
            localip = LocalIP();
            client = new UdpClient(new IPEndPoint(IPAddress.Any, broadcastPort));
            endpoint = new IPEndPoint(IPAddress.Any, 0);
            //CheckHardware.Inst.DebugLog( "开始接收广播! " + localip + " " + broadcastPort) ;
            RcvThread = new Thread(new ThreadStart(RcvMsg))
            {
                IsBackground = true
            };
            RcvThread.Start();
        }
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
                //CheckHardware.Inst.DebugLog("UDP: " + msg);
               // clientUdpDataQueue.Enqueue(msg);
                if (!GetHost)
                {
                    HostIp = endpoint.Address.ToString();
                    serverinfo = (ServerInfo)JsonConvert.DeserializeObject(msg, typeof(ServerInfo));
                  
                    GetHost = true;
                }

            }
        }
    }
    public void InitSocket(int port)
    {
        try
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iPPoint = new IPEndPoint(IPAddress.Any, port);
            serverSocket.Bind(iPPoint);
            serverSocket.Listen(10);

            //myThread = new Thread(ListenClientConnect);
           // myThread.Start();
            //myThread.IsBackground = true;
            myThread1     = new Thread(ClientConnectProc);
            myThread1.Start();
            myThread1.IsBackground = true;
            host_flag = true;
            for (int i = 0; i < 21; i++)
            {
                ClientRecv r = new ClientRecv();
                r.id = i;
                r.use = false;
                r.clientDataQueue = new ConcurrentQueue<string>();
                clientRecvList.Add(r);
            }
            serverSocket.BeginAccept(new AsyncCallback(AcceptClient), null);
            Debug.Log("Server Running...");
        }
        catch (System.Exception ex)
        {

            Debug.Log(ex.Message);
        }
    }

    void ListenClientConnect()
    {
        while (true)
        {
            if (exitTheadFlag)
                break;
            //开始接受客户端连接请求
           

            //Debug.Log("检测客户端连接中.....");

            //每隔1s检测 有没有连接我
            Thread.Sleep(5);
        }
    }
    void ClientConnectProc()
    {
        string str = "";
        lostConnect = 0;
       // Queue<string> sbuf = new Queue<string>();
        Queue<string> pbuf = new Queue<string>();
        //Queue<Socket> socketbuf = new Queue<Socket>();
        while (true)
        {
            if (exitTheadFlag)
                break;

            connect_flag = true;
            do
            {
                if (clientDataQueue.Count > 0)
                {
                    if (host_flag == false)
                        lostConnect = 0;
                    clientDataQueue.TryDequeue(out str);
                    //ProcessRecv(str);
                    ProcessRecv?.Invoke(-1,str);
                }
                else
                {

                    break;
                }
            } while (true);

            for (int i = 0; i < clientRecvList.Count; i++)
            {

                do
                {
                    if (clientRecvList[i].clientDataQueue.Count > 0)
                    {
                        clientRecvList[i].clientDataQueue.TryDequeue(out str);
                        //ProcessRecv(str);
                        ProcessRecv?.Invoke(i,str);
                        if (clientRecvList[i].clientSDataQueue.Count > 0)
                        {
                            clientRecvList[i].clientSDataQueue.TryDequeue(out str);
                            if (clientRecvList[i].Client != null)
                                SendClient(clientRecvList[i].Client, str);
                        }
                    }
                    else
                    {

                        break;
                    }
                } while (true);

            }


            do
            {
                if (clientSDataQueue.Count > 0)
                {
                    clientSDataQueue.TryDequeue(out str);
                    pbuf.Enqueue(str);
                }
                else
                    break;


            } while (true);

            do
            {
                if (pbuf.Count > 0)
                {
                    if (host_flag)
                        AllSendMs(pbuf.Dequeue());
                    else
                        SendServer(pbuf.Dequeue());
                }
                else
                {
                    pbuf.Clear();
                    break;
                }
            } while (true);

            //Debug.Log("检测客户端连接中.....");
            if (host_flag == false)
            {
                if (connect_flag)
                    lostConnect++;
            }
            if (lostConnect > 2000)
                break;


            //每隔1s检测 有没有连接我
            Thread.Sleep(1);
        }
        if (host_flag == false)
        {
            clientSocket.Close();
            lock( SocketListLock )
            {
                exitTheadFlag = true;
                connect_flag = false;
            }
        }
    }
    private void AcceptClient(IAsyncResult ar)
    {

        int i = 0;
        Socket client = serverSocket.EndAccept(ar);
        receiveThread = new Thread(ReceiveServerMsg);
        lock (SocketListLock)
        {
            clientSocketList.Add(client);
            clientThreadList.Add(receiveThread);
            do
            {
                for (i = 0; i < clientThreadList.Count; i++)
                {
                    if (clientThreadList[i].IsAlive == false)
                    {
                        break;
                    }
                }
                if (i < clientThreadList.Count)
                    clientThreadList.Remove(clientThreadList[i]);
                else
                    break;
            } while (true);

        }
        ClientRecv rec = null;
        for (i = 0; i < clientRecvList.Count; i++)
        {
            if (clientRecvList[i].use == false)
            {
                rec = clientRecvList[i];
                rec.Client = client;
                clientRecvList[i].use = true;
                break;
            }
        }
        if (i == clientRecvList.Count)
        {
            ClientRecv r = new ClientRecv();
            r.id = i;
            r.use = false;
            r.Client = client;
            r.clientDataQueue = new ConcurrentQueue<string>();
            clientRecvList.Add(r);
            rec = r;
        }


     
        receiveThread.Start(rec);
        receiveThread.IsBackground = true;
        Debug.Log("连接成功 " + clientSocketList.Count);
        serverSocket.BeginAccept(new AsyncCallback(AcceptClient), null);
        //int i = 0;
        //Socket client = serverSocket.EndAccept(ar);
        //receiveThread = new Thread(ReceiveServerMsg);
        //ClientRecv r = new ClientRecv();
        //r.id = clientRecvList.Count;
        //r.Client = client;
        //receiveThread.IsBackground = true;
        //receiveThread.Start(r);
        //lock (SocketListLock)
        //{                     

        //    clientRecvList.Add(r);
        //    clientSocketList.Add(client);
        //    clientThreadList.Add(receiveThread);
        //    do {
        //        for (i = 0; i < clientThreadList.Count; i++)
        //        {
        //            if (clientThreadList[i].IsAlive == false)
        //            {
        //                break;
        //            }
        //        }
        //        if (i < clientThreadList.Count)
        //            clientThreadList.Remove(clientThreadList[i]);
        //        else
        //            break;
        //    } while (true);
        //}

        ////client.Blocking = true;
        //Debug.Log("连接成功 " + clientThreadList.Count);
    }
    void GetStrinData(ref int allLenth, ref byte[] alldata,byte[] data )
    {
        
        do
        {
            int i = 0;
            int first = 0;
            int second = 0;
            int index = 0;
           
            for (i = 0; i < allLenth; i++)
            {
                if (index == 0)
                {
                    if (alldata[i] == '#')
                    {
                        first = i;
                        index++;
                    }
                }
                else
                {
                    if (alldata[i] == '#')
                    {
                        second = i;
                        index++;
                    }
                }
                if (index == 2)
                    break;
            }
            if(first > 0)
            {
               // string str = Encoding.UTF8.GetString(alldata, 0, allLenth);
              //  Debug.Log("数据解析错误: " + str);
            }
            if (index == 2)
            {
                for (int j = 0; j < (second - first-1); j++)
                {
                    data[j] = alldata[first + 1+ j];
                }
                //string str = );
                //if (ProcessRecv != null)
                //{
                //    ProcessRecv(str);
                //    //Debug.Log("处理数据回传: " + str);
                //}
             
                clientDataQueue.Enqueue(Encoding.UTF8.GetString(data, 0, second - first - 1));
                int last = allLenth - i;
                for (int j = 0; j < last; j++)
                {
                   alldata[j] = alldata[i + j];
                }
                allLenth = last;
                index = 0;

            }
            else
            {
                break;
            }
        } while (true);

        ///for(  )
    }
    void GetStrinDataA(ref int allLenth, ref byte[] alldata, byte[] data,int id )
    {

        do
        {
            int i = 0;
            int first = 0;
            int second = 0;
            int index = 0;

            for (i = 0; i < allLenth; i++)
            {
                if (index == 0)
                {
                    if (alldata[i] == '#')
                    {
                        first = i;
                        index++;
                    }
                }
                else
                {
                    if (alldata[i] == '#')
                    {
                        second = i;
                        index++;
                    }
                }
                if (index == 2)
                    break;
            }
            if (first > 0)
            {
                // string str = Encoding.UTF8.GetString(alldata, 0, allLenth);
                //  Debug.Log("数据解析错误: " + str);
            }
            if (index == 2)
            {
                for (int j = 0; j < (second - first - 1); j++)
                {
                    data[j] = alldata[first + 1 + j];
                }
                //string str = );
                //if (ProcessRecv != null)
                //{
                //    ProcessRecv(str);
                //    //Debug.Log("处理数据回传: " + str);
                //}
                //MsgInfo info =(MsgInfo)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data, 0, second - first - 1), typeof(MsgInfo));
                //info.socketid = id;
                clientRecvList[id].clientDataQueue.Enqueue(Encoding.UTF8.GetString(data, 0, second - first - 1));
                //clientDataQueue.Enqueue(Encoding.UTF8.GetString(data, 0, second - first - 1));
                int last = allLenth - i;
                for (int j = 0; j < last; j++)
                {
                    alldata[j] = alldata[i + j];
                }
                allLenth = last;
                index = 0;

            }
            else
            {
                break;
            }
        } while (true);

        ///for(  )
    }
    void ReceiveServerMsg(object clientSocket)
    {
        ClientRecv r = clientSocket as ClientRecv;
        Socket client = r.Client;
        int id = r.id;
        byte[] data = new byte[1024*2];
        byte[] alldata = new byte[1024 * 10];
        byte[] buf = new byte[1024*2];
        int allLenth = 0;
        SocketError errCode = 0;
        while (true)
        {
            try
            {
                if (exitTheadFlag)
                {
                    //connect_flag = false;
                    Debug.Log("退出线程 " + id);
                    break;
                }

                //connect_flag = true;
                int lenght = 0;
                lenght = client.Receive(data, 0, 2048, 0, out errCode);
                if (lenght == 0)
                {
                    //  Debug.Log("未接收到数据!");
                    if (client.Poll(100, SelectMode.SelectRead))
                    {

                        //if (errCode != 0)
                        //{
                        string s = "客户端：" + client.RemoteEndPoint + "断开了连接！";
                        Debug.Log(s);

                        client.Close();
                        lock (SocketListLock)
                        {
                            clientSocketList.Remove(client);
                        }
                        Debug.Log("退出线程 " + id);
                        break;

                        //   }
                    }

                }
                else
                {
                    int k = 0;
                    for (k = 0; k < lenght; k++)
                    {
                        alldata[k + allLenth] = data[k];
                    }
                    allLenth += k;
                    GetStrinDataA(ref allLenth, ref alldata, buf, id);
                }
                 // Thread.Sleep(1);
            }
            catch (System.Exception ex)
            {
                client.Close();
                lock (SocketListLock)
                {
                    clientSocketList.Remove(client);
                }
                Debug.Log("从服务器获取数据错误 " + ex.Message);
                Debug.Log("退出线程 " + id);
                break;
            }
        }
    }
    public string GetClientMsg()
    {
        //if (connect_flag == false)
        //    return "#_NO_CONNECTED_#";
        string recv = "";
        if (clientDataQueue.Count > 0)
        {
           
           clientDataQueue.TryDequeue(out recv);
           
        }
        return recv;
    }

    public void AllSendMs(string ms)
    {
        string cstr = "#";
        cstr += ms;
        cstr += "#";
        Socket tmp = null;
        int i = 0;
        int j = 0;
        try
        {
            for (i = 0; i < clientSocketList.Count; i++)
            {
                tmp = clientSocketList[i];

                if (tmp.Poll(5, SelectMode.SelectWrite) && tmp.Poll(5, SelectMode.SelectError) == false)
                    clientSocketList[i].Send(Encoding.UTF8.GetBytes(cstr));

            }
        }
        catch (Exception e)
        {
            Debug.Log(" 发送数据失败：  " + e.Message);
            if (tmp != null)
                clientSocketList.Remove(tmp);
        }

        //dealTim = Time.frameCount;
    }
    public void InitSocket(string server_ip, int port)
    {
        try
        {
            IPEndPoint iPPoint = new IPEndPoint(IPAddress.Parse(server_ip), port);
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.BeginConnect(iPPoint, new AsyncCallback(ConnectServer), null);
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
    private void ConnectServer(IAsyncResult ar)
    {
        try {
            clientSocket.EndConnect(ar);


            receiveThread = new Thread(ReceiveMsg);
            receiveThread.Start(clientSocket);
            receiveThread.IsBackground = true;
            //clientSocket.BeginReceive(ClientReadBytes, 0,ClientReadBytes.Length,0, ReceiveCallBack, null);

            lock (SocketListLock)
            {
                // myThread1.IsBackground = true;
                exitTheadFlag = false;
                connect_flag = true;
            }
            myThread1 = new Thread(ClientConnectProc);
            myThread1.Start();
            myThread1.IsBackground = true;

            Debug.Log("连接成功");
        }
        catch (System.Exception ex)
        {
            Debug.Log("连接失败");
            connect_flag = false;
            GetHost = false;
            Debug.Log(ex.Message);
        }
      

    }
     void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            int lenght = clientSocket.EndReceive(ar);//结束异步读取
            if (lenght > 0)
            {
                //string str = Encoding.UTF8.GetString(ReadBytes, 0, len);
                int k = 0;
                for (k = 0; k < lenght; k++)
                {
                    ClientAllData[k + ClientAllLenght] = ClientReadBytes[k];
                }
                ClientAllLenght += k;
                GetStrinData(ref ClientAllLenght, ref ClientAllData, ClientTmpData);

                clientSocket.BeginReceive(ClientReadBytes, 0, ClientReadBytes.Length, 0,new AsyncCallback( ReceiveCallBack), null);

            }
        }
        catch (Exception ex)
        {
            Debug.Log("从服务器获取数据错误" + ex.Message);
            //lock (SocketListLock)
            //{
            //    // myThread1.IsBackground = true;
            //    exitTheadFlag = true;
            //    connect_flag = false;
            //}
        }
    }
    void ReceiveMsg(object clientSocket)
    {
        Socket client = clientSocket as Socket;
        byte[] data = new byte[1024*2];
        byte[] alldata = new byte[1024 * 10];
        byte[] buf = new byte[1024*2];
        int allLenth = 0;
        SocketError errCode = 0;
        exitClientThreadFlag = 1;
        while (true)
        {
            try
            {
                if (exitTheadFlag)
                {
                    exitClientThreadFlag = 0;
                    Debug.Log("退出线程 ");
                    break;
                }
                
                int lenght = client.Receive(data, 0, 2048, 0, out errCode);
                if (lenght == 0)
                {
                    //  int lenght = client.Receive(data, 0, 2048, 0, out errCode);
                    if (client.Poll(5, SelectMode.SelectRead))
                    {
                        //if (errCode!= 0)
                        //{
                        string s = " 服务端：" + client.RemoteEndPoint + "断开了连接！";
                        Debug.Log(s);
                        Debug.Log("退出线程 ");
                        client.Close();
                        lock (SocketListLock)
                        {
                            // myThread1.IsBackground = true;
                            exitTheadFlag = true;
                            connect_flag = false;
                        }
                        exitClientThreadFlag = 0;
                        break;

                        //}

                    }
                }
                else
                {
                    int k = 0;
                    for (k = 0; k < lenght; k++)
                    {
                        alldata[k + allLenth] = data[k];
                    }
                    allLenth += k;
                    GetStrinData(ref allLenth, ref alldata, buf);
                }
                //string str = "tcp comunication test !!";//Encoding.ASCII.GetString(data, 0, data.Length);
                //Thread.Sleep(1);
                // AllSendMs(str);
                //Thread.Sleep(5);
            }
            catch (System.Exception ex)
            {
                //if (client.Connected == false)
                //{
                //  client.Close();
                //  exitTheadFlag = true;
                //}
                Debug.Log("从服务器获取数据错误" + ex.Message);
                Debug.Log("退出线程 ");
                client.Close();
                lock (SocketListLock)
                {
                    exitTheadFlag = true;
                    connect_flag = false;
                }
                exitClientThreadFlag = 0;
                break;
            }
        }
    }
    public string GetMsg()
    {

        string recv = "";
       // if (connect_flag == false)
       //     return "#_NO_CONNECTED_#";
        if (clientDataQueue.Count > 0)
        {
             clientDataQueue.TryDequeue(out recv);
        }

        return recv;
    }

    public void SendServer(string cmd)
    {

        string cstr = "#";
        cstr += cmd;
        cstr += "#";
        //if (connect_flag == false)
        //    return;
        try
        {
          //if (clientSocket.Poll(5, SelectMode.SelectWrite)&& clientSocket.Poll(5, SelectMode.SelectError) == false )
           clientSocket.Send(Encoding.UTF8.GetBytes(cstr));
        }
        catch (Exception e)
        {
            clientSocket.Close();
            Debug.Log("发送失败1 " + e.Message);
        }

    }
    public void SendClientSingle(int id, string cmd)
    {
        //SendClient(clientRecvList[id].Client, cmd);
        clientRecvList[id].clientSDataQueue.Enqueue(cmd);
    }
    public void SendClient(Socket send, string cmd)
    {

        string cstr = "#";
        cstr += cmd;
        cstr += "#";
        //if (connect_flag == false)
        //    return;
        try
        {
            if (send.Poll(5, SelectMode.SelectWrite) && send.Poll(5, SelectMode.SelectError) == false)
                send.Send(Encoding.UTF8.GetBytes(cstr));
        }
        catch (Exception e)
        {
            send.Close();
            Debug.Log("发送失败2 " + e.Message);
        }

    }
    void Connect_Host()
    {
        //   if(exitTheadFlag == true)
        if (myThread1 != null)
        {
            myThread1.Abort();
            
        }
        InitSocket(serverinfo.IP, serverinfo.port);
    }

    //分机向主机发送数据
    public void cClientTCPSend(string cmd_str)
    {
        clientSDataQueue.Enqueue(cmd_str);
    }
    //主机向所有分机广播
    public void cServerTCPSend(string cmd_str)
    {
        clientSDataQueue.Enqueue(cmd_str);
    }
    //主机向当前分机发送数据
    public void cServerTCPSendA(string cmd_str)
    {
        clientSDataQueueA.Enqueue(cmd_str);
    }
    private void Update()
    {
        //没有启动，返回
        if (!FirstStart) return;

        if (CloseAllAniCount == 0)
        {
            CloseAllAniCount = 1;
        }

        beginTime = DateTime.Now;
        if (beginTime.Ticks > endTime.Ticks)
        {

            long tmpp = (beginTime.Ticks - endTime.Ticks) / 10000;
            endTime = DateTime.Now;
            if (tmpp >= 1000)
            {
                pauseCount++;
            }
        }
        //主机
        if (IsHost)
        {
            Connect = true;
            dtime += Time.deltaTime;
            if (dtime > dismax)
            {
                // UiData.Inst.UI_state(str1);
                //Debug.Log("当前运行的线程数: " + tcpTool.clientThreadList.Count);
                dtime = 0;
                recvHeartBeat++;
                string strSerializeJSON = JsonConvert.SerializeObject(serverinfo);

                SendMsg(strSerializeJSON);


                if (firstdis > 0) firstdis--;
                else dismax = 1.0f;
            }

        }
        else //分机
        {
            if (Connect != connect_flag)
            {
                if (connect_flag)
                    IOEventCenter.SendEvent(IOCenterEvent.Event_NetworkErr, new object[1] { 0 });
                else
                    IOEventCenter.SendEvent(IOCenterEvent.Event_NetworkErr, new object[1] { 1 });
                Connect = connect_flag;
            }
            if (Connect)
            {
                reConnect = 0;
                connect_step = 1;
                dtime += Time.deltaTime;
                if (dtime >= 0.5)
                {
                    // UiData.Inst.UI_state(str1);
                    //Debug.Log("当前运行的线程数: " + tcpTool.clientThreadList.Count);
                    dtime = 0;
                    MsgInfo cmd = MemManager<MsgInfo>.tryGet(MemType.Mem_MsgInfo);// new MsgInfo();
                    if (cmd == default(MsgInfo))
                    {
                        CheckHardware.Inst.DebugLog("没有足够的: "+ MemType.Mem_MsgInfo);
                    }
                    else
                    {
                        cmd.cmd = 204;
                        cmd.id = -1;
                       // cmd.data = new int[2];
                        cmd.data[0] = 1;
                        string s = JsonConvert.SerializeObject(cmd);
                        cClientTCPSend(s);
                        MemManager<MsgInfo>.Reset(MemType.Mem_MsgInfo, cmd);
                    }
                    


                }
            }
            else
            {
                if (connect_step == 1&& exitClientThreadFlag == 0)
                {
                    dtime += Time.deltaTime;
                    if (dtime > 0.5)
                    {
                        dtime = 0;
                        if (GetHost)
                        {
                            Connect_Host();
                            connect_step++;
                        }
                    }
                    
                }
                else if (connect_step == 2)
                {
                    dtime += Time.deltaTime;
                    if (dtime > 3.0f)
                    {
                        dtime = 0;
                        reConnect++;
                        connect_step = 1;
                        GetHost = false;
                        if (reConnect > 3)
                        {
                            reConnect = 0;
                            
                        }
                    }
                }
            }

        }
    }
    public void CloseClient()
    {
    }
    void OnDestroy()
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

        try
        {
            exitTheadFlag = true;

            Thread.Sleep(200);
            //关闭线程
            if (myThread != null || receiveThread != null)
            {
                if (myThread.IsAlive == false)
                    myThread.Abort();
                //if (myThread1.IsAlive == false)
                //    myThread1.Abort();
                // receiveThread.Interrupt();
                if (receiveThread.IsAlive == false)
                    receiveThread.Abort();

            }
            for (int i = 0; i < clientThreadList.Count; i++)
            {
                clientThreadList[i].Abort();
            }
            //最后关闭socket
            if (serverSocket != null)
            {
                for (int i = 0; i < clientSocketList.Count; i++)
                {
                    clientSocketList[i].Close();
                }

                //serverSocket.Shutdown(SocketShutdown.Both);
                serverSocket.Close();

            }
            clientRecvList.Clear();
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
        print("disconnect");
    }

}