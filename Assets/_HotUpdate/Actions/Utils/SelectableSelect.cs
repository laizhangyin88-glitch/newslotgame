using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Utils")]
public class SelectableSelect: ActionTask<Transform>
{
    protected override void OnExecute()
    {
        var selectable = agent.GetComponent<UnityEngine.UI.Selectable>();
        selectable.Select();
        EndAction();
    }
}

}
