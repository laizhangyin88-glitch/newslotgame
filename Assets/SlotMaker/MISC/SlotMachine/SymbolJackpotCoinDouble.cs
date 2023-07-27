using System;
namespace SlotMaker
{
    public class SymbolJackpotCoinDouble : SymbolJackpotCoinBase<double>
    {
        protected override long GetCoinValue(long betCredit)
        {
            return Convert.ToInt64(Convert.ToDouble(betCredit) * coinValues[coinIndex]);
        }
    }
}
