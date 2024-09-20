using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using SlotMaker;

/// <summary>
/// 当toggle的值改变时，同步改变背景，文字材质和文字颜色
/// </summary>
/// <remarks>
/// From:whh - 2024年8月5日
/// </remarks>
public class Popup_Announcement_Toggle : MonoBehaviour
{
    [SerializeField] private Toggle _toggle;

    [SerializeField] private Sprite _normalSprite;
    [SerializeField] private Sprite _selectedSprite;

    [SerializeField] private Material _normalFontMaterial;
    [SerializeField] private Material _selectedFontMaterial;

    [SerializeField] private Color _normalFontColor;
    [SerializeField] private Color _selectedFontColor;

    [SerializeField] private GameObject _relationObj;

    [Space]

    [SerializeField] private Image _imageCom;
    [SerializeField] private TextMeshProUGUI _textCom;

    [Space]

    [SerializeField] private GameSoundPlayer _gameSoundPlayer;

    protected void OnEnable()
    {
        _toggle.onValueChanged.AddListener(OnToggleValueChangeHandle);
    }
    protected void OnDisable()
    {
        _toggle.onValueChanged.RemoveListener(OnToggleValueChangeHandle);
    }

    private void OnToggleValueChangeHandle(bool value)
    {
        if (value && _gameSoundPlayer) _gameSoundPlayer.PlayGameSound("UI_Button_Normal");
        if (_imageCom) _imageCom.sprite = value ? _selectedSprite : _normalSprite;

        if (_textCom)
        {
            if (_selectedFontMaterial != null && _normalFontMaterial)
                _textCom.fontMaterial = value ? _selectedFontMaterial : _normalFontMaterial;

            if (_selectedFontColor != null && _normalFontColor != null)
                _textCom.color = value ? _selectedFontColor : _normalFontColor;
        }

        if (_relationObj) _relationObj.SetActive(value);
    }

}
