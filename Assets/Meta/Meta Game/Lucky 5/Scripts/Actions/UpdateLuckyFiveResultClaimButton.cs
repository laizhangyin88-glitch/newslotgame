using System;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.LuckyFive;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Lucky Five")]
    public class UpdateLuckyFiveResultClaimButton : ActionTask<Blackboard>
    {
        public BBParameter<Blackboard> responseBB;

        private ContextElement agentElement;

        protected override string info
        {
            get { return "Update Lucky Five Result Claim"; }
        }

        protected override void OnExecute()
        {
            agentElement = agent.gameObject.GetComponent<ContextElement>();

            UpdateClaim();

            EndAction();
        }

        private void UpdateClaim()
        {
            ContextElement unClaimeCoinTextElement = ContextUtils.FindElement(agentElement, "Unclaimed Wins Chart/Text Right", ContextSearchingType.FullNameSearch);
            ContextElement unClaimedButtonElement = ContextUtils.FindElement(agentElement, "Unclaimed Wins Chart/Button Claim", ContextSearchingType.FullNameSearch);
            ContextElement unClaimedButtonTextElement = ContextUtils.FindElement(agentElement, "Unclaimed Wins Chart/Button Claim/Text", ContextSearchingType.FullNameSearch);
            
            long unClaimeCoins = responseBB.value.GetValue<long>("unclaimedWin");

            if(unClaimeCoins == 0)
            {
                SetBooleanElement(unClaimedButtonElement, false);
                SetTextElementText(unClaimedButtonTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "LUCKY_FIVE_BUTTON_CLAIMED"));
            }
            else
            {
                SetBooleanElement(unClaimedButtonElement, true);
                SetTextElementText(unClaimedButtonTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "LUCKY_FIVE_BUTTON_CLAIM"));
            }

            SetTextElementText(unClaimeCoinTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "LUCKY_FIVE_RESULT_COIN_TEXT", unClaimeCoins));
        }

        private void SetTextElementText(ContextElement element, string text)
        {
            IContextText textElement = element as IContextText;
            if(textElement != null)
                textElement.SetText(text);
        }

        private void SetBooleanElement(ContextElement element, bool value)
        {
            IContextBooleanProperty property = element as IContextBooleanProperty;
            if(property != null)
            {
                property.SetBooleanProperty(value);
            }
            
        }
    }
}
