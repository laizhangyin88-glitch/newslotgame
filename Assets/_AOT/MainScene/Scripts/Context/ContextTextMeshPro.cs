using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace SlotMaker
{
	public class ContextTextMeshPro : ContextCompositor, IContextText
	{
		public TextMeshPro textMeshPro;

		public void SetText(string text)
		{
			textMeshPro.text = text;
		}

		public string GetText()
		{
			return textMeshPro.text;
		}
	}
}
