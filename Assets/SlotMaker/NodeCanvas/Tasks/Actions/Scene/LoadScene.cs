using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Scene")]
public class LoadScene : ActionTask<Transform>
{
	public BBParameter<string> bundleName;
	public BBParameter<string> assetName;
	public BBParameter<bool> combineApplicationType;
	public BBParameter<string> parentName;

	[BlackboardOnly]
	public BBParameter<GameObject> saveAs;

	protected override string info
		{
			get
			{
				if (string.IsNullOrEmpty(parentName.value))
					return string.Format("{0} = {1}.LoadScene({2})", saveAs, agentInfo, assetName);
				else
					return string.Format("{0} = Find({1}).LoadScene({2})", saveAs, parentName, assetName);
			}
		}

	protected override void OnExecute()
	{
		var sceneInfo = AssetBundleManager.LoadAsset<SceneInfoObject>(GetBundleName(), assetName.value).GetSceneInfo();
		Transform root = agent;
		if (!string.IsNullOrEmpty(parentName.value))
		{
			var go = GameObject.Find(parentName.value);
			if (go != null)
				root = go.transform;
		}
		var temp = SceneManager.LoadScene(root, sceneInfo);
		if (!saveAs.isNone)
			saveAs.value = temp;
		EndAction();
	}

	protected string GetBundleName()
	{
		return combineApplicationType.value ? ApplicationSettings.MakeApplicationBundleName(bundleName.value) : bundleName.value;
	}
}

}