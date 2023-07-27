using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;

namespace BagelCode
{
    public class PopupWaitPendingPurchaseController : EventMonoBehaviour
    {
        public float commaChangeDelay = 1f;
        private ContextElement root;

        private bool isInit = false;

        private const string PENDING_MESSAGE_KEY = "POPUP_PURCHASE_PENDING_MESSAGE_TEXT";

        private List<string> textList = new List<string>();
        private ContextElement textElement = null;
        private int textIndex = 0;

        private System.Action cancelCallback = null;

        private void Start()
        {
            InitProperty();
            StartCoroutine(LoadingTextCoroutine());
        }

        protected override void OnDisable()
        {
            StopAllCoroutines();
            base.OnDisable();
        }

        private void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();

            root.UpdateContext(false);

            textElement = ContextUtils.FindElement(root, "Text", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SimpleSetClickable(root, "Button Close", () => OnCancel() );

            string text = StringTableUtils.GetString(StringTable.StringTableType.Global, PENDING_MESSAGE_KEY);
            textList.Add(text + ".");
            textList.Add(text + "..");
            textList.Add(text + "...");

            UpdateText();

            PopupManager.Instance.Open(gameObject);

            isInit = true;
        }

        private IEnumerator LoadingTextCoroutine()
        {
            while(true)
            {
                yield return new WaitForSeconds(commaChangeDelay);
                UpdateText();
            }
        }

        private void UpdateText()
        {
            if(textList.Count <= textIndex) return;
            MetaContextElementUtils.SetText(textElement, textList[textIndex]);
            ++textIndex;
            textIndex%=textList.Count;
        }

        public void SetCalcenCallback(System.Action callback)
        {
            cancelCallback = callback;
        }

        private void OnCancel()
        {
            if(cancelCallback != null)
                cancelCallback();

            EventSender.SendCalleeCallback(gameObject);
            OnClose();
        }

        public void OnClose()
        {
            cancelCallback = null;

            StopAllCoroutines();
            PopupManager.Instance.Close(gameObject);
            Destroy(gameObject);
        }

        private void SendEvent(string eventName)
        {
            if(gameObject == null) return;
            EventSender.SendEvent(gameObject, eventName);
        }
    }
}
