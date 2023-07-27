using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Canvas")]
public class SetCanvasGroupInteractable: ActionTask<Transform>
{
    public BBParameter<bool> on;

    protected override void OnExecute()
    {
        var canvasGroup = agent.GetComponent<UnityEngine.CanvasGroup>();
        canvasGroup.interactable = on.value;
        EndAction();
    }
}

}
