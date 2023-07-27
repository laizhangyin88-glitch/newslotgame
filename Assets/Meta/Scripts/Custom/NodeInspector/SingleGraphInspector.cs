using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using NodeCanvas;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.StateMachines;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
#if UNITY_EDITOR
    using NodeCanvas.Editor;
    using UnityEditor;
    using BSS.Utils.Editor;
    public class SingleGraphInspector : MonoBehaviour
    {

        private Object activeObj => Selection.activeObject;
        private Object[] activeObjs => Selection.objects;
        private bool isRuntime => Application.isPlaying;

        #region Main (0~10)
        [PropertyOrder(0)]
        public Object selectedGraph;

        [ButtonGroup(group: "Base Control", order: 1)]
        [Button(ButtonSizes.Medium)]
        public void Select()
        {
            var obj = activeObj;
            if (obj == null) return;
            if (obj is Graph)
            {
                var graph = obj as Graph;
                selectedGraph = graph;
            }
            if (obj is GameObject)
            {
                var go = obj as GameObject;
                var owner = go.GetComponent<GraphOwner>();
                if (owner != null)
                {
                    selectedGraph = owner;
                }
            }
        }

        [ButtonGroup(group: "Base Control", order: 1)]
        [Button(ButtonSizes.Medium)]
        public void Clear()
        {
            selectedGraph = null;
            ResetInfomation();
        }

        [PropertyOrder(2)]
        [Button(ButtonSizes.Medium, Name = "Apply")]
        public void ApplyInfomation()
        {
            if (selectedGraph == null)
            {
                ResetInfomation();
                return;
            }
            path = AssetDatabase.GetAssetPath(selectedGraph);
            uid = AssetDatabase.AssetPathToGUID(path);
            if (selectedGraph is GraphOwner)
            {
                var owner = selectedGraph as GraphOwner;
                serialText = UnityObjectUtility.ReadTextFile(owner.graph);
                metaSerialText = UnityObjectUtility.ReadMetaTextFile(owner.graph);
            } else
            {
                serialText = UnityObjectUtility.ReadTextFile(selectedGraph);
                metaSerialText = UnityObjectUtility.ReadMetaTextFile(selectedGraph);
            }
            GetNetestedGraphs();
        }

        private void GetNetestedGraphs()
        {
            if (selectedGraph == null)
            {
                return;
            }
            if (selectedGraph is Graph)
            {
                var graph = selectedGraph as Graph;
                serialText = graph.Serialize(false, null);
                nestedGraphs = graph.GetAllNestedGraphs<Graph>(isNestedRecursive);

            }
            if (selectedGraph is GraphOwner)
            {
                var owner = selectedGraph as GraphOwner;
                serialText = owner.graph.Serialize(true, null);
                nestedGraphs = owner.graph.GetAllNestedGraphs<Graph>(isNestedRecursive);
            }
        }

        #endregion

        #region Base Infomation (11~20)
        [FoldoutGroup("Base Infomation")]
        [PropertyOrder(11)]
        public string path;
        [FoldoutGroup("Base Infomation")]
        [PropertyOrder(12)]
        public string uid;
        [FoldoutGroup("Base Infomation")]
        [PropertyOrder(13)]
        [Multiline(8)]
        public string serialText;
        [FoldoutGroup("Base Infomation")]
        [PropertyOrder(13)]
        [Multiline(5)]
        public string metaSerialText;


        public void ResetInfomation()
        {
            uid = "";
            path = "";
            serialText = "";
            metaSerialText = "";
            nestedGraphs.Clear();
        }
        #endregion

        #region Childs Infomation (21~30)
        [FoldoutGroup("Childs Infomation")]
        [PropertyOrder(21)]
        public bool isNestedRecursive=true;

        [LabelText("Nested Graphs")]
        [FoldoutGroup("Childs Infomation")]
        [PropertyOrder(22)]
        public List<Graph> nestedGraphs = new List<Graph>();


        #endregion
        
    }
    //Made By SH.LEE
#endif
}
