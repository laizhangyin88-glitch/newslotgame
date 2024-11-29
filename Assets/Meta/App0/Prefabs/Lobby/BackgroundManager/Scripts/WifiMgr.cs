using SlotMaker;
using System.Collections.Generic;
using UnityEngine;

public class WifiMgr : MonoSingleton<WifiMgr>
{
    private AndroidJavaObject nativeObject;
    // Start is called before the first frame update
    void Start()
    {
        nativeObject = new AndroidJavaObject("com.cryfx.game.libserialport.WifiPlugin");
        if (nativeObject == null) return;
        nativeObject.Call("Init");
    }

    public void Init()
    {
        nativeObject = new AndroidJavaObject("com.cryfx.game.libserialport.WifiPlugin");
        if (nativeObject == null) return;
        nativeObject.Call("Init");
    }

    public List<string> getWifiNameList()
    {
        List<string> list = new List<string>();

        if (nativeObject == null) return list;

        int len = nativeObject.Call<int>("getWifiListSize");

#if UNITY_EDITOR
        for (int i = 0; i < 15; i++)
        {
            list.Add("test wifi" + i);
        }
#endif

        for (int i = 0; i < len; i++)
        {
            string txt = nativeObject.Call<string>("getWifiName", i);
            list.Add(txt);
        }

        return list;
    }

    public void connectToWifi(string ssid, string pwd)
    {
        if (nativeObject == null) return;
        Debug.LogError("connect wifi :" + ssid + "pwd:" + pwd);
        nativeObject.Call("connectToWifi", ssid, pwd);
    }


    /// <summary>
    /// 获取当前连接wifi名
    /// </summary>
    /// <remarks>
    /// 周期性调用，判断连接
    /// </remarks>
    /// <returns>如果没有连接wifi则返回空字符串</returns>
    public string getCurrentConnectedWifiSsid()
    {
        if (nativeObject == null) return null;
        return nativeObject.Call<string>("getCurrentConnectedWifiSsid");
    }

    /// <summary>
    /// 获取wifi信号等级
    /// </summary>
    /// <param name="uuid">wifi名</param>
    /// <returns>如果</returns>
    public int getWifiSignalStrength(string uuid)
    {
        if (nativeObject == null) return -1;
        return nativeObject.Call<int>("getWifiSignalStrength");
    }

    /// <summary>
    /// 获取系统wifi管理器的状态
    /// </summary>
    /// <returns>
    /// 0 - 已断开WiFi
    /// 1 - 正在断开WiFi
    /// 2 - 正在连接WiFi
    /// 3 - 已连上WiFi
    /// 4 - 连接状态未知
    /// </returns>
    public int getWifiState()
    {
        if (nativeObject == null) return 4;
        return nativeObject.Call<int>("getWifiState");
    }
}
