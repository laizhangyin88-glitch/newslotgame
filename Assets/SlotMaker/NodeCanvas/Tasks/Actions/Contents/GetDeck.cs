using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class GetDeck : ActionTask
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<string> targetBB = "./spin/deck";

    protected override void OnExecute()
    {
        var deck = (Deck)ContentCustomData.GetSlotData(slotIndex.value).deck.Clone();
 
        var variable = BlackboardUtils.GetOrCreateVariable<Deck>(null, targetBB.value);
        variable.value = deck;

        EndAction();
    }
}

}
