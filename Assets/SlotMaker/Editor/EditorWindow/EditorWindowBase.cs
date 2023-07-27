using System;
using UnityEngine;
using UnityEditor;

namespace SlotMaker
{
    public class EditorWindowBase<T> : EditorWindowBaseInterface where T : EditorWindowBaseInterface
    {
    	protected static T _editor;

    	protected static void CreateWindow()
    	{
    		_editor = (T)EditorWindow.GetWindow(typeof(T));
    		_editor.titleContent = new GUIContent(_editor.GetEditorName());
    	}

    	protected virtual void BeginHorizontal()
    	{
    		EditorGUILayout.BeginHorizontal();
    	}

        protected virtual void BeginHorizontal(GUIStyle style)
        {
            EditorGUILayout.BeginHorizontal(style);
        }

    	protected virtual void EndHorizontal()
    	{
    		EditorGUILayout.EndHorizontal();
    	}

    	protected virtual void BeginCheck()
    	{
    		EditorGUI.BeginChangeCheck();
    	}

    	protected virtual bool EndCheck()
    	{
    		return EditorGUI.EndChangeCheck();
    	}

    	protected virtual int Popup(int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options)
    	{
    		return EditorGUILayout.Popup(selectedIndex, displayedOptions, options);
    	}

    	protected virtual void LabelField(string label, params GUILayoutOption[] options)
    	{
    		EditorGUILayout.LabelField(label, options);
    	}

        protected virtual bool Toggle(bool value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.Toggle(value, options);
        }

    	protected virtual Enum EnumMaskField(Enum enumValue, params GUILayoutOption[] options)
    	{
    		return EditorGUILayout.EnumFlagsField(enumValue);
    	}

    	protected virtual int MaskField(int mask, string[] displayedOptions, params GUILayoutOption[] options)
    	{
    		return EditorGUILayout.MaskField(mask, displayedOptions, options);
    	}

        protected virtual Enum EnumPopup(Enum selected, params GUILayoutOption[] options)
        {
            return EditorGUILayout.EnumPopup(selected, options);
        }

        protected virtual int IntField(int value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.IntField(value, options);
        }

    	protected virtual int IntField(string label, int value, params GUILayoutOption[] options)
    	{
    		return EditorGUILayout.IntField(label, value, options);
    	}

        protected virtual float FloatField(float value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.FloatField(value, options);
        }

    	protected virtual float FloatField(string label, float value, params GUILayoutOption[] options)
    	{
    		return EditorGUILayout.FloatField(label, value, options);
    	}

        protected virtual Vector2 Vector3Field(string label, Vector3 value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.Vector3Field(label, value, options);
        }

    	protected virtual Vector2 Vector2Field(string label, Vector2 value, params GUILayoutOption[] options)
    	{
    		return EditorGUILayout.Vector2Field(label, value, options);
    	}

        protected virtual string TextField(string text, params GUILayoutOption[] options)
        {
            return EditorGUILayout.TextField(text, options);
        }

    	protected virtual string TextField(string label, string text, params GUILayoutOption[] options)
        {
            return EditorGUILayout.TextField(label, text, options);
        }

    	protected virtual Color ColorField(string label, Color color, params GUILayoutOption[] options)
    	{
    		return EditorGUILayout.ColorField(label, color, options);
    	}

    	protected virtual UnityEngine.Object ObjectField(string label, UnityEngine.Object obj, Type objType, bool allowSceneObjects, params GUILayoutOption[] options)
    	{
    		return EditorGUILayout.ObjectField(label, obj, objType, allowSceneObjects, options);
    	}

        protected virtual float Slider(float value, float leftValue, float rightValue, params GUILayoutOption[] options)
        {
            return EditorGUILayout.Slider(value, leftValue, rightValue, options);
        }

    	protected virtual float Slider(string label, float value, float leftValue, float rightValue, params GUILayoutOption[] options)
    	{
    		return EditorGUILayout.Slider(label, value, leftValue, rightValue, options);
    	}

    	protected virtual bool Button(string text, params GUILayoutOption[] options)
    	{
    		return GUILayout.Button(text, options);
    	}

        protected virtual bool Button(string text, TextAnchor alignment, params GUILayoutOption[] options)
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.alignment = alignment;

            return GUILayout.Button(text, buttonStyle, options);
        }

    	protected virtual bool IconButton(string text, string iconPath, TextAnchor alignment, params GUILayoutOption[] options)
    	{
    		Texture iconTexture = AssetDatabase.GetCachedIcon(iconPath);
    		GUIContent icon = new GUIContent(text, iconTexture);

    		GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.alignment = alignment;

    		buttonStyle.fixedHeight = 20f;

            return GUILayout.Button(icon, buttonStyle, options);
    	}

        protected virtual void Space()
        {
            EditorGUILayout.Space();
        }
        
        protected virtual int Toolbar(int selected, string[] texts, params GUILayoutOption[] options)
        {
            return GUILayout.Toolbar(selected, texts, options);
        }
    }
}
