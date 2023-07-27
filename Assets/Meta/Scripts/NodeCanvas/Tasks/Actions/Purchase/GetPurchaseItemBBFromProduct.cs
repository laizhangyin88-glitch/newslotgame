using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/Purchase")]
public class GetPurchaseItemBBFromProduct : ActionTask<Blackboard> 
{
    public BBParameter<string>  valueA;
    public BBParameter<ItemType> itemType;

    [BlackboardOnly]
    public BBParameter<Blackboard>  itemBB;

    protected override string info
    {
        get { return string.Format("{0} = Get {1}({2}) Purchase Item BB", itemBB, valueA, itemType); }
    }

    protected override void OnExecute()
    {
        var productBB = BlackboardUtils.FindVariable<Blackboard>(agent, valueA.value);
        itemBB.value = BlackboardQueryUtils.GetItemFromProduct(productBB.value, itemType.value);

        EndAction();
    }
}

}
