using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class PopupMetaGameEndNoticeController : MonoBehaviour
    {
        public string infoName = "/clubArenaRankingPopupInfo";
        public string cooltimePrefsKey = "CLUBARENA_NOTICE_COOLTIME";
        public string biClientResultPopupName = "client_club_arena_result_popup";

        private ContextElement rootElement;
        private Blackboard rootBB;
        private Animator rootAnimator;

        private Blackboard rankingInfoBB;
        private ContextElement backgroundAreaElement;
        private ContextElement contentsElement;
        private ContextElement[] clubSymbolAreaElements;
        private ContextElement[] clubNameTextElements;

        private List<Blackboard> clubRankingList;

        private readonly int TOP_SYMBOL_COUNT = 3;
        private readonly int RANKING_COUNT = 20;

        private void Start()
        {
            OnInit();
        }

        private void InitProperty()
        {
            // clubArenaRankingPopupInfo
            rootElement = gameObject.GetComponent<ContextElement>();
            rootBB = gameObject.GetComponent<Blackboard>();
            rootAnimator = gameObject.GetComponent<Animator>();

            rootElement.UpdateContext(false);

            rankingInfoBB = BlackboardUtils.FindValue<Blackboard>(null, infoName);
            clubRankingList = BlackboardUtils.FindValue<List<Blackboard>>(rankingInfoBB, "clubRanking");

            backgroundAreaElement = ContextUtils.FindElement(rootElement, "Base Image Area", ContextSearchingType.ChildrenSearch);

            contentsElement = ContextUtils.FindElement(rootElement, "Contents", ContextSearchingType.ChildrenDeepSearch);

            clubSymbolAreaElements = new ContextElement[TOP_SYMBOL_COUNT];
            clubSymbolAreaElements[0] = ContextUtils.FindElement(contentsElement, "1st Club Symbol Area", ContextSearchingType.ChildrenSearch);
            clubSymbolAreaElements[1] = ContextUtils.FindElement(contentsElement, "2nd Club Symbol Area", ContextSearchingType.ChildrenSearch);
            clubSymbolAreaElements[2] = ContextUtils.FindElement(contentsElement, "3rd Club Symbol Area", ContextSearchingType.ChildrenSearch);

            clubNameTextElements = new ContextElement[RANKING_COUNT];
            for (int i = 0; i < RANKING_COUNT; ++i)
                clubNameTextElements[i] = ContextUtils.FindElement(contentsElement, string.Format("Text Name {0:00}", i + 1), ContextSearchingType.ChildrenSearch);

            for (int i = 0; i < TOP_SYMBOL_COUNT; ++i)
            {
                if (i < clubRankingList.Count)
                    MetaIconUtils.MakeClubSymbolIconObject(BlackboardUtils.FindValue<string>(clubRankingList[i], "clubSymbol"), clubSymbolAreaElements[i].transform, null);
            }

            MetaContextElementUtils.SimpleSetClickable(rootElement, "Button Close", ClosePopup);
            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(), ClosePopup);

            contentsElement.gameObject.SetActive(false);
        }

        private void InitBackground()
        {
            string backgroundImageURL = BlackboardUtils.FindValue<string>(rankingInfoBB, "backgroundImageUrl");
            if (!string.IsNullOrEmpty(backgroundImageURL))
            {
                //MetaContextElementUtils.SetWebImage(backgroundAreaElement, backgroundImageURL);
                //WebImageDownloader.ClearWebImages();
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
                            Debug.Log("[PopupMetaGameEndNotice] " + error.ToString());
                        ClosePopup();
                    });
            }
            else
                ClosePopup();
        }

        private void InitText()
        {
            int listCount = clubRankingList.Count;
            for (int i = 0; i < RANKING_COUNT; ++i)
                MetaContextElementUtils.SetText(clubNameTextElements[i], i < listCount ? BlackboardUtils.FindValue<string>(clubRankingList[i], "clubName") : "-");
        }

        public void OnInit()
        {
            InitProperty();
            InitText();
            rootAnimator.SetBool("Active", true);
            InitBackground();
            PlayerPrefs.SetString(cooltimePrefsKey, TimeUtils.GetCurrentTime().ToString());
            GSManager.Instance.GetHandler("Meta_Club_End_Notice").Play();
            BIClientMetaGameResultPopup();
        }

        private void ClosePopup()
        {
            if(backgroundAreaElement == null) return;

            MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
            PopupManager.Instance.Close(gameObject);
            rootAnimator?.SetTrigger("Close");
        }

        private void BIClientMetaGameResultPopup()
        {
            Variable<int> themeId = rankingInfoBB.GetVariable<int>("themeId");

            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["type"] = "lobby";
            customData["rank"] = null;
            customData["percentile"] = null;
            customData["reward_type"] = null;
            customData["amount_of_reward"] = null;
            if (themeId != null)
                customData["theme_id"] = themeId.value;
            Analytics.CustomEvent(biClientResultPopupName, customData);
        }
    }
}
