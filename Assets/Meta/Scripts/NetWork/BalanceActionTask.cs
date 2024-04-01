using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode;

namespace PopupCommon
{
        [Category("★ Popup/PopupCommon")]
        public class BalanceAction : ActionTask<Transform>
        {
        //public BBParameter<List<ContextElement>> goldElementList;
        //public BBParameter<List<GemJackpotWinRewardObjectController>> rewardControllerList;
        public BBParameter<string> txtKey;

            protected override string info
            {
                get
                {
                return "Popup Common Ok Scene 001"; //string.Format("Popup Common Ok Scene({0})");
                }
            }

            protected override void OnExecute()
            {
                StartCoroutine(CheckActiveGauge());
            }

            protected IEnumerator CheckActiveGauge()
            {

                Debug.Log($"txtKey = {txtKey.value}");


                MakeCommonOKPopup();

                yield return new WaitForSeconds(1);

                EndAction();
            }


        /*private IEnumerator RetryCoroutine()
        {
            var openRetryTrigger = new EventTrigger(gameObject, "OnOpenRetryPopup");
            var retryTrigger = new EventTrigger(gameObject, "OnRetry");
            var closeTrigger = new EventTrigger(gameObject, "OnClose");


            openRetryTrigger.Reset();
            yield return new WaitUntilTrigger(openRetryTrigger);

            // Make Retry Popup
            MakeCommonOKPopup();

            // Until Retry/Close
            retryTrigger.Reset();
            closeTrigger.Reset();
            yield return new WaitUntilTrigger(retryTrigger, closeTrigger);

            if (retryTrigger.IsTrigger) OnRetry();
            

        }*/



        private void MakeCommonOKPopup()
        {
            string bundle = ApplicationSettings.MakeApplicationBundleName("system");
            string asset = "Popup Common Ok Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent, null);

            string retryText = StringTableUtils.GetString(StringTable.StringTableType.Global, txtKey.value);

            MetaPopupUtils.SetCommonPopupData(popupObj, agent,
                retryText, "", "OnRetry", "OK", "", "", "", "",
                true, true, true, false, false);

            MetaPopupUtils.OpenPopup(popupObj);


            //MetaPopupUtils.SetCommonPopupData(popupObj, agent.transform, errorText, "", "RetryLoadBundles", "OK", "", "", "", "", true, true, true, false, false);
        }

    }
}
