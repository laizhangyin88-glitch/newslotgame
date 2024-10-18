using BagelCode.ClientModels;
using BagelCode;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using UnityEngine.UI;
using Spine;

public class Popup_Ranking : MonoBehaviour
{
    [SerializeField] private Button _closeBtn;

    private void Start()
    {
        InitClickEvents();
    }

    private void InitClickEvents()
    {
        _closeBtn.onClick.AddListener(() =>
        {
            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_LEAVE_RANKING);
            MetaPopupUtils.ClosePopup(gameObject);
        });
    }
}
