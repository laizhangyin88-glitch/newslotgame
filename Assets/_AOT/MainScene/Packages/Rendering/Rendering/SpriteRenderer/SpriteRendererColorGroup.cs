using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Rendering.SpriteRendererUtils
{
    [ExecuteAlways]
    public class SpriteRendererColorGroup : MonoBehaviour
    {
        private SpriteRendererColorGroup _parent;
        public SpriteRendererColorGroup parent { get { return _parent; } }

        [HideInInspector]
        [SerializeField]
        protected Color _color = Color.white;
        [ShowInInspector]
        public Color color
        {
            get { return _color; }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    SetColorDirty();
                }
            }
        }

        [PropertyOrder(100)]
        public List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

        public bool IsRoot()
        {
            return parent == null;
        }

        private void OnEnable()
        {
            _parent = FindParent();
            SetColorDirty();
        }

        private void OnTransformParentChanged()
        {
            _parent = FindParent();
            SetColorDirty();
        }

        protected void OnDidApplyAnimationProperties()
        {
            SetColorDirty();
        }

        private SpriteRendererColorGroup FindParent()
        {
            if (transform.parent == null)
                return null;
            return transform.parent.GetComponentInParent<SpriteRendererColorGroup>();
        }

        private Color GetColor()
        {
            var col = color;
            var node = this;
            while (!node.IsRoot())
            {
                node = node.parent;
                col *= node.color;
            }
            return col;
        }

        public void SetColorDirty()
        {
            var col = GetColor();
            foreach (var spriteRenderer in spriteRenderers)
            {
                if (spriteRenderer != null)
                    spriteRenderer.color = col;
            }
        }

        [Button]
        public void DetectInChildren()
        {
            spriteRenderers.Clear();
            
            var children = GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var child in children)
            {
                if (child.GetComponentInParent<SpriteRendererColorGroup>() == this)
                    spriteRenderers.Add(child);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        /// EDITOR
        //////////////////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        private void OnValidate()
        {
            SetColorDirty();
        }
#endif
    }
}