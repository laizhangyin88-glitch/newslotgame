using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.LuckyFive.Tasks.Actions
{
    [Category("★ BagelCode/LuckyFive")]
    public class UpdateLuckyFiveLobbyButton : ActionTask
    {
        private bool isInit = false;
        
        protected override string info
        {
            get { return string.Format("Update Lucky Five Lobby Button."); }
        }

        protected override void OnExecute()
        {
            InitProperty();

            EndAction();
        }

        private void InitProperty()
        {
            if(isInit) return;

            ContextElement agentElement = agent.gameObject.GetComponent<ContextElement>();
            agentElement.UpdateContext(false);

            MetaContextElementUtils.SetClickable(
                agentElement,
                "OnOpenLuckyFiveLoading",
                false,
                false,
                SendEvent,
                ownerSystem
            );

            isInit = true;
        }
    }
}

