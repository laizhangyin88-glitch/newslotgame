using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/Contents")]
    public class VideoPokerClaimBonus : ActionTask
    {
        public BBParameter<string> uid;
        public BBParameter<int> selectedIndex;

        protected override string info
        {
            get { return string.Format("VideoPokerClaimBonus {0} with {1}", uid, selectedIndex); }
        }

        protected override void OnExecute()
        {
            MetaSystem.VideoPokerClaimBonus(uid.value, selectedIndex.value, EndAction, null);
        }
    }
}
