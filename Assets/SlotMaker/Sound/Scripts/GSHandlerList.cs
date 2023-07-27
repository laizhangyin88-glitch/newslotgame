using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker
{
    [CreateAssetMenu(fileName = "New Audio List", menuName = "SlotMaker2/Audio/Audio List")]
    public class GSHandlerList : ScriptableObject
    {
        [TableList(IsReadOnly = false, DrawScrollView = false)]
        public List<GSHandler> handlers;
    }
}
