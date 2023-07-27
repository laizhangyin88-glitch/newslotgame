using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode
{
    public class MetaSoundsViewController : MonoBehaviour
    {
        private const string ON_META_UI_EVENT = "OnMetaUIEvent";
        private const string ON_MAKE_SOUND = "OnMakeSound";
        private const string ON_REMOVE_SOUND = "OnRemoveSound";
        private const string ON_REMOVE_ALL_SOUND = "OnRemoveAllSounds";

        private const string ON_SYSTEM_EVENT = "OnSystemEvent";
        private const string ON_SYSTEM_RESET_EVENT = "SystemReset";

        private const string UNIQUE_KEY_FORMAT = "{0}/{1}";

        private Dictionary<string, GameObject> soundsDict;

        public void Awake()
        {
            soundsDict = new Dictionary<string, GameObject>();
        }

        public void OnDestroy()
        {
        }

        private void OnEnable()
        {
            MessageDispatcher.Register(ON_META_UI_EVENT, OnMetaUIEvent);
            MessageDispatcher.Register(ON_SYSTEM_EVENT, OnSystemEvent);
        }

        private void OnDisable()
        {
            MessageDispatcher.UnRegister(ON_META_UI_EVENT, OnMetaUIEvent);
            MessageDispatcher.UnRegister(ON_SYSTEM_EVENT, OnSystemEvent);
        }

        private void OnMetaUIEvent(EventData eventData)
        {
            if (eventData.name == ON_MAKE_SOUND)
            {
                if(eventData.value != null && eventData.value is KeyValuePair<string, string>)
                {
                    KeyValuePair<string, string> makeData = (KeyValuePair<string, string>)eventData.value;
                    MakeSound(makeData.Key, makeData.Value);
                }
            }
            else if (eventData.name == ON_REMOVE_SOUND)
            {
                if(eventData.value != null && eventData.value is KeyValuePair<string, string>)
                {
                    KeyValuePair<string, string> makeData = (KeyValuePair<string, string>)eventData.value;
                    RemoveSound(GetUniqueKey(makeData.Key, makeData.Value));
                }
            }
            else if (eventData.name == ON_REMOVE_ALL_SOUND)
            {
                RemoveAllSounds();
            }
        }

        private void OnSystemEvent(EventData eventData)
        {
            if (eventData.name == ON_SYSTEM_RESET_EVENT)
            {
                RemoveAllSounds();
            }
        }

        private void MakeSound(string bundleName, string assetName)
        {
            string uniqueName = GetUniqueKey(bundleName, assetName);

            if(IsExistSound(uniqueName)) return;

            var prefab = AssetBundleManager.LoadAsset<GameObject>(bundleName, assetName);
            if(prefab != null)
            {
                GameObject go = GameObject.Instantiate(prefab) as GameObject;
                go.name = uniqueName;
                go.transform.SetParent(transform, false);

                soundsDict.Add(uniqueName, go);
            }
        }

        private void RemoveSound(string uniqueName)
        {
            if(IsExistSound(uniqueName))
            {
                GameObject.Destroy(soundsDict[uniqueName]);
                soundsDict.Remove(uniqueName);
            }
        }

        private bool IsExistSound(string uniqueName)
        {
            return soundsDict.ContainsKey(uniqueName);
        }

        private void RemoveAllSounds()
        {
            foreach(KeyValuePair<string, GameObject> data in soundsDict)
            {
                GameObject.Destroy(data.Value);
            }

            soundsDict.Clear();
        }

        private string GetUniqueKey(string bundleName, string assetName)
        {
            return string.Format(UNIQUE_KEY_FORMAT, bundleName, assetName);
        }
    }
}
