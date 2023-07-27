using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/SlotMachine")]
public class ApplySymbol : ActionTask<Transform>
{
    protected override string info { get { return "Apply"; } }

    protected override void OnExecute()
    {
        agent.GetComponent<BaseSymbol>().Apply();

        EndAction();
    }
}

}
