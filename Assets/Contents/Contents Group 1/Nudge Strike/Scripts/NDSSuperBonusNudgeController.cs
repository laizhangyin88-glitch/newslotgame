using System.Collections;
using System.Collections.Generic;
using ParadoxNotion;
using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;

namespace GameStudio.Slot.NDS
{
    public class NDSSuperBonusNudgeController : FeatureController
    {
		private const string ON_SLOT_EVENT = "OnSlotEvent";
        protected override string ON_FEATURE_BEGIN_EVENT { get => "SuperBonusNudgeFeature"; }
        protected const string ON_CHECK_FEATURE_EVENT = "CheckSuperBonusNudgeFeature";
        protected override string ON_FEATURE_END_EVENT { get => "EndSuperBonusNudgeFeature"; }
        protected const string ON_CHECK_FEATURE_END_EVENT = "EndCheckSuperBonusNudgeFeature";
        private string isTriggeredNudgePath = "./customData/_isTriggeredNudge";
        private string nudgeTriggeredSlotIndexListPath = "./customData/_nudgeTirggeredSlotIndexList";
        private List<string> slotMachinePathList = new List<string>(new string[] {"./slotMachine01", "./slotMachine02", "./slotMachine03", "./slotMachine04", "./slotMachine05", "./slotMachine06", "./slotMachine07", "./slotMachine08"});
        private List<string> deckPathList = new List<string>(new string[] {"./spin/deck0", "./spin/deck1", "./spin/deck2", "./spin/deck3", "./spin/deck4", "./spin/deck5", "./spin/deck6", "./spin/deck7"});
        private List<int> nudgeSlotIndexList = new List<int>();
        private List<List<int>> nudgeDirListForEvent = new List<List<int>>();
        private const int DOWN = 3;
        private const int UP = 4;

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
            nudgeDirListForEvent = new List<List<int>>();
            bool isTriggeredNudge = false;
            for (int slotIndex = 0; slotIndex < slotMachinePathList.Count; slotIndex++)
            {
                bool isSlotNudgeTriggered = false;
                nudgeDirListForEvent.Add(new List<int>());
                Deck deck = ContentCustomData.GetSlotData(slotIndex).deck;
                GameObject slotMachine = BlackboardUtils.FindValue<GameObject>(null, slotMachinePathList[slotIndex]);

                for (int colIndex = 0; colIndex < 3; colIndex++)
                {
                    bool isAppearWild = false;
                    int nudgeCount = 0;
                    int nudgeDirection = 0;
                    for (int rowIndex = 2; rowIndex < 5; rowIndex++)
                    {
                        var symbolInfo = deck.GetSymbol(colIndex, rowIndex);
                        int symbolIndex = symbolInfo.symbol;

                        if (symbolIndex != 8)
                        {
                            nudgeCount++;
                        }
                        else
                        {
                            isAppearWild = true;
                            if (nudgeCount != 0)
                            {
                                nudgeDirection = UP;
                            }
                            else
                            {
                                nudgeDirection = DOWN;
                            }
                        }
                    }
                    if (nudgeCount == 0) nudgeDirection = 0;
                    nudgeDirListForEvent[slotIndex].Add(nudgeDirection);
                    if (isAppearWild && nudgeCount > 0 && !isSlotNudgeTriggered)
                    {
                        isTriggeredNudge = true;
                        isSlotNudgeTriggered = true;
                        nudgeSlotIndexList.Add(slotIndex);
                    }
                }
            }
            BlackboardUtils.FindVariable<bool>(null, isTriggeredNudgePath).value = isTriggeredNudge;
            yield break;
        }
        protected override IEnumerator OnPlayCoroutine()
        {
            List<Blackboard> reelOutputTable = BlackboardUtils.FindValue<List<Blackboard>>(null, "./spin/response/reelOutputTable");
            List<Blackboard> multiplierOutputTable = BlackboardUtils.FindVariable<List<Blackboard>>(null, "./spin/response/multiplierOutputTable").value;

            for (int index = 0; index < nudgeSlotIndexList.Count; index++)
            {
                int slotIndex = nudgeSlotIndexList[index];
                var e = new EventData<List<int>>("PlayWildReadySuperBonusNudge", slotIndex, nudgeDirListForEvent[slotIndex]);
			    MessageDispatcher.Dispatch(ON_SLOT_EVENT, e);
            }
            yield return new WaitForSeconds(2f);
            for (int index = 0; index < nudgeSlotIndexList.Count; index++)
            {
                int slotIndex = nudgeSlotIndexList[index];
                List<Blackboard> multiplierOutputList = BlackboardUtils.FindVariable<List<Blackboard>>(multiplierOutputTable[slotIndex], "value").value;
                GameObject slotMachine = BlackboardUtils.FindValue<GameObject>(null, slotMachinePathList[slotIndex]);
                List<int> reelOutputList = BlackboardUtils.FindValue<List<int>>(reelOutputTable[slotIndex], "value");
                Deck deck = ContentCustomData.GetSlotData(slotIndex).deck;
                bool isSlotNudgeTriggered = false;

                // var e = new EventData<List<int>>("PlayWildReadySuperBonusNudge", slotIndex, nudgeDirListForEvent[slotIndex]);
			    // MessageDispatcher.Dispatch(ON_SLOT_EVENT, e);
                // yield return new WaitForSeconds(2f);

                for (int colIndex = 0; colIndex < 3; colIndex++)
                {
                    bool isAppearWild = false;
                    int nudgeDirection = 0;
                    int nudgeCount = 0;
                    int wildMultiplier = 1;
                    for (int rowIndex = 2; rowIndex < 5; rowIndex++)
                    {
                        var symbolInfo = deck.GetSymbol(colIndex, rowIndex);
                        int symbolIndex = symbolInfo.symbol;

                        if (symbolIndex != 8)
                        {
                            nudgeCount++;
                        }
                        else
                        {
                            isAppearWild = true;
                            wildMultiplier = symbolInfo.multiplier;
                            if (nudgeCount != 0)
                            {
                                nudgeDirection = UP;
                            }
                            else
                            {
                                nudgeDirection = DOWN;
                            }
                        }
                    }
                    if (isAppearWild && nudgeCount > 0)
                    {
                        isSlotNudgeTriggered = true;
                        BaseReel reel = slotMachine.GetComponent<SlotMachine>().GetReel(colIndex);
                        BlackboardUtils.FindVariable<int>(reel.GetComponent<Blackboard>(), "nudgeCount").value = nudgeCount;
                        var movement = reel.movement;
                        movement.Play(nudgeDirection);
                        reelOutputList[colIndex] += nudgeDirection == DOWN ? -nudgeCount : +nudgeCount;
                        NudgeMultiplier(multiplierOutputList, wildMultiplier, colIndex);
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
        private void NudgeMultiplier(List<Blackboard> multiplerList, int wildMultiplier, int reelIndex)
        {
            for (int i = 2; i < 5; i++)
            {
                BlackboardUtils.FindVariable<List<int>>(multiplerList[i], "value").value[reelIndex] = wildMultiplier;
            }
        }
    }
}
