using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Scene")]
public class UnloadUnusedAssets : ActionTask
{
    private AsyncOperation operation;

    protected override void OnExecute()
    {
        operation = Resources.UnloadUnusedAssets();
    }

    protected override void OnUpdate()
    {
        if (operation.isDone)
            EndAction();
    }
}

}
