using System.Collections;
using System.Collections.Generic;
using BagelCode.Internal;
using UnityEngine;
using SlotMaker;
using ParadoxNotion;
using Sirenix.OdinInspector;

namespace BagelCode
{
    public class RetryHandler : MonoWeakSingleton<RetryHandler>
    {
        private List<CallRequestContainer> containerList = new List<CallRequestContainer>();

        [Button]
        private void TestRetry()
        {
            OnRetryEvent(new CallRequestContainer());
        }

        [Button]
        private void TestRetryByCoroutine()
        {
            StartCoroutine(TestCoroutine());
        }

        private IEnumerator TestCoroutine()
        {
            OnRetryEvent(new CallRequestContainer());
            yield break;
        }

        private void Start()
        {
            BagelCodeHTTP.retryEventHandler += OnRetryEvent;

            StartCoroutine(RetryCoroutine());

            MessageDispatcher.Register(EventSender.ON_CUSTOM_EVENT, OnCustomEvent);
            MessageDispatcher.Register(MetaEventDefine.ON_SYSTEM_EVENT, OnSystemEvent);
        }

        private void OnCustomEvent(EventData eventData)
        {
            if (eventData.name == "RebootComplete")
            {
                OnSystemReset();
            }
        }

        private void OnSystemEvent(EventData eventData)
        {
            if (eventData.name == MetaEventDefine.SYSTEM_RESET)
            {
                OnSystemReset();
            }
        }

        private IEnumerator RetryCoroutine()
        {
            var openRetryTrigger = new EventTrigger(gameObject, "OnOpenRetryPopup");
            var retryTrigger = new EventTrigger(gameObject, "OnRetry");
            var closeTrigger = new EventTrigger(gameObject, "OnClose");

            while (true)
            {
                openRetryTrigger.Reset();
                yield return new WaitUntilTrigger(openRetryTrigger);

                // Make Retry Popup
                MakeCommonOKPopup();

                // Until Retry/Close
                retryTrigger.Reset();
                closeTrigger.Reset();
                yield return new WaitUntilTrigger(retryTrigger, closeTrigger);

                if (retryTrigger.IsTrigger) OnRetry();
            }
        }

        private void MakeCommonOKPopup()
        {
            string bundle = ApplicationSettings.MakeApplicationBundleName("system");
            string asset = "Popup Common Ok Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent, null);

            string retryText = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_RETRY");

            MetaPopupUtils.SetCommonPopupData(popupObj, transform,
                retryText, "", "OnRetry", "OK", "", "", "", "",
                true, true, true, false, false);

            MetaPopupUtils.OpenPopup(popupObj);
        }

        protected override void OnDestroy()
        {
            BagelCodeHTTP.retryEventHandler -= OnRetryEvent;

            base.OnDestroy();
        }

        public void OnRetryEvent(CallRequestContainer reqContainer)
        {
            containerList.Add(reqContainer);

            EventSender.SendEvent(gameObject, "OnOpenRetryPopup");
        }

        private void OnRetry()
        {
            for (int i = 0; i < containerList.Count; ++i)
            {
                BagelCodeHTTP.RetryRequest(containerList[i]);
            }

            containerList.Clear();
        }

        private void OnSystemReset()
        {
            containerList.Clear();
        }
    }
}
