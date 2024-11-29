#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace SlotMaker
{
    public static class EditorTools
    {
        public static string GetBundleName(UnityEngine.Object obj)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(path))
                return null;

            var importer = AssetImporter.GetAtPath(path);
            if (importer == null)
                return null;

            return importer.assetBundleName;
        }
    }
}
#endif