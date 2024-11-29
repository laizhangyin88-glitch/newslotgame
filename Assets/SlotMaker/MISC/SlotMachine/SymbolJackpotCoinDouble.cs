using System;
namespace SlotMaker
{
    public class SymbolJackpotCoinDouble : SymbolJackpotCoinBase<double>
    {
        protected override long GetCoinValue(long betCredit)
        {
            var temp = (Convert.ToDouble(betCredit) * coinValues[coinIndex]);
            if(globalStore.nowGameID == 168)    ///168这款游戏的数值向上取整
            {
                return (long)Math.Ceiling(temp);
            }
            return Convert.ToInt64(temp); 
        }
    }
}
