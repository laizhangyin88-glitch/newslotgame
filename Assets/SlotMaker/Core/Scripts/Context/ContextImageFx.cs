using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

namespace SlotMaker
{
	public class ContextImageFx : ContextImage, IContextFloatProperty
	{
	    public ImageFx imageFx;

		public void SetFloatProperty(float value)
	    {
	        imageFx.fillAmount = value;
	    }

	    public float GetFloatProperty()
	    {
	        return imageFx.fillAmount;
	    }
	}
}
