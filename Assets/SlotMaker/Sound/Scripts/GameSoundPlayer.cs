using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker
{
    public class GameSoundPlayer : MonoBehaviour
    {
        public bool playOnEnable = false;
        [ShowIf("playOnEnable", true)]
        public string soundName;

        public void OnEnable()
        {
            if (playOnEnable)
            {
                var mgr = GSManager.Instance;
                if (mgr != null) mgr.GetHandler(soundName).Play();
            }
        }

        public void PlayGameSound(string id)
        {
            var mgr = GSManager.Instance;
            if (mgr != null) mgr.GetHandler(id).Play();
        }

        public void StopGameSound(string id)
        {
            var mgr = GSManager.Instance;
            if (mgr != null) mgr.GetHandler(id).Stop();
        }
    }
}
