using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMaker
{
    public class ImageColorSetter : MonoBehaviour
    {
        public Image image;
        public List<Color> colors;

        public void SetColor(bool value)
        {
            image.color = colors[value ? 1 : 0];
        }

        public void SetColor(int value)
        {
            image.color = colors[value];
        }
    }
}