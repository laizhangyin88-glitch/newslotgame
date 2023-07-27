using SlotMaker;
using UnityEngine;

namespace BagelCode.HiddenObjects
{
    public class HiddenObjectsInGameStageController : EventMonoBehaviour
    {
        public AudioClip clip;
        public string bgmName;

        private void Start()
        {
            PlayBGM();
        }

        private void OnDestroy()
        {
            if (!string.IsNullOrEmpty(bgmName))
                GSManager.Instance.GetHandler(bgmName).Stop();
        }

        private void PlayBGM()
        {
            if (!string.IsNullOrEmpty(bgmName))
                GSManager.Instance.GetHandler(bgmName).Play();
        }
    }
}
