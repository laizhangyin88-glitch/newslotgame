using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using ParadoxNotion;
using NodeCanvas.Framework;
using TMPro;
using System;

namespace GameStudio.Slot.FSF
{
    public class FSFCommunityFrameWinHandler : MonoBehaviour
    {
        [SerializeField]
        private BaseSlotMachine slotMachine;
        [SerializeField]
        private TextMeshProUGUI multiplierText;
        [SerializeField]
        private Animator multiplierAnimator;
        [SerializeField]
        private GameObject winFrame;

        private Dictionary<int, long> multiplierPerSymbolIndex;
        private List<Blackboard> communityUserResultList;
        protected long accumulatedMultiplier;
        protected int winCalcTryCount;

        public BaseSlotMachine SlotMachine { get => slotMachine; }
        public TextMeshProUGUI MultiplierText { get => multiplierText; }
        public GameObject WinFrame { get => winFrame; }
        public Animator MultiplierAnimator { get => multiplierAnimator; }
        public IReadOnlyList<Blackboard> CommunityUserResultList { get => communityUserResultList.AsReadOnly(); }
        public IReadOnlyDictionary<int, long> MultiplierPerSymbolIndex {
            get
            {
                if(multiplierPerSymbolIndex == null)
                {
                    var symbolList = BlackboardUtils.FindValue<List<int>>("./game/symbolMultiplierInfo/symbolList");
                    var multiplierList = BlackboardUtils.FindValue<List<long>>("./game/symbolMultiplierInfo/multiplierList");

                    multiplierPerSymbolIndex = new Dictionary<int, long>();
                for (int i = 0; i < symbolList.Count; i++) multiplierPerSymbolIndex.Add(symbolList[i], multiplierList[i]);
                }

                return multiplierPerSymbolIndex;
            }
        }


        public const int BLANK_SYMBOL_INDEX = 8;
        public const int PLUS_SPIN_SYMBOL_INDEX = 16;

        virtual public void OnEnable()
        {
            if(FSFCommunityGameController.IsOnCommunity) communityUserResultList = BlackboardUtils.FindValue<List<Blackboard>>(null, "./bonus/response/userGameResultList");
            accumulatedMultiplier = 0;
            multiplierText.text = "X0";
            winCalcTryCount = 0;
        }


        virtual public IEnumerator StartFrameWinFlow(Frame frame, long frameMultiplier)
        {
            long addedMultiplier = CommunityUserResultList[slotMachine.slotIndex].GetValue<List<long>>("symbolMultiplierPerSpin")[winCalcTryCount++];
            long newMultiplier = accumulatedMultiplier + addedMultiplier;
            accumulatedMultiplier = newMultiplier;
            multiplierText.text = string.Format(FSFSymbolEventHandler.MULTIPLIER_STRING_FORMAT, newMultiplier);
            MultiplierAnimator.SetTrigger("Increase");
            yield break;
        }
    }
}
