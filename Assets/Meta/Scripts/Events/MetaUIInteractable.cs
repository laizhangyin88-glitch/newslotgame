using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using ParadoxNotion;
using SlotMaker;

namespace BagelCode
{
    public class MetaUIInteractable : MonoBehaviour
    {
        private CanvasGroup canvasGroup;
        private Selectable selectable;
        private UnityAction<bool> SetInteractable;

        private SlotMaker.MessageDelegates metaUIEventDelegates;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                SetInteractable = SetCanvasGroupInteractable;
            }
            else
            {
                selectable = GetComponent<Selectable>();
                if (selectable != null)
                {
                    SetInteractable = SetSelectableInteractable;
                }
                else
                {
                    if (ApplicationSettings.LogTest())
                        Debug.LogWarning(gameObject.name + " need CanvasGroup or Selectable component!");
                }
            }

            metaUIEventDelegates = new SlotMaker.MessageDelegates
            (
                new Dictionary<string, SlotMaker.MessageDispatcher.EventDelegate>
                {
                    { MetaEventDefine.ACTIVE_META_UI,   ActiveMetaUI   },
                    { MetaEventDefine.INACTIVE_META_UI, InActiveMetaUI }
                }
            );
        }

        protected virtual void OnEnable()
        {
            SlotMaker.MessageDispatcher.Register("OnMetaUIEvent", metaUIEventDelegates.Delegate);
        }

        protected virtual void OnDisable()
        {
            SlotMaker.MessageDispatcher.UnRegister("OnMetaUIEvent", metaUIEventDelegates.Delegate);
        }

        public void ActiveMetaUI(EventData eventData)
        {
            SetInteractable(true);
        }

        public void InActiveMetaUI(EventData eventData)
        {
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
