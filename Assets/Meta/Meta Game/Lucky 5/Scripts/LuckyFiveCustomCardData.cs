using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;
using SlotMaker.Json;

namespace BagelCode.LuckyFive
{
    public class LuckyFiveCustomCardData : MonoWeakSingleton<LuckyFiveCustomCardData>
    {
        public LuckyFiveCardAssets cardAssets;

        void Awake()
        {
            var find = Instance;
            // BlackboardUtils.SetOrCreateValue<Blackboard>(MainBlackboard.Get(), "customData", GetComponent<Blackboard>());
        }
    }
}
