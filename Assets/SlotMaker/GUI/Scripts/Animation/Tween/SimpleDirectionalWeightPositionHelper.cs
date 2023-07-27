using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    // todo
    [RequireComponent(typeof(DirectionalWeightPositionController))]
    public class SimpleDirectionalWeightPositionHelper : MonoBehaviour
    {
        public DirectionalWeightPositionController controller;

        public float time;

        private void LateUpdate()
        {
            if (controller.from == null || controller.to == null
             || (controller.weight.y >= 1f)) return;

            Vector3 diffWeight = Vector3.one / time;
            controller.weight += diffWeight * Time.deltaTime;

            // Vector3 diffWeight = Vector3.one - controller.weight;
            // Debug.Log(diffWeight.x);
            // controller.weight += diffWeight * Time.deltaTime / time;
        }
    }
}
