using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    [CreateAssetMenu(fileName = "New Paytable", menuName = "SlotMaker2/Math/Paytable")]
    public class Paytable : VariableList<WinningCombination>
    {
    }
}