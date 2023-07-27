using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class RestoreDeck : ActionTask<Blackboard>
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<string> key = "./spin/deck";

    protected override void OnExecute()
    {
        var variable = BlackboardUtils.FindVariable<Deck>(null, key.value);
        ContentCustomData.GetSlotData(slotIndex.value).deck = variable.value;

        EndAction();
    }
}

}
