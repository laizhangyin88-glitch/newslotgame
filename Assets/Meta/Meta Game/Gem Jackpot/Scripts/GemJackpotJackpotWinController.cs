using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion;

namespace BagelCode.GemJackpot
{
    public class GemJackpotJackpotWinController : MonoBehaviour
    {
        public ContextElement textElement;
        public ContextElement buttonElement;

        private bool isInit = false;

        private void InitProperty()
        {
            if (isInit) return;

            ContextElement rootElement = GetComponent<ContextElement>();
            Blackboard rootBB = GetComponent<Blackboard>();

            rootElement.UpdateContext(true);

            textElement = ContextUtils.FindElement(rootElement, "Text", ContextSearchingType.ChildrenSearch);
            buttonElement = ContextUtils.FindElement(rootElement, "Button Collect", ContextSearchingType.ChildrenSearch);
            
            MetaContextElementUtils.SetClickable(
                buttonElement,
                "OnBonusClose",
                rootElement,
                null);

            isInit = true;
        }

        public void OnInit()
        {
            InitProperty();
            InitValue();
        }

        private void InitValue()
        {
            Blackboard bb = BlackboardUtils.FindValue<Blackboard>(GemJackpotUtils.GemJackpotInfo, "bonus");
            long credit = BlackboardUtils.FindValue<long>(bb, "credit");
            ContextUtils.SetText(textElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_COMMA_NUMBER", credit));
        }

        public void PlaySound()
        {
            string soundName = "Meta_Gemjackpot_Resultnormal";
            Blackboard bb = GemJackpotUtils.GemJackpotInfo;
            if (bb != null)
            {
                Blackboard bonusBB = BlackboardUtils.FindValue<Blackboard>(bb, "bonus");
                if (bonusBB != null)
                {
                    GemJackpotJackpotSymbolType type = BlackboardUtils.FindValue<GemJackpotJackpotSymbolType>(bonusBB, "type");

                    switch (type)
                    {
                        case GemJackpotJackpotSymbolType.MINI:
                            soundName = "Meta_Gemjackpot_ResultJackpot_Mini";
                            break;
                        case GemJackpotJackpotSymbolType.MINOR:
                            soundName = "Meta_Gemjackpot_ResultJackpot_Minor";
                            break;
                        case GemJackpotJackpotSymbolType.MAJOR:
                            soundName = "Meta_Gemjackpot_ResultJackpot_Major";
                            break;
                        case GemJackpotJackpotSymbolType.GRAND:
                            soundName = "Meta_Gemjackpot_ResultJackpot_Grand";
                            break;
                    }
                }
            }
            GSManager.Instance.GetHandler(soundName).Play();
        }

        public void OnCollectClick()
        {
            GSManager.Instance.GetHandler("Meta_Gemjackpot_Click").Play();
        }

        public void CloseSound()
        {
            GSManager.Instance.GetHandler("Meta_Gemjackpot_change").Play();
        }
    }
}