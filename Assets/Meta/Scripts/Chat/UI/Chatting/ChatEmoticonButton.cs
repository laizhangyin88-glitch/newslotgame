using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode.Chat
{
    [RequireComponent(typeof(Button))]
    public class ChatEmoticonButton : MonoBehaviour
    {
        public ChattingController owner;
        public EmojiChat emojiChat;
        private Button button;
        private ContextElement parent;

        private void Start()
        {
            button = GetComponent<Button>();
            parent = owner.rootElement.Find("Emoticon List Base");

            button.onClick.AddListener(() =>
            {
                owner.SendChatMessageEmoji(emojiChat);
                parent.gameObject.SetActive(false);
            });
        }
    }
}