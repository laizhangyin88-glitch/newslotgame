using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
using System.Collections.Generic;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class InboxCellDataHogDeal : InboxCellData
    {
        public InboxCellDataHogDeal(InboxCellController _owner, Blackboard _inboxInfo)
            : base(_owner, _inboxInfo) { }

        private GameObject dealLoadingObj;

        public override IEnumerator OnAcceptSuccessCoroutine()
        {
            Blackboard cellBB = cell.GetComponent<Blackboard>();
            Blackboard response = cellBB.GetValue<Blackboard>("response");

            // Clear Hog Deal Prize
            if (isFirstReward)
            {
                BlackboardQueryUtils.ClearHogDealTotalPrizeInfo(cellBB);
            }

            // Load Hog Deal Common Bundle
            // Make Loading Popup
            yield return cell.StartCoroutine(MakeCommonBundleLoadingCoroutine());

            // Make Hog Deal In Game
            yield return cell.StartCoroutine(HogDealInGameCoroutine(response));

            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_ENTER_META_GAME);

            // Wait Hog Deal Clear
            var callbackTrigger = new EventTrigger(cell, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);

            // Check Accept Next
            // callbackEventData: (isAcceptNext, popupObj)
            var callbackEventData = ((bool, GameObject))callbackTrigger.EventData.value;
            if (!callbackEventData.Item1) // Finish Accept
            {
                // Unload hog contents
                yield return cell.StartCoroutine(MakeHogDealUnloadingCoroutine(response));

                MetaPopupUtils.ClosePopup(callbackEventData.Item2);

                // Unload hog common
                yield return cell.StartCoroutine(MakeCommonBundleUnloadingCoroutine());

                // Check Total Prize Count
                int totalPrizeCount = BlackboardQueryUtils.GetHogDealTotalPrizeCount(cellBB);
                if(totalPrizeCount >= 2)
                {
                    // Show Total Result Popup
                    if (ApplicationSettings.LogTest())
                        Debug.Log("Show Total Result");

                    // Make Total Result
                    yield return cell.StartCoroutine(MakeTotalResultPopupCoroutine());
                }
            }
            else
            {
                // close popup self
            }

            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_LEAVE_META_GAME);
        }

        private IEnumerator MakeCommonBundleLoadingCoroutine()
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Bundle Loading Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject commonLoadingObj = null;

            // Make Common Loading
            yield return cell.StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, parent,
                (GameObject popupObj) => commonLoadingObj = popupObj));

            var commonLoadingBB = commonLoadingObj.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(commonLoadingBB, "bundle", HogDeal.Defines.COMMON_BUNDLE);
            BlackboardUtils.SetOrCreateValue(commonLoadingBB, "nextScene", "Hog Deal Loading Scene");
            BlackboardUtils.SetOrCreateValue(commonLoadingBB, "openNextSceneInstantly", false);
            MetaObjectUtils.SetCalleeCaller(commonLoadingObj, cell.gameObject);
            MetaPopupUtils.OpenPopup(commonLoadingObj);

            dealLoadingObj = null;

            // Wait Deal Load Bundle Loading
            var callbackTrigger = new EventTrigger(cell, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);

            dealLoadingObj = (GameObject)callbackTrigger.EventData.value;
            if (dealLoadingObj == null)
            {
                if (ApplicationSettings.LogTest())
                {
                    Debug.LogError("InboxCellDataHogDeal.OnAcceptSuccessCoroutine failure. Failed to load \"Hog Deal Loading Scene\".");
                    yield break;
                }
            }
        }

        private IEnumerator HogDealInGameCoroutine(Blackboard response)
        {
            // Set Loading Popup Data
            var dealLoadingBB = dealLoadingObj.GetComponent<Blackboard>();
            MetaObjectUtils.SetCalleeCaller(dealLoadingObj, cell.gameObject);

            BlackboardUtils.SetOrCreateValue(dealLoadingBB, "isEnter", true);
            BlackboardUtils.SetOrCreateValue(dealLoadingBB, "targetSymbol", response.GetValue<string>("symbol"));
            BlackboardUtils.SetOrCreateValue(dealLoadingBB, "targetStage", response.GetValue<int>("stage"));
            BlackboardUtils.SetOrCreateValue(dealLoadingBB, "remainingCount", cell.InboxInfoGroupCount - 1);

            dealLoadingObj.SetActive(true);

            // Catch OnMakeHogDealInGame
            cell.Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_MAKE_RESULT_POPUP, OnMakeHogDealInGame);

            // Wait Hog Deal Stage Loading
            var callbackTrigger = new EventTrigger(cell, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);

            cell.UnRegister(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_MAKE_RESULT_POPUP);
        }

        private IEnumerator MakeHogDealUnloadingCoroutine(Blackboard response)
        {
            string bundle = HogDeal.Defines.COMMON_BUNDLE;
            string asset = "Hog Deal Loading Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject unloadingObj = null;
            yield return cell.StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                (GameObject popupObj) => unloadingObj = popupObj));

            MetaObjectUtils.SetCalleeCaller(unloadingObj, cell.gameObject);

            var unloadingBB = unloadingObj.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(unloadingBB, "isEnter", false);
            BlackboardUtils.SetOrCreateValue(unloadingBB, "targetSymbol", response.GetValue<string>("symbol"));
            BlackboardUtils.SetOrCreateValue(unloadingBB, "targetStage", response.GetValue<int>("stage"));

            MetaPopupUtils.OpenPopup(unloadingObj);

            // Wait Unload
            var callbackTrigger = new EventTrigger(cell, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);
        }

        private IEnumerator MakeCommonBundleUnloadingCoroutine()
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Bundle Unloading Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject commonUnloadingObj = null;
            yield return cell.StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                (GameObject popupObj) => commonUnloadingObj = popupObj));

            MetaObjectUtils.SetCalleeCaller(commonUnloadingObj, cell.gameObject);

            var commonUnloadingBB = commonUnloadingObj.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(commonUnloadingBB, "bundle", HogDeal.Defines.COMMON_BUNDLE);

            MetaPopupUtils.OpenPopup(commonUnloadingObj);

            // Wait Unload
            var callbackTrigger = new EventTrigger(cell, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);
        }

        private void OnMakeHogDealInGame(EventData eventData)
        {
            var inGameObj = (GameObject)eventData.value;
            var inGameBB = inGameObj.GetComponent<Blackboard>();

            var cellBB = cell.GetComponent<Blackboard>();
            var response = cellBB.GetValue<Blackboard>("response");

            BlackboardUtils.CopyBlackboardVariables(response, inGameBB);

            inGameObj.SetActive(true);
        }

        private IEnumerator MakeTotalResultPopupCoroutine()
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Contents Total Result Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject totalResultObj = null;
            yield return cell.StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                (GameObject popupObj) => totalResultObj = popupObj));

            var totalResultBB = totalResultObj.GetComponent<Blackboard>();

            MetaObjectUtils.SetCalleeCaller(totalResultObj, cell.gameObject);

            var cellBB = cell.GetComponent<Blackboard>();
            var prizeList = cellBB.GetValue<List<long>>("prizeList");
            var jackpotTypeList = cellBB.GetValue<List<HogDealJackpotType>>("jackpotTypeList");

            BlackboardUtils.SetOrCreateValue(totalResultBB, "_isFromHogDeal", true);
            BlackboardUtils.SetOrCreateValue(totalResultBB, "prizeList", prizeList);
            BlackboardUtils.SetOrCreateValue(totalResultBB, "jackpotTypeList", jackpotTypeList);

            MetaPopupUtils.OpenPopup(totalResultObj);

            var callbackTrigger = new EventTrigger(cell, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);
        }

        public override void OnRemoveInboxItem()
        {
            text = GetText();
            cell.UpdateText(text);
        }

        protected override string GetInternalText()
        {
            string resultText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);

            string groupText = StringTableUtils.GetString(StringTable.StringTableType.Global, "INBOX_ITEM_HOG_DEAL_GROUP", cell.InboxInfoGroupCount);
            TextDecoUtils.ConvertStringFormat(ref resultText, "{group_count}", groupText);

            return resultText;
        }

        protected override string GetButtonText()
        {
            return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_BUTTON_PLAY2");
        }

        protected override IconType GetIconType()
        {
            return IconType.HOG_DEAL;
        }
    }
}
