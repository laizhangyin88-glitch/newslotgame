using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using SlotMaker;
using UnityEngine.Networking;
using System;

namespace BagelCode
{
    public class MetaStreamingPlayerDefault : MonoBehaviour, IMetaStreamingPlayer
    {
        private string bgmURL;

        public AudioClip audioClip;
        public AudioSource source;

        private UnityWebRequest audioRequest;

        private bool readyToPlay = false;

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
            readyToPlay = false;
            contextID = null;

            StopAllCoroutines();

            if(audioRequest != null && !audioRequest.isDone)
            {
                audioRequest.Abort();
                audioRequest = null;
            }
        }

        public static string GenerateContextID()
        {
            return Guid.NewGuid().ToString();
        }

        public void Load()
        {
            if(string.IsNullOrEmpty(bgmURL)) return;

            if(audioClip == null)
            {
                // start_downloading
                // Generate Context ID
                if(!FileDownloader.Instance.CheckCachedFile(bgmURL, CacheType.FileCache))
                {
                    if(string.IsNullOrEmpty(contextID))
                        contextID = GenerateContextID();

                    if(FileDownloader.Instance.CheckRequestedFileByURL(bgmURL))
                        ClientBgmDownloadingFunnel(bgmURL, "start_downloading");
                    
                }

                FileDownloader.Instance.LoadFile( bgmURL, 
                    CacheType.FileCache, 
                    false,
                    ReadyToSound,
                    ReadyToSoundFile,
                    null,
                    DownloadError);
            }
        }

        public void Play()
        {
            if(audioClip == null)
            {
                readyToPlay = true;
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
            if(audioClip != null) return true;

            return FileDownloader.Instance.CheckCachedFile(bgmURL, CacheType.FileCache);
        }

        private void ReadyToSound(string url)
        {
            // Ready
        }

        private void ReadyToSoundFile(FileDownloader.FileDownloadedInfo info)
        {
            if(audioRequest == null)
                StartCoroutine("LoadAudioClip", info);
        }

        private void DownloadError(FileDownloader.FileDownloadError error)
        {
            if(!string.IsNullOrEmpty(contextID))
            {
                // error
                ClientBgmDownloadingFunnel(bgmURL, "error");
            }
        }

        private IEnumerator LoadAudioClip(FileDownloader.FileDownloadedInfo info)
        {
            if(!string.IsNullOrEmpty(bgmURL))
            {
                using(audioRequest = new UnityWebRequest("file://" + info.path))
                {
                    audioRequest.downloadHandler = new DownloadHandlerAudioClip(string.Empty, AudioType.WAV);

                    var synRes = audioRequest.SendWebRequest();
                    yield return synRes;

                    if (audioRequest.isNetworkError || audioRequest.isHttpError)
                    {
                        // File cache problem. // error
                        if(!string.IsNullOrEmpty(contextID))
                            ClientBgmDownloadingFunnel(bgmURL, "error");
                    }
                    else
                    {
                        // complete_downloading
                        if(!string.IsNullOrEmpty(contextID))
                            ClientBgmDownloadingFunnel(bgmURL, "complete_downloading");

                        audioClip = DownloadHandlerAudioClip.GetContent(audioRequest);
                        source.clip = audioClip;
                        if(readyToPlay)
                            Play();
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
