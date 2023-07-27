using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Conditions.Contents
{

[Category("★ BagelCode/Contents")]
public class CheckUnusedBonus : ConditionTask
{
    protected override string info
    {
        get { return "Check there is unused Bonus"; }
    }

    protected override bool OnCheck()
    {
        IBlackboard bb = ContentBlackboard.Get();
        var spin       = bb.GetVariable<Blackboard>("spin").value;
        var response   = ContentBlackboardUtils.GetNextUnusedBonusResponse(spin);

        return response != null;
    }
}

}
