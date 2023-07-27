using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/GameObject")]
public class SetGameObjectName : ActionTask 
{
    public BBParameter<GameObject> target;
    public BBParameter<string> objectName;

    protected override string info
    {
        get { return string.Format("{0}.name= {1}", target, objectName); }
    }

    protected override void OnExecute()
    {
        target.value.name = objectName.value;
        EndAction();
    }
}

}
