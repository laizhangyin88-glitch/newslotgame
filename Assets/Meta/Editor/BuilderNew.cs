using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using SlotMaker.Json;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using System;
using BagelCode;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// 此脚本属于<see cref="Builder"/>的扩展，以适应新的打包需求
/// </remarks>
public static class BuilderNew
{
    /// <summary>
    /// 桌面/AssetBundles/{渠道名}/{平台名}
    /// </summary>
    /// <param name="channelType"></param>
    /// <returns></returns>
    public static string GetAssetBundleBuildPath(BuildTarget buildTarget, ChannelType channelType)
    {
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        return Path.Combine(desktopPath, "AssetBundles", channelType.ToString(), buildTarget.ToString());
    }

    public static void AllPlatformsBuild(BuildTarget buildTarget, ChannelType channelType, string buildPath, bool isGenerateAB, BuildOptions buildOptions)
    {
        //切换平台
        BuildTargetGroup buildTargetGroup = ConvertBuildTarget(buildTarget);
        EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);

        //打包ab
        if (isGenerateAB)
        {
            string ABTargetPath = GetAssetBundleBuildPath(buildTarget, channelType);
            Builder.Build_Assetbundles(ABTargetPath, BuildAssetBundleOptions.ChunkBasedCompression, buildTarget, true, true);
        }

        //开始构建
        Builder.BuildPlayer(buildTarget, buildTargetGroup, buildPath, ApplicationSettings.Instance.defineFlags, buildOptions);
    }

    static BuildTargetGroup ConvertBuildTarget(BuildTarget buildTarget)
    {
        switch (buildTarget)
        {
            case BuildTarget.StandaloneOSX:
            case BuildTarget.iOS:
                return BuildTargetGroup.iOS;
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneLinux:
            case BuildTarget.StandaloneWindows64:
            case BuildTarget.StandaloneLinux64:
            case BuildTarget.StandaloneLinuxUniversal:
                return BuildTargetGroup.Standalone;
            case BuildTarget.Android:
                return BuildTargetGroup.Android;
            case BuildTarget.WebGL:
                return BuildTargetGroup.WebGL;
            case BuildTarget.WSAPlayer:
                return BuildTargetGroup.WSA;
            case BuildTarget.Tizen:
                return BuildTargetGroup.Tizen;
            case BuildTarget.PSP2:
                return BuildTargetGroup.PSP2;
            case BuildTarget.PS4:
                return BuildTargetGroup.PS4;
            case BuildTarget.PSM:
                return BuildTargetGroup.PSM;
            case BuildTarget.XboxOne:
                return BuildTargetGroup.XboxOne;
            case BuildTarget.N3DS:
                return BuildTargetGroup.N3DS;
            case BuildTarget.WiiU:
                return BuildTargetGroup.WiiU;
            case BuildTarget.tvOS:
                return BuildTargetGroup.tvOS;
            case BuildTarget.Switch:
                return BuildTargetGroup.Switch;
            case BuildTarget.NoTarget:
            default:
                return BuildTargetGroup.Standalone;
        }
    }
}

public enum ChannelType
{
    K3K,
    MarsFortune
}

