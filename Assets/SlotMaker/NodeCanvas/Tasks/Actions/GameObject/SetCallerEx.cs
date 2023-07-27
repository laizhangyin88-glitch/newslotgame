using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace SlotMaker.Tasks.Actions
{

[Category("★ BagelCode/Scene")]
public class SetCallerEx : ActionTask<Transform>
{
    public BBParameter<string> callerName;
    public BBParameter<GameObject> callee;

    private const string CALLER = "caller";

    protected override string info
    {
        get { return string.Format("Set GameObject to callee ({0})", callerName); }
    }

    protected override void OnExecute()
    {
        if(agent != null && agent.gameObject != null)
        {
            var bb = callee.value.GetComponent<Blackboard>();
            if (bb != null)
            {
                Variable<GameObject> variable = null;

                if(callerName != null && !string.IsNullOrEmpty(callerName.value))
                {
                    variable = BlackboardUtils.GetOrCreateVariable<GameObject>(bb, callerName.value);
                }
                else
                {
                    variable = BlackboardUtils.GetOrCreateVariable<GameObject>(bb, CALLER);
                }

                variable.value = agent.gameObject;
            }
        }
        
        EndAction();
    }
}

}
