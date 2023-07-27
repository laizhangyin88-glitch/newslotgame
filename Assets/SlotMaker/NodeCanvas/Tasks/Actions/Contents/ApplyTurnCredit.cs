using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class ApplyTurnCredit : ActionTask
{
    protected override void OnExecute()
    {
        var turnCredit = BlackboardUtils.FindVariable<long>(null, "./turn/earnCredit");
        var meCredit = BlackboardUtils.FindVariable<long>(null, "/me/credit");
        meCredit.value += turnCredit.value;

        EndAction();
    }
}

}
