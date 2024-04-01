using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using SlotMaker;
using UnityEngine.Networking;

namespace BagelCode
{
    public class MetaStreamingPlayerWebGL : MonoBehaviour, IMetaStreamingPlayer
    {
        private string bgmURL;

        public AudioClip audioClip;
        public AudioSource source;

        private UnityWebRequest audioRequest;

        private string contextID;

        public void Init(float volume, bool isLoop)
        {
            source = gameObject.GetComponent<AudioSource>();
            if(source == null)
                source = gameObject.AddComponent<AudioSource>();

            source.volume = volume;
            source.loop = isLoop;
            source.outputAudioMixerGroup = GSManager.Instance.GetAudioMixerGroup(GSMixerGroup.Music);
        }

        public void SetURL(string url)
        {
            Clear();

            bgmURL = url;
            Load();
        }

        public string GetURL()
        {
            return bgmURL;
        }

        public void Clear()
        {
            Stop();
            audioClip = null;
            bgmURL = null;
            contextID = null;

            StopCoroutine("LoadAudioClip");

            if(audioRequest != null && !audioRequest.isDone)
            {
                audioRequest.Abort();
                audioRequest = null;
            }
        }

        public void Load()
        {
            if(string.IsNullOrEmpty(bgmURL)) return;

            if(audioRequest == null)
                StartCoroutine("LoadAudioClip");
        }

        public void Play()
        {
            if(audioClip == null)
            {
                Load();
            }
            else
            {
                if(!source.isPlaying)
                {
                    if(!string.IsNullOrEmpty(contextID))
                    {
                        // start_playing_bgm
                        ClientBgmDownloadingFunnel(bgmURL, "start_playing_bgm");
                        contextID = null;
                    }
                    
                    source.Play();
                }
            }
        }

        public void Stop()
        {
            if(audioClip != null)
            {
                source.Stop();
            }
        }

        public bool IsLoaded()
        {
            if(string.IsNullOrEmpty(bgmURL)) return false;
            return audioClip != null;
        }

        private IEnumerator LoadAudioClip()
        {
            if(!string.IsNullOrEmpty(bgmURL))
            {
                using(audioRequest = new UnityWebRequest(bgmURL))
                {
                    // start_downloading
                    // Generate Context ID
                    if(string.IsNullOrEmpty(contextID))
                        contextID = BiEventUtils.GenerateContextID();

                    ClientBgmDownloadingFunnel(bgmURL, "start_downloading");

                    audioRequest.downloadHandler = new DownloadHandlerAudioClip(string.Empty, AudioType.WAV);

                    var synRes = audioRequest.SendWebRequest();
                    yield return synRes;

                    if (audioRequest.isNetworkError || audioRequest.isHttpError)
                    {
                        ClientBgmDownloadingFunnel(bgmURL, "error");
                    }
                    else
                    {
                        audioClip = DownloadHandlerAudioClip.GetContent(audioRequest);
                        source.clip = audioClip;

                        // complete_downloading
                        ClientBgmDownloadingFunnel(bgmURL, "complete_downloading");
                    }

                    audioRequest.Dispose();
                    audioRequest = null;
                }
            }
        }

        public void ClientBgmDownloadingFunnel(string url, string step)
        {
            if(string.IsNullOrEmpty(contextID)) return;

            Dictionary<string, object> customData = new Dictionary<string, object>();

            // step : start_downloading, complete_downloading, error, start_playing_bgm
            customData["url"] = url;
            customData["step"] = step;
            customData["context_id"] = contextID;

            Analytics.CustomEvent("client_bgm_downloading_funnel", customData);

        }
    }
}
