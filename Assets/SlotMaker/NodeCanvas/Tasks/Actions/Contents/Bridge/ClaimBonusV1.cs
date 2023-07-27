using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/Contents")]
    public class ClaimBonusV1 : ActionTask
    {
        public BBParameter<string> uid;
        public BBParameter<int> selectedIndex;

        protected override string info
        {
            get { return string.Format("Claim Bonus {0} with {1}", uid, selectedIndex); }
        }

        protected override void OnExecute()
        {
            MetaSystem.SlotClaimBonus(uid.value, selectedIndex.value, EndAction, null);
        }
    }
}
