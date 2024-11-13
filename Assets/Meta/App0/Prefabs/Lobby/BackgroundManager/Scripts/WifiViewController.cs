using InnerKeyboard;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WifiViewController : MonoBehaviour
{
    private List<WifiItemController> wifiItemControllers = new List<WifiItemController>();

    private Keyboard keyboard;

    private Transform NoWifi;
    private void Start()
    {
        WifiMgr.Instance.Init();
        NoWifi = transform.Find("list/NoWifi");
        Button button = transform.Find("list/Button").GetComponent<Button>();
        button.onClick.AddListener(OnCloseBtn);
        InitWifiList();
    }

    private void OnCloseBtn()
    {
        Destroy(gameObject);
    }

    private void InitWifiList()
    {
        List<string> nameList = WifiMgr.Instance.getWifiNameList();
        if(nameList == null)
        {
            nameList = WifiMgr.Instance.getWifiNameList();
        }
        Transform content = transform.Find("list/ScrollView/Viewport/Content");
        if (nameList.Count > 0)
        {
            GameObject wifiItem = transform.Find("WifiItem").gameObject;
            content.gameObject.SetActive(true);
            NoWifi.gameObject.SetActive(false);
            for (global::System.Int32 i = 0; i < nameList.Count; i++)
            {
                GameObject item = Instantiate(wifiItem);
                item.transform.SetParent(content.transform, false);
                WifiItemController controller = item.GetComponent<WifiItemController>();
                controller.wifiName = nameList[i];
                controller.wifiViewController = this;
                wifiItemControllers.Add(controller);
            }
        }
        else
        {
            content.gameObject.SetActive(false);
            NoWifi.gameObject.SetActive(true);
        }
    }


    public void OnpenKeyboard()
    {
        
    }
}
