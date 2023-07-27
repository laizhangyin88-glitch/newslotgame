using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using ParadoxNotion;

namespace BagelCode.Chat {
    public class TextBalloonRequest : TextBalloonBase {
        private ContextSlider gaugeContext => mContext.FindElement<ContextSlider>("Text Balloon/Gauge");
        private ContextTextMeshProUGUI gaugeTextContext => mContext.FindElement<ContextTextMeshProUGUI>("Text Balloon/Gauge/Text");
        private ContextText titleTextContext => mContext.FindElement<ContextText>("Text Balloon/Title Text");
        private ContextText subTextContext => mContext.FindElement<ContextText>("Text Balloon/Sub Text");
        private GameObject shareIcon;
        private ContextButton giftButton;


        public override void Init(ChattingController _owner)
        {
            base.Init(_owner);


            titleTextContext.SetGlobalText("CHAT_CLUB_SHARE_TEXT_BALLOON_TITLE");
            subTextContext.SetGlobalText("CHAT_CLUB_SHARE_TEXT_BALLOON_SUB");
        }

        public override void Refresh(ChatMessageData _chatData)
        {
            base.Refresh(_chatData);

            StopAllCoroutines();
            // StartCoroutine(RefreshIconAsync());
            // StartCoroutine(RefreshPossessionAreaAsync());
            // StartCoroutine(RefreshGiftAreaAsync());
        }
    }
}
