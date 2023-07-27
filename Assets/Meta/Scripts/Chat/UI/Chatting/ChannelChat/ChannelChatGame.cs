using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Chat {
    [System.Serializable]
    public class ChannelChatGame : ChannelChatBase {
        public override ChannelType channelType => ChannelType.Game;


        public override void OnChannelSelected() {
            mContext.FindElement("Buttons").gameObject.SetActive(true);
        }
        public override void OnChannelUnselected() {
            mContext.FindElement("Buttons").gameObject.SetActive(false);
        }
    }
}
