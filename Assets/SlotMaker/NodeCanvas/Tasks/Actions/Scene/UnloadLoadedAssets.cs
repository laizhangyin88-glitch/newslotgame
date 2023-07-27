using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Scene")]
public class UnloadLoadedAssets : ActionTask
{
    public BBParameter<string> bundleName;
    public BBParameter<bool> unloadAllLoadedObjects;
    public BBParameter<bool> combineApplicationType;

    protected override void OnExecute()
    {
        AssetBundleManager.UnloadLoadedAssets(GetBundleName(), unloadAllLoadedObjects.value);
        EndAction();
    }

    protected string GetBundleName()
    {
        return combineApplicationType.value ? ApplicationSettings.MakeApplicationBundleName(bundleName.value) : bundleName.value;
    }
}

}
