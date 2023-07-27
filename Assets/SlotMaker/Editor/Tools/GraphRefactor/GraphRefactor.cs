using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeCanvas.Framework;
using SlotMaker.Json;

namespace SlotMaker
{
	public abstract class GraphRefactor : ScriptableObject
	{
		public enum GraphType
		{
			FSM           = 0x01,
			BehaviourTree = 0x02
		};

		public class RefactoringResult
		{
			public string assetPath;
			public int found;
		}

		public const string TYPE_ACTION_LIST = "NodeCanvas.Framework.ActionList";

		public const string TYPE_FSM = "NodeCanvas.StateMachines.FSM";
		public const string TYPE_ACTION_STATE = "NodeCanvas.StateMachines.ActionState";

		public const string TYPE_BEHAVIOUR_TREE = "NodeCanvas.BehaviourTrees.BehaviourTree";
		public const string TYPE_ACTION_NODE = "NodeCanvas.BehaviourTrees.ActionNode";

		public abstract List<RefactoringResult> Run(bool readOnly);

		protected virtual int VisitProperty(JsonObject property, bool readOnly) { return 0; }

		protected string GetFilter(GraphType graphType)
		{
			bool fsm = ((graphType & GraphType.FSM) == GraphType.FSM);
			bool behaviourTree = ((graphType & GraphType.BehaviourTree) == GraphType.BehaviourTree);
			if (fsm && behaviourTree)
				return "t:Graph";
			else if (fsm)
				return "t:FSM";
			else if (behaviourTree)
				return "t:BehaviourTree";

			return null;
		}

		protected List<RefactoringResult> Run(GraphType graphType, List<string> includePaths, List<string> excludePaths, bool readOnly)
		{
			var result = new List<RefactoringResult>();

			string[] assetGUIDs = AssetDatabase.FindAssets(GetFilter(graphType), includePaths.ToArray());
			foreach (var assetGUID in assetGUIDs)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);

				bool excluded = false;
				foreach (var excludePath in excludePaths)
				{
					if (assetPath.IndexOf(excludePath, StringComparison.OrdinalIgnoreCase) >= 0)
					{
						excluded = true;
						break;
					}
				}
				if (excluded)
					continue;

    			var graph = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Graph)) as Graph;
    			int found = Run(graph, readOnly);

    			if (found > 0)
    			{
    				result.Add(new RefactoringResult
    				{
    					assetPath = assetPath,
    					found = found
					});
    			}
			}

			if (!readOnly)
			{
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}

			return result;
		}

		public virtual int Run(Graph graph, bool readOnly)
		{
			string json;
			List<UnityEngine.Object> references;
			graph.GetSerializationData(out json, out references);

			int found = 0;

			var jgraph = (JsonObject)SlotSimpleJson.DeserializeObject(json);
			var type = jgraph["type"];
			if (string.Equals(type, TYPE_FSM))
				found += VisitFSM((JsonArray)jgraph["nodes"], readOnly);
			else if (string.Equals(type, TYPE_BEHAVIOUR_TREE))
				found += VisitBehaviourTree((JsonArray)jgraph["nodes"], readOnly);

			if (found > 0)
			{
				if (!readOnly)
				{
					json = SlotSimpleJson.SerializeObject(jgraph);
					graph.Deserialize(json, false, references);
					EditorUtility.SetDirty(graph);
				}
			}

			return found;
		}

		protected virtual int VisitFSM(JsonArray nodes, bool readOnly)
	    {
	    	int found = 0;

	    	foreach (JsonObject node in nodes)
	    	{
	    		var type = node["$type"];
	    		if (string.Equals(type, TYPE_ACTION_STATE))
	    		{
	    			if (node.ContainsKey("_actionList"))
	    			{
		    			var _actionList = (JsonObject)node["_actionList"];
		    			var actionList = (JsonArray)_actionList["actions"];
		    			found += VisitActionList(actionList, readOnly);
	    			}
	    		}
	    	}

	    	return found;
	    }

	    protected virtual int VisitBehaviourTree(JsonArray nodes, bool readOnly)
	    {
	    	int found = 0;

	    	foreach (JsonObject node in nodes)
	    	{
	    		var type = node["$type"];
	    		if (string.Equals(type, TYPE_ACTION_NODE))
	    		{
	    			if (node.ContainsKey("_action"))
	    			{
	    				var actionTask = (JsonObject)node["_action"];
	    				type = actionTask["$type"];
	    				if (string.Equals(type, TYPE_ACTION_LIST))
	    				{
	    					var actionList = (JsonArray)actionTask["actions"];
	    					found += VisitActionList(actionList, readOnly);
	    				}
	    				else 
	    				{
	    					found += VisitAction((JsonObject)actionTask, readOnly);
	    				}
	    			}
	    		}
	    	}

	    	return found;
	    }

	    protected virtual int VisitActionList(JsonArray actionList, bool readOnly)
	    {
	    	int found = 0;

	    	foreach (var action in actionList)
	    	{
	    		found += VisitAction((JsonObject)action, readOnly);
	    	}

	    	return found;
	    }

	    protected virtual int VisitAction(JsonObject actionTask, bool readOnly)
	    {
	    	int found = 0;

	    	foreach (var pair in actionTask)
	    	{
	    		if (pair.Value is JsonObject)
	    		{
	    			found += VisitProperty((JsonObject)pair.Value, readOnly);
	    		}
	    	}

	    	return found;
	    }
	}
}