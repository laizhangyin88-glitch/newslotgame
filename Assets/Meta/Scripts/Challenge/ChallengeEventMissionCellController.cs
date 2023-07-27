using System.Collections.Generic;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using BagelCode.ClientModels;
using BSS.Utils;
using System.Collections;
using UnityEngine.UI;
using ParadoxNotion;

namespace BagelCode
{
    public class ChallengeEventMissionCellController : EventMonoBehaviour
    {
        public bool isPersonal;
        public bool isCompleteToUnlock;
        public State state;
        public bool isDummy;

        private GameObject caller;
        private Blackboard callerBB;

        private ContextElement root;
        private Blackboard rootBB;

        private Blackboard missionInfo;

        private ContextElement anchorElement;

        private ContextElement imageAreaElement;
        private ContextElement textMissionNameElement;
        private ContextElement thumbnailElement;
        private ContextElement thumbnailAreaElement;
        private ContextElement thumbnailButtonSpinAreaElement;
        private ContextElement thumbnailButtonSpinElement;
        private ContextElement thumbnailButtonSpinTextElement;
        private ContextElement imageWinsElement;
        private ContextElement lockElement;
        private ContextElement completeElement;

        private ContextElement rewardElement;
        private ContextElement rewardTextElement;

        private ContextElement gaugeElement;
        private ContextElement gaugeTextElement;

        private ContextElement arrowElement;

        private ContextElement leadersElement;
        private ContextElement leadersPopupButtonAreaElement;
        private List<ContextElement> contributionListElement = new List<ContextElement>();

        private GameObject leadersPopupButtonObj;
        private GameObject leadersPopupObj;
        private GameObject missionImageObj;
        private GameObject gameImageObj;

        private string missionName;
        private long progressMax;
        private long progress;
        private long rewardCoin;
        private long rewardGem;
        private long rewardLp;

        private bool isUpdated = false;

        private Dictionary<string, ContextElement> winElementDict = new Dictionary<string, ContextElement>();

        private const int TOP_CONTRIBUTION_COUNT = 3;

        ///

        private bool isInit = false;

        private const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;
        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private enum MissionIconType
        {
            NONE = 0,
            SPIN,
            WIN,
            FREE_GAMES,
        }

        public enum State
        {
            DEFAULT = 0,
            LOCK,
            COMPLETE,
        }

        //

        private void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            rootBB = GetComponent<Blackboard>();

            root.UpdateContext(false);

            caller = rootBB.GetValue<GameObject>("caller");
            callerBB = caller.GetComponent<Blackboard>();

            anchorElement = ContextUtils.FindElement(root, "Anchor", FULL);

            textMissionNameElement = ContextUtils.FindElement(anchorElement, "Text Mission Name", FULL);
            imageAreaElement = ContextUtils.FindElement(anchorElement, "Image Area", FULL);
            thumbnailElement = ContextUtils.FindElement(anchorElement, "Thumbnail", FULL);
            thumbnailAreaElement = ContextUtils.FindElement(thumbnailElement, "Thumbnail Area", FULL);
            thumbnailButtonSpinAreaElement = ContextUtils.FindElement(thumbnailElement, "Button Spin Area", FULL);
            thumbnailButtonSpinElement = ContextUtils.FindElement(thumbnailButtonSpinAreaElement, "Button Spin", FULL);
            thumbnailButtonSpinTextElement = ContextUtils.FindElement(thumbnailButtonSpinElement, "Text", FULL);

            lockElement = ContextUtils.FindElement(anchorElement, "Lock", FULL);
            completeElement = ContextUtils.FindElement(anchorElement, "Complete", FULL);

            arrowElement = ContextUtils.FindElement(anchorElement, "Arrow", FULL);

            rewardElement = ContextUtils.FindElement(anchorElement, "Reward", FULL);
            rewardTextElement = ContextUtils.FindElement(rewardElement, "Text", FULL);

            gaugeElement = ContextUtils.FindElement(anchorElement, "Gauge", FULL);
            gaugeTextElement = ContextUtils.FindElement(gaugeElement, "Text", FULL);

            // Club Leaders Element
            if (!isPersonal)
            {
                contributionListElement.Clear();
                leadersElement = ContextUtils.FindElement(anchorElement, "Leaders", FULL);
                for (int i = 0; i < TOP_CONTRIBUTION_COUNT; ++i)
                {
                    var e = ContextUtils.FindElement(leadersElement, string.Format("{0:00}", i + 1), FULL);
                    if (e != null) contributionListElement.Add(e);
                }

                leadersPopupButtonAreaElement = ContextUtils.FindElement(root, "Button Leaders Area", FULL);
            }

            // Win Element
            imageWinsElement = ContextUtils.FindElement(anchorElement, "Image Wins", FULL);
            foreach (var w in (WinType[])System.Enum.GetValues(typeof(WinType)))
            {
                if (w == WinType.UNKNOWN) continue;

                string elementName = TextDecoUtils.EnumTypeToText<WinType>(
                    (int)w, TextDecoUtils.TextFormat.PASCAL_CASE, " ") + " Win";

                winElementDict.Add(w.ToString(),
                    ContextUtils.FindElement(
                        imageWinsElement,
                        elementName,
                        CHILDREN));
            }

            MetaContextElementUtils.SetClickable(thumbnailButtonSpinElement, EnterGame);
            MetaContextElementUtils.SetTextGlobal(thumbnailButtonSpinTextElement, "BUTTON_SPIN");

            isInit = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            // Update Profile
            if (isUpdated && !isPersonal && !isDummy)
            {
                SetMissionLeaders();
            }
        }

        private void SetMissionCellEmpty()
        {
            root = GetComponent<ContextElement>();
            root.UpdateContext(false);
            ContextUtils.FindElement(root, "Anchor", FULL).gameObject.SetActive(false);
        }

        public void UpdateView(Blackboard _missionInfo)
        {
            isUpdated = true;

            if (_missionInfo is null)
            {
                SetMissionCellEmpty();
                return;
            }

            InitProperty();

            missionInfo = _missionInfo;

            // Set State
            bool isDone = missionInfo.GetValue<bool>("done");
            if (isDone) state = State.COMPLETE;
            else if (isCompleteToUnlock)
                state = missionInfo.GetVariable<bool>("isUnlock")?.value ?? false ? State.DEFAULT : State.LOCK;
            else state = State.DEFAULT;

            var rewardInfo = missionInfo.GetValue<Blackboard>("reward");
            rewardCoin = rewardInfo.GetVariable<long>("credit")?.value ?? 0L;
            rewardGem = rewardInfo.GetVariable<long>("gem")?.value ?? 0L;
            rewardLp = missionInfo.GetVariable<long>("leaguePoint")?.value ?? 0L;

            if (isPersonal)
            {
                missionName = ChallengeUtils.GetChallengeMissionTitle(missionInfo);
            }
            else
            {
                missionName = ChallengeUtils.ClubChallengeMissionTypeToText(missionInfo);
            }

            CreateOrChangeIcon();
        }

        //

        private void SetCellImage()
        {
            int gameId = missionInfo.GetVariable<int>("gameId")?.value ?? -1;
            string gameTitle = gameId != -1 ? BlackboardQueryUtils.GetGameTitle(gameId) : string.Empty;
            string winType = missionInfo.GetVariable<string>("winType")?.value ?? string.Empty;
            MissionIconType missionIconType = MissionIconType.NONE;
            if (isPersonal)
            {
                var missionType = missionInfo.GetValue<ChallengeMissionType>("missionType");
                switch (missionType)
                {
                    case ChallengeMissionType.SPIN_ANY:
                        missionIconType = MissionIconType.SPIN;
                        break;
                    case ChallengeMissionType.WIN_ANY:
                    case ChallengeMissionType.WIN_TARGETED:
                    case ChallengeMissionType.WIN_BIG_WIN_ANY:
                    case ChallengeMissionType.WIN_CREDIT_MORE_THAN_ANY:
                    case ChallengeMissionType.WIN_CREDIT_MORE_THAN_TARGETED:
                    case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_ANY:
                    case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_TARGETED:
                    case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_ANY:
                    case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_TARGETED:
                    case ChallengeMissionType.WIN_BIG_WIN_TARGETED:
                        missionIconType = MissionIconType.WIN;
                        break;
                    case ChallengeMissionType.ENTER_FREE_SPIN_ANY:
                    case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_ANY:
                    case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_TARGETED:
                    case ChallengeMissionType.ENTER_FREE_SPIN_TARGETED:
                        missionIconType = MissionIconType.FREE_GAMES;
                        break;
                }
            }
            else
            {
                var missionType = missionInfo.GetValue<ClubChallengeMissionType>("missionType");
                switch (missionType)
                {
                    case ClubChallengeMissionType.SPIN_ANY:
                    case ClubChallengeMissionType.SPIN_TARGETED:
                    case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_ANY:
                    case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_TARGETED:
                        missionIconType = MissionIconType.SPIN;
                        break;
                    case ClubChallengeMissionType.WIN_ANY:
                    case ClubChallengeMissionType.WIN_TARGETED:
                    case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_ANY:
                    case ClubChallengeMissionType.WIN_CREDIT_MORE_THAN_ANY:
                    case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_ANY:
                    case ClubChallengeMissionType.WIN_BIG_WIN_TARGETED:
                    case ClubChallengeMissionType.WIN_CREDIT_MORE_THAN_TARGETED:
                    case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_TARGETED:
                    case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_TARGETED:
                    case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_BIG_WIN_ANY:
                    case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_BIG_WIN_TARGETED:
                        missionIconType = MissionIconType.WIN;
                        break;
                    case ClubChallengeMissionType.ENTER_FREE_SPIN_ANY:
                    case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_ANY:
                    case ClubChallengeMissionType.ENTER_FREE_SPIN_TARGETED:
                    case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_TARGETED:
                    case ClubChallengeMissionType.WIN_BONUS_GAME_TARGETED:
                    case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BONUS_TARGETED:
                    case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_FREE_SPIN_ANY:
                        missionIconType = MissionIconType.FREE_GAMES;
                        break;
                }
            }

            if (!string.IsNullOrEmpty(gameTitle))
            {
                SetGameIcon(gameTitle);
                SetMissionImage(MissionIconType.NONE);
                SetWinText("");
            }
            else if (!string.IsNullOrEmpty(winType))
            {
                SetGameIcon("");
                SetWinText(winType);
                SetMissionImage(MissionIconType.NONE);
            }
            else if (missionIconType != MissionIconType.NONE)
            {
                SetGameIcon("");
                SetWinText("");
                SetMissionImage(missionIconType);
            }
        }

        private void CreateOrChangeIcon()
        {
            // State
            lockElement.gameObject.SetActive(state == State.LOCK);
            completeElement.gameObject.SetActive(state == State.COMPLETE);

            // Arrow
            bool isArrowEnable = missionInfo.GetVariable<bool>("isArrowEnable")?.value ?? false;
            arrowElement.gameObject.SetActive(isCompleteToUnlock && isArrowEnable);

            SetCellImage();
            SetProgress();

            if (!isPersonal) SetMissionLeaders();

            // Mission Text
            MetaContextElementUtils.SetText(textMissionNameElement, missionName);

            // Credit
            string rewardText = "";
            if (rewardCoin > 0L)
            {
                rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_COIN", rewardCoin);
            }
            if (rewardGem > 0L)
            {
                if (!string.IsNullOrEmpty(rewardText)) rewardText += " + ";
                rewardText += StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_GEM", rewardGem);
            }
            if (rewardLp > 0L)
            {
                if (!string.IsNullOrEmpty(rewardText)) rewardText += " + ";
                rewardText += StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_LP", rewardLp);
            }

            MetaContextElementUtils.SetText(rewardTextElement, rewardText);
        }

        private void SetProgress()
        {
            bool progressTypeLong = true;
            double dProgress = 0.0;
            double dProgressMax = 0.0;

            ChallengeUtils.MissionProgressType progressType;
            if (isPersonal)
            {
                var missionType = missionInfo.GetValue<ChallengeMissionType>("missionType");
                progressType = ChallengeUtils.ChallengeMissionTypeToProgressType(missionType);

                progressMax = missionInfo.GetValue<long>("completeCount");
                progress = missionInfo.GetValue<long>("progress");

                if (missionType == ChallengeMissionType.WIN_CREDIT_MORE_THAN_ANY ||
                    missionType == ChallengeMissionType.WIN_CREDIT_MORE_THAN_TARGETED)
                {
                    var targetCredit = missionInfo.GetValue<long>("targetCredit");
                    progress = progress >= progressMax ? targetCredit : 0L;
                    progressMax = targetCredit;
                }
                else if (missionType == ChallengeMissionType.PURCHASE_PRICE_MORE_THAN)
                {
                    progressTypeLong = false;

                    var targetPrice = missionInfo.GetValue<long>("targetPrice");
                    dProgress = progress >= progressMax ? dProgressMax : 0L;
                    dProgressMax = targetPrice / 100.0;
                }
                else if (missionType == ChallengeMissionType.ACHIEVE_TOTAL_PURCHASE_PRICE)
                {
                    progressTypeLong = false;

                    dProgress = progress / 100.0;
                    dProgressMax = progressMax / 100.0;
                }
            }
            else
            {
                var missionType = missionInfo.GetValue<ClubChallengeMissionType>("missionType");
                progressType = ChallengeUtils.ChallengeMissionTypeToProgressType(missionType);

                progressMax = missionInfo.GetValue<long>("completeCount");
                progress = missionInfo.GetValue<long>("progress");
            }

            string formatKey = "POPUP_CHALLENGE_MISSION_PROGRESS_DEFAULT";
            if (progressType == ChallengeUtils.MissionProgressType.COIN)
                formatKey = "POPUP_CHALLENGE_MISSION_PROGRESS_COIN";
            else if (progressType == ChallengeUtils.MissionProgressType.CURRENCY)
                formatKey = "POPUP_CHALLENGE_MISSION_PROGRESS_CURRENCY";

            if (progressTypeLong)
            {
                float ratio = (float)progress / (float)progressMax;
                MetaContextElementUtils.SetSliderValue(gaugeElement, ratio);
                MetaContextElementUtils.SetTextGlobal(gaugeTextElement, formatKey, progress, progressMax);
            }
            else
            {
                float ratio = (float)dProgress / (float)dProgressMax;
                MetaContextElementUtils.SetSliderValue(gaugeElement, ratio);
                MetaContextElementUtils.SetTextGlobal(gaugeTextElement, formatKey, dProgress, dProgressMax);
            }
        }

        private void SetMissionLeaders()
        {
            var response = callerBB?.GetVariable<Blackboard>("challengeInfoResponse")?.value;
            if (response == null)
            {
                if (ApplicationSettings.LogTest())
                    Debug.LogWarning("ChallengeEventMissionCellController.SetMissionLeaders failure. challengeInfoResponse is null.");
                return;
            }

            var contributionList = BlackboardQueryUtils.GetClubChallengeContributionInfo(response, missionInfo, true);
            if (contributionList == null) return;

            int count = Mathf.Min(TOP_CONTRIBUTION_COUNT, contributionList.Count);

            for (int i = 0; i < count; ++i)
            {
                var e = contributionListElement[i];
                if (e == null) continue;

                if (contributionList[i] == null) continue;

                var contributionBB = e.GetComponent<Blackboard>();
                contributionBB.AddVariable("userInfo", contributionList[i]);
                contributionBB.AddVariable("updateProfile", true);
            }

            // Leaders Popup Button
            if (leadersPopupButtonObj is null)
            {
                string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
                string asset = "Button Primary";
                Transform parent = leadersPopupButtonAreaElement.transform;
                leadersPopupButtonObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);
                leadersPopupButtonObj.name = "Button Leaders";

                var buttonElement = leadersPopupButtonObj.GetComponent<ContextElement>();
                buttonElement.UpdateContext(false);
                var buttonTextElement = ContextUtils.FindElement(buttonElement, "Text", FULL);
                MetaContextElementUtils.SetTextGlobal(buttonTextElement, "BUTTON_LEADERS");
                MetaContextElementUtils.SetClickable(buttonElement, OpenLeadersPopup);

                leadersPopupButtonAreaElement.gameObject.SetActive(true);
            }
        }

        private void OpenLeadersPopup()
        {
            string missionId = missionInfo.GetValue<string>("missionId");
            EventSender.SendEvent(caller, MetaEventDefine.ON_META_UI_EVENT, new EventData<string>("OnOpenLeadersPopup", missionId));
        }

        private void SetWinText(string winType)
        {
            winElementDict.Keys.ForEach(w => winElementDict[w].gameObject.SetActive(w == winType));
        }

        private void SetMissionImage(MissionIconType iconType)
        {
            if (missionImageObj != null)
                missionImageObj.DestroyThis();

            if (iconType == MissionIconType.NONE) return;

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string missionImageAsset = "Challenge Event Icon " +
                TextDecoUtils.EnumTypeToText<MissionIconType>(
                    (int)iconType, TextDecoUtils.TextFormat.PASCAL_CASE, " ");

            missionImageObj = MetaObjectUtils.MakePrefab(bundle, missionImageAsset, imageAreaElement.transform);
        }

        private void SetGameIcon(string gameName)
        {
            StartCoroutine(SetGameIconCoroutine(gameName));
        }

        private IEnumerator SetGameIconCoroutine(string gameName)
        {
            if (gameImageObj != null)
                gameImageObj.DestroyThis();

            if (string.IsNullOrEmpty(gameName))
            {
                thumbnailElement.gameObject.SetActive(false);
                yield break;
            }

            thumbnailElement.gameObject.SetActive(true);

            gameImageObj = MetaIconUtils.MakeSlotImageObjectFromGameTitle(gameName, false, false, thumbnailAreaElement.transform, "");

            if(gameImageObj != null)
            {
                gameImageObj.SetActive(true);
            }
        }

        private void EnterGame()
        {
            int gameId = missionInfo.GetVariable<int>("gameId")?.value ?? -1;
            if (gameId == -1)
            {
                Debug.LogWarning("EnterGame failure. Check the gameId.");
                return;
            }

            bool isInGame = MainBlackboard.Get().GetVariable<bool>("inGame")?.value ?? false;
            if (isInGame)
            {
                int nowGameId;
                var enterGameInfo = MainBlackboard.Get().GetVariable<Blackboard>("enterGameInfo")?.value;
                if (enterGameInfo != null)
                {
                    nowGameId = enterGameInfo.GetValue<int>("gameId");
                    if (gameId == nowGameId)
                    {
                        // Close Popup
                        EventSender.SendEvent(caller, ChallengeEventManager.ON_CLOSE);
                        return;
                    }
                }
            }

            var slotInfo = BlackboardQueryUtils.GetEarlyAccessSlotInfo(gameId);
            if (slotInfo == null)
            {
                slotInfo = BlackboardQueryUtils.GetSlotInfoBB(gameId);
            }

            string fromType = "challenge";
            if (fromType == "challenge")
            {
                int minVersion = 0;
                var gameInfo = BlackboardQueryUtils.GetGameInfo(gameId);
                if (gameInfo != null)
                    minVersion = BlackboardUtils.FindVariable<int>(gameInfo, "minClientVersion")?.value ?? 0;

                int recentVersion = BlackboardUtils.FindVariable<int>(null, "/recentClientNumberVersion").value;
                if (isPersonal)
                {
                    long missionId = missionInfo.GetValue<long>("id");
                    BICustomEvents.SendUpdateAppRecommandAEChallenge(recentVersion, minVersion, gameId, missionId);
                }
                else
                {
                    string missionId = missionInfo.GetValue<string>("missionId");
                    BICustomEvents.SendUpdateAppRecommandAEClubChallenge(recentVersion, minVersion, gameId, missionId);
                }
            }

            bool available = false;
            if (slotInfo != null)
            {
                int flags = BlackboardUtils.FindVariable<int>(slotInfo, "flags/status")?.value ?? 0;
                if (flags != 4) available = true;
            }

            if (available)
            {
                BlackboardQueryUtils.SetEnterGameInfo(
                    gameId,
                    "EnterGame",
                    fromType);

                EventSender.SendGlobalEvent("OnEnterGame");
            }
            else
            {
                StartCoroutine(OpenOkayPopupCoroutine());
            }
        }

        private IEnumerator OpenOkayPopupCoroutine()
        {
            var popupObj = MetaPopupUtils.OpenOKPopup();

            string text = StringTableUtils.GetString(GLOBAL, "POPUP_COMMON_NEED_TO_UPDATE");
            string buttonText = StringTableUtils.GetString(GLOBAL, "BUTTON_OK");
            MetaPopupUtils.SetCommonPopupData(popupObj, transform,
                text, "", "OnOkay", buttonText,
                "", "", "OnClose", "", true, true, true, true, true);

            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);

            var okayTrigger = new EventTrigger(gameObject, "OnOkay");
            var closeTrigger = new EventTrigger(gameObject, "OnClose");
            yield return new WaitUntilTrigger(okayTrigger, closeTrigger);

            if (okayTrigger.IsTrigger)
            {
                string appDownloadUrl = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "/appDownloadUrl").value;

                // Clicked Update
                if (appDownloadUrl != null)
                {
#if UNITY_WEBGL && !UNITY_EDITOR
                    NativeHelper.Instance.OpenUrl(appDownloadUrl);
#else
                    Application.OpenURL(appDownloadUrl);
#endif
                }
            }
        }
    }
}
