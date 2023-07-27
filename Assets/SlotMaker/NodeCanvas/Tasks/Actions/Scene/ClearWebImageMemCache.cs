using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Scene")]
public class ClearWebImageMemCache : ActionTask
{
    protected override void OnExecute()
    {
        WebImageDownloader.Instance.ClearMemCache();
        EndAction();
    }
}

}
