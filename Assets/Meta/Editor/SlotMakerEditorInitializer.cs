using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SlotMaker;

namespace BagelCode
{
	public class SlotMakerEditorInitializer
	{
		[InitializeOnLoadMethod]
		static void Initialize()
		{
			StringTableUtils.customProvider = new BagelCodeFormatProvider();
		}
	}
}
