using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Slots
{
    public class BGMSoundController : MonoBehaviour
    {
        public GameSound lastBGM;
        public float waitTime = 2f;
        public string transitionName = "Content_Mute";
        public float transitionTime = 3f;

        private Coroutine coroutine;

        public void InActiveAutoMute()
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }

        public void ActiveAutoMute()
        {
            InActiveAutoMute();
            coroutine = StartCoroutine(AutoMute());
        }

        private IEnumerator AutoMute()
        {
            yield return new WaitForSeconds(waitTime);

            var snapshot = GSManager.Instance.GetAudioMixerSnapshot(transitionName);
            snapshot.TransitionTo(transitionTime);

            yield return new WaitForSeconds(transitionTime);

            lastBGM.Stop();
        }
    }
}
