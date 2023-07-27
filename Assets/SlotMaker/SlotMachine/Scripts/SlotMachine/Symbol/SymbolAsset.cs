using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;

namespace SlotMaker
{
    [CreateAssetMenu(fileName="New SymbolAsset", menuName="SlotMaker/ScriptableObject/SymbolAsset")]
    public class SymbolAsset : BaseSymbolAsset, ISymbolSprite, ISymbolPrefab, ISymbolGraph
    {
        [SerializeField]
        private List<Sprite> sprites;
        public List<Sprite> Sprites { get { return sprites; } }

        [SerializeField]
        private List<GameObject> prefabs;
        public List<GameObject> Prefabs { get { return prefabs; } }

        [SerializeField]
        private Graph graph;
        public Graph Graph { get { return graph; } set { graph = value; } }
    }
}
