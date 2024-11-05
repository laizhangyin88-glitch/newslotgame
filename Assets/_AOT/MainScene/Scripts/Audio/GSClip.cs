using System;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SlotMaker
{
	[Serializable]
	public class GSClip : BundleResource
	{
		protected AudioClip clip;

#if UNITY_EDITOR
		[ShowInInspector]
		[HorizontalGroup(width: 20)]
		[HideLabel]
		private AudioClip _clip
		{
			get { return null; }
			set
			{
				bundleName = EditorTools.GetBundleName(value);
				assetName = value.name;

				if (Application.isPlaying)
					clip = AssetBundleManager.LoadAsset<AudioClip>(bundleName, assetName);
			}
		}
#endif

        public AudioClip Load()
		{
			if (clip == null)
				clip = AssetBundleManager.LoadAsset<AudioClip>(bundleName, assetName);

	        IncreaseUseCount();
			
			return clip;
		}

		public void UnLoad()
		{
	        DecreaseUseCount();
		}

	    public void Clear()
	    {
	        clip = null;
	        AssetBundleManager.UnloadAsset(bundleName, assetName);
	    }
	}
}
