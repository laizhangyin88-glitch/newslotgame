using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Cards.Tasks.Conditions
{
    [Category("★ SlotMaker/Cards")]
    public class CheckVideoPokerReady : ConditionTask
    {
        public BBParameter<GameObject> videoPoker;

        protected override bool OnCheck()
        {        
            return videoPoker.value.GetComponent<VideoPoker>().IsReady();
        }
    }
}
