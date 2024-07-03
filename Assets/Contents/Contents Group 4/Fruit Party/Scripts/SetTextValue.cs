using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetTextValue : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;

    public long value;

    public string textValue;

    private void Start()
    {
        if (!string.IsNullOrEmpty(textValue) && textMeshProUGUI != null)
        {
            textMeshProUGUI.text = textValue;
        }
    }

    private void OnEnable()
    {
        if (!string.IsNullOrEmpty(textValue) && textMeshProUGUI != null)
        {
            textMeshProUGUI.text = textValue;
        }
    }
}
