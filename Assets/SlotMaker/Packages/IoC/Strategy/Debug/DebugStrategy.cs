using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.IoC
{
    [CreateAssetMenu(fileName="New Debug", menuName="SlotMaker2/IoC/Debug")]
    public class DebugStrategy : ScriptableObject
    {
        public void Log(string message)
        {
            Debug.Log(message);
        }
    }
}