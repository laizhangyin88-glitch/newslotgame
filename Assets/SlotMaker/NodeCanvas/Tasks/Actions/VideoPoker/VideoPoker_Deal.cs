using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Cards.Tasks.Actions
{
    [Category("★ SlotMaker/Cards")]
    public class VideoPoker_Deal : ActionTask
    {
        public BBParameter<GameObject> videoPoker;

        protected override string info { get { return "VideoPoker.Deal()"; } }

        protected override void OnExecute()
        {   
            videoPoker.value.GetComponent<VideoPoker>().Deal();
            EndAction();     
        }
    }
}
