using System;
using System.Collections;
using UnityEngine;

namespace SlotMaker
{
	[Serializable]
	public class GameSound
	{
		public string id;

		private GSSource _source;
	    private GSSource source
	    {
	        get
	        {
	            if (_source == null || !_source.gameObject.activeSelf)
	                _source = GSManager.Instance.GetHandler(id).GetSource();
	            return _source;
	        }
	    }

		public bool IsPlaying
		{
			get
			{
				if (source == null) return false;

				return source.IsPlaying;
			}
		}

		public bool IsFading
		{
			get
			{
				if (source == null) return false;

				return source.IsFading;
			}
		}

	    public float Volume
		{
	        get { return source.Volume; }
			set { source.Volume = value; }
		}

		public bool Mute
		{
	        get { return source.Mute; }
			set { source.Mute = value; }
		}

		public void Play()
		{
			if (source == null) return;

			source.Play();
		}

		public void Play(float delay)
		{
			source.Play(delay);
		}

		public void Stop()
		{
			if (source == null) return;

			source.Stop();
		}

		public void Pause()
		{
	        source.Pause();
		}

		public void UnPause()
		{
	        source.UnPause();
		}

	    public void Clear()
	    {
			if (source == null) return;
			
	        source.Clear();
	    }
	}
}
