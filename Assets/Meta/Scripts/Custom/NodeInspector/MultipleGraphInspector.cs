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
    public class MultipleGraphInspector : MonoBehaviour
    {
        private Object activeObj => Selection.activeObject;
        private Object[] activeObjs => Selection.objects;
        private bool isRuntime => Application.isPlaying;

        #region Main (0~10)

        [ListDrawerSettings(NumberOfItemsPerPage = 10)]
        [PropertyOrder(0)]
        public List<Object> selectedGraphs = new List<Object>(1024);

        [PropertyTooltip("Add Selected Graph Or GraphOwner")]
        [ButtonGroup(group: "Base Control", order: 1)]
        [Button(ButtonSizes.Medium)]
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
                    var owners = ((GameObject)obj).transform.GetComponentsInChildren<GraphOwner>();
                    foreach (var owner in owners)
                    {
                        AddInCondition(owner.gameObject);
                    }
                }
            }
        }

        [ButtonGroup(group: "Base Control", order: 1)]
        [Button(ButtonSizes.Medium, Name = "Add All Scene")]
        public void AddAllScene()
        {
            foreach (var owner in FindObjectsOfType<GraphOwner>())
            {
                AddInCondition(owner.gameObject);
            }
        }


        [PropertyOrder(2)]
        [Button(ButtonSizes.Medium)]
        public void Clear()
        {
            selectedGraphs.Clear();
        }


        private void AddInCondition(Object obj)
        {
            if (obj == null) return;
            if (obj is Graph)
            {
                var graph = obj as Graph;
                selectedGraphs.AddUnique(graph);
            }
            if (obj is GraphOwner)
            {
                var owner = obj as GraphOwner;
                selectedGraphs.AddUnique(owner);
            }
            if (obj is GameObject)
            {
                var go = obj as GameObject;
                var owner = go.GetComponent<GraphOwner>();
                if (owner != null)
                {
                    selectedGraphs.AddUnique(owner);
                }
            }

        }
        #endregion


        #region Load (11~20)

        [FoldoutGroup("Load in Project")]
        [FolderPath]
        [PropertyOrder(11)]
        public string projectPath = "Assets";
        private string[] pathArray = new string[1];

        [PropertyTooltip("Load Assets All at path with graph type")]
        [FoldoutGroup("Load in Project")]
        [PropertyOrder(12)]
        [Button(ButtonSizes.Medium,Name="Load")]
        public void LoadGraphs()
        {
            if (string.IsNullOrEmpty(projectPath)) return;

            pathArray[0] = projectPath;
            string[] guids= AssetDatabase.FindAssets("t:graph", pathArray);

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Graph g = (Graph)AssetDatabase.LoadAssetAtPath(path, typeof(Graph));
                AddInCondition(g);
            }
        }

        #endregion

        #region Search for Text (21~30)

        [PropertyOrder(21)]
        [FoldoutGroup("Search for Text")]
        public string searchText;


        [FoldoutGroup("Search for Text")]
        [PropertyOrder(22)]
        [Button(ButtonSizes.Medium, Name = "Search")]
        public void SearchForText()
        {
            resultGraphsForText.Clear();
            if (string.IsNullOrWhiteSpace(searchText)) return;

            var graphs = selectedGraphs.Where(x => x is Graph).Select(x => x as Graph).Select(x=>(x,x.Serialize(false,null)));
            var filterGraphs=graphs.Where(x => x.Item2.ToLower().Contains(searchText.ToLower())).Select(x => x.Item1);

            var owners = selectedGraphs.Where(x => x is GraphOwner).Select(x => x as GraphOwner).Select(x => (x, x.graph.Serialize(false, null)));
            var filterOwners= owners.Where(x => x.Item2.ToLower().Contains(searchText.ToLower())).Select(x => x.Item1).ToList();

            resultGraphsForText.AddRange(filterGraphs);
            resultGraphsForText.AddRange(filterOwners);
        }

        [Header("Result")]
        [ShowIf("hasResultGraphsForText")]
        [FoldoutGroup("Search for Text")]
        [LabelText("Results")]
        [PropertyOrder(23)]
        [ReadOnly]
        public List<Object> resultGraphsForText = new List<Object>();
        private bool hasResultGraphsForText => resultGraphsForText.Count > 0;

        [ShowIf("hasResultGraphsForText")]
        [FoldoutGroup("Search for Text")]
        [PropertyOrder(24)]
        [Button(ButtonSizes.Medium, Name = "Filter")]
        public void AddTextResults()
        {
            selectedGraphs.Clear();
            foreach (var it in resultGraphsForText)
                AddInCondition(it);
            resultGraphsForText.Clear();
        }

        

        


        #endregion

        #region Search With Graph (31~40)

        [PropertyOrder(31)]
        [FoldoutGroup("Search for Graph")]
        public Graph searchGraph;


        [FoldoutGroup("Search for Graph")]
        [PropertyOrder(32)]
        [Button(ButtonSizes.Medium, Name = "Search")]
        public void SearchForGraph()
        {
            resultGraphsForGraph.Clear();
            if (searchGraph==null) return;

            var graphs = selectedGraphs.Where(x => x is Graph).Select(x => x as Graph);
            var owners = selectedGraphs.Where(x => x is GraphOwner).Select(x => x as GraphOwner);

            resultGraphsForGraph.AddRange(graphs.Where(x=>x.GetAllNestedGraphs<Graph>(false).Exists(g=>g==searchGraph)));
            resultGraphsForGraph.AddRange(owners.Where(x=>x.graph.GetAllNestedGraphs<Graph>(false).Exists(g=>g==searchGraph)));
            resultGraphsForGraph.Distinct();
        }

        [Header("Result")]
        [ShowIf("hasResultGraphsForGraph")]
        [FoldoutGroup("Search for Graph")]
        [LabelText("Results")]
        [PropertyOrder(33)]
        [ReadOnly]
        public List<Object> resultGraphsForGraph = new List<Object>();
        private bool hasResultGraphsForGraph => resultGraphsForGraph.Count > 0;

        [ShowIf("hasResultGraphsForGraph")]
        [FoldoutGroup("Search for Graph")]
        [PropertyOrder(34)]
        [Button(ButtonSizes.Medium, Name = "Filter")]
        public void AddGraphResults()
        {
            selectedGraphs.Clear();
            foreach (var it in resultGraphsForGraph)
                AddInCondition(it);
            resultGraphsForGraph.Clear();
        }

        

        #endregion
    }
    //Made By SH.LEE
#endif
}
