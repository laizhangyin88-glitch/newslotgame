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

    private void OnClickButton()
    {
        OpenBackgroundManager.OpenView("lobby0", "WifiConnectView", PopupManager.Instance.BackgroundSetting);
        Timer.DelayAction(0.2f, () =>
        {
            MessageDispatcher.Dispatch(EVTType.ON_CUSTOM_EVENT, new ParadoxNotion.EventData<string>("OpenSoftKeyboard", name.text));
        });
    }

    public void UpdateViews(string wifiName)
    {
        name.text = wifiName;
    }
}
