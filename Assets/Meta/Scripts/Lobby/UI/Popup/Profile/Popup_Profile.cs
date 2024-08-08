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

    /// <summary>
    /// 【网络数据】用户id
    /// </summary>
    public static string NetData_UserId => BlackboardUtils.FindVariable<string>("/me/userId")?.value ?? "";
    /// <summary>
    /// 【网络数据】用户名字
    /// </summary>
    public static string NetData_UserName => BlackboardUtils.FindVariable<string>("/me/name")?.value ?? "";
    /// <summary>
    /// 【网络数据】用户资产
    /// </summary>
    public static long NetData_UserCredit => BlackboardUtils.FindVariable<long>("/me/credit")?.value ?? default;

    // Start is called before the first frame update
    void Start()
    {
        _closeBtn.onClick.AddListener(() =>
        {
            PopupManager.Instance.Close();
            Destroy(this.gameObject);
        });

        _idTextCom.text = NetData_UserId;
        _nameInputField.Init(NetData_UserName);
    }
}
