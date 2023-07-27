using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker")]
public class SetAnimatorsParameterTrigger : ActionTask
{
    public BBParameter<List<int>> conditions;
    public BBParameter<List<Animator>> animators;
    public BBParameter<List<string>> triggers;

    protected override string info
    {
        get { return "SetAnimatorsParameterTrigger"; }
    }

    protected override void OnExecute()
    {
        for (int i = 0; i < conditions.value.Count; ++i)
        {
            int exception = conditions.value[i];
            animators.value[i].SetTrigger(triggers.value[exception]);
        }
        EndAction();
    }
}

}
