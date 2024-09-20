using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SlotMaker;
using BagelCode;

public class UIProfileController : MonoBehaviour
{
    [SerializeField] private WebImageController _iconCom;
    [SerializeField] private TextMeshProUGUI _idCom;
    [SerializeField] private Text _levelCom;
    [SerializeField] private GameObject _vipCom;
    [SerializeField] private Button _btnCom;

    // Start is called before the first frame update
    void Start()
    {
        _btnCom.onClick.AddListener(() =>
        {
            SlotMaker.Tasks.Actions.OpenLobbyPopup.LoadAndOpenLobbyPopup("lobby", "Popup_Profile", false, this.gameObject);
        });

        _idCom.text = NetData_Login.Instance.NetData_UserId;

        _levelCom.text = NetData_Login.Instance.UserLevel.ToString();
        NetData_Login.Instance.AddNetDataChangeEvent(NetData_Login.Path_UserLevel, OnUserLevelChangeHandle);

        _iconCom.SetWebImage(NetData_Login.Instance.UserProfileUrl, false);
        NetData_Login.Instance.AddNetDataChangeEvent(NetData_Login.Path_UserProfileUrl, OnUserProfileChangeHandle);
    }

    private void OnDestroy()
    {
        NetData_Login.Instance.RemoveNetDataChangeEvent(NetData_Login.Path_UserLevel, OnUserLevelChangeHandle);
        NetData_Login.Instance.RemoveNetDataChangeEvent(NetData_Login.Path_UserProfileUrl, OnUserProfileChangeHandle);
    }

    public void OnUserLevelChangeHandle(string k, object v)
    {
        _levelCom.text = v.ToString();
    }

    public void OnUserProfileChangeHandle(string k, object v)
    {
         string url = v.ToString().Trim('"');
        _iconCom.SetWebImage(url, false);
    }
}
