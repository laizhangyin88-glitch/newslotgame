using UnityEngine;
using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;
using System.Linq;

namespace BagelCode.Tasks.Actions
{

    [Category("★ BagelCode/Utils")]
    public class GetFavoriteIdBB : ActionTask<Blackboard>
    {
        public BBParameter<int> saveAs;

        protected override string info
        {
            get { return $"{saveAs} = Favorite Id"; }
        }

        protected override void OnExecute()
        {
            var favoriteList = MainBlackboard.Get().GetValue<List<int>>("favoriteSlotIdList");
            if (favoriteList!=null && favoriteList.Count>0)
            {
                saveAs.value = favoriteList[0];
            } 
            EndAction();
        }
    }

}
