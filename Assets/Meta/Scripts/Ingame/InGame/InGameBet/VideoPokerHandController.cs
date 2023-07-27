using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
	public class VideoPokerHandController : MonoBehaviour 
	{
        Variable<int> handsGroupIndex;
        Variable<List<int>> handList;
        Variable<int> handCount;
		Variable<bool> isGameSpin;
        Variable<SpinType> spinType;

        void Awake()
        {
            handsGroupIndex = BlackboardUtils.GetOrCreateVariable<int>("./handsGroupIndex");
            handList        = BlackboardUtils.FindVariable<List<int>>("./game/handList");
            handCount       = BlackboardUtils.GetOrCreateVariable<int>("./handCount");
            isGameSpin      = BlackboardUtils.FindVariable<bool>("./isGameSpin");
            spinType        = BlackboardUtils.FindVariable<SpinType>("./spinType");
        }

        public void OnUpdateBetCredit(long credit)
        {
        	if (isGameSpin.value && (spinType.value != SpinType.BuyABonus))
            {
                if (handsGroupIndex.value != (handList.value.Count - 1))
                {
                    handsGroupIndex.value = handList.value.Count - 1;
                    UpdateHandsGroup();
                }
            }
        }

        public void NextHand()
        {
            if (isGameSpin.value)
                return;

            ++handsGroupIndex.value;
            if (handsGroupIndex.value > (handList.value.Count - 1))
                handsGroupIndex.value = 0;
            UpdateHandsGroup();
        }

        void UpdateHandsGroup()
        {
            handCount.value = handList.value[handsGroupIndex.value];
            
            MessageDispatcher.Dispatch("OnContentEvent", new EventData<int>("UpdateHandsGroup", handsGroupIndex.value));
            MessageDispatcher.Dispatch("OnContentEvent", new EventData<int>("UpdateHandsCount", handList.value[handsGroupIndex.value]));
        }
	}
}
