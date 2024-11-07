using InnerKeyboard;
using ParadoxNotion;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WifiConnectViewController : MonoBehaviour
{
    private Keyboard keyboard;
    private TextMeshProUGUI wifiName;
    private KeyboardParam KeyboardPara = new KeyboardParam("");  //键盘参数
    private TextMeshProUGUI password;
    private Button ConnectButton;
    private string passwordValue;

    Button Closebutton;
    private void Start()
    {
        MessageDispatcher.Register(EVTType.ON_CUSTOM_EVENT, OnListenerWifiName);

        ConnectButton = transform.Find("Info/ConnectButton").GetComponent<Button>();
        ConnectButton.onClick.AddListener(OnClickConnectBtn);
        Closebutton = transform.Find("Button").GetComponent<Button>();
        Closebutton.onClick.AddListener(OnClickCloseBtn);
        keyboard = transform.Find("KeyBoard").GetComponent<Keyboard>();
        keyboard.CancelBtnEvent += OnClickCloseBtn;
        keyboard.ConfirmBtnEvent += OnClickConnectBtn;
        wifiName = transform.Find("Info/Image/Name").GetComponent<TextMeshProUGUI>();
        password = transform.Find("Info/Image/password").GetComponent<TextMeshProUGUI>(); 
        StartCoroutine(InitKeyboardEvent());
    }

    private void OnClickCloseBtn()
    {
        Destroy(gameObject);
    }

    private void OnClickConnectBtn()
    {

    }

    private IEnumerator InitKeyboardEvent()
    {
        yield return new WaitForSeconds(Time.deltaTime * 2);
        if (Keyboard.Instance)
            Keyboard.Instance.ShowKeyboard(KeyboardPara, EditCallBack);
    }
    void EditCallBack(KeyboardParam kbpara)
    {
        if (kbpara != null)
        {
            password.text = kbpara.OutputStr;
            passwordValue = kbpara.OutputStr;
        }
    }

    private void OnListenerWifiName(EventData eventData)
    {
        if(eventData.name == "OpenSoftKeyboard")
        {
            wifiName.text = "Wifi Name:  " + eventData.value.ToString();
        }
    }
}
