using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace SlotMaker
{
	public class ContextTextMeshProUGUI : ContextCompositor, IContextText
	{
		public TextMeshProUGUI textMeshProUGUI;

		public void SetText(string text)
		{
			textMeshProUGUI.text = text;
		}

		public string GetText()
		{
			return textMeshProUGUI.text;
		}
	}
}