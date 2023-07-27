using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.IoC.Strategy.Audio
{
    [Serializable]
    public abstract class AudioHandlerStrategy : ScriptableObject
    {
        public abstract void Play(string handlerId);
        public abstract void Stop(string handlerId);
        public abstract void Stop();
    }
}