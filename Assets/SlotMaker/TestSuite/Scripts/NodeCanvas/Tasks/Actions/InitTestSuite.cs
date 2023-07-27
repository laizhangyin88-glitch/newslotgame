using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.TestSuite.Tasks.Actions
{
	[Category("★ SlotMaker/TestSuite")]
	public class InitTestSuite : ActionTask
	{
#if DEV
		private AssetBundleLoadOperation bundleOperation;

		protected override void OnExecute()
		{
			bundleOperation = AssetBundleManager.LoadAssetBundleInternal("testsuite");
		}

		protected override void OnUpdate()
		{
			if (bundleOperation.IsDone())
			{
				var prefab = AssetBundleManager.LoadAsset<GameObject>("testsuite", "TestSuite Manager");
				var go = GameObject.Instantiate(prefab) as GameObject;
				go.name = "TestSuite Manager";

				EndAction();
			}
		}
#else
		protected override void OnExecute()
		{
			EndAction();
		}
#endif
	}
}
