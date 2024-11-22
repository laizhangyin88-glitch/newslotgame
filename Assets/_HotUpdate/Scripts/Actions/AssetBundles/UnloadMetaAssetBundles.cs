using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.AssetBundle
{

[Category("★ BagelCode/AssetBundle")]
public class UnloadMetaAssetBundles : ActionTask<Blackboard>
{
    public bool unloadAllLoadedObjects = false;

    protected override void OnExecute()
    {
        var usingBundleList = BlackboardQueryUtils.GetUsingMetaAssetBundles();

        if(usingBundleList != null)
        {
            for(int i=0; i<usingBundleList.Count; ++i)
            {
                AssetBundleManager.RemoveLoadedAssets(usingBundleList[i]);
                AssetBundleManager.UnloadAssetBundle(usingBundleList[i], unloadAllLoadedObjects);
            }
        }

        BlackboardQueryUtils.ClearUsingMetaAssetBundles();

        EndAction();
    }
}

}
