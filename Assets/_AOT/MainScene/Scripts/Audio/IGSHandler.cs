namespace SlotMaker
{
	public interface IGSHandler
	{
	    int   UseCount { get; }
		float Volume   { set; }
		bool  Mute     { get; set; }

	    GSSource GetSource();
		void RegisterSource(GSSource source);

		void Play();
		void Play(float delay);
		void Pause();
		void UnPause();
		void Stop();
	    void Clear();
	}
}
