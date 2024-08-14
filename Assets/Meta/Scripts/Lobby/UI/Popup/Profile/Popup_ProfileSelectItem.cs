using BagelCode;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_ProfileSelectItem : MonoBehaviour
{
    [SerializeField] private WebImageController _webImageCom;
    [SerializeField] private Toggle _toggle;
    [SerializeField] private Button _btn;
    [SerializeField] private Image _webImage;

    public bool IsSelected => _toggle.isOn;
    public string Url { get; private set; }

    private void Awake()
    {
        _toggle.onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                _webImage.color = Color.white;
            }
            else
            {
                _webImage.color = new Color(0.36f, 0.36f, 0.36f);
            }
        });

        _btn.onClick.AddListener(() =>
        {
            _toggle.isOn = true;
        });
    }

    public void SetImageUrl(string url, ToggleGroup tg = null)
    {
        Url = url;
        _webImageCom.SetWebImage(url, false);
        if(url == NetData_Login.Instance.UserProfileUrl)
        {
            _toggle.isOn = true;
        }

        if(tg != null)
        {
            _toggle.group = tg;
        }
    }
}
