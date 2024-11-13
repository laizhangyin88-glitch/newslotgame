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
public class GetShopProductGroups : ActionTask<Blackboard>
{
    public BBParameter<ShopType> shopType;

    [BlackboardOnly]
    public BBParameter<List<Blackboard>> saveAsProducGroupList;

    protected override string info
    {
        get { return string.Format("{0} = Get {1} Product Groups", saveAsProducGroupList, shopType); }
    }

    protected override void OnExecute()
    {
        saveAsProducGroupList.value = BlackboardQueryUtils.GetShopProductGroups(shopType.value);

        EndAction();
    }
}

}
