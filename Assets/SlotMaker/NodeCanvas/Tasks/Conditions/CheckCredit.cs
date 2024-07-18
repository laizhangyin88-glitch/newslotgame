using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Tasks.Condition
{
    public class CheckCredit : ConditionTask
    {
        protected override bool OnCheck()
        {
            int gameId = BlackboardUtils.GetOrCreateVariable<int>(null, "./game/gameId").value;
            if (globalStore.IsHaveLineSelect.ContainsKey(gameId))
            {
                int line = BlackboardUtils.GetOrCreateVariable<int>(ContentBlackboard.Get(), "./game/lineCount").value;
                long betCredit = BlackboardUtils.FindVariable<long>("./betCredit").value;
                var meCredit = BlackboardUtils.FindVariable<long>(null, "/me/credit");
                if (line * betCredit > meCredit.value)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
