using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NodeCanvas.Framework;
using BagelCode.OSA_Scroll;

namespace BagelCode.BossRaiders
{
    public class BossRaidersPopupRaidConquerors : MonoBehaviour
    {
        public GameObject caller;

        private ContextElement rootElement;

        private ContextElement titleTextElement;
        private OSA_BossRaidersRaidConquerors osaScrollContrller;

        public void OnInit()
        {
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            ContextElement titleAreaElement = ContextUtils.FindElement(rootElement, "Title Area", ContextSearchingType.ChildrenSearch);
            titleTextElement = ContextUtils.FindElement(titleAreaElement, "Text", ContextSearchingType.ChildrenSearch);

            ContextElement buttonCloseElement = ContextUtils.FindElement(rootElement, "Button Close", ContextSearchingType.ChildrenSearch);
            ContextElement osaScrollElement = ContextUtils.FindElement(rootElement, "Raid Conquerors Contents Area", ContextSearchingType.ChildrenSearch);
            osaScrollContrller = osaScrollElement.GetComponent<OSA_BossRaidersRaidConquerors>();
            osaScrollContrller.SetCaller(rootElement);

            MetaContextElementUtils.SetClickable(
                buttonCloseElement,
                "OnClose",
                rootElement,
                null
            );

            InitTexts();
        }

        private void InitTexts()
        {
            MetaContextElementUtils.SetTextGlobal(titleTextElement, "BOSS_RAIDERS_POPUP_RAID_CONQUERORS_TEXT");
        }

        public void SendUserProfile(string userId)
        {
            MetaContextElementUtils.SendEvent(rootElement, "OnClickClubProfile", userId, caller.GetComponent<ContextElement>(), null);
        }
    }
}