using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Slots
{
    public class SymbolInstanceSpriteBinder : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public List<Sprite> sprites;

        public void BindSprite(SymbolInstance symbolInstance)
        {
            spriteRenderer.sprite = sprites[symbolInstance.value];
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        }
#endif
    }
}