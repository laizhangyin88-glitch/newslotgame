using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public class MetronomeListener : MonoBehaviour 
    {
        public string bundleName;
        public string assetName;
        public bool combineApplicationType;

        public Metronome metronome;

        public UnityIntEvent onTrigger;

        private void OnEnable()
        {
            Varidate();

            metronome.onTrigger += OnTrigger;
        }

        private void OnDisable()
        {
            if (metronome != null)
                metronome.onTrigger -= OnTrigger;
        }

        private void OnTrigger(int beatType)
        {
            onTrigger.Invoke(beatType);
        }

        private void Varidate()
        {
            if (metronome == null)
                metronome = AssetBundleManager.LoadAsset<Metronome>(GetBundleName(), assetName);
        }
        
        private string GetBundleName()
        {
            return combineApplicationType ? ApplicationSettings.MakeApplicationBundleName(bundleName) : bundleName;
        }
    }
}