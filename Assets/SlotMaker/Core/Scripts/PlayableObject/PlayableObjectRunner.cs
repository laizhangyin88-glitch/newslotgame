using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public class PlayableObjectRunner : MonoBehaviour 
    {
        public string bundleName;
        public string assetName;
        public bool combineApplicationType;

        public ScriptableObject playableObject;
        public float updateInterval;

        public bool autoStart;
        public bool autoStop;

        public bool IsRunning { get { return coroutine != null; } }

        private IPlayableObject playable { get { return playableObject as IPlayableObject; } }

        private Coroutine coroutine;

        private void OnEnable()
        {
            if (autoStart)
                StartPlayableObject();
        }

        private void OnDisable()
        {
            if (autoStop)
                StopPlayableObject();
        }

        public void StartPlayableObject()
        {
            Varidate();

            StopPlayableObject();
 
            playable.StartPlayableObject();
            coroutine = StartCoroutine(UpdatePlayableObject());
        }

        public void StopPlayableObject()
        {
            if (coroutine != null)
                StopCoroutine(coroutine);

            if (playable != null)
                playable.StopPlayableObject();
        }

        private IEnumerator UpdatePlayableObject()
        {
            while (true)
            {
                playable.UpdatePlayableObject();
                yield return new WaitForSeconds(updateInterval);
            }
        }

        private void Varidate()
        {
            if (playableObject == null)
                playableObject = AssetBundleManager.LoadAsset<Metronome>(GetBundleName(), assetName);
        }
        
        private string GetBundleName()
        {
            return combineApplicationType ? ApplicationSettings.MakeApplicationBundleName(bundleName) : bundleName;
        }
    }
}