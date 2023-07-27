using System;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine.UI;

namespace BagelCode.Tasks.Actions
{

    [Category("★ BagelCode/Club")]
    public class UpdateClubContentsMain : ActionTask<ContextElement>
    {
        public BBParameter<ContextElement> memberElement;
        public BBParameter<ContextElement> newsFeedElement;
        public BBParameter<ContextElement> challengeElement;
        public BBParameter<ContextElement> leagueElement;
        public BBParameter<ContextElement> feedInputElement;
        public BBParameter<ContextElement> fullTextElement;
        public BBParameter<ContextElement> joinButtonElement;
        public BBParameter<ContextElement> clubButtonAreaElement;
        public BBParameter<ContextElement> clubContentElement;
        public BBParameter<ContextElement> donateButtonElement;
        public BBParameter<ContextElement> informationBalloonElement;
        public BBParameter<ContextElement> donationBalloonElement;
        public BBParameter<ContextElement> contributionBalloonElement;
        public BBParameter<ContextElement> leaguePointTableBalloonElement;
        public BBParameter<ContextElement> leaguePointBalloonElement;
        public BBParameter<ContextElement> giftsSentBalloonElement;
        public BBParameter<ContextElement> badgeElement;
        public BBParameter<ContextElement> noticeEditButtonElement;
        public BBParameter<ContextElement> noticeEditButtonAreaElement;
        public BBParameter<ContextElement> noticeTextElement;
        public BBParameter<ContextElement> collectAllBalloonElement;
        public BBParameter<ContextElement> collectAllBaseGray;
        public BBParameter<ContextElement> clubDealElement;

        private ContextElement feedMessageInputFieldElement;
        // private ContextElement informationElement;
        private ContextElement noticeEditButtonTextElement;
        private ContextElement newsFeedTitleTextElement;
        private ContextElement informationButtonCollectAllElement;
        private int newsfeedMaxLength;
        private int noticeMaxLength;


        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        protected override string info
        {
            get { return "Update Club Contents Main"; }
        }

        protected override void OnExecute()
        {
            memberElement.value = ContextUtils.FindElement(agent, "Member", ContextSearchingType.ChildrenSearch);
            newsFeedElement.value = ContextUtils.FindElement(agent, "News Feed", ContextSearchingType.ChildrenSearch);
            challengeElement.value = ContextUtils.FindElement(agent, "Challenge", ContextSearchingType.ChildrenSearch);
            leagueElement.value = ContextUtils.FindElement(agent, "League", ContextSearchingType.ChildrenSearch);
            feedInputElement.value = ContextUtils.FindElement(agent, "Post Input", ContextSearchingType.ChildrenSearch);
            memberElement.value.gameObject.SetActive(false);
            newsFeedElement.value.gameObject.SetActive(false);
            challengeElement.value.gameObject.SetActive(false);
            leagueElement.value.gameObject.SetActive(true);
            feedInputElement.value.gameObject.SetActive(false);

            fullTextElement.value = ContextUtils.FindElement(agent, "Button Area/Text Full", ContextSearchingType.FullNameSearch);
            joinButtonElement.value = ContextUtils.FindElement(agent, "Button Area/Button Join", ContextSearchingType.FullNameSearch);
            clubButtonAreaElement.value = ContextUtils.FindElement(agent, "Button Area", ContextSearchingType.ChildrenSearch);
            clubContentElement.value = ContextUtils.FindElement(agent, "Bottom Tab", ContextSearchingType.ChildrenSearch);
            donateButtonElement.value = ContextUtils.FindElement(agent, "Bottom Tab/Button Donate", ContextSearchingType.FullNameSearch);

            informationBalloonElement.value = ContextUtils.FindElement(agent, "Member/Club Information Area/Club Information Speech Bubble", ContextSearchingType.FullNameSearch);
            donationBalloonElement.value = ContextUtils.FindElement(agent, "Member/Donation Area/Donation Speech Bubble", ContextSearchingType.FullNameSearch);
            contributionBalloonElement.value = ContextUtils.FindElement(agent, "Member/Contribution Area/Contribution Speech Bubble", ContextSearchingType.FullNameSearch);
            leaguePointTableBalloonElement.value = ContextUtils.FindElement(agent, "Member/League Point Table Area/Club Speech Balloon League Points", ContextSearchingType.FullNameSearch);
            leaguePointBalloonElement.value = ContextUtils.FindElement(agent, "Member/League Point Area/League Points Speech Bubble", ContextSearchingType.FullNameSearch);
            giftsSentBalloonElement.value = ContextUtils.FindElement(agent, "Member/Gifts Sent/Gifts Sent Area/Gifts Sent Speech Bubble", ContextSearchingType.FullNameSearch);

            feedMessageInputFieldElement = ContextUtils.FindElement(agent, "Post Input/Input Field Enter", ContextSearchingType.FullNameSearch);

            newsfeedMaxLength = BlackboardUtils.FindValue<int>(MainBlackboard.Get(), "values/club/FEED_MESSAGE_MAX_LENGTH");
            MetaContextElementUtils.SetInputFieldAttrribute(feedMessageInputFieldElement, newsfeedMaxLength, InputField.ContentType.Standard);

            badgeElement.value = ContextUtils.FindElement(agent, "Bottom Tab/Tab News Feed/Badge", ContextSearchingType.FullNameSearch);

            noticeTextElement.value = ContextUtils.FindElement(agent, "News Feed/Text News Feed Club Notice", ContextSearchingType.FullNameSearch);
            newsFeedTitleTextElement = ContextUtils.FindElement(agent, "News Feed/Text News Feed Title", ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SetText(newsFeedTitleTextElement, StringTableUtils.GetString(tableType, "CLUB_NEWS_FEED_SUB_TITLE_TEXT"));
            collectAllBalloonElement.value = ContextUtils.FindElement(agent, "News Feed/Button Area/Button Collect All/Information", ContextSearchingType.FullNameSearch);
            informationButtonCollectAllElement = ContextUtils.FindElement(agent, "News Feed/Button Area/Button Collect All/Information/Text", ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SetText(informationButtonCollectAllElement, StringTableUtils.GetString(tableType, "INBOX_COLLECT_ALL_BALLOON"));
            collectAllBaseGray.value = ContextUtils.FindElement(agent, "News Feed/Button Area/Button Collect All/Base Gray", ContextSearchingType.FullNameSearch);

            clubDealElement.value = ContextUtils.FindElement(clubContentElement.value, "Bottom Tab/Button Club Deal", ContextSearchingType.FullNameSearch);

            EndAction();
        }
    }
}