using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode;
using System.Collections.Generic;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]
    public class GambleDeal : ActionTask
    {
        public BBParameter<int> selectedIndex;

        protected override string info
        {
            get { return string.Format("GambleDeal {0}", selectedIndex); }
        }

        protected override void OnExecute()
        {            
            MetaSystem.GambleDeal(selectedIndex.value, EndAction, null);
        }
    }
}
