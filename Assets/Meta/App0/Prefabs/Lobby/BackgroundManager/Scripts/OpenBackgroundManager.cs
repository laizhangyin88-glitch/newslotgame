using BagelCode;
using ParadoxNotion;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenBackgroundManager : MonoBehaviour
{
    private void Start()
    {
        MessageDispatcher.Register(EVTType.ON_CUSTOM_EVENT, OnListenerOpenEvent);
    }

    private void OnDestroy()
    {
        MessageDispatcher.UnRegister(EVTType.ON_CUSTOM_EVENT, OnListenerOpenEvent);
    }

    private void OnListenerOpenEvent(EventData eventData)
    {
        if(eventData.name == "OnOpenBackground")
        {
            OpenBackgroundManagerMainView();
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Q)) 
        {
            OpenBackgroundManagerMainView();
        }
    }

    private void OpenBackgroundManagerMainView()
    {
        var lobbyController = FindObjectOfType<LobbyController>();
        if (lobbyController == null) { return; }
        BackgroundManagerMainViewController backgroundManagerMainViewController = FindObjectOfType<BackgroundManagerMainViewController>();
        if (backgroundManagerMainViewController != null)
        {
            return;
        }
        OpenView("lobby0", "BackgroundManagerMainView", PopupManager.Instance.BackgroundSetting);
    }

    public static GameObject OpenView(string bundleName, string assetName, Transform parent = null)
    {
        var prefab = AssetBundleManager.LoadAsset<GameObject>(bundleName, assetName);
        if (prefab != null)
        {
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            if (parent != null)
            {
                go.transform.SetParent(parent, false);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                go.transform.localRotation = Quaternion.identity;
            }
            return go;
        }
        return null;
    }
}

