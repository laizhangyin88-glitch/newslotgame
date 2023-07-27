using SlotMaker;
using UnityEngine;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using System.Collections.Generic;
using static BagelCode.ClubUtils;

namespace BagelCode
{
    public class PopupMetaGameEndNoticeMyController : EventMonoBehaviour
    {
        public string pointKey = "CLUB_ARENA_POPUP_END_NOTICE_MY_FINAL_POINT";
        public string pointName = "point";

        private ContextElement rootElement;
        private Blackboard rootBB;
        private Animator rootAnimator;

        private Blackboard rewardInfoBB;
        private Blackboard clubInfoBB;
        private ContextElement contentsElement;
        private ContextElement symbolAreaElement;
        private ContextElement clubNameTextElement;
        private ContextElement clubPointTextElement;
        private ContextElement clubRewardTextElement;
        private ContextElement clubFinalRankTextElement;
        private ContextElement backgroundAreaElement;

        private ResultPopupMetaGameName metaGameType;

        private void Start()
        {
            OnInit();
        }

        private void InitProperty()
        {
            rootElement = gameObject.GetComponent<ContextElement>();
            rootBB = gameObject.GetComponent<Blackboard>();
            rootAnimator = gameObject.GetComponent<Animator>();

            rootElement.UpdateContext(false);

            contentsElement = ContextUtils.FindElement(rootElement, "Contents", ContextSearchingType.ChildrenSearch);
            symbolAreaElement = ContextUtils.FindElement(contentsElement, "Club Symbol Area", ContextSearchingType.ChildrenSearch);
            clubNameTextElement = ContextUtils.FindElement(contentsElement, "Text Club Name", ContextSearchingType.ChildrenSearch);
            clubPointTextElement = ContextUtils.FindElement(contentsElement, "Text Final Point", ContextSearchingType.ChildrenSearch);
            clubRewardTextElement = ContextUtils.FindElement(contentsElement, "Text Reward", ContextSearchingType.ChildrenSearch);
            clubFinalRankTextElement = ContextUtils.FindElement(contentsElement, "Text Final Rank", ContextSearchingType.ChildrenSearch);
            backgroundAreaElement = ContextUtils.FindElement(rootElement, "Base Image Area", ContextSearchingType.ChildrenSearch);

            rewardInfoBB = BlackboardUtils.FindVariable<Blackboard>(rootBB, "rewardInfo")?.value;
            clubInfoBB = BlackboardUtils.FindVariable<Blackboard>(rootBB, "clubInfo")?.value;
            metaGameType = BlackboardUtils.FindVariable<ResultPopupMetaGameName>(rootBB, "metaGameType").value;
            MetaIconUtils.MakeClubSymbolIconObject(BlackboardUtils.FindValue<string>(clubInfoBB, "symbol"), symbolAreaElement.transform, null);

            MetaContextElementUtils.SimpleSetClickable(rootElement, "Button Close", ClosePopup);
            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(), ClosePopup);

            contentsElement.gameObject.SetActive(false);

            if (rewardInfoBB == null)
                ClosePopup();
        }

        private void InitText()
        {
            int rank = rewardInfoBB.GetValue<int>("rank");
            MetaContextElementUtils.SetText(clubNameTextElement, BlackboardUtils.FindValue<string>(clubInfoBB, "name"));
            MetaContextElementUtils.SetTextGlobal(clubPointTextElement, pointKey, rewardInfoBB.GetValue<long>(pointName));
            MetaContextElementUtils.SetTextGlobal(clubRewardTextElement, "CLUB_ARENA_POPUP_END_NOTICE_MY_REWARD", rewardInfoBB.GetValue<long>("gem"));
            MetaContextElementUtils.SetTextGlobal(clubFinalRankTextElement, "CLUB_ARENA_POPUP_END_NOTICE_MY_RANK", rank, rank, rewardInfoBB.GetValue<int>("percentile"));
        }

        private void InitBackground()
        {
            string backgroundImageURL = rewardInfoBB.GetValue<string>("backgroundImageUrl");
            if (!string.IsNullOrEmpty(backgroundImageURL))
            {
                IContextImage imageElement = backgroundAreaElement as IContextImage;
                imageElement.SetHash(backgroundImageURL.GetHashCode().ToString());
                WebImageDownloader.Instance.LoadWebImage(
                    backgroundImageURL,
                    CacheType.FileCache,
                    false,
                    null,
                    delegate (Sprite img)
                    {
                        if (backgroundAreaElement != null && imageElement != null && img != null)
                        {
                            contentsElement?.gameObject.SetActive(true);
                            if (imageElement.CheckHash(backgroundImageURL.GetHashCode().ToString()))
                                imageElement.SetSprite(img);
                        }
                    },
                    null,
                    delegate (WebImageDownloader.WebImageDownloadError error)
                    {
                        if (ApplicationSettings.LogSystem())
                            Debug.Log("[PopupMetaGameEndNoticeMy] " + error.ToString());
                        ClosePopup();
                    });
            }
            else
                ClosePopup();
        }

        public void OnInit()
        {
            InitProperty();
            InitText();
            rootAnimator.SetBool("Active", true);
            InitBackground();
            BISendResultPopup();
            //GSManager.Instance.GetHandler("Meta_Club_End_Notice").Play();
        }

        private void ClosePopup()
        {
            if(backgroundAreaElement == null) return;

            MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
            EventSender.SendCalleeCallback(gameObject);
            PopupManager.Instance.Close(gameObject);
            rootAnimator?.SetTrigger("Close");
        }

        private void BISendResultPopup()
        {
            switch (metaGameType)
            {
                case ResultPopupMetaGameName.BOSS_RAIDERS:
                    BIClientClubBossRaidersResultPopup();
                    break;
                case ResultPopupMetaGameName.CLUB_ARENA:
                    BIClientClubArenaResultPopup();
                    break;
            }
        }

        private void BIClientClubBossRaidersResultPopup()
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["rank"] = (long)rewardInfoBB.GetValue<int>("rank");
            customData["percentile"] = rewardInfoBB.GetValue<int>("percentile");
            customData["reward_type"] = "gem";
            customData["amount_of_reward"] = rewardInfoBB.GetValue<long>("gem");
            customData["club_boss_raiders_point"] = rewardInfoBB.GetValue<long>("totalAttack");
            customData["type"] = "in_club";
            customData["theme_id"] = rewardInfoBB.GetValue<int>("themeId");
            Analytics.CustomEvent("client_club_boss_raiders_result_popup", customData);
        }

        private void BIClientClubArenaResultPopup()
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["type"] = "in_club";
            customData["rank"] = (long)rewardInfoBB.GetValue<int>("rank");
            customData["percentile"] = rewardInfoBB.GetValue<int>("percentile");
            customData["reward_type"] = "gem";
            customData["amount_of_reward"] = rewardInfoBB.GetValue<long>("gem");
            Analytics.CustomEvent("client_club_arena_result_popup", customData);
        }
    }
}
