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
        AndroidSystemHelper.Instance.Init();
    }

    private void OnDestroy()
    {
        MessageDispatcher.UnRegister(EVTType.ON_CUSTOM_EVENT, OnListenerOpenEvent);
        SBoxSandboxListener.Instance.RemoveButtonLongPress(SBoxApi.SBoxSandbox.SBOX_SWITCH.SWITCH_ROOT_SET, OpenBackgroundManagerMainView);
    }



    private void OnListenerOpenEvent(EventData eventData)
    {
        if(eventData.name == "OnOpenBackground")
        {
            OpenBackgroundManagerMainView();
        }
        if(eventData.name == "SBoxSandboxListenerInit")
        {
            SBoxSandboxListener.Instance.AddButtonLongPress(SBoxApi.SBoxSandbox.SBOX_SWITCH.SWITCH_ROOT_SET, OpenBackgroundManagerMainView);
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
        //var lobbyController = FindObjectOfType<LobbyController>();
        //if (lobbyController == null) { return; }
        BackgroundManagerMainViewController backgroundManagerMainViewController = FindObjectOfType<BackgroundManagerMainViewController>();
        if (backgroundManagerMainViewController != null)
        {
            return;
        }
        NumericKeypadController numericKeypadController = FindObjectOfType<NumericKeypadController>();
        if (numericKeypadController != null)
        {
            return;
        }
        OpenView("lobby0", "NumericKeypadView", PopupManager.Instance.BackgroundSetting);
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

