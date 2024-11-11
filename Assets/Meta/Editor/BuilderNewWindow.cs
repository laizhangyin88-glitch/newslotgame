using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

public class BuilderNewWindow : OdinEditorWindow
{
    public enum ValueType
    {
        None,
        Number,
        String
    }

    [LabelText("渠道")]
    public ChannelType Channel = ChannelType.K3K;

    [LabelText("平台")]
    public BuildTarget TargetPlaform = BuildTarget.Android;

    [LabelText("Unity打包配置")]
    public BuildOptions BuildOptions = BuildOptions.CompressWithLz4HC;

    [LabelText("是否生成ab")]
    public bool GenerateAssetBundle = true;

    [LabelText("生成路径"), FolderPath]
    public string BuildPath;

    [PropertySpace, Button("开始打包")]
    public void StartBuild()
    {
        if (string.IsNullOrEmpty(BuildPath))
            return;


    }

    [MenuItem("Tools/Build/全平台出包")]
    public static void OpenWindow()
    {
        GetWindow<BuilderNewWindow>().Show();
    }
}
