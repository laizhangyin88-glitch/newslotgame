using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class CreateReelStrips : ActionTask
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<int> reelSetIndex;

    public BBParameter<GameObject> saveAs;

    protected override void OnExecute()
    {
        var parent = ContentCustomData.Instance.transform;

        var reelStripsObject = new GameObject();
        reelStripsObject.name = "Local ReelStrips";
        reelStripsObject.transform.SetParent(parent, false);

        var reelStrips = reelStripsObject.AddComponent<ReelStrips>();
        reelStrips.reelStrips = new List<BaseReelStrip>();

        var symbolMask = ContentCustomData.GetSlotData(slotIndex.value).symbolMask;

        var reelSetList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "./game/reelSetList").value;
        var reelSet = reelSetList[reelSetIndex.value];
        var reelSequenceList = reelSet.GetValue<List<Blackboard>>("reelSequenceList");

        for (int i = 0; i < reelSequenceList.Count; ++i)
        {
            var go = new GameObject();
            go.name = "ReelStrip";
            go.transform.SetParent(reelStrips.transform, false);

            var reelStrip = go.AddComponent<ReelStrip>();
            reelStrip.stripIndex = i;
            reelStrip.strip = new List<SymbolInfo>();

            var indexList = reelSequenceList[i].GetValue<List<int>>("value");
            for (int j = 0; j < indexList.Count; ++j)
            {
                reelStrip.strip.Add(SlotUtils.CreateSymbolInfo(indexList[j], symbolMask));
            }

            reelStrips.reelStrips.Add(reelStrip);
        }

        saveAs.value = reelStripsObject;
        EndAction();
    }
}

}
