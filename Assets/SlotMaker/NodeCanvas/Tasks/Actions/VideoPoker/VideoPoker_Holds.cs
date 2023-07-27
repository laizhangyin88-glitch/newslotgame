using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Cards.Tasks.Actions
{
    [Category("★ SlotMaker/Cards")]
    public class VideoPoker_Holds : ActionTask 
    {
        public BBParameter<GameObject> videoPoker;
        public BBParameter<int> handsCount;
        public BBParameter<List<bool>> helds;

        protected override string info { get { return string.Format("VideoPoker.Holds()"); } }

        protected override void OnExecute()
        {
            videoPoker.value.GetComponent<VideoPoker>().ToggleCards(helds.value, handsCount.value);

            EndAction();
        }
    }
}