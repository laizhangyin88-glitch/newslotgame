using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/TierUtils")]
    public class GetClubTierGroup : ActionTask<Blackboard>
    {
        public BBParameter<string> valueA;

        [BlackboardOnly]
        public BBParameter<int> saveAs;

        protected override string info
        {
            get
            { 
                return string.Format("Get Club Tier Group of {1} as {0}", saveAs, valueA); 
            }
        }

        protected override void OnExecute()
        {
            var tierValue = BlackboardUtils.FindVariable<int>(agent, valueA.value);

            if (tierValue != null)
                saveAs.value = ClubUtils.GetClubTierGroup(tierValue.value);
            
            EndAction();
        }
    }
}

