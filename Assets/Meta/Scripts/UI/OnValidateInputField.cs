using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnValidateInputField : MonoBehaviour
{
	private InputField inputField;
	private TMP_InputField inputFieldTMP;

	public void Awake()
	{
		inputField = gameObject.GetComponent<InputField>();
		inputFieldTMP = gameObject.GetComponent<TMP_InputField>();
		if (inputField != null)
		{
	        inputField.onValidateInput += delegate(string input, int charIndex, char addedChar) { return IsValidate(input, charIndex, addedChar); };
		}
		if (inputFieldTMP != null)
		{
	        inputFieldTMP.onValidateInput += delegate(string input, int charIndex, char addedChar) { return IsValidate(input, charIndex, addedChar); };
		}
	}

	private char IsValidate(string input, int index, char addedChar)
	{
		if (char.IsHighSurrogate(addedChar) || char.IsLowSurrogate(addedChar))
		{
			return '\0';
		}

		return addedChar;
	}
}

