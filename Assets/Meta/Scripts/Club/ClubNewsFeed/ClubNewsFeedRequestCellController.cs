using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public abstract class ClubNewsFeedRequestCellController : ClubNewsFeedCellController
    {
        private ContextElement receivedAllTextElement;
        private ContextElement myRequestTextElement;
        private ContextElement buttonGiftAnchorElement;
        private ContextElement giftButtonElement;

        protected int requirement;

        protected override void InitProperty()
        {
            base.InitProperty();

            receivedAllTextElement = ContextUtils.FindElement(root, "Received Area/Text Received All", FULL);
            myRequestTextElement = ContextUtils.FindElement(root, "Received Area/Text My Request", FULL);
            buttonGiftAnchorElement = ContextUtils.FindElement(root, "Received Area/Button Gift Anchor", FULL);
            giftButtonElement = ContextUtils.FindElement(buttonGiftAnchorElement, "Button Gift", CHILDREN);
        }
        protected override void UpdateCellData()
        {
            base.UpdateCellData();

            // Profile
            string profileUrl = feedInfo.GetValue<string>("profileUrl");
            var imageElement = ContextUtils.FindElement(root, "Profile Picture Area/Profile Picture Small/Image", FULL);
            MetaContextElementUtils.SetWebImage(imageElement, profileUrl, CacheType.FileCache, true, null);

            // Tier
            int tier = feedInfo.GetValue<int>("tier");
            int tierGroup = TierUtils.GetTierGroup(tier);
            var profileElement = ContextUtils.FindElement(root, "Profile Picture Area/Profile Picture Small", FULL);
            MetaContextElementUtils.SetPropertySafty(profileElement, tierGroup);

            // Name
            string userId = feedInfo.GetValue<string>("userId");
            bool isMe = userId == BlackboardQueryUtils.GetMyUserId();
            if (isMe)
            {
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Text Name", "HIDDEN_OBJECTS_NEWS_FEED_ME", CHILDREN);
            }
            else
            {
                string name = feedInfo.GetValue<string>("name");
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Text Name", "HIDDEN_OBJECTS_NEWS_FEED_NICKNAME", CHILDREN, name);
            }

            // Title
            string title = GetTitleText();
            MetaContextElementUtils.SimpleSetText(root, "Text Requesting", title);

            // Received
            string receivedText = GetReceivedText();
            MetaContextElementUtils.SimpleSetText(root, "Text Received", receivedText);

            // Progress
            int like = feedInfo.GetValue<int>("like");
            int max = feedInfo.GetValue<int>("capacity");
            int receive = Mathf.Min(like, max);
            float ratio = (float)receive / max;
            var sliderElement = ContextUtils.FindElement(root, "Progress Bar", CHILDREN);
            MetaContextElementUtils.SetSliderValue(sliderElement, ratio);
            MetaContextElementUtils.SimpleSetTextGlobal(sliderElement, "Text Progress Bar", "A_PER_B", CHILDREN, receive, max);

            // Gift
            bool isMyRequest = isMe;
            bool isRecevedAll = receive >= max;
            MetaContextElementUtils.SetActive(receivedAllTextElement, isRecevedAll);
            MetaContextElementUtils.SetActive(myRequestTextElement, !isRecevedAll && isMyRequest);
            MetaContextElementUtils.SetActive(buttonGiftAnchorElement, !isRecevedAll && !isMyRequest);
            if (isMyRequest)
            {
                MetaContextElementUtils.SetTextGlobal(myRequestTextElement, "CLUB_NEWS_FEED_REQUEST_STATE_TEXT_01");
            }
            else if(isRecevedAll)
            {
                MetaContextElementUtils.SetTextGlobal(receivedAllTextElement, "CLUB_NEWS_FEED_REQUEST_STATE_TEXT_00");
            }
            else
            {
                int own = HiddenObjects.HiddenObjects.Utils.Finder;
                requirement = feedInfo.GetValue<int>("requirement");

                var pidButton = giftButtonElement.GetComponent<PIDButton>();
                bool buttonState = own > 0 && requirement > 0;
                pidButton.interactable = !buttonState;
                pidButton.interactable = buttonState;

                MetaContextElementUtils.SimpleSetTextGlobal(giftButtonElement, "Text", "BUTTON_GIFT", CHILDREN);

                MetaContextElementUtils.SimpleSetTextGlobal(buttonGiftAnchorElement, "Text Gift Remain",
                    "HIDDEN_OBJECTS_NEWS_FEED_GIFT_REMAIN", CHILDREN, own, requirement);

                MetaContextElementUtils.SetClickable(giftButtonElement, OnClickGift, true);
            }
        }

        protected abstract string GetTitleText();

        protected abstract string GetReceivedText();

        protected abstract void OnClickGift();
    }
}
