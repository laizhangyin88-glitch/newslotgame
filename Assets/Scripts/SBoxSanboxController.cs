using BagelCode;
using BlizzEvent;
using Newtonsoft.Json;
using ParadoxNotion;
using SBoxApi;
using Sirenix.OdinInspector;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using static SBoxApi.SBoxSandbox;

public class SBoxSanboxController : EventMonoSingleton<SBoxSanboxController>
{
    private int coinOutNum;

    /*private enum LightStatus
    {
        OFF = 0,
        ON = 1,
    }*/

    /*[Button]
    void test_showBtns()
    {
        string temp = "";
        sceneBtns.ForEach(item =>
        {
            temp += $"#{item}";
        });
        Debug.Log($" @btns = {temp};");
    }*/

    void Start()
    {
        if (!ApplicationSettings.Instance.isMachine)
            return;

        AddButtonEvent();
        AddEventListener();

        MessageDispatcher.Register("MachineBtnEvent", OnMachineBtnEvent);


        this._taskTimer = new System.Timers.Timer(3000);
        this._taskTimer.AutoReset = true; // 是否重复执行
        this._taskTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
        {
            task = () =>  //延时，避免   OSA_LobbySlots.ResetCurSelect() 影响
            {
                Debug.Log("【BillLst】get bill list ...");
                GetBillLst();
                GetPrintList();
            };
        };
        //this._keepAliveTimer.Enabled = true; //开始执行
        this._taskTimer.Start();
    }


    System.Action task;
    bool isRuning = false;
    public void Update()
    {
        if (!isRuning)
        {
            isRuning = true;
            if (task != null)
            {
                task();
                task = null;
            }
            isRuning = false;
        }
    }

    protected override void OnDestroy()
    {
        MessageDispatcher.UnRegister("MachineBtnEvent", OnMachineBtnEvent);  //"ShowLightSelectTip"

        if (this._taskTimer != null)
        {
            this._taskTimer.Stop();
            this._taskTimer.Dispose();
            this._taskTimer = null;
        }
    }

    protected System.Timers.Timer _taskTimer = null;

    /*
    System.Action task;

    protected System.Timers.Timer _taskTimer = null;

    bool isRuning = false;

    private void Update()
    {
        if (!isRuning)
        {
            isRuning = true;
            if (task != null)
            {
                task();
                task = null;
            }
            isRuning = false;
        }
    }
    */


    private void OnMachineBtnEvent(ParadoxNotion.EventData eventData)
    {

        if (eventData.name == "LightBtnSelectShowTipOn")//"LightBtnSelectShowTip"  "LightBtnSelect"
        {
            StartCoroutine(OnLightBtnSelectShowTip((int)eventData.value));
        }

        if (eventData.name == "LightBtnSelectShowTipOff")
        {
            CloseLightTip();
        }
        /*
        if (eventData.name == "LightBtnOn")
        {
            LightOn((SBOX_SWITCH)eventData.value);
        }
        if (eventData.name == "LightBtnOff")
        {
            LightOff((SBOX_SWITCH)eventData.value);
        }
        */
        if (eventData.name == "LightBtnOpenSpin")
        {
            LightOn(SBOX_SWITCH.SWITCH_ENTER);
        }

        if (eventData.name == "LightBtnCloseSpin")
        {
            LightOff(SBOX_SWITCH.SWITCH_ENTER);
        }

        if (eventData.name == "ChangeSceneBtnLight")
        {
            string[] map = (string[])eventData.value;

            List<SBOX_SWITCH> include = new List<SBOX_SWITCH>();
            for (int i = 0; i < map.Length; i++)
            {
                if (!include.Contains(keyMap[map[i]]))
                {
                    include.Add(keyMap[map[i]]);
                }
            }

            sceneBtns = include;
            setSceneLightOn();
        }

    }

    readonly Dictionary<string, SBOX_SWITCH> keyMap = new Dictionary<string, SBOX_SWITCH>()
    {
        { "BtnSpin", SBOX_SWITCH.SWITCH_ENTER },
        { "BtnPre", SBOX_SWITCH.SWITCH_YELLOW },
        { "BtnNext", SBOX_SWITCH.SWITCH_BET4 },
        { "BtnExit", SBOX_SWITCH.SWITCH_SWITCH },
        { "BtnSwitch", SBOX_SWITCH.SWITCH_BET5 },
        { "BtnBetUp", SBOX_SWITCH.SWITCH_RED },
        { "BtnBetDown", SBOX_SWITCH.SWITCH_GREEN },
        { "BtnBetMax", SBOX_SWITCH.SWITCH_AUTO},
        { "BtnHelp", SBOX_SWITCH.SWITCH_ESC},
    };


    bool isLightBtnSelectShowTip = false;

    public List<SBOX_SWITCH> lightBtn = new List<SBOX_SWITCH>() {
        //上一排（从左到右）
        SBOX_SWITCH.SWITCH_RED,
        SBOX_SWITCH.SWITCH_GREEN,
        SBOX_SWITCH.SWITCH_YELLOW,
        SBOX_SWITCH.SWITCH_BET4,
        //下一排（从左到右）
        SBOX_SWITCH.SWITCH_AUTO,
        SBOX_SWITCH.SWITCH_ESC,
        SBOX_SWITCH.SWITCH_SWITCH,
        SBOX_SWITCH.SWITCH_BET5,
        //大健
        SBOX_SWITCH.SWITCH_ENTER,
    };


    List<SBOX_SWITCH> sceneBtns = new List<SBOX_SWITCH>();

    public void setSceneLightOn()
    {
        for (int i = 0; i < lightBtn.Count; i++)
        {
            if (sceneBtns.Contains(lightBtn[i]))
            {
                LightOn(lightBtn[i]);
            }
            else
            {
                LightOff(lightBtn[i]);
            }
        }
    }

    public IEnumerator OnLightBtnSelectShowTip(int num)
    {

        for (int i = 0; i < lightBtn.Count; i++)
        {
            LightOff(lightBtn[i]);
        }

        isLightBtnSelectShowTip = true;
        while (isLightBtnSelectShowTip)
        {
            for (int i = 0; i < num; i++)
            {
                LightOn(lightBtn[i]);
            }
            yield return new WaitForSeconds(1);

            for (int i = 0; i < num; i++)
            {
                LightOff(lightBtn[i]);
            }
            yield return new WaitForSeconds(0.8f);
        }
        setSceneLightOn();
        yield return null;
    }


    public IEnumerator OnSpin()
    {
        isLightBtnSelectShowTip = true;
        while (isLightBtnSelectShowTip)
        {

            LightOn(SBOX_SWITCH.SWITCH_ENTER);

            yield return new WaitForSeconds(1);


            LightOff(SBOX_SWITCH.SWITCH_ENTER);

            yield return new WaitForSeconds(0.8f);
        }

        yield return null;
    }

    /*
    void LightOnAll()
    {
        setSceneLightOn();

        LightOn(SBOX_SWITCH.SWITCH_ENTER); //Spin
    }*/

    void LightOn(SBOX_SWITCH key)
    {
#if UNITY_EDITOR
        MatchDebugManager.Instance.SendUdpMessage(EventHandle.SBOX_SADNBOX_SWITCH_ON, ((ulong)key).ToString());
#else
        SwitchOutStateOn((ulong)key);
#endif
    }
    void LightOff(SBOX_SWITCH key)
    {
#if UNITY_EDITOR
        MatchDebugManager.Instance.SendUdpMessage(EventHandle.SBOX_SADNBOX_SWITCH_OFF, ((ulong)key).ToString());
#else
        SwitchOutStateOff((ulong)key);
#endif
    }



    private void AddButtonEvent()
    {

#if UNITY_EDITOR
        EventCenter.Instance.AddEventListener<SBOX_SWITCH>(EventHandle.HARDWARE_KEY_DOWN, OnKeyDown);
        EventCenter.Instance.AddEventListener<SBOX_SWITCH>(EventHandle.HARDWARE_KEY_UP, OnKeyUp);
#else
        if (ApplicationSettings.Instance.isMachine){
            //SBoxSandboxListener.Instance.AddButtonDown(SBOX_SWITCH.SWITCH_UP, () => { EventSender.SendGlobalEvent(MetaEventDefine.ON_MACHINE, new EventData<int>(MachineEventDefine.ON_KEY_UP, 0)); });
            //SBoxSandboxListener.Instance.AddButtonDown(SBOX_SWITCH.SWITCH_DOWN, () => { EventSender.SendGlobalEvent(MetaEventDefine.ON_MACHINE, new EventData<int>(MachineEventDefine.ON_KEY_DOWN, 0)); });
            //SBoxSandboxListener.Instance.AddButtonDown(SBOX_SWITCH.SWITCH_LEFT, () => {
            //    //选择框左移
            //    MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_LEFT));
            //    //降低押注
            //    MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetDown"));
            //});
            //SBoxSandboxListener.Instance.AddButtonDown(SBOX_SWITCH.SWITCH_RIGHT, () => {
            //    //选择框右移
            //    MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_RIGHT));
            //    //提高押注
            //    MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetUp"));
            //});



           /* SBoxSandboxListener.Instance.AddButtonDown(SBOX_SWITCH.SWITCH_ENTER, () => {
                if (PopupManager.Instance.popupCount > 0)
                    {
                        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Return"));
                        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Collect"));
                        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnClose"));
                        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnCollect"));
                        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnPointerClick"));
                        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnStartFreeSpin"));
                    }
                    else
                        MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData<int>(MachineEventDefine.ON_KEY_START, 1));
            });
            SBoxSandboxListener.Instance.AddButtonDown(SBOX_SWITCH.SWITCH_ESC, () => {
                if (PopupManager.Instance.popupCount == 0)
                    EventSender.SendGlobalEvent("OpenPaytable");
            });
            SBoxSandboxListener.Instance.AddButtonDown(SBOX_SWITCH.SWITCH_SWITCH, () => {
                //最大下注
                MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetMax"));
                //炮升级
                //EventSender.SendGlobalEvent(MetaEventDefine.ON_MACHINE, new EventData<int>(MachineEventDefine.ON_KEY_MAX_BET, data));
            });
            SBoxSandboxListener.Instance.AddButtonDown(SBOX_SWITCH.SWITCH_RED, () => {
                StartCoroutine(PurchaseCreditRequest(1, 100));
            });
            SBoxSandboxListener.Instance.AddButtonDown(SBOX_SWITCH.SWITCH_GREEN, () => {
                StartCoroutine(PurchaseCreditRequest(2, 100));
            });
            SBoxSandboxListener.Instance.AddButtonDown(SBOX_SWITCH.SWITCH_YELLOW, () => {
                //选择框左移
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_LEFT));
                //降低押注
                MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetDown"));
            });
            SBoxSandboxListener.Instance.AddButtonDown(SBOX_SWITCH.SWITCH_BET4, () => {
                //选择框右移
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_RIGHT));
                //提高押注
                MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetUp"));
            });
            SBoxSandboxListener.Instance.AddButtonDown(SBOX_SWITCH.SWITCH_BET5, () => {
                if (BlackboardQueryUtils.IsSpin()
                        || BlackboardQueryUtils.IsAutoSpin())
                    return;
                //退出
                EventSender.SendGlobalEvent("OnLobby");
                //鱼房间退出
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_BET5));
                //鱼机 退出
                MessageDispatcher.Dispatch(MetaEventDefine.ON_MACHINE, new EventData(MachineEventDefine.ON_KEY_BET5));
            });
            SBoxSandboxListener.Instance.AddButtonUp(SBOX_SWITCH.SWITCH_ENTER, () => {
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData<int>(MachineEventDefine.ON_KEY_START, 0));
            });
            */

            foreach (SBOX_SWITCH value in Enum.GetValues(typeof(SBOX_SWITCH)))
            {
                SBoxSandboxListener.Instance.AddButtonDown(value, () => {
                    OnKeyDown(value);
                });
                SBoxSandboxListener.Instance.AddButtonUp(value, () => {
                    OnKeyUp(value);
                });
            }
           //SBoxSandboxListener.Instance.AddButtonUp(SBOX_SWITCH.SWITCH_ENTER, () => {
           //     OnKeyUp(SBOX_SWITCH.SWITCH_ENTER);
           //});

        }
#endif
    }


    /*

    【上面一排（从左到右）】：
    1: KeyDown SWITCH_RED
    2: KeyDown SWITCH_GREEN
    3: KeyDown SWITCH_YELLOW
    4: KeyDown SWITCH_BET4


    【下面一排（从左到右）】：
    1: KeyDown SWITCH_AUTO
    2: KeyDown SWITCH_ESC
    3: KeyDown SWITCH_SWITCH
    4: KeyDown SWITCH_BET5

    【右边大按钮】：
    KeyDown SWITCH_ENTER；

    【左边大按钮】：
    没有接

    【斜坡按钮】：
    没有接

    【上分/下分】：
    没有接

    【退票】：
    没有接

    */

    private void CloseLightTip()
    {
        if (isLightBtnSelectShowTip)
        {
            isLightBtnSelectShowTip = false;
            StopAllCoroutines();
            setSceneLightOn();
        }
        return;
    }

    private void OnKeyDown(SBOX_SWITCH sBOX_SWITCH)
    {


#if UNITY_EDITOR
        Debug.LogError("KeyDown " + sBOX_SWITCH);
#endif


        if (isLightBtnSelectShowTip && !lightBtn.Contains(sBOX_SWITCH))
        {
            return;
        }

        if (isLightBtnSelectShowTip && lightBtn.Contains(sBOX_SWITCH))
        {
            CloseLightTip();
            MessageDispatcher.Dispatch("MachineBtnEvent", new EventData<int>("LightBtnSelect", lightBtn.IndexOf(sBOX_SWITCH)));
            return;
        }


        switch (sBOX_SWITCH)
        {
            case SBOX_SWITCH.SWITCH_UP:
                //EventSender.SendGlobalEvent(MetaEventDefine.ON_MACHINE, new EventData<int>(MachineEventDefine.ON_KEY_UP, 0));
                break;
            case SBOX_SWITCH.SWITCH_DOWN:
                //EventSender.SendGlobalEvent(MetaEventDefine.ON_MACHINE, new EventData<int>(MachineEventDefine.ON_KEY_DOWN, 0));
                break;
            case SBOX_SWITCH.SWITCH_LEFT:
                //选择框左移
                //MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_LEFT));
                //降低押注
                //MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetDown"));
                break;
            case SBOX_SWITCH.SWITCH_RIGHT:
                //选择框右移
                //MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_RIGHT));
                //提高押注
                //MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetUp"));
                break;
            case SBOX_SWITCH.SWITCH_ROOT_SET:
                break;
            case SBOX_SWITCH.SWITCH_SET:
                break;
            case SBOX_SWITCH.SWITCH_DOOR_SWITCH:
                break;
            case SBOX_SWITCH.SWITCH_PAYOUT:
                break;
            case SBOX_SWITCH.SWITCH_ENTER: //Spin
                /*if (PopupManager.Instance.popupCount > 0)
                {
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Return"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Collect"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnClose"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnCollect"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnPointerClick"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnStartFreeSpin"));
                }
                else
                    MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData<int>(MachineEventDefine.ON_KEY_START, 1));
               */

                MachineSelectManager.Instance.BtnSpinDown();

                /*if (this._taskTimer != null && sandBoxBillList == null)
                {
                    this._taskTimer.Start();
                }*/
                break;
            case SBOX_SWITCH.SWITCH_ESC:
                /*if (PopupManager.Instance.popupCount == 0)
                    EventSender.SendGlobalEvent("OpenPaytable");
                */
                MachineSelectManager.Instance.BtnHelp();
                break;
            case SBOX_SWITCH.SWITCH_SWITCH:
                /*//最大下注
                MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetMax"));
                //炮升级
                //EventSender.SendGlobalEvent(MetaEventDefine.ON_MACHINE, new EventData<int>(MachineEventDefine.ON_KEY_MAX_BET, data));
                */
                MachineSelectManager.Instance.BtnReturn();
                break;
            case SBOX_SWITCH.SWITCH_SCORE_UP:
                //MachineSelectManager.Instance.BtnAddCoin();
                MachineSelectManager.Instance.PurchaseCreditRequest(1, 10000);//加分
                break;
            case SBOX_SWITCH.SWITCH_SCORE_DOWN:
                int credit = (int)(BlackboardUtils.FindVariable<long>(null, "/me/credit").value / 1000) * 1000;
                Debug.Log($"【printer】: All dollar = {BlackboardUtils.FindVariable<long>(null, "/me/credit").value / 1000}, credit = {credit} ");
                if(credit > 0)
                {
                    MachineSelectManager.Instance.PurchaseCreditRequest(2, credit, () =>
                    {
                        test_PrinterMessage(credit);
                    });//减分
                }

                break;
            case SBOX_SWITCH.SWITCH_RED:
                MachineSelectManager.Instance.BtnBetUp();
                break;
            case SBOX_SWITCH.SWITCH_GREEN:
                MachineSelectManager.Instance.BtnBetDown();
                break;
            case SBOX_SWITCH.SWITCH_YELLOW:
                /*//选择框左移
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_LEFT));
                //降低押注
                MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetDown"));
                */
                MachineSelectManager.Instance.BtnPre();
                break;
            case SBOX_SWITCH.SWITCH_BET4:
                /*//选择框右移
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_RIGHT));
                //提高押注
                MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetUp"));
                */
                MachineSelectManager.Instance.BtnNext();
                break;
            case SBOX_SWITCH.SWITCH_BET5:
                /*if (BlackboardQueryUtils.IsSpin()
                    || BlackboardQueryUtils.IsAutoSpin())
                    return;
                //退出
                EventSender.SendGlobalEvent("OnLobby");
                //鱼房间退出
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_BET5));
                //鱼机 退出
                MessageDispatcher.Dispatch(MetaEventDefine.ON_MACHINE, new EventData(MachineEventDefine.ON_KEY_BET5));
                */
                MachineSelectManager.Instance.BtnSwitch();
                break;
            case SBOX_SWITCH.SWITCH_AUTO:
                MachineSelectManager.Instance.BtnBetMax();
                break;
            default:
                break;
        }
    }

    private void OnKeyUp(SBOX_SWITCH sBOX_SWITCH)
    {
#if UNITY_EDITOR
        Debug.LogError("KeyUp " + sBOX_SWITCH);
#endif


        if (isLightBtnSelectShowTip && !lightBtn.Contains(sBOX_SWITCH))
        {
            return;
        }


        switch (sBOX_SWITCH)
        {
            case SBOX_SWITCH.SWITCH_UP:
                break;
            case SBOX_SWITCH.SWITCH_DOWN:
                break;
            case SBOX_SWITCH.SWITCH_LEFT:
                break;
            case SBOX_SWITCH.SWITCH_RIGHT:
                break;
            case SBOX_SWITCH.SWITCH_ROOT_SET:
                break;
            case SBOX_SWITCH.SWITCH_SET:
                break;
            case SBOX_SWITCH.SWITCH_DOOR_SWITCH:
                break;
            case SBOX_SWITCH.SWITCH_PAYOUT:
                break;
            case SBOX_SWITCH.SWITCH_ENTER:
                MachineSelectManager.Instance.BtnSpinUp();
                //MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData<int>(MachineEventDefine.ON_KEY_START, 0));
                break;
            case SBOX_SWITCH.SWITCH_ESC:
                break;
            case SBOX_SWITCH.SWITCH_SWITCH:
                break;
            case SBOX_SWITCH.SWITCH_SCORE_UP:
                break;
            case SBOX_SWITCH.SWITCH_SCORE_DOWN:
                break;
            case SBOX_SWITCH.SWITCH_RED:
                break;
            case SBOX_SWITCH.SWITCH_GREEN:
                break;
            case SBOX_SWITCH.SWITCH_YELLOW:
                break;
            case SBOX_SWITCH.SWITCH_BET4:
                break;
            case SBOX_SWITCH.SWITCH_BET5:
                break;
            case SBOX_SWITCH.SWITCH_AUTO:
                break;
            default:
                break;
        }
    }

    private void AddEventListener()
    {

        Debug.Log("【BillLst】AddEventListener");
        BlizzEvent.EventCenter.Instance.AddEventListener<int>(SBoxSanboxEventHandle.COIN_IN, OnCoinIn);
        BlizzEvent.EventCenter.Instance.AddEventListener<int>(SBoxSanboxEventHandle.COIN_OUT, OnCoinOut);
        BlizzEvent.EventCenter.Instance.AddEventListener<int>(SBoxSanboxEventHandle.BILL_IN, OnBillIn);
        BlizzEvent.EventCenter.Instance.AddEventListener(SBoxSanboxEventHandle.BILL_STACKED, OnBillStacked);

        Register(MachineEventDefine.ON_LIGHT_CHANGE, OnLightChange);

        //纸钞机
        BlizzEvent.EventCenter.Instance.AddEventListener<List<string>>(SBoxEventHandle.SBOX_SADNBOX_BILL_LIST_GET, OnSboxSandBoxBillListGet);
        BlizzEvent.EventCenter.Instance.AddEventListener<int>(SBoxEventHandle.SBOX_SADNBOX_BILL_SELECT, OnSboxSandBoxBillSelect);
        //打印机
        EventCenter.Instance.AddEventListener<List<string>>(SBoxEventHandle.SBOX_SADNBOX_PRINTER_LIST_GET, OnPrinterListGet);
        EventCenter.Instance.AddEventListener<int>(SBoxEventHandle.SBOX_SADNBOX_PRINTER_SELECT, OnPrinterSelect);
        EventCenter.Instance.AddEventListener<int>(SBoxEventHandle.SBOX_SADNBOX_PRINTER_RESET, OnPrinterReset);
        EventCenter.Instance.AddEventListener<int>(SBoxEventHandle.SBOX_SADNBOX_PRINTER_FONTSIZE, OnPrinterFontsize);
        EventCenter.Instance.AddEventListener<int>(SBoxEventHandle.SBOX_SADNBOX_PRINTER_MESSAGE, OnPrinterMessage);
        EventCenter.Instance.AddEventListener<int>(SBoxEventHandle.SBOX_SADNBOX_PRINTER_DATESET, OnPrinterDateSet);
        EventCenter.Instance.AddEventListener<SBoxDate>(SBoxEventHandle.SBOX_SADNBOX_PRINTER_DATEGET, OnPrinterDateGet);
        EventCenter.Instance.AddEventListener<int>(SBoxEventHandle.SBOX_SADNBOX_PRINTER_PAPERCUT, OnPrinterCutPaper);



    }

    private void OnCoinIn(int coinCount)
    {
        Debug.LogError("CoinIn");
        StartCoroutine(PurchaseCreditRequest(1, coinCount));
    }

    public IEnumerator PurchaseCreditRequest(UInt32 operateType, long purchase)
    {
        bool requestSuccess = false;
        bool requestFail = false;

#if NEW_NET

        string rpcName = operateType == 1 ? RPCName.agentRechargeToDeviceUser : RPCName.decreaseDeviceCredit;
        //operateType == 1 ? "/v0/purchase/add_credit" : "/v0/purchase/sub_credit";

        Dictionary<string, object> req = new Dictionary<string, object>
            {
                {"balance",purchase},
            };

        NetManager.Instance.Post(rpcName, req,
        (res) =>
        {

            requestSuccess = true;
            globalStore.newCredit = res["balance"].AsLong;
            BlackboardQueryUtils.SetMyCredit(globalStore.newCredit);
            MessageDispatcher.Dispatch("OnCreditEvent", new EventData<bool>("UpdateNaviCredit", true));
        },
        (error) =>
        {
            switch (error.errorCode)
            {
                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }

            requestFail = true;
        });
#else
            BagelCodeClientAPI.PurchaseCreditRequest(operateType, purchase,
                (response) =>
                {
                    requestSuccess = true;
                    if (response.userSyncInfo != null)
                    {
                        BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                        BlackboardQueryUtils.ApplyUserSyncInfo();
                    }
                },
                (error) =>
                {
                    switch (error.errorCode)
                    {
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }

                    requestFail = true;
                });
#endif

        yield return new WaitUntil(() => requestSuccess || requestFail);
    }

    private void OnCoinOut(int coinCount)
    {
        if (coinOutNum - coinCount < 0)
            coinCount = coinOutNum;
        coinOutNum -= coinCount;
        StartCoroutine(PurchaseCreditRequest(2, coinCount * 100));
        if (coinOutNum == 0)
            CoinOutStop(0);
    }

    private void OnLightChange(EventData data)
    {
        /*switch (data.value)
        {
            case "lobby":
                //开灯
                SwitchOutStateOn((ulong)SBOX_SWITCH.SWITCH_UP);
                SwitchOutStateOn((ulong)SBOX_SWITCH.SWITCH_RED);
                SwitchOutStateOn((ulong)SBOX_SWITCH.SWITCH_GREEN);
                SwitchOutStateOn((ulong)SBOX_SWITCH.SWITCH_YELLOW);
                SwitchOutStateOn((ulong)SBOX_SWITCH.SWITCH_BET4);
                SwitchOutStateOn((ulong)SBOX_SWITCH.SWITCH_BET5);
                SwitchOutStateOn((ulong)SBOX_SWITCH.SWITCH_ENTER);
                SwitchOutStateOn((ulong)SBOX_SWITCH.SWITCH_AUTO);
                break;
            case "slots":
                //关灯
                SwitchOutStateOff((ulong)SBOX_SWITCH.SWITCH_UP);
                break;
            default:
                break;
        }*/
    }



    /// <summary>
    /// 纸钞机
    /// </summary>

    public void GetBillLst()
    {
#if UNITY_EDITOR
        MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_BILL_LIST_GET);
#else
        SBoxSandbox.BillListGet();
#endif
    }


    public List<string> sandBoxBillList;
    private void OnSboxSandBoxBillListGet(List<string> sandBoxBillList)
    {
        this.sandBoxBillList = sandBoxBillList;

        int i = 0;
        foreach (var item in sandBoxBillList)
        {
            Debug.Log($"【BillLst】sandBoxBillList: idx = {i}  val = {item}");
            i++;
        }

        if (this._taskTimer != null)
        {
            this._taskTimer.Stop();
            this._taskTimer.Dispose();
            this._taskTimer = null;
        }

        test_SetBillSelect();
    }

    [Button]
    public void test_SetBillSelect()
    {
        // 本地读取
        int data = 3; //存本地
#if UNITY_EDITOR
        MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_BILL_SELECT, data.ToString());
#else
        SBoxSandbox.BillSelect(data);
#endif
    }

    private void OnSboxSandBoxBillSelect(int res)
    {
        if (res == 0) //定时
        {
            //存本地
        }

        Debug.Log($"【BillLst】OnSboxSandBoxBillSelect  res = {res}");
    }



    int credit = 0;
    private void OnBillIn(int credit)
    {
        this.credit = credit;

        Debug.Log($"【BillLst】OnBillIn  credit = {credit}");

        // 发服务器确定接不接收
        if (true)
        {
#if UNITY_EDITOR
            MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_BILL_APPROVE);
#else
            SBoxSandbox.BillApprove();
#endif
        }
        else
        {
#if UNITY_EDITOR
            credit = 0;
            MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_BILL_REJECT);
#else
            SBoxSandbox.BillReject();
#endif

        }
    }


    private void OnBillStacked()
    {

        // 发订单号
        if (credit != 0)
        {
            Debug.Log($"【BillLst】OnBillStacked  credit = {credit}  x 1000");
            MachineSelectManager.Instance.PurchaseCreditRequest(1, credit * 1000);
            credit = 0;
        }
    }





    /// <summary>
    /// 打印机
    /// </summary>

    public void GetPrintList()
    {
#if UNITY_EDITOR
        MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_PRINTER_LIST_GET);
#else
        SBoxSandbox.PrinterListGet();
#endif
    }

    void OnPrinterListGet(List<string> strList)
    {
        if (this._taskTimer != null)
        {
            this._taskTimer.Stop();
            this._taskTimer.Dispose();
            this._taskTimer = null;
        }

        strList.ForEach(str => Debug.Log(str));
        int data = 0;
#if UNITY_EDITOR
        MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_PRINTER_SELECT, data.ToString());
#else
        SBoxSandbox.PrinterSelect(data);
#endif
    }

    void OnPrinterSelect(int result)
    {
        if (result == 0)
        {
            Debug.Log("【printer】: : select succeed");

#if UNITY_EDITOR
            MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_PRINTER_RESET);
#else
        SBoxSandbox.PrinterReset();
#endif

        }
        else
        {
            Debug.LogWarning($"【printer】: : 打印机选择失败  index = 0 ");
        }
    }

    void OnPrinterReset(int result)
    {
        if (result == 0)
        {
            Debug.Log("【printer】: : reset succeed");
            int fontSize = 5;
#if UNITY_EDITOR
            MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_PRINTER_FONTSIZE, fontSize.ToString());
#else
            SBoxSandbox.PrinterFontSize(fontSize);
#endif
        }
        else
        {
            Debug.LogWarning("【printer】: : 打印机复位失败");
        }
    }

    void OnPrinterFontsize(int result)
    {
        if (result == 0)
        {
            Debug.Log("【printer】: setFontSize succeed");
            SBoxDate sBoxDate = new SBoxDate()
            {
                result = 0,
                month = DateTime.Now.Month,
                day = DateTime.Now.Day,
                hours = DateTime.Now.Hour,
                minutes = DateTime.Now.Minute,
                seconds = DateTime.Now.Second
            };

#if UNITY_EDITOR

            MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_PRINTER_DATESET, JsonConvert.SerializeObject(sBoxDate));
#else
            SBoxSandbox.PrinterDateSet(sBoxDate);
#endif
        }
        else
        {
            Debug.LogWarning("【printer】: 打印机字体设置失败");
        }
    }
    void OnPrinterDateSet(int result)
    {
        if (result == 0)
        {
            Debug.Log("【printer】: set Date succeed");
        }
    }



    [Button]
    public void test_PrinterCutPaper(int result)
    {
#if UNITY_EDITOR
        MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_PRINTER_PAPERCUT);
#else
        SBoxSandbox.PrinterPaperCut();
#endif
    }

    void OnPrinterCutPaper(int result)
    {
        
        if (result == 0)
        {
            Debug.Log("【printer】: cut paper succeed");
        }
    }

    [Button]
    public void test_PrinterDateGet(SBoxDate sBoxDate)
    {
#if UNITY_EDITOR
        MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_PRINTER_DATEGET);
#else
        SBoxSandbox.PrinterDateGet();
#endif
    }


    void OnPrinterDateGet(SBoxDate sBoxDate)
    {
        if (sBoxDate.result == 0)
        {
        }
    }



    [Button]
    public void test_PrinterMessage(int credit = 10000)
    {
        //Debug.Log($"【printer】: All dollar = {BlackboardUtils.FindVariable<long>(null, "/me/credit").value / 1000}");
        int dollar = credit/1000;
        string testMsg = "K3K\r\n" +
            $"${dollar}\r\n" +
            $"Order number: {123456}\r\n" +
            $"Distributor: {"aa"}\r\n" +
            $"Business: {"bb"}\r\n";
#if UNITY_EDITOR

        MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_PRINTER_MESSAGE, testMsg);
#else
        SBoxSandbox.PrinterMessage(testMsg);
#endif

    }
    void OnPrinterMessage(int result)
    {
        if (result == 0)
        {
            Debug.Log("【printer】:print succeed");  
        }
    }



}
