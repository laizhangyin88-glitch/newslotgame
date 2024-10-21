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

namespace BagelCode
{

    public static class Builder
    {
        static string TARGET_DIR = "/tmp";
        const string PROJECT_PATH_KEY = "buildPath";//构建的目标路径（默认"/tmp"）
        const string FORCE_REBUILD_KEY = "force";
        const string DEVELOP_BUILD_KEY = "develop";//构建开发版本（默认false）
        const string USE_PROFILE_KEY = "profile";//构建启用所有分析调试的开发版本（默认false）
        const string IDENTIFIER_KEY = "pkgname";//包名(默认com.bagelcode.v3proto)
        const string BUNLDLE_VERSION_KEY = "bundleVersion";//资源版本（默认不设置）
        const string BUILD_ASSET_BUNDLE_KEY = "assetBundlePath";
        const string DEFAULT_IDENTIFIER = "com.bagelcode.v3proto";
        const string BUILD_CONFIG_KEY = "target";
        const string PLATFORM_CONFIG_KEY = "platform";//定义PLATFORM_{0}的预编译指令【没有用上】
        const string ASANA_ACCESS_TOKEN = "0/503c582de30b5114ddf512f521aef1f2";
        const string NATIVE_BUILD_TARGET = "nativeBuildTarget";//定义BUILD_{0}的预编译指令【没有用上】
        const string APP_NAME = "appName";//渠道名，【没有用上】
        const string SKIP_PLAYER = "skipPlayer";
        const string IGNORE_ASSETBUNDLE_DEPENDENCY_KEY = "ignoreAssetbundleDependency";//忽略ab包依赖（默认false）
        const string SKIP_LZ4HC_COMPRESSION = "skipLz4HCCompression";//跳过LZ4高压缩（默认使用）
        const string DEPENDENCY_WHITE_LIST_PATH = "Assets/Meta/Editor/BuilderAssetDependency_IgnoreDict.json";

        static Dictionary<string, List<string>> DEPENDENCY_WHITE_LIST_DICT = new Dictionary<string, List<string>>();

        static void SetEditorPrefUsingEnv(string targetKey, string envKey)
        {
            string currentValue = EditorPrefs.GetString(targetKey, "");
            string envVar = System.Environment.GetEnvironmentVariable(envKey);

            if (currentValue == "")
            {
                // Not set. Environment Variable must be present.
                if (envVar == null || envVar == "")
                {
                    string errMessage = "Failed to set EditorPref '" + targetKey + "' from EnvVar '" + envKey + "'. EnvVar is unset.";
                    Debug.LogError(errMessage);
                    throw new System.InvalidOperationException(errMessage);
                }
                EditorPrefs.SetString(targetKey, envVar);
                Debug.Log("Set EditorPref '" + targetKey + "' from EnvVar '" + envKey + "':" + envVar);
            }
            else
            {
                // Override using envVar
                if (envVar == null || envVar == "")
                {
                    Debug.Log("Skip setting EditorPref '" + targetKey + "' from EnvVar '" + envKey + "'. EnvVar is unset. Use current setting instead:" + currentValue);
                    return;
                }
                EditorPrefs.SetString(targetKey, envVar);
                Debug.Log("Override EditorPref '" + targetKey + "' using EnvVar '" + envKey + "': Current value was " + currentValue + ". New value is " + envVar);
            }
        }

        static void MakeSureAndroidPathsAreSet()
        {
            SetEditorPrefUsingEnv("AndroidSdkRoot", "ANDROID_HOME");
            SetEditorPrefUsingEnv("AndroidNdkRoot", "ANDROID_NDK_HOME");
        }

        /// <summary>
        /// 获取构建场景的文件路径列表
        /// </summary>
        /// <returns></returns>
        public static string[] GetScenePaths()
        {
            string[] scenes = new string[EditorBuildSettings.scenes.Length];
            for (int i = 0; i < scenes.Length; i++)
            {
                scenes[i] = EditorBuildSettings.scenes[i].path;
            }
            return scenes;
        }

        /// <summary>
        /// 获取命令行执行unity的参数
        /// </summary>
        /// <returns></returns>
        static Dictionary<string, string> GetAssetBuildParams()
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            foreach (string argument in System.Environment.GetCommandLineArgs())
            {
                Match m = Regex.Match(argument, @"-(?<prop>[^:]+):(?<value>.+)$");
                if (m.Success)
                {
                    string property = m.Groups["prop"].ToString();
                    param.Add(property, m.Groups["value"].ToString());
                }
            }
            return param;
        }

        public static UnityEditor.Build.Reporting.BuildReport BuildPlayer(BuildTarget buildTarget, BuildTargetGroup buildTargetGroup, string path, string flags, BuildOptions buildOptions, string identifier = DEFAULT_IDENTIFIER)
        {
            AssetDatabase.Refresh();

            PlayerSettings.bundleVersion = ProductSettings.Instance.productVersion;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, flags);

            PlayerSettings.stripEngineCode = true;

            // Platform specific setting
            if (buildTarget == BuildTarget.Android)
            {
                PlayerSettings.Android.bundleVersionCode = ProductSettings.GetProductVersionNumber();
                PlayerSettings.applicationIdentifier = identifier;

                EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
                // Suppress APK compilation. exportAsGoogleAndroidProject seems not working
                EditorUserBuildSettings.exportAsGoogleAndroidProject = true;

                // For Android, generate project folder. For iOS, modify skeleton proejct.
                buildOptions |= BuildOptions.AcceptExternalModificationsToPlayer;
            }
            else if (buildTarget == BuildTarget.iOS)
            {
                // PlayerSettings.iPhoneBundleIdentifier = identifier;
                PlayerSettings.SetApplicationIdentifier(buildTargetGroup, identifier);

                // For Android, generate project folder. For iOS, modify skeleton proejct.
                buildOptions |= BuildOptions.AcceptExternalModificationsToPlayer;
            }
            else if (buildTarget == BuildTarget.WSAPlayer)
            {
                PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
                PlayerSettings.defaultScreenWidth = 1600;
                PlayerSettings.defaultScreenHeight = 1000;

                EditorUserBuildSettings.wsaSubtarget = WSASubtarget.AnyDevice;
                EditorUserBuildSettings.wsaUWPBuildType = WSAUWPBuildType.XAML;
                EditorUserBuildSettings.wsaBuildAndRunDeployTarget = WSABuildAndRunDeployTarget.LocalMachine;
                EditorUserBuildSettings.wsaUWPVisualStudioVersion = string.Empty;
                EditorUserBuildSettings.wsaUWPSDK = string.Empty;
                EditorUserBuildSettings.wsaMinUWPSDK = "10.0.14393.0";
                EditorUserBuildSettings.wsaArchitecture = "x64";
            }
            else if (buildTarget == BuildTarget.StandaloneWindows || buildTarget == BuildTarget.StandaloneWindows64)
            {
                // first 4 lines are required by gameroom policy
                PlayerSettings.captureSingleScreen = true;
                /// BAGELCODE
                /// warning CS0618: 'PlayerSettings.displayResolutionDialog' is obsolete: 'displayResolutionDialog is deprecated and will be removed in future versions.'
                // PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.HiddenByDefault;
                PlayerSettings.resizableWindow = false;
                PlayerSettings.allowFullscreenSwitch = false;
                // PlayerSettings.Facebook.sdkVersion = "7.9.4";

                AudioConfiguration audioConfig = AudioSettings.GetConfiguration();
                audioConfig.dspBufferSize = 1024;
                AudioSettings.Reset(audioConfig);
            }
            else if (buildTarget == BuildTarget.WebGL)
            {
                // PlayerSettings.stripEngineCode = true;
            }
            else if (buildTarget == BuildTarget.StandaloneOSX)
            {
                PlayerSettings.macOS.buildNumber = ProductSettings.Instance.productVersion;
                PlayerSettings.resizableWindow = true;
            }

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = GetScenePaths();
            buildPlayerOptions.locationPathName = path;
            buildPlayerOptions.target = buildTarget;
            buildPlayerOptions.targetGroup = buildTargetGroup;
            buildPlayerOptions.options = buildOptions;
            return BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        // private static string GetPlatform()
        // {
        //  switch (Application.platform)
        //  {
        //  case RuntimePlatform.IPhonePlayer:
        //      return "236199784943687";
        //  case RuntimePlatform.Android:
        //      return "236199784943688";
        //  }

        //  return "247732496605193";//UnityEditor
        // }

        // private static Dictionary<string, object> CreateAssetBundleDependenciesReport(string sourceBundleName, string[] targetBundleNames)
        // {
        //     var report = new Dictionary<string, object>();
        //     report["name"] = string.Format("AssetBundles Dependencies Found");
        //     report["notes"] = string.Format("{0} has dependency on {1}", sourceBundleName, string.Join(", ", targetBundleNames));
        //     report["workspace"] = 11140323218804;
        //  report["projects"] = new List<long>{ 363172799137135 };

        //  var custom_fields = new Dictionary<string, string>();
        //  custom_fields["236199784943686"] = GetPlatform();
        //  custom_fields["308614635924716"] = "308614635924717";//type = bug
        //  custom_fields["308596677866206"] = ProductSettings.Instance.clientVersion;
        //  report["custom_fields"] = custom_fields;

        //     return report;
        // }

        private static void RoadDependencyWhiteListDict()
        {
            TextAsset whiteListData = (TextAsset)AssetDatabase.LoadAssetAtPath(DEPENDENCY_WHITE_LIST_PATH, typeof(TextAsset));
            DEPENDENCY_WHITE_LIST_DICT = SlotSimpleJson.DeserializeObject<Dictionary<string, List<string>>>(whiteListData.text);
        }

        private static List<string> GetWhiteList(string assetBundle)
        {
            List<string> whiteList;
            DEPENDENCY_WHITE_LIST_DICT.TryGetValue(assetBundle, out whiteList);
            return whiteList ?? new List<string>();
        }

        public static void Build_Assetbundles(string path, BuildAssetBundleOptions options, BuildTarget buildTarget, bool isDevBuild, bool ignoreAssetbundleDependency)
        {
#if !UNITY_WEBGL
            ClearWebGLAssets();
#endif

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            RoadDependencyWhiteListDict();

            var assetBundleManifest = BuildPipeline.BuildAssetBundles(path, options, buildTarget);
            if (assetBundleManifest == null)
            {
                throw new System.InvalidOperationException("Build asset bundle failed");
            }

            string[] allAssetBundles = assetBundleManifest.GetAllAssetBundles();
            bool assetBundleDependencyFound = false;
            foreach (string assetBundle in allAssetBundles)
            {
                string[] assetBundleDependencies = assetBundleManifest.GetAllDependencies(assetBundle);
                List<string> whiteList = GetWhiteList(assetBundle);
                assetBundleDependencies = assetBundleDependencies.Where(x => !whiteList.Contains(x)).ToArray();
                if (assetBundleDependencies.Length > 0)
                {
                    var errMessage = string.Format("{0} has dependency on {1}", assetBundle, string.Join(", ", assetBundleDependencies));
                    Debug.LogError(errMessage);
                    assetBundleDependencyFound = true;
                }
            }
            if (assetBundleDependencyFound && !ignoreAssetbundleDependency)
                throw new System.InvalidOperationException("Dependency between assetbundles found");

            CopySteamingAssets(path, isDevBuild);
        }

        public static void CopySteamingAssets(string path, bool isDevBuild)
        {
            if (!Directory.Exists(path))
            {
                Debug.LogError("Exists path Error : " + path);
                throw new System.InvalidOperationException("Copy Streaming Assets: invalid path");
            }

            List<string> streamingAssets = ApplicationSettings.GetStreaimingAssets();
            if (isDevBuild)
            {
                streamingAssets.Add("testsuite");
            }

            if (streamingAssets.Count == 0)
                return;

            string bundlePath = Path.Combine(Application.streamingAssetsPath, "bundles");
            if (Directory.Exists(bundlePath))
                Directory.Delete(bundlePath, true);
            Directory.CreateDirectory(bundlePath);

            for (int i = 0; i < streamingAssets.Count; ++i)
            {
                string srcPath = Path.Combine(path, streamingAssets[i]);
#if UNITY_ANDROID
                string dstPath = Path.Combine(bundlePath, streamingAssets[i] + ".assetbundle");
#else
                string dstPath = Path.Combine(bundlePath, streamingAssets[i]);
#endif
                try
                {
                    Debug.Log("Copy : " + srcPath + " To " + dstPath);
                    File.Copy(srcPath, dstPath, true);
                }
                catch (IOException copyError)
                {
                    Debug.Log("CopyFiled : " + srcPath + "\n" + copyError.Message);
                    throw copyError;
                }
            }
        }

        public static void ClearWebGLAssets()
        {
            var dstFolder = Path.Combine(Application.dataPath, "CanvasSupport");
            if (Directory.Exists(dstFolder))
            {
                Directory.Delete(dstFolder, true);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// 打包ab
        /// </summary>
        /// <param name="buildTarget">buildTarget</param>
        /// <param name="buildTargetGroup">buildTargetGroup</param>
        /// <param name="path">打包路径，注意依赖文件名会跟随文件夹名</param>
        /// <param name="isDevBuild">开发构建</param>
        /// <param name="ignoreAssetbundleDependency">忽略ab包依赖</param>
        /// <param name="flags">要设置的预编译指令（使用分号分隔的字符串）</param>
        private static void Build_Assetbundle(BuildTarget buildTarget, BuildTargetGroup buildTargetGroup, string path, bool isDevBuild, bool ignoreAssetbundleDependency, string flags)
        {
            AssetDatabase.Refresh();
            // Platform Change.
            if (path == null)
            {
                var specialFolderList = System.Enum.GetValues(typeof(System.Environment.SpecialFolder));
                string desktopPath = System.Environment.GetFolderPath((System.Environment.SpecialFolder)specialFolderList.GetValue(0));

                if (!System.IO.Directory.Exists(desktopPath))
                {
                    Directory.CreateDirectory(desktopPath);
                }

                path = Path.Combine(desktopPath, "" + buildTarget);
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, flags);
            // Build_Assetbundles(path, BuildAssetBundleOptions.None, buildTarget, isDevBuild);
            Build_Assetbundles(path, BuildAssetBundleOptions.ChunkBasedCompression, buildTarget, isDevBuild, ignoreAssetbundleDependency);
        }

        [MenuItem("BagelCode/Builder/Assetbundle/iOS", false, 300)]
        public static void Build_Assetbundle_iOS()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
            Build_Assetbundle(BuildTarget.iOS, BuildTargetGroup.iOS, null, true, true, ApplicationSettings.Instance.defineFlags);
        }

        [MenuItem("BagelCode/Builder/Assetbundle/Android", false, 301)]
        public static void Build_Assetbundle_Android()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            Build_Assetbundle(BuildTarget.Android, BuildTargetGroup.Android, null, true, true, ApplicationSettings.Instance.defineFlags);
        }

        [MenuItem("BagelCode/Builder/Assetbundle/Windows", false, 302)]
        public static void Build_Assetbundle_Windows()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WSA, BuildTarget.WSAPlayer);
            Build_Assetbundle(BuildTarget.WSAPlayer, BuildTargetGroup.WSA, null, true, true, ApplicationSettings.Instance.defineFlags);
        }

        [MenuItem("BagelCode/Builder/Assetbundle/Gameroom", false, 303)]
        public static void Build_Assetbundle_Gameroom()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
            Build_Assetbundle(BuildTarget.StandaloneWindows, BuildTargetGroup.Standalone, null, true, true, ApplicationSettings.Instance.defineFlags);
        }

        [MenuItem("BagelCode/Builder/Assetbundle/Canvas", false, 304)]
        public static void Build_Assetbundle_Canvas()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
            Build_Assetbundle(BuildTarget.WebGL, BuildTargetGroup.WebGL, null, true, true, ApplicationSettings.Instance.defineFlags);
        }

        /// <summary>
        /// 外部脚本编译实现（多平台）
        /// </summary>
        /// <remarks>
        /// 命令行参数见<see cref="Builder"/>常量的注释
        /// </remarks>
        static void BuildApplication()
        {
            Anima2D.SpriteMeshPostprocessor.InitializeBeforeBuild();

            // Assumes that this script is called with -buildTarget flag.
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup buildTargetGroup = BuildTargetGroup.Unknown;
            switch (buildTarget)
            {
                case BuildTarget.Android:
                    buildTargetGroup = BuildTargetGroup.Android;
                    break;

                case BuildTarget.iOS:
                    buildTargetGroup = BuildTargetGroup.iOS;
                    break;

                case BuildTarget.WSAPlayer:
                    buildTargetGroup = BuildTargetGroup.WSA;
                    break;

                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    // 2017.09.28: do not use Facebook buildTargetGroup due to lots of bugs
                    // buildTargetGroup = BuildTargetGroup.Facebook;
                    buildTargetGroup = BuildTargetGroup.Standalone;
                    break;

                case BuildTarget.WebGL:
                    buildTargetGroup = BuildTargetGroup.WebGL;
                    break;

                case BuildTarget.StandaloneOSX:
                    buildTargetGroup = BuildTargetGroup.Standalone;
                    break;

                default:
                    Debug.Log("Unhandled buildTarget: " + buildTarget);
                    EditorApplication.Exit(-1);
                    break;
            }

            Debug.Log("BuildApplication: " + buildTarget);

            if (buildTarget == BuildTarget.Android)
            {
                Debug.Log("Check Android Paths are set");
                MakeSureAndroidPathsAreSet();
            }

            string basePath = TARGET_DIR;
            BuildOptions options = BuildOptions.None;
            Dictionary<string, string> param = GetAssetBuildParams();
            if (!param.ContainsKey(SKIP_LZ4HC_COMPRESSION) || param[SKIP_LZ4HC_COMPRESSION].ToLower() == "false")
            {
                options |= BuildOptions.CompressWithLz4HC;
            }

            if (param.ContainsKey(PROJECT_PATH_KEY))
            {
                basePath = param[PROJECT_PATH_KEY];
            }

            if (param.ContainsKey(DEVELOP_BUILD_KEY) && param[DEVELOP_BUILD_KEY].ToLower() == "true")
            {
                options |= BuildOptions.Development;
            }
            else
            {
                options &= ~BuildOptions.Development;
            }

            if (param.ContainsKey(USE_PROFILE_KEY) && param[USE_PROFILE_KEY].ToLower() == "true")
            {
                options |= BuildOptions.Development;
                options |= BuildOptions.ConnectWithProfiler;
                options |= BuildOptions.AllowDebugging;
            }

            string defineFlag = ApplicationSettings.Instance.defineFlags;
            bool isDevBuild = false;

            if (param.ContainsKey(BUILD_CONFIG_KEY) && param[BUILD_CONFIG_KEY] == "DEV")
            {
                // UnityEditor.CrashReporting.CrashReportingSettings.captureEditorExceptions = true;
                isDevBuild = true;
                defineFlag += ";DEV";
            }
            else
            {
                defineFlag += ";DISABLE_PLAYFABCLIENT_API";

                PlayerSettings.SetStackTraceLogType(LogType.Assert, StackTraceLogType.None);
                PlayerSettings.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
                PlayerSettings.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            }
            if (param.ContainsKey(PLATFORM_CONFIG_KEY))
            {
                defineFlag += ";PLATFORM_" + param[PLATFORM_CONFIG_KEY].ToUpper();
            }
            if (param.ContainsKey(NATIVE_BUILD_TARGET))
            {
                string nativeBuildTarget = param[NATIVE_BUILD_TARGET].ToUpper();
                defineFlag += ";BUILD_" + nativeBuildTarget;
            }

            if (param.ContainsKey(APP_NAME))
            {
                if (!SetEnvironment(param[APP_NAME].ToUpper(), true))
                {
                    Debug.Log("Check environment setting.");
                    EditorApplication.Exit(-1);
                    return;
                }
            }

            // Check TMP_Settings
            if (TMP_Settings.defaultSpriteAsset == null)
            {
                Debug.Log("Check TMP_Settings.");
                EditorApplication.Exit(-1);
                return;
            }

            string pkgName = DEFAULT_IDENTIFIER;
            if (param.ContainsKey(IDENTIFIER_KEY))
            {
                pkgName = param[IDENTIFIER_KEY];
            }

            bool ignoreAssetbundleDependency = false;
            if (param.ContainsKey(IGNORE_ASSETBUNDLE_DEPENDENCY_KEY) && param[IGNORE_ASSETBUNDLE_DEPENDENCY_KEY].ToLower() == "true")
            {
                ignoreAssetbundleDependency = true;
            }
            // Team, QA, PROD applicationSettings Load..

            if (param.ContainsKey(BUNLDLE_VERSION_KEY))
                UpdateBundleVersion(param[BUNLDLE_VERSION_KEY]);

            LoadPreferredTypes();
            GenerateAOT();

            // Perform
            Debug.Log("Build Path : " + basePath);

            if (param.ContainsKey(BUILD_ASSET_BUNDLE_KEY))
            {
                string assetBundlePath = param[BUILD_ASSET_BUNDLE_KEY];
                Debug.Log("Build AssetBundle first, Assetbundle Path : " + assetBundlePath);
                string outputPath = Path.Combine(assetBundlePath, "" + buildTarget);
                if (param.ContainsKey(PLATFORM_CONFIG_KEY))
                {
                    outputPath = Path.Combine(assetBundlePath, param[PLATFORM_CONFIG_KEY]);
                }
                Build_Assetbundle(buildTarget, buildTargetGroup, outputPath, isDevBuild, ignoreAssetbundleDependency, defineFlag);
            }

            if (param.ContainsKey(SKIP_PLAYER))
            {
                return;
            }

            UnityEditor.Build.Reporting.BuildReport buildReport = BuildPlayer(buildTarget, buildTargetGroup, basePath, defineFlag, options, pkgName);

            if (buildReport.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log("Build finished with: " + buildReport.summary.result.ToString());
                EditorApplication.Exit(-1);
            }
        }

        /// <summary>
        /// 更新资源版本
        /// </summary>
        /// <param name="newVersion"></param>
        static void UpdateBundleVersion(string newVersion)
        {
            string[] guids = AssetDatabase.FindAssets("ApplicationSettings t:ApplicationSettings");
            var settings = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(ApplicationSettings)) as ApplicationSettings;
            settings.bundleVersion = newVersion;
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 没看懂是做什么的
        /// </summary>
        [MenuItem("BagelCode/Builder/Load Preferred Types", false, 350)]
        static void LoadPreferredTypes()
        {
            string preferredTypesPath = Path.Combine("Assets", "PreferredTypes.typePrefs");
            var json = System.IO.File.ReadAllText(preferredTypesPath);
            List<System.Type> typeList = ParadoxNotion.Serialization.JSONSerializer.Deserialize<List<System.Type>>(json);
            ParadoxNotion.Design.TypePrefs.SetPreferedTypesList(typeList);
        }

        [MenuItem("BagelCode/Builder/GenerateAOT", false, 351)]
        static void GenerateAOT()
        {
            string aotPath = Path.Combine("Assets", "AOTClasses.cs");
            ParadoxNotion.Design.AOTClassesGenerator.GenerateAOTClasses(aotPath, ParadoxNotion.Design.TypePrefs.GetPreferedTypesList(typeof(object), true).ToArray());

            string linkPath = Path.Combine("Assets", "link.xml");
            ParadoxNotion.Design.AOTClassesGenerator.GenerateLinkXML(linkPath, ParadoxNotion.Design.TypePrefs.GetPreferedTypesList(typeof(object)).ToArray());

            AssetDatabase.Refresh();
        }

        // environment setting. use build only.
        static void SetEnvironment()
        {
            Dictionary<string, string> param = GetAssetBuildParams();

            if (param.ContainsKey(APP_NAME))
            {
                SetEnvironment(param[APP_NAME].ToUpper(), true);
            }
        }

        [MenuItem("BagelCode/Environment/SLOTS1", false, 350)]
        static void SetSLOTS1()
        {
            SetEnvironment("SLOTS1", false);
        }

        [MenuItem("BagelCode/Environment/SLOTS3", false, 351)]
        static void SetSLOTS3()
        {
            SetEnvironment("SLOTS3", false);
        }

        [MenuItem("BagelCode/Environment/SLOTS4", false, 352)]
        static void SetSLOTS4()
        {
            SetEnvironment("SLOTS4", false);
        }

        static bool SetEnvironment(string buildTarget, bool isBuild)
        {
            Debug.Log(string.Format("Target environment({0})", buildTarget));
            string environmentPath = string.Format("Assets/Meta/Environment/{0}/EnvironmentSettings.asset", buildTarget);

            EnvironmentSettings environmentSettings = (EnvironmentSettings)AssetDatabase.LoadAssetAtPath(environmentPath, typeof(EnvironmentSettings));

            if (environmentSettings.CheckVaildation())
            {
                environmentSettings.Apply(isBuild);
                return true;
            }

            return false;
        }
    }

}
