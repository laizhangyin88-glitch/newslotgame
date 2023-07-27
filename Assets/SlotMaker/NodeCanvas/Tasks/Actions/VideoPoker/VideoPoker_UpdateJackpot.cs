using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Cards.Tasks.Actions
{
    [Category("★ SlotMaker/Cards")]
    public class VideoPoker_UpdateJackpot : ActionTask
    {
        public BBParameter<GameObject> videoPoker;
        public BBParameter<List<Blackboard>> jackpots;

        protected override string info { get { return string.Format("VideoPoker.UpdateJackpot()"); } }

        protected override void OnExecute()
        {
            videoPoker.value.GetComponent<VideoPoker>().UpdateJackpot(jackpots.value);
            EndAction();
        }
    }
}