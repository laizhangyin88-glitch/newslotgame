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
    public static class ImportStringTablesEditor
    {
        [MenuItem("Assets/String Table/Import Select Files", false, 752)]
        public static void ImportFiles()
        {
            if (Selection.assetGUIDs == null)
                return;
 
            string[] selectPaths = Selection.assetGUIDs.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();
            foreach (var path in selectPaths)
            {
                var extension = System.IO.Path.GetExtension(path);
                if(extension == ".asset")
                {
                    var stringTableObj = (StringTableObject)AssetDatabase.LoadAssetAtPath(path, typeof(StringTableObject));
                    if(stringTableObj != null)
                    {
                        Debug.Log("Import : " + path);
                        stringTableObj.OnImport();
                        stringTableObj.SetDirty();
                    }
                }
            }
        }

        [MenuItem("Assets/String Table/Import Select Folders", false, 752)]
        public static void ImportFolders()
        {
            if (Selection.assetGUIDs == null)
                return;
 
            string[] selectPaths = Selection.assetGUIDs.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();
            foreach (var path in selectPaths)
            {
                string[] files = System.IO.Directory.GetFiles(path);
                foreach(string filePath in files)
                {
                    var extension = System.IO.Path.GetExtension(filePath);
                    if(extension == ".asset")
                    {
                        var stringTableObj = (StringTableObject)AssetDatabase.LoadAssetAtPath(filePath, typeof(StringTableObject));
                        if(stringTableObj != null)
                        {
                            Debug.Log("Import : " + filePath);
                            stringTableObj.OnImport();
                            stringTableObj.SetDirty();
                        }
                    }
                }
            }
        }
    }
}