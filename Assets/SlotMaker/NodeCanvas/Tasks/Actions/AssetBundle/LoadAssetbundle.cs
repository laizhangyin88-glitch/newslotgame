using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Name("Load Assetbundle")]
[Category("★ SlotMaker/AssetBundle")]
public class LoadAssetBundle : ActionTask
{
    public BBParameter<string> bundleName;
    public bool forceDLC = false;

    private AssetBundleLoadOperation loadOperation;

    protected override void OnExecute()
    {
        if (forceDLC) AssetBundleManager.AddDLC(bundleName.value);

        loadOperation = AssetBundleManager.LoadAssetBundle(bundleName.value);
    }

    protected override void OnUpdate()
    {
        if (loadOperation.IsDone())
            EndAction();
    }
}

}
