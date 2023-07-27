using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/Contents")]
    public class ClaimBonus : ActionTask
    {
        public BBParameter<int> bonusId;
        public BBParameter<int> selectedIndex;

        protected override string info
        {
            get { return string.Format("Claim Bonus {0} with {1}", bonusId, selectedIndex); }
        }

        protected override void OnExecute()
        {
            var bonus = ContentBlackboard.Get().GetVariable<Blackboard>("bonus");
            if (bonus != null)
            {
                int id = bonus.value.GetValue<int>("bonusId");
                string uid = bonus.value.GetValue<Blackboard>("response").GetValue<string>("uid");
                if (bonusId.value == id)
                {
                    MetaSystem.SlotClaimBonus(uid, selectedIndex.value, EndAction, null);
                }
                else 
                {
                    Debug.LogError(string.Format("ERROR: bonusId({0}) is not mached in ClaimBonus({1})", bonusId.value, id));       
                }
            }
            else 
            {
                Debug.LogError(string.Format("ERROR: Null bonus found in ClaimBonus({0})", bonusId.value));
            }
        }
    }
}
