using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WifiItemController : MonoBehaviour
{
    public string wifiName;

    public WifiViewController wifiViewController;

    private void Start()
    {
        Button button = transform.Find("button").GetComponent<Button>();
        button.onClick.AddListener(OnClickButton);
        TextMeshProUGUI text = transform.Find("name").GetComponent<TextMeshProUGUI>();
        text.text = wifiName;
    }

    private void OnClickButton()
    {
        OpenBackgroundManager.OpenView("lobby0", "WifiConnectView", PopupManager.Instance.BackgroundSetting);
        this.DelayAction(0.2f, () =>
        {
            MessageDispatcher.Dispatch(EVTType.ON_CUSTOM_EVENT, new ParadoxNotion.EventData<string>("OpenSoftKeyboard", wifiName));
        });
    }
}
