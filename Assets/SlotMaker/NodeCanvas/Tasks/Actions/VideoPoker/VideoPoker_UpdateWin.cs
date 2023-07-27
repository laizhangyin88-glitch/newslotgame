using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Cards.Tasks.Actions
{
    [Category("★ SlotMaker/Cards")]
    public class VideoPoker_UpdateWin : ActionTask
    {
        public BBParameter<GameObject> videoPoker;
        public BBParameter<List<PokerWin>> winList;

        protected override string info { get { return string.Format("VideoPoker.UpdateWin({0})", winList); } }

        protected override void OnExecute()
        {
            videoPoker.value.GetComponent<VideoPoker>().UpdateWin(winList.value);

            EndAction();
        }
    }
}