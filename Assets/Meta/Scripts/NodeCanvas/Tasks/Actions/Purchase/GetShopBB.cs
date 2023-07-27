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
public class GetShopBB : ActionTask<Blackboard>
{
    public BBParameter<ShopType> shopType;

    [BlackboardOnly]
    public BBParameter<Blackboard> saveAs;

    protected override string info
    {
        get { return string.Format("{0} = Get {1} Shop", saveAs, shopType); }
    }

    protected override void OnExecute()
    {
        saveAs.value = BlackboardQueryUtils.GetShopBB(shopType.value);

        EndAction();
    }
}

}
