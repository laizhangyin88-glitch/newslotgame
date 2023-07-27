using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

namespace SlotMaker.IoC.Strategy
{
    [CreateAssetMenu(fileName="New Spline Percent Property", menuName="SlotMaker2/IoC/Spline/Property/Percent")]
    public class SplinePercentPropertyStrategy : PropertyStrategy<SplinePositioner> 
    {
        public override float GetSingle(Component comp)
        {
            return (float)Get(comp).position;
        }

        public override void SetSingle(Component comp, float value)
        {
            if (value >= 0f && value <= 1f)
                Get(comp).position = value;
            else
                Get(comp).position = Mathf.Repeat(value, 1f);
        }
    }
}