using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Chat
{
    [System.Serializable]
    public class ChannelChatClub : ChannelChatBase
    {
        public override ChannelType channelType => ChannelType.Club;

        private ContextButton requestButton => ContextUtils.FindElement(mContext, "Buttons/Button Request", ContextSearchingType.FullNameSearch).GetComponent<ContextButton>();

        private GameObject shareIcon;

        protected override void Start()
        {
            base.Start();
            requestButton.transform.parent.gameObject.SetActive(false);
            // requestButton.button.onClick.AddListener(() =>
            //     {
            //         OpenRequestScene();
            //     }
            // );
        }

        public override void OnChannelSelected()
        {
            mContext.FindElement("Buttons").gameObject.SetActive(true);
            // StartCoroutine(InitCollectingGameAsync());
        }

        public override void OnChannelUnselected()
        {
            mContext.FindElement("Buttons").gameObject.SetActive(false);
        }
    }
}
