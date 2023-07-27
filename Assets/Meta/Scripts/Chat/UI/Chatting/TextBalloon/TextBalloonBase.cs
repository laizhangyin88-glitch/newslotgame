using System.Collections;
using System.Linq;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode.Chat
{
    /// <summary>
    /// Text Balloon GameObject Base Class
    /// </summary>
    public abstract class TextBalloonBase : MonoBehaviour
    {
        public bool usingProfile;
        public string userId;

        public ChattingController owner { get; private set; }
        public ContextElement mContext;

        [SerializeField]
        private ContextText userNameContext;

        [ReadOnly]
        [ShowInInspector]
        public ChatMessageData chatData { get; private set; }

        private ContextElement deleteArea;
        private ContextElement resendArea;

        private ContextButton deleteButton;
        private ContextButton resendButton;

        private ContextElement profileAreaElement;
        private ContextElement profileElement;
        private ContextElement profileImageElement;
        private Blackboard profileElementBB;

        public bool IsMe()
        {
            return chatData.isMe;
        }

        private void Start()
        {
            if (usingProfile)
            {
                profileAreaElement = ContextUtils.FindElement(mContext, "Profile Area", ContextSearchingType.ChildrenSearch);

                if (profileAreaElement != null)
                {
                    profileAreaElement.gameObject.SetActive(false);
                    var nonDraw = profileAreaElement.gameObject.GetComponent<NonDrawingGraphic>();
                    if (nonDraw == null)
                        nonDraw = profileAreaElement.gameObject.AddComponent<NonDrawingGraphic>();

                    var button = profileAreaElement.gameObject.GetComponent<PIDButton>();
                    if (button == null)
                        button = profileAreaElement.gameObject.AddComponent<PIDButton>();

                    button.transition = Selectable.Transition.None;

                    // Dummy. Error Block.
                    button.onInteractableChanged = new UnityBoolEvent();

                    button.scaleFactor = 0.9f;
                    button.onClick.RemoveListener(ClickProfile);
                    button.onClick.AddListener(ClickProfile);

                    profileAreaElement.gameObject.SetActive(true);
                }
            }
        }

        public virtual void Init(ChattingController _owner)
        {
            owner = _owner;

            mContext = GetComponent<ContextElement>();
            mContext.UpdateContext();

            deleteArea = mContext.Find("Delete Button Area");
            resendArea = mContext.Find("Resend Button Area");

            if (usingProfile)
            {
                profileAreaElement = ContextUtils.FindElement(mContext, "Profile Area", ContextSearchingType.ChildrenSearch);
                profileElement = MetaObjectUtils.MakePrefab<ContextElement>("Profile Picture Chatting Balloon", profileAreaElement.transform);
                mContext.UpdateContext(true);
                profileImageElement = ContextUtils.FindElement(profileElement, "Image", ContextSearchingType.ChildrenSearch);
                profileElementBB = profileElement.gameObject.GetComponent<Blackboard>();
            }
        }

        private void ClickProfile()
        {
            if (usingProfile == false) return;
            if (chatData == null) return;
            if (chatData.chatPoll == null) return;
            if (chatData.chatPoll.profile == null) return;
            if (string.IsNullOrEmpty(chatData.chatPoll.profile.userId)) return;

            var profileObj = MetaObjectUtils.MakeScene("Profile Popup Scene", PopupManager.Instance.transform.Find("Area"));
            var profilePopupBB = profileObj.GetComponent<Blackboard>();
            profileObj.SetActive(false);

            profilePopupBB.SetValue("_userId", chatData.chatPoll.profile.userId);
            profilePopupBB.SetValue("bi_fromType", "in_chat");

            if (chatData.chatPoll.profile.userId == BlackboardQueryUtils.GetMyUserId())
                profilePopupBB.SetValue("isMe", true);

            PopupManager.Instance.Open(profileObj);
            profileObj.SetActive(true);
        }

        public virtual void Refresh(ChatMessageData data)
        {
            chatData = data;
            chatData.ownerBalloon = this;
            userId = data.chatPoll.userId;

            UpdateRetry();
            UpdateProfile();
        }

        private IEnumerator WaitResponse()
        {
            yield return new WaitUntil(() => chatData == null || chatData.isResponse);

            if (chatData != null)
            {
                UpdateRetry();
                // Refresh(chatData);
            }
        }

        private void UpdateRetry()
        {
            if (!IsMe()) return;
            if (deleteArea != null)
            {
                if (deleteButton == null)
                    deleteButton = MetaObjectUtils.MakePrefab<ContextButton>("Button Delete", deleteArea.transform);

                deleteButton.RemoveAllListener();
                deleteButton.AddListenerOnClick(
                    (context) =>
                    {
                        owner.RemoveChatPoll(chatData);
                    }
                );

                deleteArea.gameObject.SetActive(chatData.isResponse && !chatData.isSuccess);
            }

            if (resendArea != null)
            {
                if (resendButton == null)
                    resendButton = MetaObjectUtils.MakePrefab<ContextButton>("Button Resend", resendArea.transform);

                resendButton.RemoveAllListener();
                resendButton.AddListenerOnClick(
                    (context) =>
                    {
                        // owner.chatDataList.RemoveAll(x => x.chatPoll.id == chatData.chatPoll.id);
                        owner.ResendChatPoll(chatData);
                        // owner.Refresh();
                    }
                );

                resendArea.gameObject.SetActive(chatData.isResponse && !chatData.isSuccess);
            }
        }

        private void UpdateProfile()
        {
            if (userNameContext != null)
            {
                if (chatData.chatPoll.profile != null)
                {
                    userNameContext.SetGlobalText("CHAT_PROFILE_LIST_NAME_TEXT", chatData.chatPoll.profile.name);
                }
                else
                {
                    userNameContext.SetGlobalText("CHAT_PROFILE_LIST_NAME_TEXT", "Unknown");
                }
            }

            if (profileElement != null && chatData.chatPoll.profile != null)
            {
                MetaContextElementUtils.SetWebImage(profileImageElement, chatData.chatPoll.profile.profileUrl, CacheType.MemCache, false, null);
                profileElement.gameObject.SetActive(false);
                BlackboardUtils.SetOrCreateValue<int>(profileElementBB, "tierGroup", TierUtils.GetTierGroup(chatData.chatPoll.profile.tier));
                profileElement.gameObject.SetActive(true);
            }
        }


        private static int GetWidthOfMessage(Text textComp, string message)
        {
            int totalWidth = 0;

            Font font = textComp.font; //text is my UI text
            font.RequestCharactersInTexture(message);

            foreach (char c in message)
            {
                CharacterInfo characterInfo = font.characterInfo.FirstOrDefault(x => x.index == c);
                totalWidth += characterInfo.advance;
            }

            return totalWidth;
        }

        public void SendFailed(ChatMessageData failedData)
        {
            if (chatData == null) return;
            if (failedData.chatPoll.id == chatData.chatPoll.id)
            {
                UpdateRetry();
            }
        }

        protected void SetUserName(string key, string name)
        {
            if (userNameContext != null)
                userNameContext.SetGlobalText(key, name);
        }

        protected void SetUserName(string name)
        {
            if (userNameContext != null)
                userNameContext.SetText(name);
        }
    }
}