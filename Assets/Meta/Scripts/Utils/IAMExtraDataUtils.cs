using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public static class IAMExtraDataUtils
    {
        public static Blackboard IAMExtraBB => BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "iamExtraData")?.value ?? null;

        public static long GetBossRaidersDealBaseAttack()
        {
            if (IAMExtraBB != null)
                return BlackboardUtils.FindVariable<long>(IAMExtraBB, "bossRaidersDealBaseAttackHp")?.value ?? 0L;
            return 0L;
        }
    }
}