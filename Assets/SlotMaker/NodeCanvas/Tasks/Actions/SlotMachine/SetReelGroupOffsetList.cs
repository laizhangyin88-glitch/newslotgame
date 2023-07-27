using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Tasks.Actions.Contents
{
    [Name("Set ReelGroupOffsetList")]
    [Category("★ SlotMaker/SlotMachine")]
    [Description("Use for set ReelGroupOffsetList in MultiReelLayoutGroup component, usually MultiReelLayoutGroup is located in SlotMachine → Reels")]
    public class SetReelGroupOffsetList : ActionTask
    {
        public BBParameter<GameObject> slotMachine;
        public BBParameter<List<MultiReelLayoutGroup.ReelGroupOffset>> reelGroupOffsetList;

        protected override void OnExecute()
        {
            var slotMachine = this.slotMachine.value.GetComponent<BaseSlotMachine>();
            var multiReelLayoutGroup = slotMachine.layoutGroup as MultiReelLayoutGroup;
            multiReelLayoutGroup.ReelGroupOffsetList = reelGroupOffsetList.value;
            EndAction();
        }
    }
}
