using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PasswordTxtShow : MonoBehaviour
{
    private TextMeshProUGUI _TextMeshProUGUI;

    private int length;

    private void Start()
    {
        _TextMeshProUGUI = GetComponent<TextMeshProUGUI>();
        length = 0;
    }

    private void Update()
    {
        if(_TextMeshProUGUI == null) return;
        if( _TextMeshProUGUI.text.Length <= 0)
        {
            return;
        }
        if(_TextMeshProUGUI.text.Length > 0 && length != _TextMeshProUGUI.text.Length)
        {
            StartCoroutine(ReplaceTxtValue());
            length = _TextMeshProUGUI .text.Length;
        }
    }

    private IEnumerator ReplaceTxtValue()
    {
        for (int i = 0; i < _TextMeshProUGUI.text.Length; i++)
        {
            var temp = _TextMeshProUGUI.text[i];
            if(temp != '*')
            {
                yield return new WaitForSeconds(0.5f);
                InputText(_TextMeshProUGUI.text.Length);
            }
        }
    }

    private void InputText(int count)
    {
        _TextMeshProUGUI.text = "";
        for (int i = 0; i < count; i++)
        {
            _TextMeshProUGUI.text += "*";
        }
        StopAllCoroutines();
    }
}
