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
public class GetPurchaseDailySpinBB : ActionTask<Blackboard> 
{
    public BBParameter<ItemType> purchaseItemType;
    public BBParameter<string>  valueA;

    [BlackboardOnly]
    public BBParameter<int> spinCount;

    [BlackboardOnly]
    public BBParameter<long> earnRP;

    [BlackboardOnly]
    public BBParameter<float>  origItemPrice;

    [BlackboardOnly]
    public BBParameter<float>  itemPrice;

    [BlackboardOnly]
    public BBParameter<Blackboard>  infoBB;

    protected override string info
    {
        get { return "Set Daily Spin Item BB"; }
    }

    protected override void OnExecute()
    {
        var productBB = BlackboardUtils.FindVariable<Blackboard>(agent, valueA.value);
        infoBB.value = BlackboardQueryUtils.GetItemFromProduct(productBB.value, purchaseItemType.value);

        earnRP.value    = BlackboardUtils.FindVariable<long>(infoBB.value, "rp").value;
        spinCount.value = BlackboardUtils.FindVariable<int>(infoBB.value, "spinCount").value;
        itemPrice.value = System.Convert.ToSingle(BlackboardUtils.FindVariable<double>(productBB.value, "price").value);
        origItemPrice.value = System.Convert.ToSingle(BlackboardUtils.FindVariable<double>(productBB.value, "originalPrice").value);

        EndAction();
    }
}

}
