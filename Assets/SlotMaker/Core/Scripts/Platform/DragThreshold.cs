using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SlotMaker
{
	// http://ilkinulas.github.io/programming/unity/2016/03/18/unity_ui_drag_threshold.html
	// https://developer.android.com/guide/practices/screens_support.html
	[RequireComponent(typeof(EventSystem))]
	public class DragThreshold : MonoBehaviour
	{
	    private const float baseDpi = 160f;

	    private void Start()
	    {
	        int defaultThreshold = EventSystem.current.pixelDragThreshold;
	        EventSystem.current.pixelDragThreshold =
	            Mathf.Max(defaultThreshold, (int)(defaultThreshold * Screen.dpi / baseDpi));
	    }
	}
}
