using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using NodeCanvas.Framework;

namespace SlotMaker
{
    [AddComponentMenu("SlotMaker/Sound/Manager")]
    public class GSManager : MonoWeakSingleton<GSManager>
    {
        public AudioMixer masterMixer;
        public AudioMixer musicMixer;
        public AudioMixer sfxMixer;

        // Meta
        public AudioMixer metaMusicMixer;
        //

        public Blackboard mixerGroups;
        public Blackboard snapshots;
        public Blackboard easeCurves;
        public List<Blackboard> handlers;
    	public ObjectPool pool;

        public float clipLifeTime = 60f;
        public const float volumeOfMute = -80f;

        protected void Awake()
        {
            MusicVolume = (float)PlayerPrefs.GetInt("MUTE_MUSIC", 1);
            SfxVolume = (float)PlayerPrefs.GetInt("MUTE_SFX", 1);

            // StartCoroutine(ClearUnusedClipsCoroutine());
        }

    	public float MusicVolume
    	{
    		get
    		{
                float vol;
                musicMixer.GetFloat("musicVol", out vol);
                if (metaMusicMixer != null)
                    metaMusicMixer.GetFloat("musicVol", out vol);
                return 1f - vol / volumeOfMute;
    		}

    		set
    		{
                musicMixer.SetFloat("musicVol", (1f - value) * volumeOfMute);
                if (metaMusicMixer != null)
                    metaMusicMixer.SetFloat("musicVol", (1f - value) * volumeOfMute);
            }
    	}

    	public float SfxVolume
    	{
    		get
    		{
                float vol;
                sfxMixer.GetFloat("sfxVol", out vol);
                return 1f - vol / volumeOfMute;
    		}

    		set
    		{
                sfxMixer.SetFloat("sfxVol", (1f - value) * volumeOfMute);
    		}
    	}

        public AudioMixerGroup GetAudioMixerGroup(GSMixerGroup output)
        {
            return mixerGroups.GetValue<AudioMixerGroup>(output.ToString());
        }

        public AudioMixerSnapshot GetAudioMixerSnapshot(string snapshotName)
        {
            return snapshots.GetValue<AudioMixerSnapshot>(snapshotName);
        }

        public AnimationCurve GetEaseCurve(GSEaseType easeType)
        {
            return easeCurves.GetValue<AnimationCurve>(easeType.ToString());
        }

    	public GSSource GetSource()
    	{
    		return pool.GetObject().GetComponent<GSSource>();
    	}

    	public IGSHandler GetHandler(string handlerId)
    	{
    		for (int i = 0; i < handlers.Count; ++i)
            {
                var variable = handlers[i].GetVariable<GSHandler>(handlerId);
                if (variable != null)
                    return variable.value;
            }

    		Debug.LogWarning(string.Format("Do not found GSHander({0})!", handlerId));

    		return new GSNullHandler();
    	}

        public void ClearUnusedClips(float cachingTime = float.MinValue)
        {
            for (int i = 0; i < handlers.Count; ++i)
            {
                var variables = handlers[i].variables;
                foreach (var pair in variables)
                {
                    var handler = (GSHandler)(pair.Value.value);
                    var clip = handler.clip;

                    if (clip.IsUnUsedPtr(cachingTime))
                        clip.Clear();
                }
            }
        }

        private IEnumerator ClearUnusedClipsCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(clipLifeTime);

                ClearUnusedClips(clipLifeTime);

                yield return Resources.UnloadUnusedAssets();
            }
        }
    }
}
