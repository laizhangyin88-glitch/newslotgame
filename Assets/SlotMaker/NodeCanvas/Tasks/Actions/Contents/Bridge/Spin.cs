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
    public class Spin : ActionTask
    {
        public BBParameter<long> betCredit;
        public BBParameter<long> extraBetCredit;
        public BBParameter<object> customData = null;

        protected override string info { get { return "Request SlotSpin"; } }

        protected override void OnExecute()
        {
            MetaSystem.SlotSpin(betCredit.value, extraBetCredit.value, customData.value, () => { if (agent != null) EndAction(); }, null);
        }
    }
}
