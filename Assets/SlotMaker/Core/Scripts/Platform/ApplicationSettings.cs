using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SlotMaker
{
    [Flags]
    public enum LogFilter
    {
    	System 		= (1 << 0),
    	Unity		= (1 << 1),
    	NodeCanvas	= (1 << 2),
    	Bundle		= (1 << 3),
    	Scene		= (1 << 4),
    	Network		= (1 << 5),
    	Analytics	= (1 << 6),
    	Performance	= (1 << 7),
    	TestSuite	= (1 << 8),
    	Test		= (1 << 9)
    };

    [CreateAssetMenu(fileName="ApplicationSettings", menuName="SlotMaker/ScriptableObject/ApplicationSettings")]
    public class ApplicationSettings : ScriptableObjectSingleton<ApplicationSettings>
    {
    	public int applicationType;
    	public string clientVersion;
        public string bundleVersion;

        public string autoUrl;
        public string apiUrl;
        public string loginUrl;
        public string newLoginUrlApp;
        public string newLoginUrlMechine;
        public string chattingApiUrl;
    	public string bundlePath;
    	public string bundleUrl;

    	public int webRequestRetry;
    	public float webRequestTimeout;
    	public float longPollTimeout;
        public int webImageExpireDays;

        public bool asyncLoadBundleTimeoutEnabled;
    	public float asyncLoadBundleTimeout;
    	public int asyncLoadWebImageLimit;

    	public string defineFlags;

        public bool accountLogin;
        //public bool isNewNetwork;

        public bool isMachine;

        public List<string> streamingAssets = new List<string>();
        public List<string> staticStreamingAssets = new List<string>();
        public List<string> staticMachineStreamingAssets = new List<string>();


        public bool isMachineOrMachineApp()
        {
           return  isMachine || newLoginUrlApp.Contains(":7502");
        }

        public LogFilter logFilter { get; set; }

    	public static int GetClientVersionNumber()
    	{
    		string[] versions = Instance.clientVersion.Split(new char[]{ '.' });
    		return Int32.Parse(versions[0]) * 10000 + Int32.Parse(versions[1]) * 100 + Int32.Parse(versions[2]);
    	}
    	public static int GetClientMajorVersionNumber()
    	{
    		string[] versions = Instance.clientVersion.Split(new char[]{ '.' });
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

    	public static string GetStreamingAssetsPath()
    	{
            return Application.streamingAssetsPath;
    	}
        public static string GetAssetBundlesPath()
        {
            return Instance.bundleUrl;
        }
        public static string GetStreamingBundlePath()
    	{
    		return Path.Combine(GetStreamingAssetsPath(), Instance.bundlePath);
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

            if(string.IsNullOrEmpty(language))
                language = "EN";

            return language;
        }

    	public static List<string> GetStreaimingAssets()
    	{
    		List<string> returnValue = new List<string>();

    		 for (int i = 0; i < Instance.streamingAssets.Count; ++i)
    		 {
    		 	returnValue.Add( string.Format("{0}{1}", Instance.streamingAssets[i], Instance.applicationType).ToLower() );
    		 }

             returnValue.AddRange(Instance.staticStreamingAssets);


            if (Instance.isMachine)
            {
                returnValue.AddRange(Instance.staticMachineStreamingAssets);
            }

    		return returnValue;
    	}

        public static string GetDeviceModel()
        {
            string deviceModel = SystemInfo.deviceModel;
            if (string.IsNullOrEmpty (deviceModel)) {
                deviceModel = "ModelUnknown";
            }
            return deviceModel;
        }

        public static string GetOperatingSystem()
        {
            string operatingSystem = SystemInfo.operatingSystem;
            if (string.IsNullOrEmpty (operatingSystem)) {
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

            if(platform == "Android")
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
            return  (Instance.logFilter & LogFilter.Bundle) == LogFilter.Bundle;
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
