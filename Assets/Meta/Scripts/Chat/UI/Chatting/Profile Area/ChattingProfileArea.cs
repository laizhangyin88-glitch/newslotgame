using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode.Chat
{
    public class ChattingProfileArea : MonoBehaviour
    {
        public ChattingController owner;

        public OSA_ChatProfiles osaChatProfiles
        {
            get
            {
                if (owner.CurrentChannelType == ChannelType.Global)
                {
                    return globalOsaChatProfiles;
                }
                else
                {
                    return _osaChatProfiles;
                }
            }
        }

        public OSA_ChatProfiles _osaChatProfiles;
        public OSA_ChatProfiles globalOsaChatProfiles;

        private bool isInit = false;

        public void Refresh()
        {
            if (isInit)
                osaChatProfiles.ResetItems(owner.UserProfiles.Count);
        }

        private void Start()
        {
            owner.OnChannelChanged += (_channelType) =>
            {
                if (_channelType == ChannelType.Global)
                {
                    _osaChatProfiles.gameObject.SetActive(false);
                    globalOsaChatProfiles.gameObject.SetActive(true);
                }
                else
                {
                    _osaChatProfiles.gameObject.SetActive(true);
                    globalOsaChatProfiles.gameObject.SetActive(false);
                }
            };

            isInit = true;
        }
    }

}
