using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.Events;

namespace SlotMaker
{
    public static class SlotThumbnailCoverEditor
    {
        [MenuItem("Assets/MISC/Make SlotThumbnail", false, 751)]
        public static void MakeSlotThumbnail()
        {
            if (Selection.assetGUIDs == null)
                return;

            string[] paths = Selection.assetGUIDs.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();
            foreach (var path in paths)
            {
                GameObject prefab = PrefabUtility.LoadPrefabContents(path);
                Process(prefab);
                PrefabUtility.SaveAsPrefabAsset(prefab, path);
                PrefabUtility.UnloadPrefabContents(prefab);
            }
        }

        [MenuItem("Assets/MISC/Patch SlotThumbnail Color", false, 752)]
        public static void PatchSlotThumbnail()
        {
            if (Selection.assetGUIDs == null)
                return;

            string[] paths = Selection.assetGUIDs.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();
            foreach (var path in paths)
            {
                GameObject prefab = PrefabUtility.LoadPrefabContents(path);
                Patch(prefab);
                PrefabUtility.SaveAsPrefabAsset(prefab, path);
                PrefabUtility.UnloadPrefabContents(prefab);
            }
        }

        private static void Process(GameObject prefab)
        {
            var image = GetOrAddComponent<Image>(prefab);
            var listener = GetOrAddComponent<PIDButtonInteractableListener>(prefab);
            var setter = GetOrAddComponent<ImageColorSetter>(prefab);
            listener.onInteractableChanged = new UnityBoolEvent();
            UnityEventTools.AddPersistentListener(listener.onInteractableChanged, setter.SetColor);
            setter.image = image;
            setter.colors = new List<Color>();
            setter.colors.Add(new Color(0f, 0f, 0f, 150f / 255f));
            setter.colors.Add(new Color(0f, 0f, 0f, 0f));
        }

        private static void Patch(GameObject prefab)
        {
            var setter = GetOrAddComponent<ImageColorSetter>(prefab);
            setter.colors[0] = new Color(150f / 255f, 150f / 255f, 150f / 255f, 1f);
            setter.colors[1] = new Color(1f, 1f, 1f, 1f);
        }

        private static T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            T comp = go.GetComponent<T>();
            if (!comp)
                comp = go.AddComponent<T>();
            return comp;
        }
    }
}