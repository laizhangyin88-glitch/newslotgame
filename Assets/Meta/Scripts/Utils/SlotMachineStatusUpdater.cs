using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{

    public class SlotMachineStatusUpdater : MonoBehaviour
    {
        private const string IS_SPIN = "isSpin";

        public void Init()
        {
            BlackboardUtils.SetOrCreateValue<bool>(MainBlackboard.Get(), IS_SPIN, false);
        }

        public void BeginTurn()
        {
            BlackboardUtils.SetOrCreateValue<bool>(MainBlackboard.Get(), IS_SPIN, true);
        }

        public void EndTurn()
        {
            BlackboardUtils.SetOrCreateValue<bool>(MainBlackboard.Get(), IS_SPIN, false);
        }
    }
}