using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SetGameObjectAssetBundleName
{
    private static string m_AssetBundleName = "fpy";

    [MenuItem("Assets/Set GameObject AssetBundle Name", false, 703)]
    public static void SetAssetBundleName()
    {
        Object go = Selection.activeObject;
        string path = AssetDatabase.GetAssetPath(go);
        Debug.Log(path);
        AssetImporter importer = AssetImporter.GetAtPath(path);
        if(importer != null)
        {
            importer.assetBundleName = m_AssetBundleName;
            AssetDatabase.Refresh();
        }
    }
    [MenuItem("Assets/Set Directroy Texture AssetBundle Name", false, 703)]
    public static void BundleAssetNameDirectroy()
    {
        string[] guids = Selection.assetGUIDs;
        foreach (var guid in guids)
        {
            // 将 GUID 转换为 路径
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Log(assetPath);
            DirectoryInfo directoryInfo = new DirectoryInfo(assetPath);
            FileInfo[] fileInfo = directoryInfo.GetFiles("*.png", SearchOption.AllDirectories);
            foreach (FileInfo fileInfo2 in fileInfo)
            {
                string path = fileInfo2.ToString().Replace("D:\\SlotClientProjects\\", "");
                path = path.Replace("\\", "/");
                AssetImporter importer = AssetImporter.GetAtPath(path);
                if(importer != null)
                {
                    importer.assetBundleName = m_AssetBundleName;
                    Debug.Log(importer.assetBundleName);
                }
            }
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/Set Directroy Prefabs AssetBundle Name", false, 703)]
    public static void BundlePrefabsAssetName()
    {
        string[] guids = Selection.assetGUIDs;
        foreach (var guid in guids)
        {
            // 将 GUID 转换为 路径
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Log(assetPath);
            DirectoryInfo directoryInfo = new DirectoryInfo(assetPath);
            FileInfo[] fileInfo = directoryInfo.GetFiles("*.prefab", SearchOption.AllDirectories);
            foreach (FileInfo fileInfo2 in fileInfo)
            {
                string path = fileInfo2.ToString().Replace("D:\\SlotClientProjects\\", "");
                path = path.Replace("\\", "/");
                AssetImporter importer = AssetImporter.GetAtPath(path);
                if (importer != null)
                {
                    importer.assetBundleName = m_AssetBundleName;
                    Debug.Log(importer.assetBundleName);
                }
            }
        }
        AssetDatabase.Refresh();
    }
}
