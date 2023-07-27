using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SlotMaker.Json;
using Sirenix.OdinInspector;

namespace SlotMaker
{
	[CreateAssetMenu(fileName="New RemoveAction", menuName="SlotMaker/Refactor/RemoveAction")]
	public class GraphRefactor_RemoveAction : GraphRefactor
	{
		public GraphType graphType = GraphType.FSM | GraphType.BehaviourTree;
		public List<string> includePaths;
		public List<string> excludePaths;

		public string actionType;

		const string TYPE_VALUE = "$type";

		[Button]
		public void Run()
		{
			Run(false);
		}

		public override List<RefactoringResult> Run(bool readOnly)
		{
			return Run(graphType, includePaths, excludePaths, readOnly);
		}

		protected override int VisitBehaviourTree(JsonArray nodes, bool readOnly)
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
	    					int hit = VisitAction((JsonObject)actionTask, readOnly);
	    					if (!readOnly && hit > 0)
	    						node.Remove("_action");

    						found += hit;
	    				}
	    			}
	    		}
	    	}

	    	return found;
	    }

	    protected override int VisitActionList(JsonArray actionList, bool readOnly)
	    {
	    	int found = 0;

	    	var removeList = new List<JsonObject>();
	    	foreach (var action in actionList)
	    	{
	    		int hit = VisitAction((JsonObject)action, readOnly);
	    		if (hit > 0)
	    			removeList.Add((JsonObject)action);

	    		found += hit;
	    	}

	    	if (!readOnly && removeList.Count > 0)
	    	{
	    		foreach (var action in removeList)
	    		{
	    			actionList.Remove(action);
	    		}
	    	}

	    	return found;
	    }

	    protected override int VisitAction(JsonObject actionTask, bool readOnly)
	    {
	    	int found = 0;

	    	var type = actionTask[TYPE_VALUE];
	    	if (string.Equals((string)type, actionType))
    			++found;

	    	return found;
	    }
	}
}