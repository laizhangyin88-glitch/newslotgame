using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    [CreateAssetMenu(fileName="New Void", menuName="SlotMaker2/VariableAsset/Void")]
    public class VariableVoid : VariableAsset
    {
        protected override object objectValue { get { return null; } set {} }
        public override Type varType { get { return typeof(void); } }
    }
}