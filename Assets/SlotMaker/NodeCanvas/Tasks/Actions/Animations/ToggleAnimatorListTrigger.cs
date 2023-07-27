using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Animator")]
public class ToggleAnimatorListTrigger : ActionTask
{
    public BBParameter<int>             index;
    public BBParameter<List<Animator>>  animatorList;
    public BBParameter<string>          active;
    public BBParameter<string>          inactive;

    protected override string info{
        get{ return string.Format("if {0} == index than SetTrigger {3}, otherwise {4}", 
                                    index,
                                    animatorList,
                                    index,
                                    active, inactive); }           
    }

    protected override void OnExecute()
    {
        for (int count = 0; count < animatorList.value.Count; ++count)
        {
            if (count == index.value)
            {
                animatorList.value[count].SetTrigger(active.value);
            }
            else
            {
                animatorList.value[count].SetTrigger(inactive.value);
            }
        }

        EndAction();
    }
}

}
