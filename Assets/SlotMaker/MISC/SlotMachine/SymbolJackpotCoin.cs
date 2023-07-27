namespace SlotMaker
{
    public class SymbolJackpotCoin : SymbolJackpotCoinBase<int>
    {
        protected override long GetCoinValue(long betCredit)
        {
            return betCredit * (long)coinValues[coinIndex];
        }
    }
}
