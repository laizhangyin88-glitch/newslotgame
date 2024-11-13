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
public class IsMaxTier : ActionTask<Blackboard>
{
    public BBParameter<string>  valueA;

    [BlackboardOnly]
    public BBParameter<bool>     saveAs;

    protected override string info
    {
        get { return string.Format("{0} = TierUtils.IsMaxTier({1})", saveAs, valueA ); }
    }

    protected override void OnExecute()
    {
        Variable<int> tier = BlackboardUtils.FindVariable<int>(agent, valueA.value);

        saveAs.value = TierUtils.IsMaxTier(tier.value);

        EndAction();
    }
}

}
