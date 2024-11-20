using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Task.Actions
{

    [Category("★ BagelCode/Friend")]
    public class GetChallengeFriendRecommendationList : ActionTask
    {
        public BBParameter<int> saveAsCount;
        public BBParameter<List<Blackboard>> saveAsList;

        protected override void OnExecute()
        {
            saveAsList.value = BlackboardQueryUtils.GetChallengeFriendRecommendationList();
            saveAsCount.value = saveAsList.value.Count;
            EndAction();
        }
    }
}