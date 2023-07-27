using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using NodeCanvas.Framework;
using TMPro;
namespace SlotMaker
{
    public abstract class SymbolJackpotCoinBase<T> : MonoBehaviour
    {
        private const string SYMBOL_COIN_BASE_BET_PATH = "./game/symbolCoinBaseBet";
        private const string BET_CREDIT = "betCredit";
        private const string TEXT_FORMAT = "{0:SimpleNumber}";

        private Variable<long> symbolCoinBaseBetVar = null;

        [InfoBox("[ Constant Info ]\nCOIN_BASE_BET_PATH = \"./game/symbolCoinBaseBet\"\nTEXT_FORMAT = \"{0:SimpleNumber}\"")]
        public T[] coinValues;
        public TextMeshProUGUI coinText;
        public string coinTextString
        {
            get
            {
                return coinText.text;
            }
        }

        [ShowIf("@jackpotImage != null")]
        public int jackpotIndex;
        public Sprite[] jackpotSprites;
        public SpriteRenderer jackpotImage;
        public WeightRandomGeneratorBase randomGenerator;
        private long _coinCredit;
        public long coinCredit
        {
            get { return _coinCredit; }
            set { _coinCredit = value; }
        }
        public int coinIndex
        {
            get
            {
                return symbolInfo != null && symbolInfo.customData != null
                    ? (int) symbolInfo.customData["index"]
                    : _coinIndex;
            }
            set
            {
                if (symbolInfo != null) symbolInfo.customData = null;
                _coinIndex = value;
            }
        }
        private int _coinIndex = -1;
        private SymbolInfo symbolInfo;
        private enum CoinType
        {
            None,
            Coin,
            Jackpot
        };

        public void Clear(BaseSymbol symbol)
        {
            coinText.gameObject.SetActive(false);
            jackpotImage.gameObject.SetActive(false);
        }
        public void Change(BaseSymbol symbol)
        {
            symbolInfo = symbol.symbolInfo;
            if (SymbolMask.HasScatter2(symbolInfo))
            {
                if (symbolInfo.customData == null)
                {
                    symbolInfo.customData = new Dictionary<string, object>();
                    symbolInfo.customData.Add("index", randomGenerator.TakeOne());
                }
            }
            else
            {
                symbolInfo.customData = null;
                coinIndex = -1;
            }
        }

        // NOTE: It can be used in "OnApply Event"(BaseSymbol) instead of Apply()
        // Update just value of coin
        // If you want custom data, you can use 'symbolCoinBaseBet' BBParameter. (path is ./game/symbolCoinBaseBet)
        // If not, you just use betCredit in Content Blackboard.
        public void UpdateCoin(BaseSymbol symbol)
        {
            if (symbolCoinBaseBetVar == null)
            {
                var cb = ContentBlackboard.Get();
                symbolCoinBaseBetVar = BlackboardUtils.FindVariable<long>(null, SYMBOL_COIN_BASE_BET_PATH) ?? cb.GetVariable<long>(BET_CREDIT);
            }
            coinCredit = GetCoinValue(symbolCoinBaseBetVar.value);
        }
        public void Apply(BaseSymbol symbol)
        {
            symbolInfo = symbol.symbolInfo;
            CoinType coinType = CoinType.None;
            if (coinIndex != -1 && symbolInfo.link.isPivot)
            {
                UpdateCoin(symbol);
                if (!jackpotImage || coinIndex < jackpotIndex)
                {
                    coinType = CoinType.Coin;
                    SetText();
                }
                else
                {
                    coinType = CoinType.Jackpot;
                    jackpotImage.sprite = jackpotSprites[coinIndex - jackpotIndex];
                }
            }
            coinText.gameObject.SetActive(coinType == CoinType.Coin);
            if (jackpotImage)
                jackpotImage.gameObject.SetActive(coinType == CoinType.Jackpot);
        }
        private void SetText()
        {
            coinText.SetText(string.Format(StringTableUtils.customProvider, TEXT_FORMAT, coinCredit));
        }
        /// <summary>
        /// Convert and Get coinCredit to corresponding payout
        /// </summary>
        /// <param name="betCredit"></param>
        /// <returns>converted long</returns>
        protected abstract long GetCoinValue(long betCredit);
    }
}
