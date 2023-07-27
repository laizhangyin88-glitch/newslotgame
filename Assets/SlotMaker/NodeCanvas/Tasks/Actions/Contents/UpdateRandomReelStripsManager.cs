using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class UpdateRandomReelStripsManager : ActionTask
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<int> stripsCount;
    public BBParameter<int> stripLength;

    // Update Reel Sets in [startReelSetIndex, endReelSetIndex] (inclusive)
    public BBParameter<int> startReelSetIndex;
    public BBParameter<int> endReelSetIndex;

    protected override void OnExecute()
    {
        var mgr = GlobalReelStrips.Instance;
        var symbolMask = ContentCustomData.GetSlotData(slotIndex.value).symbolMask;

        var reelSetList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "./game/reelSetList").value;
        for (int reelSetIndex = startReelSetIndex.value; reelSetIndex <= endReelSetIndex.value; ++reelSetIndex)
        {
            var reelStrips = mgr.stripsList[reelSetIndex];

            var reelSet = reelSetList[reelSetIndex];
            var reelSequenceList = reelSet.GetValue<List<Blackboard>>("reelSequenceList");
            for (int reelSequenceIndex = 0; reelSequenceIndex < reelSequenceList.Count; ++reelSequenceIndex)
            {
                for (int stripIndexInReelSequence = 0; stripIndexInReelSequence < stripsCount.value; ++stripIndexInReelSequence)
                {
                    var stripIndex = reelSequenceIndex * stripsCount.value + stripIndexInReelSequence;
                    var reelStrip = GetOrCreateClearedReelStrip(reelStrips, stripIndex);

                    var weights = reelSequenceList[reelSequenceIndex].GetValue<List<int>>("value");
                    int totalWeight = 0;
                    weights = RandomUtils.GetAccumulatedWeightList(weights, out totalWeight);
                    for (int stripPos = 0; stripPos < stripLength.value; ++stripPos)
                    {
                        int symbolIndex = RandomUtils.WeightRandom(weights, totalWeight);
                        reelStrip.strip.Add(SlotUtils.CreateSymbolInfo(symbolIndex, symbolMask));
                    }
                }
            }
        }

        EndAction();
    }

    private ReelStrip GetOrCreateClearedReelStrip(ReelStrips reelStrips, int stripIndex)
    {
        ReelStrip reelStrip;
        if (stripIndex < reelStrips.reelStrips.Count)
        {
            reelStrip = (ReelStrip)reelStrips.reelStrips[stripIndex];
            reelStrip.strip.Clear();
        }
        else
        {
            var go = new GameObject();
            go.name = "ReelStrip";
            go.transform.SetParent(reelStrips.transform, false);

            reelStrip = go.AddComponent<ReelStrip>();
            reelStrip.stripIndex = stripIndex;
            reelStrip.strip = new List<SymbolInfo>();
            
            reelStrips.reelStrips.Add(reelStrip);
        }

        return reelStrip;
    }
}

}
