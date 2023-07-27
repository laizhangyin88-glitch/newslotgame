using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/GemJackpot")]
    public class InitGemJackpot : ActionTask<Blackboard>
    {
        protected override string info
        {
            get
            {
                return string.Format("Init GemJackpot");
            }
        }

        protected override void OnExecute()
        {
            GemJackpotUtils.CreateGemJackpotReelStripsBB();

            EndAction();
        }
    }
}