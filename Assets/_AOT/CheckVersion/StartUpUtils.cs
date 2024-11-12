using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

public class VersionData
{
    public string Version;
}

public static class StartUpUtils
{

    public static VersionData CreateVersionData(string version)
    {
        if(Version.TryParse(version, out _) == false)
        {
            Debug.Log("版本号格式错误");
            return null;
        }

        return new VersionData()
        {
            Version = version
        };
    }

    public static void SaveVersionData(VersionData data, string path, string fileName = "Version.txt")
    {
        if (string.IsNullOrEmpty(fileName))
            return;

        if (Directory.Exists(path) == false)
            Directory.CreateDirectory(path);

        string filePath = Path.Combine(path, fileName);
        File.WriteAllText(filePath, JsonConvert.SerializeObject(data));
    }

    public static int ParseVersion(string version)
    {
        string[] versionArray = version.Split('.');
        int result = 0;
        for (int i = 0; i < versionArray.Length; i++)
            result = result * 100 + int.Parse(versionArray[i]);
        return result;
    }

    public static void GetFromStreamingAssets(string path, Action<byte[]> action)
    {
        string localPath = "";
        if (Application.platform == RuntimePlatform.Android)
            localPath = Application.streamingAssetsPath + "/" + path;
        else
            localPath = "file:///" + Application.streamingAssetsPath + "/" + path;

        Debug.Log($"GetFromStreamingAssets:{localPath}");

        UnityWebRequest www = UnityWebRequest.Get(localPath);
        var operation = www.SendWebRequest();
        while (!operation.isDone)
        { }
        if (www.result == UnityWebRequest.Result.ConnectionError
            || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(www.error);
            action?.Invoke(null);
        }
        else
        {
            action?.Invoke(www.downloadHandler.data);
        }
    }

}

