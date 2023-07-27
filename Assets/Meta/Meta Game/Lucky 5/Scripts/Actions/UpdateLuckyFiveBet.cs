using System;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.LuckyFive;
using BagelCode.ClientModels;
using ParadoxNotion;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Lucky Five")]
    public class UpdateLuckyFiveBet : ActionTask<Blackboard> // todo remove
    {
        public BBParameter<bool> isEligible;

        private ContextElement agentElement;
        private ContextElement buttonElement;
        private ContextElement speechBalloonElement;
        private ContextElement speechBalloonTextElement;

        private Animator buttonAnimator;
        private Animator speechBallonAnimator;

        private long eligibleMinBet = 0;

        private bool isInit = false;
        private bool isPrevEligible = false;
        private bool isShowText = false;
        private bool isFirst = true;

        private float showTime = 3f;
        private float remainingShowTime = 0f;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private const string ENABLE_TEXT_KEY = "LUCKY_FIVE_ELIGIBLE_BET_ACTIVE";
        private const string DISABLE_TEXT_KEY = "LUCKY_FIVE_ELIGIBLE_BET_INACTIVE";

        protected override string info
        {
            get { return "Update Lucky Five Eligible"; }
        }

        protected override void OnExecute()
        {
            InitProperty();
            UpdateValues();
        }

        protected override void OnUpdate()
        {
            if(isInit)
            {
                if(isShowText)
                {
                    remainingShowTime -= Time.deltaTime;
                    if(remainingShowTime <= 0f)
                    {
                        isShowText = false;
                        speechBallonAnimator.SetBool("IsActive", false);
                        EndAction();
                    }
                }
                else
                {
                    EndAction();
                }
            }
        }

        private void InitProperty()
        {
            if(isInit) return;

            agentElement = agent.gameObject.GetComponent<ContextElement>();

            buttonElement = ContextUtils.FindElement(agentElement, "Lucky 5 Button", ContextSearchingType.ChildrenSearch);
            buttonAnimator = buttonElement.gameObject.GetComponent<Animator>();

            speechBalloonElement = ContextUtils.FindElement(agentElement, "Lucky 5 Info Speech Balloon", ContextSearchingType.ChildrenSearch);
            speechBallonAnimator = speechBalloonElement.gameObject.GetComponent<Animator>();

            speechBalloonTextElement = ContextUtils.FindElement(speechBalloonElement, "Text", ContextSearchingType.ChildrenSearch);

            var metaGameInfoBB = BlackboardQueryUtils.GetMetaGameEnterInfo();
            if(metaGameInfoBB != null)
                eligibleMinBet = metaGameInfoBB.GetValue<long>("eligibleMinBet");

            var betCredit = BlackboardUtils.FindVariable<long>("./betCredit");
            isPrevEligible = eligibleMinBet <= betCredit.value;

            MessageDispatcher.Register(MetaEventDefine.ON_META_UI_EVENT, OnMetaUIEvent);

            isInit = true;
        }

        private void OnMetaUIEvent(EventData eventData)
        {
            if(eventData.name == MetaEventDefine.ON_OPEN_META_EVENT_GROUP)
            {
                isShowText = false;
                speechBallonAnimator.SetBool("IsActive", false);
                EndAction();
            }
        }

        private void UpdateValues()
        {
            var betCredit = BlackboardUtils.FindVariable<long>("./betCredit");
            isEligible.value = eligibleMinBet <= betCredit.value;

            buttonAnimator.SetBool("IsBetMore", !isEligible.value);

            if(eligibleMinBet > 0 && (isFirst || isEligible.value != isPrevEligible) && !MetaGameUtils.IsMetaGameLevelLocked())
            {
                isShowText = true;
                remainingShowTime = showTime;
                speechBallonAnimator.SetBool("IsActive", true);

                if(isEligible.value)
                {
                    MetaContextElementUtils.SetText(speechBalloonTextElement, StringTableUtils.GetString(tableType, ENABLE_TEXT_KEY));
                }
                else
                {
                    MetaContextElementUtils.SetText(speechBalloonTextElement, StringTableUtils.GetString(tableType, DISABLE_TEXT_KEY));
                }

                isPrevEligible = isEligible.value;
            }

            isFirst = false;
        }
    }
}
