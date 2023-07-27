using UnityEngine;
using UnityEditor;
using System.Collections;

namespace SlotMaker
{
	public class PlayerPrefsMenu
	{
		[MenuItem("SlotMaker/Etc/ClearPlayerPrefs", false, 100)]
		public static void ClearPlayerPrefs()
		{
			PlayerPrefs.DeleteAll();
		}

	    [MenuItem("SlotMaker/Etc/ClearCache", false, 101)]
	    public static void ClearCache()
	    {
	        Caching.ClearCache();
	        WebImageDownloader.ClearWebImages();
	    }
	}
}
