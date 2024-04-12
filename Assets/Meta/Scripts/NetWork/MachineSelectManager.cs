

using BagelCode;
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
        return false;
    }

    public bool isMinGamePop()
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

        if (globalStore.nowGameID == 93)
        {
            return false;
        }


        if (globalStore.nowGameID == 28)
        {
            return false;
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

        Debug.Log($"【machine】: free game select {name}");

    }

}
