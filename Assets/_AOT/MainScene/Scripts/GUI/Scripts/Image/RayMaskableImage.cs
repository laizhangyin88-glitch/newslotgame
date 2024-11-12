using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMaker
{
    public class RayMaskableImage : Image
    {
        [SerializeField] private Shader _shader;
        public Shader shader { get { return _shader; } set { if (SetPropertyUtility.SetClass(ref _shader, value)) SetMaterialDirty(); } }

        [SerializeField] private RectTransform _rayMaskTarget;
        public RectTransform rayMaskTarget { get { return _rayMaskTarget; } set { if (SetPropertyUtility.SetClass(ref _rayMaskTarget, value)) SetAllDirty(); } }

        [SerializeField] private float _maskSize;
        public float maskSize { get { return _maskSize; } set { if (SetPropertyUtility.SetStruct(ref _maskSize, value)) SetAllDirty(); } }

        [SerializeField] private bool _rayMask;
        public bool rayMask { get { return _rayMask; } set { if (SetPropertyUtility.SetStruct(ref _rayMask, value)) SetAllDirty(); } }

        [SerializeField] private Texture2D _maskTex;
        public Texture2D maskTex { get { return _maskTex; } set { if (SetPropertyUtility.SetClass(ref _maskTex, value)) SetAllDirty(); } }

        public override Material material
        {
            get
            {
                if (m_Material == null)
                {
                    m_Material = new Material(shader);
                    m_Material.hideFlags = HideFlags.DontSave;
                }
                return m_Material;
            }
            set
            {
                base.material = value;
            }
        }

        public override bool Raycast(Vector2 sp, Camera eventCamera)
        {
            bool result = base.Raycast(sp, eventCamera);
            if (result && rayMask && (rayMaskTarget != null))
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(rayMaskTarget, sp, eventCamera))
                    result = false;
            }
            return result;
        }

        protected virtual void LateUpdate()
        {
            if (rayMaskTarget == null || maskTex == null || !ValidateMaterial())
                return;

            Vector3 position = transform.position;
            Vector4 cr = new Vector4
            (
                rayMaskTarget.position.x,
                rayMaskTarget.position.y,
                rayMaskTarget.rect.width * 0.5f * rectTransform.lossyScale.x * maskSize,
                rayMaskTarget.rect.height * 0.5f * rectTransform.lossyScale.y * maskSize
            );

            // material.hideFlags = HideFlags.HideAndDontSave;
            material.SetTexture("_ClipTex", maskTex);
            material.SetFloat("_Cutoff", 0f);
            material.SetFloat("_CutoffSharpness", 1f);
            material.SetVector("_ClipPivot", new Vector4(position.x, position.y, 0f, 0f));
            material.SetVector("_ClipRange", new Vector4(cr.x / cr.z, cr.y / cr.w, 1f / cr.z, 1f / cr.w));
        }

        protected bool ValidateMaterial()
        {
            return material.HasProperty("_ClipTex") &&
                material.HasProperty("_Cutoff") &&
                material.HasProperty("_CutoffSharpness") &&
                material.HasProperty("_ClipPivot") &&
                material.HasProperty("_ClipRange");
        }
    }
}
