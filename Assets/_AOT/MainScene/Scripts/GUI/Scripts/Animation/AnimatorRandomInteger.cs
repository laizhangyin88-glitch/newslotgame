using UnityEngine;
using System.Collections;

namespace SlotMaker
{
	[AddComponentMenu("SlotMaker/Animator/Animator Random Integer")]
	[RequireComponent(typeof(Animator))]
	public class AnimatorRandomInteger : MonoBehaviour
	{
		public float  minTime;
		public float  maxTime;
		public string key;
		public int    minValue;
		public int    maxValue;
		public string subKey;

		private int HASH_KEY;
		private int HASH_SUB_KEY;

		private Animator _animator;
		private Animator animator
		{
			get
			{
				if (_animator == null)
					_animator = GetComponent<Animator>();
				return _animator;
			}
		}

		private void Awake()
		{
			HASH_KEY = Animator.StringToHash(key);
			HASH_SUB_KEY = (!string.IsNullOrEmpty(subKey)) ? Animator.StringToHash(subKey) : 0;
		}

		private void OnEnable()
		{
			StartCoroutine("UpdateCoroutine");
		}

		private void OnDisable()
		{
			StopCoroutine("UpdateCoroutine");
		}

		private IEnumerator UpdateCoroutine()
		{
			while (true)
			{
				float time = UnityEngine.Random.Range(minTime, maxTime);
				yield return new WaitForSeconds(time);

				int value = UnityEngine.Random.Range(minValue, maxValue);
				animator.SetInteger(HASH_KEY, value);
				if (HASH_SUB_KEY != 0)
					animator.SetTrigger(HASH_SUB_KEY);
			}
		}
	}
}
