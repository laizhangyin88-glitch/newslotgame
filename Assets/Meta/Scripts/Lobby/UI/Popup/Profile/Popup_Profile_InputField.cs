using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_InputField))]
public class Popup_Profile_InputField : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputFieldCom;
    [SerializeField] private Button _editorBtnCom;

    public void Init(string name)
    {
        _inputFieldCom.text = name;
    }

    protected virtual void OnButtonClickHandle()
    {
        _inputFieldCom.readOnly = false;
        _inputFieldCom.Select();
    }

    protected virtual void OnInputFieldDeselectHandle(string str)
    {
        //判断名称格式否是符合
        //发送更名协议
        //监听返回，弹出更名成功显示
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
