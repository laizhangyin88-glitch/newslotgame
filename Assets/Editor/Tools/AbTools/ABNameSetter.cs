using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class ABNameSetter : EditorWindow
{
    private string inputABName = ""; // 合并的输入框内容
    private Vector2 assetScrollPosition;
    private Vector2 abScrollPosition;
    private Object[] selectedAssets;
    private List<string> allABNames = new List<string>(); // 所有 AB 名列表
    private List<string> filteredABNames = new List<string>(); // 过滤后的 AB 名列表

    [MenuItem("Tools/ab工具集/设置AB名")]
    public static void ShowWindow()
    {
        var window = GetWindow<ABNameSetter>("AB Name Setter");
        window.LoadAllABNames();
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("设置资源的 AB 名", EditorStyles.boldLabel);

        // 选择资源按钮
        if (GUILayout.Button("选择资源"))
        {
            ShowSelectedAssets();
        }

        // 显示所选资源
        GUILayout.Label("所选资源：", EditorStyles.boldLabel);
        assetScrollPosition = GUILayout.BeginScrollView(assetScrollPosition, GUILayout.Height(100)); // 限制高度
        if (selectedAssets != null && selectedAssets.Length > 0)
        {
            foreach (var asset in selectedAssets)
            {
                GUILayout.Label(asset.name);
            }
        }
        GUILayout.EndScrollView();

        // 输入框用于搜索和设置新的 AB 名
        GUILayout.Space(10); // 增加空间使得界面更美观
        GUILayout.Label("输入或搜索 AB 名：", EditorStyles.boldLabel);
        string previousInputABName = inputABName; // 保存上一个输入
        inputABName = EditorGUILayout.TextField("AB 名：", inputABName);

        // 如果输入框的当前值和前一个值不同，重新过滤
        if (previousInputABName != inputABName)
        {
            FilterABNames();
        }

        // 显示过滤后的 AB 名列表供用户选择
        GUILayout.Label("选择现有的 AB 名：", EditorStyles.boldLabel);
        abScrollPosition = GUILayout.BeginScrollView(abScrollPosition, GUILayout.Height(150)); // 限制高度
        foreach (var abName in filteredABNames)
        {
            if (GUILayout.Button(abName, GUILayout.Height(25))) // 按钮高度
            {
                // 立即更新输入框，不等待焦点丢失
                inputABName = abName;
                GUI.FocusControl(null); // 确保输入框失去焦点更新
            }
        }
        GUILayout.EndScrollView();

        // 空间分隔
        GUILayout.Space(10);

        // 应用按钮区域
        GUILayout.Label("操作选项：", EditorStyles.boldLabel);
        if (GUILayout.Button("应用 AB 名", GUILayout.Height(30))) // 按钮高度
        {
            ApplyABName();
        }

        GUILayout.Space(10); // 增加空间使得界面更美观
        GUILayout.Label("提示：输入新的 AB 名并点击应用，或选择已有 AB 名后点击应用。", EditorStyles.wordWrappedLabel);
    }

    private void ShowSelectedAssets()
    {
        selectedAssets = Selection.objects;
    }

    private void LoadAllABNames()
    {
        string[] allAssetBundles = AssetDatabase.GetAllAssetBundleNames();
        HashSet<string> abNamesSet = new HashSet<string>(allAssetBundles); // 去重
        allABNames.Clear();
        allABNames.AddRange(abNamesSet);
    }

    private void FilterABNames()
    {
        filteredABNames.Clear();

        if (string.IsNullOrEmpty(inputABName))
        {
            filteredABNames.AddRange(allABNames);
        }
        else
        {
            foreach (var abName in allABNames)
            {
                if (abName.ToLower().Contains(inputABName.ToLower()))
                {
                    filteredABNames.Add(abName);
                }
            }
        }
    }

    private void ApplyABName()
    {
        if (selectedAssets != null && selectedAssets.Length > 0)
        {
            foreach (var asset in selectedAssets)
            {
                string path = AssetDatabase.GetAssetPath(asset);
                string extension = System.IO.Path.GetExtension(path);

                // 检查文件是资源，设置 AB 名
                if (!string.IsNullOrEmpty(extension))
                {
                    Debug.Log($"设置AB名：{asset.name} -> {inputABName}");
                    AssetImporter importer = AssetImporter.GetAtPath(path);
                    importer.SetAssetBundleNameAndVariant(inputABName, "");
                }
            }
            // 刷新资产数据库
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogWarning("未选中任何资源。");
        }
    }
}
