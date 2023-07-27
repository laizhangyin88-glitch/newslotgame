using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class InputFieldTrimAddOn : MonoBehaviour
{
    public InputField inputField;

    private void Awake()
    {
        inputField = gameObject.GetComponent<InputField>();

        if(inputField != null)
        {
            inputField.onEndEdit.RemoveListener( OnValueChanged );
            inputField.onEndEdit.AddListener( OnValueChanged );
        }
    }

    public void OnValueChanged(string text)
    {
        if(text.Length > 0)
        {
            string trimText = text.Trim();

            if(inputField != null && trimText != text)
            {
                inputField.text = trimText;
            }
        }
        
    }
}
