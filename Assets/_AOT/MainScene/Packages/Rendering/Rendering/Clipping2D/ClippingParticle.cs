using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Rendering.Clipping2D
{
	[RequireComponent(typeof(ParticleSystemRenderer))]
	public class ClippingParticle : ClippingElement
	{
		public ParticleSystemRenderer particleSystemRenderer;

		public override void OnPopulateMaterial(Material sharedMaterial)
	    {
	    	particleSystemRenderer.sharedMaterial = sharedMaterial;
	    }

		//////////////////////////////////////////////////////////////////////////////////////////
        /// EDITOR
        //////////////////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        private void OnValidate()
        {
        	if (particleSystemRenderer == null) particleSystemRenderer = GetComponent<ParticleSystemRenderer>();
        }
#endif
	}
}