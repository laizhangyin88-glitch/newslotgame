using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BagelCode.ClientModels;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;
using Action = System.Action;

namespace BagelCode.Chat
{
    public class ChattingArea : MonoBehaviour
    {
        public ChattingController owner;
        public OSA_Chatting osaChatting {
            get {
                if (owner.CurrentChannelType == ChannelType.Global) {
                    return globalOsaChatting;
                } else {
                    return _osaChatting;
                }
            }
        }
        [SerializeField]
        private OSA_Chatting _osaChatting;
        [SerializeField]
        private OSA_Chatting globalOsaChatting;

        private float refreshHeight = 50f;


        public void Refresh()
        {
            // Debug.LogError("Refresh");
            osaChatting.ResetItems(owner.ChatDataList.Count, false, true);
            // osaChatting.MoveToLast();
        }

        public void ScrollToLastAndRefresh()
        {
            // Debug.LogError("ScrollToLastAndRefresh");
            osaChatting.ResetItems(owner.ChatDataList.Count, false, true);
            if (owner.ChatDataList.Count == 0) return;
            osaChatting.MoveToLast();
        }

        public void ScrollToFirstAndRefresh(int insertCount)
        {
            // Debug.LogError("ScrollToFirstAndRefresh");
            osaChatting.InsertToFirstItems(insertCount, false, true);
        }

        public void InsertMyChatItem(int insertIndex)
        {
            // Debug.LogError(owner.chatDataList.Count);
            InsertItem(insertIndex);
            osaChatting.MoveToLast(true);
        }

        public void InsertPollChat(int insertIndex)
        {
            // Debug.LogError(owner.chatDataList.Count);

            bool moveLast = false;
            if(osaChatting.GetVirtualAbstractNormalizedScrollPosition() < 0.05f)
                moveLast = true;

            InsertItem(insertIndex);

            if(moveLast)
                osaChatting.MoveToLast(true);
        }

        public void InsertItem(int insertIndex)
        {
            osaChatting.InsertItem(insertIndex, false, true);
        }

        public void RemoveItem(int removeIndex)
        {
            // Debug.LogError(string.Format("Remove Index {0}", removeIndex));
            osaChatting.RemoveItem(removeIndex, false, true);
        }

        private void Start()
        {
            _osaChatting.SubscribeEndDragEvent(RefreshClubRecent);
            globalOsaChatting.SubscribeEndDragEvent(RefreshGlobalScroll);

            owner.OnChannelChanged += (_channelType) =>
            {
                if (_channelType == ChannelType.Global)
                {
                    _osaChatting.gameObject.SetActive(false);
                    globalOsaChatting.gameObject.SetActive(true);
                }
                else
                {
                    _osaChatting.gameObject.SetActive(true);
                    globalOsaChatting.gameObject.SetActive(false);
                }
            };
        }

        private void Destroy()
        {
            _osaChatting.UnSubscribeEndDragEvent(RefreshClubRecent);
            globalOsaChatting.UnSubscribeEndDragEvent(RefreshGlobalScroll);
        }

        public void RefreshGlobalScroll(PointerEventData eventData)
        {
            if(owner.CurrentChannelType != ChannelType.Global) return;
            if(IsOverScroll())
            {
                owner.RequestRecentAsync();
            }
        }

        public void RefreshClubRecent(PointerEventData eventData)
        {
            if(owner.CurrentChannelType != ChannelType.Club) return;
            if(IsOverScroll())
            {
                owner.RequestRecentAsync();
            }
        }

        private bool IsOverScroll()
        {
            if(osaChatting.GetNormalizedPosition() >= 1f 
                && osaChatting.ContentVirtualInsetFromViewportStart >= refreshHeight)
            {
                return true;
            }

            return false;
        }

    }
}
