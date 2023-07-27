using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using TMPro;

namespace GameStudio.Slot.FHL
{
    public class FHLSymbolCoinController : MonoBehaviour
    {
        public Double[] coinValues;
        public WeightRandomGeneratorBase randomGenerator;
        public ContextTextMeshProUGUI coinText;
        public List<ContextTextMeshProUGUI> coinTextList;
        public bool useCoinTextList = false;
        public int coinIndex;
        private const string TEXT_FORMAT = "{0:SimpleNumber}";
        private const string SYMBOL_COIN_BASE_BET_PATH = "./game/symbolCoinBaseBet";
        private const string BET_CREDIT = "betCredit";
        public void Apply(BaseSymbol symbol)
        {
            var cb = ContentBlackboard.Get();
            var symbolCoinBaseBetVar = BlackboardUtils.FindVariable<long>(null, SYMBOL_COIN_BASE_BET_PATH) ?? cb.GetVariable<long>(BET_CREDIT);
            long betCredit = symbolCoinBaseBetVar.value;
            SymbolInfo symbolInfo = symbol.symbolInfo;
            if (symbolInfo.customData == null)
            {
                coinIndex = randomGenerator.TakeOne();
            }
            else
            {
                coinIndex = (int) symbolInfo.customData["index"];
            }
            if (coinIndex >= coinValues.Length)
            {
                // coinIndex = randomGenerator.TakeOne();
                Debug.Log("index : " + coinIndex);
            }
            if (useCoinTextList)
            {
                coinText = coinTextList[CalCoinGroupIndex(coinIndex)];
                for (int i = 0; i < coinTextList.Count; i++)
                {
                    coinTextList[i].gameObject.SetActive(false);
                }
            }
            coinText.gameObject.SetActive(true);
            long coinCredit = Convert.ToInt64(Convert.ToDouble(betCredit) * coinValues[coinIndex]);
            coinText.SetText(string.Format(StringTableUtils.customProvider, TEXT_FORMAT, coinCredit));
        }

        private int CalCoinGroupIndex(int coinIndex)
        {
            if (coinIndex < 6)
            {
                return 0;
            }
            return 1;
        }
    }
}