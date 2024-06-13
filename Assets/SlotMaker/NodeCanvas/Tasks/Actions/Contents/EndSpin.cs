using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/Contents")]
    public class EndSpin : ActionTask
    {
        protected override void OnExecute()
        {
            Blackboard cb = ContentBlackboard.Get();
            var spin = cb.GetValue<Blackboard>("spin");
            var parent = spin.GetValue<Blackboard>("parent");
            long timestamp = MetaSystem.GetTimeStamp();

            BlackboardUtils.SetOrCreateValue<long>(spin, "endTime", timestamp);

            ContentEvent.EndSpin(spin);

            cb.SetValue("current", parent);
            cb.RemoveVariable("spin");

            EndAction();
        }
    }
}
