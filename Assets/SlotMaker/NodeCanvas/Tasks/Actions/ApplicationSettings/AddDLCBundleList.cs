using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/ApplicationSettings")]
public class AddDLCBundleList : ActionTask
{
    public BBParameter<List<string>> dlcList;

    protected override void OnExecute()
    {
        for (int i = 0; i < dlcList.value.Count; ++i)
        {
            AssetBundleManager.AddDLC(dlcList.value[i]);
        }

        EndAction();
    }
}

}
