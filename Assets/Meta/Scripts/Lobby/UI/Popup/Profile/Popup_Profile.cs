using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Popup_Profile : MonoBehaviour
{
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Popup_Profile_InputField _nameInputField;
    [SerializeField] private TextMeshProUGUI _goldTextCom;
    [SerializeField] private TextMeshProUGUI _idTextCom;
    [SerializeField] private TextMeshProUGUI _descTextCom;

    // Start is called before the first frame update
    void Start()
    {
        _closeBtn.onClick.AddListener(() =>
        {
            PopupManager.Instance.Close();
            Destroy(this.gameObject);
        });

        _idTextCom.text = NetData_Login.Instance.NetData_UserId;
        _nameInputField.Init(NetData_Login.Instance.NetData_UserName);
    }
}
