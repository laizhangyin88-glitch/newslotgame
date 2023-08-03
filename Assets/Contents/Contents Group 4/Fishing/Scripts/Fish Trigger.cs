using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class FishTrigger : MonoBehaviour
    {
        public Action<Collider> onTriggerCallBack;

        protected void OnTriggerEnter(Collider other)
        {
            if (onTriggerCallBack != null)
            {
                onTriggerCallBack(other);
            }
        }
    }
}

