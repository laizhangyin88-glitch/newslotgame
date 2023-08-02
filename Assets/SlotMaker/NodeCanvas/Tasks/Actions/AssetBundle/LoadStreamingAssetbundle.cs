using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Name("Load Streaming Assetbundle")]
[Category("★ SlotMaker/AssetBundle")]
public class LoadStreamingAssetBundle : ActionTask 
{
    public BBParameter<string> bundleName;
    public BBParameter<bool> combineApplicationType;

    private AssetBundleLoadOperation loadOperation;

    protected override void OnExecute()
    {
        loadOperation = AssetBundleManager.LoadAssetBundleInternal(GetBundleName());
    }

    protected override void OnUpdate()
    {
        if (loadOperation == null) return;

        if (loadOperation.IsDone())
        {
            EndAction();
        }
    }

    protected string GetBundleName()
    {
        return combineApplicationType.value ? ApplicationSettings.MakeApplicationBundleName(bundleName.value) : bundleName.value;
    }
}

}
