using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Utils")]
    public class OpenCommonAppUpdate : ActionTask
    {
        protected override string info
        {
            get { return "Open Popup App Update"; }
        }

        protected override void OnExecute()
        {
            OpenPopup();
            EndAction();
        }

        private void OpenPopup()
        {
            string appDownloadURL = BlackboardUtils.FindVariable<string>(null, "/appDownloadUrl").value;

            var rewardCoins = BlackboardUtils.FindVariable<long>(null, "/values/misc/VERSION_UPDATE_REWARD_CREDIT");

            bool isError = false;

            ErrorPopupInfo info = new ErrorPopupInfo();

            info.type = ErrorPopupType.OkWithTitle;
            info.title = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_DEPRECATED_VERSION_ERROR_WITH_TITLE", out isError);
            info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_DEPRECATED_VERSION_ERROR_WITH_REWARD", out isError, rewardCoins.value);

            info.useXButton = true;

            info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OK", out isError);
            info.buttonAutoClose1 = false;

            info.callback1 = delegate
            {
                Application.OpenURL(appDownloadURL);
            };

            info.callbackX = delegate
            {
            };

            ErrorPopupHandler.Instance.OpenError(info);
        }
    }
}