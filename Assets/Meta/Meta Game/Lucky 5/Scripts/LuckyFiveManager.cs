using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using System.Collections;

namespace BagelCode.LuckyFive
{
    public class LuckyFiveManager : LuckyFiveMoveContainerController
    {
        private ContextElement buttonElement;
        private Animator buttonAnim;

        private ContextElement iconAreaElement;
        private ContextElement lockedIconElement;
        private ContextElement lockedBalloonElement;

        private ContextElement speechBalloonElement;
        private ContextElement speechBalloonTextElement;
        private Animator speechBalloonAnim;

        public GraphOwner owner;
        public List<Transform> resultCardObjectList = new List<Transform>();

        public string completeAddCardEvent;
        public string beginResultEvent;

        public string bundleName;
        public string cardPrefabName;

        public Transform appearStartTransform;

        private long eligibleMinBet = 0L;
        private bool isPrevEligible = false;

        public List<int> deckList = new List<int>();
        public List<bool> hitList = new List<bool>();

        private bool isEligibleBet = false;

        IContextText avgBetTextElement = null;
        IContextText earnCoinTextElement = null;

        public static string COMMON_BUNDLE_NAME =>
            ApplicationSettings.MakeApplicationBundleName("mglucky5common");

        public void InitLuckyFive()
        {
            if (isInit) return;

            base.InitProperty();

            if (buttonElement == null)
                buttonElement = ContextUtils.FindElement(root, "Lucky 5 Button", CHILDREN);
            buttonAnim = buttonElement.GetComponent<Animator>();

            iconAreaElement = ContextUtils.FindElement(buttonElement, "Icon Area", CHILDREN);
            lockedIconElement = ContextUtils.FindElement(buttonElement, "Locked Area/Lucky 5 Icon Locked", ContextSearchingType.FullNameSearch);
            lockedBalloonElement = ContextUtils.FindElement(buttonElement, "Locked Area/Lucky 5 Locked Info Speech Balloon", ContextSearchingType.FullNameSearch);

            speechBalloonElement = ContextUtils.FindElement(root, "Lucky 5 Info Speech Balloon", CHILDREN);
            speechBalloonAnim = speechBalloonElement.GetComponent<Animator>();

            speechBalloonTextElement = ContextUtils.FindElement(speechBalloonElement, "Text", CHILDREN);

            var metaGameInfoBB = BlackboardQueryUtils.GetMetaGameEnterInfo();
            if (metaGameInfoBB != null)
                eligibleMinBet = metaGameInfoBB.GetValue<long>("eligibleMinBet");

            var betCredit = BlackboardUtils.FindVariable<long>("./betCredit");
            isPrevEligible = eligibleMinBet <= betCredit.value;

            // Update All
            BlackboardQueryUtils.InitMetaGame();

            appearStartTransform = MetaGameAppearTransformManager.Instance.GetTransform("SpinButton");
            LoadLuckyFiveWinInfo();
            LoadTargetAnchors();
            LoadShowCards();
            LoadResultObjects();

            InitPosition();

            bool isEligible = ShowEligible(true);
            OnSetEligible(isEligible, true);

            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_OPEN_META_EVENT_GROUP, OnOpenMetaEventGroup);

            isInit = true;
        }

        protected override ContextElement GetButtonClickable()
        {
            if (buttonElement == null)
                buttonElement = ContextUtils.FindElement(root, "Lucky 5 Button", CHILDREN);
            return buttonElement;
        }

        private void OnOpenMetaEventGroup()
        {
            var balloonAnim = speechBalloonElement.GetComponent<Animator>();
            balloonAnim.SetBool("IsActive", false);
        }

        public void OnAddCard()
        {
            BlackboardQueryUtils.LuckyFiveAddCardComplete();
            LoadLuckyFiveWinInfo();

            isItemEarning = true;

            // Make Flying Card
            string bundle = COMMON_BUNDLE_NAME;
            string asset = "In Game Lucky 5 Card";
            Transform parent = earningItemRootArea;

            earnEffectObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
            if (earnEffectObj == null) return;

            Vector3 from = MetaGameAppearTransformManager.Instance.GetTransform("SpinButton").position;
            Vector3 to = transform.position;

            // Movement
            float EFFECT_MOVEMENT_TIME = 1.3f;
            var controller = earnEffectObj.GetComponent<EarningMetaGameItemController>();
            controller.flyingTime = EFFECT_MOVEMENT_TIME;
            AsyncActionUtils.ApplyMovement(this,
                earnEffectObj.transform, from, to, EFFECT_MOVEMENT_TIME,
                TweenUtils.VectorTweenCollectMove,
                0f,
                OnArriveEarningMetaGameItem);
        }

        public void OnMoveCard()
        {
            Move();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (isInit)
            {
                var buttonAnim = buttonElement.GetComponent<Animator>();
                buttonAnim.SetBool("IsActive", true);
                buttonAnim.SetBool("IsEnable", true);

                UpdateLuckyFiveLevelLocked();
                OnUpdateBet(true);
            }
        }

        public void UpdateLuckyFiveLevelLocked()
        {
            Blackboard metaGameEnterInfo = BlackboardQueryUtils.GetMetaGameEnterInfo();
            bool isLockedFeature = BlackboardQueryUtils.IsLockedFeature(LockedFeatureType.META_GAME);

            if (metaGameEnterInfo != null && (!isLockedFeature || MetaGameUtils.IsMetaEventLevelLock()))
            {
                bool isLockedLevel = MetaGameUtils.IsMetaGameLevelLocked();

                iconAreaElement.gameObject.SetActive(!isLockedLevel);
                lockedIconElement.gameObject.SetActive(isLockedLevel);
                lockedBalloonElement.gameObject.SetActive(isLockedLevel);
            }
        }

        public void OnUpdateBet(bool forceUpdate = false)
        {
            bool isEligible = ShowEligible(forceUpdate);
            OnSetEligible(isEligible, forceUpdate);
        }

        public void OnResultCardList()
        {
            // Update Result Card List
            for (int i = 0; i < BlackboardQueryUtils.LUCKY_FIVE_DECK_COUNT; ++i)
            {
                var cardInstance = resultCardObjectList[i].GetComponent<LuckyFiveCardInstance>();

                cardInstance.cardInfo = GetLuckyFiveCardInfo(i);
                cardInstance.hit = IsHit(i);

                if (!cardInstance.fsmOwner.isRunning)
                    cardInstance.fsmOwner.graph.isRunning = true;

                if (cardInstance.cardInfo == null)
                {
                    cardInstance.fsmOwner.TriggerState("Close");
                }
                else
                {
                    cardInstance.fsmOwner.TriggerState("Set");
                }
            }

            // Update Rewards Text.
            avgBetTextElement.SetText(StringTableUtils.GetString(StringTable.StringTableType.Global, "LUCKY_FIVE_RESULT_AVG_BET_TEXT", BlackboardQueryUtils.GetLuckyFiveAverageBet()));

            string ruleText = StringTableUtils.GetString(StringTable.StringTableType.Global, "LUCKY_FIVE_RULE_FORMAT", BlackboardQueryUtils.GetLuckyFiveRule().ToString());
            ruleText = StringTableUtils.GetString(StringTable.StringTableType.Global, ruleText);

            earnCoinTextElement.SetText(StringTableUtils.GetString(StringTable.StringTableType.Global, "LUCKY_FIVE_RESULT_TOTAL_WIN_TEXT", ruleText, BlackboardQueryUtils.GetLuckyFiveEarnCoins()));
        }

        private void OnSetEligible(bool isEligible, bool forceUpdate)
        {
            if (forceUpdate || isEligibleBet != isEligible)
            {
                isEligibleBet = isEligible;
                // Update Result Card List
                for (int i = 0; i < BlackboardQueryUtils.LUCKY_FIVE_DECK_COUNT; ++i)
                {
                    var cardInstance = objectList[i].GetComponent<LuckyFiveCardInstance>();

                    cardInstance.cardInfo = GetLuckyFiveCardInfo(i);
                    cardInstance.hit = IsHit(i);

                    if (!cardInstance.fsmOwner.isRunning)
                        cardInstance.fsmOwner.graph.isRunning = true;

                    if (cardInstance.cardInfo == null)
                    {
                        cardInstance.fsmOwner.TriggerState("Close");
                    }
                    else
                    {
                        cardInstance.fsmOwner.TriggerState("Set");
                    }
                }
            }
        }

        public override void RemoveFirstContainer(Transform targetObject)
        {
            for (int i = 0; i < BlackboardQueryUtils.LUCKY_FIVE_DECK_COUNT; ++i)
            {
                var cardInstance = objectList[i].GetComponent<LuckyFiveCardInstance>();

                if (!cardInstance.fsmOwner.isRunning)
                    cardInstance.fsmOwner.graph.isRunning = true;

                if (objectList[i] == targetObject || cardInstance.cardInfo == null)
                {
                    cardInstance.fsmOwner.TriggerState("Close");
                }
                else
                {
                    if (BlackboardQueryUtils.LuckyFiveReadyDeck())
                    {
                        cardInstance.fsmOwner.TriggerState("Deactive");
                    }
                    else
                    {
                        cardInstance.hit = true;
                        cardInstance.fsmOwner.TriggerState("Set");
                    }
                }
            }
        }

        public override void AddLastContainer(Transform targetObject)
        {

        }

        public override void MoveComplete(Transform targetObject)
        {
            var cardInstance = targetObject.GetComponent<LuckyFiveCardInstance>();

            cardInstance.cardInfo = GetLuckyFiveCardInfo(BlackboardQueryUtils.LUCKY_FIVE_DECK_COUNT - 1);
            cardInstance.hit = IsHit(BlackboardQueryUtils.LUCKY_FIVE_DECK_COUNT - 1);

            if (!cardInstance.fsmOwner.isRunning)
                cardInstance.fsmOwner.graph.isRunning = true;

            cardInstance.fsmOwner.TriggerState("Open");
        }

        public override void OpenComplete()
        {
            for (int i = 0; i < BlackboardQueryUtils.LUCKY_FIVE_DECK_COUNT; ++i)
            {
                var cardInstance = objectList[i].GetComponent<LuckyFiveCardInstance>();

                cardInstance.cardInfo = GetLuckyFiveCardInfo(i);
                cardInstance.hit = IsHit(i);

                if (!cardInstance.fsmOwner.isRunning)
                    cardInstance.fsmOwner.graph.isRunning = true;

                if (cardInstance.cardInfo == null)
                {
                    cardInstance.fsmOwner.TriggerState("Close");
                }
                else
                {
                    cardInstance.fsmOwner.TriggerState("Set");
                }
            }

            if (BlackboardQueryUtils.ExistLuckyFiveRule())
            {
                owner.SendEvent(beginResultEvent);
            }
            else
            {
                owner.SendEvent(completeAddCardEvent);
            }
        }

        private void LoadLuckyFiveWinInfo()
        {
            deckList = BlackboardQueryUtils.GetLuckyFiveDeck();
            hitList = BlackboardQueryUtils.GetLuckyFiveHitList();
        }

        private void LoadTargetAnchors()
        {
            targetList.Add(transform.Find("Anchor/Lucky 5 Button/Anchor/Icon Area/Card Anchor/Lucky 5 Card Area/Card Area 1"));
            targetList.Add(transform.Find("Anchor/Lucky 5 Button/Anchor/Icon Area/Card Anchor/Lucky 5 Card Area/Card Area 2"));
            targetList.Add(transform.Find("Anchor/Lucky 5 Button/Anchor/Icon Area/Card Anchor/Lucky 5 Card Area/Card Area 3"));
            targetList.Add(transform.Find("Anchor/Lucky 5 Button/Anchor/Icon Area/Card Anchor/Lucky 5 Card Area/Card Area 4"));
            targetList.Add(transform.Find("Anchor/Lucky 5 Button/Anchor/Icon Area/Card Anchor/Lucky 5 Card Area/Card Area 5"));
        }

        private void LoadShowCards()
        {
            Transform parent = transform.Find("Anchor/Lucky 5 Button/Anchor/Icon Area/Card Anchor/Lucky 5 Card Area");
            var prefab = AssetBundleManager.LoadAsset<GameObject>(bundleName, cardPrefabName);

            for (int i = 0; i < BlackboardQueryUtils.LUCKY_FIVE_DECK_COUNT; ++i)
            {
                GameObject go = MakeCard(prefab, string.Format("Show Card {0}", i + 1), i, parent);

                objectList.Add(go.transform);
            }
        }

        private bool ShowEligible(bool forceShow)
        {
            var betCredit = BlackboardUtils.FindVariable<long>("./betCredit");
            bool isEligible = eligibleMinBet <= betCredit.value;

            buttonAnim.SetBool("IsBetMore", !isEligible);

            bool isItemEarning = false;
            if(BlackboardQueryUtils.IsIngame())
            {
                // not good coupling..
                // todo think better solution
                if (InGameMetaIconGroupController.Instance != null)
                    isItemEarning = InGameMetaIconGroupController.Instance.isItemEarning;
            }

            if (eligibleMinBet > 0 && isEligible && !isItemEarning &&
                (forceShow || isEligible != isPrevEligible) &&
                !MetaGameUtils.IsMetaGameLevelLocked())
            {
                speechBalloonAnim.SetBool("IsActive", true);
                StartCoroutine(HideEligibleBetCoroutine());

                MetaContextElementUtils.SetTextGlobal(speechBalloonTextElement, "LUCKY_FIVE_ELIGIBLE_BET_ACTIVE");

                isPrevEligible = isEligible;
            }

            return isEligible;
        }

        private IEnumerator HideEligibleBetCoroutine()
        {
            yield return new WaitForSeconds(3f);
            speechBalloonAnim.SetBool("IsActive", false);
        }

        private void LoadResultObjects()
        {
            var prefab = AssetBundleManager.LoadAsset<GameObject>(bundleName, cardPrefabName);

            for (int i = 0; i < BlackboardQueryUtils.LUCKY_FIVE_DECK_COUNT; ++i)
            {
                Transform parent = transform.Find(string.Format("Anchor/Lucky 5 Win/Anchor/Win Card Area/Card Area {0}", i + 1));

                GameObject go = MakeCard(prefab, string.Format("Result Card {0}", i + 1), i, parent);

                resultCardObjectList.Add(go.transform);
            }

            ContextElement agentElement = gameObject.GetComponent<ContextElement>();
            ContextElement betTextElement = ContextUtils.FindElement(agentElement, "Lucky 5 Win/Text Bet", ContextSearchingType.FullNameSearch);
            ContextElement winTextElement = ContextUtils.FindElement(agentElement, "Lucky 5 Win/Text Win", ContextSearchingType.FullNameSearch);

            avgBetTextElement = betTextElement as IContextText;
            earnCoinTextElement = winTextElement as IContextText;
        }

        private GameObject MakeCard(GameObject prefab, string cardName, int cardIndex, Transform parent)
        {
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            go.name = cardName;
            var cardInstance = go.GetComponent<LuckyFiveCardInstance>();

            cardInstance.fsmOwner.StopBehaviour();
            cardInstance.fsmOwner.StartBehaviour();

            cardInstance.cardInfo = GetLuckyFiveCardInfo(cardIndex);
            cardInstance.hit = IsHit(cardIndex);

            if (cardInstance.cardInfo != null)
            {
                if (!cardInstance.fsmOwner.isRunning)
                    cardInstance.fsmOwner.graph.isRunning = true;
                cardInstance.fsmOwner.TriggerState("Set");
            }

            go.transform.SetParent(parent, false);

            return go;
        }

        private LuckyFiveCardInfo GetLuckyFiveCardInfo(int deckIndex)
        {
            if (deckList.Count > deckIndex && deckList[deckIndex] > -1)
                return LuckyFiveCustomCardData.Instance.cardAssets.cards[deckList[deckIndex]];

            return null;
        }

        private bool IsHit(int deckIndex)
        {
            if (!isEligibleBet) return false;
            if (!BlackboardQueryUtils.LuckyFiveReadyDeck()) return true;
            if (hitList.Count > deckIndex)
                return hitList[deckIndex];

            return true;
        }
    }
}
