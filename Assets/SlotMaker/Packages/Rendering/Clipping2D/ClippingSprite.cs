using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Rendering.Clipping2D
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class ClippingSprite : ClippingElement
	{
	    public SpriteRenderer spriteRenderer;

	    public override void OnPopulateMaterial(Material sharedMaterial)
	    {
	    	spriteRenderer.sharedMaterial = sharedMaterial;
	    }

		//////////////////////////////////////////////////////////////////////////////////////////
        /// EDITOR
        //////////////////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        private void OnValidate()
        {
        	if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        }
#endif
	}
}