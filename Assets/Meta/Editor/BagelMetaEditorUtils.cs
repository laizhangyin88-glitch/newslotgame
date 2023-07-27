using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using SlotMaker;

namespace BagelCode
{
    public static partial class BagelMetaEditorUtils
    {
        [MenuItem("Assets/Utils/Icon/Make Simple Icon Prefab", false, 751)]
        public static void MakeSimpleIconPrefab()
        {
            if (Selection.assetGUIDs == null)
                return;

            string folderName = "Prefabs";
            List<string> usableExtensions = new List<string>(){ ".png", ".jpg", ".jpeg" };

            string[] selectPaths = Selection.assetGUIDs.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();

            foreach (string path in selectPaths)
            {
                string extension = System.IO.Path.GetExtension(path);

                if(!string.IsNullOrEmpty(extension) && usableExtensions.Contains(extension.ToLower()))
                {
                    // Make Local Folder. 
                    string itemDirectoryPath = FileUtils.GetDirectoryName(path);
                    string makeLocalFolderPath = itemDirectoryPath + "/" + folderName;
                    if(!AssetDatabase.IsValidFolder(makeLocalFolderPath))
                    {
                        string guid = AssetDatabase.CreateFolder(itemDirectoryPath, folderName);
                        makeLocalFolderPath = AssetDatabase.GUIDToAssetPath(guid);
                    }

                    var simpleIconObject = new GameObject();
                    simpleIconObject.AddComponent<CanvasRenderer>();

                    simpleIconObject.layer = 5;

                    var rectTransform = simpleIconObject.AddComponent<RectTransform>();
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);

                    var simpleIconSprite = simpleIconObject.AddComponent<Image>();
                    simpleIconSprite.raycastTarget = false;
                    
                    var sizeFitter = simpleIconObject.AddComponent<ContentSizeFitter>();
                    sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    
                    simpleIconObject.AddComponent<ContextCompositor>();
                    simpleIconSprite.sprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;

                    string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                    string localPath = makeLocalFolderPath + "/" + fileName + ".prefab";
                    localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

                    PrefabUtility.SaveAsPrefabAssetAndConnect(simpleIconObject, localPath, InteractionMode.UserAction);

                    GameObject.DestroyImmediate(simpleIconObject);

                    AssetImporter.GetAtPath(localPath).SetAssetBundleNameAndVariant("epicalbum", "");
                }
            }
        }
    }
}