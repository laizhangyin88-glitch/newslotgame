using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{
    [Category("★ BagelCode/BI")]
    public class BI_client_gem_transaction : ActionTask<Blackboard>
    {
        public BBParameter<Blackboard> productBB;
        public BBParameter<string> contextID;
        public BBParameter<bool> isSuccess;

        public BBParameter<bool> isEvent;

        protected override void OnExecute()
        {
            BiEventUtils.GemTransaction(productBB.value, contextID.value, isSuccess.value, isEvent == null ? false : isEvent.value);

            EndAction();
        }
    }
}
