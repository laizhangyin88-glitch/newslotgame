using System.Collections.Generic;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode
{
    public static partial class BlackboardQueryUtils
    {
        public static void ClearHogDealTotalPrizeInfo(Blackboard popupBB)
        {
            var prizeList = popupBB.GetVariable<List<long>>("prizeList");
            prizeList?.value.Clear();

            var jackpotTypeList = popupBB.GetVariable<List<HogDealJackpotType>>("jackpotTypeList");
            jackpotTypeList?.value.Clear();
        }

        public static void AddHogDealTotalPrizeInfo(Blackboard popupBB, long prize, HogDealJackpotType jackpotType)
        {
            var prizeListVar = BlackboardUtils.GetOrCreateVariable<List<long>>(popupBB, "prizeList");
            if (prizeListVar.value == null) prizeListVar.value = new List<long>();
            prizeListVar.value.Add(prize);

            var prizeTypeListVar = BlackboardUtils.GetOrCreateVariable<List<HogDealJackpotType>>(popupBB, "jackpotTypeList");
            if (prizeTypeListVar.value == null) prizeTypeListVar.value = new List<HogDealJackpotType>();
            prizeTypeListVar.value.Add(jackpotType);
        }

        public static int GetHogDealTotalPrizeCount(Blackboard popupBB)
        {
            var prizeListVar = BlackboardUtils.GetOrCreateVariable<List<long>>(popupBB, "prizeList");
            if (prizeListVar.value == null) prizeListVar.value = new List<long>();

            return prizeListVar.value.Count;
        }
    }
}
