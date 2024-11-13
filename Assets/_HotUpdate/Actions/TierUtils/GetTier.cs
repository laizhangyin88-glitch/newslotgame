using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BlackboardQuery
{

[Category("★ BagelCode/TierUtils")]
public class GetTier : ActionTask<Blackboard>
{
    public BBParameter<string>  valueA;

    [BlackboardOnly]
    public BBParameter<int>     saveTier;
    [BlackboardOnly]
    public BBParameter<int>     saveTierGroup;

    protected override string info
    {
        get { return string.Format("{0} = TierUtils.GetTier({1})", saveTier, valueA); }
    }

    protected override void OnExecute()
    {
        Variable<long> accRP = BlackboardUtils.FindVariable<long>(agent, valueA.value);
        
        saveTier.value = TierUtils.GetTier(accRP.value);
        saveTierGroup.value = TierUtils.GetTierGroup(saveTier.value);

        EndAction();
    }
}

}
