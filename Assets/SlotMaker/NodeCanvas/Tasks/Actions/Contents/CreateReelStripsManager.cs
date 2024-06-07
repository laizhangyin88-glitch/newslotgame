using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class CreateReelStripsManager : ActionTask
{
    public BBParameter<int> slotIndex = 0;
    public bool singleStrip = false;

    protected override void OnExecute()
    {
        var parent = ContentCustomData.Instance.transform;
        var go = new GameObject();
        go.name = "ReelStrips Manager";
        go.transform.SetParent(parent, false);

        var mgr = go.AddComponent<GlobalReelStrips>();
        mgr.stripsList = new List<ReelStrips>();

        var symbolMask = ContentCustomData.GetSlotData(slotIndex.value).symbolMask;

        var reelSetList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "./game/reelSetList").value;
        for (int i = 0; i < reelSetList.Count; ++i)
        {
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
