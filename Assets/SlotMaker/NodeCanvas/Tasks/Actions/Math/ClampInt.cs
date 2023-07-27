using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Math")]
public class ClampInt : ActionTask 
{
    public BBParameter<int> value;
    public ClampOperationMethod Operation = ClampOperationMethod.Clamp;
    public BBParameter<int> minValue;
    public BBParameter<int> maxValue;

    protected override string info
    {
        get 
        {
            return string.Format("{0}({1}, {2}, {3})", OperationUtils.GetOperationString(Operation), 
                value, minValue, maxValue);
        }
    }

    protected override void OnExecute()
    {
        value.value = OperationUtils.Operate(value.value, minValue.value, maxValue.value, Operation);
        EndAction();
    }
}

}
