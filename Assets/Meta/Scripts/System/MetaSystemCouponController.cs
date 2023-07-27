using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode
{
    // Meta System 컨플릭 나는 경우 처리하기 쉽도록 nested로.
    // 팝업 안띄워도 쿠폰 수령 가능하게 하는 용도로 팝업과 분리
    public class MetaSystemCouponController : EventMonoSingleton<MetaSystemCouponController>
    {
        private Blackboard bb;

        private const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        public void InitProperty()
        {
            bb = GetComponent<Blackboard>();

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
        }

        public IEnumerator RedeemCoroutine(string code)
        {
            Error errorCode = Error.UNKNOWN;
            if (string.IsNullOrEmpty(code))
            {
                errorCode = Error.NOT_EXIST_COUPON_CODE_ERROR;
                yield break;
            }

            var loadingObj = MetaPopupUtils.OpenLoadingPopup();

            bool success = false;
            BagelCodeClientAPI.CouponRedeem(code,
                (response) =>
                {
                    success = true;

                    BlackboardUtils.DestroyBlackboard(bb, "rewardResultList");
                    BlackboardUtils.SetOrCreateValue(bb, "error", Error.UNKNOWN);

                    MetaBlackboardUtils.SerializeList(
                        bb,
                        response.rewardResultList,
                        ClientAPI2Blackboard.Serialize,
                        "rewardResultList");
                },
                (error) =>
                {
                    errorCode = error.errorCode;
                    BlackboardUtils.SetOrCreateValue(bb, "error", errorCode);
                });

            yield return new WaitUntil(() => success || errorCode != Error.UNKNOWN);

            MetaPopupUtils.ClosePopup(loadingObj);
        }

        public IEnumerator ShowRewardsCoroutine()
        {
            var rewardResultList = BlackboardUtils.FindVariable<List<Blackboard>>(bb, "rewardResultList")?.value;
            var popupObj = MetaCommonRewardUtils.OpenCommonRewardResultPopup(gameObject, rewardResultList, false, false);

            if (popupObj != null)
            {
                var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
                yield return new WaitUntilTrigger(callbackTrigger);
            }

            EventSender.SendGlobalMetaEvent(MetaEventDefine.ON_COUPON_REDEEM_SUCCESS);
        }

        public IEnumerator RedeemFailureCoroutine(Error error)
        {
            string errorMessage;
            switch (error)
            {
                case Error.COUPON_EXPIRED_ERROR:
                    errorMessage = StringTableUtils.GetString(GLOBAL, "POPUP_COUPON_EXPIRED");
                    break;
                case Error.RATE_LIMIT_EXCEEDED_ERROR:
                    errorMessage = StringTableUtils.GetString(GLOBAL, "POPUP_COUPON_EXCESSIVE");
                    break;
                case Error.COUPON_COUNT_EXCEEDED_ERROR:
                    errorMessage = StringTableUtils.GetString(GLOBAL, "POPUP_COUPON_EXCEEDED");
                    break;
                case Error.NOT_EXIST_COUPON_CODE_ERROR:
                    errorMessage = StringTableUtils.GetString(GLOBAL, "POPUP_COUPON_WRONG");
                    break;
                case Error.COUPON_CODE_ALREADY_EXIST_ERROR:
                case Error.ALREADY_USED_COUPON_ERROR:
                    errorMessage = StringTableUtils.GetString(GLOBAL, "POPUP_COUPON_USED");
                    break;
                case Error.NOT_ELIGIBLE_COUPON_ERROR:
                    errorMessage = StringTableUtils.GetString(GLOBAL, "POPUP_COUPON_RESTRICTED");
                    break;
                default:
                    errorMessage = StringTableUtils.GetString(GLOBAL, "POPUP_COUPON_GENERAL");
                    break;
            }

            var popupObj = MetaPopupUtils.OpenOKPopup();
            string buttonText = StringTableUtils.GetString(GLOBAL, "BUTTON_OK");
            MetaPopupUtils.SetCommonOKPopupData(popupObj, transform, errorMessage, buttonText);
            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);

            var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);

            EventSender.SendGlobalMetaEvent(MetaEventDefine.ON_COUPON_REDEEM_FAILURE);
        }
    }
}
