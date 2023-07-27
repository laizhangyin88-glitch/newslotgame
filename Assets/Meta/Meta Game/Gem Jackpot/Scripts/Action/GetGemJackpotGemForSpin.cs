using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/GemJackpot")]
    public class GetGemJackpotForSpin : ActionTask<Blackboard>
    {
        public BBParameter<long> saveAs;
        protected override string info
        {
            get
            {
                return string.Format("Get GemJackpot Gem For Spin");
            }
        }

        protected override void OnExecute()
        {
            saveAs.value = GemJackpotUtils.GemForSpinSale;
            EndAction();
        }
    }
}