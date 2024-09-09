using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

    [Category("★ BagelCode/Contents")]
    public class GetNextUnusedBonusResponseByContentSpin : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<Blackboard> response;

        protected override string info
        {
            get { return string.Format("Get Next Unsed Bonus Response save as {0}", response); }
        }

        protected override void OnExecute()
        {
            response.value = ContentBlackboardUtils.GetNextUnusedBonusResponseByContentSpin();
            bool hasResponse = (response != null && response.value != null);
            EndAction(hasResponse);
        }
    }

}
