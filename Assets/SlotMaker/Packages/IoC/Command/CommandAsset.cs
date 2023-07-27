using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.IoC
{
    [Serializable]
    public abstract class CommandAsset : ScriptableObject
    {
        public bool skippable;
        
        public abstract Command Create();
    }
}