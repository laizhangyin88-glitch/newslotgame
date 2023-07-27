using System;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using SlotMaker;
using frame8.Logic.Misc.Other.Extensions;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class UpdateCollectingGame : ActionTask<ContextElement>
    {
        public BBParameter<int> currentScratcherId;
        public BBParameter<bool> isLoaded;
        public BBParameter<string> bundle;
        public BBParameter<string> sharedBundle;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;
        private ContextElement baseElement;
        private Blackboard agentBB;

        protected override string info
        {
            get { return String.Format("Update Collecting Game Scratcher Info, Id = {0}", currentScratcherId); }
        }

        protected override void OnExecute()
        {
            agentBB = agent.gameObject.GetComponent<Blackboard>();

            Blackboard currentScratcher = BlackboardQueryUtils.GetScratcher(currentScratcherId.value);
            List<Blackboard> itemList = currentScratcher.GetValue<List<Blackboard>>("pieceList");

            bool gameClosed = false;

            baseElement = ContextUtils.FindElement(agent, "Base", ContextSearchingType.ChildrenSearch);

            for (int i = 0; i < itemList.Count; i++)
            {
                Blackboard itemInfo = itemList[i];
                ContextElement scratcherItemElement = ContextUtils.FindElement(baseElement, String.Format("Scratcher Item 0{0}", i + 1), ContextSearchingType.ChildrenSearch);

                ContextElement itemAreaElement = ContextUtils.FindElement(scratcherItemElement, "Item Area", ContextSearchingType.ChildrenSearch);

                if (itemAreaElement.ChildCount > 0)
                {
                    foreach (Transform child in itemAreaElement.transform.GetChildren())
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                }

                MetaObjectUtils.MakePrefab(bundle.value, "Scratcher Item", itemAreaElement.transform);
                itemAreaElement.UpdateContext(true);

                ContextElement starElement = ContextUtils.FindElement(itemAreaElement, "Star", ContextSearchingType.ChildrenSearch);

                var rarity = itemInfo.GetValue<ScratcherPieceRarity>("rarity");
                MetaContextElementUtils.SetSprite(starElement, CollectingGameCustomData.Instance.collectingGameAssets.starAssets[(int)rarity - 1]);

                ContextElement ItemImageElement = ContextUtils.FindElement(itemAreaElement, "Image", ContextSearchingType.ChildrenSearch);

                int itemId = itemInfo.GetValue<int>("pieceId");
                Sprite itemAsset = BlackboardQueryUtils.GetAssetOfItem(itemId);
                MetaContextElementUtils.SetSprite(ItemImageElement, itemAsset);

                ContextElement amountTextElement = ContextUtils.FindElement(scratcherItemElement, "Text", ContextSearchingType.ChildrenSearch);
                int possessions = itemInfo.GetValue<int>("possessions");
                int requirement = itemInfo.GetValue<int>("requirement");

                string amountText;
                if (possessions < requirement)
                {
                    amountText = StringTableUtils.GetString(tableType, "COLLECTING_GAME_PIECE_AMOUNT_GRAY_TEXT", possessions, requirement);
                    gameClosed = true;
                }
                else
                {
                    amountText = StringTableUtils.GetString(tableType, "COLLECTING_GAME_PIECE_AMOUNT_GREEN_TEXT", possessions, requirement);
                }

                MetaContextElementUtils.SetText(amountTextElement, amountText);

                ContextElement pieceCoverElement = ContextUtils.FindElement(itemAreaElement, "Cover", ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SetActive(pieceCoverElement, possessions == 0);

                ContextElement pieceBadgeAreaElement = ContextUtils.FindElement(scratcherItemElement, "Badge Area", ContextSearchingType.ChildrenSearch);
                bool pieceNewExist = BlackboardQueryUtils.GetPieceNewBadge(itemId).GetValue<bool>("new");
                if (pieceNewExist && pieceBadgeAreaElement.transform.childCount == 0)
                {
                    MetaObjectUtils.UpdateBadge(pieceBadgeAreaElement, true);
                }
                else if (!pieceNewExist && pieceBadgeAreaElement.transform.childCount > 0)
                {
                    GameObject.Destroy(pieceBadgeAreaElement.transform.GetChild(0).gameObject);
                }
            }

            baseElement.GetComponent<Animator>().SetBool("GameClosed", gameClosed);

            UpdateScratchButton(gameClosed);
            UpdateScratcherImage();
            UpdateLeftTabArea();
            UpdateBottomTabArea();
            UpdateRequestButton();
            UpdateFreeChest();
            BlackboardQueryUtils.InitScratcherNewBadgeInfo(currentScratcherId.value);

            EndAction();
        }

        private void UpdateLeftTabArea()
        {
            int scratcherCount = BlackboardQueryUtils.GetScratcherCount();
            int currentIndex = BlackboardQueryUtils.GetIndexOfScratcher(currentScratcherId.value);

            for (int i = 0; i < scratcherCount; i++)
            {
                ContextElement tabScratcherElement = ContextUtils.FindElement(agent, String.Format("Tab Base Area/Tab Scratcher {0}", i + 1), ContextSearchingType.FullNameSearch);

                if (i == currentIndex)
                    tabScratcherElement.GetComponent<Animator>().SetBool("Active", true);
                else
                    tabScratcherElement.GetComponent<Animator>().SetBool("Active", false);


                ContextElement tabScratcherBadgeAreaElement = ContextUtils.FindElement(tabScratcherElement, "Badge Area", ContextSearchingType.ChildrenSearch);

                Blackboard bottomScratcher = BlackboardQueryUtils.GetScratcherOfIndex(i);
                bool newExist = BlackboardQueryUtils.CheckNewExistInScratcher(bottomScratcher.GetValue<int>("scratcherId"));

                int makableScratcherCount = BlackboardQueryUtils.GetMakableScratcherCount(bottomScratcher.GetValue<int>("scratcherId"));

                if ((i != currentIndex && newExist) || (makableScratcherCount != Int32.MaxValue && makableScratcherCount > 0))
                {
                    MetaContextElementUtils.SetActive(tabScratcherBadgeAreaElement, true);

                    if (i != currentIndex && newExist)
                        MetaObjectUtils.UpdateBadge(tabScratcherBadgeAreaElement, true);
                    else
                        MetaObjectUtils.UpdateBadge(tabScratcherBadgeAreaElement, false, makableScratcherCount);
                }
                else
                {
                    MetaContextElementUtils.SetActive(tabScratcherBadgeAreaElement, false);
                }
            }
        }

        private void UpdateScratcherImage()
        {
            Blackboard currentScratcher = BlackboardQueryUtils.GetScratcher(currentScratcherId.value);
            ContextElement scratcherAreaElement = ContextUtils.FindElement(baseElement, "Scratcher Area", ContextSearchingType.ChildrenSearch);

            if (scratcherAreaElement.transform.childCount == 0)
            {
                MetaObjectUtils.MakePrefab(sharedBundle.value, "Scratcher Simple Image", scratcherAreaElement.transform);
                scratcherAreaElement.UpdateContext(true);
            }

            ContextElement simpleImageElement = ContextUtils.FindElement(scratcherAreaElement, "Scratcher Simple Image/Base", ContextSearchingType.FullNameSearch);

            MetaContextElementUtils.SetWebImage(
                simpleImageElement,
                currentScratcher.GetValue<string>("scratcherSimpleImageUrl"),
                CacheType.FileCache,
                false,
                () => { isLoaded.value = true; }
            );

            var reward = currentScratcher.GetValue<Blackboard>("reward");
            long maxPrize = reward.GetVariable<long>("maxWinCredit")?.value ?? 0L;
            int version = reward.GetVariable<int>("version")?.value ?? 0;
            var topPrizeArea = ContextUtils.FindElement(scratcherAreaElement, "Scratcher Simple Image/Top Prize Area", ContextSearchingType.FullNameSearch);
            var simpleImagePrizeTextElement = ContextUtils.FindElement(topPrizeArea, "Text", ContextSearchingType.ChildrenSearch);

            if (version == 0 || maxPrize == 0L)
            {
                topPrizeArea.gameObject.SetActive(false);
            }
            else
            {
                maxPrize = NumberUtils.GetMultiplierNumeratorValue(maxPrize, reward.GetValue<long>("tierMultiplierNumerator"));

                topPrizeArea.gameObject.SetActive(true);

                MetaContextElementUtils.SetTextGlobal(simpleImagePrizeTextElement, "COLLECTING_GAME_SIMPLE_IMAGE_PRIZE_TEXT", maxPrize);
            }
        }

        private void UpdateScratchButton(bool gameClosed)
        {
            ContextElement buttonScratchElement = ContextUtils.FindElement(baseElement, "Button Scratch", ContextSearchingType.ChildrenSearch);
            buttonScratchElement.GetComponent<PIDButton>().interactable = !gameClosed;

            ContextElement buttonScratchBadgeAreaElement = ContextUtils.FindElement(baseElement, "Badge Area", ContextSearchingType.ChildrenSearch);

            int makableScratcherCount = BlackboardQueryUtils.GetMakableScratcherCount(currentScratcherId.value);
            if (makableScratcherCount != Int32.MaxValue && makableScratcherCount > 0)
            {
                MetaObjectUtils.UpdateBadge(buttonScratchBadgeAreaElement, false, makableScratcherCount);
            }
            else
            {
                if (buttonScratchBadgeAreaElement.transform.childCount > 0)
                {
                    foreach (var child in buttonScratchBadgeAreaElement.transform.GetChildren())
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                    buttonScratchBadgeAreaElement.UpdateContext(true);
                }
            }
        }

        private void UpdateBottomTabArea()
        {
            List<Blackboard> chestList = BlackboardQueryUtils.GetChestList();

            for (int i = 0; i < chestList.Count; i++)
            {
                Blackboard chestInfo = chestList[i];
                ContextElement chestElement = ContextUtils.FindElement(agent, string.Format("Chest 0{0}", i + 1), ContextSearchingType.ChildrenSearch);
                int possessions = chestInfo.GetValue<int>("possessions");
                chestElement.GetComponent<PIDButton>().interactable = possessions > 0;

                ContextElement baseOnElement = ContextUtils.FindElement(chestElement, "Base On", ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SetActive(baseOnElement, possessions > 0);

                ContextElement badgeAreaElement = ContextUtils.FindElement(agent, string.Format("Badge Area 0{0}", i + 1), ContextSearchingType.ChildrenSearch);
                if (possessions == 0 && badgeAreaElement.transform.childCount > 0)
                    GameObject.Destroy(badgeAreaElement.transform.GetChild(0).gameObject);
                else if (possessions > 0)
                    MetaObjectUtils.UpdateBadge(badgeAreaElement, false, possessions);
            }
        }

        public void UpdateFreeChest()
        {
            long lastCollectTimestamp = BlackboardQueryUtils.GetLastFreePackCollectTimestamp();
            long nextCollectTimestamp = lastCollectTimestamp + BlackboardQueryUtils.GetFreePackCollectCoolTime();

            long currentTimestamp = TimeUtils.GetTimeStamp();

            ContextElement chestFreeElement = ContextUtils.FindElement(agent, "Chest Free", ContextSearchingType.ChildrenSearch);
            ContextElement chestFreeADElement = ContextUtils.FindElement(agent, "Chest Bonus AD", ContextSearchingType.ChildrenSearch);
            ContextElement timerAreaElement = ContextUtils.FindElement(chestFreeElement, "Event Timer Area", ContextSearchingType.ChildrenSearch);
            ContextElement eventTimerElement = ContextUtils.FindElement(timerAreaElement, "Event Timer", ContextSearchingType.ChildrenSearch);
            ContextElement timerElement = ContextUtils.FindElement(eventTimerElement, "Remaining Timer", ContextSearchingType.ChildrenSearch);
            // ContextElement adsElement = ContextUtils.FindElement(chestFreeElement, "Ads", ContextSearchingType.ChildrenSearch);
            ContextElement textElement = ContextUtils.FindElement(chestFreeElement, "Text", ContextSearchingType.ChildrenSearch);
            ContextElement freeChestBaseOnElement = ContextUtils.FindElement(chestFreeElement, "Base On", ContextSearchingType.ChildrenSearch);

            if (currentTimestamp >= nextCollectTimestamp)
            {

                MetaContextElementUtils.SetActive(chestFreeElement, true);
                MetaContextElementUtils.SetActive(chestFreeADElement, false);

                MetaContextElementUtils.SetActive(eventTimerElement, false);
                MetaContextElementUtils.SetActive(textElement, true);
                MetaContextElementUtils.SetActive(freeChestBaseOnElement, true);
                MetaContextElementUtils.SetActive(timerElement, false);
            }
            else
            {
                if(IsShowADS())
                {
                    var reserveTimer = agent.gameObject.GetComponent<SimpleReserveTimer>();
                    if(reserveTimer == null)
                        reserveTimer = agent.gameObject.AddComponent<SimpleReserveTimer>();

                    reserveTimer.SetReserveCallback(nextCollectTimestamp,
                    () =>
                    {
                        var targetOwner = ownerSystem.agent.GetComponent<GraphOwner>();
                        if(targetOwner != null)
                        {
                            targetOwner.SendEvent("OnEndTimer");
                        }
                    }
                    );

                    MetaContextElementUtils.SetActive(chestFreeElement, false);
                    MetaContextElementUtils.SetActive(chestFreeADElement, true);
                }
                else
                {
                    MetaContextElementUtils.SetActive(chestFreeElement, true);
                    MetaContextElementUtils.SetActive(chestFreeADElement, false);
                    MetaContextElementUtils.SetActive(eventTimerElement, true);
                    MetaContextElementUtils.SetActive(textElement, false);
                    MetaContextElementUtils.SetActive(freeChestBaseOnElement, false);
                    MetaContextElementUtils.SetActive(timerElement, true);

                    var timerBB = timerElement.GetComponent<Blackboard>();

                    BlackboardUtils.SetOrCreateValue<long>(timerBB, "targetTimestamp", nextCollectTimestamp);
                    BlackboardUtils.SetOrCreateValue<string>(timerBB, "textFormatKey", "TIME_FORMAT_HHMMSS_TOTALHOUR");
                    BlackboardUtils.SetOrCreateValue<bool>(timerBB, "_initTimer", true);
                    BlackboardUtils.SetOrCreateValue<GameObject>(timerBB, "caller", agent.gameObject);

                    timerElement.GetComponent<GraphOwner>().StopBehaviour();
                    timerElement.GetComponent<GraphOwner>().StartBehaviour();
                }
            }
        }

        private bool IsShowADS()
        {
            if(BlackboardQueryUtils.GetLastFreePackVideoAdsCollectingTimestamp() >= BlackboardQueryUtils.GetLastFreePackCollectTimestamp())
                return false;

            var inhouseAdsEnabled = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/INHOUSE_ADS_ENABLED");

            var iamTriggerType = BlackboardUtils.GetOrCreateVariable<InAppMessageTriggerType>(agentBB, "iamTriggerType");
            iamTriggerType.value = InAppMessageTriggerType.UNKNOWN;

            if(inhouseAdsEnabled.value)
            {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                if(!BlackboardQueryUtils.UserOptionsPushNotification() || !NativeHelper.Instance.GetPushNotificationSubscribed())
                {
                    if(IAMRouter.Instance.CheckTriggerIAM(InAppMessageTriggerType.INHOUSE_ADS_WITH_PUSH_OFF))
                    {
                        iamTriggerType.value = InAppMessageTriggerType.INHOUSE_ADS_WITH_PUSH_OFF;
                        return true;
                    }
                }
#endif
                var inhouseAD = IAMRouter.Instance.CheckTriggerIAM(ClientModels.InAppMessageTriggerType.INHOUSE_ADS_FOR_COLLECTING_GAME);
                if(inhouseAD)
                {
                    iamTriggerType.value = InAppMessageTriggerType.INHOUSE_ADS_FOR_COLLECTING_GAME;
                    return true;
                }
            }
            else
            {
                var videoAdsEnabled = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/VIDEO_ADS_ENABLED");

                if(videoAdsEnabled.value)
                {
                    var placement = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "videoAdsPlacementNames/collectingGame").value;
                    if(!string.IsNullOrEmpty(placement) && VideoAdsController.Instance.IsVideoAdsAvailable(placement))
                        return true;
                }
            }

            return false;
        }

        private void UpdateRequestButton()
        {
            ContextElement requestButtonElement = ContextUtils.FindElement(agent, "Button Request", ContextSearchingType.ChildrenSearch);

            ContextElement activeElement = ContextUtils.FindElement(requestButtonElement, "Active", ContextSearchingType.ChildrenSearch);
            ContextElement inactiveElement = ContextUtils.FindElement(requestButtonElement, "Inactive", ContextSearchingType.ChildrenSearch);

            var clubId = BlackboardUtils.GetOrCreateVariable<long>(null, "/me/clubId");
            if (clubId == null || clubId.value == 0)
            {
                requestButtonElement.GetComponent<PIDButton>().interactable = true;
                MetaContextElementUtils.SetActive(activeElement, true);
                MetaContextElementUtils.SetActive(inactiveElement, false);
            }
            else
            {
                var requestResetTimestampVariable = agentBB.GetVariable<long>("metaGameRequestResetTimestamp");
                long requestResetTimestamp = requestResetTimestampVariable != null ? requestResetTimestampVariable.value : BlackboardQueryUtils.GetCollectingGameRequestResetTimestamp();
                long currentTimestamp = TimeUtils.GetTimeStamp();

                if (currentTimestamp >= requestResetTimestamp)
                {
                    requestButtonElement.GetComponent<PIDButton>().interactable = true;
                    MetaContextElementUtils.SetActive(activeElement, true);
                    MetaContextElementUtils.SetActive(inactiveElement, false);
                }
                else
                {
                    requestButtonElement.GetComponent<PIDButton>().interactable = false;
                    MetaContextElementUtils.SetActive(activeElement, false);
                    MetaContextElementUtils.SetActive(inactiveElement, true);

                    ContextElement timerElement = ContextUtils.FindElement(inactiveElement, "Remaining Timer", ContextSearchingType.ChildrenSearch);

                    MetaContextElementUtils.SetCommonRemainingTimer(
                        timerElement,
                        requestResetTimestamp,
                        0,
                        "TIME_FORMAT_HHMMSS_TOTALHOUR",
                        "",
                        "",
                        "Ended",
                        true,
                        agent.gameObject);
                }
            }
        }
    }
}
