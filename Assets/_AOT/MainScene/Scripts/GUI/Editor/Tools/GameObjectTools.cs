using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SlotMaker
{
	public class GameObjectTools : EditorWindow
	{
		public static Rect toolRect;
	    public static Rect drawingRect;

	    public static int selectedTool = 0;
	    public static string[] toolbarStrings = new string[]
	    {
	    	"Clone",
	    	"Movement",
	    	"Rotation",
	    	"Naming"
	    };
	    public static GUI.WindowFunction[] toolbarFunctions = new GUI.WindowFunction[]
	    {
	    	new GUI.WindowFunction(DrawClone),
	    	new GUI.WindowFunction(DrawMovement),
	    	new GUI.WindowFunction(DrawRotation),
	    	new GUI.WindowFunction(DrawNaming)
	    };

	    public static float toolWidth = 400f;

		[MenuItem("SlotMaker/Tools/GameObject Tools", false, 201)]
		public static void Init()
		{
			GameObjectTools editor = EditorWindow.GetWindow<GameObjectTools>();
			editor.titleContent = new GUIContent("GameObject Tools");
		}

		private void OnGUI()
		{
			BeginWindows();

	        toolRect = new Rect(Screen.width - toolWidth, 0f, toolWidth, Screen.height);
	        drawingRect = new Rect(0f, 0f, Screen.width - toolWidth, Screen.height);

	        GUI.Window(1, toolRect, DrawTools, "Tools");
	        GUI.Window(2, drawingRect, toolbarFunctions[selectedTool], toolbarStrings[selectedTool]);

	        EndWindows();
		}

		public static void DrawTools(int id)
		{
			EditorLayout.BeginWindow(toolRect);

			selectedTool = EditorLayout.Toolbar(selectedTool, toolbarStrings, 400f);

			EditorLayout.EndWindow();
		}

		public static GameObject cloneObj;
		public static int        cloneX;
		public static int        cloneY;
		public static int        cloneZ;
		public static float      cloneMargin;

		public static void DrawClone(int id)
		{
			EditorLayout.BeginWindow(drawingRect);

			cloneX = EditorLayout.IntField("X Axis Count", cloneX, 30f);
			cloneY = EditorLayout.IntField("Y Axis Count", cloneY, 30f);
			cloneZ = EditorLayout.IntField("Z Axis Count", cloneZ, 30f);
			cloneMargin = EditorLayout.FloatField("Margin", cloneMargin, 30f);
			if (Selection.activeGameObject != null && EditorLayout.Button("Clone", 50f))
			{
				cloneObj = Selection.activeGameObject;
				var parent = cloneObj.GetParent();
				List<GameObject> objs = new List<GameObject>();
				List<GameObject> temp = new List<GameObject>();

				Undo.RegisterFullObjectHierarchyUndo(parent, "Clone");

				int index = 0;

				for (int x = 0; x < cloneX; ++x)
				{
					if (objs.Count > 0)
					{
						foreach (var obj in objs)
						{
							var go = CloneGameObject(ref index, parent, obj, Vector3.up, (float)x * cloneMargin);
							temp.Add(go);
						}
					}
					else
					{
						var go = CloneGameObject(ref index, parent, cloneObj, Vector3.up, (float)x * cloneMargin);
						objs.Add(go);
					}
				}
				objs.AddRange(temp);
				temp.Clear();

				for (int y = 0; y < cloneY; ++y)
				{
					if (objs.Count > 0)
					{
						foreach (var obj in objs)
						{
							var go = CloneGameObject(ref index, parent, obj, Vector3.up, (float)y * cloneMargin);
							temp.Add(go);
						}
					}
					else
					{
						var go = CloneGameObject(ref index, parent, cloneObj, Vector3.up, (float)y * cloneMargin);
						objs.Add(go);
					}
				}
				objs.AddRange(temp);
				temp.Clear();

				for (int z = 0; z < cloneZ; ++z)
				{
					if (objs.Count > 0)
					{
						foreach (var obj in objs)
						{
							var go = CloneGameObject(ref index, parent, obj, Vector3.forward, (float)z * cloneMargin);
							temp.Add(go);
						}
					}
					else
					{
						var go = CloneGameObject(ref index, parent, cloneObj, Vector3.forward, (float)z * cloneMargin);
						objs.Add(go);
					}
				}
				objs.AddRange(temp);
				temp.Clear();
			}

			EditorLayout.EndWindow();
		}

		private static GameObject CloneGameObject(ref int index, GameObject parent, GameObject obj, Vector3 direction, float magnitude)
		{
			var go = parent.CopyChild(obj);
			go.name = string.Format("{0} {1}", cloneObj.name, ++index);
			var localPosition = go.transform.localPosition;
			localPosition += direction * magnitude;
			go.transform.localPosition = localPosition;
			return go;
		}

		public static Vector3 movementOffset = new Vector3();

		public static void DrawMovement(int id)
		{
			EditorLayout.BeginWindow(drawingRect);

			movementOffset = EditorLayout.Vector3Field("Offset", movementOffset, 200f);
			if (EditorLayout.Button("Move(Selections)", 110f))
			{
				Undo.RecordObjects(Selection.transforms, "Move(Selections)");

				foreach (var transform in Selection.transforms)
				{
					var localPosition = transform.localPosition;
					localPosition += movementOffset;
					transform.localPosition = localPosition;
				}
			}

			EditorLayout.EndWindow();
		}

		public static Vector3 rotationAxis = new Vector3();
		public static float   rotationAngle;

		public static void DrawRotation(int id)
		{
			EditorLayout.BeginWindow(drawingRect);

			rotationAxis = EditorLayout.Vector3Field("Axis", rotationAxis, 200f);
			rotationAngle = EditorLayout.FloatField("Angle", rotationAngle, 50f);
			EditorLayout.BeginHorizontal();
			if (EditorLayout.Button("Rotate(Selections)", 110f))
			{
				Undo.RecordObjects(Selection.transforms, "Rotate(Selections)");

				float angle = 0f;
				foreach (var transform in Selection.transforms)
				{
					transform.rotation = Quaternion.AngleAxis(angle, rotationAxis);
					angle += rotationAngle;
				}
			}
			if (Selection.activeGameObject != null && EditorLayout.Button("Rotate Children", 120f))
			{
				var go = Selection.activeGameObject;

				Undo.RecordObjects(go.GetComponentsInChildren<Transform>(), "Rotate Children");

				for (int i = 0; i < go.transform.childCount; ++i)
				{
					go.transform.GetChild(i).rotation = Quaternion.AngleAxis((float)i * rotationAngle, rotationAxis);
				}
			}
			EditorLayout.EndHorizontal();

			EditorLayout.EndWindow();
		}

		public static void DrawNaming(int id)
		{
			EditorLayout.BeginWindow(drawingRect);

			if (Selection.activeGameObject != null && EditorLayout.Button("Numbering Children", 150f))
			{
				var go = Selection.activeGameObject;
				Undo.RegisterFullObjectHierarchyUndo(go, "Renaming");
				for (int i = 0; i < go.transform.childCount; ++i)
				{
					var child = go.transform.GetChild(i);
					child.gameObject.name = string.Format("{0} {1:00}", child.gameObject.name, (i + 1));
				}
			}

			EditorLayout.EndWindow();
		}
	}
}
