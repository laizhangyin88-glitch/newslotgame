using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.IoC.Strategy
{
    [CreateAssetMenu(fileName="New Position Property", menuName="SlotMaker2/IoC/Transform/Property/Position")]
    public class PositionPropertyStrategy : PropertyStrategy<Transform> 
    {
        public TransformSpace positionSpace = TransformSpace.Local;

        public override Vector3 GetVector3(Component comp)
        {
            switch (positionSpace)
            {
            case TransformSpace.Local:
                return Get(comp).localPosition;
            case TransformSpace.World:
                return Get(comp).position;
            default:
                break;
            }
            
            return Vector3.zero;
        }

        public override void SetVector3(Component comp, Vector3 value)
        {
            switch (positionSpace)
            {
            case TransformSpace.Local:
                Get(comp).localPosition = value;
                break;
            case TransformSpace.World:
                Get(comp).position = value;
                break;
            default:
                break;
            }
        }
    }
}