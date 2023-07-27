using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
	public class ContextSetter : MonoBehaviour 
	{
		public void SetText(int value)
		{
			var contextText = GetComponent<IContextText>();
			if (contextText != null)
				contextText.SetText(value.ToString());
		}
	}
}