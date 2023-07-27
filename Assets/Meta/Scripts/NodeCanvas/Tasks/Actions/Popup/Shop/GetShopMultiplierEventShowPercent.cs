using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

    [Category("★ BagelCode/Meta/Shop")]
    public class GetShopMultiplierEventShowPercent : ActionTask<Blackboard> 
    {
        public BBParameter<ShopType> shopType;
        
        public BBParameter<bool> saveAsShowPercent;

        protected override void OnExecute()
        {
            saveAsShowPercent.value = BlackboardQueryUtils.IsShopEventPercentText(shopType.value);
            EndAction();
        }
    }

}

