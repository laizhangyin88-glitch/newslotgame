using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using SlotMaker.Keno;
using SlotMaker;
using ParadoxNotion;
using NodeCanvas.Framework;
using BagelCode;

public class CustomSelectButton_ID7 : MonoBehaviour
{
    [SerializeField] private Dictionary<string, string> _registerBtnDict;
    private GameCustomsButton GameCustomsButton;


    private void OnEnable()
    {
        GameCustomsButton = new GameCustomsButton()
        {
            mark = "LuckyRain",
            btnType = GameCustomsButtonType.BtnAll,
            dicBtnAndSound = new Dictionary<string, string>()
            {
                ["BtnPre"] = GameCustomsButton.SOUND_DEFAULT,
                ["BtnNext"] = GameCustomsButton.SOUND_DEFAULT,
                ["BtnSpin"] = GameCustomsButton.SOUND_DEFAULT,
            }
        };

        MachineSelectManager.Instance.SetCustomsButton(GameCustomsButton);
        MessageDispatcher.Register(EVTType.ON_MACHINE_BUTTON_EVENT, OnMachineButtonEvent);
    }

    private void OnDisable()
    {
        MachineSelectManager.Instance.ClearCustomsButton("LuckyRain");
        MessageDispatcher.UnRegister(EVTType.ON_MACHINE_BUTTON_EVENT, OnMachineButtonEvent);
    }

    public void OnMachineButtonEvent(EventData eventData)
    {
        if (eventData.name.StartsWith("GameCustomsButton/"))
        {
            Debug.LogError(eventData.name);
            string btnName = eventData.name.Replace("GameCustomsButton/", "");
            switch (btnName)
            {
                case "BtnPre_DOWN":
                    EventData<int> ed = new EventData<int>("OnSelectObj", 1);
                    //Graph.SendGlobalEvent(ed, this);
                    //MessageDispatcher.Dispatch("OnCustomEvent", ed);
                    EventSender.SendGlobalEvent("OnCustomEvent", ed);
                    //BlackboardUtils.SetOrCreateValue<int>(null, "./selectObjIndex", 1);
                    break;
                case "BtnNext_DOWN":
                    //EventData<int> ed2 = new EventData<int>("OnSelectObj", 2);
                    //Graph.SendGlobalEvent(ed, this);
                    EventData<int> ed2 = new EventData<int>("OnSelectObj", 2);
                    //Graph.SendGlobalEvent(ed, this);
                    //MessageDispatcher.Dispatch("OnCustomEvent", ed2);
                    EventSender.SendGlobalEvent("OnCustomEvent", ed2);
                    //BlackboardUtils.SetOrCreateValue<int>(null, "./selectObjIndex", 2);
                    break;
                case "BtnPre_UP":
                case "BtnNext_UP":
                    break;
                case "BtnSpin_DOWN":
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("MachineSpinClick"));
                    break;
            }
        }
    }
}
