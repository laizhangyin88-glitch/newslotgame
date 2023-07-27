using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Cards.Tasks.Actions
{
    [Category("★ SlotMaker/Cards")]
    public class VideoPoker_GetHits : ActionTask 
    {
        public BBParameter<PokerWin> win;
        public BBParameter<List<Blackboard>> decks;

        public BBParameter<List<bool>> saveAs;

        protected override string info { get { return string.Format("{0} = VideoPoker.GetHits()", saveAs); } }

        protected override void OnExecute()
        {
            saveAs.value = win.value.hitmap;
            EndAction();
        }
    }
}