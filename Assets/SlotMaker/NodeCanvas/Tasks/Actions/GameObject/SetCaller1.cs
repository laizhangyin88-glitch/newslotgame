using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace SlotMaker.Tasks.Actions
{

[Category("★ BagelCode/Scene")]
public class SetCaller1 : ActionTask<Transform>
{
    public BBParameter<GameObject> caller;
    public BBParameter<GameObject> callee;

    private const string CALLER = "caller";

    protected override string info
    {
        get { return "Set GameObject to callee"; }
    }

    protected override void OnExecute()
    {
        if(caller != null && caller.value != null)
        {
            var bb = callee.value.GetComponent<Blackboard>();
            if (bb != null)
            {
                var variable = BlackboardUtils.GetOrCreateVariable<GameObject>(bb, CALLER);
                variable.value = caller.value;
            }
        }
        
        EndAction();
    }
}

}
