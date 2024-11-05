using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMaker
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Image))]
    public class ImageColorBlend : MonoBehaviour
    {
        public Vector2 mainTextureScale = Vector2.one;
        public Vector2 mainTextureOffset = Vector2.zero;
        public Vector2 blendTextureScale = Vector2.one;
        public Vector2 blendTextureOffset = Vector2.zero;
        public Color blendColor = Color.white;
        [Range(0, 1)]
        public float blendWeight;

        public enum TextureAlignment
        {
            None,
            FitToHorizontal,
            FitToVertical,
        };
        public TextureAlignment textureAlignment = TextureAlignment.None;

        private Image target;
        private RectTransform rectTransform;
        private int _MainTex;
        private int _BlendTex;
        private int _BlendColor;
        private int _BlendWeight;

        private void Awake()
        {
            target = GetComponent<Image>();
            rectTransform = target.GetComponent<RectTransform>();

            _MainTex = Shader.PropertyToID("_MainTex");
            _BlendTex = Shader.PropertyToID("_BlendTex");
            _BlendColor = Shader.PropertyToID("_BlendColor");
            _BlendWeight = Shader.PropertyToID("_BlendWeight");
        }

        private void UpdateMainTextureScale()
        {
            if (target.sprite == null || textureAlignment == TextureAlignment.None)
                return;

            Vector2 rs = rectTransform.rect.size;
            Vector2 ss = target.sprite.textureRect.size;

            if (textureAlignment == TextureAlignment.FitToHorizontal)
                mainTextureScale = new Vector2(1f, rs.y * ss.x / rs.x / ss.y);
            else if (textureAlignment == TextureAlignment.FitToVertical)
                mainTextureScale = new Vector2(rs.x * ss.y / rs.y / ss.x, 1f);
        }

        private void Update()
        {
            UpdateMainTextureScale();

            var mat = target.material;
            mat.mainTextureScale = mainTextureScale;
            mat.mainTextureOffset = mainTextureOffset;
            mat.SetTextureScale(_BlendTex, blendTextureScale);
            mat.SetTextureOffset(_BlendTex, blendTextureOffset);
            mat.SetColor(_BlendColor, blendColor);
            mat.SetFloat(_BlendWeight, blendWeight);
        }
    }
}