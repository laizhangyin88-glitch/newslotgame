using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ParadoxNotion.Serialization;
using SlotMaker.Json;

namespace SlotMaker.Slots.Editor
{
    public static class SlotsEditorUtils
    {
        public static T CreateScriptableObject<T>(string path) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            Selection.activeObject = asset;
            return asset;
        }

        public static string GetDirectoryName(ScriptableObject scriptableObject)
        {
            return FileUtils.GetDirectoryName(AssetDatabase.GetAssetPath(scriptableObject));
        }

        public static void SaveJson(TextAsset asset, object obj, bool prettify = true)
        {
            var path = AssetDatabase.GetAssetPath(asset);
            SaveJson(path, obj, prettify);
            Selection.activeObject = asset;
        }

        public static void SaveJson(string path, object obj, bool prettify = true)
        {
            var json = SlotSimpleJson.SerializeObject(obj);
            if (prettify) json = JSONSerializer.PrettifyJson(json);
            FileSystem.SaveTextAsset(path, json);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static string ToJson<T>(T obj, bool prettify = true)
        {
            var json = SlotSimpleJson.SerializeObject(obj);
            if (prettify) json = JSONSerializer.PrettifyJson(json);
            return json;
        }
    }
}
