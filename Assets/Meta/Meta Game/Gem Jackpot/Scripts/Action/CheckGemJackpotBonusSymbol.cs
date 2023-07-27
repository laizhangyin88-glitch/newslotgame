using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/Meta Games/GemJackpot")]
    public class CheckGemJackpotBonusSymbol : ConditionTask
    {
        protected override string info
        {
            get { return "Check GemJackpot Bonus Symbol"; }
        }

        protected override bool OnCheck()
        {
            return GemJackpotUtils.CheckConsecutiveBonusSymbol(4);
        }
    }
}