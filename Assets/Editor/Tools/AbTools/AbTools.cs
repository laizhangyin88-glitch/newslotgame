using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// ab工具集
/// </summary>
public class AbTools : MonoBehaviour
{
    [MenuItem("Tools/ab工具集/删除未使用的ab名")]
    static void RemoveUnusedAssetBundles()
    {
        var unusedBundles = AssetDatabase.GetUnusedAssetBundleNames();
        if (unusedBundles == null || unusedBundles.Length <= 0)
        {
            Debug.Log("没有未使用的空ab名");
            return;
        }

        var unusedAssetBundleString = "";

        //Add all except the last Asset Bundle name into the Unused Asset Bundle String with a comma at the end
        for (var i = 0; i < unusedBundles.Length - 1; i++)
        {
            unusedAssetBundleString += unusedBundles[i] + ", ";
        }
        //Add the last string without a comma
        unusedAssetBundleString += unusedBundles.Last();
        //Remove the asset bundles from the editor
        AssetDatabase.RemoveUnusedAssetBundleNames();
        Debug.Log($"Removed Asset Bundles: {unusedAssetBundleString}.");
    }
}
