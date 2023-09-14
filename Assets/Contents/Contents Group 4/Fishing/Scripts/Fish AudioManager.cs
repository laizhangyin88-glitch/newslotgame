using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishAudioManager : MonoSingleton<FishAudioManager>
    {
        int CurrentAudioCount = 60;
        Dictionary<int, AudioSource> AllAudioSource = new Dictionary<int, AudioSource>();
        Dictionary<string, AudioClip> AllAudioClips = new Dictionary<string, AudioClip>();
        bool IsOpenMusic = true;
        bool IsOpenSoundEffects = true;
        GameObject audioPanel;
        AudioSource AudioBG;

        public void Init(GameObject obj)
        {
            audioPanel = obj;
            audioPanel.transform.parent = FishGameUIManager.Instance.GamePanelRoot.transform;
            FindView();
        }

        private void FindView()
        {
            AudioBG = audioPanel.transform.Find("BGAO").GetComponent<AudioSource>();
            for (int i = 1; i < CurrentAudioCount + 1; i++)
            {
                AllAudioSource[i] = audioPanel.transform.Find("AO" + i).GetComponent<AudioSource>();
            }
        }

        public void ResetSoundEffect(bool isOpen, float volume)
        {
            float m_volume = isOpen ? volume : 0;
            IsOpenSoundEffects = isOpen;
            foreach (var item in AllAudioSource.Values)
                item.volume = m_volume;
        }

        public void ResetBGMusic(bool isOpen, float volume)
        {
            float m_volume = isOpen ? volume : 0;
            IsOpenMusic = isOpen;
            AudioBG.volume = m_volume;
        }

        private void PlayAssignAudio(AudioSource audioSource, float volume, AudioClip audioClip, bool isLoop)
        {
            audioSource.volume = volume;
            audioSource.clip = audioClip;
            audioSource.loop = isLoop;
            audioSource.Play();
        }


        public AudioSource GetAudioClip()
        {
            foreach (var item in AllAudioSource.Values)
            {
                if (!item.isPlaying)
                    return item;
            }
            return null;
        }

        public FishSoundConfig GetAudioInfo(int index)
        {
            if (index > 0)
                return FishGameManager.Instance.gameData.SoundConfigList[index - 1];
            else
                return null;
            
        }

        public void PlayBGAudio(int index, float volume)
        {
            volume = IsOpenMusic ? volume : 0;
            FishSoundConfig audioInfo = GetAudioInfo(index);
            if (audioInfo == null)
                return;
            if (!AllAudioClips.ContainsKey(audioInfo.soundName) || AllAudioClips[audioInfo.soundName] == null)
            {
                AudioClip gameObj = AssetBundleManager.LoadAsset<AudioClip>("fishing", audioInfo.soundName);
                if (gameObj != null)
                {
                    AudioClip obj = Instantiate(gameObj);
                    AllAudioClips[audioInfo.soundName] = obj;
                    PlayAssignAudio(AudioBG, volume, AllAudioClips[audioInfo.soundName], true);
                }
                else
                    Debug.LogError("资源加载失败==> " + audioInfo.soundName);
            }
            else
            {
                PlayAssignAudio(AudioBG, volume, AllAudioClips[audioInfo.soundName], true);
            }
        }

        public void PlayNormalAudio(int index, float volume = 1, bool isLoop = false)
        {
            volume = IsOpenSoundEffects ? volume : 0;
            var audioInfo = GetAudioInfo(index);
            if (audioInfo == null) return;
            if (!AllAudioClips.ContainsKey(audioInfo.soundName) || AllAudioClips[audioInfo.soundName] == null)
            {
                AudioClip gameObj = AssetBundleManager.LoadAsset<AudioClip>("fishing", audioInfo.soundName);
                if (gameObj != null)
                {
                    AudioClip obj = Instantiate(gameObj);
                    AllAudioClips[audioInfo.soundName] = obj;
                    var selectAudio = GetAudioClip();
                    if (selectAudio == null) return;
                    PlayAssignAudio(selectAudio, volume, AllAudioClips[audioInfo.soundName], isLoop);
                }
                else
                    Debug.LogError("资源加载失败==> " + audioInfo.soundName);
            }
            else
            {
                var selectAudio = GetAudioClip();
                if (selectAudio == null) return;
                PlayAssignAudio(selectAudio, volume, AllAudioClips[audioInfo.soundName], isLoop);
            }
        }

        public void StopMusic()
        {
            AudioBG.Stop();
        }

        public void StopNormalAudio(int index)
        {
            var audioInfo = GetAudioInfo(index);
            if (audioInfo == null) return;
            foreach (var item in AllAudioSource.Values)
            {
                if (item.isPlaying && AllAudioClips.ContainsKey(audioInfo.soundName) && item.clip == AllAudioClips[audioInfo.soundName])
                {
                    item.Stop();
                    item.clip = null;
                    break;
                }
            }
        }

        public void StopAllNormalAudio()
        {
            foreach (var item in AllAudioSource.Values)
            {
                if (item.isPlaying)
                    item.Stop();
                item.clip = null;
            }
        }

        public void StopAllAudio()
        {
            StopMusic();
            StopAllNormalAudio();
        }

        protected override void OnDestroy()
        {
            
        }
    }
}

