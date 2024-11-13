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

[Category("★ BagelCode/TierUtils")]
public class GetMultiplierFromNumerator : ActionTask<Blackboard> 
{
    public BBParameter<string>  numeratorValue;

    public BBParameter<double> saveValue;

    protected override string info
    {
        get { return string.Format("{0} = NumberUtils.GetMultiplierFromNumerator({1})", saveValue, numeratorValue); }
    }

    protected override void OnExecute()
    {
        var numerator = BlackboardUtils.FindVariable<long>(agent, numeratorValue.value);

        saveValue.value = NumberUtils.GetMultiplierFromNumerator( numerator.value );

        EndAction();
    }
}

}
