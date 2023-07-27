using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace SlotMaker.Keno.Tasks.Actions
{
[Category("★ SlotMaker/Keno")]
public class Keno_SaveBallGeneratorInstance : ActionTask<Blackboard>
{
    public BBParameter<KenoMediator> kenoMediator;
    public BBParameter<string> saveAs;

    protected override string info
    {
        get { return string.Format("Save {0}.ballGenerator as {1}", kenoMediator, saveAs); }
    }

    protected override void OnExecute()
    {
        string key = null;
        var bb = BlackboardUtils.FindBlackboard(agent, saveAs.value, ref key);

        var go = kenoMediator.value.KenoInstance.ballGenerator.GetComponent<BallGeneratorInstance>().Save().gameObject;
        go.transform.SetParent(bb.propertiesBindTarget.GetComponent<Transform>());

        var variable = bb.AddVariable(key, typeof(GameObject));
        variable.value = go;

        EndAction();
    }
}

}
