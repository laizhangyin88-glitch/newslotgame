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
    public class VideoPokerDraw : ActionTask
    {
        public BBParameter<List<bool>> helds;
        public BBParameter<object> customData = null;

        protected override string info { get { return "Request Draw"; } }

        protected override void OnExecute()
        {
            MetaSystem.VideoPokerDraw(helds.value, customData.value, EndAction, null);
        }
    }
}