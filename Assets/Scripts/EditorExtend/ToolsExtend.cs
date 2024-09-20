#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SlotMaker;
using System.IO;

public static class ToolsExtend
{
    [MenuItem("Tools/清理SteamingAssets残留/StaticMachineStreamingAssets", priority = 15)]
    public static void ClearABFileFromStaticMachineStreamingAssets()
    {
        var settings = AssetDatabase.LoadAssetAtPath<ApplicationSettings>("Assets/Meta/Resources/ApplicationSettings.asset");
        List<string> abNames = settings.staticMachineStreamingAssets;

        string dislogMessage = $"是否要清理SteamingAssets中所有存在于ApplicationSettings.staticMachineStreamingAssets中的ab相关文件?\n【此过程不可逆】";

        if(EditorUtility.DisplayDialog("确认删除?", dislogMessage, "ok", "cancel") == true)
        {
            ClearABFileByABNames(abNames);
            AssetDatabase.Refresh();
        }
    }
    [MenuItem("Tools/清理SteamingAssets残留/StreamingAssets", priority = 15)]
    public static void ClearABFileFromSteamingAssets()
    {
        var settings = AssetDatabase.LoadAssetAtPath<ApplicationSettings>("Assets/Meta/Resources/ApplicationSettings.asset");
        List<string> abNames = settings.streamingAssets;

        string dislogMessage = $"是否要清理SteamingAssets中所有存在于ApplicationSettings.streamingAssets中的ab相关文件?\n【此过程不可逆】";

        if (EditorUtility.DisplayDialog("确认删除?", dislogMessage, "ok", "cancel") == true)
        {
            ClearABFileByABNames(abNames);
            AssetDatabase.Refresh();
        }
    }
    [MenuItem("Tools/清理SteamingAssets残留/StaticSteamingAssets", priority = 15)]
    public static void ClearABFileFromStaticSteamingAssets()
    {
        var settings = AssetDatabase.LoadAssetAtPath<ApplicationSettings>("Assets/Meta/Resources/ApplicationSettings.asset");
        List<string> abNames = settings.staticStreamingAssets;

        string dislogMessage = $"是否要清理SteamingAssets中所有存在于ApplicationSettings.StaticSteamingAssets中的ab相关文件?\n【此过程不可逆】";

        if (EditorUtility.DisplayDialog("确认删除?", dislogMessage, "ok", "cancel") == true)
        {
            ClearABFileByABNames(abNames);
            AssetDatabase.Refresh();
        }
    }

    private static void ClearABFileByABNames(List<string> abNames)
    {
        string fullPath = Path.GetFullPath("Assets/StreamingAssets");
        string[] files = Directory.GetFiles(fullPath, "*", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            string fileTag = Path.GetFileName(file).Split('.')[0];
            if (abNames.Contains(fileTag) == false)
                continue;

            File.Delete(file);
        }
    }
}
#endif

