using System.Collections.Generic;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;

namespace GameStudio.Slot.EDM
{
    public class EDMBonusSlotMachineController : FeatureModule
    {
        SlotMachine bonusSlotMachine;
        void Awake()
        {
            RegisterEvent("EDM_MERGE_BASEGAME_SLOTMACHINES", OnMergeBaseGameSlomachines);
            RegisterEvent("EDM_INITIALIZE_SLOTMACHINE_SB", OnInitializeSlotMachineSB);
        }

        private void OnMergeBaseGameSlomachines(EventData eventData)
        {
            bonusSlotMachine = BlackboardUtils.FindVariable<GameObject>("./bonusSlotMachine").value.GetComponent<SlotMachine>();
            List<int> initalSymbolIndexList = BlackboardUtils.FindVariable<List<int>>("./bonus/response/initalSymbolIndexList").value;
            List<long> initalCreditValueList = BlackboardUtils.FindVariable<List<long>>("./bonus/response/initalCreditValueList").value;

            for (int i = 0; i < bonusSlotMachine.reels.Count; i++)
            {
                BaseReel reel = bonusSlotMachine.reels[i];
                BaseSymbol symbol = reel.symbols[1];

                int index = initalSymbolIndexList[i];
                symbol.GetComponent<DefaultSymbolEventHandler>().DisableAllCachedObjects();
                symbol.symbolIndex = index;
                symbol.symbolMask = (int)ContentCustomData.GetSlotData(bonusSlotMachine.slotIndex).symbolMask.GetMask(index);

                if (index == 12)
                {
                    symbol.symbolInfo.customData = new Dictionary<string, object>();
                    symbol.symbolInfo.customData.Add("value", (long)initalCreditValueList[i]);
                }

                symbol.Change(symbol.symbolInfo);
                symbol.Apply();
            }
        }
        private void OnInitializeSlotMachineSB(EventData eventData)
        {
            bonusSlotMachine = BlackboardUtils.FindVariable<GameObject>("./bonusSlotMachine").value.GetComponent<SlotMachine>();

            for (int i = 0; i < bonusSlotMachine.reels.Count; i++)
            {
                BaseReel reel = bonusSlotMachine.reels[i];
                BaseSymbol symbol = reel.symbols[1];

                symbol.GetComponent<DefaultSymbolEventHandler>().DisableAllCachedObjects();
                symbol.symbolIndex = 11;
                symbol.symbolMask = (int)ContentCustomData.GetSlotData(bonusSlotMachine.slotIndex).symbolMask.GetMask(11);

                symbol.Change(symbol.symbolInfo);
                symbol.Apply();
            }
        }
    }
}