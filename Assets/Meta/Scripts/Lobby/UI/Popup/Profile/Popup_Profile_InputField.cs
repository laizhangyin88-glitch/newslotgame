using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Dreamteck.Splines.Primitives;

[RequireComponent(typeof(TMP_InputField))]
public class Popup_Profile_InputField : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputFieldCom;
    [SerializeField] private Button _confirmBtnCom;

    public event Action<string> OnInputFieldConfirm;

    public void UpdateNameDisplay(string name)
    {
        if (_inputFieldCom.text == name)
            return;

        _inputFieldCom.text = name;
    }

    public void SetConfirmBtnActive(bool active)
    {
        _confirmBtnCom.gameObject.SetActive(active);
    }

    protected virtual void OnButtonClickHandle()
    {
        OnInputFieldConfirm?.Invoke(_inputFieldCom.text);
        SetConfirmBtnActive(false);
    }

    protected virtual void OnInputFieldChangeHandle(string str)
    {
        if (_confirmBtnCom.gameObject.activeSelf == false)
            SetConfirmBtnActive(true);
    }

    //protected virtual void OnInputFieldDeselectHandle(string str)
    //{
    //    OnInputFieldConfirm?.Invoke(str);

    //    _inputFieldCom.readOnly = true;
    //}

    private void Start()
    {
        _confirmBtnCom.onClick.AddListener(OnButtonClickHandle);
        SetConfirmBtnActive(false);
        _inputFieldCom.onValueChanged.AddListener(OnInputFieldChangeHandle);
        //_inputFieldCom.onDeselect.AddListener(OnInputFieldDeselectHandle);
    }
}
