using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker
{
	[AddComponentMenu("SlotMaker/Animator/Animator Random Trigger")]
	[RequireComponent(typeof(Animator))]
	public class AnimatorRandomTrigger : MonoBehaviour
	{
		public float minTime;
		public float maxTime;

		[System.Serializable]
		public class TriggerWeight
		{
			public string trigger;
			public int HASH_TRIGGER { get; set; }

			[Range(0f, 1f)]
			public float rate;
		}
		public List<TriggerWeight> value;

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
			foreach (var triggerWeight in value)
			{
				triggerWeight.HASH_TRIGGER = Animator.StringToHash(triggerWeight.trigger);
			}
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

				float rnd = UnityEngine.Random.value;
				for (int i = 0; i < value.Count; ++i)
				{
					if (rnd <= value[i].rate)
					{
						animator.SetTrigger(value[i].HASH_TRIGGER);
						break;
					}
				}
			}
		}
	}
}
