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
public class GetMultiplierNumeratorCoin : ActionTask<Blackboard> 
{
    public BBParameter<string>  coinValue;
    public BBParameter<string>  numeratorValue;
    public BBParameter<bool>    isAdditional;

    public BBParameter<long> saveValue;

    protected override string info
    {
        get
        {
            return string.Format("{0} = NumberUtils.GetMultiplierNumeratorValue({1}, {2}){3}", saveValue, coinValue, numeratorValue, isAdditional.value ? "Additional" : "");
        }
    }

    protected override void OnExecute()
    {
        var coin = BlackboardUtils.FindVariable<long>(agent, coinValue.value);
        var numerator = BlackboardUtils.FindVariable<long>(agent, numeratorValue.value);

        if(isAdditional.value)
            saveValue.value = NumberUtils.GetAdditionalMultiplierNumeratorValue( coin.value, numerator.value );
        else
            saveValue.value = NumberUtils.GetMultiplierNumeratorValue( coin.value, numerator.value );

        EndAction();
    }
}

}
