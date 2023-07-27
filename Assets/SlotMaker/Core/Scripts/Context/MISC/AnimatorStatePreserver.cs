using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public class AnimatorStatePreserver : MonoBehaviour
    {
        private AnimationPreserve preserveController;
        private Animator ani;

        private void Awake()
        {
            ani = GetComponent<Animator>();
        }

        private void Start()
        {
            if(ani != null)
            {
                preserveController = new AnimationPreserve();
                preserveController.Init(ani);

                preserveController.SaveState();
            }
        }

        private void OnEnable()
        {
            if(preserveController != null)
            {
                preserveController.Preserve();
            }
        }

        private void OnDisable()
        {
            if(preserveController != null)
            {
                preserveController.SaveState();
                // preserveController.PrintParams();
            }
        }
    }    
}
