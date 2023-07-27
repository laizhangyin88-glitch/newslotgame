using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using UnityEngine;

namespace BagelCode.Chat {
    /// <summary>
    /// ChatPoll wrapper class (When send chat)
    /// </summary>
    [System.Serializable]
    public class RequestChatPoll {
        public long requestId;
        public ChatPoll chatPoll;
        public bool isResponse=false;
        public bool isSuccess=false;
        public long successId;//ChatPollId
    }
}

