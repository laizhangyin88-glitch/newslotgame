using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

    [Category("★ BagelCode/Lucky Spin")]

    public class InitLuckySpinsScene : ActionTask <ContextElement>
    {
        public BBParameter<int> tierGroup;
        public BBParameter<ContextElement> animator;
        public BBParameter<ContextElement> informationBalloon;
        public BBParameter<ContextElement> coinText;
        public BBParameter<ContextElement> buttonCoin;
        public BBParameter<ContextElement> buttonCoinCover;
        public BBParameter<ContextElement> buttonPlay;
        public BBParameter<ContextElement> particleCollect;
        
        private string ON_ENTER_GAME_SPIN_EVENT = "OnEnterGameSpin";
        private string ON_COLLECT_COIN_EVENT = "onCollectCoin";
        
        protected override string info
        { 
            get 
            { 
                return string.Format("Init Lucky Spins Scene");
            } 
        }
        protected override void OnExecute()
        {
            ContextElement buttonPlayElement = ContextUtils.FindElement(agent, "Animator/Result/Button Play", ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SetClickable(
                buttonPlayElement,
                ON_ENTER_GAME_SPIN_EVENT,
                false,
                true,
                SendEvent,
                ownerSystem
            );
            buttonPlay.value = buttonPlayElement;

            ContextElement buttonPlayTextElement = ContextUtils.FindElement(buttonPlayElement, "Text", ContextSearchingType.ChildrenSearch);
            string buttonPlayText = StringTableUtils.GetString(StringTable.StringTableType.Global, "LUCKY_SPIN_PLAY_TEXT");
            MetaContextElementUtils.SetText(buttonPlayTextElement, buttonPlayText);
            
            
            ContextElement resultTextElement = ContextUtils.FindElement(agent, "Animator/Result/Text", ContextSearchingType.FullNameSearch);
            string luckySpinResultText = StringTableUtils.GetString(StringTable.StringTableType.Global, "LUCKY_SPIN_RESULT_TEXT");
            MetaContextElementUtils.SetText(resultTextElement, luckySpinResultText);
            
            var tierValue = BlackboardUtils.FindVariable<int>(null, "/me/tier");

            if (tierValue != null)
                tierGroup.value = MetaSystem.GetTierGroup(tierValue.value);

            ContextElement animatorElement = ContextUtils.FindElement(agent, "Animator", ContextSearchingType.ChildrenSearch);
            animator.value = animatorElement;

            ContextElement buttonCoinElement = ContextUtils.FindElement(animatorElement, "Result/Button Coin", ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SetClickable(
                buttonCoinElement,
                ON_COLLECT_COIN_EVENT,
                false,
                false,
                SendEvent,
                ownerSystem
            );
            buttonCoin.value = buttonCoinElement;
            
            ContextElement buttonCoinTextElement = ContextUtils.FindElement(buttonCoinElement, "Text 02", ContextSearchingType.ChildrenSearch);
            coinText.value = buttonCoinTextElement;
            
            ContextElement buttonCoinCoverElement = ContextUtils.FindElement(buttonCoinElement, "Base Gray", ContextSearchingType.ChildrenSearch);
            buttonCoinCover.value = buttonCoinCoverElement;
            
            ContextElement buttonCoinParticleElement = ContextUtils.FindElement(buttonCoinElement, "Particle Collect", ContextSearchingType.ChildrenSearch);
            particleCollect.value = buttonCoinParticleElement;

            ContextElement informationAnchorElement = ContextUtils.FindElement(animatorElement, "Result/Button Coin/Information/Anchor", ContextSearchingType.FullNameSearch);
            MetaObjectUtils.MakePrefab(
                MetaStringDefine.LOBBY_BUNDLE_NAME,
                "Speech Balloon Bottom Right",
                informationAnchorElement.transform
            );
            
            informationAnchorElement.UpdateContext(true);

            ContextElement informationElement = ContextUtils.FindElement(animatorElement, "Result/Button Coin/Information", ContextSearchingType.FullNameSearch);
            informationBalloon.value = informationElement;
            
            ContextElement informationTextElement = ContextUtils.FindElement(informationAnchorElement, "Speech Balloon Bottom Right/Text", ContextSearchingType.FullNameSearch);
            string informationText = StringTableUtils.GetString(StringTable.StringTableType.Global, "LUCKY_SPIN_COLLECT_CREDIT_BALLOON");
            MetaContextElementUtils.SetText(informationTextElement, informationText);
        
            EndAction();
        }
    }

}
