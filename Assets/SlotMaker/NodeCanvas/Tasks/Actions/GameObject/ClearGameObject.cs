using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/GameObject")]
public class ClearGameObject : ActionTask 
{
    public BBParameter<GameObject> target;

    protected override string info
    {
        get { return string.Format("{0} = null", target); }
    }

    protected override void OnExecute()
    {
        target.value = null;
        EndAction();
    }
}

}
