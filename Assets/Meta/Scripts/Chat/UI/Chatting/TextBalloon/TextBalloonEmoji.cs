using BagelCode.ClientModels;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Chat
{
    public class TextBalloonEmoji : TextBalloonBase
    {
        public ContextText contentContext;
        public ContextElement imageBalloon;
        public ContextElement contentBalloon;

        private ContextElement buttonElement;
        private Animator anim;

        private GameObject emojiObj = null;
        public override void Init(ChattingController _owner)
        {
            base.Init(_owner);
        }

        public override void Refresh(ChatMessageData _chatData)
        {
            base.Refresh(_chatData);
            var data = (chatData.chatPoll.data as ChatDataEmoji);

            if (emojiObj != null) Destroy(emojiObj);

            int emojiIndex = (int)data.emoji - 1;
            bool isValidEmoji = ChatMetaManager.Instance.emoticonList.IsValidIndex(emojiIndex);
            if (isValidEmoji)
            {
                GameObject emojiPrefab = ChatMetaManager.Instance.emoticonList[emojiIndex];

                contentBalloon.gameObject.SetActive(false);
                imageBalloon.gameObject.SetActive(true);

                Transform parent = imageBalloon.transform;
                var newEmojiObj = GameObject.Instantiate(emojiPrefab);
                newEmojiObj.transform.SetParent(parent, false);

                emojiObj = newEmojiObj;

                buttonElement = emojiObj.GetComponent<ContextElement>();
                MetaContextElementUtils.SetClickable(buttonElement, () => OnClick(buttonElement));
                OnClick(buttonElement);
            }
            else
            {
                imageBalloon.gameObject.SetActive(false);
                contentBalloon.gameObject.SetActive(true);

                int r = UnityEngine.Random.Range(0, 3);
                string assetName = string.Format("CHAT_EMOTICON_TEXT_{0}", r);

                contentContext.SetText(StringTableUtils.GetString(StringTable.StringTableType.Global, assetName));
            }
        }

        private void OnClick(ContextElement buttonElement)
        {
            var anim = buttonElement.GetComponent<Animator>();
            anim.SetBool("isActive", true);
        }
    }
}
