using UnityEditor.Animations;
using UnityEngine;

namespace SlotMaker
{
    public abstract class BasePayLineEditorConfig : ScriptableObject
    {
        public AnimatorController animator;
        public Material material;
        public ColorTableObject colorTable;
        public TextAsset json;
        public int layer;
        public string sortingLayer;
        public int sortingOrder;
        public int totalColumn;
        public int totalRow;
        public Vector2 spacing;
        public float lineWidth;
        public float xOffset;
        public float yOffset;
        public float margin;
    }
}
