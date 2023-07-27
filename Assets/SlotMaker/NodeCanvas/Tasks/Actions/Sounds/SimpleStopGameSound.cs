using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Sounds")]
public class SimpleStopGameSound : ActionTask
{
    public BBParameter<string> id;

    protected override string info { get { return "Stop " + id; } }

    protected override void OnExecute()
    {
        GSManager.Instance.GetHandler(id.value).Stop();

        EndAction();
    }
}

}
