using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Notice")]
    public class SetNoticeCooltime : ActionTask<Blackboard>
    {
        public BBParameter<Blackboard> noticeInfo;

        protected override string info
        {
            get { return string.Format("Set Notice Cooltime(PlayerPrefs) => {0}", noticeInfo); }
        }

        protected override void OnExecute()
        {
            if (noticeInfo.value != null)
                BlackboardQueryUtils.SetCooltime(noticeInfo.value);

            EndAction();
        }
    }
}