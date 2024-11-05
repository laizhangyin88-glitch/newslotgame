using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Name("Initialize AssetBundle")]
[Category("★ SlotMaker/AssetBundle")]
public class InitAssetBundle : ActionTask 
{
    public BBParameter<string> downloadUrl;
    private AssetBundleLoadOperation manifestOperation;

    protected override void OnExecute()
    {
#if USE_ASSETBUNDLE
        AssetBundleManager.BaseUrl = ApplicationSettings.GetRemoteBundlePath();// "http://8.134.90.175:22000";//downloadUrl.value;
        manifestOperation = AssetBundleManager.Initialize();
#else
        EndAction();
#endif
    }

#if USE_ASSETBUNDLE
    protected override void OnUpdate()
    {
        if(manifestOperation == null) return;

        if(manifestOperation.IsDone())
        {
            EndAction();
        }
    }
#endif
    
}

}
