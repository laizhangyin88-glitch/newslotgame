using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace SlotMaker
{
	[AddComponentMenu("SlotMaker/Animation/Simple String Trigger")]
	public class AnimationSimpleStringEventTrigger : MonoBehaviour 
	{
		public UnityStringEvent onA;
		public UnityStringEvent onB;
		public UnityStringEvent onC;
		public UnityStringEvent onD;
		public UnityStringEvent onE;
		public UnityStringEvent onF;

		public virtual void OnAs(string value)
		{
			StartCoroutine(WaitA(value));
		}

		public virtual void OnBs(string value)
		{
			StartCoroutine(WaitB(value));
		}

		public virtual void OnCs(string value)
		{
			StartCoroutine(WaitC(value));
		}

		public virtual void OnDs(string value)
		{
			StartCoroutine(WaitD(value));
		}

		public virtual void OnEs(string value)
		{
			StartCoroutine(WaitE(value));
		}

		public virtual void OnFs(string value)
		{
			StartCoroutine(WaitF(value));
		}

		protected IEnumerator WaitA(string value)
		{
			yield return new WaitForEndOfFrame();

			if (onA != null)
				onA.Invoke(value);
		}

		protected IEnumerator WaitB(string value)
		{
			yield return new WaitForEndOfFrame();

			if (onB != null)
				onB.Invoke(value);
		}

		protected IEnumerator WaitC(string value)
		{
			yield return new WaitForEndOfFrame();

			if (onC != null)
				onC.Invoke(value);
		}

		protected IEnumerator WaitD(string value)
		{
			yield return new WaitForEndOfFrame();

			if (onD != null)
				onD.Invoke(value);
		}

		protected IEnumerator WaitE(string value)
		{
			yield return new WaitForEndOfFrame();

			if (onE != null)
				onE.Invoke(value);
		}

		protected IEnumerator WaitF(string value)
		{
			yield return new WaitForEndOfFrame();

			if (onF != null)
				onF.Invoke(value);
		}
	}
}