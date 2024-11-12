using BagelCode;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using SlotMaker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

//[TypeInfoBox("<color=yellow>unity内的打包流程已封装在此窗口\n按顺序一一确认/操作\n有任何问题请滴滴whh</color>")]
public class BuilderNewWindow : OdinEditorWindow
{
    [PropertySpace(SpaceBefore = 20)]


    [Title("环境配置(这里确认正确即可，不是必须要操作)")]
    [PropertyOrder(1), LabelText("平台"), ValueDropdown("GetBuildTargetArray"), InlineButton("SwitchPlatform", "切换")]
    public BuildTarget TargetPlaform = BuildTarget.Android;

    [PropertyOrder(2), LabelText("渠道")]
    public ChannelType Channel = ChannelType.K3K;

    [PropertyOrder(3), LabelText("软件版本")]
    public SoftwareType Software = SoftwareType.Test;

    [PropertyOrder(4), Button("Apply Symbol", Style = ButtonStyle.Box), GUIColor(0.3f, 0.8f, 0.8f)]
    public void SwitchChannelAndSoftware()
    {
        var cs = GetChannelAndSoftware(TargetPlaform);
        if (cs.Item1 == Channel && cs.Item2 == Software)
            return;//未发生变动，不更新

        SwitchChannelAndSoftware(TargetPlaform, Channel, Software);

        //强制编译程序集
        UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
    }


    [PropertySpace(SpaceBefore = 20)]
    [PropertyOrder(5), Title("项目导出")]
    public string ClientVersion;


    [PropertyOrder(6), LabelText("ab生成路径"), ShowInInspector, Sirenix.OdinInspector.FilePath(), ReadOnly, HorizontalGroup("abBuild")]
    public string AbBuildPath;

    [PropertyOrder(7), Button("打ab包"), PropertyTooltip("没有资源变动可不打"), HorizontalGroup("abBuild")]
    public void BuildAb()
    {
        BuildTargetGroup group = BuilderNew.ConvertBuildTarget(TargetPlaform);
        Builder.Build_Assetbundle(TargetPlaform, group, null, true, true, null);
        AssetDatabase.Refresh();
    }

    [PropertyOrder(8), Button("生成Version文件"), PropertyTooltip("一份到StreamingAssets(随包打出)，一份到桌面(上传到cdn)")]
    public void GenVersionFile()
    {
        Debug.Log("生成版本文件");
        GenVersionFileToStreamingAssetsAndDesktop();
        AssetDatabase.Refresh();
    }

    [PropertyOrder(9), Button("生成热更dll"), PropertyTooltip("一份到StreamingAssets(随包打出)，一份到桌面(上传到cdn)")]
    public void GenHotUpdateDll()
    {
        Debug.Log("生成热更dll");
        CompileDll();
        AssetDatabase.Refresh();
    }

    [PropertyOrder(10), LabelText("项目生成路径"), FolderPath(AbsolutePath = true, RequireExistingPath = true), OnValueChanged("SaveBuildPath"), Delayed, HorizontalGroup("build"), InlineButton("BuildProject", "导出工程")]
    public string BuildPath;

    

    [LabelText("Android项目路径"), FolderPath(AbsolutePath = true, RequireExistingPath = true), PropertyTooltip("定义出包的Android Studio项目路径\n通过“复制导入”将Unity导出的Android项目的必要部分复制到Android打包项目中\n避免了手动操作"), OnValueChanged("SaveAndroidStudioProjectPath"), Delayed, HorizontalGroup("android"), PropertyOrder(11), InlineButton("ImprotProject", "复制导入")]
    public string AndroidStudioProjectPath;

    

    [MenuItem("Tools/全平台出包")]
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
        BuildPath = EditorPrefs.GetString("lastBuildProjectPath", "");
        AndroidStudioProjectPath = EditorPrefs.GetString("androidStudioProjectPath", "");
    }

    protected void ImprotProject()
    {
        if (Directory.Exists(AndroidStudioProjectPath) == false)
            return;

        if (Directory.Exists(BuildPath) == false)
            return;

        string targetPath = Path.Combine(AndroidStudioProjectPath, "unityLibrary", "src", "main");
        string targetAssetsPath = Path.Combine(targetPath, "assets");
        string targetIl2CppPath = Path.Combine(targetPath, "Il2CppOutputProject");

        if (Directory.Exists(targetAssetsPath) == false)
        {
            Debug.LogError($"不存在目录{targetAssetsPath}");
            return;
        }

        if (Directory.Exists(targetIl2CppPath) == false)
        {
            Debug.LogError($"不存在目录{targetIl2CppPath}");
            return;
        }

        string sourcePath = Path.Combine(BuildPath, "unityLibrary", "src", "main");
        string sourceAssetsPath = Path.Combine(sourcePath, "assets");
        string sourceIl2CppPath = Path.Combine(sourcePath, "Il2CppOutputProject");

        if (Directory.Exists(sourceAssetsPath) == false)
        {
            Debug.LogError($"不存在目录{sourceAssetsPath}");
            return;
        }

        if (Directory.Exists(sourceIl2CppPath) == false)
        {
            Debug.LogError($"不存在目录{sourceIl2CppPath}");
            return;
        }

        Debug.Log("将生成的AndroidStudio项目文件导入到打包项目中...");
        Directory.Delete(targetAssetsPath, true);
        Directory.Delete(targetIl2CppPath, true);

        Debug.Log($"{sourceAssetsPath} -> {targetAssetsPath}");
        Debug.Log($"{sourceIl2CppPath} -> {targetIl2CppPath}");

        CopyDirectory(sourceAssetsPath, targetAssetsPath, true);
        CopyDirectory(sourceIl2CppPath, targetIl2CppPath, true);
    }

    /// <summary>
    /// 生成热更dll，一份到StreamingAssets(随包打出)，一份到桌面(上传到cdn)
    /// </summary>
    private void CompileDll()
    {
        CompileDllCommand.CompileDllActiveBuildTarget();
        string hotUpdateDllSourcePath = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(TargetPlaform);

        string deskLibPath = ApplicationSettings.GetDesktopLibPath();
        if (Directory.Exists(deskLibPath) == false)
            Directory.CreateDirectory(deskLibPath);

        if (Directory.Exists(ApplicationSettings.GetStreamingLibPath()))
            Directory.CreateDirectory(ApplicationSettings.GetStreamingLibPath());

        foreach (var dllName in Main.dllList)
        {
            string filePath = Path.Combine(hotUpdateDllSourcePath, dllName);
            if(File.Exists(filePath) == false)
            {
                Debug.LogError($"热更文件不存在：{filePath}");
                continue;
            }

            File.Copy(filePath, ApplicationSettings.GetDesktopDllPath(dllName), true);
            File.Copy(filePath, ApplicationSettings.GetStreamingDllPath(dllName + ".bytes"), true);
        }
    }

    #region HybridCLRHelper

    protected void ResetHotUpdateOutputPath()
    {
        GenVersionFileToStreamingAssetsAndDesktop();
        //HybridCLRSettings.Instance.hotUpdateDllCompileOutputRootDir = 
    }

    private void CopyMetaAOTAndHotUpdateDllToProject(string projectPath)
    {
        SettingsUtil.GetAssembliesPostIl2CppStripDir(TargetPlaform);
    }

    #endregion

    /// <summary>
    /// 生成版本文件，一份到StreamingAssets(随包打出)，一份到桌面(上传到cdn)
    /// </summary>
    protected void GenVersionFileToStreamingAssetsAndDesktop()
    {
        VersionData vd = StartUpUtils.CreateVersionData(ApplicationSettings.Instance.libVersion);
        StartUpUtils.SaveVersionData(vd, ApplicationSettings.GetStreamingLibPath());
        StartUpUtils.SaveVersionData(vd, ApplicationSettings.GetDesktopLibPath());
    }

    protected void BuildProject()
    {
        if (string.IsNullOrEmpty(BuildPath))
            return;

        if (Directory.Exists(BuildPath) == false)
        {
            EditorUtility.DisplayDialog("警告", "导出目录不存在", "ok");
            return;
        }

        if (EditorUtility.DisplayDialog("确认", $"导出前是否清空导出目录?\n{BuildPath}", "yes", "no"))
        {
            Directory.Delete(BuildPath, true);
            Directory.CreateDirectory(BuildPath);
        }

        //项目太大，正常打热更工程时间会增加一倍
        //按HybridCLR推荐流程进行优化
        //参见：https://hybridclr.doc.code-philosophy.com/docs/basic/buildpipeline#%E4%BC%98%E5%8C%96%E7%9A%84%E6%89%93%E5%8C%85%E6%B5%81%E7%A8%8B

        //运行 HybridCLR/ Generate / LinkXml
        LinkGeneratorCommand.GenerateLinkXml();

        //导出工程
        BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(TargetPlaform);
        Builder.BuildPlayer(TargetPlaform, group, BuildPath, null, BuildOptions.None);

        //运行 HybridCLR / Generate / Il2cppDef
        Il2CppDefGeneratorCommand.GenerateIl2CppDef();

        //运行 HybridCLR/ Generate / MethodBridge生成桥接函数
        //运行 HybridCLR/ Generate / PReverseInvokeWrapper。 不需要与lua之类交互的项目可跳过此步。
        MethodBridgeGeneratorCommand.GenerateMethodBridgeAndReversePInvokeWrapper();

        //将 { proj}\HybridCLRData\LocalIl2CppData -{ platform}\il2cpp\libil2cpp\hybridclr\generated目录 替换导出工程中的此目录。
        string sourcePath = SettingsUtil.GeneratedCppDir;
        string targetPath = $"{BuildPath}/unityLibrary/src/main/Il2CppOutputProject/IL2CPP/libil2cpp/hybridclr/generated";
        Directory.Delete(targetPath, true);
        CopyDirectory(sourcePath, targetPath, true);


        //自动化流程：将AOT元数据程序集复制到导出工程的StreamingAssets中
        string sourceAotPath = SettingsUtil.GetAssembliesPostIl2CppStripDir(TargetPlaform);
        string targetAotPath = $"{BuildPath}/unityLibrary/src/main/assets/{ApplicationSettings.Instance.libPath}/AOT";
        if (Directory.Exists(targetAotPath) == false)
            Directory.CreateDirectory(targetAotPath);
        foreach (var dllName in RefTypes.AOTMetaAssemblyFiles)
        {
            string sourceAOTDllPath = Path.Combine(sourceAotPath, dllName);
            if(File.Exists(sourceAOTDllPath) == false)
            {
                Debug.LogError($"AOT程序集不存在：{sourceAOTDllPath}");
                continue;
            }

            Debug.Log($"Copy AOT程序集: {sourceAOTDllPath} -> {targetAotPath}");
            string targetAotDllPath = Path.Combine(targetAotPath, dllName + ".bytes");
            File.Copy(sourceAOTDllPath, targetAotDllPath, true);
        }

        //在导出工程上执行build

    }

    

    private void SaveBuildPath()
    {
        if (string.IsNullOrEmpty(BuildPath))
            return;

        EditorPrefs.SetString("lastBuildProjectPath", BuildPath);
    }

    private void SaveAndroidStudioProjectPath()
    {
        if (string.IsNullOrEmpty(AndroidStudioProjectPath))
            return;

        EditorPrefs.SetString("androidStudioProjectPath", AndroidStudioProjectPath);
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

    private void SwitchChannelAndSoftware(BuildTarget buildTarget, ChannelType channelType, SoftwareType softwareType)
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

    static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
    {
        // Get information about the source directory
        var dir = new DirectoryInfo(sourceDir);

        // Check if the source directory exists
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        // Cache directories before we start copying
        DirectoryInfo[] dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(destinationDir);

        // Get the files in the source directory and copy to the destination directory
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
    }

    public enum ValueType
    {
        None,
        Number,
        String
    }
}
