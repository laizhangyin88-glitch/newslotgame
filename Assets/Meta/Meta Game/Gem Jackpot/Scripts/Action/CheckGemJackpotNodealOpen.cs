using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/Meta Games/GemJackpot")]
    public class CheckGemJackpotNodealOpen : ConditionTask
    {
        protected override string info
        {
            get { return "Check GemJackpot NodealOpen"; }
        }

        protected override bool OnCheck()
        {
            return BlackboardQueryUtils.CheckGemJackPotNoDealOpen();
        }
    }
}