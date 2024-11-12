using UnityEngine;

namespace SlotMaker
{
    /// <summary>
    /// ReplaceableSpriteImage is a total copy of SpriteImage component, except for:
    /// 1) Material can be replaced from outside.
    /// 2) When a component tries to find SpritePanel, it will take first 'enabled' SpritePanel.
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("SlotMaker/UI/Replaceable Sprite Image")]
    [RequireComponent(typeof(SpriteRenderer))]
    public class ReplaceableSpriteImage : MonoBehaviour
    {
        public virtual void ReplaceMaterial(Material dynamicMaterial)
        {
            gameObject.GetComponent<SpriteRenderer>().sharedMaterial = dynamicMaterial;
        }

        private void OnEnable()
        {
            UpdateMaterial();
        }

        public void OnTransformParentChanged()
        {
            if (gameObject.activeSelf)
            {
                UpdateMaterial();
            }
        }

        public virtual void UpdateMaterial()
        {
            var panels = gameObject.GetComponentsInParent<SpritePanel>();

            for (int i = 0; i < panels.Length; i++)
            {
                var panel = panels[i];

                if (panel != null && panel.dynamicMaterial != null && panel.enabled)
                {
                    var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                    spriteRenderer.sharedMaterial = panel.dynamicMaterial;
                    break;
                }
            }
        }
    }
}
