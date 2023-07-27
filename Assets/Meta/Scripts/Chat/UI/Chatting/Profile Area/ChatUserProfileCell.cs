using System;
using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Chat
{
    public class ChatUserProfileCell : MonoBehaviour
    {
        public ChattingController owner { get; private set; }

        public ChatUserProfileInfo userProfile { get; private set; }

        private ContextButton mContextButton =>GetComponent<ContextButton>();

        private ContextElement rootElement;
        private ContextElement profileAreaElement;
        private ContextElement profileImageElement;
        private ContextElement countryIconAreaElement;
        private ContextElement countryIconElement;
        private ContextElement iconOnlineElement;
        private ContextElement iconOfflineElement;
        private ContextElement iconFriendsElement;
        private ContextElement iconClubElement;
        private ContextElement facebookIconAreaElement;
        private ContextElement userNameElement;

        private GameObject countryIconObj = null;

        private Blackboard profileTierBB;

        private GameObject facebookIcon = null;

        private void Start()
        {
            mContextButton.UpdateContext();
            mContextButton.RemoveAllListener();
            mContextButton.AddListenerOnClick((context) =>
            {
                if (userProfile == null) return;

                var profileObj = MetaObjectUtils.MakeScene("Profile Popup Scene", PopupManager.Instance.transform.Find("Area"));
                var profilePopupBB = profileObj.GetComponent<Blackboard>();
                profileObj.SetActive(false);

                profilePopupBB.SetValue("_userId", userProfile.userId);
                profilePopupBB.SetValue("bi_fromType", "chat");

                if (userProfile.userId == BlackboardQueryUtils.GetMyUserId())
                    profilePopupBB.SetValue("isMe", true);

                PopupManager.Instance.Open(profileObj);
                profileObj.SetActive(true);
                // StartCoroutine(WaitCloseProfilePopup(profilePopup.gameObject));
            });
        }

        public void Init(ChatUserProfileInfo _userProfile,ChattingController _owner)
        {
            owner = _owner;
            userProfile = _userProfile;
            Refresh(_userProfile);
        }

        public void Refresh(ChatUserProfileInfo setProfile)
        {
            userProfile = setProfile;
            UpdateProfile();
        }

        public void InitContext()
        {
            if(rootElement == null)
            {
                rootElement = GetComponent<ContextElement>();

                profileAreaElement = ContextUtils.FindElement(rootElement, "Profile Area", ContextSearchingType.ChildrenSearch);
                var profilePicture = MetaObjectUtils.MakePrefab<ContextElement>("Profile Picture Chatting", profileAreaElement.transform);
                profilePicture.UpdateContext();
                profileTierBB = profilePicture.gameObject.GetComponent<Blackboard>();
                // profilePictureElement = ContextUtils.FindElement(rootElement, "Profile Picture Small", ContextSearchingType.ChildrenSearch);
                profileImageElement = ContextUtils.FindElement(profilePicture, "Image", ContextSearchingType.ChildrenSearch);
                countryIconAreaElement = ContextUtils.FindElement(profilePicture, "Country Icon Area", ContextSearchingType.ChildrenSearch); //

                if(countryIconElement == null)
                {
                    countryIconObj = MetaObjectUtils.MakePrefab("Icon Image", countryIconAreaElement.transform);
                    countryIconElement = countryIconObj.GetComponent<ContextElement>();
                }

                iconOnlineElement = ContextUtils.FindElement(rootElement, "Icon Online", ContextSearchingType.ChildrenSearch);
                iconOfflineElement = ContextUtils.FindElement(rootElement, "Icon Offline", ContextSearchingType.ChildrenSearch);
                iconFriendsElement = ContextUtils.FindElement(rootElement, "Icon Friend", ContextSearchingType.ChildrenSearch);
                iconClubElement = ContextUtils.FindElement(rootElement, "Icon Club", ContextSearchingType.ChildrenSearch);
                facebookIconAreaElement = ContextUtils.FindElement(rootElement, "Icon Facebook Area", ContextSearchingType.ChildrenSearch);
                userNameElement = ContextUtils.FindElement(rootElement, "User Name Text", ContextSearchingType.ChildrenSearch);
            }
        }

        public void UpdateProfile()
        {
            InitContext();

            if (userProfile != null)
            {
                MetaContextElementUtils.SetWebImage(profileImageElement, userProfile.profileUrl, CacheType.MemCache, false, null);
                // profileImage.ApplyWebImage(userProfile.profileUrl);

                iconOnlineElement.gameObject.SetActive(userProfile.useOnlieMark && userProfile.isOnline);
                iconOfflineElement.gameObject.SetActive(userProfile.useOnlieMark && !userProfile.isOnline);

                iconFriendsElement.gameObject.SetActive(false);
                iconClubElement.gameObject.SetActive(false);
                facebookIconAreaElement.gameObject.SetActive(false);

                // priority, club > facebook > friend
                if(!userProfile.isMe)
                {
                    if(userProfile.IsClub())
                    {
                        iconClubElement.gameObject.SetActive(true);
                    }
                    else if(userProfile.IsFacebook())
                    {
                        if(facebookIcon == null)
                        {
                            facebookIcon = MetaObjectUtils.MakePrefab("Icon Facebook", facebookIconAreaElement.transform);
                        }

                        facebookIconAreaElement.gameObject.SetActive(true);
                    }
                    else if(userProfile.IsFriend())
                    {
                        iconFriendsElement.gameObject.SetActive(true);
                    }
                }

                profileTierBB.gameObject.SetActive(false);
                BlackboardUtils.SetOrCreateValue<int>(profileTierBB, "tierGroup", userProfile.tierGroup);
                profileTierBB.gameObject.SetActive(true);

                // Country
                if(CountryUtils.ExistCountryCode(userProfile.countryCode))
                {
                    countryIconAreaElement.gameObject.SetActive(true);
                    MetaContextElementUtils.SetContextCountryImage(countryIconElement, userProfile.countryCode);
                }
                else
                {
                    countryIconAreaElement.gameObject.SetActive(false);
                }

                //Text Display
                MetaContextElementUtils.SetTextGlobal(userNameElement, "CHAT_PROFILE_LIST_NAME_TEXT", userProfile.name);
            }
            else
            {
                MetaContextElementUtils.SetSprite(profileImageElement, null);
                iconOnlineElement.gameObject.SetActive(false);
                iconOfflineElement.gameObject.SetActive(false);
                iconClubElement.gameObject.SetActive(false);
                iconFriendsElement.gameObject.SetActive(false);
                facebookIconAreaElement.gameObject.SetActive(false);
                countryIconAreaElement.gameObject.SetActive(false);
                BlackboardUtils.SetOrCreateValue<int>(profileTierBB, "tierGroup", 0);
            }
        }
    }
}
