using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Scene")]
public class PreloadSceneInfoObjectAssets : ActionTask
{
    public BBParameter<string> bundleName;
    public BBParameter<bool> combineApplicationType;
    
    public BBParameter<string> progressKey;
    public BBParameter<object> progress;
    
    private AssetBundleRequest request = null;
    
    private PerformanceAnalyzer.TimeSample timeSample = new PerformanceAnalyzer.TimeSample("preload", "scene");
    protected override void OnExecute()
    {
#if USE_ASSETBUNDLE
        timeSample.BeginSample();
        request = AssetBundleManager.GetLoadedAssetBundle(GetBundleName()).assetBundle.LoadAllAssetsAsync(typeof(SceneInfoObject));
#else 
        WeightProgress wp = progress.value as WeightProgress;
        wp.UpdateProgress(progressKey.value, 1f);

        EndAction();
#endif
    }
    
    protected override void OnUpdate()
    {
        WeightProgress wp = progress.value as WeightProgress;
        wp.UpdateProgress(progressKey.value, request.progress);

        if (request.isDone)
        {
            var bname = GetBundleName();
            var objs = request.allAssets;
            int count = objs.Length;
            for (int i = 0; i < count; ++i)
            {
                AssetBundleManager.SetLoadedAsset(bname, objs[i].name,
                    new LoadedAsset(bname, objs[i].name, typeof(SceneInfoObject), objs[i]));
            }
            
            wp.UpdateProgress(progressKey.value, 1f);
            
            timeSample.EndSample();

            EndAction();
        }
    }
    
    protected string GetBundleName()
	{
		return combineApplicationType.value ? ApplicationSettings.MakeApplicationBundleName(bundleName.value) : bundleName.value;
	}
}

}
