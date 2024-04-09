using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using UnityEngine;
using UnityEngine.Audio;
using SlotMaker;
//using System.Diagnostics.Eventing.Reader;

namespace BagelCode
{
    public class MetaStreamingSoundController : MonoBehaviour
    {
        private GameSound assetBGM; //大厅或子游戏背景音乐对象？ "BGM_Lobby" 或子游戏背景音乐
        private GameSound assetAmbience; //"BGM_Lobby_Ambience"音乐对象

        private const string LOBBY_BGM = "BGM_Lobby";
        private const string LOBBY_AMBIENCE = "BGM_Lobby_Ambience";

        private IMetaStreamingPlayer _bgmPlayer;
        private IMetaStreamingPlayer BgmPlayer
        {
            get
            {
                if (_bgmPlayer == null)
                {
#if UNITY_WEBGL && !UNITY_EDITOR
                    _bgmPlayer = gameObject.GetComponent<MetaStreamingPlayerWebGL>();
                    if(_bgmPlayer == null)
                        _bgmPlayer = gameObject.AddComponent<MetaStreamingPlayerWebGL>();
#else
                    _bgmPlayer = gameObject.GetComponent<MetaStreamingPlayerDefault>();
                    if (_bgmPlayer == null)
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
            if (!string.IsNullOrEmpty(bgmURL))
                BgmPlayer.SetURL(GetBgmURL());

            if (assetAmbience == null)
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
            if (isPlay)
            {
                var bgmURL = GetBgmURL();

                if (!string.IsNullOrEmpty(bgmURL))
                {
                    if (BgmPlayer.GetURL() != bgmURL)
                        BgmPlayer.SetURL(bgmURL);

                    if (BgmPlayer.IsLoaded())
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


        //播放大厅背景音乐
        private void PlayAssetBGM(bool isPlay)
        {
            if (isPlay)
            {
                if (assetBGM == null)
                {
                    assetBGM = new GameSound();
                    assetBGM.id = LOBBY_BGM;
                }

                if (!assetBGM.IsPlaying)
                    assetBGM.Play();
            }
            else
            {
                if (assetBGM != null)
                    assetBGM.Stop();
            }

            _isAssetBGMPlay = isPlay;
        }

        /*public void PlayAmbience(bool isPlay)
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
        }*/


        public void PlayAmbience(bool isPlay)
        {
            //if (assetAmbience == null) return;

            if (isPlay)
            {
                if (assetAmbience == null)
                {
                    assetAmbience = new GameSound();
                    assetAmbience.id = LOBBY_AMBIENCE;
                }
                if (!assetAmbience.IsPlaying)
                    assetAmbience.Play();
            }
            else
            {
                if (assetAmbience != null)
                    assetAmbience.Stop();
            }

            _isAssetAmbiencePlay = isPlay;
        }



        public void SystemReset()
        {
            if (assetBGM != null)
            {
                assetBGM.Stop();
                assetBGM = null;
            }

            if (assetAmbience != null)
                assetAmbience.Stop();

            BgmPlayer.Clear();
        }

        private string GetBgmURL()
        {
            var lobbyBGM = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "lobbyBgm");
            if (lobbyBGM != null)
            {
                if (lobbyBGM.value.GetValue<bool>("isActive"))
                    return lobbyBGM.value.GetValue<string>("audioUrl");
            }

            return null;
        }

        public void FgBgReLoadBGM()
        {
            if (assetBGM != null)
            {
                Debug.Log("@【FgBg】 重置大厅背景音乐 ");
                assetBGM.Stop();
                assetBGM.Clear();
                assetBGM = null;
            }
            if (_isAssetBGMPlay)
            {
                PlayAssetBGM(true);
            }

            if (assetAmbience != null)
            {
                Debug.Log("@【FgBg】 重置大厅人声音乐 ");
                assetAmbience.Stop();
                assetAmbience.Clear();
                assetAmbience = null;
            }
            if (_isAssetAmbiencePlay)
            {
                PlayAmbience(true);
            }

        }

        bool _isAssetBGMPlay = false;
        bool _isAssetAmbiencePlay = false;
       /* public void FgBgSetBGMState()
        {
            if (assetBGM != null)
            {
                _isAssetBGMPlay = assetBGM.IsPlaying;
                Debug.Log($"@【FgBg】 获取背景音乐状态{assetBGM.IsPlaying}  {assetBGM.id} ");
            }
            else
            {
                _isAssetBGMPlay = false;
            }

            if (assetAmbience != null)
            {
                _isAssetAmbiencePlay = assetAmbience.IsPlaying;
                Debug.Log($"@【FgBg】 获取背景人声音效状态 {assetAmbience.IsPlaying}  {assetAmbience.id} ");
            }
            else
            {
                _isAssetAmbiencePlay = false;
            }

            Debug.Log($"@【FgBg】 背景音乐状态{_isAssetBGMPlay}  背景人声音效状态{_isAssetAmbiencePlay} ");

        }*/

    }
}
