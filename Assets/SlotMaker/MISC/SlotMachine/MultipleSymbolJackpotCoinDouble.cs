using System;
namespace SlotMaker
{
    [System.Serializable]
    public class CoinValueDoubleInfo
    {
        public double[] coinValues;
    }
    public class MultipleSymbolJackpotCoinDouble : SymbolJackpotCoinBase<CoinValueDoubleInfo>
    {
        public int coinValuesIndex;
        protected override long GetCoinValue(long betCredit)
        {
            return Convert.ToInt64((Convert.ToDouble(betCredit) * coinValues[coinValuesIndex].coinValues[coinIndex]));
        }
    }
}
