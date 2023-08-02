using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Name("UnLoad Assetbundle")]
[Category("★ SlotMaker/AssetBundle")]
public class UnLoadAssetBundle : ActionTask
{
    public BBParameter<string> bundleName;
    public BBParameter<bool> combineApplicationType;
    public bool unloadAllLoadedObjects = false;

    protected override void OnExecute()
    {
        AssetBundleManager.UnloadAssetBundle(GetBundleName(), unloadAllLoadedObjects);
        EndAction();
    }

    protected string GetBundleName()
    {
        return combineApplicationType.value ? ApplicationSettings.MakeApplicationBundleName(bundleName.value) : bundleName.value;
    }
}

}
