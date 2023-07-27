using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Scene")]
public class LoadSceneAsync1 : ActionTask
{
	public BBParameter<string> bundleName;
	public BBParameter<string> assetName;
	public BBParameter<bool> combineApplicationType;
    public BBParameter<Transform> parent;
	public BBParameter<string> parentName;
	public BBParameter<bool> constraintSceneActivation;

	[BlackboardOnly]
	public BBParameter<GameObject> saveAs;

	protected AssetBundleLoadAssetOperation loadSceneInfoOperation = null;
	protected SceneLoadOperation sceneLoadOperation = null;

	protected override string info
	{
		get
		{
            return string.Format("{0} = LoadSceneAsync({1})", saveAs, assetName);
		}
	}

	protected override void OnExecute()
	{
		loadSceneInfoOperation = null;
		sceneLoadOperation 	   = null;
	}

	protected override void OnUpdate()
	{
		if (loadSceneInfoOperation == null)
			loadSceneInfoOperation = AssetBundleManager.LoadAssetAsync<SceneInfoObject>(GetBundleName(), assetName.value);

		if (loadSceneInfoOperation.IsDone())
		{
			if (sceneLoadOperation == null)
			{
                var _parent = parent.value;
                if (!string.IsNullOrEmpty(parentName.value))
                {
                    if (_parent == null)
                        _parent = GameObject.Find(parentName.value).transform;
                    else
                        _parent = _parent.Find(parentName.value);
                }

				var sceneInfo = loadSceneInfoOperation.GetAsset<SceneInfoObject>().GetSceneInfo();

				sceneLoadOperation = SceneManager.LoadSceneAsync(_parent, sceneInfo, constraintSceneActivation.value);
			}

			if (sceneLoadOperation.IsDone())
			{
				if (!saveAs.isNone)
					saveAs.value = sceneLoadOperation.GetScene();
				EndAction();
			}
		}
	}

	protected string GetBundleName()
	{
		return combineApplicationType.value ? ApplicationSettings.MakeApplicationBundleName(bundleName.value) : bundleName.value;
	}
}

}
