using InnerKeyboard;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WifiViewController : MonoBehaviour
{
    private Keyboard keyboard;

    private Transform NoWifi;

    private List<string> wifiList;

    private WifiListOSA wifiListOSA;

    private int lastCount = 0;

    private Transform waitContent;

    private TextMeshProUGUI connectedTxt;

    Transform content;
    private void Start()
    {
        wifiList = new List<string>();
        WifiMgr.Instance.Init();
        NoWifi = transform.Find("list/NoWifi");
        connectedTxt = transform.Find("list/connected").GetComponent<TextMeshProUGUI>();
        Button button = transform.Find("list/Button").GetComponent<Button>();
        wifiListOSA = transform.Find("list/ScrollView").GetComponent<WifiListOSA>();
        content = transform.Find("list/ScrollView/Viewport/Content");
        waitContent = transform.Find("content");
        waitContent.gameObject.SetActive(false);
        button.onClick.AddListener(OnCloseBtn);
        StartCoroutine(UpdateWifiList());
        RefreshWifiList();
        CheckWifiState();
        InvokeRepeating("RefreshWifiList", Time.deltaTime * 50, Time.deltaTime * 50);
        InvokeRepeating("CheckWifiState", Time.deltaTime * 50, Time.deltaTime * 50);
    }

    private void CheckWifiState()
    {
        if(WifiMgr.Instance != null)
        {
            int state = WifiMgr.Instance.getWifiState();
            switch (state)
            {
                case 0:
                    connectedTxt.text = "wifi is disconect";
                    break;
                case 1:
                    connectedTxt.text = "wifi is disconect";
                    break;
                case 2:
                    connectedTxt.text = "wifi is conecting";
                    break;
                case 3:
                    string name = WifiMgr.Instance.getCurrentConnectedWifiSsid();
                    if (!string.IsNullOrEmpty(name))
                    {
                        connectedTxt.text = string.Format("connect wifi : {0}", name);
                    }
                    break;
                case 4:
                    break;
                default:
                    Debug.LogError("wifi has error...............");
                    break;
            }
        }
    }

    private void RefreshWifiList()
    {
        StartCoroutine(UpdateWifiList());
    }

    private void OnCloseBtn()
    {
        Destroy(gameObject);
    }

    private IEnumerator UpdateWifiList()
    {
        yield return new WaitForSeconds(0.1f);
        List<string> nameList = WifiMgr.Instance.getWifiNameList();
        if (nameList.Count > 0)
        {
            content.gameObject.SetActive(true);
            NoWifi.gameObject.SetActive(false);
            for (global::System.Int32 i = 0; i < nameList.Count; i++)
            {
                string name = nameList[i];
                if (!wifiList.Contains(name))
                {
                    wifiList.Add(name);
                }
            }
            wifiListOSA.Data = wifiList;
            if (lastCount != wifiList.Count)
            {
                lastCount = wifiList.Count;
                wifiListOSA.ResetItems(wifiList.Count);
            }
        }
        else
        {
            content.gameObject.SetActive(false);
            NoWifi.gameObject.SetActive(true);
        }
    }



    private void OnDestroy()
    {
        wifiList.Clear();
    }
}
