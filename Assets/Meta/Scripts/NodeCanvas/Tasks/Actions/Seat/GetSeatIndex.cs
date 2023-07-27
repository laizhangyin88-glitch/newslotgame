using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Seat")]
    public class GetSeatIndex : ActionTask<Blackboard>
    {
        public BBParameter<string> valueA;

        [BlackboardOnly]
        public BBParameter<int> saveAs;

        protected override string info
        {
            get { return string.Format("{0} = Get Seat BB({1})", saveAs, valueA); }
        }

        protected override void OnExecute()
        {
            var userID = BlackboardUtils.FindVariable<string>(agent, valueA.value);

            if(userID != null)
                saveAs.value = BlackboardQueryUtils.GetSeatIndex(userID.value);
            else
                saveAs.value = -1;
            
            EndAction(true);
        }
    }
}

