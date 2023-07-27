using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class InitCollectingGameInGame : ActionTask<ContextElement>
    {
        public BBParameter<ContextElement> speechBalloonElement;
        public BBParameter<ContextElement> collectingGameButtonElement;
        public BBParameter<ContextElement> collectingGameBetProgressElement;
        public BBParameter<Transform> button;

        private GameObject badgeObj = null;
        private const string ON_OPEN_COLLECTING_GAME_LOADING_EVENT = "OnOpenCollectingGameLoading";
        
        protected override string info
        {
            get { return "Init Collecting Game In Game Buton"; }
        }

        protected override void OnExecute()
        {
            if(badgeObj != null)
                GameObject.Destroy(badgeObj);

            collectingGameButtonElement.value = ContextUtils.FindElement(agent, "Collecting Game Button", ContextSearchingType.ChildrenSearch);
            collectingGameButtonElement.value.GetComponent<Animator>().SetBool("IsActive", true);
            MetaContextElementUtils.SetClickable(
                collectingGameButtonElement.value,
                ON_OPEN_COLLECTING_GAME_LOADING_EVENT,
                true,
                false,
                SendEvent,
                ownerSystem
            );

            speechBalloonElement.value = ContextUtils.FindElement(agent, "Collecting Game Info Speech Balloon", ContextSearchingType.ChildrenSearch);

            collectingGameBetProgressElement.value = ContextUtils.FindElement(agent, "Collecting Game Bet Progress", ContextSearchingType.ChildrenSearch);
            
            collectingGameButtonElement.value.GetComponent<Animator>().SetBool("IsBadge", false);
            ContextElement badgeAreaElement = ContextUtils.FindElement(collectingGameButtonElement.value, "Badge Area", ContextSearchingType.ChildrenSearch);
            MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Badge", badgeAreaElement.transform);
            badgeAreaElement.UpdateContext(true);

            GameObject go = agent.gameObject;

            while (go.HasParent())
            {
                go = go.GetParent();
                if (go.name.Equals("In Game"))
                {
                    break;
                }
            }

            if (go.name.Equals("In Game"))
            {
                var inGameElement = go.GetComponent<ContextElement>();
                inGameElement.UpdateContext(true);
                
                ContextElement buttonElement = ContextUtils.FindElement(inGameElement, "In Game Bottom/Button Spin", ContextSearchingType.FullNameSearch);
                if (buttonElement == null)
                    buttonElement = ContextUtils.FindElement(inGameElement, "In Game Bottom/Button Deal", ContextSearchingType.FullNameSearch);

                if (buttonElement != null)
                {
                    button.value = buttonElement.transform;
                }
            }   
            
            ContextElement collectingGameIconElement = ContextUtils.FindElement(agent, "Collecting Game Button/Icon Area/Collecting Game Icon", ContextSearchingType.FullNameSearch);
            Animator iconAnimator = collectingGameIconElement.GetComponent<Animator>();
            float gaugeLevelAsFloat = BlackboardQueryUtils.GetCurrentGaugeLevelAsFloat();
            iconAnimator.SetBool("IsLocked", gaugeLevelAsFloat < 1f);
            
            EndAction();
        }
    }
}
