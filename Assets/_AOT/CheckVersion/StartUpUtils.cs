using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

public class VersionData
{
    public string Version;
}

public static class StartUpUtils
{

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
        UnityWebRequest www = UnityWebRequest.Get(localPath);
        var operation = www.SendWebRequest();
        while (!operation.isDone)
        { }
        if (www.result == UnityWebRequest.Result.ConnectionError
            || www.result == UnityWebRequest.Result.ProtocolError)
            Debug.LogError(www.error);
        else
        {
            action?.Invoke(www.downloadHandler.data);
        }
    }

}

