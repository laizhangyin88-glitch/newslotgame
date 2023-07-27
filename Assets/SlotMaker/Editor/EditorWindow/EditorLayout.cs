using UnityEngine;
using UnityEditor;
using System;

namespace SlotMaker
{
	public class EditorLayout
	{
		public static float titleHeight  = 20f;
		public static float widthMargin  = 5f;
		public static float heightMargin = 2f;

		public static float cleft   = 0f;
		public static float ctop    = 0f;
		public static float cwidth  = 0f;
		public static float cheight = 0f;

		public static Rect windowRect;

		public static float defaultWidth = 100f;
		public static float defaultHeight = 17f;
		public static bool horizontalMode = false;

		public static void BeginWindow(Rect rect)
		{
			cleft   = widthMargin;
			ctop    = titleHeight;
			cwidth  = 0f;
			cheight = 0f;
			windowRect = rect;
			horizontalMode = false;
		}

		public static void EndWindow()
		{
			cleft   = 0f;
			ctop    = 0f;
			cwidth  = 0f;
			cheight = 0f;
			horizontalMode = false;
		}

		public static Vector2 BeginScrollView(
			Vector2 scrollPosition, 
			float screenWidth, float screenHeight, 
			float viewWidth, float viewHeight, 
			bool alwaysShowHorizontal = false, bool alwaysShowVertical = false)
		{
			if (!horizontalMode)
			{
				cleft = 0f;
			}		

			return GUI.BeginScrollView(
				new Rect(cleft, ctop, screenWidth, screenHeight), 
				scrollPosition, 
				new Rect(cleft, ctop, viewWidth, viewHeight), 
				alwaysShowHorizontal, alwaysShowVertical);
		}

		public static void EndScrollView()
		{
			GUI.EndScrollView();
		}

		public static void BeginHorizontal()
		{
			cleft = 0f;
			horizontalMode = true;
		}

		public static void EndHorizontal()
		{
			ctop += defaultHeight + heightMargin;
			horizontalMode = false;
		}

		public static void BeginChangeCheck()
		{
			GUI.changed = false;
		}

		public static bool EndChangeCheck()
		{
			return GUI.changed;
		}

		public static void Space(float width = 0f, float height = -1f)
		{
			if (height < 0f) height = defaultHeight;

			if (!horizontalMode)
			{
				cleft = 0f;
			}

			cleft += width + widthMargin;

			if (!horizontalMode)
			{
				ctop += height;
			}
		}

		public static void LabelField(string label, float width, float height = -1f)
		{
			if (height < 0f) height = defaultHeight;

			if (!horizontalMode)
			{
				cleft = 0f;
			}

			EditorGUI.LabelField(new Rect(cleft, ctop, width, height), label);
			cleft += width + widthMargin;

			if (!horizontalMode)
			{
				ctop += height + heightMargin;
			}
		}

		public static bool Toggle(bool value, float width, float height = -1f)
		{
			if (height < 0f) height = defaultHeight;

			if (!horizontalMode)
			{
				cleft = 0f;
			}

			value = EditorGUI.Toggle(new Rect(cleft, ctop, width, height), value);
			cleft += width + widthMargin;

			if (!horizontalMode)
			{
				ctop += height + heightMargin;
			}

			return value;
		}

		public static int IntField(int value, float width, float height = -1f)
		{
			if (height < 0f) height = defaultHeight;

			if (!horizontalMode)
			{
				cleft = 0f;
			}

			value = EditorGUI.IntField(new Rect(cleft, ctop, width, height), value);
			cleft += width + widthMargin;

			if (!horizontalMode)
			{
				ctop += height + heightMargin;
			}

			return value;
		}

		public static int IntField(string label, int value, float width, float height = -1f)
		{
			if (height < 0f) height = defaultHeight;

			if (!horizontalMode)
			{
				cleft = 0f;
			}

			EditorGUI.LabelField(new Rect(cleft, ctop, defaultWidth, height), label);
			cleft += defaultWidth + widthMargin;

			value = EditorGUI.IntField(new Rect(cleft, ctop, width, height), value);
			cleft += width + widthMargin;

			if (!horizontalMode)
			{
				ctop += height + heightMargin;
			}

			return value;
		}

		public static float FloatField(float value, float width, float height = -1f)
		{
			if (height < 0f) height = defaultHeight;

			if (!horizontalMode)
			{
				cleft = 0f;
			}

			value = EditorGUI.FloatField(new Rect(cleft, ctop, width, height), value);
			cleft += width + widthMargin;

			if (!horizontalMode)
			{
				ctop += height + heightMargin;
			}

			return value;
		}

		public static float FloatField(string label, float value, float width, float height = -1f)
		{
			if (height < 0f) height = defaultHeight;

			if (!horizontalMode)
			{
				cleft = 0f;
			}

			EditorGUI.LabelField(new Rect(cleft, ctop, defaultWidth, height), label);
			cleft += defaultWidth + widthMargin;

			value = EditorGUI.FloatField(new Rect(cleft, ctop, width, height), value);
			cleft += width + widthMargin;

			if (!horizontalMode)
			{
				ctop += height + heightMargin;
			}

			return value;
		}

		public static Vector3 Vector3Field(string label, Vector3 value, float width, float height = -1f)
		{
			if (height < 0f) height = defaultHeight * 2f;

			if (!horizontalMode)
			{
				cleft = 0f;
			}

			value = EditorGUI.Vector3Field(new Rect(cleft, ctop, width, height), label, value);
			cleft += width + widthMargin;

			if (!horizontalMode)
			{
				ctop += height + heightMargin;
			}

			return value;
		}

		public static string TextField(string text, float width, float height = -1f)
		{
			if (height < 0f) height = defaultHeight;
			
			if (!horizontalMode)
			{
				cleft = 0f;
			}

			text = EditorGUI.TextField(new Rect(cleft, ctop, width, height), text);
			cleft += width + widthMargin;

			if (!horizontalMode)
			{
				ctop += height + heightMargin;
			}

			return text;	
		}

		public static string TextField(string label, string text, float width, float height = -1f)
		{
			if (height < 0f) height = defaultHeight;
			
			if (!horizontalMode)
			{
				cleft = 0f;
			}

			EditorGUI.LabelField(new Rect(cleft, ctop, defaultWidth, height), label);
			cleft += defaultWidth + widthMargin;
			
			text = EditorGUI.TextField(new Rect(cleft, ctop, width, height), text);
			cleft += width + widthMargin;

			if (!horizontalMode)
			{
				ctop += height + heightMargin;
			}

			return text;
		}

		public static UnityEngine.Object ObjectField(UnityEngine.Object value, Type objType, float width, float height = -1f)
		{
			if (height < 0f) height = defaultHeight;

			if (!horizontalMode)
			{
				cleft = 0f;
			}

			value = EditorGUI.ObjectField(new Rect(cleft, ctop, width, height), value, objType, true);
			cleft += width + widthMargin;

			if (!horizontalMode)
			{
				ctop += height + heightMargin;
			}

			return value;
		}

		public static UnityEngine.Object ObjectField(string label, UnityEngine.Object value, Type objType, float width, float height = -1f)
		{
			if (height < 0f) height = defaultHeight;

			if (!horizontalMode)
			{
				cleft = 0f;
			}

			EditorGUI.LabelField(new Rect(cleft, ctop, defaultWidth, height), label);
			cleft += defaultWidth + widthMargin;

			value = EditorGUI.ObjectField(new Rect(cleft, ctop, width, height), value, objType, true);
			cleft += width + widthMargin;

			if (!horizontalMode)
			{
				ctop += height + heightMargin;
			}

			return value;
		}

		public static bool Button(string label, float width, float height = -1f)
		{
			if (height < 0f) height = defaultHeight;

			if (!horizontalMode)
			{
				cleft = 0f;
			}

			bool ret = GUI.Button(new Rect(cleft, ctop, width, height), label);
			cleft += width + widthMargin;

			if (!horizontalMode)
			{
				ctop += height + heightMargin;
			}

			return ret;
		}

		public static int Toolbar(int selectedTool, string[] toolbarStrings, float width, float height = -1f)
		{
			if (height < 0f) height = defaultHeight;

			if (!horizontalMode)
			{
				cleft = 0f;
			}

			selectedTool = GUI.Toolbar(new Rect(cleft, ctop, width, height), selectedTool, toolbarStrings);
			cleft += width + widthMargin;

			if (!horizontalMode)
			{
				ctop += height + heightMargin;
			}

			return selectedTool;
		}
	}
}
