

using BagelCode;
using BagelCode.OSA_Scroll;
//using Boo.Lang;
using ParadoxNotion;
using PlayFab.ClientModels;
using Sirenix.OdinInspector;
using SlotMaker;
using SlotMaker.Cards;
using SlotMaker.Slots.Tasks.Actions.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

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

public class MachineSelectManager : MonoSingleton<MachineSelectManager>
{

    private void Start()
    {
        MessageDispatcher.Register(MetaEventDefine.ON_META_UI_EVENT, OnChangeGameListShowMode);
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
        base.OnDestroy();
    }
    private void OnChangeGameListShowMode(ParadoxNotion.EventData eventData)
    {
        //Debug.LogError("OnChangeGameListShowMode   001");
        if (eventData.name != "OnChangeGameListShowMode") return;

        //Debug.LogError("OnChangeGameListShowMode   002");
        task = () =>  //延时，避免   OSA_LobbySlots.ResetCurSelect() 影响
        {
            ReflashHallBtnRegion();
        };
    }

    System.Action task;

    private void Update()
    {
        if (task != null)
        {
            task();
            task = null;
        }
    }


    /// <summary>心跳定时器</summary>
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
    public void test_ShowInfo()
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
        {
            markStack = hallMarkStack;
        }
        else
        {
            markStack = gameMarkStack;
        }
        markStack.Add(new MarkInfo(name, index, type));
    }


    /* private int lastGameID = -1;

     private void ResetMark()
     {
         if (lastGameID != globalStore.nowGameID)
          {
              lastGameID = globalStore.nowGameID;

              if (globalStore.nowGameID == -1) //在大厅
              {
                  _curSelectMark = hallMarkStack[0].name;
                  _curSelectNumb = hallMarkStack[0].index;
              }
              else //在游戏时
              {
                  _curSelectMark = gameMarkStack[0].name;
                  _curSelectNumb = gameMarkStack[0].index;
              }
          }
     }*/



    private void ResetMarkStack()
    {
        if (globalStore.nowGameID == -1) //在大厅时复位游戏 mark
        {
            gameMarkStack = new List<MarkInfo> { new MarkInfo("", 0, MarkType.Game) };
        }
        else //在游戏时复位大厅 mark
        {
            hallMarkStack = new List<MarkInfo> { hallMarkStack[0] };
        }
    }
    private void ClearMark()
    {
        List<string> markLst = new List<string>();

        MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();

        for (int i = 0; i < comps.Length; i++)
        {
            if (comps[i].transform.parent.gameObject.active && !markLst.Contains(comps[i].mark))
            {
                markLst.Add(comps[i].mark);
            }
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
            {
                markStack.RemoveAt(j);
            }
            else
            {
                j++;
            }
        }

        ResetMarkStack();
    }
    private void RemoveMark(string removeMark)
    {

        ClearMark();

        List<MarkInfo> markStack;

        if (globalStore.nowGameID == -1)
        {
            markStack = hallMarkStack;
        }
        else
        {
            markStack = gameMarkStack;
        }

        if (markStack.Count > 1 && markStack[markStack.Count - 1].name == removeMark)
        {
            markStack.RemoveAt(markStack.Count - 1);
        }

        MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();
        MarkInfo mk = markStack[markStack.Count - 1];

        for (int i = 0; i < comps.Length; i++)
        {
            if (comps[i].index == mk.index && comps[i].mark == mk.name)
            {
                //comps[i].selectBorder.SetActive(true);
                comps[i].isSelected = true;
            }
            else
            {
                //comps[i].selectBorder.SetActive(false);
                comps[i].isSelected = false;
            }
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
            }
            GameObject Base = GameObject.Find("Lobby/Anchor/Lobby Pages/Anchor/Slots Area/ScrollView Slots");
            if (Base != null && Base.active)
            {
                if (hallMarkStack[0].name == "")
                {
                    Base.GetComponent<OSA_LobbySlots>().SetCurSelect(hallMarkStack[0].index);
                }
                else
                {
                    Base.GetComponent<OSA_LobbySlots>().SetCurSelect(-1);
                }
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
            }
        }

    }


    /// <summary>“游戏配置弹窗”是否可见 </summary>
    public bool isPopGameConfigSelect()
    {

        if (globalStore.nowGameID == 142) //GOLDEN_PICTURES
        {
            GameObject Pick = GameObject.Find("Popup Manager/Contents/Denomination Popup FIJ");

            if (Pick != null && Pick.active)
            {
                return true;
            }
        }
        return false;
    }



    public void ConfirmPopGameConfigSelect()
    {
        sound = null;
        if (globalStore.nowGameID == 142) //GOLDEN_PICTURES
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


    /// <summary>“免费游戏选择弹窗”是否可见 </summary>
    public bool isPopFreeGameTimeSelect()
    {

        if (globalStore.nowGameID == 37) //GOLDEN_PICTURES
        {
            GameObject Pick = GameObject.Find("Game Canvas/Game Contents/Animator/Anchor/Effect Midground/Pick Bonus/Animator/Base/Base/Pick");

            if (Pick != null && Pick.active)
            {
                return true;
            }
        }

        if (globalStore.nowGameID == 93) //HAPPY_DOLLARS
        {
            GameObject Pick = GameObject.Find("Popup Manager/Contents/Free Game Select Popup");
            if (Pick != null && Pick.active)
            {
                return true;
            }
        }

        if (globalStore.nowGameID == 105)
        {
            GameObject Pick = GameObject.Find("Popup Manager/Contents/Select a Feature Popup");
            if (Pick != null && Pick.active)
            {
                return true;
            }
        }

        if (globalStore.nowGameID == 116) //魔术师 - 选牌
        {
            GameObject Pick = GameObject.Find("Popup Manager/Contents/Bonus Trigger Popup");
            if (Pick != null && Pick.active)
            {
                return true;
            }
        }

        if (globalStore.nowGameID == 149) //白虎 - 选免费游戏
        {
            GameObject Pick = GameObject.Find("Popup Manager/Contents/Free Game Select Popup");
            if (Pick != null && Pick.active)
            {
                return true;
            }
        }
        return false;
    }

    public void ConfirmPopFreeGameSelect()
    {
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


        ///
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Collect"));
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnClose"));
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnCollect"));
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnPointerClick"));


        /**
         * Big Win Text Event Mega Win
         * Big Win Text Event Super Mega Win
         * Big Win Text Event Big Win
         */
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("MachineSpinClick"));

        // free game start
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnStartFreeSpin")); // 多数游戏免费游戏的开始提示弹窗

        //ID:39  mini game start
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnStartBigWheel"));//Big Wheel Trigger Popup
                                                                                       //EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Collect"));//Big Wheel Result Popup


        //ID:21 mini game result
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("collectEvent")); //


        //ID:93 mini game result
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Clicked")); //Free Game Select Popup（免费游戏结算确认界面）


        //ID:40
        //EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnStartFreeSpin"));//Free Game Trigger Popup
        //EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Collect"));//Free Game Result Popup


        //ID:37
        //EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Collect")); //Free Game Result Popup（免费游戏结算确认界面）

        //ID:142  mini game result
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnFinishBonus")); //Double Jackpot Major Popup FIJ（免费游戏结算确认界面）


        //ID:149 - 白虎  mini game result
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnButtonClicked")); //Double Jackpot Major Popup FIJ（免费游戏开始界面）


        //ID:144 - 狮子  mini game result
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnClickButton")); //免费游戏开始界面、免费游戏结算界面



        // ID:103 -  奖励弹窗
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnTicketRegularWin")); // 奖励弹窗
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnBonusGameStart"));  // mini game start
        EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnBackClicked"));  // mini game result


    }


    /// <summary>“小游戏选择弹窗”是否可见 </summary>
    public bool isPopMiniGameSelect()
    {

        if (globalStore.nowGameID == 100003) //Jack
        {

            GameObject Base = GameObject.Find("Popup Manager/Contents/Red Or Black");

            if (Base != null && Base.active)
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

        return false;
    }


    public void ConfirmNodeMiniGameSelect()
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
                    _AddTack(() => {
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
    public bool isNodeMiniGame()
    {


        if (globalStore.nowGameID == 21)
        {
            GameObject Base = GameObject.Find("Game Canvas/Game Contents/Animator/Anchor/Effect Midground/Wheel Bonus");

            if (Base != null && Base.active)
            {
                return true;
            }
        }


        if (globalStore.nowGameID == 7)
        {
            GameObject Base = GameObject.Find("Game Contents/Animator/Anchor/Effect Midground/Dice Game");

            if (Base != null && Base.active)
            {
                return true;
            }

        }

        if (globalStore.nowGameID == 37)
        {
            GameObject Base = GameObject.Find("Game Canvas/Game Contents/Animator/Anchor/Effect Midground/Pick Bonus/Animator/Base");

            if (Base != null && Base.active)
            {
                return true;
            }
        }

        if (globalStore.nowGameID == 183) //辣椒 - 滚轮滑动界面
        {
            GameObject Base = GameObject.Find("Game Canvas/Game Contents/Animator/Anchor/Base/Slot Frame/Wheel Bonus");

            if (Base != null && Base.active)
            {
                return true;
            }
        }


        return false;


    }


    public void ConfirmNodeMiniGameSpin()
    {

        string name = "";
        if (globalStore.nowGameID == 183) //辣椒 - 滚轮滑动界面
        {
            name = "OnBigWheelClick";
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData<int>(name, _curSelectNumb));
        }
        else
        {
            name = "MachineSpinClick";
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData("MachineSpinClick"));
        }
        Debug.Log($"【machine】: mini game spin click  {name}  {_curSelectNumb}");
    }


    public bool isSpecialButtonShow()
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
    }





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


    /*Dictionary<int, string[]> excludeButtonRegions = new Dictionary<int, string[]>
    {
        { -1, new string[] {"LobbyCategory" } },
    };*/

    Dictionary<int, string[]> buttonRegions = new Dictionary<int, string[]>
    {
        //{ -1, new string[] { "", "LobbyCategory" } },
        { -1, new string[] {"",} },
        { 200002,new string[] { "SPIN", "SideUI", "ButtonUI", "Spot", }},
        { 100003, new string[] { "SPIN", "JacksGamble", "JacksHands", "JacksCard" }},
        { 43, new string[] { "SPIN", "GameGonfig", }}
    };


    public List<string> GetButtonRegionLst()
    {
        List<string> marks = null;
        /* if (globalStore.nowGameID == -1)  // 大厅
         {
             marks = new List<string> { "", "LobbyCategory" };
         }

         if (globalStore.nowGameID == 200002)
         {
             marks = new List<string> { "SPIN", "SideUI", "ButtonUI", "Spot", };
         }

         if (globalStore.nowGameID == 100003) //jacks
         {
             marks = new List<string> { "SPIN", "JacksGamble", "JacksHands", "JacksCard" }; //"JacksGamble"按钮可能隐藏
         }

         if (globalStore.nowGameID == 43)
         {
             marks = new List<string> { "SPIN", "GameGonfig", };
         }*/

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
    public void ConfirmChangeButtonRegionUp()
    {

        List<string> marks = GetVisableButtonRegionLst();
        /*List<string> marks = GetButtonRegionLst();

        MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();

        //去掉隐藏的 MachineSelectBorder
        List<string> visibleMarks = globalStore.nowGameID == -1? new List<string>() {""} : new List<string>();

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
        }*/
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
        _curSelectMark = marks[i];
        _curSelectNumb = 0;


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
        }
    }

    /*
    public void ConfirmChangeButtonRegionDown()
    {


        List<string> marks = null;
        if (globalStore.nowGameID == -1)  // 大厅
        {
            marks = new List<string>{ "", "LobbyCategory" };
        }

        if (globalStore.nowGameID == 200002) 
        {
            marks = new List<string> { "SideUI" ,"ButtonUI","Spot",};
        }


        int i = 0;
        for (; i < marks.Count; i++)
        {
            if (_curSelectMark == marks[i])
            {
                break;
            }
        }

        i--;

        if (i < 0)
        {
            i = marks.Count - 1;
        }
        _curSelectMark = marks[i];
        _curSelectNumb = 0;



        MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();

        //多个页面，都有MachineSelectBorder组件的情况下
        foreach (var item in comps)
        {
            if (item.index == 0 && item.mark == _curSelectMark)
            {
                item.selectBorder.SetActive(true);
            }
            else
            {
                item.selectBorder.SetActive(false);
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
        }

    }
    */




    public bool isMenuOpen()
    {
        if (globalStore.nowGameID == -1)  // 大厅
        {
            GameObject Base = GameObject.Find("Lobby/Anchor/Navigation Bar/Anchor/Layout/Right/Menu/Dropdown Menu/Anchor");

            if (Base != null && Base.active)
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

            if (Base != null && Base.active)
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

        if (Base != null && Base.active)
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

        if (Base != null && Base.active)
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
        }

        setMark(_curSelectMark, _curSelectNumb);
    }
    public void NextSelectItem()
    {
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
        for (int i = 0; i < comps.Length; i++)
        {
            //comps[i].selectBorder.SetActive(false);
            comps[i].isSelected = false;
            if (comps[i].index == 0)
            {
                compFirst = comps[i];
            }
            if (comps[i].index == index)
            {
                comps[i].isSelected = true;
                //comps[i].selectBorder.SetActive(true);
                _curSelectNumb = index;
                isFind = true;
            }
        }

        if (!isFind && compFirst != null)
        {
            //_curSelectNumb = compFirst.index;
            _curSelectNumb = 0;
            // compFirst.selectBorder.SetActive(true);
            compFirst.isSelected = true;
        }

        setMark(_curSelectMark, _curSelectNumb);
    }


    public void PurchaseCreditRequest(int operateType, long purchase)
    {
        string rpcName = operateType == 1 ? RPCName.agentRechargeToDeviceUser : RPCName.decreaseDeviceCredit;
        Dictionary<string, object> req = new Dictionary<string, object>
        {
            {"balance",purchase},
        };

        NetManager.Instance.Post(rpcName, req,
        (res) =>
        {
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
        });
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


    public void BtnSpinDown()
    {
#if !UNITY_EDITOR
        if (!ApplicationSettings.Instance.isMachine)
            return;
#endif

        if (isPopSysSettingSelect())
        {
            ConfirmSysSettingSelect();
        }
        else if (isPopFreeGameTimeSelect())
        {
            ConfirmPopFreeGameSelect();
        }
        else if (isPopGameConfigSelect())
        {
            ConfirmPopGameConfigSelect();
        }
        else if (isPopMiniGameSelect())
        {
            ConfirmPopMiniGameSelect();
        }
        else if (isPopCommon())
        {
            ConfirmPopCommon();
        }

        else if (isMenuOpen())
        {
            ConfirmMenuSelect();
        }
        else if (isHallGameCategorySelect())
        {
            ConfirmHallGameCategorySelect();
        }

        else if (isNodeMiniGameSelect())
        {
            ConfirmNodeMiniGameSelect();
        }
        else if (isNodeMiniGame())
        {
            ConfirmNodeMiniGameSpin();
        }
        else if (isNodeGameButtonSelect())
        {
            ConfirmNodeGameButtonSelect();
        }
        else if (isIgnoreSpin())
        {
            // 忽略spin
            sound = null;
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
        }

        PlaySound();
    }



    public bool isIgnoreSpin()
    {
        //在游戏中，且多个按钮选择区域，且当前没有其他节点，且没选中SPIN
        return BlackboardQueryUtils.IsIngame()
            && isChangeButtonRegion()
            && gameMarkStack.Count == 1
            && _curSelectMark != "SPIN";
    }

    public void BtnSpinUp()
    {
#if !UNITY_EDITOR
        if (!ApplicationSettings.Instance.isMachine)
            return;
#endif
        if (isIgnoreSpin())
        {
            // 忽略spin
            sound = null;
        }
        else //if (BlackboardQueryUtils.IsIngame()  && !isNodeGameButtonSelect())
        {
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData<int>(MachineEventDefine.ON_KEY_START, 0));
        }
    }
    /**
     *       //(int)args[1] 0: 抬起
            //(int)args[1] 1: 按下
    */



    public void BtnReturn()
    {
#if !UNITY_EDITOR
        if (!ApplicationSettings.Instance.isMachine)
            return;
#endif
        if (isPopCommon())  //关闭所有弹窗
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


    public void BtnSwitch()
    {
#if !UNITY_EDITOR
        if (!ApplicationSettings.Instance.isMachine)
            return;
#endif

        if (!isMenuOpen()
            && !isPopCommon()
               //&& !isPopSysSettingSelect()
               && isChangeButtonRegion())
        {
            ConfirmChangeButtonRegionUp();
        }
        PlaySound();
    }


    public void BtnHelp()
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
        PlaySound();
    }
    public void BtnMenu()
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

    public void BtnNext()
    {
#if !UNITY_EDITOR
        if (!ApplicationSettings.Instance.isMachine)
            return;
#endif

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
        else if (isPopFreeGameTimeSelect()
            || isNodeMiniGameSelect()
            || isPopGameConfigSelect()
            || isPopMiniGameSelect())
        {
            NextSelectItem();
        }
        else if (isNodeGameButtonSelect())
        {
            NextSelectItem();
        }
        else if (globalStore.nowGameID == -1 && _curSelectMark != "")
        {
            NextSelectItem();
        }
        else
        {
            sound = null;
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_RIGHT));
        }

        PlaySound();
    }

    public void BtnPre()
    {
#if !UNITY_EDITOR
        if (!ApplicationSettings.Instance.isMachine)
            return;
#endif

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
        else if (isPopFreeGameTimeSelect()
            || isNodeMiniGameSelect()
            || isPopGameConfigSelect()
            || isPopMiniGameSelect())
        {
            PreviousSelectItem();
        }
        else if (isNodeGameButtonSelect())
        {
            PreviousSelectItem();
        }
        else if (globalStore.nowGameID == -1 && _curSelectMark != "")
        {
            PreviousSelectItem();
        }
        else
        {
            sound = null;
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_LEFT));
        }

        PlaySound();
    }

    public void BtnAddCoin()
    {
#if !UNITY_EDITOR
        if (!ApplicationSettings.Instance.isMachine)
            return;
#endif
        PurchaseCreditRequest(1, 100);//加分

    }

    public void BtnMinusCoin()
    {
#if !UNITY_EDITOR
        if (!ApplicationSettings.Instance.isMachine)
            return;
#endif

        PurchaseCreditRequest(2, 100); //减分
    }



    public void BtnBetMax()
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
        else
        {
            sound = null;
        }
        PlaySound();
    }
    /**
     * 
     * */
    public void BtnBetUp()
    {
#if !UNITY_EDITOR
        if (!ApplicationSettings.Instance.isMachine)
            return;
#endif

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
    public void BtnBetDown()
    {
#if !UNITY_EDITOR
        if (!ApplicationSettings.Instance.isMachine)
            return;
#endif

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
        else
        {
            sound = null;
        }
        //炮左移
        //EventSender.SendGlobalEvent(MetaEventDefine.ON_MACHINE, new EventData<int>(MachineEventDefine.ON_KEY_LEFT, data));
        PlaySound();
    }
}
