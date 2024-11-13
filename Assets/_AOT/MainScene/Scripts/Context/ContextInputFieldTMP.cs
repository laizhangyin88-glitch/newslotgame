using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace SlotMaker
{
	public class ContextInputFieldTMP : ContextCompositor, IContextInputField, IContextText, IContextListenable<string>
	{
		public TMP_InputField tmpInputField;
	    private UnityAction<string> callBack;

	    private const char zeroWidthSpace = '\u200B';

		public int characterLimit
		{
			get { return tmpInputField.characterLimit; }
			set { tmpInputField.characterLimit = value; }
		}

		public InputField.ContentType contentType
		{
			get { return (InputField.ContentType)tmpInputField.contentType; }
			set { tmpInputField.contentType = (TMP_InputField.ContentType)value; }
		}

		public void SetText(string text)
		{
	        if (ApplicationSettings.LogTest())
	            Debug.Log(gameObject.name);
			tmpInputField.text = text;
		}

		public string GetText()
		{
	        return tmpInputField.text.Trim(zeroWidthSpace);
		}

	    public void AddListener(UnityAction<string> action)
	    {
	        callBack += action;
	    }

	    public void OnValueChanged(string text)
	    {
	        if (callBack != null)
	            callBack(text.Trim(zeroWidthSpace));
	    }
	}
}
