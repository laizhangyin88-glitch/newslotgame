using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Math")]
public class MultiplierLong<T> : ActionTask<Blackboard>
{
    public BBParameter<string> longValue;
    public BBParameter<T> multiplierValue;

    public BBParameter<long> saveAs;

    protected override string info
    {
        get 
        {
            return string.Format("{0} = {1} * {2})", saveAs, longValue, multiplierValue);
        }
    }

    protected override void OnExecute()
    {
        var srcValue = BlackboardUtils.FindVariable<long>(agent, longValue.value);

        saveAs.value = System.Convert.ToInt64(System.Convert.ToDouble(srcValue.value) * System.Convert.ToDouble(multiplierValue.value));

        EndAction();
    }
}

}
