using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameStudio.Slot.HOC
{
    public class HOCBaseSpinController : FeatureController
    {
        [SerializeField] private List<BaseSlotMachine> slotMachineList;
        [SerializeField] private List<float> wildExpandingWaitDelayPerHeight = new List<float>();
        [SerializeField] private float infectWaitDelayPerSymbol = 0.5f;
        [SerializeField] private float highTurnToWildAnimTime = 2f;

        public const string SLOT_MACHINE_EVENT = "OnSlotEvent";
        public const string SPIN_SLOT_EVENT = "SpinSlotMachine";
        public const string STOP_SLOT_EVENT = "StopSlotMachine";


        public static long ThisSpinLinePay => BlackboardUtils.FindVariable<long>(null, "./spin/response/result/earnCredit").value;

        protected override string ON_FEATURE_BEGIN_EVENT => "ProcessStopSlotsFlow";

        protected override string ON_FEATURE_END_EVENT => "ProcessStopSlotsFlowDone";

        protected override IEnumerator OnPlayCoroutine()
        {
            var waitForSecondStorage = new WaitForSecondsStorage();
            Variable<bool> isSpinSkipped = BlackboardUtils.FindVariable<bool>(null, "./customData/isSpinSkipped");

            for (int i = 0; i < slotMachineList.Count; i++)
            {
                MessageDispatcher.Dispatch(SLOT_MACHINE_EVENT, new EventData(STOP_SLOT_EVENT, i));
                if (i == slotMachineList.Count - 1)
                    yield return new WaitUntil(() => slotMachineList[i].movement.spinState >= SpinState.Stopped);
                else
                    yield return new WaitUntil(() =>
                        slotMachineList[i].movement.spinState >= SpinState.Stopped || isSpinSkipped.value);
            }

            bool hasInfection = false;
            bool hasWildExpand = false;
            int maxExpandingCount = -1;
            for (int i = 0; i < slotMachineList.Count; i++)
            for (int colIndex = 0; colIndex < slotMachineList[i].ColumnCount; colIndex++)
            {
                int wildStackStartIndex = HOCFeatureUtils.GetRowIndexOfSymbolAtReel(slotMachineList[i], colIndex,
                    HOCFeatureUtils.WILD_SYMBOL_INDEX);
                if (wildStackStartIndex > -1)
                {
                    HOCFeatureUtils.StackWild(slotMachineList[i], colIndex);
                    hasWildExpand = true;
                    int expandCount = slotMachineList[0].RowCount - wildStackStartIndex;
                    if (expandCount > maxExpandingCount) maxExpandingCount = expandCount;
                }
            }

            if (hasWildExpand)
            {
                yield return waitForSecondStorage.GetWaitForSeconds(wildExpandingWaitDelayPerHeight[maxExpandingCount -1]);
            }

            int maxInfectionCount = -1;
            for (int i = 0; i < slotMachineList.Count; i++)
            {
                var slotMachine = slotMachineList[i];

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
    }
}
