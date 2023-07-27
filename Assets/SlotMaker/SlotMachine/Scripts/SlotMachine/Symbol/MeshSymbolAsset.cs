using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;

namespace SlotMaker
{
    [CreateAssetMenu(fileName="New MeshSymbolAsset", menuName="SlotMaker/ScriptableObject/MeshSymbolAsset")]
    public class MeshSymbolAsset : BaseSymbolAsset, ISymbolMaterial, ISymbolPrefab, ISymbolGraph, ISymbolBehaviour
    {
        [SerializeField]
        private List<Material> materials;
        public List<Material> Materials { get { return materials; } }

        [SerializeField]
        private List<GameObject> prefabs;
        public List<GameObject> Prefabs { get { return prefabs; } }

        [SerializeField]
        private Graph graph;
        public Graph Graph { get { return graph; } set { graph = value; } }

        [SerializeField]
        public SymbolBehaviour symbolBehaviourPrefab;
        public SymbolBehaviour Behaviour { get { return symbolBehaviourPrefab; } set { symbolBehaviourPrefab = value; } }
    }
}
