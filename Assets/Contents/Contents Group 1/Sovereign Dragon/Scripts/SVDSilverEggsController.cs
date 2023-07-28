using System.Collections;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace GameStudio.Slot.SVD
{
    public class SVDSilverEggsController : FeatureController
    {
        private const int MaxExpandType = 4;
        private const int FrontSymbolIndex = 1;
        private const int SilverEggSymbolIndex = 12;
        private const string BlankStateName = "SetBlankState";

        protected override string ON_FEATURE_BEGIN_EVENT => "StartSilverEggsController";
        protected override string ON_FEATURE_END_EVENT => "SilverEggsControllerDone";

        [SerializeField] private Blackboard mainFSMBB;

        protected override IEnumerator OnPlayCoroutine()
        {
            var currentStep = BlackboardUtils.FindVariable<Blackboard>(mainFSMBB, "_currentStep");
            var expandType = BlackboardUtils.FindVariable<int>(currentStep.value, "initialExpandType");

            if (expandType.value < MaxExpandType)
            {
                yield break;
            }

            var expandedSlotMachineGO =
                BlackboardUtils.FindVariable<GameObject>(mainFSMBB, "_slotMachineExpand");
            var reels = expandedSlotMachineGO.value.GetComponent<BaseSlotMachine>().reels;

            for (int reelIndex = 0; reelIndex < reels.Count; reelIndex++)
            {
                var symbols = reels[reelIndex].symbols;

                for (int symbolIndex = 0; symbolIndex < symbols.Count; symbolIndex++)
                {
                    if (symbolIndex != FrontSymbolIndex)
                    {
                        var symbol = symbols[symbolIndex];

                        if (symbol.symbolInfo.symbol == SilverEggSymbolIndex)
                        {
                            symbol.GetComponent<SVDSymbolEventHandler>().ChangeSate(BlankStateName);
                        }
                    }
                }
            }
        }
    }
}

