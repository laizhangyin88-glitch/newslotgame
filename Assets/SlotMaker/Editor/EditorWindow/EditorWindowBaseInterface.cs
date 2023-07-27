using UnityEngine;
using UnityEditor;

namespace SlotMaker
{
	public class EditorWindowBaseInterface : EditorWindow
	{
		public virtual string GetEditorName()
		{
			return string.Empty;
		}
	}
}