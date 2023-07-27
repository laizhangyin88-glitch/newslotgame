using System;
using System.Collections;
using System.Collections.Generic;
using BagelCode;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEditor;
using UnityEngine;

namespace GameStudio.Slot.HOC
{
    public class HOCFreeSpinController : FeatureController
    {
        [SerializeField] private List<BaseSlotMachine> slotMachineList;
        [SerializeField] private List<ObjectPool> wildFlyingObjPoolList;
        [SerializeField] private Animator freeSpinWinMultiplier;

        [SerializeField] private float additionalReelStopDelayForFeature = 1.5f;
        [SerializeField] private float wildStopEffectDelay = 1f;
        [SerializeField] private List<float> wildExpandingWaitDelayPerHeight = new List<float>();
        [SerializeField] private float wildCopyFlyingTime = 1f;
        [SerializeField] private float wildLandTimeFromFlyingStart = 0.7f;
        [SerializeField] private float infectWaitDelayPerSymbol = 0.5f;
        [SerializeField] private float highTurnToWildAnimTime = 2f;

        public const string WILD_EXPANDING_FLAG_LIST_VARIABLE_NAME = "wildExpandingFlagListPerSlot";

        public const string UPDATE_ALL_WIN_MULTIPLIER = "UpdateAllWinMultiplier";
        public const string UPDATE_ALL_WIN_MULTIPLIER_DONE = "UpdateAllWinMultiplierDone";
        public const string WILD_COPY_SHAKE_ANIM_EVENT = "StartShake";

        protected override string ON_FEATURE_BEGIN_EVENT => "ProcessFreeSpinStopSlotsFlow";

        protected override string ON_FEATURE_END_EVENT => "ProcessFreeSpinStopSlotsFlowDone";

        protected override IEnumerator OnPlayCoroutine()
        {
            var waitForSecondStorage = new WaitForSecondsStorage();
            InitWildExpandingFlagList();
            HOCFeatureUtils.UpdateStopEffectSpots(slotMachineList);
            HOCFeatureUtils.UpdateStopDelaysPerSlot(slotMachineList, additionalReelStopDelayForFeature);

            var wildExpandFlagListPerSlot = ContentCustomData.Instance.GetComponent<Blackboard>()
                .GetValue<List<List<bool>>>(WILD_EXPANDING_FLAG_LIST_VARIABLE_NAME);

            List<Action> actionBuffer = new List<Action>();
            Variable<bool> isSpinSkipped = BlackboardUtils.FindVariable<bool>(null, "./customData/isSpinSkipped");
            int currentFlyingObjCount = 0;
            int currentExpandingObjCount = 0;
            for (int i = 0; i < slotMachineList.Count; i++)
            {
                var slotMachine = slotMachineList[i];
                var wildExpandFlagList = wildExpandFlagListPerSlot[i];
                for (int j = 0; j < wildExpandFlagList.Count; j++) wildExpandFlagList[j] = false;
                MessageDispatcher.Dispatch(HOCBaseSpinController.SLOT_MACHINE_EVENT,
                    new EventData(HOCBaseSpinController.STOP_SLOT_EVENT, i));

                bool needToWildExpand = false;
                int wildStackStartIndex = -1;
                for (int reelIndex = 0; reelIndex < slotMachineList[i].ColumnCount; reelIndex++)
                {
                    wildStackStartIndex = HOCFeatureUtils.GetRowIndexOfSymbolAtReel(slotMachine, reelIndex,
                        HOCFeatureUtils.WILD_SYMBOL_INDEX);

                    needToWildExpand = wildStackStartIndex > -1;
                    if (needToWildExpand && i > 0)
                    {
                        int wildCopyStartIndex = int.MaxValue;
                        for (int previousSlotIndex = i - 1; previousSlotIndex >= 0; previousSlotIndex--)
                        {
                            int wildCopyStartIndexFromPrevious =     HOCFeatureUtils.GetRowIndexOfSymbolAtReel(slotMachineList[previousSlotIndex],
                                reelIndex, HOCFeatureUtils.WILD_SYMBOL_INDEX);

                            wildCopyStartIndex =
                                wildCopyStartIndexFromPrevious > -1 &&
                                wildCopyStartIndexFromPrevious < wildCopyStartIndex
                                    ? wildCopyStartIndexFromPrevious
                                    : wildCopyStartIndex;
                        }

                        needToWildExpand = wildCopyStartIndex > slotMachine.RowCount || (wildStackStartIndex < wildCopyStartIndex && wildCopyStartIndex < slotMachine.RowCount );
                    }

                    if (needToWildExpand)
                    {
                        int expandCount = slotMachine.RowCount - wildStackStartIndex;
                        float wildExpandingWaitDelay = wildExpandingWaitDelayPerHeight[expandCount - 1];

                        yield return new WaitUntil(() =>
                            slotMachine.reels[reelIndex].movement.spinState >= SpinState.Stopped);

                        int targetReelIndex = reelIndex;
                        currentExpandingObjCount++;
                        StartCoroutine(CallActionAfterDelay(() => { currentExpandingObjCount--; },
                            wildStopEffectDelay + wildExpandingWaitDelay));
                        StartCoroutine(CallActionAfterDelay(() => { HOCFeatureUtils.StackWild(slotMachine, targetReelIndex);},wildStopEffectDelay)) ;

                        int thisColWildStackStartIndex = wildStackStartIndex;
                        // actions to do after all expanding and flying done
                        actionBuffer.Add(() =>
                        {
                            for (int rowOffset = thisColWildStackStartIndex; rowOffset < slotMachine.RowCount; rowOffset++)
                                slotMachine.RemoveOverlaySymbol(new Cell(targetReelIndex, rowOffset).GetHashCode());
                        });

                        if (i < slotMachineList.Count - 1)
                        {
                            int slotIndex = i;
                            currentFlyingObjCount++;
                            StartCoroutine(HOCFeatureUtils.CallActionAfterDelay(() =>
                            {
                                StartCoroutine(CopyWild(slotIndex, thisColWildStackStartIndex, targetReelIndex,
                                    wildCopyFlyingTime, () => { currentFlyingObjCount--;}));
                            }, wildExpandingWaitDelay+wildStopEffectDelay));
                        }
                    }
                }

                Variable<bool> hasWildCopyInFS = BlackboardUtils.FindVariable<bool>(null, "./customData/hasWildCopyInFS");
                if (i == slotMachineList.Count - 1)
                {
                    yield return new WaitUntil(() => slotMachineList[i].movement.spinState == SpinState.Stopped);

                    yield return new WaitUntil(() => currentFlyingObjCount == 0 && currentExpandingObjCount == 0);
                }
                else
                {
                    // wait if last column has expanding
                    if (needToWildExpand)
                    {
                        // if wild is 1x1
                        if (wildStackStartIndex == slotMachine.RowCount - 1)
                            yield return waitForSecondStorage.GetWaitForSeconds(
                                wildStopEffectDelay + additionalReelStopDelayForFeature -1);
                        else
                        {
                            yield return waitForSecondStorage.GetWaitForSeconds(
                                wildStopEffectDelay + additionalReelStopDelayForFeature);
                        }
                    }

                    yield return new WaitUntil(() =>
                        slotMachineList[i].movement.spinState >= SpinState.Stopped ||
                        (isSpinSkipped.value && !hasWildCopyInFS.value));
                }
            }


            bool hasInfection = false;
            int maxInfectionCount = -1;
            foreach (var action in actionBuffer)
            {
                action();
            }
            actionBuffer.Clear();

            for (int i = 0; i < slotMachineList.Count; i++)
            {
                var slotMachine = slotMachineList[i];
                var overlaiedSymbolList =
                    HOCFeatureUtils.TurnOverlaySymbolToSlotSymbol(slotMachine, HOCFeatureUtils.WILD_SYMBOL_INDEX);

                overlaiedSymbolList.ForEach((symbol) =>
                {
                    symbol.Change(symbol.symbolInfo);
                    symbol.Apply();
                    symbol.Play("Skip");
                    slotMachine.overlay.RemoveSymbol(symbol.column, symbol.row);
                });

                overlaiedSymbolList =
                    HOCFeatureUtils.TurnOverlaySymbolToSlotSymbol(slotMachine, HOCFeatureUtils.BLANK_SYMBOL_INDEX);

                overlaiedSymbolList.ForEach((symbol) =>
                {
                    symbol.Change(symbol.symbolInfo);
                    symbol.Apply();
                    symbol.Play("Skip");
                    slotMachine.overlay.RemoveSymbol(symbol.column, symbol.row);
                });

                Queue<int> visitRequireReelIndexQueue = new Queue<int>();
                for (int reelIndex = 0; reelIndex < slotMachine.ColumnCount; reelIndex++)
                {
                    if (HOCFeatureUtils.GetRowIndexOfSymbolAtReel(slotMachine, reelIndex,
                            HOCFeatureUtils.WILD_SYMBOL_INDEX) > -1)
                        visitRequireReelIndexQueue.Enqueue(reelIndex);
                }

                while (visitRequireReelIndexQueue.Count > 0)
                {
                    var reelIndex = visitRequireReelIndexQueue.Dequeue();
                    int wildStartIndex = HOCFeatureUtils.GetRowIndexOfSymbolAtReel(slotMachine, reelIndex,
                        HOCFeatureUtils.WILD_SYMBOL_INDEX);

                    int highStartIndex = HOCFeatureUtils.GetRowIndexOfSymbolAtReelInverseOrder(slotMachine,
                        reelIndex,
                        HOCFeatureUtils.HIGH_SYMBOL_INDEX);

                    if (highStartIndex >= wildStartIndex - 1)
                    {
                        StartCoroutine(HOCFeatureUtils.TurnHighToWildOfColumn(slotMachine, reelIndex,
                            infectWaitDelayPerSymbol));
                        int highStackCount = HOCFeatureUtils.GetRowIndexOfSymbolAtReelInverseOrder(slotMachine,
                            reelIndex,
                            HOCFeatureUtils.HIGH_SYMBOL_INDEX);
                        ;
                        if (highStackCount + 1 > maxInfectionCount) maxInfectionCount = highStackCount + 1;
                    }

                    wildStartIndex = HOCFeatureUtils.GetRowIndexOfSymbolAtReel(slotMachine, reelIndex,
                        HOCFeatureUtils.WILD_SYMBOL_INDEX);

                    var leftIndex = reelIndex - 1;
                    var rightIndex = reelIndex + 1;

                    if (leftIndex > -1)
                    {
                        int highStackStartIndex = HOCFeatureUtils.GetRowIndexOfSymbolAtReel(slotMachine,
                            leftIndex,
                            HOCFeatureUtils.HIGH_SYMBOL_INDEX);
                        int highStackEndIndex = HOCFeatureUtils.GetRowIndexOfSymbolAtReelInverseOrder(slotMachine,
                            leftIndex,
                            HOCFeatureUtils.HIGH_SYMBOL_INDEX);

                        if (HOCFeatureUtils.IsInRange(wildStartIndex,highStackStartIndex,highStackEndIndex))
                        {
                            StartCoroutine(HOCFeatureUtils.TurnHighToWildOfColumn(slotMachine, leftIndex,
                                infectWaitDelayPerSymbol));
                            visitRequireReelIndexQueue.Enqueue(leftIndex);
                            hasInfection = true;
                            if (highStackEndIndex + 1 > maxInfectionCount)
                                maxInfectionCount = highStackEndIndex + 1;
                        }
                    }

                    if (rightIndex < slotMachine.ColumnCount)
                    {
                        int highStackStartIndex = HOCFeatureUtils.GetRowIndexOfSymbolAtReel(slotMachine,
                            rightIndex,
                            HOCFeatureUtils.HIGH_SYMBOL_INDEX);
                        int highStackEndIndex = HOCFeatureUtils.GetRowIndexOfSymbolAtReelInverseOrder(slotMachine,
                            rightIndex,
                            HOCFeatureUtils.HIGH_SYMBOL_INDEX);

                        if (HOCFeatureUtils.IsInRange(wildStartIndex,highStackStartIndex,highStackEndIndex))
                        {
                            StartCoroutine(HOCFeatureUtils.TurnHighToWildOfColumn(slotMachine, rightIndex,
                                infectWaitDelayPerSymbol));
                            visitRequireReelIndexQueue.Enqueue(rightIndex);
                            hasInfection = true;
                            if (highStackEndIndex + 1 > maxInfectionCount)
                                maxInfectionCount = highStackEndIndex + 1;
                        }
                    }
                }
            }

            if (hasInfection)
            {
                var infectionSound = GSManager.Instance.GetHandler("Wild Blink");
                infectionSound.Play();
                yield return waitForSecondStorage.GetWaitForSeconds((maxInfectionCount - 1) * infectWaitDelayPerSymbol +
                                                                    highTurnToWildAnimTime);
                infectionSound.Stop();
            }
        }

        private List<List<bool>> InitWildExpandingFlagList()
        {
            var customDataBB = ContentCustomData.Instance.GetComponent<Blackboard>();
            var wildExpandingFlagListPerSlot = new List<List<bool>>();

            for (int i = 0; i < slotMachineList.Count; i++)
            {
                var flagList = new List<bool>();
                for (int j = 0; j < slotMachineList[i].ColumnCount; j++) flagList.Add(false);

                wildExpandingFlagListPerSlot.Add(flagList);
            }
            BlackboardUtils.SetOrCreateValue(customDataBB,WILD_EXPANDING_FLAG_LIST_VARIABLE_NAME, wildExpandingFlagListPerSlot);
            return wildExpandingFlagListPerSlot;
        }

        public static IEnumerator CallActionAfterDelay(Action func, float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            func();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterEvent(UPDATE_ALL_WIN_MULTIPLIER, (eventData) =>
            {
                var multiplier = BlackboardUtils.FindValue<long>("./bonus/response/winMultiplier");

                freeSpinWinMultiplier.gameObject.SetActive(false);
                freeSpinWinMultiplier.gameObject.SetActive(true);
                freeSpinWinMultiplier.SetInteger("Multiplier", Convert.ToInt32(multiplier));
                StartCoroutine(CallActionAfterDelay(() => { ContentEvent.SendEvent(UPDATE_ALL_WIN_MULTIPLIER_DONE); },
                    0.1f)
                );
            });
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterEvent(UPDATE_ALL_WIN_MULTIPLIER);
        }

        private IEnumerator CopyWild(int startSlotIndex, int wildStackStartIndex, int reelIndex, float wildCopyFlyingTime, Action endCallBack )
        {
            bool isAllFlyingDone = false;
            for (int slotIndex = startSlotIndex; slotIndex < slotMachineList.Count - 1; slotIndex++)
            {
                var previousSlot = slotMachineList[slotIndex];
                var targetSlot = slotMachineList[slotIndex + 1];
                var flyingObject =
                    wildFlyingObjPoolList[targetSlot.RowCount - wildStackStartIndex - 1]
                        .GetObject();

                StartCoroutine(HOCFeatureUtils.CopyStackedWild(previousSlot, targetSlot, reelIndex,
                    wildStackStartIndex, flyingObject.gameObject, wildCopyFlyingTime));

                var targetSlotIndex = slotIndex;
                StartCoroutine(HOCFeatureUtils.CallActionAfterDelay(
                    () => {
                        BlackboardUtils.GetOrCreateVariable<int>("./customData/wildExpandingTargetIndex")
                            .value = targetSlotIndex + 1;
                        ContentEvent.SendEvent(WILD_COPY_SHAKE_ANIM_EVENT);
                        GSManager.Instance.GetHandler("Wild Symbol Land FG").Play();
                    },
                    wildLandTimeFromFlyingStart));

                yield return new WaitForSeconds(wildCopyFlyingTime);
            }

            endCallBack();
        }
    }

    public class WaitForSecondsStorage
    {
        private Dictionary<float, WaitForSeconds> waitForSecondsMap = new Dictionary<float, WaitForSeconds>();

        public WaitForSeconds GetWaitForSeconds(float waitTime)
        {
            if (waitForSecondsMap.ContainsKey(waitTime)) return waitForSecondsMap[waitTime];

            var waitForSeconds = new WaitForSeconds(waitTime);
            waitForSecondsMap.Add(waitTime, waitForSeconds);
            return waitForSeconds;
        }
    }
}
