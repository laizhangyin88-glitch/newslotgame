using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/Contents")]
    public class EndGame : ActionTask
    {
        protected override void OnExecute()
        {
            var mb = MainBlackboard.Get();
            BlackboardUtils.SetOrCreateValue(mb, "inGame", false);

            var cb = ContentBlackboard.Get();
            var game = cb.GetValue<Blackboard>("game");

            BlackboardUtils.SetOrCreateValue<long>(game, "endCredit", BlackboardUtils.FindVariable<long>(null, "/me/credit").value);
            BlackboardUtils.SetOrCreateValue<long>(game, "endTime", MetaSystem.GetTimeStamp());

            ContentEvent.EndGame(game);

            EndAction();
        }
    }
}
