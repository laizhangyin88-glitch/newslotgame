using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandboxApi;
using System;
namespace SboxSpace
{


    //操作通讯的，挂到1物体上去
    public class IOManager : MonoBehaviour
    {
        public static IOManager Inst;



        //public enum goldFingerKey
        //{
        //    Key_Bet1=4,

        //};
        
        public SandboxIOCtrol cckeyio = new SandboxIOCtrol(0);
        public CmdBackSignl cmdinfo = new CmdBackSignl(0);
        public SandboxData boxdata = new SandboxData(0);
        public SandboxIOCtrol ioKey3588 = new SandboxIOCtrol(0);
        public DataManger master_storedata = new DataManger(0); //
        public DataManger idea_storedata = new DataManger(0);

        private SandboxIOCtrol cckeyiotmp = new SandboxIOCtrol(0);

        private int[] signlcounter = new int[12];
        public int initBoardFlag = 0;
        //小键盘
        public KeypadDeleate KeypadDeleate;
        //算法卡
        public RevDataIdeaDeleate RevDataIdeaDeleate;
        /// 打印机初始化
        public PrintIntDelegate PrintIntDelegate;
        /// <summary>
        /// 打印机内容
        /// </summary>
        public PrintContentDelegate PrintContentDelegate;
        /// <summary>
        /// 打印机切纸
        /// </summary>
        public PrintCutDelegate PrintCutDelegate;
        /// <summary>
        /// 打印机电磁铁
        /// </summary>
        public PrintElecDelegate PrintElecDelegate;
        /// <summary>
        /// 打印机字体
        /// </summary>
        public PrintFontDelegate PrintFontDelegate;
        /// <summary>
        /// 打印机马达
        /// </summary>
        public PrintMotorDelegate PrintMotorDelegate;
        /// <summary>
        /// 打印机行距
        /// </summary>
        public PrintLineDelegate PrintLineDelegate;
        /// <summary>
        /// 打印机浓度
        /// </summary>
        public PrintNDDelegate PrintNDDelegate;
        /// <summary>
        /// 打印机状态查询
        /// </summary>
        public PrintStateDelegate PrintStateDelegate;
        /// <summary>
        /// 顶球机数据接收
        /// </summary>
        public PrintStateDelegate HoldBallStateDelegate;

        public CasherStatusProcess CasherStatusProcess;
        public PrinterStatusProcess PrinterStatusProcess;
        public bool InitHoldBallCore(int port)
        {
           

            SandboxPacket packet = new SandboxPacket();
            packet.cmd = 8000;
            packet.source = 1;
            packet.target = 4;
            packet.data = new int[10];
            packet.data[0] = 0;
            packet.data[1] = 1;
            packet.data[2] = 19200;
            packet.data[3] = 0;
            packet.data[4] = 8001;
            if (CheckHardware.Inst.useRemotDebug == 1)
            {
                UDPGameNet.Inst.SendMsg(JsonUtility.ToJson(packet));
                return true;
            }
            else
            {
                if (Application.platform != RuntimePlatform.Android) return false;
                return Sandbox.write(packet);
            }
                
        }
        public bool InitPrinterPort()
        {
           

            SandboxPacket packet = new SandboxPacket();
            packet.cmd = 8000;
            packet.source = 1;
            packet.target = 4;
            packet.data = new int[18];
            packet.data[0] = 0;
            packet.data[1] = 2;
            packet.data[2] = 115200;
            packet.data[3] = 8;
            packet.data[4] = 1;
            packet.data[5] = 0;
            packet.data[6] = 0;
            packet.data[7] = 0;
            packet.data[8] = 0;
            packet.data[8] = 0;

            packet.data[9] = 8105;
            packet.data[10] = 8106;
            packet.data[11] = 8107;
            packet.data[12] = 8108;
            packet.data[13] = 8109;
            packet.data[14] = 8110;
            packet.data[15] = 8111;
            packet.data[16] = 8112;
            packet.data[17] = 8113;
            if (CheckHardware.Inst.useRemotDebug == 1)
            {
                UDPGameNet.Inst.SendMsg(JsonUtility.ToJson(packet));
                return true;
            }
            else
            {
                if (Application.platform != RuntimePlatform.Android) return false;
                return Sandbox.write(packet);
            }
        }
        private void Awake()
        {
            if (Inst == null)
            {
                Inst = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }
        private void Start()
        {
            IOEventCenter.AddListener(IOCenterEvent.EVENT_OneLightStatus, SetLightSataus);
            IOEventCenter.AddListener(IOCenterEvent.Event_StartCoinOut, StartPayout);
            IOEventCenter.AddListener(IOCenterEvent.EVENT_StopCoinOut, StopPayout);
            IOEventCenter.AddListener(IOCenterEvent.Event_StartCoinCoder, Send_StartCoinCoder);
        }
        void Send_StartCoinCoder(object[] args)
        {
            int[] buf = new int[12] { 0,0,0,0,0,0, 0, 0, 0, 0, 0, 0 };
            buf[0] = 2;//追加
            buf[1] = (int)args[0];//上分码表
            buf[2] = 2;//追加
            buf[3] = (int)args[1];//下分码表
            buf[4] = 2;//追加
            buf[5] = (int)args[2];//投币码表
            buf[6] = 2;//追加
            buf[7] = (int)args[3];//退币码表
            SendDataToBoard(1807, buf);
        }
        void SetLightSataus(object[] args)
        {
            if (CheckHardware.Inst.device == DeviceBoradMode.Device_3288)
            {
                if ((int)args[1] == 1)
                {
                    int no = (int)args[0];
                    OpenOneLight3288(no);
                }
                else
                {
                    int no = (int)args[0];
                    CloseOneLight3288(no);
                }
            }
        }
        void StartPayout(object[] args)
        {
            PayOutCoins3128((int)args[0],0);
        }
        void StopPayout(object[] args)
        {
            StopPayOut3128((int)args[0],0);
        }
        private void OnApplicationQuit()
        {
            Sandbox.exit();
        }

        public void ExitIO()
        {
            Sandbox.exit();
        }

        /// <summary>
        /// 固定时间接收数据
        /// </summary>
        private void FixedUpdate()
        {
            if (Application.platform != RuntimePlatform.Android) return;
            
           // CheckConnectErr();
            RevData();
        }

        /// <summary>
        /// 循环查询是否有连接错误
        /// </summary>
        private void CheckConnectErr()
        {
            if (Sandbox.connected(2) == false)
            {
                Master_DisConnect();
            }
            if (Sandbox.connected(4) == false)
            {
                Idea_DisConnect();
            }
        }

        /// 接收底层和算法卡传来的数据
        private void RevData()
        {
            if (Application.platform != RuntimePlatform.Android) return ;
             Sandbox.exec();
             SandboxPacket packet = Sandbox.read();
            //CheckHardware.Inst.DebugLog("收到数据");
            
            if (packet != null)
            {

                if(packet.cmd>8000)
                CheckHardware.Inst.DebugLog("Recv Box: " + JsonUtility.ToJson(packet));
                //     Delegate_Testcmd(packet.source, packet.cmd, packet.data.Length);
                //
                if (packet.target == 1)
                {
                   
                    if (packet.source == 4)
                    {
                           
                         //   Debug.Log($"SandBox: 收到数据! ");
                        Get_BoardData(packet.cmd, packet.data);
                    }
                    else if (packet.source == 2)
                    {
                        Get_IdeaData(packet.cmd, packet.data);
                    }
                }
               // else Debug.Log("tar:" + packet.target );
            }
        }
        /// 接收底层和算法卡传来的数据
        public  void RevData(SandboxPacket packet)
        {

            //if (Application.platform != RuntimePlatform.Android) return;
            //Sandbox.exec();
            //SandboxPacket packet = Sandbox.read();
            //CheckHardware.Inst.DebugLog("收到数据");

            if (packet != null)
            {

                if (packet.cmd > 8000)
                    CheckHardware.Inst.DebugLog("Recv Box: " + JsonUtility.ToJson(packet));
                //     Delegate_Testcmd(packet.source, packet.cmd, packet.data.Length);
                //
                if (packet.target == 1)
                {

                    if (packet.source == 4)
                    {

                        //   Debug.Log($"SandBox: 收到数据! ");
                        Get_BoardData(packet.cmd, packet.data);
                    }
                    else if (packet.source == 2)
                    {
                        Get_IdeaData(packet.cmd, packet.data);
                    }
                }
                // else Debug.Log("tar:" + packet.target );
            }
        }
        private bool OpenPort(ref bool sandBox, ref bool ideaBox, ref string ver, int useSandbox )
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                ideaBox = true;
                sandBox = true;
                ver = "1.0";
                return true;
            }
            Sandbox.init();
            ver = Sandbox.version();
  
           sandBox = Sandbox.connected(4);
            if (useSandbox == 1)
            {
               ideaBox = Sandbox.connected(2);
                return sandBox && ideaBox;
            }
            else
            {
                ///CheckHardware.Inst.DebugLog( " 初始化 " + sandBox);
                return sandBox;
            }
        }

        /// 向算法卡发送数据
        public bool SendDataToIdea(int cmd, int[] data)
        {

            SandboxPacket packet = new SandboxPacket
            {
                cmd = cmd,
                source = 1,
                target = 2,
                data = data
            };
            if (CheckHardware.Inst.useRemotDebug == 1)
            {
                UDPGameNet.Inst.SendMsg(JsonUtility.ToJson(packet));
                return true;
            }
            else
            {
                if (Application.platform != RuntimePlatform.Android)
                    return true;
                return Sandbox.write(packet);
            }
        }

        public bool SendDataToBoard(int cmd, int[] data)
        {

            SandboxPacket packet = new SandboxPacket
            {
                cmd = cmd,
                source = 1,
                target = 4,
                data = data
            };
            // CheckHardware.Inst.DebugLog("Send SandBox：" + JsonUtility.ToJson(packet));
            if (CheckHardware.Inst.useRemotDebug == 1)
            {
                UDPGameNet.Inst.SendMsg(JsonUtility.ToJson(packet));
                return true;
            }
            else
            {
                if (Application.platform != RuntimePlatform.Android)
                    return true;
                return Sandbox.write(packet);
            }
        }

        //************** 逻辑处理   ******************

        //检测连接状态 
        private void Master_DisConnect()
        {
            cckeyio.m_connt = false;
        }
        private void Idea_DisConnect()
        {
            cckeyio.i_connt = false;
        }

        private void Delegate_IOCtrl_init(int data)
        {
            cckeyio.io_init = data;
        }
      
        //开灯反馈
        private void Delegate_OpenLight_Success(int data)
        {
            cmdinfo.light_open_success = data;
        }
        //关灯反馈
        private void Delegate_CloseLight_Success(int data)
        {
            cmdinfo.light_close_success = data;
        }
        //存储是否成功
        private void Delegate_MasterWriteData(int data)
        {
            master_storedata._isok = (byte)data;
        }
        //退币反馈
        private void Delegate_CoinOut_Success(int data)
        {
            cmdinfo.coin_payout_success = data;
        }

        //停止退币
        private void Delegate_StopCoinOut_Success(int data)
        {
            cmdinfo.coin_stopout_success = data;
        }
        //码表反馈
        private void Delegate_Codetab(int data)
        {
            cmdinfo.codetab_open_success = data;
        }
        //停止码表反馈
        private void Delegate_StopCodetab(int data)
        {
            cmdinfo.codetab_close_success = data;
        }
        //读数据成功
        private void Delegate_MasterReadData(int[] data)
        {
            master_storedata._isok = (byte)data[0];
            if (data[0] == 0)
            {
                if (master_storedata._len > 256) master_storedata._len = 256;
                Array.Copy(data, 1, master_storedata._data, 0, master_storedata._len);
            }
        }

        //接收主板的数据包-解析
        private void Get_BoardData(int cmd, int[] data)
        {
            if (cmd == 1101) UpdateKey(data);   //按键比较频繁,放这里
            else if (cmd == 101) Delegate_MasterWriteData(data[0]);   //data[0]=存储成功=0 ，失败>0
            else if (cmd == 103) Delegate_MasterReadData(data); //data[0]=读取成功=0 ，失败>0, [1-x]=数据
            else if (cmd == 1801) {
                if (CheckHardware.Inst.device == DeviceBoradMode.Device_3128)
                    UpdateKey3128(data);
                else if (CheckHardware.Inst.device == DeviceBoradMode.Device_3588)
                    UpdateKey3588(data);
                else if (CheckHardware.Inst.device == DeviceBoradMode.Device_3288)
                    UpdateKey3288(data);
            }   //按键比较频繁,放这里
            else if( cmd > 2004) Manager_Print(cmd, data);  //打印机
            else 
            {
                switch (cmd)
                {
                    case 20:
                        Check_Saveboxdata(data[0]);
                        break;
                    case 21:
                        Get_Date(data);
                        break;
                    case 1100:
                        Delegate_IOCtrl_init(data[0]);  //初始化成功
                        break;
                    case 1102:
                        Delegate_CoinOut_Success(data[0]); //退币成功
                        break;
                    case 1103:
                        Delegate_StopCoinOut_Success(data[0]); //停止退币 
                        break;
                    case 1104:
                        Delegate_OpenLight_Success(data[0]);
                        break;
                    case 1105:
                        Delegate_CloseLight_Success(data[0]);
                        break;
                    case 1106:
                        Delegate_Codetab(data[0]);
                        break;
                    case 1107:
                        Delegate_StopCodetab(data[0]);
                        break;
                    case 1850:
                    case 1851:
                    case 1853:
                    case 1854:
                    case 1855:
                    case 1856:
                    case 1857:
                        PrinterStatusProcess?.Invoke(cmd, data);
                        break;
                    case 1830:
                    case 1831:
                    case 1833:
                    case 1834:
                        CasherStatusProcess?.Invoke(cmd, data);
                        break;
                    case 1805:
                        CheckHardware.Inst.DebugLog("开始退币");
                        break;
                    default:
                       // UpdateKey3588(data);
                        //cckeyio.cmd_err = 1;
                        break;
                }
            }
        }
        private void Manager_Print(int cmd, int[] data)
        {
           // Debug.Log($"SandBox：{string.Join(",", data)}");
            switch (cmd)
            {
                case 8105:
                    PrintIntDelegate?.Invoke( data[0]);          
                    break;
                    
                case 8106:
                    PrintNDDelegate?.Invoke( data[0]);
                    break;

                case 8107:
                    PrintFontDelegate?.Invoke(data[0]);
                    break;

                case 8108:
                    PrintLineDelegate?.Invoke( data[0]);
                    break;

                case 8109:
                    PrintCutDelegate?.Invoke(data[0]);
                    break;

                case 8110:
                    PrintContentDelegate?.Invoke(data[0]);
                    break;

                case 8111:                   
                    PrintElecDelegate?.Invoke(data[0], data[1]);
                    break;

                case 8112:                    
                    PrintMotorDelegate?.Invoke(data[0], data[1]);
                    break;

                case 8113:
                    {
                        PrintStateDelegate?.Invoke(data);            
                    }
                    break;
                case 8000:
                    {
                        initBoardFlag = 1;
                       //Debug.Log("宝箱:宝箱通讯配置成功！");
                    }
                    break;
                case 8001:
                    {
                        HoldBallStateDelegate?.Invoke(data);
                       // Debug.Log("机芯:收到机芯返回数据！");
                    }
                    break;
                default:
                    break;
            }
        }
        //算法卡的数据
        private void Get_IdeaData(int cmd, int[] data)
        {
            idea_storedata._len = data.Length;
            if (idea_storedata._len > 256) idea_storedata._len = 256;
            Array.Copy(data, 0, idea_storedata._data, 0, idea_storedata._len);
            idea_storedata._isok = 0;
            if (cmd == 0x405)
                HoldBallStateDelegate?.Invoke(data);
            else if (cmd >= 2005 && cmd <= 2013)
            {
                Manager_Print(cmd, data);
            }
            else
                RevDataIdeaDeleate?.Invoke(cmd, data);
        }
        private void UpdateKey(int[] data)
        {
            for (int i = 0; i < 8; i++)
            {
                cckeyiotmp.skey[i] = (byte)((data[0] >> i) & 1);
                if(cckeyiotmp.skey[i] ==1 )
                {
                    KeypadDeleate?.Invoke(i);
                    break;
                }
            }

            if (((data[0] >> 8) & 1) == 1) cckeyiotmp.coinouttime = 1;
            if (((data[0] >> 9) & 1) == 1) cckeyiotmp.tickouttime = 1;

            for (int i = 0; i < 8; i++)
            {
                cckeyiotmp.swtab[i] = (byte)((data[0] >> (i + 16)) & 1);
            }
            //1-19个按键
            for (int i = 5; i < 32; i++)
            {
                if (cckeyiotmp.key[i] != (byte)((data[1] >> i) & 1))
                {
                    cckeyiotmp.key[i] = (byte)((data[1] >> i) & 1);
                    if (cckeyiotmp.key[i] == 1)
                        IOEventCenter.SendEvent(IOCenterEvent.EVENT_KeyStatus, new object[2] { i,1 });
                    else
                        IOEventCenter.SendEvent(IOCenterEvent.EVENT_KeyStatus, new object[2] { i, 0 });
                }
                
            }

            //12个计数器,0退币,1退彩票,2投币,3纸币进,4纸币出
            for (int i = 0; i < 12; i++)
            {
                if (signlcounter[i] != data[2 + i])
                {
                    if (signlcounter[i] > data[2 + i])
                        cckeyiotmp.counter[i] += (256 + data[2 + i] - signlcounter[i]);
                    else
                        cckeyiotmp.counter[i] += (data[2 + i] - signlcounter[i]);
                    if( i == 0 )
                        IOEventCenter.SendEvent(IOCenterEvent.EVENT_CoinOut, new object[1] { cckeyiotmp.counter[i] });
                    else if (i == 1)
                        IOEventCenter.SendEvent(IOCenterEvent.EVENT_CoinOut, new object[1] { cckeyiotmp.counter[i] });
                    else if( i == 2 ) 
                        IOEventCenter.SendEvent(IOCenterEvent.EVENT_CoinIn, new object[1] { cckeyiotmp.counter[i] });
                    else if (i == 3)
                        IOEventCenter.SendEvent(IOCenterEvent.EVENT_CoinIn, new object[1] { cckeyiotmp.counter[i] });
                    cckeyiotmp.counter[i] = 0;
                    signlcounter[i] = data[2 + i];
                }
            }
            cckeyiotmp.update = true;
        }
        private void UpdateKey3288(int[] data)
        {
            //for (int i = 0; i < 8; i++)
            //{
            //    cckeyiotmp.skey[i] = (byte)((data[0] >> i) & 1);
            //    if (cckeyiotmp.skey[i] == 1)
            //    {
            //        KeypadDeleate?.Invoke(i);
            //        break;
            //    }
            //}
            //
            //if (((data[0] >> 8) & 1) == 1) cckeyiotmp.coinouttime = 1;
            //if (((data[0] >> 9) & 1) == 1) cckeyiotmp.tickouttime = 1;

            for (int i = 0; i < 8; i++)
            {
                cckeyiotmp.swtab[i] = (byte)((data[0] >> i) & 1);
            }
            string strinfo = "";
            int flag = 0;
            //1-19个按键
            for (int i = 0; i < 32; i++)
            {
                if (cckeyio.key[i] != ((data[1] >> i) & 1))
                {
                    cckeyio.key[i] = (byte)((data[1] >> i) & 1);
                    flag = 1;
                    strinfo += i + " ";
                    if (cckeyio.key[i] == 1)
                        IOEventCenter.SendEvent(IOCenterEvent.EVENT_KeyStatus, new object[2] { i, 1 });
                    else
                        IOEventCenter.SendEvent(IOCenterEvent.EVENT_KeyStatus, new object[2] { i, 0 });
                }


            }
            if (flag == 1)
                CheckHardware.Inst.DebugLog(" 按键： " + strinfo);
            //
            ////12个计数器
            //for (int i = 0; i < 12; i++)
            //{
            //    if (signlcounter[i] != data[2 + i])
            //    {
            //        if (signlcounter[i] > data[2 + i])
            //            cckeyiotmp.counter[i] += (256 + data[2 + i] - signlcounter[i]);
            //        else
            //            cckeyiotmp.counter[i] += (data[2 + i] - signlcounter[i]);
            //        signlcounter[i] = data[2 + i];
            //    }
            //}
            //CheckHardware.Inst.DebugLog("数值:  " + data[0] + " " + data[1] + " " + data[2] + " " + data[3] + " " + data[4] + " " + data[5] + " " + data[6] + " " + data[7] + " " + data[8] );

            //投币计数
            if (signlcounter[2] != data[7])
            {
                if (signlcounter[2] > data[7])
                    cckeyiotmp.counter[2] += (256 + data[7] - signlcounter[2]);
                else
                    cckeyiotmp.counter[2] += (data[7] - signlcounter[2]);
                signlcounter[2] = data[7];
                IOEventCenter.SendEvent(IOCenterEvent.EVENT_CoinIn, new object[1] { cckeyiotmp.counter[2] });
                cckeyiotmp.counter[2] = 0;
            }
            //退币计数
            if (signlcounter[0] != data[8])
            {
                if (signlcounter[0] > data[8])
                    cckeyiotmp.counter[0] += (256 + data[8] - signlcounter[0]);
                else
                    cckeyiotmp.counter[0] += (data[8] - signlcounter[0]);
                signlcounter[0] = data[8];
                IOEventCenter.SendEvent(IOCenterEvent.EVENT_CoinOut, new object[1] { cckeyiotmp.counter[0] });
                cckeyiotmp.counter[0] = 0;
            }
            cckeyiotmp.update = true;
            CasherStatusProcess?.Invoke(1801, data);
            PrinterStatusProcess?.Invoke(1801, data);

        }
        public bool Inter_ioKey3588()
        {
            int[] data = new int[1];
            data[0] = 0x7fff;
            cckeyio.io_init = 0x11;
            return SendDataToBoard(1800, data);
        }
        public bool OpenOneLight3588(int no)
        {
            if (no < 0 || no > 31) return false;
            int[] data = new int[2];
            data[0] = (1 << no);
            cmdinfo.light_open_success = 0x11;
            return SendDataToBoard(1802, data);
        }
        public bool CloseOneLight3588(int no)
        {
            if (no < 0 || no > 31) return false;
            int[] data = new int[2];
            data[0] = (1 << no);
            cmdinfo.light_close_success = 0x11;
            return SendDataToBoard(1803, data);
        }
        public bool OpenOneLight3288(int no)
        {
            if (no < 0 || no > 31) return false;
            int[] data = new int[2];
            data[0] = (1 << no);
            cmdinfo.light_open_success = 0x11;
            return SendDataToBoard(1802, data);
        }
        public bool CloseOneLight3288(int no)
        {
            if (no < 0 || no > 31) return false;
            int[] data = new int[2];
            data[0] = (1 << no);
            cmdinfo.light_close_success = 0x11;
            return SendDataToBoard(1803, data);
        }
        private void UpdateKey3588(int[] data)
        {
            // Debug.Log("数据长度: " + data.Length + "," + data[2]);
            for (int i = 0; i < 32; i++)
            {
                ioKey3588.iokey3588[i] = (byte)((data[0] >> i) & 1);
            }
            //for (int i = 0; i < 32; i++)
            //{
            //    if (ioKey3588.iokey3588[i] == 1)
            //    {
            //        Debug.Log("按键状态: " + (IOKey3588)i + " 按下！");
            //    }
            //}
            for (int i = 0; i < 32; i++)
            {
                ioKey3588.iokey3588[32 + i] = (byte)((data[1] >> i) & 1);
            }
            for (int i = 0; i < 32; i++)
            {
                ioKey3588.iokey35881[i] = (byte)((data[2] >> i) & 1);
            }
            //for (int i = 0; i < 32; i++)
            //{
            //    if (ioKey3588.iokey35881[i] == 1)
            //    {
            //        Debug.Log("按键状态1: " + (IOKey3588)(i ) + " 按下！");
            //    }
            //}
            //for (int i = 0; i < 32; i++)
            //{
            //    ioKey3588.iokey3588[64 + i] = (byte)((data[2] >> i) & 1);
            //}
            //for (int i = 0; i < 32; i++)
            //{
            //    if (ioKey3588.iokey3588[64 + i] == 1)
            //    {
            //        Debug.Log("按键状态2: " + (IOKey3588)(i + 64) + " 按下！");
            //    }
            //}


            CasherStatusProcess?.Invoke(1801, data);
            PrinterStatusProcess?.Invoke(1801, data);







        }
        private void UpdateKey3128(int[] data)
        {
            //for (int i = 0; i < 8; i++)
            //{
            //    cckeyiotmp.skey[i] = (byte)((data[0] >> i) & 1);
            //    if (cckeyiotmp.skey[i] == 1)
            //    {
            //        KeypadDeleate?.Invoke(i);
            //        break;
            //    }
            //}
            //
            //if (((data[0] >> 8) & 1) == 1) cckeyiotmp.coinouttime = 1;
            //if (((data[0] >> 9) & 1) == 1) cckeyiotmp.tickouttime = 1;
            
            for (int i = 0; i < 8; i++)
            {
                cckeyiotmp.swtab[i] = (byte)((data[0] >> i) & 1);
            }
            string strinfo = "";
            //1-19个按键
            for (int i = 0; i < 32; i++)
            {
                cckeyiotmp.key[i] = (byte)((data[1] >> i) & 1);
                strinfo += cckeyiotmp.key[i] + " ";
            }
           // CheckHardware.Inst.DebugLog( " 按键： " + strinfo);
            //
            ////12个计数器
            //for (int i = 0; i < 12; i++)
            //{
            //    if (signlcounter[i] != data[2 + i])
            //    {
            //        if (signlcounter[i] > data[2 + i])
            //            cckeyiotmp.counter[i] += (256 + data[2 + i] - signlcounter[i]);
            //        else
            //            cckeyiotmp.counter[i] += (data[2 + i] - signlcounter[i]);
            //        signlcounter[i] = data[2 + i];
            //    }
            //}
            //CheckHardware.Inst.DebugLog("数值:  " + data[0] + " " + data[1] + " " + data[2] + " " + data[3] + " " + data[4] + " " + data[5] + " " + data[6] + " " + data[7] + " " + data[8] );
            
            //投币计数
            if (signlcounter[2] != data[7])
            {
                if (signlcounter[2] > data[7])
                    cckeyiotmp.counter[2] += (256 + data[7] - signlcounter[2]);
                else
                    cckeyiotmp.counter[2] += (data[7] - signlcounter[2]);
                signlcounter[2] = data[7];
            }
            //退币计数
            if (signlcounter[0] != data[8])
            {
                if (signlcounter[0] > data[8])
                    cckeyiotmp.counter[0] += (256 + data[8] - signlcounter[0]);
                else
                    cckeyiotmp.counter[0] += (data[8] - signlcounter[0]);
                signlcounter[0] = data[8];
            }
            cckeyiotmp.update = true;
        }
        //**************对外接口*****

        //开始第一步
        public bool Open_Sendbox_Port(int useSandBox)
        {
            return IOManager.Inst.OpenPort(ref cckeyio.m_connt, ref cckeyio.i_connt, ref cckeyio.ver, useSandBox);
        }

        /// 初始化IO口
        public bool IntIoParameter()
        {
            for (int i = 0; i < 12; i++)
                cckeyiotmp.counter[i] = 0;
            int[] data = new int[10];
            return SendDataToBoard(1100, data);
        }

        public bool Write_masterdata_val(int addr,int buff)
        {
            if (addr < 0 || addr > 7400 || addr % 2 == 1) return false;

            int[] data = new int[3];
            data[0] = addr;
            data[1] = buff;
            data[2] = 0;
            master_storedata._isok = 0x11;
            return SendDataToBoard(100, data);
        }

        //写数据
        public bool Write_masterdata(int addr, int len, int[] buff)
        {
            if (addr < 0 || addr > 7400 || addr % 2 == 1) return false;
            if (len > 256 || len % 2 == 1) return false;

            int[] data = new int[len + 1];
            data[0] = addr;
            Array.Copy(buff, 0, data, 1, len);
            master_storedata._isok = 0x11;
            return SendDataToBoard(100, data);
        }

        //读数据
        public bool Read_masterdata(int addr, int len)
        {
            if (addr < 0 || addr > 7400 || addr % 2 == 1) return false;
            if (len > 512 || len % 2 == 1) return false;

            int[] data = new int[2];
            data[0] = addr;
            data[1] = len;

            master_storedata._isok = 0xff;
            master_storedata._len = data[1];
            master_storedata._addr = addr;
            return SendDataToBoard(102, data);
        }

        //算法卡交互内容-操作方式自行定义
        public bool Inter_ideadata(int cmd, int[] buff)
        {
            return SendDataToIdea(cmd, buff);
        }


        //开所有灯
        public bool OpenAllLight()
        {
            int[] data = new int[1];
            data[0] = 0x7fff;
            cmdinfo.light_open_success = 0x11;
            return SendDataToBoard(1104, data);
        }
        //关所有灯
        public bool CloseAllLight()
        {
            int[] data = new int[1];
            data[0] = 0x7fff;
            cmdinfo.light_close_success = 0x11;
            return SendDataToBoard(1105, data);
        }

        //开某一个灯
        public bool OpenOneLight(int no)
        {
            if (no < 0 || no > 31) return false;
            int[] data = new int[1];
            data[0] = (1 << no);
            cmdinfo.light_open_success = 0x11;
            return SendDataToBoard(1104, data);
        }
        public bool OpenOneLight3128(int no)
        {
            if (no < 0 || no > 31) return false;
            int[] data = new int[2];
            data[0] = (1 << no);
            cmdinfo.light_open_success = 0x11;
            return SendDataToBoard(1802, data);
        }
        //关闭某一个灯
        public bool CloseOneLight(int no)
        {
            if (no < 0 || no > 31) return false;
            int[] data = new int[1];
            data[0] = (1 << no);
            cmdinfo.light_close_success = 0x11;
            return SendDataToBoard(1105, data);
        }
        public bool CloseOneLight3128(int no)
        {
            if (no < 0 || no > 31) return false;
            int[] data = new int[2];
            data[0] =  (1 << no);
            cmdinfo.light_close_success = 0x11;
            return SendDataToBoard(1803, data);
        }
        public bool CloseAllLight3128()
        {
            int[] data = new int[2];
            data[0] = 0x0f00;
            cmdinfo.light_close_success = 0x11;
            return SendDataToBoard(1803, data);
        }
        //开所有灯
        public bool OpenAllLight3128()
        {
            int[] data = new int[2];
            data[0] = 0x0f00;
            cmdinfo.light_open_success = 0x11;
            return SendDataToBoard(1802, data);
        }
        //退币-退彩票
        public bool PayOutCoins(int coins, int tickets)
        {
            if (coins < 0 || tickets < 0) return false;
            int[] data = new int[2];
            data[0] = coins;
            data[1] = tickets;

            cmdinfo.coin_payout_success = 0x11;
            cmdinfo.coin_stopout_success = 0x10;

            cckeyio.coinouttime    = 0;
            cckeyio.tickouttime    = 0;
            cckeyiotmp.coinouttime = 0;
            cckeyiotmp.tickouttime = 0;

            cckeyiotmp.counter[0] = 0;
            cckeyio.counter[0] = 0;

            cckeyiotmp.counter[1] = 0;
            cckeyio.counter[1] = 0;

            return SendDataToBoard(1102, data);
        }
        public bool PayOutCoins3128(int coins, int tickets)
        {
            if (coins < 0 || tickets < 0) return false;
            int[] data = new int[2];
            data[0] = coins;
            data[1] = tickets;

            cmdinfo.coin_payout_success = 0x11;
            cmdinfo.coin_stopout_success = 0x10;

            cckeyio.coinouttime = 0;
            cckeyio.tickouttime = 0;
            cckeyiotmp.coinouttime = 0;
            cckeyiotmp.tickouttime = 0;

            cckeyiotmp.counter[0] = 0;
            cckeyio.counter[0] = 0;

            cckeyiotmp.counter[1] = 0;
            cckeyio.counter[1] = 0;

            return SendDataToBoard(1805, data);
        }
        public bool StopPayOut(int coins, int tickets)
        {
            int[] data = new int[2];
            data[0] = coins;
            data[1] = tickets;
            cmdinfo.coin_stopout_success = 0x11;

            cckeyio.coinouttime = 0;
            cckeyio.tickouttime = 0;
            cckeyiotmp.coinouttime = 0;
            cckeyiotmp.tickouttime = 0;

            return SendDataToBoard(1103, data);
        }
        public bool StopPayOut3128(int coins, int tickets)
        {
            int[] data = new int[1];
            data[0] = coins;
            cmdinfo.coin_stopout_success = 0x11;

            cckeyio.coinouttime = 0;
            cckeyio.tickouttime = 0;
            cckeyiotmp.coinouttime = 0;
            cckeyiotmp.tickouttime = 0;

            return SendDataToBoard(1806, data);
        }

        //走码表--7个
        public bool Set_Codetab(int id, int codetab)
        {
            if (id > 7 || id < 0) return false;
            int[] data = new int[9];
            data[2 + id] = codetab;
            cmdinfo.codetab_open_success = 0x11;

            return SendDataToBoard(1106, data);
        }

       
        //停止码表--7个
        public bool Stop_Codetab(int id, int stopcodetab)
        {
            if (id > 7 || id < 0) return false;
            int[] data = new int[9];
            data[2 + id] = stopcodetab;
            cmdinfo.codetab_close_success = 0x11;

            return IOManager.Inst.SendDataToBoard(1107, data);
        }
        

        //测试用的 
      //  public void Delegate_Testcmd(int sc, int data, int len)
      //  {
      //      Test_source = sc;
      //      Test_cmd = data;
      //      Test_data_len = len;
      //  }

        public void Clear_Signl(int id)
        {
            cckeyio.counter[id] = 0;
        }
        public void Reset_Signl(int id)
        {
             cckeyiotmp.counter[id] = 0;
             cckeyio.counter[id] = 0;
           
        }
        public void ResetBoard3128()
        { 
            int[] data = new int[1];
            data[0] = 0;
            SendDataToBoard(1800, data);
        }
        public void Clear_key()
        {
            for (int i = 0; i < 32; i++)
            {
                cckeyio.key[i] = 0;
            }
            for (int i = 0; i < 8; i++)
            {
                cckeyio.skey[i] = 0;
            }
        }

         public void Get_key( )
        {
            if (cckeyiotmp.update)
            {
             
                for (int i = 0; i < 8; i++)
                {
                    cckeyio.skey[i] = cckeyiotmp.skey[i];
                    cckeyiotmp.skey[i] = 0;
                    cckeyio.swtab[i] = cckeyiotmp.swtab[i];
                }
                //CheckHardware.Inst.DebugLog("拨码1:  " + cckeyio.swtab[0]);
                for(int i = 0; i < 32; i++)
                {
                    cckeyio.key[i] = cckeyiotmp.key[i];
                 //   cckeyiotmp.key[i] = 0;
                }
                for (int i = 0; i < 12; i++)
                {
                    cckeyio.counter[i] += cckeyiotmp.counter[i];
                    cckeyiotmp.counter[i] = 0 ;
                }
                if(cckeyiotmp.tickouttime == 1 ) 
                   cckeyio.tickouttime = cckeyiotmp.tickouttime;
                if(cckeyiotmp.coinouttime == 1 )
                   cckeyio.coinouttime = cckeyiotmp.coinouttime;

                cckeyiotmp.update = false;
            }
        }
        public void Ask_Date()
        {
            int[] data = new int[2] { 0, 0 };
            CheckHardware.Inst.DebugLog( "保箱: " +"请求日期" );
            SendDataToBoard(21, data);
        }
        private void Get_Date(int[] data)
        {
            if (data[0] == 0)
            {
                boxdata.year = data[1];
                boxdata.month = data[2];
                boxdata.day = data[3];
                boxdata.hour = data[5];
                boxdata.minute = data[6];
            }
        }
        public void Save_Boxdata(int[] data)
        {
            int[] buff = new int[8];
            buff[0] = data[0];
            buff[1] = data[1];
            buff[2] = data[2];
            buff[3] = 0;   //
            buff[4] = data[3]; //hour
            buff[5] = data[4]; //分
            buff[6] = 0; //0
            buff[7] = 0;
            boxdata.saveok = -1;
            SendDataToBoard(20, buff);
        }
        private void Check_Saveboxdata(int data)
        {
            boxdata.saveok = data;
        }
    }
}

