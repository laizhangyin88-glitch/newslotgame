using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenBackgroundManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Q)) 
        {
            BackgroundManagerMainViewController backgroundManagerMainViewController = FindObjectOfType<BackgroundManagerMainViewController>();
            if(backgroundManagerMainViewController != null)
            {
                return;
            }
            OpenView("lobby0", "BackgroundManagerMainView", PopupManager.Instance.BackgroundSetting);
        }
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

