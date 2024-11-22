using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SlotMaker
{
    [Flags]
    public enum LogFilter
    {
        System = (1 << 0),
        Unity = (1 << 1),
        NodeCanvas = (1 << 2),
        Bundle = (1 << 3),
        Scene = (1 << 4),
        Network = (1 << 5),
        Analytics = (1 << 6),
        Performance = (1 << 7),
        TestSuite = (1 << 8),
        Test = (1 << 9),
        Event = (1 << 10)
    };

    [Serializable]
    [CreateAssetMenu(fileName = "ApplicationSettings", menuName = "SlotMaker/ScriptableObject/ApplicationSettings")]
    public partial class ApplicationSettings : ScriptableObjectSingleton<ApplicationSettings>
    {
        [Title("打包设置")]
        public int applicationType;
        public string clientVersion;
        public string bundleVersion;
        [Tooltip("热更lib版本，仅用于打热更包")]
        public string libVersion;

        [Title("服务器&CDN")]

        [Tooltip("服务器链接关键字")]
        public string autoUrl;
        [Tooltip("遺留字段，參見InitBaseURL")]
        public string apiUrl;
        public string loginUrl;
        public string newLoginUrlApp;
        public string newLoginUrlMechine;
        [Tooltip("遺留字段，參見chattingApiUrl")]
        public string chattingApiUrl;
        [Space]
        [Tooltip("ab包在本地StreamingAssets中的路径，示例bundles")]
        public string bundlePath;
        [Tooltip("ab包网络base地址，示例http://8.138.140.180:8124/AssetBundles/")]
        public string bundleUrl;
        [Space]
        [Tooltip("热更dll在本地StreamingAssets中的路径，示例Lib")]
        public string libPath;
        [Tooltip("热更lib网络base地址, 示例http://8.138.140.180:8124/Lib/")]
        public string libUrl;

        [Title("网络设置")]

        [Tooltip("web请求重试次数")]
        public int webRequestRetry;
        [Tooltip("web请求超时")]
        public float webRequestTimeout;
        public float longPollTimeout;
        [Tooltip("网页图片过期天数")]
        public int webImageExpireDays;
        [Tooltip("启用ab加载超时")]
        public bool asyncLoadBundleTimeoutEnabled;
        [Tooltip("ab加载超时时间"),]
        public float asyncLoadBundleTimeout;
        [Tooltip("webImage同时加载的数量限制")]
        public int asyncLoadWebImageLimit;

        [Tooltip("预处理指令，会在打ab时覆盖PlayerSetting中的设置\n(慎用)现在打ab包的位置根据预处理指令来的，设置完后要重新编译代码才能生效，这里改了打ab包时使用的还是之前的预处理指令")]
        public string defineFlags;

        [Tooltip("启用登录认证")]
        public bool accountLogin;
        //public bool isNewNetwork;

        [Tooltip("是否是机台包")]
        public bool isMachine;

        public bool isExchangeUI;

        [Tooltip("log按类型过滤,todo：整理项目log")]
        public LogFilter logFilter;

        [Title("静态ab资源，定位ab包时会添加ApplicationType后缀，打包时会直接copy到StreamingAssets")]
        public List<string> streamingAssets = new List<string>();
        [Title("静态ab资源，打包时会直接copy到StreamingAssets")]
        public List<string> staticStreamingAssets = new List<string>();
        [Title("机台特有的静态ab资源，打机台包时会直接copy到StreamingAssets")]
        public List<string> staticMachineStreamingAssets = new List<string>();

        /// <summary>
        /// 分支名，K3K或MarsFortune，默认是SlotClientMain
        /// </summary>
        public string BranchlName
        {
            get
            {
#if K3K_TEST
                return "K3K";
#elif K3K_RELEASE
                return "K3K";
#elif MARS_FORTUNE_TEST
                return "MarsFortune";
#elif MARS_FORTUNE_RELEASE
                return "MarsFortune";
#else
                return "SlotClientMain";
#endif
            }
        }

        /// <summary>
        /// 软件版本类型,Relaese或Test
        /// </summary>
        public string SoftwareVersionType
        {
            get
            {
#if K3K_TEST || MARS_FORTUNE_TEST
                return "Test";
#elif K3K_RELEASE || MARS_FORTUNE_RELEASE
                return "Release";
#else
                return "Release";
#endif
            }
        }

        public bool isMachineOrMachineApp()
        {
            return isMachine || newLoginUrlApp.Contains(":7502");
        }



        public static int GetClientVersionNumber()
        {
            string[] versions = Instance.clientVersion.Split(new char[] { '.' });
            return Int32.Parse(versions[0]) * 10000 + Int32.Parse(versions[1]) * 100 + Int32.Parse(versions[2]);
        }
        public static int GetClientMajorVersionNumber()
        {
            string[] versions = Instance.clientVersion.Split(new char[] { '.' });
            return Int32.Parse(versions[0]);
        }

        public static string GetPlatformName()
        {
#if UNITY_EDITOR
            return GetPlatformName(EditorUserBuildSettings.activeBuildTarget);
#else
            return GetPlatformName(Application.platform);
#endif
        }

#if UNITY_EDITOR
        private static string GetPlatformName(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "iOS";
                case BuildTarget.WebGL:
                    return "Canvas";
                case BuildTarget.WSAPlayer:
                    return "Windows";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Gameroom";
                case BuildTarget.StandaloneOSX:
                    return "OSX_Standalone";
                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatform(RuntimePlatform) function.
                default:
                    return null;
            }
        }
#endif

        private static string GetPlatformName(RuntimePlatform runtimePlatform)
        {
            switch (runtimePlatform)
            {
                case RuntimePlatform.Android:
#if PLATFORM_AMAZON
                return "Amazon";
#else
                    return "Android";
#endif
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.WebGLPlayer:
                    return "Canvas";
                case RuntimePlatform.WSAPlayerARM:
                case RuntimePlatform.WSAPlayerX64:
                case RuntimePlatform.WSAPlayerX86:
                    return "Windows";
                case RuntimePlatform.WindowsPlayer:
                    return "Gameroom";
                case RuntimePlatform.OSXPlayer:
                    return "OSX_Standalone";
                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatform(RuntimePlatform) function.
                default:
                    return "UNKNOWN";
            }
        }

        #region Path

        public static string GetRemoteVersionPath()
        {
            string path = Path.Combine(GetRemoteLibPath(), "Version.txt");
            return path.Replace('\\', '/');//ftp只认/组成的路径
        }
        public static string GetRemoteBundlePath()
        {
            //            string url = Instance.bundleUrl;
            //#if K3K_TEST
            //            url += "K3K/Test/";
            //#elif K3K_REALSE
            //            url += "K3K/Realse/";
            //#elif MARS_FORTUNE_TEST
            //            url += "MarsFortune/Test/";
            //#elif MARS_FORTUNE_REALSE
            //            url += "MarsFortune/Realse/";
            //#elif SLOTCLIENT_TEST
            //            url += "SlotClientMain/Test"; 
            //#endif

            //From:whh - 2024年10月30日
            //调整网络ab包的读取路径，以适应多平台和多渠道
            return GetAbOrLibPath(Instance.bundleUrl);
        }
        public static string GetRemoteManifestPath()
        {
            string path = GetRemoteBundlePath();
            path = Path.Combine(path, GetPlatformName());
            return path.Replace('\\', '/');
        }
        public static string GetRemoteLibPath()
        {
            return GetAbOrLibPath(Instance.libUrl);
        }
        public static string GetRemoteDllPath(string name)
        {
            string path = Path.Combine(GetRemoteLibPath(), name);
            return path.Replace('\\', '/');
        }

        public static string GetStreamingMetaDataPath(string aotDllName)
        {
            return Path.Combine(GetStreamingLibPath(), "AOT", aotDllName);
        }
        public static string GetStreamingVersionPath()
        {
            return Path.Combine(GetStreamingLibPath(), "Version.txt");
        }
        public static string GetStreamingBundlePath()
        {
            return Path.Combine(Application.streamingAssetsPath, Instance.bundlePath);
        }
        public static string GetStreamingLibPath()
        {
            return Path.Combine(Application.streamingAssetsPath, Instance.libPath);
        }
        public static string GetStreamingDllPath(string name)
        {
            return Path.Combine(GetStreamingLibPath(), name);
        }

        public static string GetPerVersionPath()
        {
            return Path.Combine(GetPerLibPath(), "Version.txt");
        }
        public static string GetPerBundlePath()
        {
            return Path.Combine(Application.persistentDataPath, Instance.bundlePath);
        }
        public static string GetPerLibPath()
        {
            return Path.Combine(Application.persistentDataPath, Instance.libPath);
        }
        public static string GetPerDllPath(string name)
        {
            return Path.Combine(GetPerLibPath(), name);
        }

#if UNITY_EDITOR
        public static string GetDesktopLibPath()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string libPath = Path.Combine(desktopPath, Instance.libPath);
            libPath = GetAbOrLibPath(libPath);
            return libPath;
        }
        public static string GetDesktopVersionPath()
        {
            return Path.Combine(GetDesktopLibPath(), "Version.txt");
        }
        public static string GetDesktopDllPath(string dllName)
        {
            return Path.Combine(GetDesktopLibPath(), dllName);
        }
        public static string GetDesktopAbPath()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            path = Path.Combine(path, "Assetbundles");
            return GetAbOrLibPath(path);
        }

        public static string GetBackUpLibPath(string basePath, string version)
        {
            string path;

            if (string.IsNullOrEmpty(basePath))
                path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            else
                path = basePath;

            path = Path.Combine(path, "BackUp", Instance.libPath);
            path = GetAbOrLibPath(path);
            path = Path.Combine(path, version);
            return path;
        }
        public static string GetBackUpVersionPath(string basePath, string version)
        {
            return Path.Combine(GetBackUpLibPath(basePath, version), "Version.txt");
        }
        public static string GetBackUpAbPath(string basePath, string version)
        {
            string path;

            if (string.IsNullOrEmpty(basePath))
                path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            else
                path = basePath;

            path = Path.Combine(path, "BackUp", "Assetbundles");
            path = GetAbOrLibPath(path);
            path = Path.Combine(path, version);
            return path;
        }
#endif
        #endregion

#if UNITY_EDITOR
        /// <summary>
        /// 获取完整ab文件路径
        /// </summary>
        /// <remarks>
        /// 在<paramref name="basePath"/>后追加{分支名}/{软件版本类型}/{平台名}
        /// </remarks>
        /// <param name="basePath"></param>
        /// <param name="buildTarget"></param>
        /// <returns></returns>
        public static string GetAbOrLibPath(string basePath, BuildTarget? buildTarget)
        {
            string platformName = buildTarget == null ? GetPlatformName() : GetPlatformName(buildTarget.Value);
            string url = Path.Combine(basePath, Instance.BranchlName, Instance.SoftwareVersionType, platformName);
            url = url.Replace("\\", "/");
            return url;
        }
#endif
        public static string GetAbOrLibPath(string basePath)
        {
            string url = Path.Combine(basePath, Instance.BranchlName, Instance.SoftwareVersionType, GetPlatformName());
            url = url.Replace("\\", "/");
            return url;
        }
        

        public static string GetFullUrl(string apiEndpoint)
        {
#if DEV
            string customURL = PlayerPrefs.GetString("Custom_URL", "");
            if(string.IsNullOrEmpty(customURL))
            {
                return Instance.apiUrl + apiEndpoint;
            }

            return customURL + apiEndpoint;
#else
            return Instance.apiUrl + apiEndpoint;
#endif
        }

        public static string GetChattingFullUrl(string apiEndpoint)
        {
            return Instance.chattingApiUrl + apiEndpoint;
        }

        public static string MakeApplicationBundleName(string bundleName)
        {
            return bundleName + Instance.applicationType;
        }

        public static string GetSystemLanguage()
        {
            return "EN";

            // TODO
            // return Application.systemLanguage.ToString();
        }

        public static string GetDeviceLanguage()
        {
            string language = Application.systemLanguage.ToString();

            if (string.IsNullOrEmpty(language))
                language = "EN";

            return language;
        }

        /// <summary>
        /// 获取要复制到StreamingAssets的静态ab包名
        /// </summary>
        /// <returns></returns>
    	public static List<string> GetStreaimingAssets()
        {
            List<string> returnValue = new List<string>();

            for (int i = 0; i < Instance.streamingAssets.Count; ++i)
            {
                returnValue.Add(string.Format("{0}{1}", Instance.streamingAssets[i], Instance.applicationType).ToLower());
            }

            returnValue.AddRange(Instance.staticStreamingAssets);


            if (Instance.isMachine)
            {
                returnValue.AddRange(Instance.staticMachineStreamingAssets);
            }

            return returnValue;
        }

        public static List<string> GetMachineStreamingAssets()
        {
            List<string> returnValue = new List<string>();

            for (int i = 0; i < Instance.staticMachineStreamingAssets.Count; ++i)
            {
                returnValue.Add(string.Format("{0}{1}", Instance.staticMachineStreamingAssets[i], Instance.applicationType).ToLower());
            }

            returnValue.AddRange(Instance.staticMachineStreamingAssets);

            return returnValue;
        }

        public static string GetDeviceModel()
        {
            string deviceModel = SystemInfo.deviceModel;
            if (string.IsNullOrEmpty(deviceModel))
            {
                deviceModel = "ModelUnknown";
            }
            return deviceModel;
        }

        public static string GetOperatingSystem()
        {
            string operatingSystem = SystemInfo.operatingSystem;
            if (string.IsNullOrEmpty(operatingSystem))
            {
                operatingSystem = "Unknown";
            }

            return operatingSystem;
        }

        public static string GetDeviceType()
        {
#if UNITY_IPHONE && !UNITY_EDITOR
            if(SystemInfo.deviceModel.Contains("iPad"))
            {
                return "IPAD";
            }
            else
            {
                return "IPHONE";
            }
#elif UNITY_ANDROID && PLATFORM_AMAZON && !UNITY_EDITOR
            return "KINDLE";
#elif UNITY_ANDROID && !UNITY_EDITOR
            return "GOOGLE";
#elif UNITY_WSA && !UNITY_EDITOR
            return "WSA";
#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
            return "GAMEROOM";
#elif UNITY_WEBGL && !UNITY_EDITOR
            return "CANVAS";
#elif UNITY_STANDALONE_OSX && !UNITY_EDITOR
            return "IPHONE"; // OSX is treated as iOS in server logic. Thus, treat this test platform like iOS.
#else
            string platform = GetPlatformName();

            if (platform == "Android")
            {
                return "GOOGLE";
            }
            else
            {
                return "IPHONE";
            }
#endif
        }

        public static string GetCachePath()
        {
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
            return Application.dataPath;
#else
            return Application.temporaryCachePath;
#endif
        }

        public static string GetApplicationStage()
        {
#if BUILD_RROD
            return "prod";
#elif BUILD_ST
            return "st";
#elif BUILD_QA
            return "qa";
#elif BUILD_QA_DEV
            return "qa_dev";
#else
            return "dev";
#endif
        }

        public static bool LogSystem()
        {
            return (Instance.logFilter & LogFilter.System) == LogFilter.System;
        }

        public static bool LogUnity()
        {
            return (Instance.logFilter & LogFilter.Unity) == LogFilter.Unity;
        }

        public static bool LogNodeCanvas()
        {
            return (Instance.logFilter & LogFilter.NodeCanvas) == LogFilter.NodeCanvas;
        }

        public static bool LogBundle()
        {
            return (Instance.logFilter & LogFilter.Bundle) == LogFilter.Bundle;
        }

        public static bool LogScene()
        {
            return (Instance.logFilter & LogFilter.Scene) == LogFilter.Scene;
        }

        public static bool LogNetwork()
        {
            return (Instance.logFilter & LogFilter.Network) == LogFilter.Network;
        }

        public static bool LogAnalytics()
        {
            return (Instance.logFilter & LogFilter.Analytics) == LogFilter.Analytics;
        }

        public static bool LogPerformance()
        {
            return (Instance.logFilter & LogFilter.Performance) == LogFilter.Performance;
        }

        public static bool LogTestSuite()
        {
            return (Instance.logFilter & LogFilter.TestSuite) == LogFilter.TestSuite;
        }

        public static bool LogTest()
        {
            return (Instance.logFilter & LogFilter.Test) == LogFilter.Test;
        }
    }
}
