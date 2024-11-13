using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{
    [Category("★ BagelCode/BI")]
    public class BI_event_enter : ActionTask<Blackboard>
    {
        public BBParameter<string> enterType;
        public BBParameter<string> type;
        public BBParameter<string> contextId;

        protected override string info
        {
            get { return string.Format("BI Event Enter {0} {1} {2}", enterType?.value, type?.value, contextId?.value); }
        }

        protected override void OnExecute()
        {
            BiEventUtils.SendBiEventEnter(enterType?.value, type?.value, contextId?.value);

            EndAction();
        }
    }
}
