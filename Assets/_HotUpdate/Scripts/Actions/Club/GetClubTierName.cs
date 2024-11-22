using System;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Club")]
    public class GetClubTierName : ActionTask<Blackboard>
    {
        public BBParameter<string>  tierValue;
        public BBParameter<bool>    isUpper;
        public BBParameter<bool>    useTierColor;

        public BBParameter<string>  saveAs;

        protected override string info
        {
            get { return string.Format("Get Club Tier Name{0}{1}", isUpper.value ? " Upper" : "", useTierColor.value ? " Use Tier Color": "" ); }
        }

        protected override void OnExecute()
        {
            
            EndAction();
        }
    }
}
