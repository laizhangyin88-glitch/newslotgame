using BagelCode.ClientModels;
using NodeCanvas.Framework;
using UnityEngine;
using SlotMaker;

namespace BagelCode.Scratcher
{
    [System.Serializable]
    public class ScratcherCellSymbol
    {
        public ScratcherSymbolType type;
        public int id;

        [System.NonSerialized]
        public long prize;
        [System.NonSerialized]
        public string value;
        [System.NonSerialized]
        public Sprite sprite;
        [System.NonSerialized]
        public string text;
        [System.NonSerialized]
        public Color color;
        [System.NonSerialized]
        public bool isHighWin;

        public ScratcherCellSymbol(Blackboard scratcherBB, Blackboard symbolBB)
        {
            type = symbolBB.GetValue<ScratcherSymbolType>("symbolType");
            id = symbolBB.GetValue<int>("symbolId");
            isHighWin = symbolBB.GetValue<bool>("isHighWin");

            prize = symbolBB.GetValue<long>("prize");
            bool isReward = scratcherBB.GetVariable<bool>("_isReward")?.value ?? false;
            if (isReward)
            {
                var scratcherRewardResultBB = BlackboardUtils.FindVariable<Blackboard>(scratcherBB, "_scratcherRewardResult")?.value;
                var rewardInfo = BlackboardUtils.FindVariable<Blackboard>(scratcherRewardResultBB, "rewardInfo")?.value;
                if (rewardInfo != null)
                {
                    prize = MultiplierUtils.GetRewardMultiplierValue(prize, rewardInfo, "scratcher");
                }
            }

            MakeScratcherCellSymbol();
        }

        public ScratcherCellSymbol(int makeSymbolID)
        {
            type = ScratcherSymbolType.SYMBOL;
            id = makeSymbolID;
            prize = 10000;
            isHighWin = false;

            MakeScratcherCellSymbol();
        }

        private void MakeScratcherCellSymbol()
        {
            ScratcherSymbolAsset symbolAsset = GetScratcherSymbolAsset();
            sprite = symbolAsset.sprite;
            text = symbolAsset.text;
            value = symbolAsset.value;
            color = symbolAsset.color;
        }

        public ScratcherSymbolAsset GetScratcherSymbolAsset()
        {
            return ScratcherCustomData.Instance.scratcherCellAssets.symbolAssets[id];
        }
    }
}
