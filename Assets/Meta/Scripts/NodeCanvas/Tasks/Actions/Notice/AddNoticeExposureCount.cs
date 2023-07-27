using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Notice")]
    public class AddNoticeExposureCount : ActionTask<Blackboard>
    {
        public BBParameter<Blackboard> noticeInfo;

        protected override string info
        {
            get { return string.Format("Add Notice Exposure Count(PlayerPrefs) => {0}", noticeInfo); }
        }

        protected override void OnExecute()
        {
            if (noticeInfo.value != null)
            {
                int noticeId = BlackboardUtils.FindValue<int>(noticeInfo.value, "id");
                string triggeredMaxExposureKey = string.Format(BlackboardQueryUtils.noticeExposureCountKey, noticeId);
                int prefsMaxExposureCount = PlayerPrefs.GetInt(triggeredMaxExposureKey, 0);

                PlayerPrefs.SetInt(triggeredMaxExposureKey, prefsMaxExposureCount + 1);
            }
            EndAction();
        }
    }
}