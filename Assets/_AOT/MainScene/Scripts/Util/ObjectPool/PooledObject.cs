using UnityEngine;
using System.Collections;

namespace SlotMaker
{
	[AddComponentMenu("SlotMaker/GameObject/Object Pool/Pooled Object")]
	public class PooledObject : MonoBehaviour, IPooledObject
	{
		public ObjectPool pool;

		public virtual void ReturnToPool()
		{
			if (pool)
			{
				pool.AddObject(this);
			}
			else
			{
				Debug.LogWarning("PooledObject has not pool.");
				Destroy(gameObject);
			}
		}

		public virtual void WaitAndReturnToPool(float time)
		{
			Invoke("ReturnToPool", time);
		}
	}
}