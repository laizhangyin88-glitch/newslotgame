using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/ApplicationSettings")]
public class PatchDLCBundleList : ActionTask
{
	public BBParameter<List<string>> dlcList;
	public BBParameter<List<string>> bundleList;

	protected override void OnExecute()
	{
		bundleList.value = new List<string>();

		var streamingAssets = ApplicationSettings.GetStreaimingAssets();
		for (int i = 0; i < streamingAssets.Count; ++i)
		{
			string bundleName = streamingAssets[i];
			bundleList.value.Add(bundleName);
		}

		for (int i = 0; i < dlcList.value.Count; ++i)
		{
			AssetBundleManager.AddDLC(dlcList.value[i]);

			if (!bundleList.value.Contains(dlcList.value[i]))
				bundleList.value.Add(dlcList.value[i]);
		}

		EndAction();
	}
}

}
