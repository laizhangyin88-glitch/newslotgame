using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.IoC.Strategy
{
    [CreateAssetMenu(fileName="New Scale Property", menuName="SlotMaker2/IoC/Transform/Property/Scale")]
    public class ScalePropertyStrategy : PropertyStrategy<Transform> 
    {
        public override Vector3 GetVector3(Component comp)
        {
            return Get(comp).localScale;
        }

        public override void SetVector3(Component comp, Vector3 value)
        {
            Get(comp).localScale = value;
        }
    }
}