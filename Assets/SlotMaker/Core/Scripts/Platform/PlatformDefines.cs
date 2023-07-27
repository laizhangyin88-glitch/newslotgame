namespace SlotMaker
{
	[System.Flags]
	public enum TargetPlatform
	{
	    IOS = (1 << 0),
	    Android = (1 << 1),
	    Standalone = (1 << 2),
	    WebGL = (1 << 3),
        WSA = (1 << 4)
	};
}
