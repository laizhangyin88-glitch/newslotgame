using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeCanvas.Framework;
using NodeCanvas.Editor;

namespace SlotMaker
{
	public class GraphRefactorEditor : EditorWindowBase<GraphRefactorEditor> 
	{
		GraphRefactor refactor;
		List<GraphRefactor.RefactoringResult> refactoringResult;
		int resultCount;
		Vector2 scrollPosition;

		public override string GetEditorName()
	    {
	        return "Graph Refactor";
	    }

	    [MenuItem("SlotMaker/Tools/Graph Refactor", false, 198)]
	    private static void Initialize()
	    {
	        CreateWindow();
	    }

	    void OnGUI()
	    {
	    	refactor = ObjectField("Refactor", refactor, typeof(GraphRefactor), false) as GraphRefactor;

	    	BeginHorizontal();
		    	if (Button(string.Format("Find({0})", resultCount)))
		    	{
		    		refactoringResult = refactor.Run(true);
		    	}
		    	if (Button("Run"))
		    	{
		    		refactoringResult = refactor.Run(false);
		    		LogResult(refactoringResult);
		    		refactoringResult = null;
		    	}
	    	EndHorizontal();

	    	resultCount = 0;
	    	if (refactoringResult != null)
	    	{
	    		using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition))
	    		{
	    			scrollPosition = scrollView.scrollPosition;

	    			for (int i = 0; i < refactoringResult.Count; ++i)
	    			{
	    				if (refactoringResult[i].found == 0)
	    					continue;
    					++resultCount;

	    				BeginHorizontal();
	    					if (IconButton(refactoringResult[i].found.ToString(), "Assets", TextAnchor.MiddleCenter, GUILayout.Width(30f)))
	    					{
	    						var graph = LoadGraph(refactoringResult[i].assetPath);
	    						GraphEditor.OpenWindow(graph);
	    					}
	    					TextField(refactoringResult[i].assetPath);
	    					if (Button("F", GUILayout.Width(25f)))
	    					{
	    						var graph = LoadGraph(refactoringResult[i].assetPath);
	    						refactoringResult[i].found = refactor.Run(graph, true);
	    					}
	    					if (Button("R", GUILayout.Width(25f)))
	    					{
	    						var graph = LoadGraph(refactoringResult[i].assetPath);
	    						refactor.Run(graph, false);	
	    						refactoringResult[i].found = 0;

	    						AssetDatabase.SaveAssets();
								AssetDatabase.Refresh();
	    					}
	    				EndHorizontal();
	    			}
	    		}
	    	}
	    }

	    void LogResult(List<GraphRefactor.RefactoringResult> results)
	    {
	    	foreach (var result in results)
	    	{
	    		Debug.Log(string.Format("{0}({1})", result.assetPath, result.found));
	    	}
	    }

	    Graph LoadGraph(string assetPath)
	    {
	    	return AssetDatabase.LoadAssetAtPath(assetPath, typeof(Graph)) as Graph;
	    }
	}
}