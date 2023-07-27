using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SlotMaker.Templates
{
    public class ContextMenu
    {
        private static readonly string templatesPath = "Assets/SlotMaker/Packages/Presets/";

        [MenuItem("GameObject/UI/SlotMaker/Canvas (Overriden)", false, 1)]
        private static void CreateOverridenCanvas(MenuCommand command) { CreateTemplatePrefab("UI/Overriden Canvas"); }

        [MenuItem("GameObject/UI/SlotMaker/Anchor", false, 2)]
        private static void CreateAnchor(MenuCommand command) { CreateTemplatePrefab("UI/Anchor"); }

        [MenuItem("GameObject/UI/SlotMaker/Button", false, 3)]
        private static void CreateButton(MenuCommand command) { CreateTemplatePrefab("UI/Button"); }

        [MenuItem("GameObject/UI/Contents/Game Contents", false, 1)]
        private static void CreateGameContents(MenuCommand command) { CreateTemplatePrefab("UI/Contents/Game Contents"); }

        private static void CreateTemplatePrefab(string templateName)
        {
            GameObject selectedGo = Selection.activeGameObject;
            if (selectedGo)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GetPath("Prefabs/" + templateName + ".prefab"));
                var go = selectedGo.CopyChild(prefab);
                Undo.RegisterCreatedObjectUndo(go, templateName);
                Selection.activeGameObject = go;
            }    
        }

        private static string GetPath(string path)
        {
            return Path.Combine(templatesPath, path);
        }
    }
}