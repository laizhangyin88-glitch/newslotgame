using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.IoC.Strategy.Audio
{
    [CreateAssetMenu(fileName="New AudioHandlerDispatcher", menuName="SlotMaker2/IoC/Audio/AudioHandlerDispatcher")]
    public class AudioHandlerDispatcher : AudioHandlerStrategy
    {
        public override void Play(string handlerId)
        {
            var mgr = GSManager.Instance;
            if (mgr) mgr.GetHandler(handlerId).Play();
        }

        public override void Stop(string handlerId)
        {
            var mgr = GSManager.Instance;
            if (mgr) mgr.GetHandler(handlerId).Stop();
        }

        public override void Stop() {}
    }
}