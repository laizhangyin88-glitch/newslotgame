using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
    public class InboxCellDataBossRaidersDeal : InboxCellData
    {
        private GameObject dealLoadingObj;

        private int dealThemeId = 0;

        public InboxCellDataBossRaidersDeal(InboxCellController _owner, Blackboard _inboxInfo)
            : base(_owner, _inboxInfo) { }

        public override IEnumerator OnAcceptSuccessCoroutine()
        {
            Blackboard cellBB = cell.GetComponent<Blackboard>();
            Blackboard response = cellBB.GetValue<Blackboard>("response");

            dealThemeId = response.GetValue<int>("themeId");

            yield return cell.StartCoroutine(MakeCommonBundleLoadingCoroutine());
            // Make Boss Raiders Deal Loading to In Game
            yield return cell.StartCoroutine(BossRaidersDealLoadingCoroutine(response));

            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_ENTER_META_GAME);

            // Boss Raiders Deal Clear
            var callbackTrigger = new EventTrigger(cell, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);

            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_LEAVE_META_GAME);
        }

        public override void OnRemoveInboxItem()
        {
            text = GetText();
            cell.UpdateText(text);
        }

        protected override string GetInternalText()
        {
            string resultText = StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);

            string groupText = StringTableUtils.GetString(StringTable.StringTableType.Global, "INBOX_ITEM_BOSS_RAIDERS_DEAL_GROUP", cell.InboxInfoGroupCount);
            TextDecoUtils.ConvertStringFormat(ref resultText, "{group_count}", groupText);

            return resultText;
        }

        protected override string GetButtonText()
        {
            return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_BUTTON_PLAY2");
        }

        protected override IconType GetIconType()
        {
            return IconType.BOSSRAIDERS_DEAL;
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

            List<string> bundles = new List<string>();
            bundles.Add(BossRaidersUtils.GetBossRaidersSharedBundleName(false));

            var commonLoadingBB = commonLoadingObj.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(commonLoadingBB, "bundle", BossRaidersUtils.GetBossRaidersDealBundleName(dealThemeId, false));
            BlackboardUtils.SetOrCreateValue(commonLoadingBB, "nextScene", "Boss Raiders Deal Loading Scene");
            BlackboardUtils.SetOrCreateValue(commonLoadingBB, "openNextSceneInstantly", false);
            BlackboardUtils.SetOrCreateValue(commonLoadingBB, "bundles", bundles);
            MetaObjectUtils.SetCalleeCaller(commonLoadingObj, cell.gameObject);
            MetaPopupUtils.OpenPopup(commonLoadingObj);

            dealLoadingObj = null;

            // Wait Boss Raiders Deal Load Bundle Loading
            var callbackTrigger = new EventTrigger(cell, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);

            dealLoadingObj = (GameObject)callbackTrigger.EventData.value;
            if (dealLoadingObj == null)
            {
                if (ApplicationSettings.LogTest())
                {
                    Debug.LogError("InboxCellDataBossRaidersDeal.OnAcceptSuccessCoroutine failure. Failed to load \"Boss Raiders Deal Loading Scene\".");
                    yield break;
                }
            }
        }

        private IEnumerator BossRaidersDealLoadingCoroutine(Blackboard response)
        {
            var dealLoadingBB = dealLoadingObj.GetComponent<Blackboard>();
            MetaObjectUtils.SetCalleeCaller(dealLoadingObj, cell.gameObject);

            BlackboardUtils.SetOrCreateValue(dealLoadingBB, "isEnter", true);
            BlackboardUtils.SetOrCreateValue(dealLoadingBB, "_biContextID", BiEventUtils.GenerateContextID());
            BlackboardUtils.SetOrCreateValue(dealLoadingBB, "enterType", "inbox");
            BlackboardUtils.SetOrCreateValue(dealLoadingBB, "themeId", dealThemeId);
            BlackboardUtils.SetOrCreateValue(dealLoadingBB, "dealSpinCount", response.GetValue<int>("spinCount"));
            BlackboardUtils.SetOrCreateValue(dealLoadingBB, "dealUuid", response.GetValue<string>("dealUuid"));

            dealLoadingObj.SetActive(true);

            // Wait Boss Raiders Deal Loading
            var callbackTrigger = new EventTrigger(cell, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);
        }
    }
}