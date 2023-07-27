using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Scene")]
public class GCCollect : ActionTask
{
    protected override void OnExecute()
    {
        System.GC.Collect();
        EndAction();
    }
}

}
