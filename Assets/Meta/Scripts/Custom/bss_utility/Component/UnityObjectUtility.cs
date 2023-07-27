using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using System.IO;

#if UNITY_EDITOR
namespace BSS.Utils.Editor
{
    using UnityEditor;
    public static class UnityObjectUtility 
    {
        private static string PROJECT_PATH => Application.dataPath.Replace("Assets", "");
        
        public static T ToUnityObjectInGuid<T>(string guid) where T : UnityObject
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
        public static T ToUnityObjectInAssetPath<T>(string path) where T : UnityObject
        {
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }


        public static string GetProjectPath(UnityObject obj)
        {
            return PROJECT_PATH + AssetDatabase.GetAssetPath(obj);
        }
        public static string GetMetaProjectPath(UnityObject obj)
        {
            return GetProjectPath(obj) + ".meta";
        }
        public static string GetAssetPath(UnityObject obj)
        {
            return AssetDatabase.GetAssetPath(obj);
        }
        public static string GetGuid(UnityObject obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            return AssetDatabase.AssetPathToGUID(path);
        }
        
        public static string ReadTextFile(UnityObject obj)
        {
            return File.ReadAllText(GetProjectPath(obj), System.Text.Encoding.UTF8);
        }
        public static string ReadMetaTextFile(UnityObject obj)
        {
            return File.ReadAllText(GetMetaProjectPath(obj), System.Text.Encoding.UTF8);
        }
    }
}
#endif
