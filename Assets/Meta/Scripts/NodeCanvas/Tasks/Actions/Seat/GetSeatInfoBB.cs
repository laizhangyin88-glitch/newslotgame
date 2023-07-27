using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Seat")]
    public class GetSeatInfoBB : ActionTask
    {
        private enum SeatBadgeType
        {
            None = 0,
            Facebook,
            Friend,
            Club,
            ClubTier
        }

        public BBParameter<Blackboard> userProfileBB;

        public BBParameter<GameObject> badgeArea;
        public BBParameter<List<GameObject>> badgeList;

        private ContextElement countryIconElement;
        private ContextElement countryIconAreaElement;

        private ContextElement clubTierIconElement;
        private ContextElement clubTierIconAreaElement;
        private ContextElement clubTierIconBadgeAreaElement;

        private GameObject clubTierIcon;
        private GameObject clubTierBadge;

        private bool isInit = false;
        private bool useClubTierIcon = false;
        private int clubLeagueTier;
        private int prevClubLeagueTier;

        protected override string info
        {
            get { return "Get Seat Info BB(Update)"; }
        }

        private void InitProperty()
        {
            if(isInit) return;

            var rootElement = agent.gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            var pictureAreaElement = ContextUtils.FindElement(rootElement, "Picture", ContextSearchingType.ChildrenSearch);
            countryIconAreaElement = ContextUtils.FindElement(pictureAreaElement, "Country Icon Area", ContextSearchingType.ChildrenSearch);
            var imageIconObj = MetaObjectUtils.MakePrefab("Icon Image", countryIconAreaElement.transform);
            countryIconElement = imageIconObj.GetComponent<ContextElement>();

            clubTierIconElement = ContextUtils.FindElement(rootElement, "Club Tier", ContextSearchingType.ChildrenSearch);
            if(clubTierIconElement != null)
            {
                clubTierIconAreaElement = ContextUtils.FindElement(clubTierIconElement, "Icon Area", ContextSearchingType.ChildrenSearch);
                clubTierIconBadgeAreaElement = ContextUtils.FindElement(clubTierIconElement, "Base", ContextSearchingType.ChildrenSearch);
            }

            prevClubLeagueTier = -1;

            isInit = true;
        }

        protected override void OnExecute()
        {
            InitProperty();

            SeatBadgeType badgeType = SeatBadgeType.None;
            useClubTierIcon = false;

            var clubID = userProfileBB.value.GetValue<long>("clubId");
            clubLeagueTier = -1;

            if (BlackboardQueryUtils.IsMyClubMember(clubID))
            {
                badgeType = SeatBadgeType.Club;
            }
            else
            {
                var userID = userProfileBB.value.GetValue<string>("userId");

                List<Blackboard> friendsList = BlackboardQueryUtils.GetFriendList(true);
                Blackboard friendInfo = BlackboardQueryUtils.GetFriendInfo(userID, friendsList);

                if(friendInfo != null)
                {
                    var friendType = friendInfo.GetValue<BagelCode.ClientModels.FriendType>("type");
                    switch(friendType)
                    {
                        case BagelCode.ClientModels.FriendType.FACEBOOK:
                            badgeType = SeatBadgeType.Facebook;
                            break;
                        default:
                            badgeType = SeatBadgeType.Friend;
                            break;
                    }
                }
                else
                {
                    if(clubID > 0)
                    {
                        clubLeagueTier = userProfileBB.value.GetValue<int>("clubLeagueTier");
                        badgeType = SeatBadgeType.ClubTier;
                        useClubTierIcon = true;
                    }
                }
            }

            badgeArea.value.SetActive(true);

            for(int i=0; i<badgeList.value.Count; ++i)
            {
                if((int)badgeType == i+1)
                    badgeList.value[i].SetActive(true);
                else
                    badgeList.value[i].SetActive(false);
            }

            UpdateTierIcon();

            // if(!useClubTierIcon || prevClubLeagueTier != clubLeagueTier)
            // {
            //     if(clubTierIcon != null)
            //     {
            //         GameObject.Destroy(clubTierIcon);
            //         clubTierIcon = null;
            //     }

            //     if(clubTierBadge != null)
            //     {
            //         GameObject.Destroy(clubTierBadge);
            //         clubTierBadge = null;
            //     }
            // }

            // if(useClubTierIcon)
            // {
            //     prevClubLeagueTier = clubLeagueTier;

            //     if(clubTierIconAreaElement != null && clubTierIcon == null)
            //         clubTierIcon = MetaIconUtils.MakeClubTierIconSmallObject(clubLeagueTier, clubTierIconAreaElement.transform, null);

            //     if(clubTierIconBadgeAreaElement != null && clubTierBadge == null)
            //         clubTierBadge = MetaIconUtils.MakeClubTierSmallBadgeObject(clubLeagueTier, clubTierIconBadgeAreaElement.transform, null);
            // }

            var countryCode = BlackboardUtils.GetOrCreateVariable<string>(userProfileBB.value, "countrySelected");
            if(CountryUtils.ExistCountryCode(countryCode.value))
            {
                countryIconAreaElement.gameObject.SetActive(true);
                MetaContextElementUtils.SetContextCountryImage(countryIconElement, countryCode.value);
            }
            else
            {
                countryIconAreaElement.gameObject.SetActive(false);
            }

            EndAction(true);
        }

        private void UpdateTierIcon()
        {
            if(!useClubTierIcon || prevClubLeagueTier != clubLeagueTier)
            {
                if(clubTierIcon != null)
                {
                    GameObject.Destroy(clubTierIcon);
                    clubTierIcon = null;
                }

                if(clubTierBadge != null)
                {
                    GameObject.Destroy(clubTierBadge);
                    clubTierBadge = null;
                }
            }

            if(useClubTierIcon)
            {
                if(clubTierIconAreaElement != null && clubTierIcon == null)
                    clubTierIcon = MetaIconUtils.MakeClubTierIconSmallObject(clubLeagueTier, clubTierIconAreaElement.transform, null);

                // TODO: Legacy. will be remove this all dependecy code.
                // if(clubTierIconBadgeAreaElement != null && clubTierBadge == null)
                //     clubTierBadge = MetaIconUtils.MakeClubTierSmallBadgeObject(clubLeagueTier, clubTierIconBadgeAreaElement.transform, null);
            }

            prevClubLeagueTier = clubLeagueTier;
        }
    }
}

