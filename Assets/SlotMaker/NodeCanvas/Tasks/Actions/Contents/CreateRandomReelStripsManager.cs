using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class CreateRandomReelStripsManager : ActionTask
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<int> stripsCount;
    public BBParameter<int> stripCount;

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

            var reelSet = reelSetList[i];
            var reelSequenceList = reelSet.GetValue<List<Blackboard>>("reelSequenceList");
            for (int j = 0; j < reelSequenceList.Count; ++j)
            {
                for (int k = 0; k < stripsCount.value; ++k)
                {
                    go = new GameObject();
                    go.name = "ReelStrip";
                    go.transform.SetParent(reelStrips.transform, false);

                    var reelStrip = go.AddComponent<ReelStrip>();
                    reelStrip.stripIndex = k;
                    reelStrip.strip = new List<SymbolInfo>();

                    var weights = reelSequenceList[j].GetValue<List<int>>("value");
                    int totalWeight = 0;
                    weights = RandomUtils.GetAccumulatedWeightList(weights, out totalWeight);
                    for (int l = 0; l < stripCount.value; ++l)
                    {
                        int symbolIndex = RandomUtils.WeightRandom(weights, totalWeight);
                        reelStrip.strip.Add(SlotUtils.CreateSymbolInfo(symbolIndex, symbolMask));
                    }

                    reelStrips.reelStrips.Add(reelStrip);
                }
            }

            mgr.stripsList.Add(reelStrips);
        }

        EndAction();
    }
}

}
