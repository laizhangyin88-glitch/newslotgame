using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Scene")]
public class LoadSceneAsync : ActionTask<Transform> 
{
	public BBParameter<string> bundleName;
	public BBParameter<string> assetName;
	public BBParameter<bool> combineApplicationType;
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
			if (string.IsNullOrEmpty(parentName.value))
				return string.Format("{0} = {1}.LoadSceneAsync({2})", saveAs, agentInfo, assetName);
			else
				return string.Format("{0} = Find({1}).LoadSceneAsync({2})", saveAs, parentName, assetName);
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
				Transform root = agent;
				if (!string.IsNullOrEmpty(parentName.value))
				{
					var go = GameObject.Find(parentName.value);
					if (go != null)
						root = go.transform;
				}

				var sceneInfo = loadSceneInfoOperation.GetAsset<SceneInfoObject>().GetSceneInfo();

				sceneLoadOperation = SceneManager.LoadSceneAsync(root, sceneInfo, constraintSceneActivation.value);
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
