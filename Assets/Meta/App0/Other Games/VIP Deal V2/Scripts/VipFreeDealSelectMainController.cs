using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using System.Linq;
using ParadoxNotion;

namespace BagelCode
{
    public class VipFreeDealSelectMainController : VipDealV2SelectMainControllerBase
    {
        private const float FREE_EFFECT_TOTAL_SEC = 4f;
        private const float FREE_EFFECT_FIX_DELAY_SEC = 1f;
        private const float FREE_EFFECT_MIN_DELAY = 0.05f;
        private const float FREE_EFFECT_MAX_DELAY = 0.7f;

        private List<Animator> selectCellAnimList = new List<Animator>();

        private Blackboard targetFreeDealInfo = null;

        public override void Init()
        {
            base.Init();
            isPayDeal = false;
        }

        protected override IEnumerator RequestSelectItemCoroutine()
        {
            var loadingObj = MetaPopupUtils.OpenLoadingPopup();

            int vipDealId = BlackboardUtils.FindValue<int>(vipInfo, "vipDealInfoId");
            bool isDone = false;
            BagelCodeClientAPI.VipFreeDealSelectComplete(vipDealId,
            (response) =>
            {
                VipDealV2.Utils.SetIsViewed(true, isPayDeal);
                isDone = true;
            },
            (error) =>
            {
                GlobalErrorHandler.GlobalError(error);
                isDone = true;
            });

            yield return new WaitUntil(() => isDone);

            MetaPopupUtils.ClosePopup(loadingObj);
        }

        public override IEnumerator OnSelectCoroutine(int index)
        {
            if (dealSelectCount.value == 0) // first
            {
                yield return StartCoroutine(RequestSelectItemCoroutine());

                bool isViewed = VipDealV2.Utils.GetIsViewed(isPayDeal);
                if (!isViewed) yield break; // request failed
            }

            var currentDealInfo = dealInfoList[dealSelectCount.value];
            dealSelectCount.value += 1;

            targetItemCellObj = itemCellObjectList[index];
            selectCellObjectList.Add(targetItemCellObj);

            long dealMultiplierNumerator = BlackboardUtils.FindValue<long>(currentDealInfo, "multiplierNumerator");
            bool isMax = dealMultiplierNumerator == wheelMultiplierNumeratorList.Max();

            var cellBB = targetItemCellObj.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(cellBB, "dealInfo", currentDealInfo);
            BlackboardUtils.SetOrCreateValue(cellBB, "isMax", isMax);

            var eventData = new EventData<int>(VipDealV2.Events.ON_OPEN_CARD, index);
            EventSender.SendGlobalEvent(eventData);
        }

        public override void Close()
        {
            var vipDealInfo = VipDealV2.Utils.GetInfo();
            BlackboardUtils.SetOrCreateValue(vipDealInfo, "isLeaveFreeDeal", true);
            EventSender.SendGlobalEvent(MetaEventDefine.ON_HOME_BUTTON);
            anim.SetTrigger("Close");
        }

        public IEnumerator FreeEffectTransitionCoroutine()
        {
            EventSender.SendGlobalMetaEvent(MetaEventDefine.ON_DISABLE_BACK_BUTTON);

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Bonus Transition Events Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);

            string titleText = StringTableUtils.GetString(GLOBAL, "POPUP_VIP_DEAL_BONUS_INTRO");
            var popupBB = popupObj.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(popupBB, "title", titleText);
            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
            PopupManager.Instance.Open(popupObj);

            GSManager.Instance.GetHandler(VipDealV2.Defines.SOUND_TRANSITION).Play();

            yield return new WaitForSeconds(0.5f);

            popupObj.SetActive(true);

            var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);

            yield return new WaitForSeconds(2f);
        }

        public IEnumerator FreeEffectDirectionCoroutine()
        {
            // 선택 순서가 정렬된 순서가 아닐 수 있음
            selectCellObjectList.OrderBy(o =>
            {
                var cellBB = o.GetComponent<Blackboard>();
                return BlackboardUtils.FindValue<int>(cellBB, "index");
            });

            // Find Free Deal
            int freeBonusIndex = selectCellObjectList.FindIndex(o =>
            {
                var cellBB = o.GetComponent<Blackboard>();
                var cellDealInfo = BlackboardUtils.FindValue<Blackboard>(cellBB, "dealInfo");
                if (BlackboardUtils.FindValue<bool>(cellDealInfo, "isFree"))
                {
                    targetFreeDealInfo = cellDealInfo;
                    return true;
                }
                else
                {
                    return false;
                }
            });

            foreach (var selectCellObj in selectCellObjectList)
                selectCellAnimList.Add(selectCellObj.GetComponent<Animator>());

            int index = 0;
            float elapsed = 0f;
            Animator currentCellAnim = selectCellAnimList[0];
            while (elapsed < FREE_EFFECT_TOTAL_SEC || index != freeBonusIndex)
            {
                currentCellAnim.SetBool("FreeSelectOn", true);

                // FREE_EFFECT_FIX_DELAY_SEC 동안 FREE_EFFECT_MIN_DELAY 간격으로 swap
                // 이후 FREE_EFFECT_MAX_DELAY 까지 간격 증가 (lerp)
                float t = Mathf.Max((elapsed - FREE_EFFECT_FIX_DELAY_SEC) / (FREE_EFFECT_TOTAL_SEC - FREE_EFFECT_FIX_DELAY_SEC), 0f);
                float wait = Mathf.Lerp(FREE_EFFECT_MIN_DELAY, FREE_EFFECT_MAX_DELAY, t);

                yield return new WaitForSeconds(wait);
                elapsed += wait;

                currentCellAnim.SetBool("FreeSelectOn", false);

                ++index;
                currentCellAnim = selectCellAnimList.CircularIndexing(ref index);
            }

            yield return new WaitForSeconds(1f);

            var freeCellObj = selectCellObjectList[freeBonusIndex];
            EventSender.SendEvent(freeCellObj, VipDealV2.Events.ON_CHANGE_FREE_START);
            EventSender.SendGlobalMetaEvent(MetaEventDefine.ON_ENABLE_BACK_BUTTON);

            var eventTrigger = new EventTrigger(gameObject, VipDealV2.Events.ON_SET_FREE_COMPLETE);
            yield return new WaitUntilTrigger(eventTrigger);
        }

        public void OpenWinItemPopup()
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "VIP Free Deal Win Popup Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);

            long dealMultiplierNumerator = BlackboardUtils.FindValue<long>(targetFreeDealInfo, "multiplierNumerator");
            bool isMax = dealMultiplierNumerator == wheelMultiplierNumeratorList.Max();

            var popupBB = popupObj.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(popupBB, "dealInfo", targetFreeDealInfo);
            BlackboardUtils.SetOrCreateValue(popupBB, "isMax", isMax);
            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
            MetaPopupUtils.OpenPopup(popupObj);
        }

        public IEnumerator OpenCommonRewardPopupCoroutine()
        {
            var rewardResult = BlackboardUtils.FindVariable<Blackboard>(bb, "redeemResponse/rewardResult")?.value;
            if (rewardResult != null)
            {
                var rewardList = new List<Blackboard>() { rewardResult };
                MetaCommonRewardUtils.OpenCommonRewardResultPopup(gameObject, rewardList, false);

                var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
                yield return new WaitUntilTrigger(callbackTrigger);
            }

            yield return new WaitForSeconds(0.2f); // delay
        }
    }
}
