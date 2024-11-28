using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker
{
    [Serializable]
    public class SceneInfo : BundleResource
    {
    	public string uniqueName;
    	public string parentName;
    	public bool activate;
    	public List<SceneInfo> children = new List<SceneInfo>();
    }

    [Serializable]
    public class SerializableSceneInfo
    {
        public string bundleName;
        public string assetName;
        public string uniqueName;
        public string parentName;
        public bool activate;
        public int childCount;
        public int indexOfFirstChild;
    }
}
