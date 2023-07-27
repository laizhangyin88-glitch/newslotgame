using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Slots
{
    public class GameSoundController : MonoBehaviour
    {
        public GameSound gameSound;
        public bool excludePlaying;
        public float transitionTime;

        public void Play()
        {
            var mgr = GSManager.Instance;
            if (mgr) 
            {
                if (!(excludePlaying && (gameSound.IsPlaying && !gameSound.IsFading)))
                    gameSound.Play();
            }
        }

        public void Stop()
        {
            var mgr = GSManager.Instance;
            if (mgr) gameSound.Stop();
        }

        public void TransitionTo(string transitionName)
        {
            var mgr = GSManager.Instance;
            if (mgr) mgr.GetAudioMixerSnapshot(transitionName).TransitionTo(transitionTime);
        }
    }
}
