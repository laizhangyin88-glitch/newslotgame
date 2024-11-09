using SlotMaker;
using System.Collections;
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

        if(nativeObject == null) return list;

        int len = nativeObject.Call<int>("getWifiListSize");
        Debug.LogError("the getWifiListSize is " + len);

#if UNITY_EDITOR
        for (int i = 0; i < 15; i++)
        {
            list.Add("test wifi" + i);
        }
#endif

        for (int i = 0;i < len;i++)
        {
            string txt = nativeObject.Call<string>("getWifiName", i);
            Debug.LogError("find wifi:" +  txt);
            list.Add(txt);
        }

        return list;
    }

    public void connectToWifi(string ssid,string pwd) {
        if (nativeObject == null) return;
        Debug.LogError("connect wifi :" + ssid + "pwd:" + pwd);
        nativeObject.Call("connectToWifi",ssid,pwd);
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}
}
