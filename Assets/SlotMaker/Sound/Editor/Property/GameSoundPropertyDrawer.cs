using UnityEngine;
using UnityEditor;

namespace SlotMaker
{
	[CustomPropertyDrawer(typeof(GameSound))]
	public class GameSoundPropertyDrawer : SlotPropertyBase
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Begin(position, property, label);

			PrefixLabel(label, lineRect.width * 0.5f);
			PropertyField(FindProperty(property, "id"));

			End();
		}
	}
}