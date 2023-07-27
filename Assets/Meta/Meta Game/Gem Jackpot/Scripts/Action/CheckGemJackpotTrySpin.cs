using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/Meta Games/GemJackpot")]
    public class CheckGemJackpotTrySpin : ConditionTask
    {
        protected override string info
        {
            get { return "Check Try Spin(gem & freeSpin)"; }
        }

        protected override bool OnCheck()
        {
            return GemJackpotUtils.CheckGemJackpotTrySpin();
        }
    }
}