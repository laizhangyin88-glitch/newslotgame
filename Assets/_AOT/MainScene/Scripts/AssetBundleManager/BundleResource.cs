using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker
{
	[Serializable]
	public class BundleResource : SharedPtr
	{
        [HorizontalGroup(width: 20)]
        [HideLabel]
		public string bundleName;

        [HorizontalGroup]
        [HideLabel]
		public string assetName;

	    public BundleResource() : base() {}
	}
}
