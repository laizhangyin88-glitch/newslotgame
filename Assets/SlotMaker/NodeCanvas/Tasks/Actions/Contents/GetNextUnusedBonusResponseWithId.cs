using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class GetNextUnusedBonusResponseWithId : ActionTask
{
    public BBParameter<int> bonusId;
    [BlackboardOnly]
    public BBParameter<Blackboard> response;

    protected override string info
    {
        get {return string.Format("Get Next Unsed Bonus {0} Response save as {1}", bonusId, response);}
    }

    protected override void OnExecute()
    {
        var cb = ContentBlackboard.Get();
        var spin = cb.GetValue<Blackboard>("spin");     
        response.value = ContentBlackboardUtils.GetNextUnusedBonusResponse(spin, bonusId.value);
        bool hasResponse = (response != null && response.value != null);
        EndAction(hasResponse);
    }
}

}
