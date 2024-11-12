using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SlotMaker
{
    [ExecuteInEditMode]
    public class PIDButtonInteractableListener : MonoBehaviour
    {
        public UnityBoolEvent onInteractableChanged;
        private PIDButton button;

        private void Start()
        {
            button = GetComponentInParent<PIDButton>();
            if (button != null)
            {
                OnInteractableChanged(button.IsInteractable());
                button.onInteractableChanged.AddListener(OnInteractableChanged);
            }
        }

        private void OnInteractableChanged(bool interactable)
        {
            onInteractableChanged.Invoke(interactable);
        }
    }
}