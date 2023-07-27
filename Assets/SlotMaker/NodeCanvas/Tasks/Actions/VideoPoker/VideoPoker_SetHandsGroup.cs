using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Cards.Tasks.Actions
{
    [Category("★ SlotMaker/Cards")]
    public class VideoPoker_SetHandsGroup : ActionTask
    {
        public BBParameter<GameObject> videoPoker;
        public BBParameter<int> handsGroup;

        protected override string info { get { return string.Format("VideoPoker.HandsGroup = {0}", handsGroup.value); } }

        protected override void OnExecute()
        {        
            var vp = videoPoker.value.GetComponent<VideoPoker>();
            vp.ReleaseHandsGroup();
            vp.InitializeHandsGroup(handsGroup.value);
            EndAction();
        }
    }
}
