using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Utils")]
    public class OpenVipSurvey : ActionTask
    {
        public BBParameter<string> userId;
        public BBParameter<string> surveyUrl;

        protected override string info
        {
            get
            {
                return string.Format("Open Vip Survey");
            }
        }

        protected override void OnExecute()
        {
            string url = $"{surveyUrl.value}?user_id={userId.value}";

            if (url == null)
            {
            }
            else
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                NativeHelper.Instance.OpenUrl(url);
#else
                Application.OpenURL(url);
#endif
            }
            EndAction();
        }
    }
}