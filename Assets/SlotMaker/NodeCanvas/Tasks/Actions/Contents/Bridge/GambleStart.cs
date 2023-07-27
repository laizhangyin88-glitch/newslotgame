using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode;
using System.Collections.Generic;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]
    public class GambleStart : ActionTask
    {
        protected override void OnExecute()
        {
            string ticketId = BlackboardUtils.FindVariable<string>("./gamble/ticketId").value;
            MetaSystem.GambleStart(ticketId, EndAction, null);
        }
    }
}
