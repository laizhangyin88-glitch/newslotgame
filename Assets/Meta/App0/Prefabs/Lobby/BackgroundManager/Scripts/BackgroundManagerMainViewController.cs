using SlotMaker;
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
    public static BackgroundManagerMainViewController Instance { get; private set; }
    public List<SettingButtonType> unOpen = new List<SettingButtonType>()
    {
        SettingButtonType.GameInformation,
        SettingButtonType.BusinessRecord,
        SettingButtonType.EventRecord,
        SettingButtonType.Settings,
    };

    private Transform content;
    private GameObject selectItem;
    private List<MainItemController> mainItemControllers = new List<MainItemController>();
    private SettingButtonType settingButtonType;

    private void Awake()
    {
        Instance = this;
        SBoxSanboxController.Instance.isEnterBackgroundTestView = true;
    }

    private void Start()
    {
        OpenBackgroundManager.OpenView("lobby0", "NumericKeypadView", PopupManager.Instance.BackgroundSetting);
        settingButtonType = SettingButtonType.None;
        content = transform.Find("ScrollView/Viewport/Content");
        selectItem = transform.Find("SelectItem").gameObject;
        InitMainItemList();
    }

    private void InitMainItemList()
    {
        int index = 0;
        foreach (SettingButtonType item in Enum.GetValues(typeof(SettingButtonType)))
        {
            if (item != SettingButtonType.None)
            {
                if (!unOpen.Contains(item))
                {
                    GameObject go = Instantiate(selectItem);
                    go.transform.localScale = Vector3.one;
                    go.transform.SetParent(content.transform, false);
                    MainItemController controller = go.GetComponent<MainItemController>();
                    controller.buttonType = item;
                    controller.index = index;
                    controller.backgroundManagerMainViewController = this;
                    mainItemControllers.Add(controller);
                }
                index++;
            }
        }
    }

    private void OnDestroy()
    {
        SBoxSanboxController.Instance.isEnterBackgroundTestView = false;
    }

}
