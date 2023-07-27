using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Keno
{
    [CreateAssetMenu(fileName = "New Keno Pay", menuName = "SlotMaker/Keno/Math/Pay")]
    public class KenoPay : ScriptableObject
    {
        public List<long> pay;

        public long GetPay(int index)
        {
            if (index >= pay.Count) return 0;
            return pay[index];
        }
    }
}