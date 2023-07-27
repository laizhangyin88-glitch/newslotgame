using System;
using System.Collections;
using System.Collections.Generic;
using GameStudio.Slot;
using ParadoxNotion;
using TMPro;
using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEditor;

namespace GameStudio.Slot.CTC
{
    enum EFreeGameBonusType
    {
        PayUp = 1 << 0,
        DoubleSpin = 1 << 1,
        SuperSpin = 1 << 2,
    }

    public class CTCPostSpinController : FeatureController
    {
        [SerializeField] private List<SlotMachine> slotMachineList;
        [SerializeField] private List<TextMeshProUGUI> spinCountUIList;
        [SerializeField] private List<TextMeshProUGUI> lockedReelCountUIList;
        [SerializeField] private GameObject payUpFlyingObj;
        [SerializeField] private CTCPotController potController;

        [SerializeField] private float potAnimWaitDelay = 1f;
        [SerializeField] private float waitTimePerInitialSymbolEmerge = 0.5f;
        [SerializeField] private float copyingSymbolFlyingTime = 1.3f;

        [SerializeField] private float waitTimePerExtraSpin = 0.5f;
        [SerializeField] private float extraSpinFlyingTime = 1.2f;
        [SerializeField] private float extraSpinFlyingAnimTime = 2f;

        [SerializeField] private float waitTimePerPayUp = 0.5f;
        [SerializeField] private float payUpFlyingAppearTime = 0.4f;
        [SerializeField] private float payUpFlyingDisappearTime = 0.5f;
        [SerializeField] private float payUpFlyingFlyTime = 0.2f;
        [SerializeField] private int payUpFlyingPositionReelIndex = 7;
        [SerializeField] private float superCashFlyingTime = 0.4f;

        [SerializeField] private float waitTimePerSuperCashCollect = 0.5f;

        [SerializeField] private ObjectPool extraSpinFlyingPool;
        [SerializeField] private ObjectPool cashCopyingFlyingPool;
        [SerializeField] private ObjectPool superCashFlyingPool;

        private DirectionalWeightPositionController payUpFlyingPositionController;
        private Animator payUpFlyingAnimator;

        private List<int> remainSpinCountPerSlot;
        private List<int> lockedReelCountPerSlot;

        public const string INIT_SPIN_COUNT_UI_EVENT = "InitSpinCount";
        public const string SUBSTRACT_SPIN_COUNT_UI_EVENT = "SubstractSpinCount";
        public const string INIT_LOCKED_REELS_START_EVENT = "InitLockedReelCountStart";
        public const string ADD_LOCKED_REEL_COUNT_UI_EVENT = "AddLockedReelCount";
        public const string UPDATE_STOP_EFFECT_SPOTS_EVENT = "UpdateStopEffectSpots";
        public const string INIT_LOCKED_REELS_DONE_EVENT = "InitLockedReelCountDone";

        public const string ON_SUPER_CASH_FEATURE_EVENT = "OnSuperCashFeatureStart";
        public const string ON_PAYUP_FEATURE_EVENT = "OnPayupFeatureStart";
        public const string EXTRA_SPIN_COLLECTED_EVENT = "ExtraSpinCollected";
        public const string PAYUP_STOMPING_EVENT_EVENT = "PayupStomp";

        protected override string ON_FEATURE_BEGIN_EVENT
        {
            get => "OrderPostSpinBehaviours";
        }
        protected override string ON_FEATURE_END_EVENT
        {
            get => "DonePostSpinBehaviours";
        }

        public const int BLANK_SYMBOL_INDEX = 14;
        public const int CASH_SYMBOL_INDEX = 15;
        public const int EXTRA_SPIN_SYMBOL_INDEX = 16;
        public const int SUPER_CASH_SYMBOL_INDEX = 17;

        public static List<Blackboard> BonusGameResponseList
        {
            get
            {
                var bonusList = BlackboardUtils.FindValue<List<Blackboard>>(null, "./turn/spin/response/bonusResult");
                List<Blackboard> targetBonusRsponseList = new List<Blackboard>();
                foreach (var bonus in bonusList)
                {
                    if (bonus.GetValue<int>("bonusId") == BONUS_GAME_BONUS_ID)
                        targetBonusRsponseList.Add(bonus);
                }

                return targetBonusRsponseList;
            }
        }

        public const int BONUS_GAME_BONUS_ID = 21800;

        protected override IEnumerator OnPlayCoroutine()
        {
            var responseList = BonusGameResponseList;
            int bonusCombination = responseList[0].GetValue<int>("bonusCombination");
            bool isDoubleSpin = (bonusCombination & (int)EFreeGameBonusType.DoubleSpin) != 0;
            int slotIterStartIndex = isDoubleSpin ? 0 : 1;

            int bonusSpunCount = BlackboardUtils.FindValue<int>(null, "./bonusSpunCount");

            int responseIndex = 0;
            bool hasAddedSpin = false;
            // spin collect
            for (int slotIndex = slotIterStartIndex; slotIndex < slotMachineList.Count; slotIndex++)
            {
                var response = responseList[responseIndex++];
                if (response.GetValue<List<Blackboard>>("reelOutputListPerSpin").Count <= bonusSpunCount) continue;

                var slotMachine = slotMachineList[slotIndex];
                var spinCountUI = spinCountUIList[slotIndex];
                for (int reelIndex = 0; reelIndex < slotMachine.reels.Count; reelIndex++)
                {
                    BaseSymbol symbol;
                    if ((symbol = slotMachine.GetSymbol(reelIndex, 0)).symbolIndex != EXTRA_SPIN_SYMBOL_INDEX) continue;

                    hasAddedSpin = true;
                    symbol.Play("Win");
                    var flyingObj = extraSpinFlyingPool.GetObject();
                    var fromToController = flyingObj.GetComponentInChildren<DirectionalWeightPositionController>();
                    fromToController.from = symbol.transform;
                    fromToController.to = spinCountUI.transform;
                    flyingObj.gameObject.SetActive(true);

                    int targetSlotIndex = slotIndex;
                    GSManager.Instance.GetHandler("Cash Symbol Fly").Play();
                    StartCoroutine(CTCUtill.CallActionAfterDelay(() =>
                {
                    remainSpinCountPerSlot[targetSlotIndex] += (int)symbol.symbolInfo.customData["ExtraSpinCount"];
                    spinCountUIList[targetSlotIndex].text = (remainSpinCountPerSlot[targetSlotIndex]).ToString();
                    ContentEvent.SendEvent<int>(EXTRA_SPIN_COLLECTED_EVENT, targetSlotIndex);
                }, extraSpinFlyingTime));

                    StartCoroutine(CTCUtill.CallActionAfterDelay(() =>
                    {
                        flyingObj.GetComponent<PooledObject>().ReturnToPool();
                    }, extraSpinFlyingAnimTime));

                    yield return new WaitForSeconds(waitTimePerExtraSpin);
                }
            }

            if (hasAddedSpin) yield return new WaitForSeconds(extraSpinFlyingTime);

            // pay up
            var waitForSeconds = new WaitForSeconds(waitTimePerPayUp);
            responseIndex = 0;
            if ((bonusCombination & (int)EFreeGameBonusType.PayUp) != 0)
            {
                var payUpTriggeredFlagPerSlot = BlackboardUtils.FindValue<List<bool>>(null, "./customData/payUpTriggeredFlagPerSlot");
                payUpTriggeredFlagPerSlot[0] = false;
                payUpTriggeredFlagPerSlot[1] = false;

                bool isPayupTrigger = false;
                bool hasPopupOn = false;
                for (int slotIndex = slotIterStartIndex; slotIndex < slotMachineList.Count; slotIndex++)
                {
                    var response = responseList[responseIndex++];
                    if (response.GetValue<List<Blackboard>>("payUpReelIndexListPerSpin").Count <= bonusSpunCount) continue;

                    var payUpReelIndexList = response.GetValue<List<Blackboard>>("payUpReelIndexListPerSpin")[bonusSpunCount]
                    .GetValue<List<int>>("value");

                    if (payUpReelIndexList.Count == 0) continue;
                    payUpTriggeredFlagPerSlot[slotIndex] = true;
                    isPayupTrigger = true;
                }


                if (isPayupTrigger && !hasPopupOn)
                {
                    potController.SetFeatureAnimOfPot(0, true);
                    GSManager.Instance.GetHandler("Feature On").Play();
                    ContentEvent.SendEvent(ON_PAYUP_FEATURE_EVENT);
                    hasPopupOn = true;
                    yield return new WaitForSeconds(potAnimWaitDelay);
                }

                var flyingObjAppearWaitSeconds = new WaitForSeconds(payUpFlyingAppearTime);
                var flyingObjDisappearWaitSeconds = new WaitForSeconds(payUpFlyingDisappearTime);
                var flyingObjFlyingWaitSeconds = new WaitForSeconds(payUpFlyingFlyTime);
                responseIndex = 0;
                for (int slotIndex = slotIterStartIndex; slotIndex < slotMachineList.Count; slotIndex++)
                {
                    var response = responseList[responseIndex++];
                    if (response.GetValue<List<Blackboard>>("payUpReelIndexListPerSpin").Count <= bonusSpunCount) continue;

                    var payUpReelIndexList = response.GetValue<List<Blackboard>>("payUpReelIndexListPerSpin")[bonusSpunCount]
                    .GetValue<List<int>>("value");

                    if (payUpReelIndexList.Count == 0) continue;

                    var payUpMultiplierList = response.GetValue<List<Blackboard>>("payUpMultiplierListPerSpin")[bonusSpunCount]
                    .GetValue<List<double>>("value");
                    var slotMachine = slotMachineList[slotIndex];
                    payUpFlyingObj.SetActive(true);
                    var flyingFromTransform = slotMachine.GetSymbol(payUpFlyingPositionReelIndex, 0).transform;
                    payUpFlyingObj.transform.position = flyingFromTransform.position;
                    payUpFlyingPositionController.from = flyingFromTransform;
                    yield return flyingObjAppearWaitSeconds;

                    for (int i = 0; i < payUpReelIndexList.Count; i++)
                    {
                        int targetReel = payUpReelIndexList[i];
                        double additionalMultiplier = payUpMultiplierList[i];
                        var targetSymbol = slotMachine.GetOverlaySymbol(new Cell(targetReel, 0).GetHashCode());
                        payUpFlyingPositionController.to = targetSymbol.transform;
                        payUpFlyingAnimator.SetTrigger("Fly");
                        yield return flyingObjFlyingWaitSeconds;

                        targetSymbol.symbolInfo.customData["Multiplier"] =
                            (double)targetSymbol.symbolInfo.customData["Multiplier"] + additionalMultiplier;
                        targetSymbol.Play("RefreshMultiplier");
                        GSManager.Instance.GetHandler("Symbol Pay Up").Play();
                        ContentEvent.SendEvent<int>(PAYUP_STOMPING_EVENT_EVENT, slotIndex);

                        yield return flyingObjFlyingWaitSeconds;
                    }

                    payUpFlyingAnimator.SetTrigger("Disappear");
                    yield return flyingObjDisappearWaitSeconds;
                    payUpFlyingObj.SetActive(false);
                }
            }

            potController.SetFeatureAnimOfPot(0, false);

            // super spin
            waitForSeconds = new WaitForSeconds(waitTimePerSuperCashCollect);
            responseIndex = 0;
            if ((bonusCombination & (int)EFreeGameBonusType.SuperSpin) != 0)
            {
                var superCashTriggeredFlagPerSlot = BlackboardUtils.FindValue<List<bool>>(null, "./customData/superCashTriggeredFlagPerSlot");
                superCashTriggeredFlagPerSlot[0] = false;
                superCashTriggeredFlagPerSlot[1] = false;
                for (int slotIndex = slotIterStartIndex; slotIndex < slotMachineList.Count; slotIndex++)
                {
                    var response = responseList[responseIndex++];
                    if (response.GetValue<List<Blackboard>>("reelOutputListPerSpin").Count <= bonusSpunCount) continue;

                    var thisSpinSymbolPerReel = response.GetValue<List<Blackboard>>("symbolPerReelPerSpin")[bonusSpunCount]
                    .GetValue<List<int>>("value");
                    var slotMachine = slotMachineList[slotIndex];

                    List<double> thisSpinMutiplierPerReel = response.GetValue<List<Blackboard>>("cashMultiplierListPerSpin")[bonusSpunCount].GetValue<List<double>>("value");
                    List<int> thisSpinPayupReelIndexList = response.GetValue<List<Blackboard>>("payUpReelIndexListPerSpin")[bonusSpunCount].GetValue<List<int>>("value");
                    bool isAbleToCalcSuperCash = CTCSuperCashSymbolBehaviour.IsAbleToCalcSuperCash(slotMachine, thisSpinMutiplierPerReel, thisSpinPayupReelIndexList, thisSpinSymbolPerReel);

                    bool hasSuperCash = false;
                    for (int reelIndex = 0; reelIndex < slotMachine.reels.Count; reelIndex++)
                    {
                        var originSymbol = slotMachine.GetSymbol(reelIndex, 0);
                        var overlaySymbol = slotMachine.GetOverlaySymbol(new Cell(reelIndex, 0).GetHashCode());
                        bool isPreviousLandedSuperCash = overlaySymbol != null ? (overlaySymbol.symbolIndex == SUPER_CASH_SYMBOL_INDEX && overlaySymbol.symbolInfo.customData != null) : false;
                        // check if the super cash symbol is this spin's result, not previously landed one
                        if (originSymbol.symbolIndex == SUPER_CASH_SYMBOL_INDEX && thisSpinSymbolPerReel[reelIndex] == SUPER_CASH_SYMBOL_INDEX && !isPreviousLandedSuperCash)
                        {
                            hasSuperCash = true;
                            break;
                        }
                    }

                    if (hasSuperCash == false || isAbleToCalcSuperCash == false) continue;
                    superCashTriggeredFlagPerSlot[slotIndex] = true;
                }

                bool hasPopupOn = false;
                responseIndex = 0;
                for (int slotIndex = slotIterStartIndex; slotIndex < slotMachineList.Count; slotIndex++)
                {
                    var response = responseList[responseIndex++];
                    if (response.GetValue<List<Blackboard>>("reelOutputListPerSpin").Count <= bonusSpunCount) continue;

                    var thisSpinSymbolPerReel = response.GetValue<List<Blackboard>>("symbolPerReelPerSpin")[bonusSpunCount]
                    .GetValue<List<int>>("value");
                    var slotMachine = slotMachineList[slotIndex];

                    List<double> thisSpinMutiplierPerReel = response.GetValue<List<Blackboard>>("cashMultiplierListPerSpin")[bonusSpunCount].GetValue<List<double>>("value");
                    List<int> thisSpinPayupReelIndexList = response.GetValue<List<Blackboard>>("payUpReelIndexListPerSpin")[bonusSpunCount].GetValue<List<int>>("value");
                    bool isAbleToCalcSuperCash = CTCSuperCashSymbolBehaviour.IsAbleToCalcSuperCash(slotMachine, thisSpinMutiplierPerReel, thisSpinPayupReelIndexList, thisSpinSymbolPerReel);

                    if (isAbleToCalcSuperCash == false) continue;

                    var targetSuperCashReelIndexList = new List<int>();
                    for (int reelIndex = 0; reelIndex < slotMachine.reels.Count; reelIndex++)
                    {
                        var originSymbol = slotMachine.GetSymbol(reelIndex, 0);
                        var overlaySymbol = slotMachine.GetOverlaySymbol(new Cell(reelIndex, 0).GetHashCode());
                        bool isPreviousLandedSuperCash = overlaySymbol != null ? (overlaySymbol.symbolIndex == SUPER_CASH_SYMBOL_INDEX && overlaySymbol.symbolInfo.customData != null) : false;
                        // check if the super cash symbol is this spin's result, not previously landed one
                        if (originSymbol.symbolIndex == SUPER_CASH_SYMBOL_INDEX && thisSpinSymbolPerReel[reelIndex] == SUPER_CASH_SYMBOL_INDEX && !isPreviousLandedSuperCash)
                        {
                            targetSuperCashReelIndexList.Add(reelIndex);
                        }
                    }

                    if (targetSuperCashReelIndexList.Count == 0) continue;

                    if (!hasPopupOn)
                    {
                        potController.SetFeatureAnimOfPot(2, true);
                        GSManager.Instance.GetHandler("Feature On").Play();
                        ContentEvent.SendEvent(ON_SUPER_CASH_FEATURE_EVENT);
                        hasPopupOn = true;
                        yield return new WaitForSeconds(potAnimWaitDelay);
                    }

                    foreach (var reelIndex in targetSuperCashReelIndexList)
                    {
                        var targetSymbol = slotMachine.GetOverlaySymbol(new Cell(reelIndex, 0).GetHashCode());
                        var symbolCustomData = targetSymbol.symbolInfo.customData = new Dictionary<string, object>();

                        for (int cashReelIndex = 0; cashReelIndex < slotMachine.reels.Count; cashReelIndex++)
                        {
                            var cashSymbol = slotMachine.GetOverlaySymbol(new Cell(cashReelIndex, 0).GetHashCode());
                            if (cashSymbol == null) continue;

                            if (cashSymbol.symbolIndex == CASH_SYMBOL_INDEX && (double)cashSymbol.symbolInfo.customData["Multiplier"] > 0)
                            {
                                var flyingObj = superCashFlyingPool.GetObject();
                                var fromToController =
                                    flyingObj.GetComponentInChildren<DirectionalWeightPositionController>(true);
                                fromToController.from = cashSymbol.transform;
                                fromToController.to = targetSymbol.transform;
                                flyingObj.gameObject.SetActive(true);
                                GSManager.Instance.GetHandler("Super Cash Symbol Move").Play();

                                StartCoroutine(CTCUtill.CallActionAfterDelay(() =>
                                {
                                    if (symbolCustomData.ContainsKey("Multiplier"))
                                        symbolCustomData["Multiplier"] =
                                            (double)cashSymbol.symbolInfo.customData["Multiplier"] +
                                            (double)symbolCustomData["Multiplier"];
                                    else symbolCustomData.Add("Multiplier", cashSymbol.symbolInfo.customData["Multiplier"]);
                                    targetSymbol.Play("RefreshMultiplier");
                                    GSManager.Instance.GetHandler("Symbol Pay Up").Play();

                                    StartCoroutine(CTCUtill.CallActionAfterDelay(() =>
                                    {
                                        flyingObj.ReturnToPool();
                                    }, 0.7f));
                                }, superCashFlyingTime));

                                yield return waitForSeconds;
                            }
                        }
                        potController.SetFeatureAnimOfPot(2, false);
                    }
                }
            }

            yield break;
        }

        protected override void OnEnable()
        {
            payUpFlyingAnimator = payUpFlyingObj.GetComponentInChildren<Animator>();
            payUpFlyingPositionController = payUpFlyingObj.GetComponentInChildren<DirectionalWeightPositionController>();
            base.OnEnable();
            RegisterEvent(SUBSTRACT_SPIN_COUNT_UI_EVENT, (eventData) =>
            {
                for (int i = 0; i < remainSpinCountPerSlot.Count; i++)
                {
                    if (remainSpinCountPerSlot[i] <= 0 || lockedReelCountPerSlot[i] == slotMachineList[i].ColumnCount) continue;

                    remainSpinCountPerSlot[i]--;
                    spinCountUIList[i].text = remainSpinCountPerSlot[i].ToString();
                }
            });

            RegisterEvent(INIT_SPIN_COUNT_UI_EVENT, (eventData) =>
            {
                remainSpinCountPerSlot = new List<int>();
                int initialSpinCount = BlackboardUtils.FindValue<int>(null, "./bonus/response/totalSpinCount");
                foreach (var spinCountUI in spinCountUIList)
                {
                    spinCountUI.text = initialSpinCount.ToString();
                    remainSpinCountPerSlot.Add(initialSpinCount);
                }

                foreach(var lockedReelCountUI in lockedReelCountUIList)
                {
                    lockedReelCountUI.text = 0.ToString();
                }
            });


            RegisterEvent(INIT_LOCKED_REELS_START_EVENT, (eventData) =>
            {
                StartCoroutine(EmergeInitialLockedSymbols());
            });

            RegisterEvent(ADD_LOCKED_REEL_COUNT_UI_EVENT, (eventData) =>
            {
                int slotIndex = BlackboardUtils.FindValue<int>(null, "./_currentStoppingSlotIndex");
                lockedReelCountPerSlot[lockedReelCountPerSlot.Count - slotIndex]++;
                lockedReelCountUIList[lockedReelCountUIList.Count - slotIndex].text = lockedReelCountPerSlot[lockedReelCountPerSlot.Count - slotIndex].ToString();
            });

            RegisterEvent(UPDATE_STOP_EFFECT_SPOTS_EVENT, (eventData) =>
            {
                var bonusGameResultList = BonusGameResponseList;
                int bonusSpunCount = BlackboardUtils.FindValue<int>(null, "./bonusSpunCount");
                int startSlotIndex = BonusGameResponseList.Count;
                int slotMachineIndex = startSlotIndex == 2 ? 0 : 1;

                int responseIndex = 0;
                for (int slotIndex = startSlotIndex; slotIndex > 0; slotIndex--)
                {
                    var bonusResponse = bonusGameResultList[responseIndex++];
                    var thisSpinSymbolPerReelPerSpin =
                        bonusResponse.GetValue<List<Blackboard>>("symbolPerReelPerSpin");

                    if (thisSpinSymbolPerReelPerSpin.Count <= bonusSpunCount) continue;

                    var thisSpinSymbolPerReel = thisSpinSymbolPerReelPerSpin[bonusSpunCount].GetValue<List<int>>("value");
                    var thisSpinMultplierPerReel = bonusResponse.GetValue<List<Blackboard>>("cashMultiplierListPerSpin")[bonusSpunCount].GetValue<List<double>>("value");

                    var expectation = ContentCustomData.Instance.slotDataList[slotIndex].expectation;
                    int lockedReelCount = 0;
                    var payUpReelIndexList = bonusResponse
                   .GetValue<List<Blackboard>>("payUpReelIndexListPerSpin")[bonusSpunCount].GetValue<List<int>>("value");
                    bool isAbleToCalcSuperCash = CTCSuperCashSymbolBehaviour.IsAbleToCalcSuperCash(slotMachineList[slotMachineIndex], thisSpinMultplierPerReel, payUpReelIndexList, thisSpinSymbolPerReel);

                    for (int reelIndex = 0; reelIndex < thisSpinSymbolPerReel.Count; reelIndex++)
                        lockedReelCount += slotMachineList[slotMachineIndex].GetOverlaySymbol(new Cell(reelIndex, 0).GetHashCode()) != null ? 1 : 0;

                    for (int reelIndex = 0; reelIndex < thisSpinSymbolPerReel.Count; reelIndex++)
                    {
                        var symbolIndex = thisSpinSymbolPerReel[reelIndex];
                        var cell = new Cell(reelIndex, 0);

                        if (lockedReelCount == thisSpinSymbolPerReel.Count - 1) expectation.expectations[reelIndex] = true;
                        if (symbolIndex > BLANK_SYMBOL_INDEX) expectation.expectationSpots[reelIndex].Add(cell);

                        bool isNewCashSymbolLand = symbolIndex == CASH_SYMBOL_INDEX || (symbolIndex == SUPER_CASH_SYMBOL_INDEX && isAbleToCalcSuperCash);
                        lockedReelCount += isNewCashSymbolLand ? 1 : 0;
                    }
                    slotMachineIndex++;
                }
            });
        }

        private IEnumerator EmergeInitialLockedSymbols()
        {
            var initialMultiplierPerReel = BlackboardUtils.FindValue<List<double>>(null, "./bonus/response/intialMultiplierPerReel");
            var initialJackpotIndexPerReel = BlackboardUtils.FindValue<List<int>>(null, "./bonus/response/initialJackpotIndexPerReel");
            int bonusCombination = BlackboardUtils.FindValue<int>(null, "./bonus/response/bonusCombination");
            bool isDoubleSpin = (bonusCombination & (int)EFreeGameBonusType.DoubleSpin) != 0;
            lockedReelCountPerSlot = new List<int>();

            foreach (var lockedReelCountUI in lockedReelCountUIList)
            {
                lockedReelCountUI.text = 0.ToString();
                lockedReelCountPerSlot.Add(0);
            }

            var waitForSecond = new WaitForSeconds(waitTimePerInitialSymbolEmerge);
            var lowerSlotMachine = slotMachineList[slotMachineList.Count - 1];
            var topSlotMachine = slotMachineList[0];
            int lockedReelCount = 0;
            int currentFlyingSymbolCount = 0;
            for (int i = 0; i < initialMultiplierPerReel.Count; i++)
            {
                if (initialMultiplierPerReel[i] > 0 || initialJackpotIndexPerReel[i] >= 0)
                {
                    lockedReelCount++;
                    var symbolInfo = new SymbolInfo()
                    {
                        symbol = CASH_SYMBOL_INDEX,
                        customData = new Dictionary<string, object>(),
                    };

                    symbolInfo.customData.Add("Multiplier", initialMultiplierPerReel[i]);
                    symbolInfo.customData.Add("JackpotIndex", initialJackpotIndexPerReel[i]);
                    lowerSlotMachine.GetSymbol(i, 0).gameObject.SetActive(false);
                    var overlaySymbol = lowerSlotMachine.overlay.AddSymbol(i, 0, symbolInfo);
                    overlaySymbol.Apply();
                    overlaySymbol.Play("Lock");
                    lockedReelCountPerSlot[1]++;
                    lockedReelCountUIList[1].text = lockedReelCountPerSlot[1].ToString();

                    yield return waitForSecond;
                }
            }

            lockedReelCount = 0;
            for (int i = 0; i < initialMultiplierPerReel.Count; i++)
            {
                if (initialMultiplierPerReel[i] > 0 || initialJackpotIndexPerReel[i] >= 0)
                {
                    lockedReelCount++;
                    var overlaySymbol = lowerSlotMachine.GetOverlaySymbol(new Cell(i, 0).GetHashCode());

                    if (isDoubleSpin)
                    {
                        var flyingObj = cashCopyingFlyingPool.GetObject();
                        flyingObj.GetComponentInChildren<Animator>().SetBool("IsJackpot", initialJackpotIndexPerReel[i] >= 0);
                        var fromToController = flyingObj.GetComponentInChildren<DirectionalWeightPositionController>();
                        var targetTransform = topSlotMachine.GetSymbol(i, 0).transform;
                        fromToController.from = overlaySymbol.transform;
                        fromToController.to = targetTransform;
                        flyingObj.gameObject.SetActive(true);

                        var flyingObjBB = flyingObj.GetComponent<Blackboard>();
                        int reelIndex = i;
                        currentFlyingSymbolCount++;
                        GSManager.Instance.GetHandler("LB Cash Symbol Copy").Play();
                        StartCoroutine(CTCUtill.CallActionAfterDelay(() =>
                        {
                            var topOverlaySymbol = topSlotMachine.overlay.AddSymbol(reelIndex, 0, overlaySymbol.symbolInfo);
                            topSlotMachine.GetSymbol(reelIndex, 0).gameObject.SetActive(false);
                            topOverlaySymbol.Apply();
                            topOverlaySymbol.Play("Win");
                            flyingObj.GetComponent<PooledObject>().ReturnToPool();
                            lockedReelCountPerSlot[0]++;
                            lockedReelCountUIList[0].text = lockedReelCountPerSlot[0].ToString();

                            currentFlyingSymbolCount--;
                        }, copyingSymbolFlyingTime));
                    }
                    yield return waitForSecond;
                }



            }
            // for (int i = 0; i < lockedReelCountPerSlot.Count; i++) lockedReelCountPerSlot[i] = lockedReelCount;

            yield return new WaitUntil(() => currentFlyingSymbolCount == 0);
            ContentEvent.SendEvent(INIT_LOCKED_REELS_DONE_EVENT);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterEvent(INIT_SPIN_COUNT_UI_EVENT);
            UnRegisterEvent(SUBSTRACT_SPIN_COUNT_UI_EVENT);
            UnRegisterEvent(INIT_LOCKED_REELS_START_EVENT);
            UnRegisterEvent(ADD_LOCKED_REEL_COUNT_UI_EVENT);
            UnRegisterEvent(UPDATE_STOP_EFFECT_SPOTS_EVENT);
        }
    }
}