using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(TMP_InputField))]
public class Popup_Profile_InputField : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputFieldCom;
    [SerializeField] private Button _editorBtnCom;

    public event Action<string> OnInputFieldDeselect;

    public void UpdateNameDisplay(string name)
    {
        if (_inputFieldCom.text == name)
            return;

        _inputFieldCom.text = name;
    }

    protected virtual void OnButtonClickHandle()
    {
        _inputFieldCom.readOnly = false;
        _inputFieldCom.Select();
    }

    protected virtual void OnInputFieldDeselectHandle(string str)
    {
        OnInputFieldDeselect?.Invoke(str);

        _inputFieldCom.readOnly = true;
    }

    private void OnEnable()
    {
        _editorBtnCom.onClick.AddListener(OnButtonClickHandle);
        _inputFieldCom.onDeselect.AddListener(OnInputFieldDeselectHandle);
    }

    private void OnDisable()
    {
        _editorBtnCom.onClick.RemoveListener(OnButtonClickHandle);
        _inputFieldCom.onDeselect.RemoveListener(OnInputFieldDeselectHandle);
    }
}
