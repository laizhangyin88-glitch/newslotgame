using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode
{
    [RequireComponent(typeof(Image))]
    public class AtlasVectorSetter : MonoBehaviour
    {

        public float PosX;
        public float PosY;

        private void Awake()
        {
            var image = GetComponent<Image> ();
            Sprite sprite = image.sprite;
            Vector4 result = new Vector4((sprite.textureRect.min.x/sprite.texture.width)-PosX,
                (sprite.textureRect.min.y/sprite.texture.height-PosY),
                (sprite.textureRect.max.x/sprite.texture.width)+PosX,
                 (sprite.textureRect.max.y/sprite.texture.height)+PosY);
            var mat = new Material(image.material);
            image.material = mat;
            image.material.SetVector ("_AtlasInSpriteVector", result);
        }
    }
}
