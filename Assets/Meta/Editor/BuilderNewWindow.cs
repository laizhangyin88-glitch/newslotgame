using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using BagelCode;
using System;
using System.Linq;
using SlotMaker;
using HybridCLR.Editor.Commands;
using System.IO;
using System.Reflection;

[TypeInfoBox("<color=yellow>unity内的打包流程已封装在此窗口\n按顺序一一确认/操作\n有任何问题请滴滴whh</color>")]
public class BuilderNewWindow : OdinEditorWindow
{
    [InfoBox("这里确认正确即可，不是必须要操作")]
    [LabelText("平台"), ValueDropdown("GetBuildTargetArray"), InlineButton("SwitchPlatform", "切换"), BoxGroup(GroupID = "1")]
    public BuildTarget TargetPlaform = BuildTarget.Android;

    [BoxGroup(GroupID = "1"), LabelText("渠道")]
    public ChannelType Channel = ChannelType.K3K;

    [LabelText("软件版本"), BoxGroup(GroupID = "1")]
    public SoftwareType Software = SoftwareType.Test;

    [Button("Apply Symbol", Style = ButtonStyle.Box), BoxGroup(GroupID = "1"), GUIColor(0.3f, 0.8f, 0.8f)]
    private void SwitchChannelAndSoftware()
    {
        var cs = GetChannelAndSoftware(TargetPlaform);
        if (cs.Item1 == Channel && cs.Item2 == Software)
            return;//未发生变动，不更新

        SwitchChannelAndSoftware(TargetPlaform, Channel, Software);

        //强制编译程序集
        UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
    }

    [PropertySpace(10), LabelText("当前ab生成路径"), ReadOnly]
    public string AbBuildPath;


    [Button("打开ab路径"), HorizontalGroup("btnRow1"), PropertyOrder(20)]
    public void OpenAbBuildPath()
    {
        if (Directory.Exists(AbBuildPath) == false)
            Directory.CreateDirectory(AbBuildPath);

        OpenDirectoryInExplorer(AbBuildPath);
    }
    [Button("打ab包"), HorizontalGroup("btnRow1"), PropertyOrder(21), PropertyTooltip("没有资源变动可不打")]
    public void BuildAb()
    {
        BuildTargetGroup group = BuilderNew.ConvertBuildTarget(TargetPlaform);
        Builder.Build_Assetbundle(TargetPlaform, group, null, true, true, null);
    }

    [PropertySpace(10), LabelText("(HybridCLR)生成linkXml"), Button, PropertyOrder(22)]
    public void GenerateLinkXml()
    {
        LinkGeneratorCommand.GenerateLinkXml();
    }


    [LabelText("生成路径"), FolderPath(AbsolutePath =true, RequireExistingPath = true), InlineButton("BuildProject", "导出工程"), OnValueChanged("SaveBuildPath"), Delayed, PropertyOrder(23)]
    public string BuildPath;
    private void SaveBuildPath()
    {
        if (string.IsNullOrEmpty(BuildPath))
            return;

        EditorPrefs.SetString("lastBuildProjectPath", BuildPath);
    }

    //[PropertySpace, Button("开始打包")]
    
    private void Btn()
    {
        Il2CppDefGeneratorCommand.GenerateIl2CppDef();
        MethodBridgeGeneratorCommand.GenerateMethodBridgeAndReversePInvokeWrapper();
        //todo
        //将 {proj}\HybridCLRData\LocalIl2CppData-{platform}\il2cpp\libil2cpp\hybridclr\generated目录 替换导出工程中的此目录。
        //在导出工程上执行build
    }




    [MenuItem("Tools/Build/全平台出包")]
    public static void OpenWindow()
    {
        GetWindow<BuilderNewWindow>().Show();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        TargetPlaform = EditorUserBuildSettings.activeBuildTarget;
        AbBuildPath = GetAbBuildPath();
        var cs = GetChannelAndSoftware(TargetPlaform);
        Channel = cs.Item1;
        Software = cs.Item2;
        string lastBuildProjectPath = EditorPrefs.GetString("lastBuildProjectPath", "");
        BuildPath = lastBuildProjectPath;
    }

    private void BuildProject()
    {
        if (string.IsNullOrEmpty(BuildPath))
            return;

        BuildTargetGroup group = BuilderNew.ConvertBuildTarget(TargetPlaform);
        Builder.BuildPlayer(TargetPlaform, group, BuildPath, null, BuildOptions.None);
    }

    private void SwitchPlatform()
    {
        BuilderNew.SwitchPlatform(TargetPlaform);
    }

    private (ChannelType, SoftwareType) GetChannelAndSoftware(BuildTarget buildTarget)
    {
        BuildTargetGroup group = BuilderNew.ConvertBuildTarget(buildTarget);
        string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        if (string.IsNullOrEmpty(symbols))
            return default;

        string[] channelNames = Enum.GetNames(typeof(ChannelType));
        for (int i = 0; i < channelNames.Length; i++)
        {
            channelNames[i] = channelNames[i].ToUpper();
        }

        string[] symbolArr = symbols.Split(';');
        string targetSymbol = symbolArr.First((str) =>
        {
            foreach (var channelName in channelNames)
            {
                if (str.Contains(channelName))
                    return true;
            }

            return false;
        });

        if (string.IsNullOrEmpty(targetSymbol))
            return default;

        int lastIndex = targetSymbol.LastIndexOf('_');
        if (lastIndex == -1)
            return default;

        string channelStr = targetSymbol.Substring(0, lastIndex);
        string softwareStr = targetSymbol.Substring(lastIndex + 1);

        if (Enum.TryParse(channelStr, true, out ChannelType channelEnum) == false ||
            Enum.TryParse(softwareStr, true, out SoftwareType softwareEnum) == false)
            return default;

        return (channelEnum, softwareEnum);
    }

    private void SwitchChannelAndSoftware(BuildTarget buildTarget,ChannelType channelType,  SoftwareType softwareType)
    {
        BuildTargetGroup group = BuilderNew.ConvertBuildTarget(buildTarget);
        string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        if (string.IsNullOrEmpty(symbols))
            return;

        string[] symbolArr = symbols.Split(';');
        List<string> symbolList = new List<string>(symbolArr);
        string[] channelNames = Enum.GetNames(typeof(ChannelType));
        for (int i = 0; i < channelNames.Length; i++)
        {
            channelNames[i] = channelNames[i].ToUpper();
        }

        IEnumerable<string> symbolArrNew = symbolArr.Where(item =>
        {
            foreach (var channelName in channelNames)
            {
                if (item.Contains(channelName))
                    return false;//剔除
            }

            return true;//保留
        });

        string softwareKey = softwareType.ToString().ToUpper();
        string channelKey = channelType.ToString().ToUpper();

        List<string> symbolListNew = new List<string>(symbolArrNew);
        symbolListNew.Add($"{channelKey}_{softwareKey}");
        
        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, symbolListNew.ToArray());
        Debug.Log($"调整Symbols，原：{symbols}， 现：{string.Join(";", symbolListNew)}");
    }

    private BuildTarget[] GetBuildTargetArray()
    {
        return new BuildTarget[]
        {
            BuildTarget.Android,
            BuildTarget.iOS,
            BuildTarget.StandaloneWindows,
        };
    }

    private string GetAbBuildPath()
    {
        string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        path = Path.Combine(path, "Assetbundles");
        return ApplicationSettings.GetAbOrLibPath(path);
    }
    

    private static void OpenDirectoryInExplorer(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            // 根据不同的操作系统使用不同的命令来打开目录
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                //explorer.exe只接受反斜杠路径
                directoryPath = directoryPath.Replace('/', '\\');
                System.Diagnostics.Process.Start("explorer.exe", directoryPath);
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                System.Diagnostics.Process.Start("open", directoryPath);
            }
            else
            {
                Debug.LogError("不支持的操作系统平台。");
            }
        }
        else
        {
            Debug.LogError("指定的目录不存在：" + directoryPath);
        }
    }

    public enum ValueType
    {
        None,
        Number,
        String
    }
}
