using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/Purchase")]
public class GetTierMultiplier : ActionTask<Blackboard> 
{
    public BBParameter<string>  valueA;

    // Legacy Parameter. 
    public BBParameter<TierMultiplierTableType> multiplierType;

    public BBParameter<double> saveValue;

    protected override string info
    {
        get { return string.Format("{0} = Get Tier Multiplier({1})", saveValue, valueA); }
    }

    protected override void OnExecute()
    {
        var tierValue = BlackboardUtils.FindVariable<int>(agent, valueA.value);

        saveValue.value = TierUtils.GetTierMultiplier( tierValue.value );

        EndAction();
    }
}

}
