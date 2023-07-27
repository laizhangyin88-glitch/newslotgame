using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    [ExecuteInEditMode]
    public class DirectionalWeightPositionController : MonoBehaviour, IFromToTransformPair
    {
        public Transform from;
        public Transform to;

        public Vector3 weight;
        public float width;
        public float height;

        public Transform From { get { return from; } }
        public Transform To { get { return to; } }

        private void LateUpdate()
        {
            if (from == null || to == null)
                return;

            Vector3 direction = to.position - from.position;
            float magnitude = direction.magnitude;
            direction.Normalize();

            Vector3 left = Vector3.Cross(direction, Vector3.back);

            transform.position = from.position + (direction * magnitude * weight.y) + left * width * weight.x + Vector3.back * height * weight.z;
        }
    }
}
