using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace SlotMaker
{
	public class ContextInputField : ContextCompositor, IContextInputField, IContextText, IContextListenable<string>
	{
		public InputField inputField;
	    private UnityAction<string> callBack;

		public int characterLimit
		{
			get { return inputField.characterLimit; }
			set { inputField.characterLimit = value; }
		}

		public InputField.ContentType contentType
		{
			get { return inputField.contentType; }
			set { inputField.contentType = value; }
		}

		public void SetText(string text)
		{
			inputField.text = text;
		}

		public string GetText()
		{
			return inputField.text;
		}

	    public void AddListener(UnityAction<string> action)
	    {
	        callBack += action;
	    }

	    public void OnValueChanged(string text)
	    {
	        if(inputField == null) return;
	        if(inputField.wasCanceled) return;
	        
	        if (callBack != null)
	            callBack(text);
	    }
	}
}
