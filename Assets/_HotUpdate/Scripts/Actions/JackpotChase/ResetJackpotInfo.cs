using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
[Category("★ BagelCode/JackpotChase")]
public class ResetJackpotInfo : ActionTask
{
    public BBParameter<Blackboard> jackpotBB;

    protected override string info
    {
        get
        {
            return string.Format("Reset Jackpot Info {0}", jackpotBB);
        }
    }

    protected override void OnExecute()
    {
        BlackboardQueryUtils.ResetJackpotInfo(jackpotBB.value);

        EndAction();
    }
}

}
