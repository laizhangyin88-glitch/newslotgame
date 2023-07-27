using System;
using UnityEngine;
using UnityEngine.Events;
using SlotMaker;
using ParadoxNotion;

namespace BagelCode
{
    public class EnterEarlyAccessBehaviour : MonoBehaviour
    {
        public int gameId;
        public string enterType;
        public string fromType;
        public string targetRoomId;
        public int status;

        [Serializable]
        public class ButtonClickedEvent : UnityEvent { }

        [SerializeField]
        public ButtonClickedEvent onClick;

        private const string ON_META_UI_EVENT = "OnMetaUIEvent";
        private const string ON_TRIGGER_SLOT_ENTER = "OnTriggerSlotEnterIAM";

        public void SetEnterGameInfo()
        {
            // Check EarlyAccess Subscription.
            if (BlackboardQueryUtils.IsEarlyAccessAvailable())
            {
                if (status == 4)
                {
                    OpenAppUpdatePopup();
                }
                else
                {
                    BlackboardQueryUtils.SetEnterGameInfo(gameId, enterType, fromType, targetRoomId, 0, null, true);
                    onClick.Invoke();
                }
            }
            else
            {
                MessageDispatcher.Dispatch(ON_META_UI_EVENT, new EventData(ON_TRIGGER_SLOT_ENTER));
            }
        }

        private void OpenAppUpdatePopup()
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