using SlotMaker;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class StartUpConfig
{
    public const string url = "http://8.138.140.180:8124/Lib/";

    public static Dictionary<string, AssetBundle> bundleDic = new Dictionary<string, AssetBundle>();
    public static string DllPath
    {
        get
        {
            //string path = Application.dataPath + "/StreamingAssets/Lib";
            //if (!Application.isEditor)
            //    path = Application.persistentDataPath;
            //return path;

            if (Application.isEditor)
                return ApplicationSettings.GetStreamingLibPath();
            else
                return ApplicationSettings.GetPerLibPath();
        }
    }

    public static string AssetBundlePath
    {
        get
        {
            //string path = Application.dataPath + "/StreamingAssets/AssetBundles";
            //if (!Application.isEditor)
            //    path = Application.persistentDataPath;
            //return path;

            if (Application.isEditor)
                return ApplicationSettings.GetStreamingBundlePath();
            else
                return ApplicationSettings.GetPerBundlePath();
        }
    }

    public static string VersionPath
    {
        get
        {
            //string path = Application.dataPath + "/StreamingAssets/Version.txt";
            //if (!Application.isEditor)
            //    path = Application.persistentDataPath + "/Version.txt";
            //return path;

            if (Application.isEditor)
                return ApplicationSettings.GetStreamingVersionPath();
            else
                return ApplicationSettings.GetPerVersionPath();
        }
    }

    
    

}
