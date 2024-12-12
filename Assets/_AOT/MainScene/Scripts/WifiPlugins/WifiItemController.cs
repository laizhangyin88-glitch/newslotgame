using Com.ForbiddenByte.OSA.Core;
using GameUtil;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WifiItemController : BaseItemViewsHolder
{
    public string wifiName;

    private TextMeshProUGUI name;
    private Button button;

    public override void CollectViews()
    {
        base.CollectViews();
        name = root.transform.Find("name").GetComponent<TextMeshProUGUI>();
        button = root.transform.Find("button").GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClickButton);
    }

    public static GameObject OpenViewFromResources(string assetName, Transform parent = null)
    {
        GameObject goAsset = Resources.Load<GameObject>(assetName);
        if (goAsset == null)
            return null;

        GameObject go = GameObject.Instantiate(goAsset);
        if (parent != null)
        {
            go.transform.SetParent(parent, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;
        }
        return go;
    }

    private void OnClickButton()
    {
        
        OpenViewFromResources("WifiConnectView", PopupManager.Instance.BackgroundSetting);
        Timer.DelayAction(0.2f, () =>
        {
            MessageDispatcher.Dispatch("OnCustomEvent", new ParadoxNotion.EventData<string>("OpenSoftKeyboard", name.text));
        });
    }

    public void UpdateViews(string wifiName)
    {
        name.text = wifiName;
    }
}
