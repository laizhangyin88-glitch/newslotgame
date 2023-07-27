using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.IoC.Strategy
{
    [CreateAssetMenu(fileName="New Sprite Color Property", menuName="SlotMaker2/IoC/Sprite/Property/Color")]
    public class SpriteColorPropertyStrategy : PropertyStrategy<SpriteRenderer> 
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