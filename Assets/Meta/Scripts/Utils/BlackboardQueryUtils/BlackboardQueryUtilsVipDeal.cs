using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{

public static partial class BlackboardQueryUtils
{
    public static Blackboard GetActiveVipDealInfo()
    {
        var enableVipDeal = BlackboardUtils.FindVariable<bool>("/values/misc/ENABLE_VIP_DEAL").value;
        if(enableVipDeal)
        {
            var vipDealInfo = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "vipDealInfo");

            if(vipDealInfo != null && vipDealInfo.value != null)
            {
                var startTimestamp = vipDealInfo.value.GetValue<long>("startTimestamp");
                var endTimestamp = vipDealInfo.value.GetValue<long>("endTimestamp");
                var currentTimestamp = TimeUtils.GetTimeStamp();

                if(currentTimestamp > startTimestamp && currentTimestamp <= endTimestamp)
                    return vipDealInfo.value;
            }
        }

        return null;
    }

    public static bool IsVipDealTierLock()
    {
        var meTier = TierUtils.GetMeTier();
        var minTier = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "values/tier/VIP_DEAL_MIN_TIER");

        if(meTier >= minTier.value)
            return false;

        return true;
    }
}

}

