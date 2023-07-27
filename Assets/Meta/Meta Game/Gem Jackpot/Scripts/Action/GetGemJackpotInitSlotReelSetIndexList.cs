using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/GemJackpot")]
    public class GetGemJackpotInitSlotReelSetIndexList : ActionTask<Blackboard>
    {
        public BBParameter<List<int>> saveAs;
        protected override string info
        {
            get
            {
                return string.Format("Get GemJackpot InitReelSlotIndex");
            }
        }

        protected override void OnExecute()
        {
            saveAs.value = GemJackpotUtils.InitialSlotReelSetIndexList;
            EndAction();
        }
    }
}