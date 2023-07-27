using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker
{
    [CreateAssetMenu(fileName="New Object List", menuName="SlotMaker2/VariableAsset/List/ObjectList")]
    public class ObjectListAsset : ScriptableObject
    {
        public List<UnityEngine.Object> value = new List<UnityEngine.Object>();
    }
}