using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Rendering
{
    public abstract class RenderGroup : MonoBehaviour
    {
        public Color color = Color.white;

        public abstract bool IsRoot();
        public abstract RenderGroup GetParentColorGroup();
        
        public Color GetOverridenColor()
        {
            var col = color;
            var colorGroup = this;
            while (!colorGroup.IsRoot())
            {
                colorGroup = colorGroup.GetParentColorGroup();
                col *= colorGroup.color;
            }
            return col;
        }
    }
}