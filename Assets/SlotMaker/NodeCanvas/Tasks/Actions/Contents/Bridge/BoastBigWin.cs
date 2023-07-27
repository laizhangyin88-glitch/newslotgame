using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]
    public class BoastBigWin : ActionTask
    {
        public BBParameter<long> betCredit;
        public BBParameter<string> earnCredit = "./spin/earnCredit";

        protected override void OnExecute()
        {
            var _earnCredit = BlackboardUtils.FindVariable<long>(ContentBlackboard.Get(), earnCredit.value);

            MetaSystem.BoastBigWin(betCredit.value, _earnCredit.value, EndAction, null);
        }
    }
}
