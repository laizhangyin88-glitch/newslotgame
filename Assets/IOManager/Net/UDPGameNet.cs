using Newtonsoft.Json;
using SandboxApi;
using SboxSpace;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPGameNet : MonoBehaviour
{

    public int sport = 8081;
    public int rport = 8082;
    public int mathport = 8083;
    public string remotIP = "192.168.1.26";
    public string mathIP = "192.168.1.107";
    public static UDPGameNet Inst;
    private Thread RcvThread = null;
    private UdpClient client = null;
    private IPEndPoint endpoint;
    private IPEndPoint endpointA;
    ConcurrentQueue<string> clientDataQueue = new ConcurrentQueue<string>();
    bool IsStop = false;
    float timeStep = 0;
   
    private void Awake()
    {

        if (Inst == null)
            Inst = this;
    }
    private void OnApplicationQuit()
    {
        IsStop = true;
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
    // Start is called before the first frame update
    void Start()
    {
        client = new UdpClient(new IPEndPoint(IPAddress.Parse(LocalIP()), rport));
        endpoint = new IPEndPoint(IPAddress.Parse(remotIP), sport);
        endpointA = new IPEndPoint(IPAddress.Parse(mathIP), mathport);
        RcvThread = new Thread(new ThreadStart(RcvMsg))
        {
            IsBackground = true
        };
        RcvThread.Start();
    }
    public void SendMsg(string ctrlMsg)
    {
        //Debug.Log("Send Box: " + ctrlMsg);
        byte[] bf = Encoding.UTF8.GetBytes(ctrlMsg);
        client.Send(bf, bf.Length, endpoint);
    }
    public void SendMsgA(string ctrlMsg)
    {
        //Debug.Log("Send Box: " + ctrlMsg);
        byte[] bf = Encoding.UTF8.GetBytes(ctrlMsg);
        client.Send(bf, bf.Length, endpointA);
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
                //  HostIp = endpoint.Address.ToString();
                // MsgInfo info  = (MsgInfo)JsonConvert.DeserializeObject(msg, typeof(MsgInfo));

                clientDataQueue.Enqueue(msg);
                //Sandbox.write(packet);
                //if(packet.cmd != 1101)
                //Debug.Log("Recv Box: " + msg);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        //timeStep += Time.deltaTime;
        //if (timeStep > 1.0f)
        //{
        //    timeStep = 0;
        //    MsgInfo info = new MsgInfo();
        //    info.cmd = 0x303;
        //    info.data = new int[2];

        //    SendMsg(JsonConvert.SerializeObject(info));
        //}
        string msg;
        if (clientDataQueue.TryDequeue(out msg))
       {
            SandboxPacket packet = JsonUtility.FromJson<SandboxPacket>(msg);
            IOManager.Inst.RevData(packet);
        }
    }
}
