using UnityEngine;
using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Challenge")]
    public class ToggleChallengeEventTag : ActionTask<Blackboard>
    {
        protected override string info => "EventTagElement activate when not isLocked";

        protected override void OnExecute()
        {
            var eventTagElement = BlackboardUtils.FindValue<ContextElement>(agent, "eventObject");
            var rootElement = BlackboardUtils.FindValue<ContextElement>(agent, "rootElement");
            bool isLocked = BlackboardUtils.FindValue<bool>(rootElement.GetComponent<Blackboard>(),"_isLocked");
            eventTagElement.gameObject.SetActive(!isLocked);

            EndAction();
        }

    }
}