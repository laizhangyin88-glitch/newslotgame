using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;

namespace SlotMaker
{
    public class HighestSymbolWinSoundView : MonoBehaviour
    {
        [SerializeField]
        private int slotIndex = 0;
        [SerializeField]
        private List<string> totalWinSoundList;
        [SerializeField]
        private List<int> minimumHitCountList;

        private const string WIN_EVENT = "OnWinEvent";
        private const string ON_TOTAL_WIN_EVENT = "TotalWin";
        
        private void OnEnable()
        {
            MessageDispatcher.Register(WIN_EVENT, PlayWinSound);
        }

        private void OnDisable()
        {
            MessageDispatcher.UnRegister(WIN_EVENT, PlayWinSound);
        }

        private void PlayWinSound(EventData eventData)
        {
            var symbolWinList = eventData.value as List<SymbolWin>;
            if (eventData.name != ON_TOTAL_WIN_EVENT || eventData.id != slotIndex || symbolWinList.Count <= 0)
                return;

            int highestSymbolIndex = int.MaxValue;
            foreach (var symbolWin in symbolWinList)
            {
                int symbolIndex = symbolWin.symbolIndex;
                int hitCount = symbolWin.hitCount;

                bool isValidIndex = symbolIndex < minimumHitCountList.Count && hitCount >= minimumHitCountList[symbolIndex];
                if (isValidIndex)
                    highestSymbolIndex = Mathf.Min(highestSymbolIndex, symbolIndex);
            }

            if (highestSymbolIndex < totalWinSoundList.Count)
                GSManager.Instance.GetHandler(totalWinSoundList[highestSymbolIndex]).Play();
        }
    }
}
