using SlotMaker;
using System.Collections;

namespace BagelCode
{
    public abstract class ClubNewsFeedLikableCellController : ClubNewsFeedCellController
    {
        protected int likeCount;

        private const string ON_LIKED_EVENT = "OnLiked";

        protected virtual bool IsLikable() => true;

        protected virtual IEnumerator OnLikeCoroutine()
        {
            yield break;
        }

        protected virtual string GetLikeText()
        {
            return StringTableUtils.GetString(GLOBAL, "BUTTON_LIKE");
        }

        protected virtual string GetContextText() => "";

        protected override void UpdateCellData()
        {
            base.UpdateCellData();

            // Like
            bool likable = IsLikable();
            bool liked = feedInfo.GetVariable<bool>("userLiked")?.value ?? false;
            likeCount = feedInfo.GetVariable<int>("like")?.value ?? 0;
            UpdateLikeButton(likable, liked);
            UpdateLikeCount(likeCount);

            // Text
            string contextText = GetContextText();
            if (!string.IsNullOrEmpty(contextText))
            {
                MetaContextElementUtils.SimpleSetText(root, "Text Context", contextText, CHILDREN);
            }
        }

        protected override void InitEvents()
        {
            base.InitEvents();

            if (IsLikable())
            {
                var likedTrigger = new EventTrigger(this, ON_LIKED_EVENT);
                RegisterHandlingEvent(likedTrigger, OnLikeCoroutine);
            }
        }

        protected void OnLiked()
        {
            UpdateLikeButton(true, true);
            UpdateLikeCount(++likeCount);
            UpdateFeedInfo(true, likeCount);
        }

        private void UpdateLikeButton(bool likable, bool liked)
        {
            var likedElement = ContextUtils.FindElement(root, "Liked", CHILDREN);
            var likeButtonElement = ContextUtils.FindElement(root, "Button Like", CHILDREN);

            if (likable)
            {
                MetaContextElementUtils.SetActive(likedElement, liked);
                MetaContextElementUtils.SetActive(likeButtonElement, !liked);

                if (!liked) // Like Button
                {
                    MetaContextElementUtils.SetClickable(likeButtonElement, gameObject, ON_LIKED_EVENT, false);

                    string likeText = GetLikeText();
                    MetaContextElementUtils.SimpleSetText(likeButtonElement, "Text", likeText, CHILDREN);
                }
            }
            else
            {
                MetaContextElementUtils.SetActive(likedElement, false);
                MetaContextElementUtils.SetActive(likeButtonElement, false);
            }
        }

        private void UpdateLikeCount(int _likeCount)
        {
            var likeIconElement = ContextUtils.FindElement(root, "Icon Like", CHILDREN);
            var likeCountTextElement = ContextUtils.FindElement(root, "Text Like", CHILDREN);

            bool showLikeCount = _likeCount > 0;
            MetaContextElementUtils.SetActive(likeIconElement, showLikeCount);
            MetaContextElementUtils.SetActive(likeCountTextElement, showLikeCount);

            if (showLikeCount)
                MetaContextElementUtils.SetTextGlobal(likeCountTextElement, "CLUB_NEWS_FEED_LIKE", _likeCount);
        }

        private void UpdateFeedInfo(bool isLiked, int _likeCount)
        {
            if (feedInfo == null)
                return;
            BlackboardUtils.SetOrCreateValue(feedInfo, "userLiked", isLiked);
            BlackboardUtils.SetOrCreateValue(feedInfo, "like", _likeCount);
        }
    }
}
