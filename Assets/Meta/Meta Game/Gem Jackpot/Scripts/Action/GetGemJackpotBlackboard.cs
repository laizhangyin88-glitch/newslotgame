using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/GemJackpot")]
    public class GetGemJackpotBlackboard : ActionTask<Blackboard>
    {
        public BBParameter<Blackboard> saveAs;
        protected override string info
        {
            get
            {
                return string.Format("Get GemJackpot BB");
            }
        }

        protected override void OnExecute()
        {
            saveAs.value = GemJackpotUtils.GemJackpotInfo;
            EndAction();
        }
    }
}