using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Transform")]
public class SetEulerAngles : ActionTask
{
    public BBParameter<Transform> target;
    public BBParameter<Vector3> rotation;
    public BBParameter<bool> local;

    protected override void OnExecute()
    {
        if (local.value)
            target.value.localEulerAngles = rotation.value;
        else
            target.value.eulerAngles = rotation.value;

        EndAction();
    }
}

}
