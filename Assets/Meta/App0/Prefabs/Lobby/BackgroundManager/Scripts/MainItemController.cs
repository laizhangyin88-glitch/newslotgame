using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainItemController : MonoBehaviour
{
    public SettingButtonType buttonType;
    private Button button;
    public BackgroundManagerMainViewController backgroundManagerMainViewController;

    private void Start()
    {
        button = transform.Find("icon").GetComponent<Button>();
        button.onClick.AddListener(OnClickIconBtn);
        TextMeshProUGUI name = transform.Find("name").GetComponent<TextMeshProUGUI>(); 
        name.text = AddSpaceBeforeUppercase(buttonType.ToString());
    }



    private void OnClickIconBtn()
    {
        switch (buttonType)
        {
            case SettingButtonType.None:
                break;
            case SettingButtonType.GameInformation:
                break;
            case SettingButtonType.BusinessRecord:
                break;
            case SettingButtonType.GameHistroy:
                OpenBackgroundManager.OpenView("lobby0", "GameHistroyRecordView", PopupManager.Instance.BackgroundSetting);
                break;
            case SettingButtonType.EventRecord:
                break;
            case SettingButtonType.Settings:
                break;
            case SettingButtonType.InputTest:
                OpenBackgroundManager.OpenView("lobby0", "InputKeyTestView", PopupManager.Instance.BackgroundSetting);
                break;
            case SettingButtonType.TouchCalibrate:
                OpenBackgroundManager.OpenView("lobby0", "TouchTestView", PopupManager.Instance.BackgroundSetting);
                break;
            case SettingButtonType.TimeAndDate:
                break;
            case SettingButtonType.Wifi:
                OpenBackgroundManager.OpenView("lobby0", "WifiView", PopupManager.Instance.BackgroundSetting);
                break;
            case SettingButtonType.ExitSetup:
                Destroy(backgroundManagerMainViewController.gameObject);
                break;
            default:
                break;
        }
    }

    private void OnDestroy()
    {
        
    }

    string AddSpaceBeforeUppercase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        StringBuilder sb = new StringBuilder(input.Length + input.Length / 10);
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (char.IsUpper(c) && i != 0)
            {
                sb.Append(' ');
            }
            sb.Append(c);
        }

        return sb.ToString();
    }
}
