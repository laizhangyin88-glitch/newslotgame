using System;
using UnityEngine;
using System.Collections;

namespace SlotMaker
{
    public class SymbolMotionBlur : MonoBehaviour
    {
        public BaseSymbol symbol;
        public Animator animator;
        public string parameter;
        public float maximumVelocityMagnitude;

        public void Apply()
        {
            float v = symbol.reel.movement.GetVelocity() / maximumVelocityMagnitude;
            if (v < 0) v *= -1f;

            animator.SetFloat(parameter, v);
        }

        public void Update()
        {
            Apply();
        }
    }
}
