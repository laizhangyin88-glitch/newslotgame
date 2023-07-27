using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker
{
    [CreateAssetMenu(fileName="New Json Variable Int List2", menuName="SlotMaker2/VariableAsset/Json/List2/Int")]
    public class JsonVariableIntList2 : VariableJson<List<List<int>>> {}
}