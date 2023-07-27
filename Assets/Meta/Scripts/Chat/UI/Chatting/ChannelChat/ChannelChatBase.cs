using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using SlotMaker;
using Sirenix.OdinInspector;
using BagelCode.ClientModels;
using BagelCode;
using System.Linq;
using System;
using NodeCanvas.Framework;

namespace BagelCode.Chat {
    [System.Serializable]
    public abstract class ChannelChatBase : MonoBehaviour {
        public ChattingController owner;
        [ShowInInspector]
        public abstract ChannelType channelType { get; }
        public ContextElement mContext => GetComponent<ContextElement>();

        protected virtual void Start() {
            mContext.UpdateContext();
            owner.OnChannelChanged += (_channelType) => {
                if (channelType == _channelType) {
                    OnChannelSelected();
                } else {
                    OnChannelUnselected();
                }
            };

        }
        public virtual void OnChannelSelected() { }
        public virtual void OnChannelUnselected() { }

    }
}