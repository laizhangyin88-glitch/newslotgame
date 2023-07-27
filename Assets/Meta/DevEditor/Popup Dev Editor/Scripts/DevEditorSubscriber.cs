using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.EventSystems;

// using ParadoxNotion.Services;
// using NodeCanvas.Framework;

using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SlotMaker.Simulator
{

public class DevEditorSubscriber : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject simulatorObj = null;
    public bool isDown = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        isDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(isDown == false) return;

        isDown = false;
#if DEV
        if (simulatorObj == null)
        {
            var sceneInfo = AssetBundleManager.LoadAsset<SceneInfoObject>("testsuite", "Popup Dev Editor Scene").GetSceneInfo();
            GameObject go = SceneManager.LoadScene(PopupManager.Instance.transform, sceneInfo);
            go.name = "Popup Dev Editor";
            simulatorObj = go;

            PopupManager.Instance.Open(go);
        }
#endif
    }

}

}
