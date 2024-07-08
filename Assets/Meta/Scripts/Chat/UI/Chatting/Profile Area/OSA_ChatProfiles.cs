using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BagelCode.ClientModels;
using System;
using Com.ForbiddenByte.OSA.Core;

namespace BagelCode.Chat
{
    public class OSA_ChatProfiles :  OSA<ChatProfileParams, ProfileViewHolder>
    {
        public ChattingController owner;

        protected override ProfileViewHolder CreateViewsHolder(int itemIndex)
        {
            var userProfile = owner.UserProfiles[itemIndex];
            ProfileViewHolder viewHolder = new ProfileViewHolder();
            var profileCell = MetaObjectUtils.MakePrefab<ChatUserProfileCell>("Chatting User Cell", Content);
            profileCell.Init(userProfile, owner);
            viewHolder.profileCell = profileCell;
            viewHolder.root = profileCell.GetComponent<RectTransform>();

            return viewHolder;
        }

        protected override void UpdateViewsHolder(ProfileViewHolder newOrRecycled)
        {
            var userProfile = owner.UserProfiles[newOrRecycled.ItemIndex];
            newOrRecycled.UpdateView(userProfile);
        }

        protected override bool IsRecyclable(ProfileViewHolder potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, double sizeOfItemThatWillBecomeVisible)
        {
            return true;
        }
    }

    [Serializable]
    public class ChatProfileParams : BaseParams
    {
    }

    [Serializable]
    public class ProfileViewHolder : BaseItemViewsHolder
    {
        public ChatUserProfileCell profileCell;

        public void UpdateView(ChatUserProfileInfo _userProfile)
        {
            profileCell.Refresh(_userProfile);
        }
    }
}
