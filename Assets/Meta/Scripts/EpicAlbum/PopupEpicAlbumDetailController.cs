using System;
using System.Globalization;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode.EpicAlbum
{
    public class PopupEpicAlbumDetailController : MonoBehaviour
    {
        private string ON_CLOSE_EVENT = "OnClose";
        private string ON_SHARE_EVENT = "OnShare";
        private string ON_PLAY_EVENT = "OnPlay";

        private Blackboard woeInfo;
        private string screenShotUrl;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;
        private const int STAR_COUNT = 3;

        public void OnInit(int categoryType, int gameId)
        {
            ContextElement agent = GetComponent<ContextElement>();

            ContextElement buttonCloseElement = ContextUtils.FindElement(agent, "Button Close", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(
                buttonCloseElement,
                ON_CLOSE_EVENT,
                agent,
                null
            );

            ContextElement buttonPlayElement = ContextUtils.FindElement(agent, "Button OK", ContextSearchingType.ChildrenSearch);
            ContextElement buttonPlayTextElement = ContextUtils.FindElement(buttonPlayElement, "Text", ContextSearchingType.ChildrenSearch);

            woeInfo = gameId != -1 ? BlackboardQueryUtils.GetWOEInfo((CategoryType) categoryType, gameId) : null;

            ContextElement epicWinElement = ContextUtils.FindElement(agent, "Epic Win", ContextSearchingType.ChildrenSearch);
            ContextElement epicWinDefaultElement = ContextUtils.FindElement(agent, "Epic Win Default", ContextSearchingType.ChildrenSearch);
            ContextElement noEpicWinsElement = ContextUtils.FindElement(agent, "No Epic Wins", ContextSearchingType.ChildrenSearch);
            ContextElement epicWinLoading = ContextUtils.FindElement(epicWinElement, "Loading", ContextSearchingType.ChildrenSearch);
            ContextElement gaugeElement = ContextUtils.FindElement(agent, "Title Area/Gauge", ContextSearchingType.FullNameSearch);
            ContextElement dateElement = ContextUtils.FindElement(agent, "Day Tag Base", ContextSearchingType.ChildrenSearch);
            ContextElement titleTextElement = ContextUtils.FindElement(agent, "Title Area/Title Text", ContextSearchingType.FullNameSearch);

            MetaContextElementUtils.SetActive(noEpicWinsElement, woeInfo == null);

            if (woeInfo == null)
            {
//                MetaContextElementUtils.SetActive(buttonShareElement, false);
                MetaContextElementUtils.SetActive(epicWinElement, false);
                MetaContextElementUtils.SetActive(epicWinDefaultElement, false);
                MetaContextElementUtils.SetActive(epicWinLoading, false);
                MetaContextElementUtils.SetFloatProperty(gaugeElement, 0f);
                MetaContextElementUtils.SetActive(dateElement, false);

                MetaContextElementUtils.SimpleSetText(noEpicWinsElement, "Text No Epic Wins", StringTableUtils.GetString(tableType, "EPIC_ALBUM_DETAIL_POPUP_NO_WIN_TEXT"));

                ContextElement slotImageArea = ContextUtils.FindElement(noEpicWinsElement, "Slot Image Area", ContextSearchingType.ChildrenSearch);

                Blackboard gameInfo = BlackboardQueryUtils.GetGameInfo(gameId);
                if (gameInfo != null)
                {
                    string gameTitle = gameInfo.GetValue<string>("gameTitle");
                    MetaIconUtils.MakeSlotImageObjectFromGameTitle(gameTitle, false, false, slotImageArea.transform, "");
                }
            }
            else
            {
                int woeId = woeInfo.GetValue<int>("id");
                BlackboardQueryUtils.SetNewWOE(woeId, false);

                ContextElement epicWinImage = ContextUtils.FindElement(epicWinElement, "Image", ContextSearchingType.ChildrenSearch);

                MetaContextElementUtils.SetActive(epicWinImage, false);
                MetaContextElementUtils.SetActive(epicWinLoading, true);

                long reportedTimestamp = woeInfo.GetValue<long>("reportedTimestamp");
                DateTime dateTime = TimeUtils.ParseTimestampToDateTime(reportedTimestamp);
                string dateFormat = StringTableUtils.GetString(tableType, "EPIC_ALBUM_DETAIL_POPUP_DATE_FORMAT");
                string dateString = string.Format("{0}", dateTime.ToString(dateFormat, CultureInfo.CreateSpecificCulture("en-us")));
                MetaContextElementUtils.SimpleSetText(dateElement, "Text", dateString, ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SetActive(dateElement, true);

                var starCount = woeInfo.GetValue<int>("starCount");
                for(int i=0; i<STAR_COUNT; ++i)
                {
                    MetaContextElementUtils.SimpleSetBooleanProperty(agent, string.Format("Title Area/Star {0} On", i), starCount > i, ContextSearchingType.FullNameSearch);
                }

                if(starCount < STAR_COUNT)
                {
                    float gaugeProgress = 0f;
                    float oneStepDelta = 1f/(float)STAR_COUNT;

                    float prevEpicWinCount = (float)woeInfo.GetValue<int>("prevEpicWinCount");
                    float nextRequiredEpicWinCount = (float)woeInfo.GetValue<int>("nextRequiredEpicWinCount");
                    float epicWinCount = (float)woeInfo.GetValue<int>("epicWinCount");

                    if(starCount > 0)
                        gaugeProgress = starCount * oneStepDelta;

                    if(epicWinCount > 0)
                        gaugeProgress += ((epicWinCount-prevEpicWinCount)/(nextRequiredEpicWinCount-prevEpicWinCount))/(float)STAR_COUNT;

                    MetaContextElementUtils.SetFloatProperty(gaugeElement, gaugeProgress);
                }
                else
                {
                    MetaContextElementUtils.SetFloatProperty(gaugeElement, 1f);
                }

                screenShotUrl = woeInfo.GetValue<string>("screenshotUrl");

                if (!string.IsNullOrEmpty(screenShotUrl))
                {
                    MetaContextElementUtils.SetActive(epicWinElement, true);
                    MetaContextElementUtils.SetActive(epicWinDefaultElement, false);

                    MetaContextElementUtils.SetWebImage(epicWinImage, screenShotUrl, CacheType.FileCache, true, () =>
                    {
                        if (epicWinImage == null)
                            return;

                        // 472 * 316
                        MetaContextElementUtils.UpdateContextImageOrientationScaler(epicWinImage, 472f, 316f);

                        MetaContextElementUtils.SetActive(epicWinImage, true);
                        MetaContextElementUtils.SetActive(epicWinLoading, false);
                    });
                }
                else
                {
                    MetaContextElementUtils.SetActive(epicWinElement, false);
                    MetaContextElementUtils.SetActive(epicWinDefaultElement, true);

                    long winCredit = woeInfo.GetValue<long>("winCredit");
                    ContextElement epicWinDefaultTextElement = ContextUtils.FindElement(epicWinDefaultElement, "Text", ContextSearchingType.ChildrenSearch);
                    string winCreditText = StringTableUtils.GetString(tableType, "TEXT_COMMA_NUMBER", winCredit);
                    MetaContextElementUtils.SetText(epicWinDefaultTextElement, winCreditText);
                }
            }

            if (gameId != -1)
            {
                string gameTitle = StringTableUtils.GetString(tableType, string.Format("GAME_TITLE_{0}", gameId));
                string title = StringTableUtils.GetString(tableType, "EPIC_ALBUM_DETAIL_POPUP_TEXT", gameTitle);
                MetaContextElementUtils.SetText(titleTextElement, title);
            }

            Blackboard slotInfo = BlackboardQueryUtils.GetSlotInfoBB(gameId);

            string buttonPlayText = StringTableUtils.GetString(tableType, "BUTTON_SPIN");
            if (slotInfo != null)
            {
                int status = BlackboardUtils.FindVariable<int>(slotInfo, "flags/status").value;

                if (status == 3 || status == 4)
                {
                    buttonPlayElement.GetComponent<PIDButton>().interactable = false;

                    if (status == 3)
                        buttonPlayText = StringTableUtils.GetString(tableType, "BUTTON_UNDER_CONSTRUCTION");
                    else
                        buttonPlayText = StringTableUtils.GetString(tableType, "BUTTON_UPDATE_TO_PLAY");
                }
            }

            MetaContextElementUtils.SetText(buttonPlayTextElement, buttonPlayText);

            MetaContextElementUtils.SetClickable(
                buttonPlayElement,
                ON_PLAY_EVENT,
                agent,
                null
            );
        }

        public string GetScreenShotUrl()
        {
            return screenShotUrl;
        }

        public void SetWOEShared()
        {
            if (woeInfo != null)
                BlackboardQueryUtils.SetSharedWOE(woeInfo.GetValue<int>("id"));
        }
    }
}
