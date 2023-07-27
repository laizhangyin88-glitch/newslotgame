using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMaker
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Image))]
    public class ImageTextureFitter : MonoBehaviour
    {
        public Vector2 mainTextureScale = Vector2.one;
        public enum TextureAlignment
        {
            None,
            FitToHorizontal,
            FitToVertical,
        };
        public TextureAlignment textureAlignment = TextureAlignment.None;

        private Image target;
        private RectTransform rectTransform;

        private void Awake()
        {
            target = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
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

        private void LateUpdate()
        {
            UpdateMainTextureScale();

            var mat = target.material;
            mat.mainTextureScale = mainTextureScale;
        }
    }
}