using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class EndTurn : ActionTask
{
    protected override void OnExecute()
    {
        Blackboard cb = ContentBlackboard.Get();
        var turn = cb.GetValue<Blackboard>("turn");
        BlackboardUtils.SetOrCreateValue<long>(turn, "endTime", MetaSystem.GetTimeStamp());

        ContentEvent.EndTurn(turn);

        cb.RemoveVariable("current");
        EndAction();
    }
}

}
