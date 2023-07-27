using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace SlotMaker
{
	[AddComponentMenu("SlotMaker/Animation/Animation Simple Event Trigger")]
	public class AnimationSimpleEventTrigger : MonoBehaviour 
	{
		public UnityEvent onA;
		public UnityEvent onB;
		public UnityEvent onC;
		public UnityEvent onD;
		public UnityEvent onE;
		public UnityEvent onF;

		public virtual void OnA()
		{
			StartCoroutine(WaitA());
		}

		public virtual void OnB()
		{
			StartCoroutine(WaitB());
		}

		public virtual void OnC()
		{
			StartCoroutine(WaitC());
		}

		public virtual void OnD()
		{
			StartCoroutine(WaitD());
		}

		public virtual void OnE()
		{
			StartCoroutine(WaitE());
		}

		public virtual void OnF()
		{
			StartCoroutine(WaitF());
		}

		protected IEnumerator WaitA()
		{
			yield return new WaitForEndOfFrame();

			if (onA != null)
				onA.Invoke();
		}

		protected IEnumerator WaitB()
		{
			yield return new WaitForEndOfFrame();

			if (onB != null)
				onB.Invoke();
		}

		protected IEnumerator WaitC()
		{
			yield return new WaitForEndOfFrame();

			if (onC != null)
				onC.Invoke();
		}

		protected IEnumerator WaitD()
		{
			yield return new WaitForEndOfFrame();

			if (onD != null)
				onD.Invoke();
		}

		protected IEnumerator WaitE()
		{
			yield return new WaitForEndOfFrame();

			if (onE != null)
				onE.Invoke();
		}

		protected IEnumerator WaitF()
		{
			yield return new WaitForEndOfFrame();

			if (onF != null)
				onF.Invoke();
		}
	}
}