using UnityEngine;
using System.Collections;

namespace SlotMaker
{
	[ExecuteInEditMode]
	[AddComponentMenu("SlotMaker/UI/Sprite Image")]
	[RequireComponent(typeof(SpriteRenderer))]
	public class SpriteImage : MonoBehaviour
	{
		private void OnEnable()
	    {
	        UpdateMaterial();
	    }

	    public void OnTransformParentChanged()
	    {
	        if (gameObject.activeSelf)
	            UpdateMaterial();
	    }

		public virtual void UpdateMaterial()
		{
			var panel = gameObject.GetComponentInParent<SpritePanel>();
	        if (panel != null && panel.dynamicMaterial != null)
			{
				var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
				spriteRenderer.sharedMaterial = panel.dynamicMaterial;
			}
		}
	}
}
