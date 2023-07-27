using UnityEngine;
using UnityEditor;

namespace SlotMaker
{
	public class SlotPropertyBase : PropertyDrawer 
	{
		protected Rect rect;
		protected Rect lineRect;
		protected int indent;
	    protected float height;

		protected const int lineHeight  = 16;
		protected const int indentWidth = 16;

		protected void Begin(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			indent = 0;

			rect = lineRect = position;
			height = lineRect.height = lineHeight;
		}

		protected void End()
		{
			EditorGUI.EndProperty();
		}

		protected void Space(float space)
		{
			lineRect.x += space;
			lineRect.width -= space;
		}

		protected void NewLine()
		{
			int indentSize = indent * indentWidth;
			lineRect.x = rect.x + indentSize;
			lineRect.y += lineHeight;
			lineRect.width = rect.width - indentSize;
	        height += lineHeight;
		}

		protected SerializedProperty FindProperty(SerializedProperty property, string propertyName)
		{
			return property.FindPropertyRelative(propertyName);
		}

		protected bool ShowFoldout(Rect position, SerializedProperty property, GUIContent label)
		{
			label = EditorGUI.BeginProperty(position, label, property);
			property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
			EditorGUI.EndProperty();
			if (property.isExpanded)
			{
				++indent;
				NewLine();
			}
			return property.isExpanded;
		}

		protected void EndFoldout()
		{
			--indent;
			NewLine();
		}

		protected Rect GetRect(float width)
		{
			if (width == 0f)
				width = lineRect.width;

			var newRect = new Rect(lineRect.x, lineRect.y, width, lineRect.height);

			lineRect.x     += width;
			lineRect.width -= width;

			return newRect;
		}

		protected void PrefixLabel(GUIContent label, float width = 0f)
		{
			EditorGUI.PrefixLabel(GetRect(width), GUIUtility.GetControlID(FocusType.Passive), label);
		}

		protected void PropertyField(SerializedProperty property, float width = 0f)
		{
			EditorGUI.PropertyField(GetRect(width), property, GUIContent.none);
		}

	    protected int IntField(int value, float width = 0f)
	    {
	        return EditorGUI.IntField(GetRect(width), value);
	    }

		protected Object ObjectField(System.Type objType, float width = 0f)
		{
			Object obj = null;
			obj = EditorGUI.ObjectField(GetRect(width), obj, objType, true);
			return obj;
		}

		protected float Slider(float value, float left, float right, float width = 0f)
		{
			return EditorGUI.Slider(GetRect(width), value, left, right);
		}

	    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	    {
	        return height;
	    }
	}
}
