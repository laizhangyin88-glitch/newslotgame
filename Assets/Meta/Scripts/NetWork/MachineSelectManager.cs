

using BagelCode;
using SlotMaker;
using UnityEngine;

public class MachineSelectManager
{

    private static MachineSelectManager instance;
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


    int _freeGameSelectNumb = 0;





    public bool isGameConfigSelectPop()
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


    public bool isFreeGameTimeSelectPop()
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


    public bool isMiniGameSelectPop()
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


        return false;
    }





    public bool isMiniGamePop()
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



        /* GameObject gameContents = GameObject.Find("Game Canvas/Game Contents");

         bool isEffectMidground = false;

         bool isEffectForeground = false;

         if (gameContents != null)
         {
             Transform effectMidground = gameContents.transform.Find("Animator/Anchor/Effect Midground")?.GetComponent<Transform>();
             Transform effectForeground = gameContents.transform.Find("Animator/Anchor/Effect Foreground")?.GetComponent<Transform>();
             if (effectMidground != null)
             {
                 for (var i = 0; i < effectMidground.childCount; i++)
                 {
                     var chd = effectMidground.GetChild(i);
                     if (chd.gameObject.active) //  if (chd.gameObject.activeSelf ) 
                     {
                         isEffectMidground = true;
                         break;
                     }
                 }
             }
             if (effectForeground != null)
             {
                 for (var i = 0; i < effectForeground.childCount; i++)
                 {
                     var chd = effectForeground.GetChild(i);
                     if (chd.gameObject.active) //  if (chd.gameObject.activeSelf ) 
                     {
                         isEffectForeground = true;
                         break;
                     }
                 }
             }
         }*/

        /* 可以：
        bool isEffectMidground = false;
        bool isEffectForeground = false;
        Transform effectMidground = GameObject.Find("Game Contents/Animator/Anchor/Effect Midground")?.GetComponent<Transform>();
        Transform effectForeground = GameObject.Find("Game Contents/Animator/Anchor/Effect Foreground")?.GetComponent<Transform>();
        //Transform effectMidground = GameObject.Find("Game Canvas/Game Contents/Animator/Anchor/Effect Midground")?.GetComponent<Transform>();
        //Transform effectForeground = GameObject.Find("Game Canvas/Game Contents/Animator/Anchor/Effect Foreground")?.GetComponent<Transform>();
        if (effectMidground != null)
        {
            for (var i = 0; i < effectMidground.childCount; i++)
            {
                var chd = effectMidground.GetChild(i);
                if (chd.gameObject.active) //  if (chd.gameObject.activeSelf ) 
                {
                    isEffectMidground = true;
                    break;
                }
            }
        }
        if (effectForeground != null)
        {
            for (var i = 0; i < effectForeground.childCount; i++)
            {
                var chd = effectForeground.GetChild(i);
                if (chd.gameObject.active) //  if (chd.gameObject.activeSelf ) 
                {
                    isEffectForeground = true;
                    break;
                }
            }
        }*/

        // return isEffectMidground || isEffectForeground;

    }



    public void PreviousSelectItem()
    {
        //GameObject[] selectLst =  GameObject.FindGameObjectsWithTag("MachineSelect"); //对个对象，却只能找到一个对象
        MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();

        MachineSelectBorder compLast = null;
        int index = -1;
        bool isFind = false;


        for (int i = 0; i < comps.Length; i++)
        {
            if (comps[i].selectBorder.active)
            {
                index = comps[i].index;
                break;
            }
        }
        index--;
        for (int i = 0; i < comps.Length; i++)
        {
            comps[i].selectBorder.SetActive(false);
            if (comps[i].index == comps.Length - 1)
            {
                compLast = comps[i];
            }
            if (comps[i].index == index)
            {
                comps[i].selectBorder.SetActive(true);
                _freeGameSelectNumb = index;
                isFind = true;
            }
        }

        if (!isFind && compLast != null)
        {
            compLast.selectBorder.SetActive(true);
        }
    }
    public void NextSelectItem()
    {
        MachineSelectBorder[] comps = GameObject.FindObjectsOfType<MachineSelectBorder>();

        MachineSelectBorder compFirst = null;
        int index = -1;
        bool isFind = false;

        for (int i = 0; i < comps.Length; i++)
        {
            if (comps[i].selectBorder.active)
            {
                index = comps[i].index;
                break;
            }
        }

        index++;
        for (int i = 0; i < comps.Length; i++)
        {
            comps[i].selectBorder.SetActive(false);
            if (comps[i].index == 0)
            {
                compFirst = comps[i];
            }
            if (comps[i].index == index)
            {
                comps[i].selectBorder.SetActive(true);
                _freeGameSelectNumb = index;
                isFind = true;
            }
        }

        if (!isFind && compFirst != null)
        {
            compFirst.selectBorder.SetActive(true);
        }
    }


    public void ConfirmFreeGameSelectItem()
    {
        string name = "";
        if (globalStore.nowGameID == 37)
        {
            switch (_freeGameSelectNumb)
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
            //Debug.Log($"EVT = OnSelection{_freeGameSelectNumb}");
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData(name));
            _freeGameSelectNumb = 0;
        }

        if (globalStore.nowGameID == 93)
        {
            name = $"OnSelection{_freeGameSelectNumb}";
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData(name));
            _freeGameSelectNumb = 0;
        }


        if (globalStore.nowGameID == 105)
        {
            switch (_freeGameSelectNumb)
            {
                case 0:
                    name = "SelectFreeSpin";
                    break;
                case 1:
                    name = "SelectLinkBonus";
                    break;
            }
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData(name));
            _freeGameSelectNumb = 0;
        }


        if (globalStore.nowGameID == 116) //魔术师 - 选牌
        {
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData<int>("MachineSelectEvent", _freeGameSelectNumb));
            _freeGameSelectNumb = 0;
        }

        if (globalStore.nowGameID == 149) //白虎 
        {
            name = $"OnSelection{_freeGameSelectNumb}";
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData(name));
            _freeGameSelectNumb = 0;
        }

        Debug.Log($"【machine】: free game select {name}    {_freeGameSelectNumb}");

    }
    public void ConfirmMiniGameSelectItem()
    {
        string name = "";
        if (globalStore.nowGameID == 92) // 小猪选金币（多选）
        {
            name = "MachineSelectEvent";
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData<int>(name, _freeGameSelectNumb));
        }


        if (globalStore.nowGameID == 116) //魔术师 - 魔术帽（多选）
        {
            name = "MachineSelectEvent";
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData<int>(name, _freeGameSelectNumb));
        }



        if (globalStore.nowGameID == 183) //辣椒 - 滚轮滑动界面
        {
            name = "OnBigWheelClick";
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData<int>(name, _freeGameSelectNumb));
        }



        Debug.Log($"【machine】: min game select {name}");

    }



    public void ConfirmMiniGame()
    {

        string name = "";
        if (globalStore.nowGameID == 183) //辣椒 - 滚轮滑动界面
        {
            name = "OnBigWheelClick";
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData<int>(name, _freeGameSelectNumb));
        }
        else
        {
            name = "MachineSpinClick";
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData("MachineSpinClick"));
        }
        Debug.Log($"【machine】: spin click send  {name}  {_freeGameSelectNumb}");
    }




    public void ConfirmGameConfigSelectPop()
    {
        if (globalStore.nowGameID == 142) //GOLDEN_PICTURES
        {
            //GameObject Pick = GameObject.Find("Popup Manager/Contents/Denomination Popup FIJ");

            GameObject choosePannel = GameObject.Find($"Denomination Popup FIJ/Animator/Anchor/Popup Base/Choose pannel {_freeGameSelectNumb + 1}");


            if (choosePannel != null)
            {
                choosePannel?.GetComponent<SlotMaker.Extentions.ActionListPlayer>().Play();
            }
        }

        Debug.Log($"【machine】: game config {_freeGameSelectNumb}");

    }
}
