using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SettingButtonType
{
    None,
    GameInformation,
    BusinessRecord,
    GameHistroy,
    EventRecord,
    Settings,
    InputTest,
    TouchCalibrate,
    TimeAndDate,
    Wifi,
    ExitSetup,
}

public class BackgroundManagerMainViewController : MonoBehaviour
{
    private Transform content;
    private GameObject selectItem;
    private List<MainItemController> mainItemControllers = new List<MainItemController>();
    private SettingButtonType settingButtonType;

    private void Start()
    {
        settingButtonType = SettingButtonType.None;
        content = transform.Find("ScrollView/Viewport/Content");
        selectItem = transform.Find("SelectItem").gameObject;
        InitMainItemList();
    }

    private void InitMainItemList()
    {
        foreach (SettingButtonType item in Enum.GetValues(typeof(SettingButtonType)))
        {
            if (item != SettingButtonType.None)
            {
                GameObject go = Instantiate(selectItem);
                go.transform.localScale = Vector3.one;
                go.transform.SetParent(content.transform, false);
                MainItemController controller = go.GetComponent<MainItemController>();
                controller.buttonType = item;
                controller.backgroundManagerMainViewController = this;
                mainItemControllers.Add(controller);
            }
        }
    }
}
