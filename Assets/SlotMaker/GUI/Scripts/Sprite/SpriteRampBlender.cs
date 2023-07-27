using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpritePanel))]
    public class SpriteRampBlender : MonoBehaviour
    {
        public List<float> vOffsets;
        public float vRange;
        [RangeAttribute(0f, 1f)]
        public float blendFactor;
        public int from;
        public int to;

        private SpritePanel _panel;
        private SpritePanel panel
        {
            get
            {
                if (_panel == null)
                    _panel = GetComponent<SpritePanel>();
                return _panel;
            }
        }

        private int RAMP_V1;
        private int RAMP_V2;
        private int RAMP_BLEND_FACTOR;

        private void Awake()
        {
            RAMP_V1 = Shader.PropertyToID("_RampV1");
            RAMP_V2 = Shader.PropertyToID("_RampV2");
            RAMP_BLEND_FACTOR = Shader.PropertyToID("_BlendFactor");
        }

        private void LateUpdate()
        {
            if (panel.dynamicMaterial != null)
            {
                float v1 = vOffsets[from] + vRange * blendFactor;
                float v2 = vOffsets[to] + vRange * (1f - blendFactor);

                panel.dynamicMaterial.SetFloat(RAMP_V1, v1);
                panel.dynamicMaterial.SetFloat(RAMP_V2, v2);
                panel.dynamicMaterial.SetFloat(RAMP_BLEND_FACTOR, blendFactor);
            }
        }
    }
}
