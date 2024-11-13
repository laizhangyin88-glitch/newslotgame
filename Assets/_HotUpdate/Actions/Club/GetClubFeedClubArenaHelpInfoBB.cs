using System;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Club")]
    public class GetClubFeedClubArenaHelpInfoBB : ActionTask<Blackboard>
    {
        public BBParameter<int> cellIndex;
        public BBParameter<List<GameObject>> bgList; // 0, 1

        public BBParameter<string> clubFeedInfoValue;

        public BBParameter<string> messageText;
        public BBParameter<string> likeButtonText;
        public BBParameter<string> leftTimeAgoText;

        private ContextElement rootElement;
        private ContextElement timeTextElement;
        private ContextElement nameTextElement;
        private ContextElement messageTextElement;
        private ContextElement imageElement;

        private ContextElement buttonHelpElement;
        private ContextElement buttonClearElement;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private GameObject profileObj = null;
        private bool isInit = false;

        private void InitProperty()
        {
            if (isInit) return;

            rootElement = agent.gameObject.GetComponent<ContextElement>();

            timeTextElement = ContextUtils.FindElement(rootElement, "Text Time", ContextSearchingType.ChildrenSearch);
            nameTextElement = ContextUtils.FindElement(rootElement, "Text Name", ContextSearchingType.ChildrenSearch);
            messageTextElement = ContextUtils.FindElement(rootElement, "Text Context", ContextSearchingType.ChildrenSearch);

            buttonHelpElement = ContextUtils.FindElement(rootElement, "Button Help", ContextSearchingType.ChildrenSearch);
            buttonClearElement = ContextUtils.FindElement(rootElement, "Clear", ContextSearchingType.ChildrenSearch);

            ContextElement profileAreaElement = ContextUtils.FindElement(rootElement, "Profile Picture Area", ContextSearchingType.ChildrenSearch);
            profileObj = MetaObjectUtils.MakePrefab("lobby0", "Profile Picture Small", profileAreaElement.transform, null, "Profile Picture");
            ContextElement profileElement = profileObj.GetComponent<ContextElement>();
            profileElement.UpdateContext();

            imageElement = ContextUtils.FindElement(profileElement, "Image", ContextSearchingType.ChildrenSearch);

            isInit = true;
        }

        protected override void OnExecute()
        {
            InitProperty();

            var clubFeedInfo = BlackboardUtils.FindVariable<Blackboard>(agent, clubFeedInfoValue.value);
            EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo();

            if (clubFeedInfo != null)
            {
                var meID = BlackboardUtils.FindVariable<string>(null, "/me/userId");

                string userName = clubFeedInfo.value.GetValue<string>("name");
                string feedUserID = clubFeedInfo.value.GetValue<string>("userId");

                bool isMeFeed = feedUserID == meID.value;

                bool isEnableFeed = false;
                if (!isMeFeed)
                    isEnableFeed = clubFeedInfo.value.GetValue<int>("like") == 0;

                // Event id check
                if (eventInfo == null || eventInfo.id != clubFeedInfo.value.GetValue<int>("eventId"))
                    isEnableFeed = false;

                buttonHelpElement.gameObject.SetActive(isEnableFeed);
                buttonClearElement.gameObject.SetActive(!isEnableFeed);

                var createdTimestamp = BlackboardUtils.GetOrCreateVariable<long>(clubFeedInfo.value, "createdTimestamp").value;
                leftTimeAgoText.value = ClubUtils.GetClubFeedLeftTimeText(TimeUtils.GetTimeStamp() - createdTimestamp);

                MetaContextElementUtils.SetText(timeTextElement, leftTimeAgoText.value);

                bool error = false;
                var rewardEnergy = GetRewardEnergy(clubFeedInfo.value);
                if (rewardEnergy != 0)
                    likeButtonText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_LIKE_WITH_ENERGY", out error, rewardEnergy);
                else
                    likeButtonText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_LIKE", out error);

                messageText.value = StringTableUtils.GetString(tableType, "CLUB_ARENA_CLUB_NEWS_FEED_HELP_TEXT", userName);
                MetaContextElementUtils.SetText(nameTextElement, userName);
                MetaContextElementUtils.SetText(messageTextElement, messageText.value);

                MetaContextElementUtils.SetWebImage(imageElement, clubFeedInfo.value.GetValue<string>("profileUrl"), CacheType.MemCache, true, null);

                UpdateBG();
            }

            EndAction();
        }

        private void UpdateBG()
        {
            int cellStyle = cellIndex.value % 2;

            for (int i = 0; i < bgList.value.Count; ++i)
            {
                bgList.value[i].SetActive(i == cellStyle);
            }
        }

        private long GetRewardEnergy(Blackboard clubFeedInfo)
        {
            long rewardEnergy = 0;

            var rewardBB = BlackboardUtils.FindVariable<Blackboard>(clubFeedInfo, "reward");
            if (rewardBB != null && rewardBB.value != null)
            {
                var rewardType = rewardBB.value.GetValue<RewardType>("type");

                if (rewardType == RewardType.CLUB_ARENA_ENERGY)
                    rewardEnergy = rewardBB.value.GetValue<long>("energy");
            }

            return rewardEnergy;
        }
    }
}