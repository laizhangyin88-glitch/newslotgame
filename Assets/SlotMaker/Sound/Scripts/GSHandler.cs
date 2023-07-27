using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using Sirenix.OdinInspector;

namespace SlotMaker
{
    public enum GSMixerGroup
    {
        Music,
        Default,
        Voice,
        Compressor,
        Event,
        Meta
    };

    public enum GSPlayingType
    {
    	Independent,
    	FirstOnly,
    	LastOnly,
        CountLimit
    };

    public enum GSEaseType
    {
        None,
        Linear,
        EaseInQuad
    };

    [Serializable]
    public class GSFadeInOut
    {
        [HorizontalGroup]
        [HideLabel]
        public GSEaseType easeType;

        [HorizontalGroup]
        [HideLabel]
        [HideIf("easeType", GSEaseType.None)]
        public float time;
    }

    [Serializable]
    public class GSHandler : IGSHandler
    {
    	public delegate void VolumeChangedCallBack(float vol);
    	public event VolumeChangedCallBack onVolumeChanged;

    	public delegate void MuteCallBack(bool mute);
    	public event MuteCallBack onMute;

    	public delegate void StopCallBack();
    	public event StopCallBack onStop;

    	public delegate void PauseCallBack();
    	public event PauseCallBack onPause;

    	public delegate void UnPauseCallBack();
    	public event UnPauseCallBack onUnPause;

        public delegate void ClearCallBack();
        public event ClearCallBack onClear;

        [TableColumnWidth(100)]
        public string handlerId = string.Empty;
    	
        [TableColumnWidth(200)]
        public GSClip clip = null;
        
        [TableColumnWidth(50)]
    	public GSMixerGroup output = GSMixerGroup.Default;
        
        [TableColumnWidth(22)]
    	public bool loop = false;
        
        [TableColumnWidth(40)]
        [Range(0f, 1f)]
    	public float volume = 1f;
        
        [TableColumnWidth(40)]
    	public float delay = 0f;

        [TableColumnWidth(50)]
    	public GSPlayingType playingType = GSPlayingType.Independent;
        
        [TableColumnWidth(20)]
        [EnableIf("playingType", GSPlayingType.CountLimit)]
        public int countLimit = 0;
        
        [TableColumnWidth(80)]
        public GSFadeInOut fadeIn = null;

        [TableColumnWidth(80)]
        public GSFadeInOut fadeOut = null;
        
        [TableColumnWidth(22)]
        public bool autoRelease = true;
        
        public int UseCount { get; protected set; }

    	public float Volume
    	{
    		set
    		{
    			if (onVolumeChanged != null)
    			{
    				onVolumeChanged(Mathf.Clamp01(value));
    			}
    		}
    	}

    	private bool mute;
    	public bool Mute
    	{
    		get
    		{
    			return mute;
    		}

    		set
    		{
    			mute = value;

    			if (onMute != null)
    				onMute(value);
    		}
    	}

        public AudioMixerGroup outputAudioMixerGroup
        {
            get
            {
                return GSManager.Instance.GetAudioMixerGroup(output);
            }
        }

        public GSSource GetSource()
        {
            var source = GSManager.Instance.GetSource();
            source.Initialize(this);
            ++UseCount;
            return source;
        }

        public void RegisterSource(GSSource source)
        {
            source.Initialize(this);
            ++UseCount;
        }

    	public void Play()
    	{
    		Play(delay);
    	}

    	public void Play(float delay)
    	{
    		switch (playingType)
    		{
    		case GSPlayingType.Independent:
    			break;
    		case GSPlayingType.FirstOnly:
    			{
    				if (UseCount > 0)
    					return;
    			}
    			break;
    		case GSPlayingType.LastOnly:
    			Stop();
    			break;
            case GSPlayingType.CountLimit:
                {
                    if (UseCount >= countLimit)
                        return;
                }    
                break;
    		}

    		var source = GetSource();
    		source.Play(delay);
    	}

    	public void Pause()
    	{
            if (onPause != null)
                onPause();
    	}

    	public void UnPause()
    	{
            if (onUnPause != null)
                onUnPause();
    	}

    	public void Stop()
    	{
            if (onStop != null)
                onStop();
    	}

        public void Clear()
        {
            if (onClear != null)
                onClear();
        }

        public void OnDestroySource(GSSource source)
        {
            --UseCount;
        }
    }
}
