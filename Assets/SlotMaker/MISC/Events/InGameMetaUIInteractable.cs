using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NodeCanvas.Framework;

namespace SlotMaker
{
    public class InGameMetaUIInteractable : MonoBehaviour
    {
        private Variable<bool> autoSpin;
        public bool AutoSpin { get { return autoSpin != null && autoSpin.value; } }

        private UnityEngine.CanvasGroup canvasGroup;
        private Selectable selectable;
        private UnityAction<bool> SetInteractable;

        private bool readOnly = false;

        private void Awake()
        {
            autoSpin = BlackboardUtils.FindVariable<bool>("./autoSpin");

            canvasGroup = GetComponent<UnityEngine.CanvasGroup>();
            if (canvasGroup != null)
            {
                SetInteractable = SetCanvasGroupInteractable;
            }
            else 
            {
                selectable = GetComponent<Selectable>();
                SetInteractable = SetSelectableInteractable;    
            }

            var behaviour = GetComponent<InGameBehaviour>();
            behaviour.onSystemReset.AddListener(Finalized);
            behaviour.onEnterGame.AddListener(Initialize);
            behaviour.onExitGame.AddListener(Finalized);
            behaviour.onLeaveGame.AddListener(Finalized);
            behaviour.onReadyGame.AddListener(ActiveMetaUI);
            behaviour.onExitTurn.AddListener(ExitTurn);
            behaviour.onFailSpin.AddListener(ActiveMetaUI);
            behaviour.onSpinButton.AddListener(InActiveMetaUI);
        }

        public void Initialize()
        {
            InActiveMetaUI();
        }

        public void Finalized()
        {
            InActiveMetaUI();
            readOnly = true;
        }

        public void ExitTurn()
        {
            if (!AutoSpin)
                ActiveMetaUI();
        }
        
        public void AwakeMetaUI()
        {
            readOnly = false;
            ActiveMetaUI();
        }

        public void ActiveMetaUI()
        {
            if (!readOnly)
                SetInteractable(true);
        }

        public void InActiveMetaUI()
        {
            if (!readOnly)
                SetInteractable(false);
        }

        private void SetCanvasGroupInteractable(bool interactable)
        {
            canvasGroup.interactable = interactable;
        }

        private void SetSelectableInteractable(bool interactable)
        {
            selectable.interactable = interactable;
        }
    }
}
