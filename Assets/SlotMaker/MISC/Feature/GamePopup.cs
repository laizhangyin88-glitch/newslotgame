using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace GameStudio.Slot
{
    public abstract class GamePopup : MonoBehaviour
    {
        public IEnumerator OpenPopup(Action initalize)
        {
            initalize();
            yield return StartCoroutine(OpenPopup());
        }

        public IEnumerator OpenPopup()
        {
            Open();
            yield return StartCoroutine(OnPlayCoroutine());
            Close();
        }

        private Transform originalParent;

        private void Open()
        {
            originalParent = transform.parent;
            transform.SetParent(PopupManager.Instance.contents);
            PopupManager.Instance.Open(gameObject);
        }

        private void Close()
        {
            transform.SetParent(originalParent);
            PopupManager.Instance.Close(gameObject);
        }

        protected abstract IEnumerator OnPlayCoroutine();
    }
}