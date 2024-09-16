
/**
 * 
# 去掉大厅底部分类选框（直接把框体关闭）

# 所有Spin关掉 选框(默认不打开)

# 所有子游戏选区和成一个选区，并直接用switch进行修改，并且选择。

# 游戏选择框，直接闪烁按钮进行选择

# 不要keno和jacks

# 把左右选择做到 加注减注

# 加按钮闪烁
*/

using BagelCode;
using BagelCode.OSA_Scroll;
//using Boo.Lang;
using ParadoxNotion;
using Sirenix.OdinInspector;
using SlotMaker;
using SlotMaker.Cards;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum SceneBtnType
{
    None,

    CommonOk,
    /// <summary> </summary>
    SysSetting,

    Menu,

    Help,

    EnterOrLeaveGame,

    Pop,


    MiniGameSelect,

    MiniGame,

    /// <summary>大厅节点 </summary>
    Hall,
    /// <summary>游戏节点 </summary>
    Game,
}



public enum MarkType
{
    /// <summary>大厅节点 </summary>
    Hall,
    /// <summary>游戏节点 </summary>
    Game,
    /// <summary>弹窗 </summary>
    Pop,
    /// <summary>下拉框 </summary>
    Frame,
}

public class MarkInfo
{
    public string name;
    public int index;

    public MarkType type;
    public MarkInfo(string name, int index, MarkType type)
    {
        this.name = name;
        this.index = index;
        this.type = type;
    }
}

class LightBtnTarget
{
    public string nodePath;
    public int btnNum;
    public LightBtnTarget(string nodePath, int btnNum)
    {
        this.nodePath = nodePath;
        this.btnNum = btnNum;
    }
}

public partial class MachineSelectManager : MonoSingleton<MachineSelectManager>
{

    [Button]
    void test_GetParm()
    {
        Debug.Log($"【MachineSelectBorder】 IsSpin ={BlackboardQueryUtils.IsSpin()}");
    }

    static string MACHINE_BTN_EVENT = "MachineBtnEvent";

    private void Start()
    {

#if !UNITY_EDITOR
        if (!ApplicationSettings.Instance.isMachine)
            return; 
#endif

        MessageDispatcher.Register(MetaEventDefine.ON_META_UI_EVENT, OnChangeGameListShowMode);

        MessageDispatcher.Register(MACHINE_BTN_EVENT, OnMachineBtnEvent);  //"ShowLightSelectTip"


        ResetSceneBtn();
    }

    [Button]
    public void ResetSceneBtn()
    {
        StartCoroutine(_ForceSetBtn());
    }
    private IEnumerator _ForceSetBtn()
    {
        isRuning = true;
        yield return new WaitForSeconds(1);
        OpenAllSceneBtn();
        // yield return new WaitForSeconds(2);
        // CloseAllSceneBtn();
        yield return new WaitForSeconds(2);
        Debug.Log("强制刷新按钮");
        lastSceneBtnType = SceneBtnType.None;
        ReflashSceneBtn();
        isRuning = false;
    }

    [Button]
    public void CloseAllBtn()
    {
        StartCoroutine(_ForceSetBtn9());
    }
    private IEnumerator _ForceSetBtn9()
    {
        isRuning = true;
        yield return new WaitForSeconds(3);
        CloseAllSceneBtn();
    }

    protected override void OnDestroy()
    {
        if (this._taskTimer != null)
        {
            this._taskTimer.Stop();
            this._taskTimer.Dispose();
            this._taskTimer = null;
        }

        MessageDispatcher.UnRegister(MetaEventDefine.ON_META_UI_EVENT, OnChangeGameListShowMode);

        MessageDispatcher.UnRegister(MACHINE_BTN_EVENT, OnMachineBtnEvent);  //"ShowLightSelectTip"

        base.OnDestroy();
    }

    private Queue<Action> taskQueue = new Queue<Action>();

    private void OnMachineBtnEvent(ParadoxNotion.EventData eventData)
    {
        //"LightBtnSelectShowTip"
        //LightBtnSelect

        if (eventData.name != "LightBtnSelect") return;

        task = () =>
        {
            ConfirmLightBtnSelectInGame((int)eventData.value);
        };
    }

    private void OnChangeGameListShowMode(ParadoxNotion.EventData eventData)
    {
        if (eventData.name != "OnChangeGameListShowMode") return;

        task = () =>  //延时，避免   OSA_LobbySlots.ResetCurSelect() 影响
        {
            ReflashHallBtnRegion();
        };
    }

    System.Action task;

    bool isRuning = false;

    long lastTime = 0;

    //200ms : updateNum = 11
    //500ms : updateNum = 29
    //1000ms : updateNum = 29

    int lastGameID = -99;
    private void Update()
    {
#if !UNITY_EDITOR
        if (!ApplicationSettings.Instance.isMachine)
            return;
#endif

        if (!isRuning)
        {
            isRuning = true;
            long nowTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (task != null)
            {
                task();
                task = null;
            }
            if (nowTime - lastTime > 500)
            {
                lastTime = nowTime;

                ReflashSceneBtn();
                ChangeSpinLightInGame();

            }
            isRuning = false;
        }
    }

    //DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
    bool isSpinLight = true;
    SpinButton SpinBtn = null;
    private void ChangeSpinLightInGame()
    {

        if (globalStore.nowGameID == -1)
            return;

        bool _isSpinLight = false;

        if (SpinBtn == null)
            SpinBtn = GameObject.Find("In Game/Anchor/In Game Bottom/Anchor/Layout/Button Spin")?.GetComponent<SpinButton>();

        if (SpinBtn == null)
        {
            _isSpinLight = true;
        }
        else
        {
            switch (SpinBtn.GetSpinButtonState())
            {
                case NextSpinState.None:
                case NextSpinState.ToPlay:
                case NextSpinState.ToStopAuto:
                    _isSpinLight = true;
                    break;
                case NextSpinState.ToStop:
                    //case NextSpinState.ToStopAuto:
                    _isSpinLight = false;
                    break;
            }
        }


        if (isSpinLight != _isSpinLight)
        {
            isSpinLight = _isSpinLight;
            if (isSpinLight)
            {
                MessageDispatcher.Dispatch(MACHINE_BTN_EVENT, new EventData("LightBtnOpenSpin"));
            }
            else
            {
                MessageDispatcher.Dispatch(MACHINE_BTN_EVENT, new EventData("LightBtnCloseSpin"));
            }
        }
    }

    protected System.Timers.Timer _taskTimer = null;

    void _AddTack(System.Action tk, int time = 0)
    {
        if (time > 0)
        {
            if (this._taskTimer != null)
            {
                this._taskTimer.Stop();
                this._taskTimer.Dispose();
                this._taskTimer = null;
            }
            this._taskTimer = new System.Timers.Timer(time);
            this._taskTimer.AutoReset = false; // 是否重复执行
            this._taskTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                task = tk;
            };
            //this._keepAliveTimer.Enabled = true; //开始执行
            this._taskTimer.Start();
        }
        else
        {
            task = tk;
        }
    }

    /*private static MachineSelectManager instance;
    public static MachineSelectManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MachineSelectManager();
            }
            return instance;
        }
    }
    */

    int _curSelectNumb = 0;

    public string _curSelectMark = "";

    // A = oldHallMark or  oldGameMark
    // A -> B -> C -> D 
    // A -> B-> D

    /// <summary>大厅mark堆栈 </summary>
    List<MarkInfo> hallMarkStack = new List<MarkInfo> { new MarkInfo("", 0, MarkType.Hall) };

    /// <summary>游戏mark堆栈 </summary>
    List<MarkInfo> gameMarkStack = new List<MarkInfo> { new MarkInfo("", 0, MarkType.Game) };

    [Button]
    void test_ShowInfo()
    {
        Debug.Log($"【machine select mgr】 _curSelectNumb = {_curSelectNumb} _curSelectMark = {_curSelectMark}  hc = {hallMarkStack.Count} gc = {gameMarkStack.Count}");
    }

    private void setMark(int index)
    {
        List<MarkInfo> markStack;

        if (globalStore.nowGameID == -1)
        {
            markStack = hallMarkStack;
        }
        else
        {
            markStack = gameMarkStack;
        }

        markStack[markStack.Count - 1].index = index;
    }

    private void setMark(string mark, int index)
    {
        List<MarkInfo> markStack;

        if (globalStore.nowGameID == -1)
        {
            markStack = hallMarkStack;
        }
        else
        {
            markStack = gameMarkStack;
        }
        markStack[markStack.Count - 1].name = mark;
        markStack[markStack.Count - 1].index = index;
    }

    private void AddMark(string name, int index, MarkType type)
    {
        List<MarkInfo> markStack;

        if (globalStore.nowGameID == -1)
            markStack = hallMarkStack;
        else
            markStack = gameMarkStack;
        markStack.Add(new MarkInfo(name, index, type));
    }


    private void ResetMarkStack()
    {
        if (globalStore.nowGameID == -1) //在大厅时复位游戏 mark
            gameMarkStack = new List<MarkInfo> { new MarkInfo("", 0, MarkType.Game) };
        else //在游戏时复位大厅 mark
            hallMarkStack = new List<MarkInfo> { hallMarkStack[0] };
    }

    private void ClearMark()
    {
        List<string> markLst = new List<string>();

        MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();

        for (int i = 0; i < comps.Length; i++)
        {
            if (comps[i].transform.parent.gameObject.activeSelf && !markLst.Contains(comps[i].mark))
                markLst.Add(comps[i].mark);
        }

        List<MarkInfo> markStack;

        if (globalStore.nowGameID == -1)
        {
            markStack = hallMarkStack;
        }
        else
        {
            markStack = gameMarkStack;
        }


        int j = 0;
        while (j < markStack.Count)
        {
            if ((markStack[j].name == null) || (markStack[j].name != "" && !markLst.Contains(markStack[j].name)))
                markStack.RemoveAt(j);
            else
                j++;
        }

        ResetMarkStack();
    }

    private void RemoveMark(string removeMark)
    {

        ClearMark();

        List<MarkInfo> markStack;

        if (globalStore.nowGameID == -1)
            markStack = hallMarkStack;
        else
            markStack = gameMarkStack;

        if (markStack.Count > 1 && markStack[markStack.Count - 1].name == removeMark)
            markStack.RemoveAt(markStack.Count - 1);

        MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();
        MarkInfo mk = markStack[markStack.Count - 1];

        for (int i = 0; i < comps.Length; i++)
        {
            if (comps[i].index == mk.index && comps[i].mark == mk.name)
                comps[i].isSelected = true;
            else
                comps[i].isSelected = false;
        }
        _curSelectMark = mk.name;
        _curSelectNumb = mk.index;

    }

    public bool isInitHallBtnRegion = false;
    public bool isInitGameBtnRegion = false;

    /// <summary>
    /// * 每次加载大厅后，调用。调整显示框。
    /// * 在大厅显示区域变化时，也可以调用。调整显示框。
    ///
    /// </summary>
    public void ReflashHallBtnRegion()
    {
        //Debug.Log($"@@ _curSelectNumb 01 = {_curSelectNumb } _curSelectMark = {_curSelectMark}");
        /**/
        if (globalStore.nowGameID != -1)
        {
            return;
        }
        ResetMarkStack();

        isInitHallBtnRegion = true;
        isInitGameBtnRegion = false;

        /*if (hallMarkStack.Count != 1)
        {
            _curSelectNumb = hallMarkStack[hallMarkStack.Count - 1].index;
            _curSelectMark = hallMarkStack[hallMarkStack.Count - 1].name;
        }*/

        /*
        * 在子游戏返回大厅时， _curSelectNumb被设置得过大,返回大厅时找不到对应的index对象;
        * 或者_curSelectMark没有被重置。
        */

        if (hallMarkStack.Count == 1)
        {
            List<string> marks = GetVisableButtonRegionLst();

            if (!marks.Contains(hallMarkStack[0].name))
            {
                hallMarkStack[0].name = marks[0];
                hallMarkStack[0].index = 0;
            }
            if (hallMarkStack.Count == 1)
            {
                _curSelectMark = hallMarkStack[0].name;
                _curSelectNumb = hallMarkStack[0].index;
            }

            MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();
            foreach (var item in comps)
            {
                if (marks.Contains(item.mark))
                {
                    if (item.index == hallMarkStack[0].index && item.mark == hallMarkStack[0].name)
                        item.isSelected = true;
                    else
                        item.isSelected = false;
                }
            }
            GameObject Base = GameObject.Find("Lobby/Anchor/Lobby Pages/Anchor/Slots Area/ScrollView Slots");
            if (Base != null && Base.activeSelf)
            {
                if (hallMarkStack[0].name == "")
                    Base.GetComponent<OSA_LobbySlots>().SetCurSelect(hallMarkStack[0].index);
                else
                    Base.GetComponent<OSA_LobbySlots>().SetCurSelect(-1);
            }


            /*======================
            _curSelectNumb = hallMarkStack[0].index;
            _curSelectMark = hallMarkStack[0].name;

            Debug.Log($"@@ _curSelectNumb 02 = {_curSelectNumb} _curSelectMark = {_curSelectMark}");

            GameObject Base = GameObject.Find("Lobby/Anchor/Lobby Pages/Anchor/Slots Area/ScrollView Slots");

            if (Base != null && Base.active)
            {
                if (_curSelectMark == "")
                {
                    Base.GetComponent<OSA_LobbySlots>().SetCurSelect(_curSelectNumb);
                }
                else
                {
                    Base.GetComponent<OSA_LobbySlots>().SetCurSelect(-1);
                }
            }
            MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();

            foreach (var item in comps)
            {
                if (item.index == _curSelectNumb && item.mark == _curSelectMark)
                {
                    //item.selectBorder.SetActive(true);
                    item.isSelected = true;
                }
                else
                {
                    //item.selectBorder.SetActive(false);
                    item.isSelected = false;
                }
            }*/
        }

    }

    public void ReflashGameBtnRegion()
    {
        if (globalStore.nowGameID == -1)
        {
            return;
        }
        ResetMarkStack();
        isInitHallBtnRegion = false;
        isInitGameBtnRegion = true;



        List<string> marks = GetVisableButtonRegionLst();

        if (marks != null)
        {
            if (!marks.Contains(gameMarkStack[0].name))
            {
                gameMarkStack[0].name = marks[0];
                gameMarkStack[0].index = 0;
            }
            if (gameMarkStack.Count == 1)
            {
                _curSelectMark = gameMarkStack[0].name;
                _curSelectNumb = gameMarkStack[0].index;
            }

            MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();
            foreach (var item in comps)
            {
                if (marks.Contains(item.mark))
                {
                    if (item.index == gameMarkStack[0].index && item.mark == gameMarkStack[0].name)
                        item.isSelected = true;
                    else
                        item.isSelected = false;
                }
            }
        }

    }

    /// <summary>“游戏配置弹窗”是否可见 </summary>
    public bool isPopGameConfigSelect()
    {

        if (globalStore.nowGameID == 142) //FLAME_IN_JUNGLE 选游戏难度
        {
            GameObject Pick = GameObject.Find("Popup Manager/Contents/Denomination Popup FIJ");

            if (Pick != null && Pick.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    public void ConfirmPopGameConfigSelect(int selectNumb)
    {
        _curSelectNumb = selectNumb;

        sound = null;
        if (globalStore.nowGameID == 142) //FLAME_IN_JUNGLE  
        {
            //GameObject Pick = GameObject.Find("Popup Manager/Contents/Denomination Popup FIJ");

            GameObject choosePannel = GameObject.Find($"Denomination Popup FIJ/Animator/Anchor/Popup Base/Choose pannel {_curSelectNumb + 1}");


            if (choosePannel != null)
            {
                choosePannel?.GetComponent<SlotMaker.Extentions.ActionListPlayer>().Play();
            }
        }

        Debug.Log($"【machine】: game config {_curSelectNumb}");

    }

    public int getPopGameConfigSelectBtnNum()
    {
        if (globalStore.nowGameID == 142) //FLAME_IN_JUNGLE  游戏选难度
            return 3;

        return -1;
    }
    public int getPopFreeGameTimeSelectBtnNum()
    {
        if (globalStore.nowGameID == 37) //GOLDEN_PICTURES 
            return 3;

        if (globalStore.nowGameID == 93) //HAPPY_DOLLARS  
            return 5;

        if (globalStore.nowGameID == 105) //SUNSET_SAFARI 狮子-斑马
            return 2;

        if (globalStore.nowGameID == 116) //魔术师 - 选牌
            return 2;

        if (globalStore.nowGameID == 149) //白虎 - 选免费游戏
            return 4;

        return -1;
    }

    /// <summary>“免费游戏选择弹窗”是否可见 </summary>
    /// <remarks>
    /// 有些游戏进入免费游戏前会存在选择弹窗，将代码加入这里，用于断线重连和机台按钮的逻辑
    /// </remarks>
    public bool isPopFreeGameTimeSelect()
    {

        if (globalStore.nowGameID == 37) //GOLDEN_PICTURES 
        {
            GameObject Pick = GameObject.Find("Game Canvas/Game Contents/Animator/Anchor/Effect Midground/Pick Bonus/Animator/Base/Base/Pick");

            if (Pick != null && Pick.activeSelf)
            {
                return true;
            }
        }

        if (globalStore.nowGameID == 93) //HAPPY_DOLLARS 
        {
            GameObject Pick = GameObject.Find("Popup Manager/Contents/Free Game Select Popup");
            if (Pick != null && Pick.activeSelf)
            {
                return true;
            }
        }

        if (globalStore.nowGameID == 105) //SUNSET_SAFARI 狮子-斑马
        {
            GameObject Pick = GameObject.Find("Popup Manager/Contents/Select a Feature Popup");
            if (Pick != null && Pick.activeSelf)
            {
                return true;
            }
        }

        if (globalStore.nowGameID == 116) //魔术师 - 选牌
        {
            GameObject Pick = GameObject.Find("Popup Manager/Contents/Bonus Trigger Popup");
            if (Pick != null && Pick.activeSelf)
            {
                return true;
            }
        }

        if (globalStore.nowGameID == 149) //白虎 - 选免费游戏
        {
            GameObject Pick = GameObject.Find("Popup Manager/Contents/Free Game Select Popup");
            if (Pick != null && Pick.activeSelf)
            {
                return true;
            }
        }
        if (globalStore.nowGameID == 142)///丛林火焰 --选择额外押注
        {
            GameObject Pick = GameObject.Find("Popup Manager/Contents/Denomination Popup FIJ");
            if (Pick != null && Pick.activeSelf)
            {
                return true;
            }
        }
        if (globalStore.nowGameID == 10)  ////幸运财富
        {
            GameObject Pick = GameObject.Find("Game Contents/Animator/Anchor/Effect Midground/Free Spins Select");
            if (Pick != null && Pick.activeSelf)
            {
                return true;
            }
        }

        //神秘宝石这款游戏的免费游戏弹窗最开始是需要点击符号，然后才会有弹窗
        if (globalStore.nowGameID == 73)
        {
            //散射是否可以被点击
            var scatterClickable = BlackboardUtils.FindVariable<bool>("./scatterClickable");
            if (scatterClickable != null && scatterClickable.value == true)
                return true;

            //弹窗是否存在
            var panel = GameObject.Find("Popup Manager/Contents/Free Game Trigger Popup");
            if (panel != null && panel.activeSelf)
                return true;
        }
        //else if (globalStore.nowGameID == 62)
        //{
        //    var panel = GameObject.Find("Popup Manager/Contents/Choose Your Bet Level Popup");
        //    if (panel != null && panel.activeSelf)
        //        return true;
        //}
        else if (globalStore.nowGameID == 54)
        {
            var panel = GameObject.Find("Popup Manager/Contents/Choose Your Bonus Popup");
            if (panel != null && panel.activeSelf)
                return true;

            var panel2 = GameObject.Find("Popup Manager/Contents/Free Game Trigger Popup");
            if (panel2 != null && panel2.activeSelf)
                return true;
        }
        else if (globalStore.nowGameID == 35)
        {
            var panel = GameObject.Find("Game Canvas/Game Contents/Animator/Anchor/Effect Midground/Pick Bonus");
            if (panel != null && panel.activeSelf)
                return true;
        }
        else if (globalStore.nowGameID == 128)
        {
            var panel = GameObject.Find("Popup Manager/Contents/Free Game Trigger Popup");
            if (panel != null && panel.activeSelf)
                return true;
        }
        else if (globalStore.nowGameID == 99)
        {
            var panel = GameObject.Find("Popup Manager/Contents/Free Game Trigger Popup");
            if (panel != null && panel.activeSelf)
                return true;
        }
        else if (globalStore.nowGameID == 83)
        {
            var panel = GameObject.Find("Popup Manager/Contents/Choose Your Bet Level Popup");
            if (panel != null && panel.activeSelf)
                return true;
        }
        else if (globalStore.nowGameID == 62)
        {
            var panel = GameObject.Find("Popup Manager/Contents/Choose Your Bet Level Popup");
            if (panel != null && panel.activeSelf)
                return true;
        }
        else if (globalStore.nowGameID == 31)
        {
            var panel = GameObject.Find("Game Canvas/Game Contents/Animator/Anchor/Effect Midground/Wheel Bonus");
            if (panel != null && panel.activeSelf)
                return true;

            var panel1 = GameObject.Find("Game Canvas/Game Contents/Animator/Anchor/Effect Midground/Wheel Result");
            if (panel1 != null && panel1.activeSelf)
                return true;
        }

        return false;
    }

    public void ConfirmPopFreeGameSelect(int selectNumb)
    {
        _curSelectNumb = selectNumb;
        sound = null;
        string name = "";
        if (globalStore.nowGameID == 37)
        {
            switch (_curSelectNumb)
            {
                case 0:
                    name = "OnClick1";
                    break;
                case 1:
                    name = "OnClick2";
                    break;
                case 2:
                    name = "OnClick3";
                    break;
            }
            //Debug.Log($"EVT = OnSelection{_curSelectNumb}");
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData(name));

            //MessageDispatcher.Dispatch(EVTType.ON_MACHINE_BUTTON_EVENT, new EventData<string>($"PopFreeGameSelect/{btnName}", $"{_curSelectMark}/{_curSelectNumb}"));

            _curSelectNumb = 0;
        }
        if (globalStore.nowGameID == 93)
        {
            name = $"OnSelection{_curSelectNumb}";
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData(name));
            _curSelectNumb = 0;
        }


        if (globalStore.nowGameID == 105)
        {
            switch (_curSelectNumb)
            {
                case 0:
                    name = "SelectFreeSpin";
                    break;
                case 1:
                    name = "SelectLinkBonus";
                    break;
            }
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData(name));
            _curSelectNumb = 0;
        }


        if (globalStore.nowGameID == 116) //魔术师 - 选牌
        {
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData<int>("MachineSelectEvent", _curSelectNumb));
            _curSelectNumb = 0;
        }

        if (globalStore.nowGameID == 149) //白虎 
        {

            name = $"OnSelection{_curSelectNumb}";
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData(name));
            _curSelectNumb = 0;
        }

        Debug.Log($"【machine】: free game select {name}    {_curSelectNumb}");

    }

    /// <summary>
    /// * BigWin、SuperWin ...
    /// * free game start pop 、 free game result pop
    /// * mini game start pop 、mini game result pop
    ///
    ///
    /// </summary>
    public bool isPopCommon()
    {
        return PopupManager.Instance.popupCount > 0 || PopupManager.Instance.Exist();
    }

    public void ConfirmPopCommon()
    {
        Debug.Log($"【machine】: Common Popup");

        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Return"));
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Finalize"));

        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Collect"));
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnClose"));
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnCollect"));
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnPointerClick"));
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnStartBonusGame"));

        /**
         * Big Win Text Event Mega Win
         * Big Win Text Event Super Mega Win
         * Big Win Text Event Big Win
         * id-153-MULTIPLIER_MAN  "Free Game Trigger Popup"
         * id-153-MULTIPLIER_MAN  "Quick Change Trigger Popupp"
         */
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("MachineSpinClick"));

        // free game start
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnStartFreeSpin")); // 多数游戏免费游戏的开始提示弹窗

        //ID:21 mini game result
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("collectEvent")); //

        //ID:39  mini game start
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnStartBigWheel"));//Big Wheel Trigger Popup

        //ID:93 mini game result
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Clicked")); //Free Game Select Popup（免费游戏结算确认界面）

        //ID:100 - 旋转闪电战  SPIN BLITZ
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("StartClosePopup"));
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("ClosedPopup"));

        //ID:107 - 旋转闪电战  SPIN BLITZ
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnClick"));

        //ID:103 -  奖励弹窗
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnTicketRegularWin")); // 奖励弹窗
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnBonusGameStart"));  // mini game start
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnBackClicked"));  // mini game result

        //ID:122 -  甜心赢了
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("GiftLeftClicked")); //选择窗,暂时这样跳过重连

        //ID:132 - 奢侈精品店  LUXURY_BOUTIQUE
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnClickStartButton"));
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnClickCollectButton"));
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnClickExtraSale"));

        //ID:142  mini game result
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnFinishBonus")); //Double Jackpot Major Popup FIJ（免费游戏结算确认界面）

        //ID:149 - 白虎  mini game result，幸运符免费游戏
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnButtonClicked")); //Double Jackpot Major Popup FIJ（免费游戏开始界面）

        //ID:144 - 狮子  mini game result
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnClickButton")); //免费游戏开始界面、免费游戏结算界面

        //ID:171 - 恶魔之心  HEART_OF_DEMONESS
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnStartReSpin")); //免费游戏开始界面、免费游戏结算界面

        //ID:151，戈斯银行 ，免费游戏选择界面，免费游戏确认界面，MR_GOOSES_BANK
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnClickEventLeft"));
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnClickEvent"));

        //IDD：149 魔术师
        EventSender.SendGlobalEvent(EVTType.ON_CONTENT_UI_EVENT, new EventData("BonusCardClicked"));
        ///152  猫咪抢劫案
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnStartJackpot"));

        ///130 金星珍珠
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Click"));
    }

    /// <summary>“小游戏选择弹窗”是否可见 </summary>
    public bool isPopMiniGameSelect()
    {

        if (globalStore.nowGameID == 100003) //Jack
        {

            GameObject Base = GameObject.Find("Popup Manager/Contents/Red Or Black");

            if (Base != null && Base.activeSelf)
            {
                if (_curSelectMark != "RedOrBlack")  //首次打开进行赋值
                {
                    _curSelectNumb = 0;
                    _curSelectMark = "RedOrBlack";
                    AddMark(_curSelectMark, _curSelectNumb, MarkType.Pop);
                }
                return true;
            }
        }
        if (_curSelectMark == "RedOrBlack")
        {
            RemoveMark("RedOrBlack");
        }
        return false;

    }

    public void ConfirmPopMiniGameSelect()
    {
        string name = "";
        if (globalStore.nowGameID == 100003) // Jack
        {

            switch (_curSelectNumb)
            {
                case 0:
                    name = "Take";
                    break;
                case 1:
                    name = "Black";
                    break;
                case 2:
                    name = "Black Clover";
                    break;
                case 3:
                    name = "Black Spade";
                    break;
                case 4:
                    name = "Red Heart";
                    break;
                case 5:
                    name = "Red Diamond";
                    break;
                case 6:
                    name = "Red";
                    break;
            }

            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData(name));
        }

        //MessageDispatcher.Dispatch(EVTType.ON_MACHINE_BUTTON_EVENT, new EventData<string>($"PopMiniGameSelect/{btnName}", $"{_curSelectMark}/{_curSelectNumb}"));

        Debug.Log($"【machine】: min game pop select {name}");

    }

    /// <summary>“小游戏选择节点”是否可见 </summary>
    public bool isNodeMiniGameSelect()
    {

        if (globalStore.nowGameID == 92) //小猪
        {

            //检测有没弹窗：Popup Manager/Contents/Coin Pick Result Popup  或  Popup Manager/Contents/Jackpot Coin Total Win Popup
            /*
            GameObject Pop = GameObject.Find("Popup Manager/Contents/Coin Pick Result Popup");
            if (Pop != null)
                return false;
            Pop = GameObject.Find("Popup Manager/Contents/Jackpot Coin Total Win Popup");
            if (Pop != null)
                return false;*/

            if (PopupManager.Instance.popupCount > 0 || PopupManager.Instance.Exist())
                return false;

            GameObject Base = GameObject.Find("Game Contents/Animator/Anchor/Midground/Pick Bonus");

            if (Base != null && Base.active)
            {
                return true;
            }
        }


        if (globalStore.nowGameID == 116) //魔术师 - 魔术帽
        {

            //if (PopupManager.Instance.popupCount > 0 || PopupManager.Instance.Exist())
            //    return false;

            GameObject Base = GameObject.Find("Game Canvas/Game Contents/Animator/Anchor/Effect Midground/Bonus Game");

            if (Base != null && Base.active)
            {
                return true;
            }
        }

        if (globalStore.nowGameID == 103) // bego-星星
        {
            GameObject Base = GameObject.Find("Game Canvas/Game Contents/Animator/Anchor/Midground/Bonus game/Anchor/Picking elements");

            if (Base != null && Base.active)
            {
                return true;
            }
        }

        if (globalStore.nowGameID == 3000)
        {
            Dictionary<string, string> nodePath = new Dictionary<string, string>()
            {
                ["BSTEgg"] = "Effect Midground/Door Bonus/miniGame00",
                ["BSTTreasure"] = "Effect Midground/Door Bonus/miniGame01",
                ["BSTHarp"] = "Effect Midground/Door Bonus/miniGame02",
            };
            for (int i = 0; i < nodePath.Count; i++)
            {

                string key = nodePath.ElementAt(i).Key;
                GameObject Base = GameObject.Find(nodePath[key]);
                if (Base != null && Base.active)
                {
                    if (_curSelectMark != key)
                    {
                        _curSelectNumb = 0;
                        _curSelectMark = key;

                        AddMark(_curSelectMark, _curSelectNumb, MarkType.Game);
                    }
                    return true;
                }
            }
        }

        if (globalStore.nowGameID == 3001)
        {
            Dictionary<string, string> nodePath = new Dictionary<string, string>()
            {
                ["Game2"] = "Effect Midground/Mini Game 2",
                ["Game3"] = "Effect Midground/Mini Game 3",
                //["BSTHarp"] = "Effect Midground/Door Bonus/miniGame02",
            };
            for (int i = 0; i < nodePath.Count; i++)
            {

                string key = nodePath.ElementAt(i).Key;
                GameObject Base = GameObject.Find(nodePath[key]);
                if (Base != null && Base.active)
                {
                    if (_curSelectMark != key)
                    {
                        _curSelectNumb = 0;
                        _curSelectMark = key;

                        AddMark(_curSelectMark, _curSelectNumb, MarkType.Game);
                    }
                    return true;
                }
            }
        }
        if (globalStore.nowGameID == 3004)
        {
            Dictionary<string, string> nodePath = new Dictionary<string, string>()
            {
                ["HalloweenGame1"] = "Effect Midground/Game1",
                ["HalloweenGame2"] = "Effect Midground/Game2",
                ["HalloweenGame3"] = "Effect Midground/Game3",
                //["BSTHarp"] = "Effect Midground/Door Bonus/miniGame02",
            };
            for (int i = 0; i < nodePath.Count; i++)
            {
                string key = nodePath.ElementAt(i).Key;
                GameObject Base = GameObject.Find(nodePath[key]);
                if (Base != null && Base.active)
                {
                    if (_curSelectMark != key)
                    {
                        _curSelectNumb = 0;
                        _curSelectMark = key;

                        AddMark(_curSelectMark, _curSelectNumb, MarkType.Game);
                    }
                    return true;
                }
            }
        }

        if (globalStore.nowGameID == 3003)
        {
            Dictionary<string, string> nodePath = new Dictionary<string, string>()
            {
                ["FTVBPCards"] = "Effect Midground/Mini Game Bonus/Mini Game2/Animator/Cards",
            };
            for (int i = 0; i < nodePath.Count; i++)
            {

                string key = nodePath.ElementAt(i).Key;
                GameObject Base = GameObject.Find(nodePath[key]);
                if (Base != null && Base.active)
                {
                    if (_curSelectMark != key)
                    {
                        _curSelectNumb = 0;
                        _curSelectMark = key;

                        AddMark(_curSelectMark, _curSelectNumb, MarkType.Game);
                    }
                    return true;
                }
            }
        }

        List<string> marks = new List<string>()
        {
            "BSTEgg",
            "BSTTreasure",
            "BSTHarp",

            "Game2",
            "Game3",

            "HalloweenGame1",
            "HalloweenGame2",
            "HalloweenGame3",
        };
        if (marks.Contains(_curSelectMark))
        {
            RemoveMark(_curSelectMark);
        }

        return false;
    }

    public void ConfirmNodeMiniGameSelect(string btnName = "BtnSpin_DOWN")
    {
        sound = null;
        string name = "";
        if (globalStore.nowGameID == 92) // 小猪选金币（多选）
        {
            name = "MachineSelectEvent";
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData<int>(name, _curSelectNumb));
        }


        if (globalStore.nowGameID == 116) //魔术师 - 魔术帽（多选）
        {
            name = "MachineSelectEvent";
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData<int>(name, _curSelectNumb));
        }



        if (globalStore.nowGameID == 183) //辣椒 - 滚轮滑动界面
        {
            name = "OnBigWheelClick";
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData<int>(name, _curSelectNumb));
        }


        if (globalStore.nowGameID == 103) // bego-星星
        {
            name = "MachineSelectEvent";
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData<int>(name, _curSelectNumb));
        }

        if (globalStore.nowGameID == 3000) //beanstalk
        {
            //EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData<int>(name, _curSelectNumb));
            name = "MachineSelectEvent";
            MessageDispatcher.Dispatch(EVTType.MACHINE_BUTTON_SELECT_UI_EVTTYPE, new ParadoxNotion.EventData<int>(name, _curSelectNumb));
        }


        if (globalStore.nowGameID == 3001) //Fruit Party
        {
            name = "MachineSelectEvent";
            //EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData<int>(name, _curSelectNumb));
            MessageDispatcher.Dispatch(EVTType.MACHINE_BUTTON_SELECT_UI_EVTTYPE, new ParadoxNotion.EventData<int>(name, _curSelectNumb));
        }

        if (globalStore.nowGameID == 3004) //Fruit Party
        {
            name = "MachineSelectEvent";
            //EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData<int>(name, _curSelectNumb));
            MessageDispatcher.Dispatch(EVTType.MACHINE_BUTTON_SELECT_UI_EVTTYPE, new ParadoxNotion.EventData<int>(name, _curSelectNumb));
        }

        MessageDispatcher.Dispatch(EVTType.ON_MACHINE_BUTTON_EVENT, new EventData<string>($"NodeMiniGameSelect/{btnName}", $"{_curSelectMark}/{_curSelectNumb}"));

        Debug.Log($"【machine】: min game node select {name}");

    }

    /**
     * 排除Spin按钮
     */
    public bool isNodeGameButtonSelect()
    {
        if (globalStore.nowGameID == -1)  // 大厅
        {
            return false;
        }

        if (isPopCommon())
            return false;


        if ( //游戏中，不给配置
            BlackboardQueryUtils.IsIngame()
            && BlackboardQueryUtils.IsSpin()
            && BlackboardQueryUtils.IsAutoSpin()
        )
        {
            return false;
        }

        List<string> marks = GetButtonRegionLst();

        if (marks != null)
        {
            if (!marks.Contains(gameMarkStack[0].name))
            {
                gameMarkStack[0].name = marks[0]; // "SPIN"
                gameMarkStack[0].index = 0;

                if (gameMarkStack.Count == 1)
                {
                    _curSelectMark = gameMarkStack[0].name;
                    _curSelectNumb = gameMarkStack[0].index;
                }
            }
            return _curSelectMark != "SPIN"; //排除Spin按钮
        }

        /*
        bool isFind = false;
        foreach (var kv in buttonRegions)
        {
            if (kv.Key == -1)
            {
                continue;
            }
            if ((new List<string>(kv.Value).Contains(_curSelectMark)))
            {
                isFind = true;
            }
        }
        if (isFind)
        {
            gameMarkStack[0].name = "";
            gameMarkStack[0].index = 0;
        }*/

        return false;

    }

    public void ConfirmNodeGameButtonSelect()
    {

        string name = "";
        if (globalStore.nowGameID == 43)
        {
            switch (_curSelectNumb)
            {
                case 0:
                    sound = "VGN VO Allison BestSpins";
                    name = "buttonClick4";
                    break;
                case 1:
                    sound = "VGN VO Sasha Funky";
                    name = "buttonClick1";
                    break;
                case 2:
                    sound = "VGN VO Chloce BornToWin";
                    name = "buttonClick2";
                    break;
                case 3:
                    sound = "VGN VO Miranda ShowtimeSpinners";
                    name = "buttonClick3";
                    break;
            }

            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData(name));

        }


        if (globalStore.nowGameID == 200002)
        {
            MachineSelectBorder nowSelectComps = GetNowSelectComp();

            if (_curSelectMark == "Spot"
                && !BlackboardQueryUtils.IsSpin()
                && !BlackboardQueryUtils.IsAutoSpin()
            )
            {
                if (nowSelectComps != null)
                {
                    nowSelectComps.transform.parent.GetComponent<UnityEngine.UI.Button>().OnSubmit(null);
                    // nowSelectComps.transform.parent.GetComponent<Button>().onClick.Invoke();
                }
            }
            else if (_curSelectMark == "ButtonUI"
                && !BlackboardQueryUtils.IsSpin()
                && !BlackboardQueryUtils.IsAutoSpin()
            )
            {
                if (nowSelectComps != null)
                {
                    nowSelectComps.transform.parent.GetComponent<ContextButton>().DoClick();
                }
            }
            else if (_curSelectMark == "SideUI")
            {
                if (nowSelectComps != null)
                {
                    nowSelectComps.transform.parent.GetComponent<ContextButton>().DoClick();
                }
            }
        }


        if (globalStore.nowGameID == 100003)
        {
            MachineSelectBorder nowSelectComps = GetNowSelectComp();
            if (nowSelectComps != null)
            {
                if (_curSelectMark == "JacksCard")
                {
                    CardInstance ci = nowSelectComps.transform.parent.GetComponent<CardInstance>();
                    if (ci.clickable)
                    {
                        ci.OnClick();
                    }
                }
                else if (_curSelectMark == "JacksGamble")
                {
                    /* name = "GambleButtonEvent";
                     EventSender.SendGlobalEvent("OnContentUIEvent", new ParadoxNotion.EventData(name));
                     MessageDispatcher.Dispatch("OnContentUIEvent", new ParadoxNotion.EventData(name));
                    */

                    /*
                    Transform parent = nowSelectComps.transform.parent;//Button Gamble
                    for (int i = 0; i<10; i++)
                    {
                        if (parent.name == "Button Gamble")
                        {
                            break;
                        }
                        else
                        {
                            parent = parent.transform.parent;
                        }
                    }*/

                    Transform parent = _FindParent(nowSelectComps, "Button Gamble");
                    parent?.GetComponent<ContextButton>()?.DoClick();
                }
                else if (_curSelectMark == "JacksHands")
                {
                    Transform parent = _FindParent(nowSelectComps, "Button Hands");
                    parent?.GetComponent<ContextButton>()?.DoClick();

                    //选框会被游戏打乱顺序排列，要重新复位index
                    _AddTack(() =>
                    {
                        MachineSelectBorder.ResetAutoIndex("JacksCard");
                        Debug.Log("【Task】: JacksCard ");
                    }, 1000);

                }
            }
        }
        Debug.Log($"【machine】: game node button select {name} {_curSelectNumb}");
    }

    private MachineSelectBorder GetNowSelectComp(string mark = null)
    {
        MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();
        MachineSelectBorder nowSelectComps = null;
        for (int i = 0; i < comps.Length; i++)
        {
            if (comps[i].mark == (mark ?? _curSelectMark) && _curSelectNumb == comps[i].index)
            {
                nowSelectComps = comps[i];
                break;
            }
        }
        return nowSelectComps;
    }

    /// <summary>“小游戏节点”是否可见 </summary>
    public bool IsNodeMiniGame()
    {
        GameObject obj = null;
        ChameleonBonusScript chameleonBonusScript = null; ////游戏名：AZTEC_CHARMS,阿兹特克魅力   id：11 特殊判断
        switch (globalStore.nowGameID)
        {
            case 7:
                obj = GameObject.Find("Game Contents/Animator/Anchor/Effect Midground/Dice Game");
                break;
            case 10:
                obj = GameObject.Find("Game Contents/Animator/Anchor/Effect Midground/Free Spins Select");
                break;
            case 11:
                chameleonBonusScript = GameObject.FindObjectOfType<ChameleonBonusScript>();
                break;
            case 21:
                obj = GameObject.Find("Game Canvas/Game Contents/Animator/Anchor/Effect Midground/Wheel Bonus");
                break;
            case 37:
                obj = GameObject.Find("Game Canvas/Game Contents/Animator/Anchor/Effect Midground/Pick Bonus/Animator/Base");
                break;
            case 52:
                obj = GameObject.Find("Game Canvas/Game Contents/LogoActive");
                break;
            case 111:
                obj = GameObject.Find("Game Canvas/Game Contents/Animator/Anchor/Foreground/Map Bonus Popup/Anchor");
                //obj = GameObject.Find("Game Canvas/Game Contents/Animator/Anchor/LogoActive");
                break;
            case 118:
                obj = GameObject.Find("Game Canvas/Game Contents/Animator/Anchor/Effect Midground/Map Bonus");
                break;
            case 123:
                obj = GameObject.Find("Game Canvas/Game Contents/Animator");
                if (obj != null)
                    return (obj.GetComponent<Animator>().GetBool("Blue wheel") || obj.GetComponent<Animator>().GetBool("Green wheel"));
                break;
            case 130:
                obj = GameObject.Find("Game Canvas/Game Contents/Animator/Anchor/Foreground/Main Wheel");
                break;
            case 152:
                obj = GameObject.Find("Game Canvas/Game Contents/Animator/Anchor/Midground/Bonus game");
                break;
            case 153:
                obj = GameObject.Find("Game Canvas/Game Contents/Anchor/Midground/Quick Change Bouns");
                if (obj != null && obj.activeSelf)
                    return true;
                obj = GameObject.Find("Game Canvas/Game Contents/Animator/Anchor/Midground/Xray Bonus");
                break;
            case 154:
                obj = GameObject.Find("Game Canvas/Game Contents/Animator/Anchor/Foreground/Hot Bonus Panel");
                break; ;
            case 175:
                obj = GameObject.Find("Game Canvas/Game Contents/Animator/Anchor/Base/Bonus Game");
                break;
            case 183:
                obj = GameObject.Find("Game Canvas/Game Contents/Animator/Anchor/Base/Slot Frame/Wheel Bonus");
                break;
            case 3004:
                obj = GameObject.Find("Game Canvas/Game Contents/Animator/Anchor/Effect Midground/MiniGameDoor");
                break;
            case 3003:
                obj = GameObject.Find("Effect Midground/Mini Game Bonus/Mini Game0");
                break;
        }
        if (obj != null && obj.activeSelf)
            return true;
        if (chameleonBonusScript != null && chameleonBonusScript.gameObject.activeSelf)
            return true;
        return false;
    }

    public void ConfirmNodeMiniGameSpin(string btnName = "BtnSpin_DOWN")
    {
        switch (globalStore.nowGameID)
        {
            case 10:

                //EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnRoyalFreeSpinClick"));
                break;
            case 11:
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData("ChameleonAttackStart"));
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData("ChameleonAttack"));
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData("FinishedChameleonAnimation"));
                break;
            case 21:
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData("collectEvent"));
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData("MachineSpinClick"));
                break;
            case 52:
                EventSender.SendGlobalEvent("OnContentUIDetailEvent", new EventData<int>("EndToyCrane", 0));
                break;
            case 111:
                EventSender.SendGlobalEvent(EVTType.ON_CONTENT_UI_EVENT, new EventData("OnOpenMapBonusPopup"));
                EventSender.SendGlobalEvent("OnContentUIDetailEvent", new EventData("OnClickMap0"));
                EventSender.SendGlobalEvent("OnContentUIDetailEvent", new EventData("OnBeginMegaFreeSpinMap"));
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnClick"));
                break;
            case 118:
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData("SpinCompass"));
                break;
            case 123:
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData("ClickOnWheel"));
                break;
            case 130:
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Touch"));
                break;
            case 152:
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnStartWheel"));
                EventSender.SendGlobalEvent(EVTType.ON_CONTENT_UI_EVENT, new EventData("StageEnd"));
                break;
            case 153:
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Click"));
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData("0Clicked"));
                break;
            case 154:
                StartCoroutine(ConfirmNodeMiniGameSelect154());
                break;
            case 175:
                EventSender.SendGlobalEvent("OnContentUIEvent", new EventData("StageEnd"));
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Collect"));
                break;
            case 183:
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData<int>("OnBigWheelClick", _curSelectNumb));
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnStartFreeSpin"));
                break;
            case 3004:
                MessageDispatcher.Dispatch("OnCustomEvent", new EventData("MachineSpinClick"));
                break;
            default:
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData("MachineSpinClick"));
                MessageDispatcher.Dispatch(EVTType.ON_MACHINE_BUTTON_EVENT, new EventData($"NodeMiniGame/{btnName}"));
                break;
        }
    } 

    private IEnumerator ConfirmNodeMiniGameSelect154()
    {
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("ClosedPopup"));
        var btns = GameObject.Find("Game Canvas/Game Contents/Animator/Anchor/Foreground/Hot Bonus Panel/Anchor/Board/Buttons").transform;
        for (int i = 0; i < btns.childCount; i++)
        {
            var ani = btns.GetChild(i).GetComponent<Animator>();
            MessageDispatcher.Dispatch("OnContentUIDetailEvent", new EventData<Animator>("UpdateMatchCard", ani));
            //OnContentUIDetailEvent
            yield return new WaitForSeconds(1);
        }
    }

    /*public bool isSpecialButtonShow()
    {
        if (globalStore.nowGameID == 100003) //jack
        {
            GameObject Base = GameObject.Find("Main Canvas/Area/In Game/Anchor/In Game Bottom/Anchor/Layout/Win/Anchor/Button Gamble Area");

            if (Base != null && Base.active)
            {
                return true;
            }

        }

        return false;
    }*/

    public bool isHallGameCategorySelect()
    {
        if (_curSelectMark == "LobbyCategory")
        {
            return true;
        }
        return false;
    }

    public void ConfirmHallGameCategorySelect()
    {
        /*MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();

        //多个页面，都有MachineSelectBorder组件的情况下
        List<MachineSelectBorder> _comps = new List<MachineSelectBorder>();

        MachineSelectBorder nowSelectComps = null;
        for (int i = 0; i < comps.Length; i++)
        {
            if (comps[i].mark == "LobbyCategory" && _curSelectNumb == comps[i].index)
            {
                nowSelectComps = comps[i];
                break;
            }
        }*/

        MachineSelectBorder nowSelectComps = GetNowSelectComp("LobbyCategory");

        if (nowSelectComps != null)
        {
            nowSelectComps.transform.parent.GetComponent<ContextButton>().DoClick();
            //ReflashHallSelect();
        }

    }

    private Transform _FindParent(MachineSelectBorder msb, string name)
    {
        Transform parent = msb.transform.parent;
        for (int i = 0; i < 10; i++)
        {
            if (parent.name == name)
            {
                return parent;
            }
            parent = parent.parent;
            if (parent == null)
            {
                return null;
            }
        }
        return null;
    }

    public bool isChangeButtonRegion()
    {
        return globalStore.nowGameID == -1
            || globalStore.nowGameID == 43
            || globalStore.nowGameID == 200002
        || globalStore.nowGameID == 100003;
    }

    Dictionary<int, string[]> buttonRegions = new Dictionary<int, string[]>
    {
        { -1, new string[] { "", "LobbyCategory" } },
        //{ -1, new string[] {"",} },
        { 200002,new string[] { "SPIN", "SideUI", "ButtonUI", "Spot", }},
        { 100003, new string[] { "SPIN", "JacksGamble", "JacksHands", "JacksCard" }},
        { 43, new string[] { "SPIN", "GameGonfig", }}
    };

    public List<string> GetButtonRegionLst()
    {
        List<string> marks = null;

        if (buttonRegions.ContainsKey(globalStore.nowGameID))
        {
            marks = new List<string>(buttonRegions[globalStore.nowGameID]);
        }
        return marks;
    }

    public List<string> GetVisableButtonRegionLst()
    {
        List<string> marks = GetButtonRegionLst();

        if (marks == null)
            return null;

        MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();

        //去掉隐藏的 MachineSelectBorder
        List<string> visibleMarks = globalStore.nowGameID == -1 ? new List<string>() { "" } : new List<string>();

        comps.ForEach((item) =>
        {
            if (item.transform.gameObject.active && !visibleMarks.Contains(item.mark))
            {
                visibleMarks.Add(item.mark);
            }
        });
        int j = 0;
        while (j < marks.Count)
        {
            if (!visibleMarks.Contains(marks[j]))
            {
                marks.RemoveAt(j);
            }
            else
            {
                j++;
            }
        }
        return marks;
    }

    private List<MachineSelectBorder> GetAllRegionItem()
    {
        List<string> marks = GetVisableButtonRegionLst();

        if (marks == null)
            return null;

        MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();

        List<MachineSelectBorder> res = new List<MachineSelectBorder>();

        List<MachineSelectBorder> _comps = new List<MachineSelectBorder>(comps);

        for (int i = 0; i < marks.Count; i++)
        {
            List<MachineSelectBorder> temp = new List<MachineSelectBorder>();
            int j = 0;
            while (j < _comps.Count)
            {
                if (marks[i] == _comps[j].mark)
                {
                    temp.Add(_comps[j]);
                    _comps.RemoveAt(j);
                }
                else
                {
                    j++;
                }
            }
            temp = MachineSelectBorder.DoBubbling<MachineSelectBorder>(temp, (a, b) =>
            {
                return a.index > b.index;
            });
            res.AddRange(temp);
        }
        return res;
    }

    /// <summary>
    /// 除了Spin外的按钮，统一成一个集合，且“是否选择了并确定该按钮”
    /// </summary>
    Dictionary<int, bool> allRegionRegionAndConfirm = new Dictionary<int, bool>
    {
        //{ 200002,false},
        //{ 100003, false},
        { 43, true}
    };

    /// <summary>
    /// 某些游戏将除了Spin按钮外的所有其他区域的按钮统一到一个集合中，进行切换。
    /// 此时spin按钮，switch切换的队列中，所有不显示边框。
    /// </summary>
    /// <returns></returns>
    public bool isCloseSpinBorder()
    {
        return allRegionRegionAndConfirm.ContainsKey(globalStore.nowGameID) && allRegionRegionAndConfirm[globalStore.nowGameID];
    }

    /// <summary>
    /// 游戏中除了Spin按钮外的所有其他区域的按钮统一到一个集合中，进行切换并且进行确认。
    /// </summary>
    /// <returns></returns>
    public bool isNodeGameSwitchAllRegionNextButtonAndConfirm()
    {
        return isNodeGameSwitchAllRegionNextButton() && allRegionRegionAndConfirm[globalStore.nowGameID];
    }

    /// <summary>
    /// 游戏所有按钮区域，统一成同个集合，且用switch按钮进行切换
    /// </summary>
    /// <returns></returns>
    public bool isNodeGameSwitchAllRegionNextButton()
    {
        if (globalStore.nowGameID == -1)  // 大厅
        {
            return false;
        }

        if (isPopCommon())
            return false;

        return allRegionRegionAndConfirm.ContainsKey(globalStore.nowGameID);
    }

    public void ConfirmSwitchAllRegionNextButton()
    {
        List<MachineSelectBorder> comps = GetAllRegionItem();

        for (int i = 0; i < comps.Count; i++)
        {
            if (comps[i].isSelected)
            {
                int j = i + 1;
                if (j >= comps.Count)
                {
                    j = 0;
                }
                _curSelectNumb = comps[j].index;
                _curSelectMark = comps[j].mark;
                break;
            }
        }

        for (int i = 0; i < comps.Count; i++)
        {
            if (comps[i].index == _curSelectNumb && _curSelectMark == comps[i].mark)
            {
                comps[i].isSelected = true;
            }
            else
            {
                comps[i].isSelected = false;
            }
        }

        setMark(_curSelectMark, _curSelectNumb);
    }

    public void ConfirmChangeButtonRegion()
    {

        List<string> marks = GetVisableButtonRegionLst();

        MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();

        int i = 0;
        for (; i < marks.Count; i++)
        {
            if (_curSelectMark == marks[i])
            {
                break;
            }
        }
        i++;
        if (i >= marks.Count)
        {
            i = 0;
        }


        bool ischange = false;
        if (_curSelectMark != marks[i])
        {
            ischange = true;
            _curSelectMark = marks[i];
            _curSelectNumb = 0;
        }

        //多个页面，都有MachineSelectBorder组件的情况下
        foreach (var item in comps)
        {
            if (item.index == 0 && item.mark == _curSelectMark)
            {
                //item.selectBorder.SetActive(true);
                item.isSelected = true;
            }
            else
            {
                //item.selectBorder.SetActive(false);
                item.isSelected = false;
            }
        }
        //hallMarkStack[0].name = _curSelectMark;
        //hallMarkStack[0].index = _curSelectNumb;
        List<MarkInfo> markStack;
        if (globalStore.nowGameID == -1)
        {
            markStack = hallMarkStack;
        }
        else
        {
            markStack = gameMarkStack;
        }

        markStack[markStack.Count - 1].name = _curSelectMark;
        markStack[markStack.Count - 1].index = _curSelectNumb;


        if (globalStore.nowGameID == -1)  // 大厅
        {

            GameObject Base = GameObject.Find("Lobby/Anchor/Lobby Pages/Anchor/Slots Area/ScrollView Slots");

            if (Base != null && Base.activeSelf)
            {
                if (_curSelectMark == "")
                {
                    if (ischange)
                    {
                        Base.GetComponent<OSA_LobbySlots>().SetCurSelect(_curSelectNumb);
                    }
                }
                else
                {
                    Base.GetComponent<OSA_LobbySlots>().SetCurSelect(-1);
                }
            }
        }
    }

    public bool isMenuOpen()
    {
        if (globalStore.nowGameID == -1)  // 大厅
        {
            GameObject Base = GameObject.Find("Lobby/Anchor/Navigation Bar/Anchor/Layout/Right/Menu/Dropdown Menu/Anchor");

            if (Base != null && Base.activeSelf)
            {
                if (_curSelectMark != "MenuHall")
                {
                    _curSelectNumb = 0;
                    _curSelectMark = "MenuHall";

                    AddMark(_curSelectMark, _curSelectNumb, MarkType.Frame);
                }
                return true;
            }
        }
        else
        {
            GameObject Base = GameObject.Find("In Game/Anchor/Navigation Bar/Anchor/Layout/Right/Menu In Game/Dropdown Menu/Anchor");

            if (Base != null && Base.activeSelf)
            {
                if (_curSelectMark != "MenuGame")
                {
                    _curSelectNumb = 0;
                    _curSelectMark = "MenuGame";

                    AddMark(_curSelectMark, _curSelectNumb, MarkType.Frame);
                }

                return true;
            }
        }


        if (_curSelectMark == "MenuHall" || _curSelectMark == "MenuGame")
        {
            RemoveMark(_curSelectMark);
        }
        return false;
    }

    public void ConfirmMenuSelect()
    {
        string name = "";
        if (globalStore.nowGameID == -1)  // 大厅
        {

            switch (_curSelectNumb)
            {
                case 0: //设置
                    name = "OpenSettings";
                    break;
            }

            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData(name));

        }
        else
        {
            switch (_curSelectNumb)
            {
                case 0: //设置
                    name = "OpenSettings";
                    break;
                case 1: //帮助按钮
                    name = "OpenPaytable";
                    break;
            }

            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData(name));

        }

        Debug.Log($"【machine】: Menu  click  {name} ");
    }

    public bool isPopSysSettingSelect()
    {
        GameObject Base = GameObject.Find("Popup Manager/Area/Popup Settings");

        if (Base != null && Base.activeSelf)
        {
            if (_curSelectMark != "SettingsCell")  //首次打开进行赋值
            {
                _curSelectNumb = 0;
                _curSelectMark = "SettingsCell";
                AddMark(_curSelectMark, _curSelectNumb, MarkType.Pop);
            }
            return true;
        }

        if (_curSelectMark == "SettingsCell")
        {
            RemoveMark("SettingsCell");
        }
        return false;
    }
    public void ConfirmSysSettingSelect()
    {
        /*MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();

        //多个页面，都有MachineSelectBorder组件的情况下
        List<MachineSelectBorder> _comps = new List<MachineSelectBorder>();

        MachineSelectBorder nowSelectComps = null;
        for (int i = 0; i < comps.Length; i++)
        {
            if (comps[i].mark == "SettingsCell" && _curSelectNumb == comps[i].index)
            {
                nowSelectComps = comps[i];
                break;
            }
        }*/

        MachineSelectBorder nowSelectComps = GetNowSelectComp("SettingsCell");

        if (nowSelectComps != null)
        {
            string name = "";

            /*ContextToggle ctg = nowSelectComps.transform.parent.GetComponent<ContextToggle>();
            Debug.Log($"set toggle.isOn  =  {!ctg.GetBooleanProperty()}");
            ctg.SetBooleanProperty(!ctg.GetBooleanProperty());*/


            List<string> buttons = new List<string> { "Settings Cell SFX", "Settings Cell BGM" };

            Transform parent = nowSelectComps.transform.parent;
            while (!buttons.Contains(parent.name))
            {
                parent = parent.parent;
            }

            switch (parent.name)
            {
                case "Settings Cell SFX":
                    name = "OnSettingSFX";
                    break;
                case "Settings Cell BGM":
                    name = "OnSettingBGM";
                    break;
            }
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData(name));
        }

        Debug.Log($"【machine】: Sys Setting select ");
    }

    // 游戏玩法说明界面
    public bool isPopPayTab()
    {
        GameObject Base = GameObject.Find("Popup Manager/Contents/Pay Table");

        if (Base != null && Base.activeSelf)
        {
            return true;
        }

        return false;
    }

    public void PreviousPayTabPage()
    {
        EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData("Prev"));
    }

    public void NextPayTabPage()
    {
        EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData("Next"));
    }

    public void PreviousSelectItem()
    {
        //GameObject[] selectLst =  GameObject.FindGameObjectsWithTag("MachineSelect"); //对个对象，却只能找到一个对象
        MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();


        bool isExist = false;
        for (int i = 0; i < comps.Length; i++)
        {
            if (comps[i].mark == _curSelectMark)
            {
                isExist = true;
                break;
            }
        }

        if (!isExist)
            _curSelectMark = ""; //当拥有mark标志的选框都不存在时（比如：跟随弹窗关闭被销毁了），恢复为默认。


        //多个页面，都有MachineSelectBorder组件的情况下
        List<MachineSelectBorder> _comps = new List<MachineSelectBorder>();
        foreach (var item in comps)
        {
            if (item.mark == _curSelectMark)
            {
                _comps.Add(item);
            }
        }
        comps = _comps.ToArray();

        //Debug.Log($"Length = {comps.Length} selectMark = {selectMark}");

        MachineSelectBorder compLast = null;
        int index = -1;
        bool isFind = false;


        for (int i = 0; i < comps.Length; i++)
        {
            //if (comps[i].selectBorder.active)
            if (comps[i].isSelected)
            {
                index = comps[i].index;
                break;
            }
        }



        index--;
        if (index < 0)
            index = comps.Length - 1;

        int y = 0;
        while (!isFind && ++y < 200)
        {
            for (int i = 0; i < comps.Length; i++)
            {
                //comps[i].selectBorder.SetActive(false);
                comps[i].isSelected = false;
                if (comps[i].index == comps.Length - 1)
                {
                    compLast = comps[i];
                }
                if (comps[i].index == index && !comps[i].isIgnore)
                {
                    //comps[i].selectBorder.SetActive(true);
                    comps[i].isSelected = true;
                    _curSelectNumb = index;
                    isFind = true;
                }
            }
            index--;
            if (index < 0)
                index = comps.Length - 1;
        }

        if (!isFind)
        {
            Debug.LogError("==@找不到下一个对象");
        }

        /*
        index--;
        for (int i = 0; i < comps.Length; i++)
        {
            //comps[i].selectBorder.SetActive(false);
            comps[i].isSelected = false;
            if (comps[i].index == comps.Length - 1)
            {
                compLast = comps[i];
            }
            if (comps[i].index == index)
            {
                //comps[i].selectBorder.SetActive(true);
                comps[i].isSelected = true;
                _curSelectNumb = index;
                isFind = true;
            }
        }

        if (!isFind && compLast != null)
        {
            _curSelectNumb = compLast.index;
            //compLast.selectBorder.SetActive(true);
            compLast.isSelected = true;
        }*/
        setMark(_curSelectMark, _curSelectNumb);
    }

    public void NextSelectItem()
    {
        MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();


        /*
        bool isExist = false;
        for (int i = 0; i < comps.Length; i++)
        {
            if (comps[i].mark == _curSelectMark)
            {
                isExist = true;
                break;
            }
        }*/

        bool isExist = false;
        for (int i = 0; i < comps.Length; i++)
        {
            if (comps[i].mark == _curSelectMark && !comps[i].isIgnore)
            {
                isExist = true;
                break;
            }
        }

        if (!isExist)
            _curSelectMark = ""; //当拥有mark标志的选框都不存在时（比如：跟随弹窗关闭被销毁了），恢复为默认。


        List<MachineSelectBorder> _comps = new List<MachineSelectBorder>();
        foreach (var item in comps)
        {
            if (item.mark == _curSelectMark)
            {
                _comps.Add(item);
            }
        }
        comps = _comps.ToArray();

        //Debug.Log($"Length = {comps.Length} selectMark = {selectMark}");


        MachineSelectBorder compFirst = null;
        int index = -1;
        bool isFind = false;

        for (int i = 0; i < comps.Length; i++)
        {
            //if (comps[i].selectBorder.active)
            if (comps[i].isSelected)
            {
                index = comps[i].index;
                break;
            }
        }

        index++;
        int y = 0;
        while (!isFind && ++y < 200)
        {
            for (int i = 0; i < comps.Length; i++)
            {
                comps[i].isSelected = false;
                if (comps[i].index == 0)
                {
                    compFirst = comps[i];
                }
                if (comps[i].index == index && !comps[i].isIgnore)
                {
                    comps[i].isSelected = true;
                    _curSelectNumb = index;
                    isFind = true;
                }
            }
            index++;
            if (index >= comps.Length)
                index = 0;
        }

        if (!isFind)
        {
            Debug.LogError("==@ Unable to find the next MachineSelect item");
        }


        /*
        index++;
        for (int i = 0; i < comps.Length; i++)
        {
            comps[i].isSelected = false;
            if (comps[i].index == 0)
            {
                compFirst = comps[i];
            }
            if (comps[i].index == index)
            {
                comps[i].isSelected = true;
                _curSelectNumb = index;
                isFind = true;
            }
        }
        if (!isFind && compFirst != null)
        {
            _curSelectNumb = 0;
            compFirst.isSelected = true;
        }*/

        setMark(_curSelectMark, _curSelectNumb);
    }

    bool isLightBtnSelect = false;

    Dictionary<int, List<LightBtnTarget>> dicNodeMiniGameLightBtnSelect = new Dictionary<int, List<LightBtnTarget>>()
    {
        [3000] = new List<LightBtnTarget>()
        {
            new LightBtnTarget("Effect Midground/Door Bonus/Door Chose 01",3),
            //new LightBtnTarget("Effect Midground/Door Bonus/miniGame00",5),
            //new LightBtnTarget("Effect Midground/Door Bonus/miniGame01",3),
            //new LightBtnTarget("Effect Midground/Door Bonus/miniGame02",7),
        },
        [3001] = new List<LightBtnTarget>()
        {
            new LightBtnTarget("Effect Midground/Door",3),
        },
        [3003] = new List<LightBtnTarget>()
        {
            new LightBtnTarget("Effect Midground/Mini Game2/Animator/Cards/Tip Take It",2),
        },
    };

    public void ForeChangeLightBtn(bool toOpen)
    {
        //bool _islightBtnSelect = isPopGameConfigSelect();
        if (toOpen)
        {
            isLightBtnSelect = false;
            isPopGameConfigSelect();
        }
        else
        {
            MessageDispatcher.Dispatch(MACHINE_BTN_EVENT, new EventData("LightBtnSelectShowTipOff"));
            MessageDispatcher.Dispatch(MACHINE_BTN_EVENT, new EventData<string[]>("ChangeSceneBtnLight", new string[] { }));
        }
    }

    [Button]
    void test_ShowIsLightBtnSelectInGame()
    {
        isLightBtnSelectInGame(true);
    }



    public bool isLightBtnSelectInGame(bool isTest = false)
    {
        //return false;

        if (IsGameCustomsLightButton())
        {
            if (gcb.isShowBtn && !isLightBtnSelect)
            {
                isLightBtnSelect = true;
                MessageDispatcher.Dispatch(MACHINE_BTN_EVENT, new EventData<int>("LightBtnSelectShowTipOn", gcb.numlightBtn));
            }
            else if (!gcb.isShowBtn && isLightBtnSelect)
            {
                isLightBtnSelect = false;
                MessageDispatcher.Dispatch(MACHINE_BTN_EVENT, new EventData("LightBtnSelectShowTipOff"));
            }
            return true;
        }
        else if (isPopFreeGameTimeSelect())
        {
            if (!isLightBtnSelect)
            {
                isLightBtnSelect = true;
                MessageDispatcher.Dispatch(MACHINE_BTN_EVENT, new EventData<int>("LightBtnSelectShowTipOn", getPopFreeGameTimeSelectBtnNum()));
            }
            return true;
        }
        else if (isPopGameConfigSelect())
        {
            if (!isLightBtnSelect)
            {
                isLightBtnSelect = true;
                MessageDispatcher.Dispatch(MACHINE_BTN_EVENT, new EventData<int>("LightBtnSelectShowTipOn", getPopGameConfigSelectBtnNum()));
            }
            return true;
        }
        else if (dicNodeMiniGameLightBtnSelect.ContainsKey(globalStore.nowGameID) && !isPopupJackpot())
        {

            List<LightBtnTarget> lst = dicNodeMiniGameLightBtnSelect[globalStore.nowGameID];

            for (int i = 0; i < lst.Count; i++)
            {
                GameObject Base = GameObject.Find(lst[i].nodePath); //door

                if (Base != null && Base.activeSelf)
                {
                    if (!isLightBtnSelect)
                    {
                        isLightBtnSelect = true;
                        MessageDispatcher.Dispatch(MACHINE_BTN_EVENT, new EventData<int>("LightBtnSelectShowTipOn", lst[i].btnNum));
                    }

                    if (isTest)
                        Debug.Log($" ==@ [test] = true: {lst[i].nodePath}");
                    return true;
                }

                if (isTest)
                    Debug.Log($" ==@ [test] = false: {lst[i].nodePath}");
            }
        }


        if (isLightBtnSelect)
        {
            MessageDispatcher.Dispatch(MACHINE_BTN_EVENT, new EventData("LightBtnSelectShowTipOff"));
        }
        isLightBtnSelect = false;

        return false;
    }

    /// <summary>
    /// 弹窗出现选择框时，压入mark堆栈，的mark值
    /// 默认mark = ""
    /// </summary>
    Dictionary<int, String> markPopGameConfigSelect = new Dictionary<int, String>()
    {
        //{142,"" },
    };
    Dictionary<int, String> markPopFreeGameTimeSelect = new Dictionary<int, String>()
    {
        //{37,"" },
        //{93,"" },
        //{105,"" },
        //{116,"" },
        //{149,"" },
    };

    /// <summary>
    /// 选择框改用机台灯闪烁选择，而不是选择框
    /// 筛选出要屏蔽的选择框
    /// </summary>
    /// <returns></returns>
    public KeyValuePair<bool, string> GetIgonreBorderWhenUseLightBtnSelect()
    {
        if (isLightBtnSelectInGame())  //使用了机台灯闪烁选择
        {
            if (isPopFreeGameTimeSelect())
            {
                if (markPopFreeGameTimeSelect.ContainsKey(globalStore.nowGameID))
                {
                    return new KeyValuePair<bool, string>(true, markPopFreeGameTimeSelect[globalStore.nowGameID]);
                }
                else
                {
                    return new KeyValuePair<bool, string>(true, "");
                }
            }
            else if (isPopGameConfigSelect())
            {
                if (markPopGameConfigSelect.ContainsKey(globalStore.nowGameID))
                {
                    return new KeyValuePair<bool, string>(true, markPopGameConfigSelect[globalStore.nowGameID]);
                }
                else
                {
                    return new KeyValuePair<bool, string>(true, "");
                }
            }
        }
        return new KeyValuePair<bool, string>(false, "");
    }

    public void ConfirmLightBtnSelectInGame(int index)
    {
        if (isPopFreeGameTimeSelect())
        {
            ConfirmPopFreeGameSelect(index);
        }
        else if (isPopGameConfigSelect())
        {
            ConfirmPopGameConfigSelect(index);
        }
        else if (IsGameCustomsButton())
        {
            MessageDispatcher.Dispatch(EVTType.ON_MACHINE_BUTTON_EVENT, new EventData<int>("GameCustomsButton/BtnLight_DOWN", index));
        }
        else if (dicNodeMiniGameLightBtnSelect.ContainsKey(globalStore.nowGameID))
        {
            MessageDispatcher.Dispatch(EVTType.MACHINE_BUTTON_SELECT_UI_EVTTYPE, new EventData<int>("DoorSelect", index));
        }
    }

    SceneBtnType lastSceneBtnType = SceneBtnType.None;

    private void OpenAllSceneBtn()
    {
        MessageDispatcher.Dispatch(MACHINE_BTN_EVENT, new EventData<string[]>("ChangeSceneBtnLight", new string[] {
                    "BtnSpin",
                    "BtnPre",
                    "BtnNext",
                    "BtnExit",
                    "BtnSwitch",
                    "BtnBetUp",
                    "BtnBetDown",
                    "BtnBetMax",
                    "BtnHelp",

                }));
    }
    private void CloseAllSceneBtn()
    {
        MessageDispatcher.Dispatch(MACHINE_BTN_EVENT, new EventData<string[]>("ChangeSceneBtnLight", new string[] { }));
    }

    public void ReflashSceneBtn()
    {

        if (globalStore.nowGameID != -1)
        {
            if (isLightBtnSelectInGame())
            {
                return;
            }
        }
        string[] btns = null;

        if (isPopupCommonOk())
        {
            if (lastSceneBtnType != SceneBtnType.CommonOk)
            {
                lastSceneBtnType = SceneBtnType.CommonOk;
                btns = new string[] { "BtnSpin" };
            }
        }
        else if (isEnterOrLeaveGameInTop())
        {
            if (lastSceneBtnType != SceneBtnType.EnterOrLeaveGame)
            {
                lastSceneBtnType = SceneBtnType.EnterOrLeaveGame;
                btns = new string[] { };
            }
        }
        else if (isPopSysSettingSelect())
        {
            if (lastSceneBtnType != SceneBtnType.SysSetting)
            {
                lastSceneBtnType = SceneBtnType.SysSetting;
                btns = new string[] { "BtnSpin", "BtnExit", "BtnPre", "BtnNext" };
            }
        }
        else if (isMenuOpen())
        {
            if (lastSceneBtnType != SceneBtnType.Menu)
            {
                lastSceneBtnType = SceneBtnType.Menu;
                btns = new string[] { "BtnSpin", "BtnPre", "BtnNext" };
            }
        }
        else if (isPopPayTab()) //(code == "HELP")
        {
            if (lastSceneBtnType != SceneBtnType.Help)
            {
                lastSceneBtnType = SceneBtnType.Help;
                btns = new string[] { "BtnExit", "BtnPre", "BtnNext" };
            }
        }
        else if (isPopCommon() || isPopupJackpot())
        {
            if (lastSceneBtnType != SceneBtnType.Pop)
            {
                lastSceneBtnType = SceneBtnType.Pop;
                btns = new string[] { "BtnSpin" };
            }
        }
        else if (IsGameCustomsButton())
        {
            Dictionary<string, string> dic = gcb.GetCustomsButton();
            btns = dic.Keys.ToArray();
        }
        else if (isNodeMiniGameSelect())
        {
            if (lastSceneBtnType != SceneBtnType.MiniGameSelect)
            {
                lastSceneBtnType = SceneBtnType.MiniGameSelect;
                btns = new string[] { "BtnSpin", "BtnPre", "BtnNext" };
            }
        }

        else if (IsNodeMiniGame())
        {
            if (lastSceneBtnType != SceneBtnType.MiniGame)
            {
                lastSceneBtnType = SceneBtnType.MiniGame;
                btns = new string[] { "BtnSpin" };
            }
        }

        else if (globalStore.nowGameID == -1)
        {
            if (lastSceneBtnType != SceneBtnType.Hall)
            {
                lastSceneBtnType = SceneBtnType.Hall;
                btns = new string[] { "BtnSpin", "BtnPre", "BtnNext" };
            }
        }
        else if (globalStore.nowGameID != -1)
        {
            if (lastSceneBtnType != SceneBtnType.Game)
            {
                lastSceneBtnType = SceneBtnType.Game;
                List<string> common = new List<string>() { "BtnSpin", "BtnExit", "BtnBetUp", "BtnBetDown", "BtnBetMax", "BtnHelp" };

                if (globalStore.nowGameID == 200002)
                {
                    common.AddRange(new List<string>() { "BtnSwitch", "BtnPre", "BtnNext" });
                }

                if (globalStore.nowGameID == 100003)
                {
                    common.AddRange(new List<string>() { "BtnSwitch", "BtnPre", "BtnNext" });
                }

                if (globalStore.nowGameID == 43)
                {
                    common.AddRange(new List<string>() { "BtnSwitch", });
                }

                if (globalStore.nowGameID >= 3000 && globalStore.nowGameID <= 3999)
                {
                    common.AddRange(new List<string>() { "BtnPre", "BtnNext" });
                }
                btns = common.ToArray();
            }
        }

        // 进入游戏： Popup Manager/Area/Loading
        // 返回游戏： Popup Manager/Area/Loading To Lobby
        // Popup Manager/Overlay/Popup Common Ok

        if (btns != null)
        {

            string temp = "";
            btns.ForEach(item =>
            {
                temp += $"#{item}";
            });

            Debug.Log($" @btns = {temp}      ;  lastSceneBtnType = {lastSceneBtnType.ToString()}   ");
            MessageDispatcher.Dispatch(MACHINE_BTN_EVENT, new EventData<string[]>("ChangeSceneBtnLight", btns));
        }
    }

    bool isEnterOrLeaveGameInTop()
    {
        // 进入游戏： Popup Manager/Area/Loading
        // 返回游戏： Popup Manager/Area/Loading To Lobby
        // 弹窗: Popup Manager/Overlay/Popup Common Ok

        GameObject Base = GameObject.Find("Popup Manager/Area/Loading");
        if (Base != null && Base.activeSelf)
        {
            return true;
        }
        Base = GameObject.Find("Popup Manager/Area/Loading To Lobby");
        if (Base != null && Base.activeSelf)
        {
            return true;
        }
        return false;
    }

    bool isPopupCommonOk()
    {
        // 进入游戏： Popup Manager/Area/Loading
        // 返回游戏： Popup Manager/Area/Loading To Lobby
        // 弹窗: Popup Manager/Overlay/Popup Common Ok

        GameObject Base = GameObject.Find("Popup Manager/Overlay/Popup Common Ok");
        if (Base != null && Base.active)
        {
            return true;
        }
        return false;
    }

    readonly string soundDefault = "UI_Button_Normal";
    string sound;

    private void PlaySound()
    {
        if (sound != null)
        {
            GSManager.Instance.GetHandler(sound).Play();
        }
        sound = soundDefault;
    }

    public string BtnSpinDOWN()
    {
#if !UNITY_EDITOR
        if (!ApplicationSettings.Instance.isMachine)
            return "";
#endif
        string _state = "";
        if (isPopSysSettingSelect())
        {
            ConfirmSysSettingSelect();

            _state = "isPopSysSettingSelect";
        }
        else if (isPopupJackpot())
        {
            ConfirmPopCommon();

            _state = "isPopupJackpot";
        }
        else if (IsGameCustomsButton())
        {
            sound = ConfirmGameCustomsButton("BtnSpin_DOWN");
            _state = $"GameCustomsButton - {gcb.mark}";
        }
        else if (isLightBtnSelectInGame())
        {
            // 忽略spin
            sound = null;
            _state = "isLightBtnSelectInGame";
        }
        else if (isPopFreeGameTimeSelect())
        {
            ConfirmPopFreeGameSelect(_curSelectNumb);
            _state = "isPopFreeGameTimeSelect";
        }
        else if (isPopGameConfigSelect())
        {
            ConfirmPopGameConfigSelect(_curSelectNumb);
            _state = "isPopGameConfigSelect";
        }
        else if (isPopMiniGameSelect())
        {
            ConfirmPopMiniGameSelect();
            _state = "isPopMiniGameSelect";
        }
        else if (isPopCommon())
        {
            ConfirmPopCommon();
            _state = "isPopCommon";
        }
        else if (isMenuOpen())
        {
            ConfirmMenuSelect();
            _state = "isMenuOpen";
        }
        else if (isHallGameCategorySelect())
        {
            ConfirmHallGameCategorySelect();
            _state = "isHallGameCategorySelect";
        }

        else if (isNodeMiniGameSelect())
        {
            ConfirmNodeMiniGameSelect();
            _state = "isNodeMiniGameSelect";
        }
        else if (IsNodeMiniGame())
        {
            ConfirmNodeMiniGameSpin();
            _state = "isNodeMiniGame";
        }
        /**
         * Switch 选择并确定
         * Switch 选择 + Spin 确定
         * Switch切换按钮区域 + 左右选择 + Spin 确定
         
        else if (!isNodeGameSwitchAllRegionNextButtonAndConfirm() && isNodeGameSwitchAllRegionNextButton())
        {
            ConfirmNodeGameButtonSelect();
        }
        else if (!isNodeGameSwitchAllRegionNextButton() && isNodeGameButtonSelect())
        {
            ConfirmNodeGameButtonSelect();
        }*/
        else if (isNodeGameButtonSelect() && !isNodeGameSwitchAllRegionNextButtonAndConfirm())
        {
            ConfirmNodeGameButtonSelect();
            _state = "isNodeGameButtonSelect";
        }
        else if (isIgnoreComSpinInGame())
        {
            // 忽略spin
            sound = null;
            _state = "isIgnoreComSpinInGame";
        }
        else
        {
            if (globalStore.nowGameID == 100003) //Jack - 纸牌
            {
                EventSender.SendGlobalEvent("OnSpinButtonEvent", new ParadoxNotion.EventData("OnSpinButtonEvent"));
            }
            else
            {
                sound = null;
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData<int>(MachineEventDefine.ON_KEY_START, 1));
            }
            Debug.Log($"【machine】: Spin  click");

            _state = "isSpin";
        }

        PlaySound();

        return _state;
    }

    public bool isIgnoreComSpinInGame()
    {
        bool isIgon = false;
        if (BlackboardQueryUtils.IsIngame() && isChangeButtonRegion() && gameMarkStack.Count == 1)
        {
            if (isNodeGameSwitchAllRegionNextButtonAndConfirm())
            {
                isIgon = false;
            }
            else if (_curSelectMark != "SPIN")
            {
                isIgon = true;
            }
        }

        return isIgon;
    }

    public bool isPopupJackpot()
    {

        Transform parent = GameObject.Find("Popup Manager/Contents")?.transform;

        if (transform != null && parent.childCount > 0)
        {
            List<string> jackpotNode = new List<string>()
            {
                "Game Grand Jackpot Trigger Popup",
                "Game Mega Jackpot Trigger Popup",
                "Grand Jackpot Trigger Popup",
                "Major Jackpot Trigger Popup",
                "Mega Jackpot Trigger Popup",
                "Mini Jackpot Trigger Popup",
                "Minor Jackpot Trigger Popup",
                "Jackpot1 Trigger Popup",
                "Jackpot2 Trigger Popup",
                "Jackpot3 Trigger Popup",
                "Bonus Trigger Popup",
                "Jackpot Trigger Popup",
                "You Win Trigger Popup",
                "OpenLock",
            };
            for (int i = 0; i < jackpotNode.Count; i++)
            {
                Transform jackpot = parent.Find(jackpotNode[i]);
                if (jackpot != null)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool isCanDownNextOrPre()
    {
        bool isCanDownNextOrPre = false;
        isCanDownNextOrPre = BlackboardUtils.GetOrCreateVariable<bool>(MainBlackboard.Get(), "isCanDownNextOrPre").value;
        return isCanDownNextOrPre;
    }

    public void BtnSpinUP()
    {
#if !UNITY_EDITOR
        if (!ApplicationSettings.Instance.isMachine)
            return;
#endif

        if (globalStore.nowGameID != -1 && !isIgnoreComSpinInGame())
        {
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData<int>(MachineEventDefine.ON_KEY_START, 0));
        }
        else if (IsGameCustomsButton())
        {

            sound = ConfirmGameCustomsButton("BtnSpin_UP");
        }

    }
    /**
     *       //(int)args[1] 0: 抬起
            //(int)args[1] 1: 按下
    */

    public void BtnReturnDOWN()
    {
#if !UNITY_EDITOR
        if (!ApplicationSettings.Instance.isMachine)
            return;
#endif

        if (IsGameCustomsButton())
        {

            sound = ConfirmGameCustomsButton("BtnExit_DOWN");
        }
        else if (isPopCommon())  //关闭所有弹窗
        {
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData("OnClose")); // 关闭弹窗
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData("Return")); // 关闭游戏玩法说明弹窗
        }
        else if (BlackboardQueryUtils.IsIngame()
            && !BlackboardQueryUtils.IsSpin()
            && !BlackboardQueryUtils.IsAutoSpin())
        {
            sound = null;
            EventSender.SendGlobalEvent("OnLobby");  //退出游戏
        }
        else
        {
            sound = null;
        }
        PlaySound();
    }

    public void BtnSwitchDOWN()
    {
#if !UNITY_EDITOR
        if (!ApplicationSettings.Instance.isMachine)
            return;
#endif

        if (isMenuOpen()
            || isPopCommon())
        {
            return;
        }

        if (IsGameCustomsButton())
        {
            sound = ConfirmGameCustomsButton("BtnSwitch_DOWN");
        }
        else if (isNodeGameSwitchAllRegionNextButton())
        {
            ConfirmSwitchAllRegionNextButton();
            if (isNodeGameSwitchAllRegionNextButtonAndConfirm())
            {
                /*ConfirmNodeGameButtonSelect();*/
                _AddTack(() =>
                {
                    ConfirmNodeGameButtonSelect();
                }, 500);
            }
        }
        else if (isChangeButtonRegion())
        {
            ConfirmChangeButtonRegion();
        }
        PlaySound();
    }

    public void BtnHelpDOWN()
    {
#if !UNITY_EDITOR
        if (!ApplicationSettings.Instance.isMachine)
            return;
#endif

        if (
           !isPopCommon()
           && !BlackboardQueryUtils.IsSpin() //游戏中不能按菜单
           && !BlackboardQueryUtils.IsAutoSpin()
        )
        {
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData("OpenPaytable"));
        }
        else if (IsGameCustomsButton())
        {
            sound = ConfirmGameCustomsButton("BtnHelp_DOWN");
        }
        PlaySound();
    }

    public void BtnMenuDOWN()
    {
#if !UNITY_EDITOR
        if (!ApplicationSettings.Instance.isMachine)
            return;
#endif

        if (
            !isPopCommon()
            && !BlackboardQueryUtils.IsSpin() //游戏中不能按菜单
            && !BlackboardQueryUtils.IsAutoSpin()
        )
        {
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData("ToggleMenu"));
        }

        PlaySound();
    }
    public void BtnNextUP()
    {
        if (IsGameCustomsButton())
        {
            ConfirmGameCustomsButton("BtnNext_UP");
        }
    }

    public bool BtnNextDOWN()
    {
#if !UNITY_EDITOR
        if (!ApplicationSettings.Instance.isMachine)
            return false;
#endif
        bool isNext = true;
        if (isPopSysSettingSelect())
        {
            NextSelectItem();
        }
        else if (isPopPayTab())
        {
            NextPayTabPage();
        }
        else if (isMenuOpen())
        {
            NextSelectItem();
        }
        else if (isPopupJackpot())
        {
            isNext = false;
        }
        if (IsGameCustomsButton())
        {
            isNext = true;
            sound = ConfirmGameCustomsButton("BtnNext_DOWN");
        }
        else if (isCanDownNextOrPre())
        {
            isNext = false;
        }
        else if (isLightBtnSelectInGame())
        {
            isNext = false;
        }
        else if (isPopFreeGameTimeSelect()
            || isNodeMiniGameSelect()
            || isPopGameConfigSelect()
            || isPopMiniGameSelect())
        {
            NextSelectItem();
        }
        else if (isNodeGameSwitchAllRegionNextButton())
        {
            isNext = false;
        }
        else if (!isNodeGameSwitchAllRegionNextButton() && isNodeGameButtonSelect())
        {
            NextSelectItem();
        }
        else if (isGameApostarSelect()) //apostar
        {
            ConfirmNewGameApostarSelect();
        }
        else if (globalStore.nowGameID == -1 && _curSelectMark != "")
        {
            NextSelectItem();


        }
        else if (globalStore.nowGameID == -1)
        {
            sound = null;
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_RIGHT));
        }
        else
        {
            isNext = false;
        }


        if (isNext)
            PlaySound();
        return isNext;
    }

    public void BtnPreUP()
    {
        if (IsGameCustomsButton())
        {
            ConfirmGameCustomsButton("BtnPre_UP");
        }
    }

    public bool BtnPreDOWN()
    {
#if !UNITY_EDITOR
        if (!ApplicationSettings.Instance.isMachine)
            return false;
#endif

        bool isPre = true;

        if (isPopSysSettingSelect())
        {
            PreviousSelectItem();
        }
        else if (isPopPayTab())
        {
            PreviousPayTabPage();
        }
        else if (isMenuOpen())
        {
            PreviousSelectItem();
        }
        else if (isPopupJackpot())
        {
            isPre = false;
        }
        if (IsGameCustomsButton())
        {
            isPre = true;
            sound = ConfirmGameCustomsButton("BtnPre_DOWN");
        }
        else if (isCanDownNextOrPre())
        {
            isPre = false;
        }
        else if (isLightBtnSelectInGame())
        {
            isPre = false;
        }
        else if (isPopFreeGameTimeSelect()
            || isNodeMiniGameSelect()
            || isPopGameConfigSelect()
            || isPopMiniGameSelect())
        {
            PreviousSelectItem();
        }
        else if (isNodeGameSwitchAllRegionNextButton())
        {
            isPre = false;
        }
        else if (!isNodeGameSwitchAllRegionNextButton() && isNodeGameButtonSelect())
        {
            PreviousSelectItem();
        }
        else if (isGameLineSelect()) //line
        {
            ConfirmNewGameLineSelect();
        }
        else if (globalStore.nowGameID == -1 && _curSelectMark != "")
        {
            PreviousSelectItem();
        }
        else if (globalStore.nowGameID == -1)
        {
            sound = null;
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_LEFT));
        }
        else
        {
            isPre = false;
        }

        if (isPre)
            PlaySound();
        return isPre;
    }

    public void BtnBetMaxDOWN()
    {
#if !UNITY_EDITOR
        if (!ApplicationSettings.Instance.isMachine)
            return;
#endif

        if (BlackboardQueryUtils.IsIngame()  //玩时，不能加钱
            && !isPopCommon()
            && !BlackboardQueryUtils.IsSpin()
            && !BlackboardQueryUtils.IsAutoSpin()
        )
        {
            sound = "UI_Betting_Max";
            //最大下注
            MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetMax"));
        }
        else if (IsGameCustomsButton())
        {
            sound = ConfirmGameCustomsButton("BtnBetMax_DOWN");
        }
        else
        {
            sound = null;
        }
        PlaySound();
    }
    public void BtnBetUpDOWN()
    {
#if !UNITY_EDITOR
        if (!ApplicationSettings.Instance.isMachine)
            return;
#endif

        /* BtnBetUp 和 BtnNext 合成一个键
        if (BtnNext())
        {
            return;
        }
        */

        if (

            BlackboardQueryUtils.IsIngame()  //玩时，不能加钱
            && !isPopCommon()
            && !BlackboardQueryUtils.IsSpin()
            && !BlackboardQueryUtils.IsAutoSpin()
            )
        {
            sound = null;
            //提高押注
            MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetUp"));
        }
        else if (IsGameCustomsButton())
        {
            sound = ConfirmGameCustomsButton("BtnBetUp_DOWN");
        }
        else
        {
            sound = null;
        }
        //炮右移
        //EventSender.SendGlobalEvent(MetaEventDefine.ON_MACHINE, new EventData<int>(MachineEventDefine.ON_KEY_RIGHT, data));
        PlaySound();
    }

    /**
     * 
     * */
    public void BtnBetDownDOWN()
    {
#if !UNITY_EDITOR
        if (!ApplicationSettings.Instance.isMachine)
            return;
#endif

        /* BtnBetDown 和 BtnPre 合成一个键
        if (BtnPre())
        {
            return;
        }
        */

        if (BlackboardQueryUtils.IsIngame()  //玩时，不能减钱
            && !isPopCommon()
            && !BlackboardQueryUtils.IsSpin()
            && !BlackboardQueryUtils.IsAutoSpin()
        )
        {
            sound = null;
            //降低押注
            MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetDown"));
        }
        else if (IsGameCustomsButton())
        {
            sound = ConfirmGameCustomsButton("BtnBetDown_DOWN");
        }
        else
        {
            sound = null;
        }
        //炮左移
        //EventSender.SendGlobalEvent(MetaEventDefine.ON_MACHINE, new EventData<int>(MachineEventDefine.ON_KEY_LEFT, data));
        PlaySound();
    }
}

/*Test(bol: true);
private void Test(int data = 0, string str = "", bool bol = false)
{

}*/

public partial class MachineSelectManager : MonoSingleton<MachineSelectManager>
{
    [Button]
    void test_isPopCommon()
    {
        Debug.Log($"==@ is Pop Common : {isPopCommon()}");
    }
    [Button]
    void test_ShowSpinButtonState()
    {
        Debug.LogError($"==@ Spin Button State: {BtnSpinDOWN()}");
    }
}



/// <summary>
/// 新游戏选线
/// </summary>
public partial class MachineSelectManager : MonoSingleton<MachineSelectManager>
{


    bool isGameLineSelect()
    {
        if (globalStore.nowGameID >= 3000 && globalStore.nowGameID <= 3999)
        {
            return true;
        }
        return false;
    }

    bool isGameApostarSelect()
    {
        if (globalStore.nowGameID >= 3000 && globalStore.nowGameID <= 3999)
        {
            return true;
        }
        return false;
    }
    public void ConfirmNewGameLineSelect()
    {
        if (BlackboardQueryUtils.IsSpin())
            return;

        GameObject Base = GameObject.Find("In Game Bottom/Anchor/Layout/Line");
        Base.GetComponent<ContextButton>().DoClick();
    }

    public void ConfirmNewGameApostarSelect()
    {
        if (BlackboardQueryUtils.IsSpin())
            return;

        GameObject Base = GameObject.Find("In Game Bottom/Anchor/Layout/Apostar");
        Base.GetComponent<ContextButton>().DoClick();
    }
}


/// <summary>
/// 自定义按钮
/// </summary>
public partial class MachineSelectManager : MonoSingleton<MachineSelectManager>
{
    GameCustomsButton gcb;
    public void SetCustomsButton(GameCustomsButton gcb)
    {
        GameCustomsButton.isEnableGameCustomsButton = true;
        this.gcb = gcb;
    }
    public void ClearCustomsButton(string mark = null)
    {
        if (mark == null)
        {
            GameCustomsButton.isEnableGameCustomsButton = false;
            this.gcb = null;

        }
        else if (this.gcb != null && this.gcb.mark == mark)
        {
            GameCustomsButton.isEnableGameCustomsButton = false;
            this.gcb = null;
        }
    }

    bool IsGameCustomsLightButton()
    {
        return this.gcb != null && this.gcb.btnType == GameCustomsButtonType.BtnLight;
    }

    bool IsGameCustomsButton()
    {
        if (!GameCustomsButton.isEnableGameCustomsButton && this.gcb != null)
        {
            this.gcb = null;
        }
        return this.gcb != null;
    }

    string ConfirmGameCustomsButton(string btnName = "BtnSpin_DOWN")
    {
        string bn = btnName.Replace("_DOWN", "").Replace("_UP", "");
        Dictionary<string, string> btns = this.gcb.GetCustomsButton();
        if (btns.ContainsKey(bn))
        {
            MessageDispatcher.Dispatch(EVTType.ON_MACHINE_BUTTON_EVENT, new EventData($"GameCustomsButton/{btnName}"));
            return btns[bn];
        }
        return null;
    }
}

public enum GameCustomsButtonType
{
    BtnAll,
    BtnLight,
    //BtnMark,
}

public class GameCustomsButton
{
    public static readonly string SOUND_DEFAULT = "UI_Button_Normal";

    public static readonly string MACHINE_BTN_EVENT = "MachineBtnEvent";

    public static bool isEnableGameCustomsButton;

    public GameCustomsButton() { }

    public GameCustomsButtonType btnType = GameCustomsButtonType.BtnAll;

    public string mark = "";

    public bool isShowBtn = true;

    public Dictionary<string, string> dicBtnAndSound = new Dictionary<string, string>()
    {

        ["BtnPre"] = SOUND_DEFAULT,
        ["BtnNext"] = SOUND_DEFAULT,
        ["BtnSpin"] = SOUND_DEFAULT,
        /*
        ["BtnExit"] = SOUND_DEFAULT,
        ["BtnSwitch"] = SOUND_DEFAULT,
        ["BtnBetUp"] = SOUND_DEFAULT,
        ["BtnBetDown"] = SOUND_DEFAULT,
        ["BtnBetMax"] = SOUND_DEFAULT,
        ["BtnHelp"] = SOUND_DEFAULT,
        */
    };
    public int numlightBtn = 0;

    public Dictionary<string, string> GetCustomsButton()
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        if (btnType == GameCustomsButtonType.BtnLight || !isShowBtn)
        {
            return dic;
        }
        else if (dicBtnAndSound != null && dicBtnAndSound.Count > 0)
        {
            foreach (KeyValuePair<string, string> item in dicBtnAndSound)
            {
                dic.Add(item.Key, item.Value);
            }
        }
        return dic;
    }
}
