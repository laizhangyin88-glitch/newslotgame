using BagelCode.ClientModels;
using ParadoxNotion;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_ProfileChange : MonoBehaviour
{
    [SerializeField] private Popup_ProfileChange_ScrollController _scrollController;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Toggle _typeToggle1;
    [SerializeField] private Toggle _typeToggle2;
    [SerializeField] private Button _confirmBtn;

    private List<Tuple<int, string, string>> profileDatas;

    private void Start()
    {
        _closeBtn.onClick.AddListener(() =>
        {
            PopupManager.Instance.Close();
            Destroy(this.gameObject);
        });

        _confirmBtn.onClick.AddListener(OnConfirmBtnClick);

        profileDatas = NetData_Login.Instance.ProfilePicturesWithCurrentLevel;
        UpdateProfilesDisplay("male");

        _typeToggle1.onValueChanged.AddListener((value) =>
        {
            if (value == false)
                return;

            UpdateProfilesDisplay("male");
        });

        _typeToggle2.onValueChanged.AddListener((value) =>
        {
            if (value == false)
                return;

            UpdateProfilesDisplay("female");
        });
    }

    public void OnConfirmBtnClick()
    {
        var curSelect = _scrollController.CurrentSelect;
        //如果没有选择或当前选择和之前头像一致，则直接关闭弹窗
        if (curSelect == null || curSelect.Url == NetData_Login.Instance.UserProfileUrl)
        {
            PopupManager.Instance.Close(gameObject);
            Destroy(gameObject);
            return;
        }

        //发送头像更改协议
        NetManager.Instance.Post(RPCName.resetUserProfile, new Dictionary<string, string>()
            {
                { "profile_url", curSelect.Url}
            },
        (responseData) =>
        {
            Popup_Tips.OpenTips(gameObject, "Profile picture is set successfully");
            NetData_Login.Instance.SetNetDataValue(NetData_Login.Path_UserProfileUrl, curSelect.Url);
            PopupManager.Instance.Close(gameObject);
            Destroy(gameObject);
        },
        (errData) =>
        {
            Popup_Tips.OpenTips(gameObject, "Failed to set profile picture");
        });
    }

    public void UpdateProfilesDisplay(string type)
    {
        var datas = profileDatas.FindAll((Item) => Item.Item3 == type);
        _scrollController.Init(datas);
    }
}
