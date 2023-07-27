using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Scene")]
public class RemoveLoadedAssetsByBundleName : ActionTask
{
    public BBParameter<string> bundleName;
    public BBParameter<bool> combineApplicationType;

    protected override void OnExecute()
    {
        AssetBundleManager.RemoveLoadedAssets(GetBundleName());
        EndAction();
    }

    protected string GetBundleName()
    {
        return combineApplicationType.value ? ApplicationSettings.MakeApplicationBundleName(bundleName.value) : bundleName.value;
    }
}

}
