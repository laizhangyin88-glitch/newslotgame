using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class UpdateSpin : ActionTask
{
    public BBParameter<string> targetBB = "./spin";

    protected override string info { get { return string.Format("UpdateResponse"); } }

    protected override void OnExecute()
    {
        var spin = BlackboardUtils.FindVariable<Blackboard>(null, targetBB.value);
        var parent = BlackboardUtils.FindVariable<Blackboard>(spin.value, "parent");
        BlackboardUtils.GetOrCreateVariable<Blackboard>(spin.value, "response");
        var parentResponse = BlackboardUtils.FindVariable<Blackboard>(parent.value, "response");

        var bonusResult = BlackboardUtils.FindVariable<List<Blackboard>>(parentResponse.value, "bonusResult");
        if (bonusResult == null)
        {
            BlackboardUtils.GetOrCreateBlackboardList(parentResponse.value, "bonusResult");
        }

        spin.value.SetValue("response", parentResponse.value);

        EndAction();
    }
}

}
