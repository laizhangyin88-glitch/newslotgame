using System.Collections;
using UnityEngine;

namespace SlotMaker
{
	public class GSNullHandler : IGSHandler 
	{
	    public int UseCount { get { return 0; } }

		public float Volume
		{
			set {}
		}

		public bool Mute 
		{
			get 
			{ 
				return false; 
			}

			set {}
		}

	    public GSSource GetSource() { return null; }
		public void RegisterSource(GSSource source) {}

		public void Play() {}
		public void Play(float delay) {}
		public void Pause() {}
		public void UnPause() {}
		public void Stop() {}
	    public void Clear() {}
	}
}
