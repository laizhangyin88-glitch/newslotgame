using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode;
using System.Collections.Generic;

using SlotMaker.Json;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]
    public class RequestKenoPlay : ActionTask<Blackboard>
    {
        public BBParameter<long> betPerTicket;
        public BBParameter<long> extraBetPerTicket;
        public BBParameter<List<string>> pickInfoList;
        public BBParameter<object> customData = null;

        protected override string info { get { return "Request Keno Play"; } }

        protected override void OnExecute()
        {
            var pickResult = new List<List<int>>();
            foreach (var pickInfo in pickInfoList.value)
            {
                var variable = BlackboardUtils.FindVariable<List<int>>(agent, pickInfo);
                pickResult.Add(variable.value);
            }

            MetaSystem.KenoPlay(betPerTicket.value, extraBetPerTicket.value, pickResult.Count, pickResult, customData.value, EndAction, null);
        }
    }
}
