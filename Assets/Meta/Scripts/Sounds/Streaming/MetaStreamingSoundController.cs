using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using UnityEngine;
using UnityEngine.Audio;
using SlotMaker;

namespace BagelCode
{
    public class MetaStreamingSoundController : MonoBehaviour
    {
        private GameSound assetBGM;
        private GameSound assetAmbience;

        private const string LOBBY_BGM = "BGM_Lobby";
        private const string LOBBY_AMBIENCE = "BGM_Lobby_Ambience";

        private IMetaStreamingPlayer _bgmPlayer;
        private IMetaStreamingPlayer BgmPlayer
        {
            get
            {
                if(_bgmPlayer == null)
                {
#if UNITY_WEBGL && !UNITY_EDITOR
                    _bgmPlayer = gameObject.GetComponent<MetaStreamingPlayerWebGL>();
                    if(_bgmPlayer == null)
                        _bgmPlayer = gameObject.AddComponent<MetaStreamingPlayerWebGL>();
#else
                    _bgmPlayer = gameObject.GetComponent<MetaStreamingPlayerDefault>();
                    if(_bgmPlayer == null)
                        _bgmPlayer = gameObject.AddComponent<MetaStreamingPlayerDefault>();
#endif
                }
                return _bgmPlayer;
            }
        }

        public void Init()
        {
            // Init BGM streaming player.
            BgmPlayer.Init(0.5f, true);

            var bgmURL = GetBgmURL();
            if(!string.IsNullOrEmpty(bgmURL))
                BgmPlayer.SetURL(GetBgmURL());

            if(assetAmbience == null)
            {
                assetAmbience = new GameSound();
                assetAmbience.id = LOBBY_AMBIENCE;
            }
        }

        public void OnEnterMetaGameInSlot()
        {
            GSManager.Instance.GetHandler("Casino Ambience").Stop();
        }

        public void OnLeaveMetaGameInSlot()
        {
            GSManager.Instance.GetHandler("Casino Ambience").Play();
        }

        public void PlayBGM(bool isPlay)
        {
            if(isPlay)
            {
                var bgmURL = GetBgmURL();

                if(!string.IsNullOrEmpty(bgmURL))
                {
                    if(BgmPlayer.GetURL() != bgmURL)
                        BgmPlayer.SetURL(bgmURL);

                    if(BgmPlayer.IsLoaded())
                    {
                        BgmPlayer.Play();
                        PlayAssetBGM(false);
                    }
                    else
                    {
                        BgmPlayer.Load();
                        PlayAssetBGM(true);
                    }
                }
                else
                {
                    BgmPlayer.Stop();
                    PlayAssetBGM(true);
                }
            }
            else
            {
                BgmPlayer.Stop();

                PlayAssetBGM(false);
            }
        }

        private void PlayAssetBGM(bool isPlay)
        {
            if(isPlay)
            {
                if(assetBGM == null)
                {
                    assetBGM = new GameSound();
                    assetBGM.id = LOBBY_BGM;
                }
                
                if(!assetBGM.IsPlaying)
                    assetBGM.Play();
            }
            else
            {
                if(assetBGM != null)
                    assetBGM.Stop();
            }
        }

        public void PlayAmbience(bool isPlay)
        {
            if(assetAmbience == null) return;

            if(isPlay)
            {
                assetAmbience.Play();
            }
            else
            {
                assetAmbience.Stop();
            }
        }

        public void SystemReset()
        {
            if(assetBGM != null)
            {
                assetBGM.Stop();
                assetBGM = null;
            }

            if(assetAmbience != null)
                assetAmbience.Stop();

            BgmPlayer.Clear();
        }
        
        private string GetBgmURL()
        {
            var lobbyBGM = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "lobbyBgm");
            if(lobbyBGM != null)
            {
                if(lobbyBGM.value.GetValue<bool>("isActive"))
                    return lobbyBGM.value.GetValue<string>("audioUrl");
            }

            return null;
        }
    }
}
