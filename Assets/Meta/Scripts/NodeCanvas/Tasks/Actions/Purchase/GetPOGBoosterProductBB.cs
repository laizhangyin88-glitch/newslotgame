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
public class GetPOGBoosterProductBB : ActionTask<Blackboard> 
{
    public BBParameter<string> priceValue;

    public BBParameter<Blackboard> saveAsProductBB;

    protected override string info
    {
        get { return string.Format("{0} = Get POG Booster BB({1})", saveAsProductBB, priceValue); }
    }

    protected override void OnExecute()
    {
        var price = BlackboardUtils.FindVariable<double>(agent, priceValue.value);

        if(price != null)
        {
            saveAsProductBB.value = BlackboardQueryUtils.GetPriceMatchProduct(price.value, ShopType.POG_BOOSTER, ItemType.POG_BOOSTER);
        }
        else
        {
            saveAsProductBB.value = null;
        }

        EndAction();
    }
}

}
