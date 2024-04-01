using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Simulator")]
    public class GetBaseURL : ActionTask
    {
        public BBParameter<string> url;

        protected override string info
        {
            get { return "Get Base URL"; }
        }

        protected override void OnExecute()
        {
#if DEV
            string devURL = PlayerPrefs.GetString("Dev_URL", "");
            if(string.IsNullOrEmpty(devURL))
            {
                url.value = ApplicationSettings.Instance.apiUrl;
            }
            else
            {
                url.value = devURL;
            }
#else
            url.value = NativeHelper.Instance.GetServerBaseUrl();
#endif
            EndAction();
        }
    }

}
