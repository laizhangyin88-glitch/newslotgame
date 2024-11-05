using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    [ExecuteInEditMode]
    public class DirectionalWeightScaleController : MonoBehaviour
    {
        public Vector3 from;
        public Vector3 to;

        public Vector3 weight;
        public float width;
        public float height;

        private void LateUpdate()
        {
            // if (from == null || to == null)
            //     return;

            Vector3 direction = to - from;
            float magnitude = direction.magnitude;
            direction.Normalize();

            Vector3 left = Vector3.Cross(direction, Vector3.back);

            transform.localScale = from + (direction * magnitude * weight.y) + left * width * weight.x + Vector3.back * height * weight.z;
        }
    }
}
