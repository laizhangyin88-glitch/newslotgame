using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
	[RequireComponent(typeof(ParticleSystemRenderer))]
	public class OverrideRelativeParticleSortingLayer : OverrideSortingLayer
	{
	    public int relativeSortingOrder;

	    public override void UpdateSortingLayer()
	    {
	        var renderer = GetComponent<ParticleSystemRenderer>();
	        var rootCanvas = transform.parent._GetComponentInParent<Canvas>();
	        
	        renderer.sortingLayerID = rootCanvas.sortingLayerID;
	        renderer.sortingOrder = rootCanvas.sortingOrder + relativeSortingOrder;
	    }
	}
}
