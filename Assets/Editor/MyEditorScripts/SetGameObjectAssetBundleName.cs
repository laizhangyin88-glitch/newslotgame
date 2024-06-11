using System.IO;
using UnityEditor;
using UnityEngine;

public class SetGameObjectAssetBundleName
{
    private static string m_AssetBundleName = "fruitparty";

    private static string m_replacePathHead = "E:\\slotclient1\\";

    [MenuItem("Assets/Set GameObject AssetBundle Name", false, 703)]
    public static void SetAssetBundleName()
    {
        UnityEngine.Object go = Selection.activeObject;
        string path = AssetDatabase.GetAssetPath(go);
        Debug.Log(path);
        AssetImporter importer = AssetImporter.GetAtPath(path);
        if (importer != null)
        {
            importer.assetBundleName = m_AssetBundleName;
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("Assets/Set JSON AssetBundle Name", false, 703)]
    public static void SetJSONAssetBundleName()
    {
        string[] guids = Selection.assetGUIDs;
        string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        //string path = AssetDatabase.GetAssetPath(go);
        //Debug.Log(path);
        AssetImporter importer = AssetImporter.GetAtPath(assetPath);
        if (importer != null)
        {
            importer.assetBundleName = "fruitpartylang";
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
                if (importer != null)
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
        AssetImporter importer = AssetImporter.GetAtPath("Assets/Text 1.prefab");
        if (importer != null)
        {
            importer.assetBundleName = m_AssetBundleName;
            Debug.Log(importer.assetBundleName);
        }
        foreach (var guid in guids)
        {
            // 将 GUID 转换为 路径
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Log(assetPath);
            DirectoryInfo directoryInfo = new DirectoryInfo(assetPath);
            FileInfo[] fileInfo = directoryInfo.GetFiles("*.prefab", SearchOption.AllDirectories);
            foreach (FileInfo fileInfo2 in fileInfo)
            {
                string path = fileInfo2.ToString().Replace(m_replacePathHead, "");
                path = path.Replace("\\", "/");
                Debug.Log(path);
                AssetImporter importer1 = AssetImporter.GetAtPath(path);
                if (importer1 != null)
                {
                    importer1.assetBundleName = m_AssetBundleName;
                    Debug.Log(importer1.assetBundleName);
                }
            }
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/Set Directroy NodeCanvas AssetBundle Name", false, 703)]
    public static void BundleNodeCanvasName()
    {
        string[] guids = Selection.assetGUIDs;
        foreach (var guid in guids)
        {
            // 将 GUID 转换为 路径
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Log(assetPath);
            DirectoryInfo directoryInfo = new DirectoryInfo(assetPath);
            FileInfo[] fileInfo = directoryInfo.GetFiles("*.asset", SearchOption.AllDirectories);
            foreach (FileInfo fileInfo2 in fileInfo)
            {
                string path = fileInfo2.ToString().Replace(m_replacePathHead, "");
                path = path.Replace("\\", "/");
                Debug.Log(path);
                AssetImporter importer1 = AssetImporter.GetAtPath(path);
                if (importer1 != null)
                {
                    importer1.assetBundleName = m_AssetBundleName;
                    Debug.Log(importer1.assetBundleName);
                }
            }
        }
        AssetDatabase.Refresh();
    }
}
