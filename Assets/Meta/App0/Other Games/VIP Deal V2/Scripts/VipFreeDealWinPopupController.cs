using System.Collections;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;

namespace BagelCode
{
    public class VipFreeDealWinPopupController : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private Blackboard dealInfo;

        private ContextElement winItemElement;
        private ContextElement redeemButtonElement;
        private ContextElement multiplierElement;

        private Animator multiplierAnim;

        private bool isRedeemSuccess = false;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        public void Init()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            dealInfo = BlackboardUtils.FindValue<Blackboard>(bb, "dealInfo");

            winItemElement = ContextUtils.FindElement(root, "Win Item", CHILDREN);

            long originCredit = BlackboardUtils.FindValue<long>(dealInfo, "creditAmount");
            long baseCredit = VipDealV2.Utils.GetFreeDealCreditMultiplierValue(originCredit);

            long multiplierNumerator = BlackboardUtils.FindValue<long>(dealInfo, "multiplierNumerator");
            long totalCredit = NumberUtils.GetMultiplierNumeratorValue(baseCredit, multiplierNumerator);
            double multiplier = NumberUtils.GetMultiplierFromNumerator(multiplierNumerator);

            bool isMax = BlackboardUtils.FindValue<bool>(bb, "isMax");

            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text Title", "VIP_DEAL_V2_WIN_POPUP_TITLE", CHILDREN);

            multiplierElement = ContextUtils.FindElement(winItemElement, "Multiplier", CHILDREN);
            multiplierAnim = multiplierElement.GetComponent<Animator>();
            MetaContextElementUtils.SimpleSetTextGlobal(
                multiplierElement, "Text", "VIP_DEAL_SHOP_ITEM_MULTIPLIER", CHILDREN, multiplier);

            MetaContextElementUtils.SimpleSetTextGlobal(winItemElement, "Base Credit Text", "VIP_DEAL_SHOP_ITEM_BASE_COIN", CHILDREN, baseCredit);
            MetaContextElementUtils.SimpleSetTextGlobal(winItemElement, "Total Credit Text", "VIP_DEAL_SHOP_ITEM_TOTAL_COIN", CHILDREN, totalCredit);

            redeemButtonElement = ContextUtils.FindElement(winItemElement, "Button Free", CHILDREN);
            MetaContextElementUtils.SetClickable(redeemButtonElement, OnClickRedeem);
            MetaContextElementUtils.SimpleSetTextGlobal(redeemButtonElement, "Text", "VIP_DEAL_V2_WIN_POPUP_REDEEM", CHILDREN);

            multiplierAnim.SetInteger("On", isMax ? 3 : 1);

            Register("OnClose", OnClose); // by animation
        }

        public IEnumerator OnRedeemCoroutine()
        {
            bool isSuccess = false;
            bool isFail = false;
            int vipDealInfoId = VipDealV2.Utils.GetInfoID();
            BagelCodeClientAPI.VipDealV2FreebieRedeem(vipDealInfoId,
                (response) =>
                {
                    isSuccess = true;
                    var selectSceneObj = MetaObjectUtils.GetCaller(gameObject);
                    var selectSceneBB = selectSceneObj.GetComponent<Blackboard>();

                    var responseBB = BlackboardUtils.GetOrCreateBlackboard(selectSceneBB, "redeemResponse");
                    ClientAPI2Blackboard.Serialize(responseBB, response);

                    BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                },
                (error) =>
                {
                    isFail = true;
                    GlobalErrorHandler.GlobalError(error);
                });

            yield return new WaitUntil(() => isSuccess || isFail);

            isRedeemSuccess = isSuccess;

            anim.SetTrigger("Close");
        }

        private void OnClose()
        {
            PopupManager.Instance.Close(gameObject);

            if (isRedeemSuccess)
            {
                EventSender.SendCalleeCallback(gameObject, VipDealV2.Events.ON_FREEBIE_REDEEM_SUCCESS);
            }
            else
            {
                EventSender.SendCalleeCallback(gameObject, VipDealV2.Events.ON_FREEBIE_REDEEM_FAILURE);
            }
        }

        private void OnClickRedeem()
        {
            EventSender.SendEvent(gameObject, VipDealV2.Events.ON_REDEEM);
        }
    }
}
