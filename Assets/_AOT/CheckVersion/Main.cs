using Newtonsoft.Json;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    /// <summary> 是否更新: 网络版本比对 /// </summary>
    private bool needUpdateNet = false;

    private UnityWebRequest www;

    //private LoadSlider loadSlider;

    private Text versionText;

    private VersionData curVersionData = new VersionData
    {
        Version = "0.0.0",
    };

    private VersionData netVersionData = new VersionData
    {
        Version = "0.0.0",
    };

    private VersionData localVersionData = new VersionData
    {
        Version = "0.0.0",
    };

    private List<string> dllList = new List<string>
    {
        //"UnityWebSocket.Runtime.dll",
        //"Base.dll",
        //"Game.dll"
        "Assembly-CSharp.dll"
    };

    private void Awake()
    {
        //loadSlider = transform.Find("slider").GetComponent<LoadSlider>();
        //versionText = transform.Find("VersionText").GetComponent<Text>();
    }

    private IEnumerator Start()
    {
        yield return StartCoroutine(RequestUserPermissions(Permission.ExternalStorageRead));
        yield return StartCoroutine(RequestUserPermissions(Permission.ExternalStorageWrite));

        yield return StartCoroutine(RefTypes.LoadMetadataForAOTAssemblies());
        yield return StartCoroutine(CheckVersion());
        Debug.Log("The context is ready");

        AssetBundleManager.BaseUrl = ApplicationSettings.GetRemoteBundlePath();
        AssetBundleManager.BaseFilePath = ApplicationSettings.GetStreamingBundlePath();

        Debug.Log("初始化Manifest...");
        yield return AssetBundleManager.Initialize();

        Debug.Log("加载初场景资源...");
        string abName_mainscene = ApplicationSettings.MakeApplicationBundleName("mainscene");
        var abOperation = AssetBundleManager.LoadAssetBundleAndDep(abName_mainscene, false);

        if (abOperation == null || abOperation.Count <= 0)
        {
            Debug.Log("读取初始场景资源失败");
            yield break;
        }

        float totalProgress = abOperation.Count;
        while (OperationListIsDone(abOperation) == false)
        {
            float curProgress = GetOperationListTotalProgress(abOperation);
            string progress = (curProgress / totalProgress).ToString("P2");
            Debug.Log(progress);

            yield return new WaitForSeconds(0.5f);

        }

        Debug.Log("加载完成");
        Debug.Log("進入場景...");
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene", LoadSceneMode.Single);

        yield return null;
    }


    private IEnumerator RequestUserPermissions(string permissionKey)
    {
#if UNITY_ANDROID
        bool hasPermission = Permission.HasUserAuthorizedPermission(permissionKey);
        Debug.Log($"是否拥有权限:{hasPermission}");

        if (hasPermission == false)
        {
            Debug.Log($"开始请求权限{permissionKey}");
            int permissionState = 0;

            PermissionCallbacks callback = new PermissionCallbacks();
            callback.PermissionGranted += (msg) =>
            {
                Debug.Log($"授权成功：{msg}");
                permissionState = 1;
            };
            callback.PermissionDenied += (msg) =>
            {
                Debug.Log($"授权失败：{msg}");
                permissionState = -1;
            };
            callback.PermissionDeniedAndDontAskAgain += (msg) =>
            {
                Debug.Log($"授权失败并且不再询问：{msg}");
                permissionState = -1;
            };

            Permission.RequestUserPermission(permissionKey, callback);

            yield return new WaitUntil(() => permissionState != 0);
            Debug.Log($"{permissionKey}.permissionState:{permissionState}");

            if (permissionState < 0)//未授权，退出程序(后面可以跳过热更）
                Application.Quit();
        }
        else
        {
            yield return null;
        }
#endif
    }

    private bool OperationListIsDone(List<AssetBundleLoadOperation> operations)
    {
        foreach (var item in operations)
        {
            if (item.IsDone() == false)
                return false;
        }

        return true;
    }
    private float GetOperationListTotalProgress(List<AssetBundleLoadOperation> operations)
    {
        float progress = 0f;
        foreach (var item in operations)
        {
            progress += item.Progress();
        }

        return progress;
    }

    private bool GetBitValue(byte value, byte bit)
    {
        return (value & (byte)Math.Pow(2, bit)) > 0 ? true : false;
    }

    private void Update()
    {
        //if (www != null)
        //    loadSlider.SetSliderValue(www.downloadProgress);
    }

    /// <summary>
    /// 獲取持久化目錄中版本號
    /// </summary>
    private void GetCurVersion()
    {
        Debug.Log($"获取持久化目录中的版本号:{StartUpConfig.VersionPath}");
        bool exists = File.Exists(StartUpConfig.VersionPath);
        Debug.Log($"持久化目录是否存在Version.txt:{exists}");
        if (exists)
        {
            string versionJson = File.ReadAllText(StartUpConfig.VersionPath);
            curVersionData = JsonConvert.DeserializeObject<VersionData>(versionJson);
            PlayerPrefs.SetString("CurVersion", curVersionData.Version);
            //versionText.text = $"Version: {curVersionData.Version}";
            Debug.Log($"CurVersion:{curVersionData.Version}");
        }
    }

    private IEnumerator CheckVersion()
    {
        string versionUrl = ApplicationSettings.GetRemoteVersionPath();
        Debug.Log($"读取远程版本文件：{versionUrl}");
        //string versionUrl = StartUpConfig.url + "/Version.txt";
        www = UnityWebRequest.Get(versionUrl);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError
           || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(www.error);
            //CompareVersion();
        }
        else if (www.isDone)
        {
            netVersionData = JsonConvert.DeserializeObject<VersionData>(www.downloadHandler.text);
            CompareVersion();
        }
    }

    private void CompareVersion()
    {
        GetCurVersion();
        GetLocalVersion();
        Debug.Log($"NetVersion:{netVersionData.Version}");
        needUpdateNet = StartUpUtils.ParseVersion(netVersionData.Version) > StartUpUtils.ParseVersion(curVersionData.Version);
        if (needUpdateNet)
        {
            Debug.Log("需要更新");

            //如果網絡版本大於Streaming版本
            if (StartUpUtils.ParseVersion(netVersionData.Version) > StartUpUtils.ParseVersion(localVersionData.Version))
                StartCoroutine(UpdateFromNet());
            else
                UpdateFromLocal();
        }
        else
        {
            if (StartUpUtils.ParseVersion(localVersionData.Version) > StartUpUtils.ParseVersion(curVersionData.Version))
                UpdateFromLocal();
            else
                LoadFromMemory();
        }
    }

    /// <summary>
    /// 獲取StreamingAssets中的版本號
    /// </summary>
    private void GetLocalVersion()
    {
        StartUpUtils.GetFromStreamingAssets("Lib/Version.txt", (data) =>
        {
            string versionJson = System.Text.Encoding.UTF8.GetString(data);
            localVersionData = JsonConvert.DeserializeObject<VersionData>(versionJson);
            Debug.Log($"LocalVersion:{localVersionData.Version}");
        });
    }

    private IEnumerator UpdateFromNet()
    {
        Debug.Log("UpdateFormNet");

        if (Directory.Exists(StartUpConfig.VersionPath) == false)
            Directory.CreateDirectory(StartUpConfig.VersionPath);

        File.WriteAllText(StartUpConfig.VersionPath, JsonConvert.SerializeObject(netVersionData));
        curVersionData = netVersionData;
        PlayerPrefs.SetString("CurVersion", curVersionData.Version);
        for (int i = 0; i < dllList.Count; i++)
        {
            //string dllUrl = StartUpConfig.url + "/Lib/" + dllList[i];
            string dllUrl = ApplicationSettings.GetRemoteDllPath(dllList[i]);
            www = UnityWebRequest.Get(dllUrl);
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError
            || www.result == UnityWebRequest.Result.ProtocolError)
                Debug.LogError(www.error);
            else if (www.isDone)
            {
                Debug.Log($"下载远程dll完成：{dllUrl}");
                File.WriteAllBytes(StartUpConfig.DllPath + "/" + dllList[i], www.downloadHandler.data);
            }

        }
        LoadDllFromMemory();
        //StartCoroutine(LoadAssetBundleFromNet());
    }

    /// <summary>
    /// 從網絡加載所有ab包
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadAssetBundleFromNet()
    {
        string manifestPath = ApplicationSettings.GetRemoteManifestPath();
        www = UnityWebRequestAssetBundle.GetAssetBundle(manifestPath);
        //www = UnityWebRequestAssetBundle.GetAssetBundle(StartUpConfig.url + "/AssetBundles/AssetBundles");
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError
            || www.result == UnityWebRequest.Result.ProtocolError)
            Debug.LogError(www.error);
        else if (www.isDone)
        {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
            AssetBundleManifest manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            string[] names = manifest.GetAllAssetBundles();
            AssetBundle.UnloadAllAssetBundles(true);
            www = UnityWebRequest.Get(StartUpConfig.url + "/AssetBundles/AssetBundles");
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError
            || www.result == UnityWebRequest.Result.ProtocolError)
                Debug.LogError(www.error);
            else if (www.isDone)
                File.WriteAllBytes(StartUpConfig.AssetBundlePath + "/AssetBundles", www.downloadHandler.data);
            for (int i = 0; i < names.Length; i++)
            {
                www = UnityWebRequest.Get(StartUpConfig.url + "/AssetBundles/" + names[i]);
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.ConnectionError
                || www.result == UnityWebRequest.Result.ProtocolError)
                    Debug.LogError(www.error);
                else if (www.isDone)
                    File.WriteAllBytes(StartUpConfig.AssetBundlePath + "/" + names[i], www.downloadHandler.data);
            }
            StartCoroutine(LoadAssetBundleFromMemoryAsync());
        }
    }

    private void UpdateFromLocal()
    {
        Debug.Log("UpdateFromLocal");

        if (Directory.Exists(StartUpConfig.VersionPath) == false)
            Directory.CreateDirectory(StartUpConfig.VersionPath);

        File.WriteAllText(StartUpConfig.VersionPath, JsonConvert.SerializeObject(localVersionData));
        curVersionData = localVersionData;
        PlayerPrefs.SetString("CurVersion", curVersionData.Version);
        CopyDllFromStreamingAssets();
        LoadDllFromMemory();
        //StartCoroutine(CopyAssetBundleFromStreamingAssets());
        //StartCoroutine(LoadAssetBundleFromMemoryAsync());
    }

    private void LoadFromMemory()
    {
        LoadDllFromMemory();
        //StartCoroutine(LoadAssetBundleFromMemoryAsync());
    }

    private void LoadDllFromMemory()
    {
        for (int i = 0; i < dllList.Count; i++)
        {
            string path = StartUpConfig.DllPath + "/" + dllList[i];
            if (!File.Exists(path))
            {
                StartUpUtils.GetFromStreamingAssets($"{ApplicationSettings.Instance.libPath}/" + dllList[i] + ".bytes", (data) =>
                {
                    File.WriteAllBytes(path, data);
                    Assembly.Load(data);
                });
            }
            else
                Assembly.Load(File.ReadAllBytes(path));
        }
    }

    private void CopyDllFromStreamingAssets()
    {
        for (int i = 0; i < dllList.Count; i++)
        {
            string path = StartUpConfig.DllPath + "/" + dllList[i];
            StartUpUtils.GetFromStreamingAssets($"{ApplicationSettings.Instance.libPath}/" + dllList[i] + ".bytes", (data) =>
            {
                File.WriteAllBytes(path, data);
            });
        }
    }


    private IEnumerator CopyAssetBundleFromStreamingAssets()
    {
        AssetBundleCreateRequest createRequest = null;
        StartUpUtils.GetFromStreamingAssets("AssetBundles/AssetBundles", (data) =>
        {
            File.WriteAllBytes(StartUpConfig.AssetBundlePath + "/AssetBundles", data);
            createRequest = AssetBundle.LoadFromMemoryAsync(data);
        });
        yield return createRequest;
        if (createRequest != null)
        {
            AssetBundle bundle = createRequest.assetBundle;
            AssetBundleManifest manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            string[] names = manifest.GetAllAssetBundles();
            for (int i = 0; i < names.Length; i++)
            {
                createRequest = null;
                StartUpUtils.GetFromStreamingAssets("AssetBundles/" + names[i], (data) =>
                {
                    File.WriteAllBytes(StartUpConfig.AssetBundlePath + "/" + names[i], data);
                    createRequest = AssetBundle.LoadFromMemoryAsync(data);
                });
                yield return createRequest;
                if (createRequest != null)
                {
                    bundle = createRequest.assetBundle;
                    StartUpConfig.bundleDic[names[i]] = bundle;
                    if (names[i] == "load")
                    {
                        var prefab = bundle.LoadAsset<GameObject>("load");
                        Instantiate(prefab, transform);
                    }
                }
            }
            Debug.Log($"curVersion = {curVersionData.Version}");
        }
    }

    private IEnumerator LoadAssetBundleFromMemoryAsync()
    {
        AssetBundleCreateRequest createRequest = null;
        bool needCopy = !File.Exists(StartUpConfig.AssetBundlePath + "/AssetBundles");
        if (needCopy)
        {
            StartUpUtils.GetFromStreamingAssets("AssetBundles/AssetBundles", (data) =>
            {
                File.WriteAllBytes(StartUpConfig.AssetBundlePath + "/AssetBundles", data);
                createRequest = AssetBundle.LoadFromMemoryAsync(data);
            });
        }
        else
            createRequest = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(StartUpConfig.AssetBundlePath + "/AssetBundles"));
        yield return createRequest;
        if (createRequest != null)
        {
            AssetBundle bundle = createRequest.assetBundle;
            AssetBundleManifest manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            string[] names = manifest.GetAllAssetBundles();
            for (int i = 0; i < names.Length; i++)
            {
                createRequest = null;
                needCopy = !File.Exists(StartUpConfig.AssetBundlePath + "/" + names[i]);
                if (needCopy)
                {
                    StartUpUtils.GetFromStreamingAssets("AssetBundles/" + names[i], (data) =>
                    {
                        File.WriteAllBytes(StartUpConfig.AssetBundlePath + "/" + names[i], data);
                        createRequest = AssetBundle.LoadFromMemoryAsync(data);
                    });
                }
                else
                {
                    //Debug.LogError($"Read: {StartUpConfig.AssetBundlePath + "/" + names[i]}");
                    createRequest = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(StartUpConfig.AssetBundlePath + "/" + names[i]));
                }
                yield return createRequest;
                if (createRequest != null)
                {
                    bundle = createRequest.assetBundle;
                    StartUpConfig.bundleDic[names[i]] = bundle;
                    if (names[i] == "load")
                    {
                        var prefab = bundle.LoadAsset<GameObject>("load");
                        Instantiate(prefab, transform);
                    }
                }
            }
            Debug.Log($"curVersion = {curVersionData.Version}");
        }
    }
}
