using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SlotMaker;

public class UIProfileController : MonoBehaviour
{
    [SerializeField] private Image _iconCom;
    [SerializeField] private TextMeshProUGUI _idCom;
    [SerializeField] private Text _levelCom;
    [SerializeField] private GameObject _vipCom;
    [SerializeField] private Button _btnCom;

    // Start is called before the first frame update
    void Start()
    {
        _btnCom.onClick.AddListener(() =>
        {
            SlotMaker.Tasks.Actions.OpenLobbyPopup.LoadAndOpenLobbyPopup("lobby", "Popup_Profile", true, this.gameObject);
        });

        _idCom.text = NetData_Login.Instance.NetData_UserId;

        _levelCom.text = NetData_Login.Instance.UserLevel.ToString();
        NetData_Login.Instance.AddNetDataChangeEvent(NetData_Login.Path_UserLevel, OnUserLevelChangeHandle);
    }

    public void OnUserLevelChangeHandle(string k, object v)
    {
        _levelCom.text = v.ToString();
    }


    public Sprite GetProfile(string name)
    {
        return null;
    }
}
