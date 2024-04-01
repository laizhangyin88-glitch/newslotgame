using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandboxApi;
using System;
using System.Text;

namespace SboxSpace
{
    //纸钞机
    public enum CahserErr
    {
        Err_Normal = 0,
        Err_Notspecified = -1,    //未指定
        Err_Unconnect=-2,       //未连接      
        Err_CashBlock=-3,       //卡钞
        Err_BoxErr=-4,        //钱箱错误 
        Err_Unkown=-5,      //未知错误
        //Err_BoxCheat,        //作弊
       
        //Err_BoxOpen,        //开箱
    }
    public class Casher : MonoBehaviour
    {
        public static Casher Inst;
        public List<string> CashBoxList = new List<string>();//纸钞机列表
        public List<string> CashBoxListA = new List<string>();
        public int CasherErr = 0;//纸钞机故障代码
        public bool rejactCash = false;//拒收纸钞 可以设置为拒收
        public int deposit = 0;//接收到纸钞数额
        int currCasherID = -1;//设置的ID

        int setList = 0;
        int tdeposit = 0;
        private void Awake()
        {
            if (Inst == null)
            {
                Inst = this;
                DontDestroyOnLoad(this);
            }
        }
        void Start()
        {
            IOEventCenter.AddListener(IOCenterEvent.Event_RejectCash,  RejectCash );
            IOEventCenter.AddListener(IOCenterEvent.Event_SetCahserID, SetCasherID);
        }
        public void Init()
        {
             IOManager.Inst.CasherStatusProcess += RecvPro;
  
        }

        void SetCasherID(object[] args)
        {
            int[] data = new int[2];
            data[0] = (int)args[0];
            currCasherID = (int)args[0];
            IOManager.Inst.SendDataToBoard(1831, data);
        }
        //获取打印机列表
        public bool GetCasherList()
        {
            if (CashBoxListA.Count > 0)
                return true;
            int[] data = new int[1];
            data[0] = 0x7fff;
            return IOManager.Inst.SendDataToBoard(1830, data);
        }
        //收钞
        public bool AcceptCash()
        {
            int[] data = new int[1];
            data[0] = 0x7fff;
            return IOManager.Inst.SendDataToBoard(1833, data);
        }
        //拒钞
        public bool RejectCash()
        {
            int[] data = new int[1];
            data[0] = 0x7fff;
            return IOManager.Inst.SendDataToBoard(1834, data);
        }
        public void  RejectCash(object[] args)
        {
            rejactCash = (bool)args[0];
           // int[] data = new int[1];
           // data[0] = 0x7fff;
           //IOManager.Inst.SendDataToBoard(1834, data);
        }
        //设置纸钞机
        public bool SetCasher(int id )
        {
            int[] data = new int[2];
            data[0] = id;
            currCasherID = id;
            return IOManager.Inst.SendDataToBoard(1831, data);
        }
        public void RecvPro(int cmd, int[] data)
        {
            switch (cmd)
            {
                case 1830:
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
                       // Debug.Log("纸钞机列表A: " +s);
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
                                    CashBoxList.Add(head);
                                    string[] brand_buf1 = dev_buf[j].Split(':');
                                        if (brand_buf1.Length > 1)
                                            CashBoxListA.Add(brand_buf1[1]);
                                        else if (brand_buf1.Length > 0)
                                            CashBoxListA.Add(brand_buf1[0]);
                                        else
                                            CashBoxListA.Add(dev_buf[j]);
     //                               Debug.Log("纸钞机列表: " + dev_buf[j]+ " " + brand_buf[0] + " ");

                                }
                            }
                            else
                            {
                                CashBoxList.Add(head);
                                string[] brand_buf1 = dev_buf[i].Split(':');
                                    if (brand_buf1.Length > 1)
                                        CashBoxListA.Add(brand_buf1[1]);
                                    else if (brand_buf1.Length > 0)
                                        CashBoxListA.Add(brand_buf1[0]);
                                    else
                                        CashBoxListA.Add(dev_buf[i]);
                                // Debug.Log("纸钞机列表: " + head + ":" + devBuf[i]);
                                // SQLite.Instance.SetData_ZCJ(head, devBuf[i], (int)CasherErr);
                            }
                        }
                        if(setList == 1)
                           setList = 2;

                        for ( int i = 0; i < CashBoxList.Count-1; i ++ )
                            Debug.Log("纸钞机列表: " + CashBoxListA[i]);

                    }
                    break;
                case 1831:
                    {
                        if( data[0] == 0 )
                        {
                            Debug.Log("设置纸钞机成功ID: " + currCasherID);
                        }
                        else
                            Debug.Log("设置纸钞机失败");

                    }
                    break;
                case 1801:
                    {
                        if (CasherErr != data[4])
                        {
                            CasherErr = data[4];
                            IOEventCenter.SendEvent(IOCenterEvent.EVENT_CashStatus, new object[1] { CasherErr });
                        }
                      //  
                        if (data[5] > 0)
                        {
                            if (rejactCash == false)
                                AcceptCash();
                            else
                                RejectCash();
                            tdeposit = data[5];
                        }
                        if (setList == 2)
                        {
                            setList = 3;
                            Debug.Log("纸钞机状态：" + CasherErr + "  " + data[3] + "  " + data[4] + "  " + data[5]);
                            for (int i = 0; i < CashBoxList.Count - 1; i++)
                            {
                                Debug.Log("纸钞列表：" + CashBoxList[i] + ",  " + CashBoxListA[i]);
                                IOEventCenter.SendEvent(IOCenterEvent.EVENT_CahserList, new object[4] { i, CashBoxList[i], CashBoxListA[i], CasherErr });
                                //SQLite.Instance.SetData_ZCJ(CashBoxList[i], CashBoxListA[i], (int)CasherErr);
                            }
                        }
                        else
                        {
                            if (setList == 0)
                            {
                                setList = 1;
                                GetCasherList();
                            }
                        }
                        if (data[6] == 1)
                        {
                            deposit += tdeposit;
                            if(tdeposit>0)
                            IOEventCenter.SendEvent(IOCenterEvent.EVENT_CashIn, new object[1] { tdeposit });
                            tdeposit = 0;
                        }
                    }
                    break;
                case 1833:
                    {
                        if (data[0] == 0)
                        {
                            Debug.Log("纸钞机接收成功");
                            //deposit += tdeposit;
                            //tdeposit = 0;
                        } 
                        else
                            Debug.Log("纸钞机接收失败");
                        
                    }
                    break;
                case 1834:
                    {
                        if (data[0] == 0)
                        {
                            Debug.Log("纸钞机拒收成功");
                            tdeposit = 0;
                        }
                        else
                            Debug.Log("纸钞机拒收失败");
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
            if (Application.platform != RuntimePlatform.Android)
                return;
        }
    }
}