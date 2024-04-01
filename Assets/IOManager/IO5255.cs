using System;
using System.Threading;
using System.Runtime.InteropServices;

using UnityEngine;
using System.Collections.Concurrent;

namespace SboxSpace
{
    public class SandBoxCmd
    {
        public int cmd { get; set; }
        public int[] data;
    }
    public class IO5255 : MonoBehaviour
    {
        public static IO5255 Inst;

        public DataSet dataSet = new DataSet();

        private bool  Use5255 = false;
    
        private  int  sendcmd = 0  ;  //有发送

        public  int   checkok = 0  ;
        int readySend = 0;
        private float dtime  = 0   ;  
        private float timeout = 2.0f;
        //重复发
        private int[]   replay_data = new int[256];
        public int[] recpBuff = new int[256];
        private int     replay_cmd = 0 ;     //指令
        private int     replay_len = 0 ;     //长度
        private int     replay = 1 ; //重复几次
        private ConcurrentQueue<SandBoxCmd> clientDataQueue = new ConcurrentQueue<SandBoxCmd>();
        public delegate void ReadCellDataDelegate(int[] data);
        public ReadCellDataDelegate ReadCellData;
        private void Awake()
        {
            if(Inst == null )
            {
                Inst = this;
                DontDestroyOnLoad(this);
            }
            else 
            {
                Destroy(this);
            }
        }

        private void OnApplicationQuit()
        {
            if(Use5255)
            {
                IOManager.Inst.RevDataIdeaDeleate -= Loop_232_Data;
            }
        }

        public void Reset_5255()
        {
            if (IOManager.Inst != null )
            {
                IOManager.Inst.RevDataIdeaDeleate += Loop_232_Data;
                Use5255 = true;
                IOEventCenter.AddListener(IOCenterEvent.Event_ClientSendBaox, Send_getSlot5x3Info);
                IOEventCenter.AddListener(IOCenterEvent.Event_RecvNetworkBox, Get_Slot5x3Info);
               
                Debug.Log("IO5250 初始化");
            }
        }
        //void UpdateJPNumber(object[] args)
        //{
        //    IO5255.Inst.dataSet.major_curr = (int)args[0];
        //    IO5255.Inst.dataSet.minor_curr = (int)args[1];
        //    IO5255.Inst.dataSet.mini_curr = (int)args[2];
        //}
        public void IOLoop()
        {
            if (Use5255)
            {

                if (clientDataQueue.Count > 0)
                {

                    if (readySend == 0)
                    {
                        SandBoxCmd cmdinfo;
                        if (clientDataQueue.TryDequeue(out cmdinfo))
                        {
                            readySend = 1;
                            for (int i = 0; i < cmdinfo.data.Length; i++)
                                replay_data[i] = cmdinfo.data[i];
                            replay_len = cmdinfo.data.Length;
                            replay_cmd = cmdinfo.cmd;
                            replay = 1;
                            dtime = 0;
                            Reset_config(cmdinfo.cmd);
                            if (cmdinfo.cmd == 0x305)
                            {
                                if (CheckHardware.Inst.useGroup == 1&& CheckHardware.Inst.isHost == 0)
                                {
                                    //object[] args = new object[cmdinfo.data.Length];
                                    //for (int i = 0; i < cmdinfo.data.Length; i++)
                                    //    args[i] = cmdinfo.data[i];
                                    //IOEventCenter.SendEvent(IOCenterEvent.Event_SendNetworkBox, args);
                                }
                                else
                                    IOManager.Inst.Inter_ideadata(cmdinfo.cmd, cmdinfo.data);
                            }
                            else
                                IOManager.Inst.Inter_ideadata(cmdinfo.cmd, cmdinfo.data);
                        }
                    }
                }
                if (checkok == 1)  //发送
                {
                    dtime += Time.deltaTime;
                    if (dtime > timeout)
                    {
                        dtime = 0;
                        checkok = 4;
                    }
                }
                else
                {
                    if (checkok == 3)
                    {
                        Send_Replay();
                    }
                }
            }
        }
        private void Send_Replay()
        {
            replay -- ;
            int[] buf = new int[replay_len];
            for(int i = 0 ; i < replay_len ; i++)
               buf[i] = replay_data[i]; 
             Reset_config(replay_cmd);   
            //获取当前时钟    
            IOManager.Inst.Inter_ideadata(replay_cmd, buf);  
        }

        //private void Copy_send(int cmd , int[] data, int len)
        //{
        //    for(int i = 0 ; i < data.Length; i++)
        //       replay_data[i] = data[i];
        //    replay_len = len ;
        //    replay_cmd = cmd ;
        //    replay = 1 ;
        //    Reset_config(cmd);
        //    IOManager.Inst.Inter_ideadata(cmd, data);  
        //}
        private void Copy_send(int cmd, int[] data, int len)
        {
            SandBoxCmd cmdinfo = new SandBoxCmd();
            cmdinfo.cmd = cmd;
            cmdinfo.data = new int[data.Length];
            data.CopyTo(cmdinfo.data, 0);
            clientDataQueue.Enqueue(cmdinfo);

            //IOManager.Inst.Inter_ideadata(cmd, data);  
        }
        //发送初始化
        public void Send_Init( )
        {
            int[] buf = new int[1];
            buf[0] = UnityEngine.Random.Range(0,10000000);
            Copy_send(0x100,buf,1); 
        }

        //获得开机信息
        private void Get_Init( int[] data)
        {
            dataSet.ver = data[0] ;
            dataSet.token = data[1];
            dataSet.lineid = data[2];
            dataSet.macid = data[3];             
        }

         //获得参数
        public void Send_Getoption( )
        {
            int[] buf = new int[1];
            Copy_send(0x102,buf,1);
        }

        private void Get_Option(int[] data)
        {

            dataSet.OneCoinVal = data[0];
            dataSet.BetTime = data[1];
            dataSet.MinSBet = data[2];
            dataSet.MaxSBet = data[3];
            dataSet.MaxTBet = data[4];
            dataSet.MaxTWin = data[5];
            dataSet.COINX   = data[6];
            dataSet.LOTX    = data[7];

            dataSet.LOTX1   = data[8];
            
            dataSet.Cm[0]  = data[9];
            dataSet.Cm[1] = data[10];
            dataSet.Cm[2] = data[11];

            dataSet.MaxPlayer = data[12];
            dataSet.g_display_mode = data[13];    
            dataSet.b_PeriodMaxLose = data[14];

            //dataSet.mega_base = data[15];
            //dataSet.major_base = data[16];
            //dataSet.minor_base = data[17];
            //dataSet.mini_base = data[18];

            //dataSet.mega_add = data[19];
            //dataSet.major_add = data[20];
            //dataSet.minor_add = data[21];
            //dataSet.mini_add = data[22];


            //dataSet.mega_curr = data[23];
            //dataSet.major_curr = data[24];
            //dataSet.minor_curr = data[25];
            //dataSet.mini_curr = data[26];

            //dataSet.rate_index = data[27];


            //string str_info = "";
            //for (int i = 0; i < 12; i++)
            //{
            //    str_info += data[15 + i];
            //    str_info += ",";
            //}
         //   CheckHardware.Inst.DebugLog("彩金设置: " + str_info );

        }

        //是否需要算码
        public void Send_needsm()
        {
            int[] buf = new int[1];
            dataSet.checkinfo_ok = 0;
            buf[0] = 0;
            Copy_send(0x104,buf,1);
        }

        private void Get_needsm(int[] data)
        {
        
             dataSet.checkinfo_ok = data[0];
             
             dataSet.zs = data[1];
             dataSet.zt = data[2];
             dataSet.cur_playin = data[3];
             dataSet.cur_playout = data[4];
             dataSet.mingpaiid = data[5];
             dataSet.bonusid = data[6];
          //  CheckHardware.Inst.DebugLog( " 算码信息: " + data[0] + " " + data[1] + " " + data[2] + " " + data[3] + " " + data[4] + " " + data[5] + " " + data[6]);
        }

        //获取算码信息
        public void Send_Getsminfo(int type)
        {
            int[] buf = new int[1];
            buf[0] = type;
            Copy_send(0x106,buf,1);
            dataSet.checkinfo_ok = 0;
        }

        private void Get_sminfo( int[] data)
        {
     
            dataSet.checkinfo_ok = data[0];
            dataSet.macid = data[1];
            dataSet.zs = data[2] ;
            dataSet.zt = data[3] ;
            dataSet.smcount = data[4] ;
            dataSet.checkcode = data[5];
            dataSet.realcode = data[6];
        }

        //验证算码信息
        public void Send_checksm ( int[] data)
        {
            Copy_send(0x108, data, 8);
            dataSet.checkinfo_ok = 0 ;
        }
        void Get_checksm ( int []data)
        {
            dataSet.checkinfo_ok = data[0];
           // CheckHardware.Inst.DebugLog( "算码回传：" + data[0] + "," + data[1] + "," + data[2] + "," + data[3] + "," + data[4] + "," + data[5] + "," + data[6] + "," + data[7] + "," + data[8] + "," ) ;
        }
        
        //******************cmd == 110--118 ******/
        public void Clear_cmd ( )
        {
            int[] buf = new int[1];
            buf[0] = 0;
            Copy_send(0x404, buf, 1);
            dataSet.checkinfo_ok = 0;
        }

        //系统密码
        public void Send_syspwd(int type, int data )
        {
            int[] buf = new int[2];
            buf[0] = type ;
            buf[1] = data;
            Copy_send(0x110,buf,2);
            dataSet.checkinfo_ok = 0 ;
        }
        
        private void Get_syscheck(int []data)
        {
            dataSet.checkinfo_ok = data[0];
        }
        public void Send_modiypwd(int type, int data)
        {
            int[] buf = new int[2];
            buf[0] = type ;
            buf[1] = data;
            Copy_send(0x114, buf, 2);
            dataSet.checkinfo_ok = 0;
        }
        private void Get_modiypwd(int[] data)
        {
            dataSet.checkinfo_ok = data[0];
        }

        //设置机台号-只有一次机会
        public void Send_sysmacid (int syspwd, int macid )
        {
            int[] buf = new int[2];
            buf[0] = syspwd;
            buf[1] = macid;
            Copy_send(0x112,buf,2);
            dataSet.checkinfo_ok = 0 ;
        }

        void Get_check_macid ( int []data)
        {
            dataSet.checkinfo_ok = data[0];
        }

        //************0x200--0x220************//
        public void Send_Isnew( int err ) //新一局
        {
            int[] buf = new int[1];
            buf[0] = err;
            Copy_send(0x200,buf,1);
            dataSet.checkinfo_ok = 0 ;
        }
        
        void Get_Isnew(int[] data)
        {
             dataSet.checkinfo_ok = data[0];
        }

        //获得新一局牌路
        public void Send_getpkroad()
        {
            int[] buf = new int[1];
            buf[0] = 0;
            Copy_send(0x202,buf,1);
            timeout = 5.0f;
            dataSet.checkinfo_ok = 0;
            //Debug.Log("获取路单: 0x202");
        }
        public void Send_addLastBouns(int val )
        {
            int[] buf = new int[1];
            buf[0] = val;
            Copy_send(0x212, buf, 1);
            timeout = 5.0f;
            dataSet.checkinfo_ok = 0;
            //Debug.Log("获取路单: 0x202");
        }
        private void Get_newpkroad(int []data)
        {
            string str = "";
            string str1 = "";
            int[] perCount = { 0,0,0,0,0};
            if (data[0] == 10)
            {
                dataSet.checkinfo_ok = 10;
                byte[] pkroad = new byte[100];
                byte[] buff = new byte[4];

                dataSet.ju = data[1];
                dataSet.gamecount = 1;
                for (int i = 0; i < 25; i++)
                {
                    buff = BitConverter.GetBytes(data[i + 2]);
                    dataSet.printresult[4 * i] = buff[0];
                   
                    dataSet.printresult[4 * i + 1] = buff[1];

                    dataSet.printresult[4 * i + 2] = buff[2];
                    dataSet.printresult[4 * i + 3] = buff[3];

                    str += Convert.ToString(buff[0], 16);
                    str += ",";
                    str += Convert.ToString(buff[1], 16);
                    str += ",";
                    str += Convert.ToString(buff[2], 16);
                    str += ",";
                    str += Convert.ToString(buff[3], 16);
                    str += ",";

                    str1 += Convert.ToString(data[i + 2], 16) + " ";

                    if ((buff[0] >> 4) == 0)
                    {
                        perCount[0]++;
                    }
                    else if ((buff[0] >> 4) == 1)
                    {
                        perCount[1]++;
                    }
                    else if ((buff[0] >> 4) == 2)
                    {
                        perCount[2]++;
                    }
                    else if ((buff[0] >> 4) == 3)
                    {
                        perCount[3]++;
                    }
                    /////////
                    if ((buff[1] >> 4) == 0)
                    {
                        perCount[0]++;
                    }
                    else if ((buff[1] >> 4) == 1)
                    {
                        perCount[1]++;
                    }
                    else if ((buff[1] >> 4) == 2)
                    {
                        perCount[2]++;
                    }
                    else if ((buff[1] >> 4) == 3)
                    {
                        perCount[3]++;
                    }

                    /////////
                    if ((buff[2] >> 4) == 0)
                    {
                        perCount[0]++;
                    }
                    else if ((buff[2] >> 4) == 1)
                    {
                        perCount[1]++;
                    }
                    else if ((buff[2] >> 4) == 2)
                    {
                        perCount[2]++;
                    }
                    else if ((buff[2] >> 4) == 3)
                    {
                        perCount[3]++;
                    }
                    /////////
                    if ((buff[3] >> 4) == 0)
                    {
                        perCount[0]++;
                    }
                    else if ((buff[3] >> 4) == 1)
                    {
                        perCount[1]++;
                    }
                    else if ((buff[3] >> 4) == 2)
                    {
                        perCount[2]++;
                    }
                    else if ((buff[3] >> 4) == 3)
                    {
                        perCount[3]++;
                    }
                }
        
            }
           
            //Debug.Log("统计: " + perCount[0] + " " + perCount[1] + " " + perCount[2] + " " + perCount[3] + " ");
        }


        //获得本场信息
        public void Send_getcurinfo()
        {
            int[] buf = new int[1];

            buf[0] = 0;
            Copy_send(0x204,buf,1);
        }
        //开始选球
        public void Send_HoldBallSelect()
        {
            int[] buf = new int[1];

            buf[0] = 0;
            Copy_send(0x211, buf, 1);
        }
        //获得明牌等
        private void Get_mptype(int []data)
        {
            dataSet.ju = data[0]       ;
            dataSet.gamecount = data[1];
            dataSet.mp_type = data[2]  ;
            dataSet.first_pk = data[3] ;
            dataSet.two_pk = data[4]   ;
            dataSet.randjack = data[5] ;
        }

        //获得本次结果:指令，玩家押分
        public void Send_getbcresult(int[] buf)
        {
            Copy_send(0x206, buf, buf.Length);

        }
        //保存当前彩金数
        public void Send_saveBounsCurr(int[] buf)
        {
            Copy_send(0x304, buf, 4);
        }
        //获得结果  -获得特殊奖励：彩金-翻倍等. 参数：指令-在线标志

        private void Get_bcresult( int[] data)
        {
            dataSet.gamecount = data[0];
            dataSet.first_pk = data[1]  ;
            dataSet.two_pk = data[2] ;
            dataSet.lucky_type = data[3]   ;//彩金类型
            dataSet.jack_type = data[4];//废弃
            for (int i = 0; i < dataSet.MaxPlayer; i++)
                dataSet.playerwon[i] = data[5 + i];//玩家普通彩金赢分
            for (int i = 0; i < dataSet.MaxPlayer; i++)
                dataSet.playerjactype[i] = data[16 + i];//幸运玩家，获得的彩金类型
            for (int i = 0; i < dataSet.MaxPlayer; i++)
                dataSet.playeraddrate[i] = data[27 + i];//加倍数值
            for (int i = 0; i < dataSet.MaxPlayer; i++)
                dataSet.playerbounsmode[i] = data[38 + i];//彩金模式，直接派送，或者抢金币模式
            for (int i = 0; i < dataSet.MaxPlayer; i++)
                dataSet.playerresult1[i] = data[49 + i];//二连中结果
            for (int i = 0; i < dataSet.MaxPlayer; i++)
                dataSet.playerresult2[i] = data[60 + i];//三连中结果
           //CheckHardware.Inst.DebugLog("数据: " + dataSet.lucky_type + ","+ dataSet.playerjactype[1] + "," + dataSet.playeraddrate[1] + "," + dataSet.playerbounsmode[1] + "," + dataSet.playerresult1[1]);
        }
      
        //牌路信息
        public void Send_pkroadinfo( )
        {
            int[] buf = new int[1];
            buf[0] = 0 ;
            Copy_send(0x210,buf,1);
            dataSet.checkinfo_ok = 0;
        }
        //获得牌路
        private void Get_pkroadinfo(int[] data)
        {
            byte[] buff= new byte[4];
            dataSet.checkinfo_ok = 10;
            dataSet.ju = data[0];
            dataSet.gamecount = data[1];
            for(int i=0;i<25;i++)
            {
                buff =  BitConverter.GetBytes(data[i+2]);
                dataSet.result[4*i ] = buff[0];
                dataSet.result[4*i+1] = buff[1];
                dataSet.result[4*i+2] = buff[2];
                dataSet.result[4*i+3] = buff[3];
            } 
            for(int i=0;i<25;i++)
            {
                buff =  BitConverter.GetBytes(data[i+27]);
                dataSet.lucky[4*i ] = buff[0];
                dataSet.lucky[4*i+1] = buff[1];
                dataSet.lucky[4*i+2] = buff[2];
                dataSet.lucky[4*i+3] = buff[3];
            }
        }
        //当前主机账目
        public void Send_hostrecord()
        {
            int[] buf = new int[1];
            buf[0] = 0;
            Copy_send(0x116, buf, 1);
        }

        private void Get_HostRecord(int[] data )
        {
            dataSet.checkinfo_ok = data[0];
            dataSet.cur_playin = data[1];
            dataSet.cur_playout = data[2];
            dataSet.zs = data[3];
            dataSet.zt = data[4];
        }

        //清空主机账目
        public void Send_clearrecord()
        {
            int[] buf = new int[1];
            buf[0] = 0;
            Copy_send(0x118, buf, 1);
            dataSet.checkinfo_ok = 0;
        }
        private void Get_clearRecord(int [] data)
        {
            dataSet.checkinfo_ok = data[0];
            dataSet.cur_playin = data[1];
            dataSet.cur_playout = data[2];
        }


        //读取数据-        
        private void Loop_232_Data( int cmd,int[] data)
        {
            readySend = 0;
            if (cmd == sendcmd)
            {
                checkok = 2;
            }
            else
            {
                if (replay > 0) checkok = 3;  //重发
                else
                {
                    checkok = 4;
                }
            }
            dtime = 0;
            if (cmd < 0x110)
            {
                switch(cmd)
                {
                case 0x100:  //初始化
                    Get_Init(data);
                    break ;
                case 0x102:  //获取参数
                    Get_Option(data);
                    break ; 
                case 0x104:
                    Get_needsm(data);
                    break ; 
                case 0x106:
                    Get_sminfo(data);
                    break ;
                case 0x108:
                    Get_checksm(data);
                    break;
                default:
                    break ;
                }
            }
            else if( cmd < 0x200)
            {
               switch(cmd)
               {
                case 0x110:
                     Get_syscheck(data);
                     break;
                case 0x112:
                     Get_check_macid(data);
                     break ;
               case 0x114:
                     Get_modiypwd(data);
                     break;
               case 0x116:
                    Get_HostRecord(data);
                    break;
               case 0x118:
                     Get_clearRecord(data);
                    break;
               case 0x120:
                    Get_Clearbl(data);
                    break;

               default:
                    break;
               } 
            }
            else if(cmd < 0x300)
            {
                switch(cmd)
                {
                case 0x200: Get_Isnew(data);    //是否是新一局
                            break ;
                case 0x202: Get_newpkroad(data);  //获得新一局牌路
                            break ;
                case 0x204: Get_mptype(data);
                            break ;
                case 0x206: Get_bcresult(data);
                            break ;
                case 0x210: Get_pkroadinfo(data);
                            break ;
                case 0x211:
                       // CheckHardware.Inst.DebugLog("选球回传!");
                        break;
                    case 0x212:
                        //CheckHardware.Inst.DebugLog("当前账目: " + data[0] + "," + data[1]);
                        break;
                    default:    break ;
                }
            }
            else
            {
                switch(cmd)
                {
                case 0x300:
                        Get_SaveOption(data);
                        break;
                case 0x302:
                        Get_SaveSuper(data);
                        break;
                    case 0x303:
                        Get_BounsNumberInfo(data);
                        break;
                    case 0x304:
                        Get_BallNumberInfo(data);
                        break;
                    case 0x305:
                        {
                            dataSet.get5x3buf_ok = 1;
                           
                            if (CheckHardware.Inst.macID == 0)
                            {
                                data[15] = IO5255.Inst.dataSet.major_curr;
                                data[16] = IO5255.Inst.dataSet.minor_curr;
                                data[17] = IO5255.Inst.dataSet.mini_curr;
                                object[] args = new object[data.Length];
                                for (int i = 0; i < data.Length; i++)
                                    args[i] = data[i];
                                IOEventCenter.SendEvent(IOCenterEvent.Event_ServerSendNetwork, args);
                                CheckHardware.Inst.DebugLog($"路单：{string.Join(",", data)}");
                            }
                            else
                                Get_Slot5x3Info(data);
                            // 
                            checkok = 0;
                        }
                        // Get_BounsNumberInfo(data);
                        break;
                    case 0x316:
                        {
                            //int ttbet = 0;
                            //int ttwin = 0;
                            //for (int i = 0; i < 11; i++)
                            //{
                            //    ttbet += HostInfo.Instance.playrecord[i, 0];
                            //    ttwin += HostInfo.Instance.playrecord[i, 1];
                            //}
                            //checkok = 0;
                            //if (ttwin !=data[1])
                            //{
                            CheckHardware.Inst.DebugLog(" 存储回传: " + data[0] + " " + data[1]);
                            // }
                        }
                        break;
                    case 0x317:
                        Get_readPlayerInfo(data);
                        break;
                    case 0x318:
                        Get_updatesmInfo(data);
                        break;

                    case 0x319:
                        {
                           // ReadCellData?.Invoke(data);
                            checkok = 0;
                        }
                        break;
                    case 0x320:
                        {
                            Get_DataCell(data);
                            checkok = 0;
                        }
                        break;
                    case 0x321:
                        {                         
                            checkok = 0;
                        }
                        break;

                    default:break;
                }
            }

             // Debug.Log("发送:" + sendcmd + "--回传:" + cmd );


               
        }

        private void Reset_config(int cmd )
        {
           timeout = 2.0f;
           sendcmd = cmd ;
           checkok =  1 ;
           dtime   = 0  ;
        }

        //保存参数
        public void Send_Saveoption(int[] data)
        {
            int[] buf = new int[4];
            buf[0] = data[0];
            buf[1] = data[1];
            buf[2] = data[2];
            buf[3] = data[3];
            Copy_send(0x300, buf, 4);
            dataSet.checkinfo_ok = 0;
        }

        //发送球号设置
        public void Send_SaveBallSet(int[] data)
        {
            
            Copy_send(0x305, data, data.Length);
            dataSet.checkinfo_ok = 0;
        }
        //保存参数
        public void Send_Savebouns(int[] data)
        {
            int[] buf = new int[data.Length];
            buf[0] = data[0]*100;
            buf[1] = data[1]*100;
            buf[2] = data[2]*100;
            buf[3] = data[3]*100;
            buf[4] = data[4];
            buf[5] = data[5];
            buf[6] = data[6];
            buf[7] = data[7];
            Copy_send(0x303, buf, buf.Length);
            dataSet.checkinfo_ok = 0;
        }
        private void Get_SaveOption(int[] data)
        {
            dataSet.checkinfo_ok = data[0];
        }

        //保存参数
        public void Send_SaveSuper(int[] data,int len)
        {
            Copy_send(0x302, data, len);
            dataSet.checkinfo_ok = 0;
        }
        private void Get_SaveSuper(int[] data)
        {
            dataSet.checkinfo_ok = data[0];
        }

        //5x3布局信息
        private void Get_Slot5x3Info(int[] data)
        {
            //收到彩金号列表
            
            data.CopyTo(recpBuff, 0);
            Debug.Log($"路单：{string.Join(",", data)}");
            IOEventCenter.SendEvent(IOCenterEvent.Event_SlotRetrun, new object[1] { 1 });
        }
        private void Get_Slot5x3Info(object[] data)
        {
            //收到彩金号列表

            data.CopyTo(recpBuff, 0);
            Debug.Log($"路单：{string.Join(",", data)}");
            IOEventCenter.SendEvent(IOCenterEvent.Event_SlotRetrun, new object[1] { 1 });
            readySend = 0;
        }
        //获取5x3布局信息
        public void Send_getSlot5x3Info(int[] data)
        {
            if (CheckHardware.Inst.useGroup == 1 && CheckHardware.Inst.isHost == 0)
            {
                object[] args = new object[data.Length];
                for (int i = 0; i < data.Length; i++)
                    args[i] = data[i];
                IOEventCenter.SendEvent(IOCenterEvent.Event_SendNetworkBox, args);
            }
            else
            {
                //收到彩金号列表
                int[] buf = new int[data.Length];
                data.CopyTo(buf, 0);
                Copy_send(0x305, buf, data.Length);
                dataSet.checkinfo_ok = 0;
            }
            Debug.Log("获取5x3布局信息 " + CheckHardware.Inst.isHost + " " + CheckHardware.Inst.useGroup);
        }
        public void Send_getSlot5x3Info(object[] data)
        {
            //收到彩金号列表
            int[] buf = new int[data.Length];
            data.CopyTo(buf, 0);
            Copy_send(0x305, buf, data.Length);
            Debug.Log("获取5x3布局信息");
            dataSet.checkinfo_ok = 0;
            if (CheckHardware.Inst.isHost == 1)
            {
                if((int)data[5] == 1)
                IOEventCenter.SendEvent(IOCenterEvent.Event_JPAdd, new object[2] { data[10], data[11] });
            }
        }
        //彩金号信息回传
        private void Get_BounsNumberInfo(int[] data)
        {
            //收到彩金号列表
            Debug.Log($"算法卡彩金号：{string.Join(",", data)}");
        }
        //获取彩金号信息
        public  void Send_getBounsNubeerInfo(int[] data)
        {
            //收到彩金号列表
            int[] buf = new int[1];
            buf[0] = 0;
            Copy_send(0x303, buf, 1);
            Debug.Log("获取彩金号信息");
            dataSet.checkinfo_ok = 0;
        }
        
        //获取认球结果
        public void Send_getCurrBallNumber()
        {
            int[] buf = new int[1];
            buf[0] = 0;
            Copy_send(0x304, buf, 1);
            Debug.Log("获取球会号信息");
            dataSet.checkinfo_ok = 0;
        }
        //认球结果回传
        public void Get_BallNumberInfo(int[] data)
        {
            Debug.Log($"认球结果回传： {string.Join(", ", data)}");
        }
        //清空本轮
        public void Send_Clearbl( )
        {
            int[] buf = new int[1];
            buf[0] = 0;
            Copy_send(0x120, buf, 1);
            dataSet.checkinfo_ok = 0;
        }
        private void Get_Clearbl(int[] data )
        {
            dataSet.checkinfo_ok = data[0];
        }
        //读取单元数据
        public void Read_DataCell(int addr, int len)
        {
            int[] buf = new int[2];
            buf[0] = addr;
            buf[1] = len;
            Copy_send(0x320, buf, len);
            dataSet.checkinfo_ok = 0;
        }
        //读到单元数据
        public void Get_DataCell(int[] data)
        {
            //这里接收
            ReadCellData?.Invoke(data);
        }
        //保存所有玩家信息
        public void Send_savePlayerInfo(int[] data)
        {
            Copy_send(0x316, data, data.Length);
            dataSet.checkinfo_ok = 0;
        }
        //读取所有玩家信息
        public void Send_readPlayerInfo()
        {
            int[] buf = new int[1];
            buf[0] = 0;
            Copy_send(0x317, buf, 1);
            dataSet.checkinfo_ok = 0;
        }
        public void Send_updatesmTime()
        {
            int[] buf = new int[1];
            buf[0] = 0;
            Copy_send(0x318, buf, 1);
            dataSet.checkinfo_ok = 0;
            //IOManager.Inst.Inter_ideadata(0x318, buf);
        }
        //读到所有玩家信息
        public void Get_readPlayerInfo(int[] data)
        {
            //HostInfo.Instance.ReadPlayerInfo(data);
            checkok = 0;
        }
        public void Get_updatesmInfo(int[] data)
        {
            CheckHardware.Inst.DebugLog("当前算码信息：" + data[0] + " " + data[1]);
            checkok = 0;
        }
        //存储单元数据
        public void Write_DataCell(int addr, int[] data)
        {
            int[] buf = new int[data.Length + 1];
            buf[0] = addr;
            data.CopyTo(buf, 1);
            Copy_send(0x319, buf, data.Length + 1);
            dataSet.checkinfo_ok = 0;
        }
        //写入序列化数据
        public void Write_DataCellA(int[] data)
        {
            //int[] buf = new int[data.Length + 1];
            //data.CopyTo(buf, 1);
            Copy_send(0x321, data, data.Length);
            dataSet.checkinfo_ok = 0;
        }
    }
}
