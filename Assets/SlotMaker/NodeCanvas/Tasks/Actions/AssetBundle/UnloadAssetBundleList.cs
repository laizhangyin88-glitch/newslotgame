using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Name("UnLoad Assetbundle List")]
[Category("★ SlotMaker/AssetBundle")]
public class UnLoadAssetBundleList : ActionTask
{
    public BBParameter<List<string>> bundleNameList;
    public BBParameter<bool> combineApplicationType;
    public bool unloadAllLoadedObjects = false;

    protected override void OnExecute()
    {
        for(int i=0; i<bundleNameList.value.Count; ++i)
        {
            AssetBundleManager.UnloadAssetBundle( GetBundleName(bundleNameList.value[i]), unloadAllLoadedObjects );
        }

        EndAction();
    }

    protected string GetBundleName(string bundleName)
    {
        return combineApplicationType.value ? ApplicationSettings.MakeApplicationBundleName(bundleName) : bundleName;
    }
}

}
