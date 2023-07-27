using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SlotMaker.IoC.Strategy
{
    [CreateAssetMenu(fileName="New TMP UIText Property", menuName="SlotMaker2/IoC/UI/Property/TMP UIText")]
    public class TMPUITextPropertyStrategy : PropertyStrategy<TextMeshProUGUI>
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