using System;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/EarlyAccess")]

public class EarlyAccessRefill : ActionTask<Blackboard>
{
    public BBParameter<int> grade;
    public BBParameter<int> saveAsRefillSpinCount;

    protected override string info
    {
        get { return "Refill Early Access Bonus Spin Counts"; }
    }

    protected override void OnExecute()
    {
        saveAsRefillSpinCount.value = BlackboardQueryUtils.RefillEarlyAccessBonusSpins(grade.value);

        EndAction();
    }
}

}
