
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Transform")]
public class FindChild : ActionTask<Transform>
{
    public BBParameter<string> childName;

    public BBParameter<Transform> saveAs;

    protected override string info
    {
        get
        {
            return string.Format("Find Child {0} save as {1}", childName, saveAs);
        }
    }

    protected override void OnUpdate()
    {
        saveAs.value = agent.Find(childName.value);

        if (saveAs.value == null)
        {
            Debug.LogError("["+agent.name + "] Can't find child(" + childName.value+")");
        }
        EndAction();
    }
}

}
