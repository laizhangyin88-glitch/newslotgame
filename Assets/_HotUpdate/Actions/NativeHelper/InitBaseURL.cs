using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/NativeHelper")]
    public class InitBaseURL : ActionTask
    {
        protected override string info
        {
            get
            {
                return "Init Base URL";
            }
        }

        protected override void OnExecute()
        {
            AssetBundleManager.BaseFilePath = ApplicationSettings.GetStreamingBundlePath();
            string baseURL = NativeHelper.Instance.GetServerBaseUrl();

            if(!string.IsNullOrEmpty(baseURL))
            {
                ApplicationSettings.Instance.apiUrl = baseURL;
            }

            string chattingUrl = NativeHelper.Instance.GetChattingUrl();

            if(!string.IsNullOrEmpty(chattingUrl))
            {
                ApplicationSettings.Instance.chattingApiUrl = chattingUrl;
            }

            EndAction();
        }
    }

}
