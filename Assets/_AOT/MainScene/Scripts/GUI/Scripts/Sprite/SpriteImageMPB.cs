using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

namespace SlotMaker
{
    [ExecuteInEditMode]
    [AddComponentMenu("SlotMaker/UI/Sprite Image MPB")]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteImageMPB : SpriteImage
    {
        [SerializeField] private Vector4 _args = new Vector4(0f, 0f, 0f, 0f);
        private MaterialPropertyBlock mpb;
        private SpriteRenderer spriteRenderer;

        public Vector4 args
        {
            get { return _args; }
            set
            {
                if (_args != value)
                {
                    _args = value;
                    UpdateMPB(gameObject.GetComponentInParent<SpritePanel>());
                }
            }
        }

        public override void UpdateMaterial()
        {
            SpritePanel panel = gameObject.GetComponentInParent<SpritePanel>();
            if (panel != null && panel.dynamicMaterial != null)
            {
                spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                spriteRenderer.sharedMaterial = panel.dynamicMaterial;
                UpdateMPB(panel);
            }
        }

        private void UpdateMPB(SpritePanel panel)
        {
            if (spriteRenderer.sprite != null)
            {
                mpb = new MaterialPropertyBlock();
                spriteRenderer.GetPropertyBlock(mpb);
                if (panel.clipping == SpritePanel.Clipping.Distortion)
                {
                    float width = spriteRenderer.sprite.rect.width / spriteRenderer.sprite.texture.width;
                    float height = spriteRenderer.sprite.rect.height / spriteRenderer.sprite.texture.height;
                    float minX = spriteRenderer.sprite.uv[0].x;
                    float minY = spriteRenderer.sprite.uv[0].y - height;
                    float maxX = spriteRenderer.sprite.uv[0].x + width;
                    float maxY = spriteRenderer.sprite.uv[0].y;
                    mpb.SetVector(SpriteShaderUtils.UVS_PROPERTY_ID, new Vector4(minX, minY, maxX, maxY));
                    mpb.SetFloat(SpriteShaderUtils.CENTER_OFFSET_PROPERTY_ID, args.x);
                }
                spriteRenderer.SetPropertyBlock(mpb);
            }
        }
    }
}
