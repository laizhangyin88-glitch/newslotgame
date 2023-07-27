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
    public class MultiplePrefabInspector : MonoBehaviour
    {
        private Object activeObj => Selection.activeObject;
        private Object[] activeObjs => Selection.objects;
        private bool isRuntime => Application.isPlaying;

        #region Main (0~10)

        [ListDrawerSettings(NumberOfItemsPerPage = 10)]
        [PropertyOrder(0)]
        public List<GameObject> selectedPrefabs = new List<GameObject>();

        [PropertyOrder(2)]
        [Button(ButtonSizes.Medium)]
        public void Clear()
        {
            selectedPrefabs.Clear();
        }
        #endregion

        #region Load (11~20)

        [FoldoutGroup("Load in Project")]
        [FolderPath]
        [PropertyOrder(11)]
        public string projectPath = "Assets";
        private string[] projectPathArray = new string[1];

        [PropertyTooltip("Load Prefabs All at path with GameObject type")]
        [FoldoutGroup("Load in Project")]
        [PropertyOrder(12)]
        [Button(ButtonSizes.Medium, Name = "Load")]
        public void LoadPrefabs()
        {
            if (string.IsNullOrWhiteSpace(projectPath)) return;
            projectPathArray[0] = projectPath;
            string[] guids = AssetDatabase.FindAssets("t:GameObject", projectPathArray);
            var filter=guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).Select(path => (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)));
            selectedPrefabs.AddRange(filter);
        }

        #endregion


        #region Search for Graph (21~30)

        [PropertyOrder(21)]
        [FoldoutGroup("Search for Graph")]
        public Graph searchGraph;


        [FoldoutGroup("Search for Graph")]
        [PropertyOrder(22)]
        [Button(ButtonSizes.Medium, Name = "Search")]
        public void SearchForGraph()
        {
            resultPrefabsForGraph.Clear();
            if (searchGraph == null) return;

            var owners = selectedPrefabs.Where(x => x.GetComponent<GraphOwner>() != null).Select(x => x.GetComponent<GraphOwner>());
            var filters = owners.Where(x => x.graph == searchGraph).Select(x => x.gameObject);
            resultPrefabsForGraph.AddRange(filters);
        }

        [Header("Result")]
        [ShowIf("hasResultPrefabsForGraph")]
        [FoldoutGroup("Search for Graph")]
        [LabelText("Results")]
        [PropertyOrder(23)]
        [ReadOnly]
        public List<GameObject> resultPrefabsForGraph = new List<GameObject>();
        private bool hasResultPrefabsForGraph => resultPrefabsForGraph.Count > 0;

        #endregion
    }
#endif
}
