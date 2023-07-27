using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Seat")]
    public class GetSeatBB : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<int> seatIndex;

        [BlackboardOnly]
        public BBParameter<Blackboard> saveAs;

        protected override string info
        {
            get { return string.Format("{0} = Get Seat BB({1})", saveAs, seatIndex); }
        }

        protected override void OnExecute()
        {
            saveAs.value = BlackboardQueryUtils.GetSeatBlackboard(seatIndex.value);
            EndAction(true);
        }
    }
}

