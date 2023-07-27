using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMaker.IoC.Strategy
{
    [CreateAssetMenu(fileName="New CanvasGroup alpha Property", menuName="SlotMaker2/IoC/UI/Property/CanvasGroup alpha")]
    public class CanvasGroupAlphaPropertyStrategy : PropertyStrategy<CanvasGroup> 
    {
        public override float GetSingle(Component comp)
        {
            return Get(comp).alpha;
        }

        public override void SetSingle(Component comp, float value)
        {
            Get(comp).alpha = value;
        }
    }
}