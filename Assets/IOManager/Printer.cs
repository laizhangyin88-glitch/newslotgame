using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandboxApi;
using System;
using System.Text;

namespace SboxSpace
{
    public enum PrinterErr
    {
        Err_Normal,
        Err_Notspecified=-1,//未指定
        Err_Unconnect=-2,   //未连接
        Err_PrintFail=-3,   //打印失败
        Err_Wait=-4,
    }
    public class Printer : MonoBehaviour
    {
        public static Printer Inst;
        public List<string> PrinterList = new List<string>();//打印机列表
        public List<string> PrinterListA = new List<string>();
        public int PrinterErr =-2;
        public int print_ok = 0;
        public int currPrinterID = -1;

        int     fontSize = 5;
        public int store_id = 0; //id
        public int terminal = 0; //平台号
        public int ticket = 0;//
        public string credit="";
        float tDetal = 0;
        int testPrint = 0;
        int setList = 0;
        int readyPrint = 0;
        public string tgame_name = ""; 
        public string tlineNo = "";
        public string tMacNo = "";
        public string tCasinoName = "";
        public string tTerminal = "";
        float delayTime = 0;

        private void Awake ()
        {
            if (Inst == null)
            { 
                Inst = this;
                DontDestroyOnLoad(this);
            }
            //else
            //{
            //    Destroy(this);
            //}
        }
        void Start()
        {
            //DontDestroyOnLoad(this);
            //Init();
            IOEventCenter.AddListener(IOCenterEvent.Event_SetPrinterID, SetPrinterID);
            IOEventCenter.AddListener(IOCenterEvent.Event_StartPrint, SetPrintMoney);
        }

        void SetPrinterID(object[] args)
        {
            int[] data = new int[2];
            data[0] = (int)args[0];
            currPrinterID = (int)args[0];
            IOManager.Inst.SendDataToBoard(1851, data);
        }
        public void Init()
        {

            //PrinterList.Add("打印机1");
            //PrinterList.Add("打印机2");
            IOManager.Inst.PrinterStatusProcess += RecvPro;

            // SetPrinter(0);
            // SetFontSize(5);
            // 
           // SetPrintMoney("100","GIRLE","319","31900001","cashon","001");
        }

        //获取打印机列表
        public bool GetPrinterList()
        {
            if (PrinterListA.Count > 0)
                return true;
            int[] data = new int[1];
            data[0] = 0x7fff;
            return IOManager.Inst.SendDataToBoard(1850, data);
        }
        //设置打印机
        public bool SetPrinter(int id )
        {
            int[] data = new int[2];
            data[0] = id;
            currPrinterID = id;
            return IOManager.Inst.SendDataToBoard(1851, data);
        }
        //设置字体
        public bool SetFontSize(int size )
        {
            int[] data = new int[1];
            data[0] = size;
            fontSize = size;
            return IOManager.Inst.SendDataToBoard(1854, data);
        }
/*
        //打印数据
        bool PrintData()
        {
            int j = 0;
            int i = 0;
            int k = 0;
            string str = "";
            string tmp = "";

            //  credit = tcredit;
            string[] print_buf = {
                "",
                "AGENT: ",
                "STOR ID: ",
                "TERMINAL: ",
                "NAME: ",            
                "CREDITS: ",
                "",
                 "",
                  "",
                   "",
            };
            tgame_name = "GIRLE";
            tlineNo = "319";
            tMacNo = "31900001";
            tTerminal = "001";
            tCasinoName = "cashino";
            credit = "$100.00";
            print_buf[0] = tgame_name + "\r\n";
            print_buf[1] += tlineNo + "\r\n";
            print_buf[2] += tMacNo + "\r\n";
            print_buf[3] += tTerminal + "\r\n";
            print_buf[4] += tCasinoName + "\r\n";
            print_buf[5] += credit + "\r\n";

            string print_str = "";
            for (i = 0; i < 10; i++)
                print_str += print_buf[i];

            char[] sbuf = print_str.ToCharArray();
            byte[] buf = new byte[4];
            int leng = sbuf.Length / 4;
            if (sbuf.Length % 4 > 0)
                leng++;
            int[] data = new int[leng + 1];
            j++;
            for (i = 0; i < sbuf.Length; i += 4)
            {
                if ((i + 4) <= sbuf.Length)
                {
                    buf[0] = (byte)sbuf[i];
                    buf[1] = (byte)sbuf[i + 1];
                    buf[2] = (byte)sbuf[i + 2];
                    buf[3] = (byte)sbuf[i + 3];
                    data[j] = BitConverter.ToInt32(buf, 0);
                    str += data[j] + ",";

                    j++;
                }
                else
                    break;
            }
            if (i < sbuf.Length)
            {
                k = sbuf.Length - i;
                buf[0] = 0;
                buf[1] = 0;
                buf[2] = 0;
                buf[3] = 0;

                for (int jj = 0; jj < k; jj++)
                {
                    buf[jj] = (byte)sbuf[i + jj];
                }
                data[j] = BitConverter.ToInt32(buf, 0);
                str += data[j] + ",";
            }
            data[0] = sbuf.Length;
            tmp += data[0] + ",";
            tmp += str;
            Debug.Log(" 打印数据: \r\n" + print_str);
            Debug.Log(" 打印数据1: " + tmp);
            return IOManager.Inst.SendDataToBoard(1856, data);
        }
*/
        bool PrintData()
        {
            int j = 0;
            int i = 0;
            int k = 0;
            string str = "";
            string tmp = "";

            //  credit = tcredit;
            string[] print_buf = {
                "",
                "AGENT: ",
                "STOR ID: ",
                "TERMINAL: ",
                "NAME: ",
                "CREDITS: ",
                "",
                 "",
                  "",
                   "",
            };
            print_buf[0] = tgame_name + "\r\n";
            print_buf[1] += tlineNo + "\r\n";
            print_buf[2] += tMacNo + "\r\n";
            print_buf[3] += tTerminal + "\r\n";
            print_buf[4] += tCasinoName + "\r\n";
            print_buf[5] += credit + "\r\n";

            string print_str = "";
            for (i = 0; i < 10; i++)
                print_str += print_buf[i];

            char[] sbuf = print_str.ToCharArray();
            byte[] buf = new byte[4];
            int leng = sbuf.Length / 4;
            if (sbuf.Length % 4 > 0)
                leng++;
            int[] data = new int[leng + 1];
            j++;
            for (i = 0; i < sbuf.Length; i += 4)
            {
                if ((i + 4) <= sbuf.Length)
                {
                    buf[0] = (byte)sbuf[i];
                    buf[1] = (byte)sbuf[i + 1];
                    buf[2] = (byte)sbuf[i + 2];
                    buf[3] = (byte)sbuf[i + 3];
                    data[j] = BitConverter.ToInt32(buf, 0);
                    str += data[j] + ",";

                    j++;
                }
                else
                    break;
            }
            if (i < sbuf.Length)
            {
                k = sbuf.Length - i;
                buf[0] = 0;
                buf[1] = 0;
                buf[2] = 0;
                buf[3] = 0;

                for (int jj = 0; jj < k; jj++)
                {
                    buf[jj] = (byte)sbuf[i + jj];
                }
                data[j] = BitConverter.ToInt32(buf, 0);
                str += data[j] + ",";
            }
            data[0] = sbuf.Length;
            tmp += data[0] + ",";
            tmp += str;
            Debug.Log(" 打印数据: \r\n" + print_str);
            Debug.Log(" 打印数据1: " + tmp);
            return IOManager.Inst.SendDataToBoard(1856, data);
        }

        //设置打印信息
        public bool SetPrintMoney(string tcredit, string game_name, string lineNo, string MacNo, string CasinoName, string Terminal)
        {
            if (PrinterErr < 0)
            {
                print_ok = 2;
                return false;
            }
            readyPrint = 1;
            tgame_name = game_name;
            tlineNo = lineNo;
            tMacNo = MacNo;
            tCasinoName = CasinoName;
            tTerminal = Terminal;
            credit = tcredit;
            print_ok = 1;
            delayTime = 0;
            return true;
        }
        public void SetPrintMoney(object[] args)//string tcredit, string game_name, string lineNo, string MacNo, string CasinoName, string Terminal
        {
            if (PrinterErr < 0)
            {
                IOEventCenter.SendEvent(IOCenterEvent.EVENT_PrintOK, new object[1] { 1 });
                print_ok = 2;
                return;
            }
            readyPrint = 1;
            tgame_name = args[1] + "";
            tlineNo = args[2] + "";
            tMacNo = args[3] + "";
            tCasinoName = args[4] + "";
            tTerminal = args[5] + "";
            credit = args[0]+"";
            print_ok = 1;
            delayTime = 0;
        }
        //是否打印成功
        public int CheckPrintOK()
        {
            return (int)PrinterErr;
        }
        //设置打印时间
        public bool SetPrinterTime()
        {
            int[] data = new int[6];
            System.DateTime t = new DateTime();
            data[0] = t.Year;
            data[1] = t.Month;
            data[2] = t.Day;
            data[3] = t.Hour;
            data[4] = t.Minute;
            data[5] = t.Second;
            return IOManager.Inst.SendDataToBoard(1857, data);
        }
        public void RecvPro(int cmd, int[] data)
        {
            switch (cmd)
            {
                case 1850:
                    {
                        int len = data.Length * 4;
                        byte[] sbuf = new byte[len +1];
                        byte[] buff = new byte[4];
                        string s = "";
                        string s1 = "";
                        int j = 0;
                        for (int i = 1; i < data.Length; i++)
                        {
                            buff = BitConverter.GetBytes(data[i]);
                            sbuf[4 * j] = buff[0];
                            sbuf[4 * j + 1] = buff[1];
                            sbuf[4 * j + 2] = buff[2];
                            sbuf[4 * j + 3] = buff[3];

                            //s1 += buff[0];
                            //s1 += ",";
                            //s1 += buff[1];
                            //s1 += ",";
                            //s1 += buff[2];
                            //s1 += ",";
                            //s1 += buff[3];
                            //s1 += ",";
                            j++;
                            
                        }
                        sbuf[j * 4] = 0;
                         s = Encoding.UTF8.GetString(sbuf, 0, j*4 +1 );
                        string[] devBuf = s.Split('.');
                        string head = "";
                        for (int i = 0; i < devBuf.Length; i++)
                        {
                            string[] brand_buf = devBuf[i].Split(':');
                            head = brand_buf[0];
                            string[] dev_buf = devBuf[i].Split(',');
                            if (dev_buf.Length > 0)
                            {
                                for (j = 0; j < dev_buf.Length; j++)
                                {
                                    if (dev_buf[j] != "")
                                    {
                                        //Debug.Log("打印机列表: " + head + ":" + dev_buf[j]);
                                        PrinterList.Add(head);
                                        string[] brand_buf1 = dev_buf[j].Split(':');
                                        if (brand_buf1.Length > 1)
                                            PrinterListA.Add(brand_buf1[1]);
                                        else if (brand_buf1.Length > 0)
                                            PrinterListA.Add(brand_buf1[0]);
                                        else
                                            PrinterListA.Add(dev_buf[j]);

                                    }
                                    // Debug.Log("打印机列表: " + head + ":" + dev_buf[j]);


                                }
                            }
                            else
                            {
                                //PrinterList.Add(head + ":" + devBuf[i]);
                                PrinterList.Add(head);
                                string[]  brand_buf1 =  devBuf[i].Split(':');
                                    if (brand_buf1.Length > 1)
                                        PrinterListA.Add(brand_buf1[1]);
                                    else if (brand_buf1.Length > 0)
                                        PrinterListA.Add(brand_buf1[0]);
                                    else
                                        PrinterListA.Add(dev_buf[i]);
                                // Debug.Log("打印机列表: " + head + ":" + devBuf[i]);

                            }
                        }
                        if (setList == 1)
                        {
                            SetPrinterTime();
                            setList = 2;
                        }
                        //           

                    }
                        break;
                case 1851:
                    {
                        if( data[0] == 0 )
                        {
                            Debug.Log("设置打印机成功ID: " + currPrinterID);
                        }
                        else
                            Debug.Log("设置打印机失败");
                    }
                    break;
                case 1801:
                    {
                       
                        if(PrinterErr!=data[3])
                        {
                            PrinterErr = data[3];
                            IOEventCenter.SendEvent(IOCenterEvent.EVENT_PrintStatus, new object[1] { PrinterErr });
                        }
                           
                        if (setList == 2)
                        {
                            setList = 3;
                            for (int i = 0; i < PrinterList.Count - 1; i++)
                                IOEventCenter.SendEvent(IOCenterEvent.EVENT_PrinterList,new object[4]{i,PrinterList[i],PrinterListA[i],PrinterErr });
                        //        SQLite.Instance.SetData_DYJ(PrinterList[i], PrinterListA[i], (int)PrinterErr);
                        }
                        else
                        {
                            if (setList == 0)
                            { 
                                setList = 1;
                                GetPrinterList();
                            }
                        }
                        if (PrinterErr == 0)
                        {
                            if (readyPrint == 2)
                            {
                                readyPrint = 0;
                                print_ok = 0;
                                IOEventCenter.SendEvent(IOCenterEvent.EVENT_PrintOK, new object[1] { 0 });
                            }
                            if (readyPrint == 1)
                            {
                                if (delayTime >= 1.2)
                                {

                                    readyPrint ++;
                                    PrintData();
                                }
                            }
                        }
                        else if (PrinterErr == -3 )
                        {
                            print_ok = 2;
                        }
                        if (PrinterErr < 0)
                        {
                            if (readyPrint == 2)
                            {
                                readyPrint = 0;
                                
                            }
                            if (print_ok == 1)
                            {
                                print_ok = 2;
                                IOEventCenter.SendEvent(IOCenterEvent.EVENT_PrintOK, new object[1] { 1 });
                            }
                        }

                    }
                    break;
                case 1853:
                    {
                        if (data[0] == 0)
                        {
                            Debug.Log("恢复出厂设置成功");
                        } 
                        else
                            Debug.Log("恢复出厂设置失败");
                        
                    }
                    break;
                case 1854:
                    {
                        if (data[0] == 0)
                           Debug.Log("设置字体成功");
                        else
                            Debug.Log("设置字体失败");
                    }
                    break;
                case 1855:
                    {
                        if (data[0] == 0)
                            Debug.Log("切纸成功");
                        else
                            Debug.Log("切纸失败");
                    }
                    break;
                case 1856:
                    {
                        if (data[0] == 0)
                        {
                            Debug.Log("打印成功");

                        }
                        else
                        {
                            
                            Debug.Log("打印失败");
                        }
                    }
                    break;
                case 1857:
                    {
                        if (data[0] == 0)
                            Debug.Log("时间设置成功");
                        else
                            Debug.Log("时间设置失败");
                    }
                    break;
                default:
                    {
                    }
                    break;
            }
        }
        private void FixedUpdate()
        {

            if (CheckHardware.Inst.useRemotDebug == 0)
            {
                if (Application.platform != RuntimePlatform.Android)
                    return;
            }
            if (readyPrint == 1)
                delayTime += Time.deltaTime;
            //tDetal += Time.deltaTime;
            //if (tDetal >= 5)
            //{
            //    tDetal = 0;
            //    if (testPrint == 0)
            //    {
            //        testPrint = 1;
            //        SetPrintMoney(100.00f);
            //    }
            //}
        }
    }
}