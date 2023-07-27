using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;

namespace SlotMaker.Tasks.Actions
{

[Category("★ BagelCode/Scene")]
public class ReturnToPool : ActionTask<Transform> {

    protected override string info{
        get {return string.Format("Return To Pool {0}", agentInfo);}
    }

    //in case it destroys self
    protected override void OnUpdate()
    {
        var go = agent.transform.GetComponent<PooledObject>();
        go.ReturnToPool();
        EndAction();
    }
}

}
