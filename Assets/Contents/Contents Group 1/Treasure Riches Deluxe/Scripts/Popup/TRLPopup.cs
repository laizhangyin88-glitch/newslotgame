using System;
using System.Collections;
using SlotMaker;
using UnityEngine;

namespace GameStudio.Slot.TRL.Popup
{
    public abstract class TRLPopup : MonoBehaviour
    {
        protected Transform originalParent;

        public Coroutine PlayDefaultPopup(Action initalize) => StartCoroutine(_Play(false, initalize));
        public Coroutine PlayCustomPopup(Action initalize) => StartCoroutine(_Play(true, initalize));

        private IEnumerator _Play(bool isCustomPopup, Action initalize = null)
        {
            if (!isCustomPopup)
            {
                originalParent = transform.parent;
                transform.SetParent(PopupManager.Instance.contents);
                PopupManager.Instance.Open(gameObject);
                initalize?.Invoke();
                yield return StartCoroutine(OnPlayCoroutine());

                transform.SetParent(originalParent);
                PopupManager.Instance.Close(gameObject);
            }
            else
            {
                initalize?.Invoke();
                yield return StartCoroutine(OnPlayCoroutine());
            }
        }

        protected abstract IEnumerator OnPlayCoroutine();
    }
}