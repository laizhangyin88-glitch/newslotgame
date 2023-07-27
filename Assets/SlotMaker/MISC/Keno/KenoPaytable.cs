using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Keno
{
    [CreateAssetMenu(fileName = "New Keno Paytable", menuName = "SlotMaker/Keno/Math/Paytable")]
    public class KenoPaytable : ScriptableObject
    {
        public List<KenoPay> paytable;
    }
}