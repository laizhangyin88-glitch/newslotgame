using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode.Chat {
    /// <summary>
    /// Chatting Channel Type Tab (Chatting Prefab childs)
    /// </summary>
    public class ChatToggleTab : MonoBehaviour {
        public ChattingController owner;
        public ChannelType channelType;
        public Button mButton => GetComponent<Button>();
        
        
        private ContextElement mContext => GetComponent<ContextElement>();


        void Start() {
            mContext.UpdateContext();
            mButton.onClick.AddListener(() => {
                if (owner.CurrentChannelType == channelType) return;
                StartCoroutine(owner.ChangeChannelTypeAsync(channelType));
            });
            owner.OnChannelChanged += OnChannelChanged;

            UpdateInteractable(false);
        }
        public void OnChannelChanged(ChannelType _channelType) {
            if (_channelType == channelType) {
                UpdateInteractable(true);
            } else {
                UpdateInteractable(false);
            }
        }

        public void UpdateInteractable(bool isActive) {
            mButton.interactable = ChatMetaManager.Instance.IsValidate(channelType);
            if (!ChatMetaManager.Instance.IsValidate(channelType)) {
                GetComponent<Animator>().SetBool("Disable", true);
            } else {
                GetComponent<Animator>().SetBool("Disable", false);
            }
            GetComponent<Animator>().SetBool("Active", isActive);
        }
    }
}