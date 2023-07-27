using UnityEngine;
using System.Collections;

namespace SlotMaker
{
    [ExecuteInEditMode]
    [AddComponentMenu("SlotMaker/UI/Sprite/Property/Float")]
    [RequireComponent(typeof(SpritePanel))]
    public class SpriteMaterialFloatProperty : MonoBehaviour
    {
        public string propertyName;
        public float  propertyValue;

        int propertyId = 0;

        SpritePanel _panel;
        SpritePanel panel { get { return _panel ?? (_panel = GetComponent<SpritePanel>()); } }

        private void LateUpdate()
        {
            if (panel.dynamicMaterial != null)
            {
                if (propertyId == 0)
                    propertyId = Shader.PropertyToID(propertyName);

                panel.dynamicMaterial.SetFloat(propertyId, propertyValue);
            }
        }
    }
}
