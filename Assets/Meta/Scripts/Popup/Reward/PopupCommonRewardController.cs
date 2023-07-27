using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using NodeCanvas.Framework;
using ParadoxNotion;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class PopupCommonRewardController : MonoBehaviour
    {
        private Blackboard bb;
        private Animator anim;

        private void Awake()
        {
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();
        }

        public void PlayRewardSound(string id)
        {
            bool play = true;
            if (bb != null)
            {
                var playSound = BlackboardUtils.FindVariable<bool>(bb, "playSound");
                if (playSound != null)
                {
                    play = false;
                    if (playSound.value)
                    {
                        GSManager.Instance.GetHandler(id).Play();
                    }
                }
            }

            if (play)
            {
                GSManager.Instance.GetHandler(id).Play();
            }
        }

        public IEnumerator CheckRewardBoost()
        {
            var rewardList = bb.GetValue<List<Blackboard>>("rewardList");
            foreach (var rewardResult in rewardList)
            {
                var rewardType = rewardResult.GetValue<RewardType>("rewardType");
                if (rewardType == RewardType.SPIN_DEAL)
                {
                    // Find Product From Caller
                    var caller = bb.GetVariable<GameObject>("caller")?.value;
                    Blackboard product = null;
                    if (caller != null)
                    {
                        var callerBB = caller.GetComponent<Blackboard>();
                        product = callerBB.GetVariable<Blackboard>("product")?.value;
                    }

                    // No Product
                    if (product == null)
                        yield break;

                    // Is Purchase Result
                    var purchaseResponse = BlackboardUtils.FindVariable<Blackboard>("/purchaseResponse")?.value;
                    if (purchaseResponse == null)
                        yield break;

                    // Match Price
                    double price = product.GetValue<double>("price");
                    var boostProduct = BlackboardQueryUtils.GetPriceMatchProduct(
                        price, ShopType.SPIN_BOOST, ItemType.SPIN_BOOST, out _);

                    // No Boost
                    if (boostProduct == null)
                        yield break;

                    // Disable Back Button
                    MetaObjectUtils.EnableBackButton(false);

                    // Transition Popup
                    yield return StartCoroutine(OpenBoostTransitionCoroutine());

                    // Enable Back Button
                    MetaObjectUtils.EnableBackButton(true);

                    // Boost Popup
                    string asset = "Popup Control Spin Booster Scene";
                    yield return StartCoroutine(OpenBoostPopupCoroutine(asset, product, boostProduct, purchaseResponse));

                    EventSender.SendEvent(gameObject, new EventData("OnTriggerBoost"));

                    yield break;
                }
                else if (rewardType == RewardType.TICKETED_BONUS_TICKET_FOR_BOOST) // Add Ticketed Bonus Booster
                {
                    var caller = bb.GetVariable<GameObject>("caller")?.value;
                    Blackboard product = null;
                    if (caller != null)
                    {
                        var callerBB = caller.GetComponent<Blackboard>();
                        product = callerBB.GetVariable<Blackboard>("product")?.value;

                        // No IAM
                        if (callerBB.GetVariable<int>("iamId") == null)
                            yield break;
                    }

                    // No Product
                    if (product == null)
                        yield break;

                    // Is Purchase Result
                    var purchaseResponse = BlackboardUtils.FindVariable<Blackboard>("/purchaseResponse")?.value;
                    if (purchaseResponse == null)
                        yield break;

                    // Match Price
                    double price = product.GetValue<double>("price");
                    var boostProduct = BlackboardQueryUtils.GetPriceMatchProduct(
                        price, ShopType.TICKETED_BONUS_BOOSTER, ItemType.TICKETED_BONUS_BOOSTER, out _);

                    // No Boost
                    if (boostProduct == null)
                        yield break;

                    // Disable Back Button
                    MetaObjectUtils.EnableBackButton(false);

                    // Transition Popup
                    yield return StartCoroutine(OpenBoostTransitionCoroutine());

                    // Enable Back Button
                    MetaObjectUtils.EnableBackButton(true);

                    // Boost Popup
                    string asset = "Popup Control Ticketed Bonus Booster Scene";
                    yield return StartCoroutine(OpenTicketedBonusBoostPopupCoroutine(asset, product, boostProduct, purchaseResponse));

                    EventSender.SendEvent(gameObject, new EventData("OnTriggerBoost"));

                    yield break;
                }
            }
        }

        public void Close()
        {
            MessageDispatcher.Dispatch(
                MetaEventDefine.ON_META_UI_EVENT,
                new EventData(InboxEvent.ON_COLLECT_COMMON_REWARDS));

            PopupManager.Instance.Close(gameObject);

            var owner = GetComponent<GraphOwner>();
            MetaSystem.UnSubscribeBackButton(owner.GetHashCode());

            anim.SetTrigger("Close");
        }

        //

        private IEnumerator OpenBoostTransitionCoroutine()
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Coin Booster Transition Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject transitionPopupObj = null;
            yield return StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(
                bundle, asset, parent, (GameObject popupObj) => transitionPopupObj = popupObj));

            MetaObjectUtils.SetCalleeCaller(transitionPopupObj, gameObject);

            // Delay
            yield return new WaitForSeconds(0.5f);

            MetaPopupUtils.OpenPopup(transitionPopupObj);

            // Wait Callback
            var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);
        }

        private IEnumerator OpenBoostPopupCoroutine(string asset, Blackboard purchasedProduct, Blackboard boostProduct, Blackboard purchaseResponse)
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject boosterPopupObj = null;
            yield return StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(
                bundle, asset, parent, (GameObject popupObj) => boosterPopupObj = popupObj));

            SetBoostPopupData(boosterPopupObj, false, purchasedProduct, boostProduct, purchaseResponse);
            var caller = bb.GetValue<GameObject>("caller");
            MetaObjectUtils.SetCalleeCaller(boosterPopupObj, caller);

            MetaPopupUtils.OpenPopup(boosterPopupObj);
        }

        private IEnumerator OpenTicketedBonusBoostPopupCoroutine(string asset, Blackboard purchasedProduct, Blackboard boostProduct, Blackboard purchaseResponse)
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject boosterPopupObj = null;
            yield return StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(
                bundle, asset, parent, (GameObject popupObj) => boosterPopupObj = popupObj));

            SetBoostPopupData(boosterPopupObj, true, purchasedProduct, boostProduct, purchaseResponse);
            var caller = bb.GetValue<GameObject>("caller");
            MetaObjectUtils.SetCalleeCaller(boosterPopupObj, caller);

            MetaPopupUtils.OpenPopup(boosterPopupObj);
        }

        private void SetBoostPopupData(GameObject boosterPopupObj, bool isTicketed, Blackboard purchasedProduct, Blackboard boostProduct, Blackboard purchaseResponse)
        {
            var purchasedItem = purchasedProduct.GetValue<List<Blackboard>>("itemList")[0];
            var purchasedReward = purchasedProduct.GetValue<List<Blackboard>>("rewardList")[0];
            var purchaseResponseItem = purchaseResponse.GetValue<List<Blackboard>>("itemUseResultList")[0];
            var purchaseResponseReward = purchaseResponse.GetValue<List<Blackboard>>("rewardResultList")[0];
            int gameId = purchasedReward.GetValue<int>("gameId");

            var popupBB = boosterPopupObj.GetComponent<Blackboard>();

            BlackboardUtils.SetOrCreateValue(popupBB, "purchasedCredit", purchaseResponseItem.GetValue<long>("earnCredit"));
            BlackboardUtils.SetOrCreateValue(popupBB, "gameId", gameId);
            BlackboardUtils.SetOrCreateValue(popupBB, "targetPurchaseID", purchaseResponse.GetValue<int>("purchaseId"));
            BlackboardUtils.SetOrCreateValue(popupBB, "product", boostProduct);
            BlackboardUtils.SetOrCreateValue(popupBB, "isInbox", true);

            if (isTicketed == true)
            {
                long multiBet = IAMUtils.CalculateBetLevelMultiplier(
                    purchasedReward.GetValue<long>("baseBet"),
                    purchasedReward.GetValue<long>("extraBet"),
                    BlackboardQueryUtils.GetRawBaseBet(gameId));

                BlackboardUtils.SetOrCreateValue(popupBB, "_ticketIDList", GetRewardTicketIdList(purchaseResponse));
                BlackboardUtils.SetOrCreateValue(popupBB, "baseBet", multiBet);
                BlackboardUtils.SetOrCreateValue(popupBB, "titleTextKey", GetTicketedBonusTitleKey(purchasedReward));
            }
            else
            {
                long multiBet = LevelUtils.GetLevelMultiplierNumeratorValue(purchaseResponseReward.GetValue<long>("baseBet"), "spinDeal");

                BlackboardUtils.SetOrCreateValue(popupBB, "_spinDealID", purchaseResponseReward.GetValue<int>("spinDealId"));
                BlackboardUtils.SetOrCreateValue(popupBB, "baseBet", multiBet);
            }
        }

        private string GetTicketedBonusTitleKey(Blackboard purchasedReward)
        {
            BonusTag tag = purchasedReward.GetVariable<BonusTag>("tag")?.value ?? BonusTag.UNKNOWN;
            switch (tag)
            {
                case BonusTag.BUY_A_BONUS:
                    return "POPUP_TICKETED_BONUS_BOOSTER_BAB_DEAL_TITLE";
                case BonusTag.SUPER_BONUS:
                    return "POPUP_TICKETED_BONUS_BOOSTER_SPB_DEAL_TITLE";
                case BonusTag.INSTANT_BONUS:
                    return "POPUP_TICKETED_BONUS_BOOSTER_INS_DEAL_TITLE";
            }
            return "POPUP_CONTROL_SPIN_BOOSTER_TITLE";
        }

        private List<int> GetRewardTicketIdList(Blackboard purchaseResponse)
        {
            List<Blackboard> rewardResultList = purchaseResponse.GetValue<List<Blackboard>>("rewardResultList");
            List<int> ticketIdList = new List<int>();
            if (rewardResultList != null && rewardResultList.Count > 0)
            {
                for (int i = 0; i < rewardResultList.Count; ++i)
                {
                    var ticketId = rewardResultList[i].GetVariable<int>("ticketId");
                    if (ticketId != null && ticketId.value != 0)
                        ticketIdList.Add(ticketId.value);
                }
            }
            return ticketIdList;
        }

        // Apply VIP Lounge Reward
        public void SendGlobalVIPLoungeRewardEvent()
        {
            Transform applyTransform = GetApplyVIPLoungeRewardObject()?.transform ?? null;
            var eventData = new EventData<Transform>(MetaEventDefine.ON_VIP_LOUNGE_REWARD_EVENT, applyTransform);
            EventSender.SendGlobalEvent(eventData);
        }

        private GameObject GetApplyVIPLoungeRewardObject()
        {
            ContextElement root = gameObject.GetComponent<ContextElement>();
            GameObject returnObject = ContextUtils.FindElement(root, "Button Collect", ContextSearchingType.ChildrenSearch).gameObject;

            List<Blackboard> rewardList = BlackboardUtils.FindVariable<List<Blackboard>>(bb, "rewardList")?.value ?? null;
            int rewardCount = rewardList?.Count ?? 0;
            
            for (int i = 0; i < rewardCount; ++i)
            {
                RewardType rewardType = BlackboardUtils.FindVariable<RewardType>(rewardList[i], "rewardType").value;
                if (rewardType == RewardType.CREDIT)
                {
                    ContextElement itemElement = ContextUtils.FindElement(root, string.Format("Item {0:00}", i + 1), ContextSearchingType.ChildrenSearch);
                    if (itemElement != null)
                    {
                        returnObject = ContextUtils.FindElement(itemElement, "Text", ContextSearchingType.ChildrenSearch).gameObject;
                        break;
                    }
                }
            }

            return returnObject;
        }

        public void UpdateVIPLoungeRewardCredit()
        {
            long vipLoungeRewardNumerator = BlackboardQueryUtils.GetVIPLoungeClubVegasRewardNumerator();
            List<Blackboard> rewardList = BlackboardUtils.FindVariable<List<Blackboard>>(bb, "rewardList")?.value ?? null;

            ContextElement root = gameObject.GetComponent<ContextElement>();
            for (int i = 0; i < rewardList.Count; ++i)
            {
                RewardType rewardType = BlackboardUtils.FindVariable<RewardType>(rewardList[i], "rewardType").value;
                if (rewardType == RewardType.CREDIT)
                {
                    ContextElement itemElement = ContextUtils.FindElement(root, string.Format("Item {0:00}", i + 1), ContextSearchingType.ChildrenSearch);
                    ContextElement itemTextElement = ContextUtils.FindElement(itemElement, "Text", ContextSearchingType.ChildrenSearch);
                    if (itemTextElement == null || !(itemTextElement is IContextText) || itemTextElement == null)
                        continue;

                    long vipCoins = BlackboardUtils.FindVariable<long>(rewardList[i], "credit").value;
                    long resultCoins = NumberUtils.GetMultiplierNumeratorValue(vipCoins, vipLoungeRewardNumerator);

                    MetaIncreaseNumber increaseNumber = itemElement.GetComponent<MetaIncreaseNumber>();
                    increaseNumber?.Reset(itemTextElement as IContextText, "TEXT_COIN_BONUS", vipCoins, resultCoins, 0.2f, 1, false);
                }
            }
        }
    }
}
