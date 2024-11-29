using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace SlotMaker
{
	public class ContextText : ContextCompositor, IContextText
	{
		public Text text;

		public void SetText(string text)
		{
			this.text.text = text;
		}

		public string GetText()
		{
			return this.text.text;
		}
	}
}
