using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace GameStudio.Slot.NDS
{
    public class NDSMotionBlur : MonoBehaviour
    {
        
        public BaseSymbol symbol;
        public Animator animator;
        public string parameter;
        public float maximumVelocityMagnitude;

        public void Apply()
        {
            if (symbol == null) return;
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
