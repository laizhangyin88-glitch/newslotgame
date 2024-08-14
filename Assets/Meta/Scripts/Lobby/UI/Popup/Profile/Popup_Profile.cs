using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BagelCode;
using SlotMaker.Tasks.Actions;

public class Popup_Profile : MonoBehaviour
{
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Popup_Profile_InputField _nameInputField;
    [SerializeField] private TextMeshProUGUI _goldTextCom;
    [SerializeField] private TextMeshProUGUI _idTextCom;
    [SerializeField] private TextMeshProUGUI _descTextCom;
    [SerializeField] private WebImageController _iconCom;
    [SerializeField] private Button _iconBtnCom;

    // Start is called before the first frame update
    void Start()
    {
        _closeBtn.onClick.AddListener(() =>
        {
            PopupManager.Instance.Close();
            Destroy(this.gameObject);
        });

        _iconBtnCom.onClick.AddListener(() =>
        {
            OpenLobbyPopup.LoadAndOpenLobbyPopup("lobby", "Popup_ProfileChange", false, this.gameObject);
        });

        _idTextCom.text = NetData_Login.Instance.NetData_UserId;

        _nameInputField.UpdateNameDisplay(NetData_Login.Instance.NetData_UserName);
        _nameInputField.OnInputFieldDeselect += OnInputFieldDeselectHandle;
        NetData_Login.Instance.AddNetDataChangeEvent(NetData_Login.Path_UserName, OnUserNameChangeHandle);

        _goldTextCom.text = NetData_Login.Instance.NetData_UserCredit.ToString("###,###");
        NetData_Login.Instance.AddNetDataChangeEvent(NetData_Login.Path_UserCredit, OnUserCreditChangeHandle);

        _iconCom.SetWebImage(NetData_Login.Instance.UserProfileUrl, false);
        NetData_Login.Instance.AddNetDataChangeEvent(NetData_Login.Path_UserProfileUrl, OnProfileChangeHandle);
    }

    private void OnDestroy()
    {
        NetData_Login.Instance.RemoveNetDataChangeEvent(NetData_Login.Path_UserName, OnUserNameChangeHandle);
        NetData_Login.Instance.RemoveNetDataChangeEvent(NetData_Login.Path_UserCredit, OnUserCreditChangeHandle);
        NetData_Login.Instance.RemoveNetDataChangeEvent(NetData_Login.Path_UserProfileUrl, OnProfileChangeHandle);
    }

    private void OnUserNameChangeHandle(string k, object v)
    {
        _nameInputField.UpdateNameDisplay(v.ToString());
    }

    private void OnProfileChangeHandle(string k, object v)
    {
        _iconCom.SetWebImage(v.ToString().Trim('"'), false);
    }

    private void OnUserCreditChangeHandle(string k, object v)
    {
        _goldTextCom.text = ((long)v).ToString("###,###");
    }

    private void OnInputFieldDeselectHandle(string obj)
    {
        if (string.IsNullOrEmpty(obj))
        {
            Debug.Log($"<color=red>【--名称格式错误】</color> newName:{obj}");
            _nameInputField.UpdateNameDisplay(NetData_Login.Instance.NetData_UserName);
            return;
        }
            
        if (string.Equals(obj, NetData_Login.Instance.NetData_UserName))
        {
            Debug.Log($"<color=red>【--名称未改变】</color> newName:{obj}");
            return;
        }

        NetManager.Instance.Post(RPCName.resetNickName, new Dictionary<string, string>()
        {
            { "nick_name", obj },
        },
        (responseData) =>
        {
            Debug.Log($"<color=red>【--更改名称成功】</color> newName:{obj}");
            NetData_Login.Instance.SetNetDataValue<string>(NetData_Login.Path_UserName, obj);
            Popup_Tips.OpenTips(this.gameObject, "Successful name change");
        },
        (errData) =>
        {
            Debug.Log($"<color=red>【--更改名称失败】</color> newName:{obj}");
        });
    }
}
