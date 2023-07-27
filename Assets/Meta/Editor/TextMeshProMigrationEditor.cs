using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using SlotMaker;
using TMPro;

namespace BagelCode
{
    public static class TextMeshProMigrationEditor
    {
        public static string findComponentText;

        [MenuItem("Assets/Text Mesh Pro/Print Select Files(Text Mesh Pro Character Value)", false, 750)]
        public static void PrintTMPCharacterValue()
        {
            if (Selection.assetGUIDs == null)
                return;

            findComponentText = "";
            string[] selectPaths = Selection.assetGUIDs.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();
            foreach (var path in selectPaths)
            {
                var extension = System.IO.Path.GetExtension(path);
                if(extension == ".prefab")
                {
                    PrintTMPCharacterValueFromFile(path);
                }
            }
        }

        [MenuItem("Assets/Text Mesh Pro/Print Select Folders(Text Mesh Pro Character Value)", false, 751)]
        public static void ImportFolders()
        {
            if (Selection.assetGUIDs == null)
                return;
 
            string[] selectPaths = Selection.assetGUIDs.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();
            findComponentText = "";
            if(selectPaths != null && selectPaths.Length > 0)
            {
                foreach (var path in selectPaths)
                {
                    PRintTMPCharacterValueFromFolderRecersive(path);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                System.IO.File.WriteAllText(selectPaths[0] + "/Find.txt", findComponentText, System.Text.Encoding.Default);
            }
        }

        private static void PRintTMPCharacterValueFromFolderRecersive(string folderPath)
        {
            var prefabs = System.IO.Directory.GetFiles(folderPath, "*.prefab");

            if(prefabs != null)
            {
                for(int i=0 ; i<prefabs.Length; ++i)
                {
                    PrintTMPCharacterValueFromFile(prefabs[i]);
                }
            }

            var directories = System.IO.Directory.GetDirectories(folderPath);

            if(directories != null)
            {
                for(int i=0 ; i<directories.Length; ++i)
                {
                    PRintTMPCharacterValueFromFolderRecersive(directories[i]);
                }
            }
        }

        private static void PrintTMPCharacterValueFromFile(string filePath)
        {
            var prefabObj = (GameObject)AssetDatabase.LoadAssetAtPath(filePath, typeof(GameObject));
            // Debug.LogError(prefabObj);
            bool isChange = false;
            PrintTMPForTransformRecersive(prefabObj.transform, prefabObj.transform, null, ref isChange);

            if(isChange)
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(prefabObj.transform);
                EditorUtility.SetDirty(prefabObj.transform);
            }
        }

        private static void PrintTMPForTransformRecersive(Transform targetTransform, Transform owner, string rootPath, ref bool isChange)
        {
            rootPath = string.IsNullOrEmpty(rootPath) ? "root" : rootPath + "/" + targetTransform.name;

            var tmpObj = targetTransform.GetComponent<TextMeshProUGUI>();
            if(tmpObj != null)
            {
                // tmpObj
                findComponentText += string.Format("\n{0}.prefab : {1} : {2}", owner.name, rootPath, tmpObj.characterSpacing);

                tmpObj.characterSpacing -= 4;
                isChange = true;
            }

            // Debug.LogError(targetTransform.childCount);
            for(int i=0; i< targetTransform.childCount; ++i)
            {
                PrintTMPForTransformRecersive(targetTransform.GetChild(i), owner, rootPath, ref isChange);
            }
        }
    }
}