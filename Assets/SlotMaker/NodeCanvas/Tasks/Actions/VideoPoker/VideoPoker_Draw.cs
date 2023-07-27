using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Cards.Tasks.Actions
{
    [Category("★ SlotMaker/Cards")]
    public class VideoPoker_Draw : ActionTask
    {
        public BBParameter<GameObject> videoPoker;
        public BBParameter<List<long>> multipliers;

        protected override string info { get { return "VideoPoker.Draw()"; } }

        protected override void OnExecute()
        {        
            videoPoker.value.GetComponent<VideoPoker>().Draw(multipliers.value);
            EndAction();
        }
    }
}
