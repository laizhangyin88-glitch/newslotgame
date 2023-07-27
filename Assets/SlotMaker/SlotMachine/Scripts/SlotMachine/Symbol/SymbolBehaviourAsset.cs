using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;

namespace SlotMaker
{
    [CreateAssetMenu(fileName="New SymbolBehaviourAsset", menuName="SlotMaker/ScriptableObject/SymbolBehaviourAsset")]
    public class SymbolBehaviourAsset : BaseSymbolAsset, ISymbolSprite, ISymbolPrefab, ISymbolBehaviour
    {
        [SerializeField]
        private List<Sprite> sprites;
        public List<Sprite> Sprites { get { return sprites; } }

        [SerializeField]
        private List<GameObject> prefabs;
        public List<GameObject> Prefabs { get { return prefabs; } }

        [SerializeField]
        public SymbolBehaviour behaviour;
        public SymbolBehaviour Behaviour { get { return behaviour; } set { behaviour = value; } }
    }
}
