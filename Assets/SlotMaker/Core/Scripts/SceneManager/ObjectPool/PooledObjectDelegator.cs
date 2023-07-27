using UnityEngine;
using System.Collections;

namespace SlotMaker
{
    public class PooledObjectDelegator : MonoBehaviour
    {
        public PooledObject delegator;

        public void GetObject(ObjectPool pool, Transform anchor)
        {
            ReturnToPool();

            delegator = pool.GetObject();
            delegator.transform.SetParent(anchor, false);
        }

        public void ReturnToPool()
        {
            if (delegator != null)
            {
                delegator.ReturnToPool();
                delegator = null;
            }
        }
    }
}
