using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;
using System.Linq;

namespace BagelCode
{

    public static partial class BlackboardQueryUtils
    {
        private static Dictionary<string, string> chatHeaders = new Dictionary<string, string>();

        public static Dictionary<string, string> GetChatRequestHeaders()
        {
            var chatToken = MainBlackboard.Get().GetVariable<string>("chatToken");
            if (chatToken == null) return null;

            chatHeaders["Authorization"] = "Bearer " + chatToken.value;
            return chatHeaders;
        }

    }

}

