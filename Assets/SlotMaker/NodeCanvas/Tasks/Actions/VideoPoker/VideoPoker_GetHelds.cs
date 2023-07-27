using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Cards.Tasks.Actions
{
    [Category("★ SlotMaker/Cards")]
    public class VideoPoker_GetHelds : ActionTask
    {
        public BBParameter<GameObject> videoPoker;
        
        public BBParameter<List<bool>> saveAs;

        protected override string info { get { return string.Format("{0} = VideoPoker.GetHelds()", saveAs); } }

        protected override void OnExecute()
        {        
            saveAs.value = videoPoker.value.GetComponent<VideoPoker>().GetHelds();

            EndAction();
        }
    }
}
