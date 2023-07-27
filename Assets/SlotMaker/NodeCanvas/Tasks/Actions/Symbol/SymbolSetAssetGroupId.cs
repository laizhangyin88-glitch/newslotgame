using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Symbol")]
public class SymbolSetAssetGroupId : ActionTask
{
    public BBParameter<int> assetGroupId;

    protected override string info { get { return string.Format("assetGroupId = {0}", assetGroupId.value); } }

    protected override void OnExecute()
    {
        GlobalSymbolAssets.Instance.assetGroupId = assetGroupId.value;

        EndAction();
    }
}

}
