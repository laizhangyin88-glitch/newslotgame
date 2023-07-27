using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace SlotMaker
{
	[AddComponentMenu("SlotMaker/Animation/Simple Int Trigger")]
	public class AnimationSimpleIntEventTrigger : MonoBehaviour 
	{
		public UnityIntEvent onA;
		public UnityIntEvent onB;
		public UnityIntEvent onC;
		public UnityIntEvent onD;
		public UnityIntEvent onE;
		public UnityIntEvent onF;

		public virtual void OnAi(int value)
		{
			StartCoroutine(WaitA(value));
		}

		public virtual void OnBi(int value)
		{
			StartCoroutine(WaitB(value));
		}

		public virtual void OnCi(int value)
		{
			StartCoroutine(WaitC(value));
		}

		public virtual void OnDi(int value)
		{
			StartCoroutine(WaitD(value));
		}

		public virtual void OnEi(int value)
		{
			StartCoroutine(WaitE(value));
		}

		public virtual void OnFi(int value)
		{
			StartCoroutine(WaitF(value));
		}

		protected IEnumerator WaitA(int value)
		{
			yield return new WaitForEndOfFrame();

			if (onA != null)
				onA.Invoke(value);
		}

		protected IEnumerator WaitB(int value)
		{
			yield return new WaitForEndOfFrame();

			if (onB != null)
				onB.Invoke(value);
		}

		protected IEnumerator WaitC(int value)
		{
			yield return new WaitForEndOfFrame();

			if (onC != null)
				onC.Invoke(value);
		}

		protected IEnumerator WaitD(int value)
		{
			yield return new WaitForEndOfFrame();

			if (onD != null)
				onD.Invoke(value);
		}

		protected IEnumerator WaitE(int value)
		{
			yield return new WaitForEndOfFrame();

			if (onE != null)
				onE.Invoke(value);
		}

		protected IEnumerator WaitF(int value)
		{
			yield return new WaitForEndOfFrame();

			if (onF != null)
				onF.Invoke(value);
		}
	}
}