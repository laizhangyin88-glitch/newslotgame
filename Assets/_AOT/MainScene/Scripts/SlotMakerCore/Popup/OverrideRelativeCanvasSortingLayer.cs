using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
	[RequireComponent(typeof(Canvas))]
	public class OverrideRelativeCanvasSortingLayer : OverrideSortingLayer
	{
	    public int relativeSortingOrder;

	    public override void UpdateSortingLayer()
	    {
	        var canvas = GetComponent<Canvas>();
	        var rootCanvas = transform.parent._GetComponentInParent<Canvas>();
	        
	        canvas.sortingLayerID = rootCanvas.sortingLayerID;
	        canvas.sortingOrder = rootCanvas.sortingOrder + relativeSortingOrder;
	    }
	}
}
