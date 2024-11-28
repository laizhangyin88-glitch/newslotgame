using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SlotMaker
{
	public class MeshTools : EditorWindow
	{
		public static Rect toolRect;
	    public static Rect drawingRect;

	    public static int selectedTool = 0;
	    public static string[] toolbarStrings = new string[]
	    {
	    	"Default",
	        "Cylinder Piece"
	    };
	    public static GUI.WindowFunction[] toolbarFunctions = new GUI.WindowFunction[]
	    {
	    	new GUI.WindowFunction(DrawDefault),
	        new GUI.WindowFunction(DrawCylinderPiece)
	    };

	    public static float toolWidth = 400f;

		[MenuItem("SlotMaker/Tools/Mesh Tools", false, 200)]
		public static void Init()
		{
			MeshTools editor = EditorWindow.GetWindow<MeshTools>();
			editor.titleContent = new GUIContent("Mesh Tools");
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

		public static void DrawDefault(int id)
		{
			EditorLayout.BeginWindow(drawingRect);

			if (EditorLayout.Button("New", 50f))
			{
				string path = EditorUtility.SaveFilePanelInProject("Create new mesh", "Default", "asset", "");
				if (!string.IsNullOrEmpty(path))
				{
					AssetDatabase.CreateAsset(new Mesh(), path);
				}
			}

			EditorLayout.EndWindow();
		}

		public class CylinderPieceModel
		{
			public int PieceCount { get; set; }
			public int Width      { get; set; }
			public int Height     { get; set; }
			public int SegmentX   { get; set; }
			public int SegmentY   { get; set; }
		}
		public static CylinderPieceModel cylinderPiece = new CylinderPieceModel();

		public static void DrawCylinderPiece(int id)
		{
			EditorLayout.BeginWindow(drawingRect);

			cylinderPiece.PieceCount = EditorLayout.IntField("Piece Count", cylinderPiece.PieceCount, 30f);
			cylinderPiece.Width = EditorLayout.IntField("Width", cylinderPiece.Width, 30f);
			cylinderPiece.Height = EditorLayout.IntField("Height", cylinderPiece.Height, 30f);
			cylinderPiece.SegmentX = EditorLayout.IntField("Segment X", cylinderPiece.SegmentX, 30f);
			cylinderPiece.SegmentY = EditorLayout.IntField("Segment Y", cylinderPiece.SegmentY, 30f);

			var go = Selection.activeGameObject;
			if (go != null)
			{
				var meshFilter = go.GetComponent<MeshFilter>();
				if (meshFilter != null)
				{
					if (EditorLayout.Button("Bake", 50f))
					{
						var mesh = meshFilter.sharedMesh;
						SlotMeshBaker.BakeCylinderPiece(ref mesh, cylinderPiece.PieceCount, cylinderPiece.Width, cylinderPiece.Height, cylinderPiece.SegmentX, cylinderPiece.SegmentY);
					}
				}
			}

			EditorLayout.EndWindow();
		}
	}
}
