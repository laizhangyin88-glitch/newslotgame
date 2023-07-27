using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode.Chat {
    [System.Serializable]
    public class ChannelChatPrivate : ChannelChatBase {
        public override ChannelType channelType => ChannelType.Private;

    }
}
