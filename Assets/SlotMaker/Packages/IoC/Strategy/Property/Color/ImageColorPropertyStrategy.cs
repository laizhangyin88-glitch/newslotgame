using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMaker.IoC.Strategy
{
    [CreateAssetMenu(fileName="New Image Color Property", menuName="SlotMaker2/IoC/UI/Property/Image Color")]
    public class ImageColorPropertyStrategy : PropertyStrategy<Image> 
    {
        public override Color GetColor(Component comp)
        {
            return Get(comp).color;
        }

        public override void SetColor(Component comp, Color value)
        {
            Get(comp).color = value;
        }
    }
}