using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMaker.IoC.Strategy
{
    [CreateAssetMenu(fileName="New UIText Property", menuName="SlotMaker2/IoC/UI/Property/UIText")]
    public class UITextPropertyStrategy : PropertyStrategy<Text>
    {
        public override string GetString(Component comp)
        {
            return Get(comp).text;
        }

        public override void SetString(Component comp, string value)
        {
            Get(comp).text = value;
        }
    }
}