using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ParadoxNotion;
using NodeCanvas.Framework;
using NodeCanvas.BehaviourTrees;
using SlotMaker.Json;

namespace SlotMaker
{
    public class CrashReportHelper : EditorWindowBase<CrashReportHelper>
    {
        private string callStack;

        [Serializable]
        private class CallStackResult
        {
            public string callStack;
            public string[] normalizedCallStack;
            public List<List<string>> result = new List<List<string>>();
        }
        private CallStackResult result;

        private HashSet<string> blacklist = new HashSet<string>()
        {
            "NodeCanvas.Framework.ActionTask.ExecuteAction",
            "NodeCanvas.Framework.Node.Execute",
            "NodeCanvas.Framework.Connection.Execute",
            "NodeCanvas.BehaviourTrees.BehaviourTree.Tick",
            "NodeCanvas.BehaviourTrees.BehaviourTree.OnGraphUpdate",
            "NodeCanvas.StateMachines.ActionState.OnEnter",
            "NodeCanvas.StateMachines.FSMState.OnExecute",
            "NodeCanvas.StateMachines.FSMState.Update",
            "NodeCanvas.StateMachines.FSMState.CheckTransitions",
            "NodeCanvas.StateMachines.FSM.EnterState",
            "NodeCanvas.StateMachines.FSM.OnGraphUpdate",
            "NodeCanvas.StateMachines.FSM.OnGraphStarted",
            "NodeCanvas.Framework.Graph.UpdateGraph",
            "NodeCanvas.Framework.Graph.StartGraph",
            "NodeCanvas.Framework.GraphOwner.UpdateBehaviour",
            "NodeCanvas.Framework.GraphOwner.StartBehaviour",
            "NodeCanvas.Framework.GraphOwner.Start",
            "ParadoxNotion.Services.MonoManager.Update",
            "SlotMaker.ManualGraphOwner.Update"
        };

        public override string GetEditorName()
        {
            return "CrashReport Helper";
        }

        [MenuItem("SlotMaker/Tools/CrashReport Helper", false, 203)]
        private static void Initialize()
        {
            CreateWindow();
        }

        private void OnGUI()
        {
            if (Button("Parse CallStack"))
            {
                callStack = EditorGUIUtility.systemCopyBuffer;
                Find();
            }
        }

        private void Find()
        {
            result = new CallStackResult();
            result.callStack = callStack;
            
            NormalizeLines();

            var paths = Selection.assetGUIDs.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();
            paths = AssetDatabase.FindAssets("t:Graph", paths).Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();

            foreach (var path in paths)
            {
                Graph graph = AssetDatabase.LoadAssetAtPath(path, typeof(Graph)) as Graph;

                var assetList = new List<string>();
                assetList.Add(path);

                if (Verify(graph.primeNode, result.normalizedCallStack.Length - 1, assetList))
                    result.result.Add(assetList);
            }

            //if (result.result.Count > 0)
            {
                EditorGUIUtility.systemCopyBuffer = SlotSimpleJson.SerializeObject(result);
                Debug.Log("Found: " + result.result.Count);
            }
            //else 
            {
                Debug.Log("Not found");
            }
        }

        private void NormalizeLines()
        {
            result.normalizedCallStack = result.callStack.Split('\n');
            var normalizedLines = new List<string>();
            for (int i = 0; i < result.normalizedCallStack.Length; ++i)
            {
                result.normalizedCallStack[i] = result.normalizedCallStack[i].Split(new char[] { ' ', '`' })[0];
                if (!blacklist.Contains(result.normalizedCallStack[i]))
                {
                    result.normalizedCallStack[i] = result.normalizedCallStack[i].Replace(".OnUpdate", "").Replace(".OnExecute", "");

                    normalizedLines.Add(result.normalizedCallStack[i]);
                }
            }
            result.normalizedCallStack = normalizedLines.ToArray();
        }

        private bool Verify(Node node, int depth, List<string> assetList)
        {
            if (depth < 0)
                return true;
            
            var refType = ReflectionTools.GetType(result.normalizedCallStack[depth]);
            if (node == null || node.GetType() != refType)
                return false;

            var graphAssignable = node as IGraphAssignable;
            if (graphAssignable != null)
            {
                if (graphAssignable.nestedGraph == null)
                    return false;

                assetList.Add(AssetDatabase.GetAssetPath(graphAssignable.nestedGraph));

                return Verify(graphAssignable.nestedGraph.primeNode, depth - 1, assetList);
            }

            var taskAssignable = node as ITaskAssignable;
            if (taskAssignable != null && !(taskAssignable.task is ConditionTask))
            {
                refType = ReflectionTools.GetType(result.normalizedCallStack[depth - 1]);
                var actionTask = taskAssignable.task as ActionTask;
                if (actionTask == null)
                    return false;

                if (actionTask.GetType() == refType)
                    return true;

                var actionList = actionTask as ActionList;
                if (actionList != null && depth > 1)
                {
                    refType = ReflectionTools.GetType(result.normalizedCallStack[depth - 2]);
                    foreach (var action in actionList.actions)
                    {
                        if (action.GetType() == refType)
                            return true;
                    }
                }

                return false;
            }

            bool success = false;
            foreach (var connection in node.outConnections)
            {
                if (Verify(connection.targetNode, depth - 1, assetList))
                    success = true;
            }

            return success;
        }
    }
}
