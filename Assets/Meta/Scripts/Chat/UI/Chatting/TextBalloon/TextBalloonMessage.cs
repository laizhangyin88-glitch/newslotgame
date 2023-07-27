using System.Collections.Generic;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
using UnityEngine.Events;

namespace BagelCode.Chat
{
    public class TextBalloonMessage : TextBalloonBase
    {
        [SerializeField]
        public float addHeight;
        public ContextText contentContext;

        private LongClickButton longClickComp;
        private ContextButton reportButton;


        public override void Init(ChattingController _owner)
        {
            base.Init(_owner);

            // InitReportButton();
        }

        public override void Refresh(ChatMessageData _chatData)
        {
            base.Refresh(_chatData);

            contentContext.SetText(chatData.GetMessage());

            if (!IsMe())
            {
                InitReportButton();

                if (ChatMessenger.Instance.IsReportable(chatData.chatPoll.id))
                {
                    if (reportButton != null)
                    {
                        reportButton.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (reportButton != null)
                    {
                        reportButton.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void InitReportButton()
        {
            var reportArea = mContext.Find("Report Button Area");

            if (reportButton == null)
                reportButton = MetaObjectUtils.MakePrefab<ContextButton>("Chatting Button Report", reportArea.transform);

            reportButton.RemoveAllListener();
            reportButton.AddListenerOnClick((context) =>
                {
                    BI_client_click_chat_message("report");
                    string content = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHAT_REPORT_CONTENT");
                    MetaPopupUtils.OpenYesNo(content, () =>
                        {
                            ChatMessenger.Instance.SetReportable(chatData.chatPoll.id, false);
                            BI_client_click_chat_message("popup");
                            ChatMessenger.Instance.RequestReportAsync(chatData.chatPoll,
                                (res) =>
                                {
                                    BlackboardQueryUtils.AppendMuteUserList(userId);

                                    owner.ReceiveCurrentChatDataList();
                                    owner.RefreshUserProfiles();
                                    owner.Refresh();

                                    if (ApplicationSettings.LogBundle())
                                        Debug.Log("Report Success");
                                },
                                (err) =>
                                {
                                    if (ApplicationSettings.LogBundle())
                                        Debug.Log($"Report Fail -Error Code:{err.errorCode} \nErrorInfo:{err.errorDetailInfo}");
                                }
                            );
                        }
                    );
                }
            );

            if (longClickComp == null)
            {
                longClickComp = mContext.Find("Text Balloon").gameObject.AddComponent<LongClickButton>();
                longClickComp.requiredHoldTime = 0.25f;
            }

            ChatMessenger.Instance.SetReportable(chatData.chatPoll.id, false);

            longClickComp.onLongClick = new UnityEvent();
            longClickComp.onLongClick.AddListener(() =>
                {
                    bool isReportable = ChatMessenger.Instance.IsReportable(chatData.chatPoll.id);
                    isReportable = !isReportable;
                    ChatMessenger.Instance.SetReportable(chatData.chatPoll.id, isReportable);
                    reportButton.gameObject.SetActive(isReportable);
                    if (isReportable)
                    {
                        BI_client_click_chat_message("click");
                    }
                }
            );

        }

        private void BI_client_click_chat_message(string actionType)
        {
            string chatType = owner.CurrentChannelType.ToString().ToLower();
            string language = null;
            if (owner.CurrentChannelType == ChannelType.Global)
            {
                language = ChatMetaManager.Instance.globalChannelInfos[ChatMetaManager.Instance.globalChannelIndex].channelName;
            }

            string message = chatData.chatPoll.GetString();

            Analytics.CustomEvent("client_click_chat_message", new Dictionary<string, object>()
            {
                ["target_user_id"] = chatData.chatPoll.userId,
                ["message"] = message,
                ["chat_type"] = chatType,
                ["language"] = language,
                ["chat_room_id"] = chatData.chatPoll.channelId,
                ["action_type"] = actionType,
                ["chat_message_type"] = chatData.chatPoll.type.ToString().ToLower(),
            }
            );
        }
    }
}
