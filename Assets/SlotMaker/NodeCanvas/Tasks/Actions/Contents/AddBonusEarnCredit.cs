using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
[Category("★ BagelCode/Contents")]
public class AddBonusEarnCredit : ActionTask<Blackboard>
{
    public BBParameter<string> earnCredit;

    protected override string info
    {
        get { return string.Format("Add {0} to bonusBB", earnCredit); }
    }

    protected override void OnExecute()
    {
        var bonusCredit = BlackboardUtils.FindVariable<long>(agent, earnCredit.value);
        var bonus = BlackboardUtils.FindVariable<Blackboard>(null, "./bonus").value;

        if (bonusCredit != null)
        {
            ContentBlackboardUtils.AddEarnCredit(bonus, bonusCredit.value);
        }
        else
        {
            Debug.LogError(string.Format("[AddBonusCredit] Cannot find ", bonusCredit));
        }

        EndAction();
    }
}

}
