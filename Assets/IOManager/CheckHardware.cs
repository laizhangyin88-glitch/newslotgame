using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SboxSpace;
//using System;

public enum InitHardware
{ 
    ConnetSandBox,
    InitBoardPort,
    InitPrinter,
    LoadId,
    LoadBoxVer,
    ReadBoard,
    ReadSandbox,
    SetMacId,
    Success,
    Fail,
    Wait,
};
public enum DeviceBoradMode
{ 
    Device_3288,
    Device_3128,
    Device_3588
}

public class CheckHardware : MonoBehaviour
{
    public static CheckHardware Inst;
    public GameObject Canvas;
    public int  hardware_ok = 0;
    public int  isHost  = 0;
    private InitHardware step = 0 ;
    InitHardware last_step = 0;
    InitHardware temp_step = 0;

   
    private float  dtime = 0 ;
    float fps = 0;
    float ttFps = 0;
    public  int macID = 0;
    public int useGroup = 0;
    public int useSandBox = 0;
    public DeviceBoradMode device;
    public int useRemotDebug = 0;
    public bool usePcMathDebug = false;
    public int deviceID = 0;

    //IO5255.Inst.dataSet.macid 机台号
    private void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
        }
    }
    void Start()
    {

    }
    private void OnDestroy()
    {
        //WebSocketServer.Stop();
    }
    public void DebugLog(string info)
    {
        Debug.Log(info);
    }
    // Update is called once per frame
    void Update()
    {
        fps = 1 / Time.deltaTime;
        ttFps += Time.deltaTime;
        if (ttFps >= 0.1f)
        {
            ttFps = 0;
            

        }
        if (useRemotDebug == 0)
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                if (hardware_ok == 0)
                {
                    hardware_ok = 1;
                    object[] args = new object[1];
                    args[0] = 0;
                    //WebSocketServer.StartRun();
                    IOEventCenter.SendEvent(IOCenterEvent.EVENT_HardCheckOK, args);

                    //if (isHost == 1)
                    //    NetworkDis.Inst.Start_GameNet(0);
                    //else
                    //    NetworkDis.Inst.Start_GameNet(macID);
                }
                return;
            }
        }
        if (temp_step != step)
        {
            temp_step = step;
            dtime = 0;
            DebugLog( "当前状态: " + step + " "  +last_step );
        }

        if (IOManager.Inst != null)
        {
            if (IOManager.Inst.cckeyio.m_connt == true)
            {
                //IOManager.Inst.Clear_key();
                //IOManager.Inst.Get_key();
                
            }
            if (IO5255.Inst != null)
            {
                IO5255.Inst.IOLoop();   
            }
        }
        dtime+=Time.deltaTime;
        switch(step)
        {
            case InitHardware.ConnetSandBox:
                {
                    bool open = IOManager.Inst.Open_Sendbox_Port(useSandBox);
                    last_step = step;
                    step = InitHardware.Wait;
                }
                break;
            case InitHardware.InitBoardPort:
                {//初始外部端口
                    if (IOManager.Inst.cckeyio.swtab[7] == 1)
                    {
                       // IOManager.Inst.InitPrinterPort();
                        dtime = 0;
                        last_step = step;
                        step = InitHardware.Wait;
                    }
                    else
                    {
                        step = InitHardware.LoadId;
                    }
                }
                break;
            case InitHardware.InitPrinter:
                {//初始化打印机
                    if (IOManager.Inst.cckeyio.swtab[7] == 1)
                    {
                        //PrintManger00.Inst.IntPrint();
                        IO5255.Inst.dataSet.UseBox = true;
                        dtime = 0;
                        last_step = step;
                        step = InitHardware.Wait;
                    }
                    else
                    {
                        step = InitHardware.LoadId;
                    }
                }
                break;
            case InitHardware.LoadId:
                {//加载机台号
                    Get_macid();
                    if (macID > 20)
                    {
                        //LoadManger.Inst.DisplayTs("錯誤臺號:" + PlayerInfo.Inst.id.ToString());
                        step = InitHardware.Fail;
                    }
                    else
                    {
                       // CheckHardware.Inst.DebugLog("机台号: " + PlayerInfo.Inst.id);
                        if (macID != 0)
                        {
                            if (device == DeviceBoradMode.Device_3128)
                                IOManager.Inst.ResetBoard3128();
                            else if (device == DeviceBoradMode.Device_3288)
                                IOManager.Inst.ResetBoard3128();
                            else if (device == DeviceBoradMode.Device_3588)
                                IOManager.Inst.Inter_ioKey3588();
                                step = InitHardware.ReadBoard;   //加载场景
                        }
                        else
                        {
                            last_step = step;
                            step = InitHardware.LoadBoxVer;
                        }
                    }

                }
                break;
            case InitHardware.LoadBoxVer:
                {//获取算法卡程序版本
                    IO5255.Inst.Reset_5255();
                    Init5255();
                    last_step = step;
                    step = InitHardware.Wait;
                }
                break;
            case InitHardware.ReadSandbox:
                {
                   
                    Load_5255Set();
                    last_step = step;
                    step = InitHardware.Wait;
                }
                break;
            case InitHardware.ReadBoard:
                {

                    //PlayerInfo.Inst.Read_PlayerInit();
                    last_step = step;
                    step = InitHardware.Wait;
                }
                break;
            case InitHardware.SetMacId:
                {
                    if (macID == 0)
                    {
                        if (IO5255.Inst.dataSet.macid < 9999999) //
                        {
                            GameInput.Inst.Init_input();
                            last_step = step;
                            step = InitHardware.Wait;
                        }
                        else
                        {
                            last_step = step;
                            step = InitHardware.Success;
                        }
                    }
                    else
                    {
                        last_step = step;
                        step = InitHardware.Success;
                    }
                }
                break;
            case InitHardware.Wait:
                {
                    if (last_step == InitHardware.ConnetSandBox)
                    {
                        if (dtime >= 2f)
                        {
                            if (useSandBox == 1)
                            {
                                if (IOManager.Inst.cckeyio.m_connt == true && IOManager.Inst.cckeyio.i_connt == true)
                                {

                                    step = InitHardware.InitBoardPort;
                                }
                                else
                                {
                                    if (IOManager.Inst.cckeyio.m_connt == false)
                                    {
                                        //LoadManger.Inst.DisplayTs("错误:無法讀取IO系統");
                                    }
                                    if (IOManager.Inst.cckeyio.i_connt == false)
                                    {
                                       // LoadManger.Inst.DisplayTs("错误:無法讀取IO系統2");
                                    }
                                }
                            }
                            else
                            {
                                if (IOManager.Inst.cckeyio.m_connt == true)
                                {
                                    step = InitHardware.InitBoardPort;
                                }
                                else
                                {
                                    //LoadManger.Inst.DisplayTs("错误:無法讀取IO系統");
                                }
                            }
                        }
                    }
                    else if (last_step == InitHardware.InitBoardPort)
                    {
                        if (dtime >= 5f)
                        {
                            if (IOManager.Inst.initBoardFlag == 1)
                            {
                                if (IOManager.Inst.cckeyio.swtab[7] == 1)
                                {
                                    step = InitHardware.InitPrinter;
                                }
                            }
                            else
                            {
                               // LoadManger.Inst.DisplayTs("错误:端口初始化失败");
                                step = InitHardware.Fail;
                            }
                        }
                    }
                    else if (last_step == InitHardware.InitPrinter)
                    {
                        if (dtime >= 1)
                        {
                            //if (PrintManger00.Inst.hardware_link == true)
                            //{
                            //    step = InitHardware.LoadId;
                            //}
                            //else
                            //{
                                //LoadManger.Inst.DisplayTs("错误:设备未连接");
                                step = InitHardware.Fail;
                            //}
                        }
                    }
                    else if (last_step == InitHardware.LoadBoxVer)
                    {
                        if (dtime >= 1f)
                        {
                            if (IO5255.Inst.checkok > 0)
                            {
                                if (IO5255.Inst.checkok == 2) //收到
                                {
                                    string str = "s:" + IO5255.Inst.dataSet.ver.ToString();
                                    //LoadManger.Inst.Display_IdeaTs(str);
                                    IO5255.Inst.checkok = 0;
                                    step = InitHardware.ReadSandbox;
                                }
                                else if (IO5255.Inst.checkok == 4)
                                {
                                    //LoadManger.Inst.DisplayTs("错误:通讯故障005");
                                    step = InitHardware.Fail;
                                    IO5255.Inst.checkok = 0;
                                }
                            }
                        }
                    }
                    else if (last_step == InitHardware.ReadSandbox)
                    {
                        if (dtime >= 1f)
                        {
                            if (IO5255.Inst.checkok > 0)
                            {
                                if (IO5255.Inst.checkok == 2) //收到
                                {
                                    step = InitHardware.ReadBoard;
                                    IO5255.Inst.checkok = 0;
                                }
                                else if (IO5255.Inst.checkok == 4)
                                {
                                    //LoadManger.Inst.DisplayTs("错误:通讯故障009");
                                    step = InitHardware.Fail;
                                    IO5255.Inst.checkok = 0;
                                }
                            }
                        }
                    }
                    else if (last_step == InitHardware.ReadBoard)
                    {
                        if (dtime >= 1f)
                        {
                            //if (PlayerInfo.Inst.init)
                            //{
                                step = InitHardware.SetMacId;
                            //}
                            //else
                            //{
                            //    if (dtime > 5)
                            //    {
                            //        step = InitHardware.Fail;
                            //        //LoadManger.Inst.DisplayTs("错误:通讯故障019");
                            //    }
                            //}
                        }
                    }
                    else if (last_step == InitHardware.SetMacId)
                    {
                        if (GameInput.Inst.Input_state == 1) //输入完毕
                        {
                            step = InitHardware.Success;
                            GameInput.Inst.Close_input();
                        }
                    }
                }
                break;
            case InitHardware.Success:
                {
                    if (hardware_ok == 0)
                    {
                        IOManager.Inst.IntIoParameter();
                        IOManager.Inst.CloseAllLight();
                        Casher.Inst.Init();
                        Printer.Inst.Init();
                        if (useSandBox == 0)
                            IOManager.Inst.CloseAllLight3128();
                        hardware_ok = 1;
                        isHost = IO5255.Inst.dataSet.g_display_mode;
                        //if (isHost == 1)
                        //    NetworkDis.Inst.Start_GameNet(0);
                        //else
                        //    NetworkDis.Inst.Start_GameNet(1);
                       // WebSocketServer.StartRun();
                        IOEventCenter.SendEvent(IOCenterEvent.EVENT_HardCheckOK, new object[1] { 0 });
                    }
                }
                break;
            case InitHardware.Fail:
                {
                }
                break;
        }
    }
    

    private void Init5255()
    {
        IO5255.Inst.Send_Init();
    }

    private void Load_5255Set()
    {
        IO5255.Inst.Send_Getoption();
    }

    private void OpenIO()
    {
        //if (IOManager.Inst.Open_Sendbox_Port(useSandBox))
        //{
        //    if(!IOManager.Inst.IntIoParameter())
        //    {
        //        LoadManger.Inst.DisplayTs("错误:通讯故障001");
        //        step = 100;
        //    }
        //}
        //else
        //{
        //    LoadManger.Inst.DisplayTs("错误:通讯故障100");
        //    step = 100;
        //}
    }
    private void Get_macid()
    {
        int id = 0;
        int[] sw = { 1, 2, 4, 8 };
        //PlayerInfo.Inst.id = macID;
        if (useRemotDebug == 0)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                //PlayerInfo.Inst.id = macID;
                return;
            }
        }


        for (int i = 0; i < 4; i++)
        {
            if (IOManager.Inst.cckeyio.swtab[i] == 1)
            {
                id += sw[i];
            }
        }
        macID = id;
        useGroup = IOManager.Inst.cckeyio.swtab[7];
                //CheckHardware.Inst.DebugLog( "机台号: " + PlayerInfo.Inst.id + " " + IOManager.Inst.cckeyio.swtab[0]);
    }
}
