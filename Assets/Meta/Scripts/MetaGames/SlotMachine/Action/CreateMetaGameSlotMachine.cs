using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using BagelCode;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ BagelCode/MetaGames")]
    public class CreateMetaGameSlotMachine : ActionTask<Transform>
    {
        public BBParameter<int> column;
        public BBParameter<int> row;
        public BBParameter<List<int>> visibleCounts;
        public BBParameter<List<int>> startIndex;
        public BBParameter<bool> isShuffle;

        protected override void OnExecute()
        {
            var slotMachine = agent.GetComponent<MetaSlotMachine>();
            var isStatic = slotMachine.staticReel;

            int backupIndex = MetaSlotMachineGlobalReelStrips.Instance.index;
            MetaSlotMachineGlobalReelStrips.Instance.index = slotMachine.slotIndex;

            for (int i = 0; i < column.value; ++i)
            {
                int beginRow = row.value - visibleCounts.value[i];
                int endRow = beginRow + visibleCounts.value[i];

                if (isStatic)
                {
                    MetaSlotMachineReel reel = (MetaSlotMachineReel)slotMachine.CreateReel(i, i, beginRow, i + 1, endRow, 0);
                    reel.metaStrip = reel.metaStrip;
                }
                else
                {
                    MetaSlotMachineReel reel = (MetaSlotMachineReel)slotMachine.CreateReel(i, beginRow, i + 1, endRow, 0);
                    reel.metaStrip = reel.metaStrip;
                }
            }
            
            if (isShuffle.value)
                slotMachine.Shuffle();
            else
                SetReelIndex(slotMachine);

            MetaSlotMachineContentCustomData.GetSlotData(slotMachine.slotIndex).slotMachine = slotMachine.gameObject;
            MetaSlotMachineGlobalReelStrips.Instance.index = backupIndex;
            EndAction();
        }

        protected void SetReelIndex(SlotMachine slotMachine)
        {
            for (int i = 0; i < slotMachine.reels.Count; ++i)
            {
                BaseReel reel = slotMachine.GetReel(i);
                reel.index = startIndex.value[i];
            }

            slotMachine.Visit((sb) => { sb.Initialize(); });
        }
    }
}