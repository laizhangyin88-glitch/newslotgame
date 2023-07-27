using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
	public class ReelStopSpringEffect : MonoBehaviour 
	{
		private Vector3 target;
		private IEnumerator coroutine;
		public RectTransform rectTransform;

		public void OnBeginSpring(BaseReel reel)
		{
			target = rectTransform.anchoredPosition3D;

			coroutine = StopEffectCoroutine(reel);
			StartCoroutine(coroutine);
		}

		public void OnStopSpring(BaseReel reel)
		{
			StopCoroutine(coroutine);
			coroutine = null;

			rectTransform.anchoredPosition3D = target;
		}

		private IEnumerator StopEffectCoroutine(BaseReel reel)
		{
			DefaultReelMovement reelMovement = (DefaultReelMovement)(reel.movement);

			while(enabled)
			{
				Vector3 movement = target;
				movement.y = movement.y - reelMovement.displacement.y;

				rectTransform.anchoredPosition3D = movement;
				yield return null;
			}
			rectTransform.anchoredPosition3D = target;
		}
	}
}