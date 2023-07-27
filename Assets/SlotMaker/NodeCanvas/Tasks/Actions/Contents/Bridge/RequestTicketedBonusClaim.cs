using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]
    public class RequestTicketedBonusClaim : ActionTask<Blackboard>
    {
        public BBParameter<string> ticketIDValue;
        public BBParameter<string> targetBBPath = "./ticketClaim";// Deprecated!

        protected override string info { get { return "Request Ticketed Bonus Claim"; } }

        protected override void OnExecute()
        {
            var ticketId = BlackboardUtils.FindVariable<int>(agent, ticketIDValue.value);
            MetaSystem.ClaimTicketedBonus(ticketId.value, EndAction, null);
        }
    }
}
