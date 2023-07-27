using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/Meta Games/GemJackpot")]
    public class CheckGemJackpotNodealClose : ConditionTask
    {
        protected override string info
        {
            get { return "Check GemJackpot NodealClose"; }
        }

        protected override bool OnCheck()
        {
            return BlackboardQueryUtils.CheckGemJackPotNoDealClose();
        }
    }
}