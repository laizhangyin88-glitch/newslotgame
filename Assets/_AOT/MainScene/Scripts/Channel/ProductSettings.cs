using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SlotMaker;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BagelCode
{
    [CreateAssetMenu(fileName="ProductSettings", menuName="Meta/Environment/ProductSettings")]
    public class ProductSettings : ScriptableObjectSingleton<ProductSettings>
    {
        public string productVersion;
        public string deeplinkUriScheme;

        [ShowInInspector]
        public Dictionary_string_string eventTokenDict = new Dictionary_string_string();

        [ShowInInspector]
        // Key original thumbnail name  ex) Slot Image Big JGQ
        // Value type thumbnail name ex) Slot Image Big JGQ A
        public Dictionary_string_string thumbnailNames = new Dictionary_string_string();

        [ShowInInspector]
        // Key icon prefab name ex) Slot Image Big JGQ Anim
        // Value assetbundle name ex) jgq_anim_thumb
        public Dictionary_string_string animationThumbnailNames = new Dictionary_string_string();

        public static int GetProductVersionNumber()
        {
            string[] versions = Instance.productVersion.Split(new char[]{ '.' });
            return Int32.Parse(versions[0]) * 10000 + Int32.Parse(versions[1]) * 100 + Int32.Parse(versions[2]);
        }

        public Dictionary<string, string> GetAdjustTokens()
        {
            return Instance.eventTokenDict;
        }

        public string GetThumbnailName(string name)
        {
            if(string.IsNullOrEmpty(name)) return null;

            if(thumbnailNames.ContainsKey(name))
                return thumbnailNames[name];

            return null;
        }

        public string GetAnimationThumbnailAssetBundleName(string assetName)
        {
            if(string.IsNullOrEmpty(assetName)) return null;

            if(animationThumbnailNames.ContainsKey(assetName))
                return animationThumbnailNames[assetName];

            return null;
        }
    }
}
