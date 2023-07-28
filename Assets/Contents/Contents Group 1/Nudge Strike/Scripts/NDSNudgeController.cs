using System.Collections;
using System.Collections.Generic;
using ParadoxNotion;
using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;

namespace GameStudio.Slot.NDS
{
    public class NDSNudgeController : FeatureController
    {
		private const string ON_SLOT_EVENT = "OnSlotEvent";
        protected override string ON_FEATURE_BEGIN_EVENT { get => "NudgeFeature"; }
        protected const string ON_CHECK_FEATURE_EVENT = "CheckNudgeFeature";
        protected override string ON_FEATURE_END_EVENT { get => "EndNudgeFeature"; }
        protected const string ON_CHECK_FEATURE_END_EVENT = "EndCheckNudgeFeature";
        private string isTriggeredNudgePath = "./customData/_isTriggeredNudge";
        private string nudgeTriggeredSlotIndexListPath = "./customData/_nudgeTirggeredSlotIndexList";
        private List<string> slotMachinePathList = new List<string>(new string[] {"./slotMachine01", "./slotMachine02", "./slotMachine03", "./slotMachine04", "./slotMachine05", "./slotMachine06", "./slotMachine07", "./slotMachine08"});
        private List<string> deckPathList = new List<string>(new string[] {"./spin/parent/parent/deck0", "./spin/parent/parent/deck1", "./spin/parent/parent/deck2", "./spin/parent/parent/deck3", "./spin/parent/parent/deck4", "./spin/parent/parent/deck5", "./spin/parent/parent/deck6", "./spin/parent/parent/deck7"});
        private List<int> wildNudgeDirection = new List<int>(new int[] {-1, 1});
        private List<int> reelActionIndex = new List<int>(new int[] {3, 4});
        private List<int> nudgeSlotIndexList = new List<int>();

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterEvent(ON_CHECK_FEATURE_EVENT, (EventData eventData) => StartCoroutine(OnCheckNudgeFeature()));
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterEvent(ON_CHECK_FEATURE_EVENT);
        }
        private IEnumerator OnCheckNudgeFeature()
        {
            OnStart();
            yield return StartCoroutine(CheckNudgeFeature());
            OnFinish();

            ContentEvent.SendEvent(ON_CHECK_FEATURE_END_EVENT);
        }
        private IEnumerator CheckNudgeFeature()
        {
            nudgeSlotIndexList = new List<int>();
            bool isTriggeredNudge = false;
            for (int slotIndex = 0; slotIndex < slotMachinePathList.Count; slotIndex++)
            {
                bool isSlotNudgeTriggered = false;
                Deck deck = ContentCustomData.GetSlotData(slotIndex).deck;
                GameObject slotMachine = BlackboardUtils.FindValue<GameObject>(null, slotMachinePathList[slotIndex]);

                for (int colIndex = 0; colIndex < 3; colIndex++)
                {
                    for (int rowIndex = 2; rowIndex < 5; rowIndex++)
                    {
                        var symbolInfo = deck.GetSymbol(colIndex, rowIndex);
                        int symbolIndex = symbolInfo.symbol;

                        if (CheckNudge(symbolIndex, rowIndex))
                        {
                            isTriggeredNudge = true;
                            isSlotNudgeTriggered = true;
                            nudgeSlotIndexList.Add(slotIndex);
                            break;
                        }
                    }
                    if (isSlotNudgeTriggered) break;
                }
            }
            BlackboardUtils.FindVariable<bool>(null, isTriggeredNudgePath).value = isTriggeredNudge;
            yield break;
        }
        protected override IEnumerator OnPlayCoroutine()
        {
            List<Blackboard> reelOutputTable = BlackboardUtils.FindValue<List<Blackboard>>(null, "./spin/parent/parent/response/reelOutputTable");
            List<Blackboard> multiplierOutputTable = BlackboardUtils.FindVariable<List<Blackboard>>(null, "./spin/parent/parent/response/multiplierOutputTable").value;
            List<bool> isNudgeWinList = BlackboardUtils.FindVariable<List<bool>>(null, "./bonus/response/isNudgeWinList").value;
            int spinIndex = BlackboardUtils.FindVariable<int>(null, "./customData/spinIndex").value;

            for (int index = 0; index < nudgeSlotIndexList.Count; index++)
            {
                int slotIndex = nudgeSlotIndexList[index];
                var e = new EventData("PlayWildReadyNudge", slotIndex);
			    MessageDispatcher.Dispatch(ON_SLOT_EVENT, e);
                GSManager.Instance.GetHandler("Nudge Direction").Play();
            }
            yield return new WaitForSeconds(2f);
            if (isNudgeWinList[spinIndex - 1])
            {
                GSManager.Instance.GetHandler("Nudge Win").Play();
            }
            else
            {
                GSManager.Instance.GetHandler("Nudge").Play();
            }
            for (int index = 0; index < nudgeSlotIndexList.Count; index++)
            {
                int slotIndex = nudgeSlotIndexList[index];
                List<Blackboard> multiplierOutputList = BlackboardUtils.FindVariable<List<Blackboard>>(multiplierOutputTable[slotIndex], "value").value;
                GameObject slotMachine = BlackboardUtils.FindValue<GameObject>(null, slotMachinePathList[slotIndex]);
                List<int> reelOutputList = BlackboardUtils.FindValue<List<int>>(reelOutputTable[slotIndex], "value");
                Deck deck = ContentCustomData.GetSlotData(slotIndex).deck;
                bool isSlotNudgeTriggered = false;

                // var e = new EventData("PlayWildReadyNudge", slotIndex);
			    // MessageDispatcher.Dispatch(ON_SLOT_EVENT, e);
                // yield return new WaitForSeconds(2f);
                for (int colIndex = 0; colIndex < 3; colIndex++)
                {
                    for (int rowIndex = 2; rowIndex < 5; rowIndex++)
                    {
                        var symbolInfo = deck.GetSymbol(colIndex, rowIndex);
                        int symbolIndex = symbolInfo.symbol;

                        if (CheckNudge(symbolIndex, rowIndex))
                        {
                            isSlotNudgeTriggered = true;
                            BaseReel reel = slotMachine.GetComponent<SlotMachine>().GetReel(colIndex);
                            BlackboardUtils.FindVariable<int>(reel.GetComponent<Blackboard>(), "nudgeCount").value = 1;
                            var movement = reel.movement;
                            movement.Play(reelActionIndex[symbolIndex]);
                            reelOutputList[colIndex] += wildNudgeDirection[symbolIndex];
                        }
                    }
                }

                if (isSlotNudgeTriggered)
                {
                    GameObject dividerController = BlackboardUtils.FindValue<GameObject>(slotMachine.GetComponent<Blackboard>(), "dividerController");
                    dividerController.GetComponent<NDSDividerController>().Nudge();
                    deck.stripIndices = reelOutputList;
                    ProcessUpdateDeck(deck, slotIndex);
                    ProcessUpdateDeckMultiplier(deck, multiplierOutputList);
                    var variable = BlackboardUtils.GetOrCreateVariable<Deck>(null, deckPathList[slotIndex]);
                    variable.value = deck;
                    var slotData = ContentCustomData.GetSlotData(slotIndex);
                    slotData.deck = deck;
                    // yield return new WaitForSeconds(1f);
                }
            }
            BlackboardUtils.FindVariable<List<int>>(null, nudgeTriggeredSlotIndexListPath).value = nudgeSlotIndexList;
            yield break;
        }

        private bool CheckNudge(int symbolIndex, int row)
        {
            if (symbolIndex != 0 && symbolIndex != 1)
            {
                return false;
            }

            if (symbolIndex == 0)
            {
                return row < 4;
            }
            else
            {
                return row > 2;
            }
        }

        private void ProcessUpdateDeck(Deck deck, int slotIndex)
        {
            var slotData = ContentCustomData.GetSlotData(slotIndex);
            int totalColumn = slotData.column;
            int totalRow = slotData.row;

            deck.deck = new List<List<SymbolInfo>>();
            deck.hitMap = new List<List<bool>>();
            var strips = GlobalReelStrips.Instance.GetReelStrips();
            for (int column = 0; column < totalColumn; ++column)
            {
                var hitReel = new List<bool>();
                var reel = new List<SymbolInfo>();
                var strip = strips.GetReelStrip(column);
                for (int row = 0; row < totalRow; ++row)
                {
                    int idx = strip.CalcIndex(deck.stripIndices[column] + row);
                    reel.Add(SlotUtils.GetSymbol(slotIndex, column, strip, idx));
                    hitReel.Add(false);
                }
                deck.deck.Add(reel);
                deck.hitMap.Add(hitReel);
            }
        }
        private void ProcessUpdateDeckMultiplier(Deck deck, List<Blackboard> multiplierOutputList)
        {
            for (int rowIndex = 0; rowIndex < 7; rowIndex++)
            {
                List<int> multiplierOutput = BlackboardUtils.FindVariable<List<int>>(multiplierOutputList[rowIndex], "value").value;
                for (int colIndex = 0; colIndex < 3; colIndex++)
                {
                    int multiplier = multiplierOutput[colIndex];

                    SymbolInfo symbol = deck.GetOriginalSymbol(colIndex, rowIndex);
                    symbol.multiplier = multiplier;
                }
            }
        }
    }
}
