using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Popup_Profile_CopyBtn : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textTarget;

    private Button _btnCom;

    // Start is called before the first frame update
    void Awake()
    {
        _btnCom = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _btnCom.onClick.AddListener(OnBtnClickHandle);
    }

    private void OnDisable()
    {
        _btnCom.onClick.RemoveListener(OnBtnClickHandle);
    }

    protected void OnBtnClickHandle()
    {
        if (_textTarget == null)
            return;

        GUIUtility.systemCopyBuffer = _textTarget.text;
        Debug.Log($"剪切板内容{GUIUtility.systemCopyBuffer}");
    }
}
