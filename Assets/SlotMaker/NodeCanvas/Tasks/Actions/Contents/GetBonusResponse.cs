using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class GetBonusResponse : ActionTask
{
    public BBParameter<int> bonusId;

    [BlackboardOnly]
    public BBParameter<Blackboard> response;

    protected override string info
    {
        get {return string.Format("Get Bonus Response by {0} and save as {1}",bonusId, response);}
    }

    protected override void OnExecute()
    {
        var spin = BlackboardUtils.FindVariable<Blackboard>(null, "./spin");
        if(spin == null || spin.value == null)
        {
            EndAction(false);
            return;
        }
        response.value = ContentBlackboardUtils.GetBonusResponse(spin.value, bonusId.value);
        EndAction();
    }
}

}
