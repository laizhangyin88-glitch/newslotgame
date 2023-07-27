using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
[Category("★ BagelCode/Contents")]
public class AddGambleEarnCredit : ActionTask<Blackboard>
{
    public BBParameter<string> earnCredit;

    protected override string info
    {
        get { return string.Format("Add {0} to gambleBB", earnCredit); }
    }

    protected override void OnExecute()
    {
        var gambleCredit = BlackboardUtils.FindVariable<long>(agent, earnCredit.value);
        var gamble = BlackboardUtils.FindVariable<Blackboard>(null, "./gamble").value;

        if (gambleCredit != null)
        {
            ContentBlackboardUtils.AddEarnCredit(gamble, gambleCredit.value);
        }
        else
        {
            Debug.LogError(string.Format("[AddGambleEarnCredit] Cannot find ", gambleCredit));
        }

        EndAction();
    }
}

}
