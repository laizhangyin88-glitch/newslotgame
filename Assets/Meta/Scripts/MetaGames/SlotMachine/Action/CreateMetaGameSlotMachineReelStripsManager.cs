using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/MetaGames")]
    public class CreateMetaGameSlotMachineReelStripsManager : ActionTask
    {
        public BBParameter<int> slotIndex = 0;
        public bool singleStrip = false;

        protected override string info
        {
            get
            {
                return string.Format("Create Meta Game Slot Reel Strips Manager");
            }
        }

        protected override void OnExecute()
        {
            var parent = MetaSlotMachineContentCustomData.Instance.transform;
            var go = new GameObject();
            go.name = "ReelStrips Manager";
            go.transform.SetParent(parent, false);

            var mgr = go.AddComponent<MetaSlotMachineGlobalReelStrips>();
            mgr.stripsList = new List<ReelStrips>();

            //var symbolMask = MetaSlotMachineContentCustomData.GetSlotData(slotIndex.value).symbolMask;

            var reelSetList = BlackboardUtils.FindVariable<List<Blackboard>>(GemJackpotUtils.GemJackpotInfo, "reelSetList").value;
            for (int i = 0; i < reelSetList.Count; ++i)
            {
                var symbolMask = MetaSlotMachineContentCustomData.GetSlotData(i).symbolMask;

                go = new GameObject();
                go.name = "ReelStrips";
                go.transform.SetParent(mgr.transform, false);

                var reelStrips = go.AddComponent<ReelStrips>();
                reelStrips.reelStrips = new List<BaseReelStrip>();
                reelStrips.singleStrip = singleStrip;

                var reelSet = reelSetList[i];
                var reelSequenceList = reelSet.GetValue<List<Blackboard>>("reelSequenceList");
                for (int j = 0; j < reelSequenceList.Count; ++j)
                {
                    go = new GameObject();
                    go.name = "ReelStrip";
                    go.transform.SetParent(reelStrips.transform, false);

                    var reelStrip = go.AddComponent<ReelStrip>();
                    reelStrip.stripIndex = j;
                    reelStrip.strip = new List<SymbolInfo>();

                    var indexList = reelSequenceList[j].GetValue<List<int>>("value");
                    for (int k = 0; k < indexList.Count; ++k)
                    {
                        reelStrip.strip.Add(SlotUtils.CreateSymbolInfo(indexList[k], symbolMask));
                    }

                    reelStrips.reelStrips.Add(reelStrip);
                }

                mgr.stripsList.Add(reelStrips);
            }

            EndAction();
        }
    }
}
