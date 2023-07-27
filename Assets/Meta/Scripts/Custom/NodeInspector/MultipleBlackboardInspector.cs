using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using NodeCanvas;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.StateMachines;
using NodeCanvas.Framework;
using SlotMaker;
using System.Linq;

namespace BagelCode
{
#if UNITY_EDITOR
    using BSS.Utils;
    using NodeCanvas.Editor;
    using UnityEditor;
    public class MultipleBlackboardInspector : MonoBehaviour
    {
        [System.Serializable]
        public class BlackboardResult
        {
            public BlackboardResult(Blackboard board,string name)
            {
                owner = board.gameObject.GetComponent<GraphOwner>();
                this.board = board;
                this.name = name;

                var variable=board.variables[name];
                if (variable.varType == typeof(string))
                {
                    if (variable.value != null)
                        stringValue = variable.value.ToString();
                }
                if (variable.varType == typeof(long))
                {
                    longValue = (long)variable.value;
                    hasLongValue = true;
                }
                if (variable.varType == typeof(int))
                {
                    intValue = (int)variable.value;
                    hasIntValue = true;
                }
                if (variable.varType == typeof(bool))
                {
                    boolValue = (bool)variable.value;
                    hasBoolValue = true;
                }
                if (variable.value is Object)
                {
                    objectValue = variable.value as Object;
                }
            }

            public GraphOwner owner;
            public Blackboard board;
            public string name;


            [ShowIf("hasStringValue")]
            public string stringValue;
            private bool hasStringValue => stringValue != null;

            [ShowIf("hasIntValue")]
            public int intValue;
            private bool hasIntValue = false;

            [ShowIf("hasLongValue")]
            public long longValue;
            private bool hasLongValue = false;

            [ShowIf("hasBoolValue")]
            public bool boolValue;
            private bool hasBoolValue = false;

            [ShowIf("hasObjectValue")]
            public Object objectValue;
            private bool hasObjectValue => objectValue != null;
        }

        private Object activeObj => Selection.activeObject;
        private Object[] activeObjs => Selection.objects;
        private bool isRuntime => Application.isPlaying;

    #region Main (0~10)

        [ListDrawerSettings(NumberOfItemsPerPage = 10)]
        [PropertyOrder(0)]
        public List<Blackboard> selectedBlackboards = new List<Blackboard>();

        [PropertyTooltip("Add Blackboard in selected gameobject")]
        [ButtonGroup(group: "Base Control", order: 1)]
        [Button(ButtonSizes.Medium , Name="Add")]
        public void Add()
        {
            if (activeObjs.Length > 0)
            {
                foreach (var it in activeObjs)
                {
                    AddInCondition(it);
                }
                return;
            }
        }

        [ButtonGroup(group: "Base Control", order: 1)]
        [Button(ButtonSizes.Medium, Name = "Add Childs")]
        public void AddAllChilds()
        {
            if (activeObjs.Length > 0)
            {
                foreach (var obj in activeObjs)
                {
                    if (!(obj is GameObject)) continue;
                    var blackboards = ((GameObject)obj).transform.GetComponentsInChildren<Blackboard>();
                    foreach (var blackboard in blackboards)
                    {
                        AddInCondition(blackboard);
                    }
                }
            }
        }
        [ButtonGroup(group: "Base Control", order: 1)]
        [Button(ButtonSizes.Medium, Name = "Add All Scene")]
        public void AddAllScene()
        {
            foreach (var bb in FindObjectsOfType<Blackboard>())
            {
                AddInCondition(bb);
            }
        }
        [PropertyOrder(2)]
        [Button(ButtonSizes.Medium, Name = "Add Main Blackboard")]
        public void AddMain()
        {
            AddInCondition(MainBlackboard.Get());
        }

        [PropertyOrder(3)]
        [Button(ButtonSizes.Medium, Name = "Clear")]
        public void Clear()
        {
            selectedBlackboards.Clear();
        }

        private void AddInCondition(Object obj)
        {
            if (obj == null) return;
            if (obj is Graph)
            {
                var graph = obj as Graph;
                selectedBlackboards.AddUnique(graph.blackboard as Blackboard);
            }
            if (obj is Blackboard)
            {
                var bb = obj as Blackboard;
                selectedBlackboards.AddUnique(bb);
            }
            if (obj is GameObject)
            {
                var go = obj as GameObject;
                selectedBlackboards.AddUnique(go.GetComponent<Blackboard>()); 
            }

        }
    #endregion

    #region Load (11~20)

        [PropertyOrder(11)]
        [FoldoutGroup("Load In Multiple Graphs")]
        public MultipleGraphInspector mutipleGraphs;


        [FoldoutGroup("Load In Multiple Graphs")]
        [PropertyOrder(12)]
        [Button(ButtonSizes.Medium, Name = "Load")]
        public void LoadBlackboardsInSelectedGraphs()
        {
            if (mutipleGraphs.selectedGraphs.Count == 0) return;
            foreach (var graph in mutipleGraphs.selectedGraphs)
            {
                if (graph is Graph)
                {
                    selectedBlackboards.AddUnique((graph as Graph).blackboard as Blackboard);
                }
                if (graph is GraphOwner)
                {
                    selectedBlackboards.AddUnique((graph as GraphOwner).blackboard as Blackboard);
                }
            }
        }
    #endregion

    #region Search for Name (21~30)

        [PropertyOrder(21)]
        [FoldoutGroup("Search for Name")]
        public string searchName;

        [PropertyOrder(22)]
        [FoldoutGroup("Search for Name")]
        public bool isExactly=true;


        [FoldoutGroup("Search for Name")]
        [PropertyOrder(27)]
        [Button(ButtonSizes.Medium, Name = "Search")]
        public void SearchForName()
        {
            resultBlackboardsForName.Clear();
            if (string.IsNullOrWhiteSpace(searchName)) return;

            var filterList = new List<Blackboard>();
            foreach (var board in selectedBlackboards)
            {
                if (isExactly)
                {
                    if (!board.variables.ContainsKey(searchName)) continue;
                    var result=new BlackboardResult(board, searchName);
                    resultBlackboardsForName.Add(result);
                } else
                {
                    foreach (var key in board.variables.Keys.AsEnumerable())
                    {
                        if (key.ToLower().Contains(searchName.ToLower()))
                        {
                            var result = new BlackboardResult(board, key);
                            resultBlackboardsForName.Add(result);
                        }
                    }
                }
            }
            
        }

        [ShowIf("hasResultBlackboardsForName")]
        [FoldoutGroup("Search for Name")]
        [LabelText("Results")]
        [PropertyOrder(28)]
        [ReadOnly]
        public List<BlackboardResult> resultBlackboardsForName = new List<BlackboardResult>();
        private bool hasResultBlackboardsForName => resultBlackboardsForName.Count > 0;
    #endregion

    }
    //Made By SH.LEE
#endif
}