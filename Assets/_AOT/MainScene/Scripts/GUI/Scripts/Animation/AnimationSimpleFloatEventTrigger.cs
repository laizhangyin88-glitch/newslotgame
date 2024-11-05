using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace SlotMaker
{
	[AddComponentMenu("SlotMaker/Animation/Simple Float Trigger")]
	public class AnimationSimpleFloatEventTrigger : MonoBehaviour 
	{
		public UnityFloatEvent onA;
		public UnityFloatEvent onB;
		public UnityFloatEvent onC;
		public UnityFloatEvent onD;
		public UnityFloatEvent onE;
		public UnityFloatEvent onF;

		public virtual void OnAf(float value)
		{
			StartCoroutine(WaitA(value));
		}

		public virtual void OnBf(float value)
		{
			StartCoroutine(WaitB(value));
		}

		public virtual void OnCf(float value)
		{
			StartCoroutine(WaitC(value));
		}

		public virtual void OnDf(float value)
		{
			StartCoroutine(WaitD(value));
		}

		public virtual void OnEf(float value)
		{
			StartCoroutine(WaitE(value));
		}

		public virtual void OnFf(float value)
		{
			StartCoroutine(WaitF(value));
		}

		protected IEnumerator WaitA(float value)
		{
			yield return new WaitForEndOfFrame();

			if (onA != null)
				onA.Invoke(value);
		}

		protected IEnumerator WaitB(float value)
		{
			yield return new WaitForEndOfFrame();

			if (onB != null)
				onB.Invoke(value);
		}

		protected IEnumerator WaitC(float value)
		{
			yield return new WaitForEndOfFrame();

			if (onC != null)
				onC.Invoke(value);
		}

		protected IEnumerator WaitD(float value)
		{
			yield return new WaitForEndOfFrame();

			if (onD != null)
				onD.Invoke(value);
		}

		protected IEnumerator WaitE(float value)
		{
			yield return new WaitForEndOfFrame();

			if (onE != null)
				onE.Invoke(value);
		}

		protected IEnumerator WaitF(float value)
		{
			yield return new WaitForEndOfFrame();

			if (onF != null)
				onF.Invoke(value);
		}
	}
}