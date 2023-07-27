using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode;
using System.Collections.Generic;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]
    public class VideoPokerDeal : ActionTask
    {
        public BBParameter<long> betPerHand;
        public BBParameter<int> handCount;
        public BBParameter<object> customData = null;

        protected override string info { get { return "Request Deal"; } }

        protected override void OnExecute()
        {
            MetaSystem.VideoPokerDeal(betPerHand.value, handCount.value, customData.value, EndAction, null);
        }
    }
}