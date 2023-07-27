#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace BagelCode
{
	public class TestAssetBundleBuilder : MonoBehaviour
	{
		public string bundleName = "jgq";

		public List<string> hashs;
		public List<string> Hashs
		{
			get
			{
				if (hashs is null)
					hashs = new List<string>();

				return hashs;
			}
		}

		//[MenuItem("BagelCode/Builder/BuildTargetAssetBundle")]
		[Button]
		void BuildTargetAB()
		{
			var builder = FindObjectOfType<TestAssetBundleBuilder>();
			Debug.Log("start build: " + bundleName);

			AssetBundleBuild[] abbs = new AssetBundleBuild[]
			{
			new AssetBundleBuild()
			{
				assetBundleName = bundleName,
				assetBundleVariant = "",
				assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName),
			}
			};

			var manifest = BuildPipeline.BuildAssetBundles("Assets", abbs,
				BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.Android);

			Debug.Log("done");
			Hashs.Add(manifest.GetAssetBundleHash(bundleName).ToString());
		}

		//[Button]
		//void PrintABHash()
		//{
		//	AssetBundle.UnloadAllAssetBundles(true);

		//	AssetBundle assetBundle = AssetBundle.LoadFromFile("Assets/AssetBundles/AssetBundles");
		//	AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

		//	Debug.LogError(manifest.GetAssetBundleHash(bundleName));
		//	Hashs.Add(manifest.GetAssetBundleHash(bundleName).ToString());

		//	assetBundle.Unload(true);
		//}
	}
}
#endif