using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace SlotMaker
{
	public class ContextTextMeshProDropdown : ContextCompositor, IContextIntProperty, IContextBooleanProperty, IContextListenable<int>
	{
		public TMP_Dropdown dropdown;

	    private UnityAction<int> callBack;

		public void SetIntProperty(int value)
		{
			dropdown.value = value;
		}

		public int GetIntProperty()
		{
			return dropdown.value;
		}

	    public void SetBooleanProperty(bool value)
	    {
	        dropdown.interactable = value;
	    }

	    public bool GetBooleanProperty()
	    {
	        return dropdown.interactable;
	    }

	    public void AddListener(UnityAction<int> action)
	    {
	        callBack += action;
	    }

	    public void OnValueChanged(int value)
	    {
	        if (callBack != null)
	            callBack(value);
	    }
	}
}
