using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using System.Linq;
using SlotMaker;
using ParadoxNotion;
using UnityEngine;

namespace BagelCode
{
    public class KudoEventManager : EventMonoBehaviour
    {
        public static KudoEventManager Instance;

        // Settings
        public const float FEED_PLAY_INTERVAL = 0.5f;
        public const float KUDO_DISPLAY_TIME = 4f;
        public const float KUDO_ACCEPTED_DISPLAY_TIME = 2f;

        private static List<KudoData> kudoDataList = new List<KudoData>();

        private static HashSet<PollType> ignoreKudoSet = new HashSet<PollType>();

        //

        public const string ON_ACCEPT = "OnAccept";
        public const string ON_SKIP = "OnSkip";
        public const string ON_DISAPPEAR = "OnDisappear";

        //

        public void EnableKudo()
        {
            enabled = true;
        }

        public void DisableKudo()
        {
            enabled = false;
        }

        public void InactiveTargetKudo(PollType kudoType)
        {
            ignoreKudoSet.Add(kudoType);
        }

        public void ActiveTargetKudo(PollType kudoType)
        {
            ignoreKudoSet.Remove(kudoType);
        }

        //

        public static void OnDestroyKudoPrefab(KudoController controller)
        {
            // todo shk event방식으로 변경
            var destroyedKudoData = kudoDataList.FirstOrDefault(k => k.controller == controller);
            kudoDataList.Remove(destroyedKudoData);
        }

        private void Start()
        {
            Instance = this;

            InitEvents();
        }

        private void InitEvents()
        {
            Debug.Log("Game START");
            RegisterHandleEventType(MetaEventDefine.ON_LONG_POLL_EVENT);
            RegisterHandleEventType(MetaEventDefine.ON_LOCAL_FEED_EVENT);

            RegisterSchedulingEvent("PlayKudo", PlayAllKudoCoroutine, FEED_PLAY_INTERVAL, true, true);

            Register(MetaEventDefine.ON_LONG_POLL_EVENT, MetaEventDefine.ANY_EVENT_NAME, OnLongPollEvent);
            Register(MetaEventDefine.ON_LOCAL_FEED_EVENT, MetaEventDefine.ANY_EVENT_NAME, OnLocalFeedEvent);
        }

        private void OnLongPollEvent(EventData eventData)
        {
            ParseFeed(eventData.value as Blackboard);
        }

        private void OnLocalFeedEvent(EventData eventData)
        {
            ParseLocalFeed(eventData.value as Blackboard);
        }

        private IEnumerator PlayAllKudoCoroutine()
        {
            for (int i = 0; i < kudoDataList.Count; ++i)
            {
                var kudoData = kudoDataList[i];
                if(kudoData != null)
                {
                    yield return StartCoroutine(kudoData.DisplayKudoCoroutine());
                }
            }
        }

        private bool ParseFeed(Blackboard feed)
        {
            if(feed == null) return false;
            Variable<bool> isKudoActive = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "isKudoActive");
            if (isKudoActive != null && isKudoActive.value == false) return false;

            PollType feedType = feed.GetVariable<PollType>("__event__")?.value ?? PollType.UNKNOWN;
            if (ignoreKudoSet.Contains(feedType))
            {
                if (ApplicationSettings.LogTest())
                {
                    UnityEngine.Debug.Log(string.Format(
                        "Kudo event {0} occurred but was ignored. " +
                        "It is included in the 'ignoreKudoSet'", feedType.ToString()));
                }
                return false;
            }

            switch (feedType)
            {
                case PollType.KUDO_RECEIVE:
                    var likeType = feed.GetValue<KudoLikeType>("kudoLikeType");
                    switch (likeType)
                    {
                        case KudoLikeType.JACKPOT_WIN:
                        case KudoLikeType.META_JACKPOT_WIN:
                        case KudoLikeType.TOURNAMENT_TOP_RANKER:
                            AddFeedToKudoData<KudoDataJackpotLike>(feed); // Like Winning
                            break;
                        case KudoLikeType.SOCIAL_CREDIT:
                        case KudoLikeType.CLUB_DEAL:
                        case KudoLikeType.META_GAME_SHARE:
                        case KudoLikeType.HIDDEN_UNIVERSE:
                            AddFeedToKudoData<KudoDataGiftLike>(feed); // Like Gift
                            break;
                        default:
                            AddFeedToKudoData<KudoDataJackpotLike>(feed);
                            break;
                    }
                    break;
                case PollType.JACKPOT_WIN:
                    AddFeedToKudoData<KudoDataJackpotWin>(feed);
                    break;
                case PollType.META_JACKPOT_WIN:
                    var jackpotType = feed.GetValue<MetaJackpotType>("type");
                    switch(jackpotType)
                    {
                        case MetaJackpotType.DAILY_BONUS:
                        case MetaJackpotType.DAILY_MEGA_WHEEL:
                            AddFeedToKudoData<KudoDataMetaJackpotWin>(feed);
                            break;
                        case MetaJackpotType.GEM_JACKPOT:
                            AddFeedToKudoData<KudoDataGemJackpotWin>(feed);
                            break;
                    }
                    break;
                case PollType.FRIEND_INVITE:
                    AddFeedToKudoData<KudoDataFriendInvite>(feed);
                    break;
                case PollType.TOURNAMENT_END:
                    BI_tournament_break(feed); // Send Bi
                    break;
                case PollType.TOURNAMENT_TOP_RANKER:
                    AddFeedToKudoData<KudoDataTournamentTopRanker>(feed);
                    break;
                case PollType.CLUB_COLEADER_CHANGE:
                    AddFeedToKudoData<KudoDataClubColeaderChange>(feed);
                    break;
                case PollType.CLUB_REQUEST:
                    AddFeedToKudoData<KudoDataClubRequest>(feed);
                    break;
                case PollType.CLUB_REQUEST_ACCEPTED:
                    AddFeedToKudoData<KudoDataClubRequestAccepted>(feed);
                    break;
                case PollType.SOCIAL_CREDIT:
                    AddFeedToKudoData<KudoDataSocialCredit>(feed);
                    break;
                case PollType.CLUB_DEAL:
                    AddFeedToKudoData<KudoDataClubDeal>(feed);
                    break;
                case PollType.CLUB_SHARE:
                    AddFeedToKudoData<KudoDataClubShare>(feed);
                    break;
                case PollType.CLUB_INVITE:
                    AddFeedToKudoData<KudoDataClubInvite>(feed);
                    break;
                case PollType.FIRST_GLOBAL_CHAT:
                    AddFeedToKudoData<KudoDataFirstGlobalChat>(feed);
                    break;
                case PollType.BOSS_RAIDERS_ROUND_COMPLETE:
                    AddFeedToKudoData<KudoDataBossRaidersRoundComplete>(feed);
                    break;
                case PollType.BOSS_RAIDERS_END:
                    AddFeedToKudoData<KudoDataBossRaidersEnd>(feed);
                    break;
                case PollType.CLUB_ARENA_RANKING_UP:
                    AddFeedToKudoData<KudoDataClubArenaRankingUp>(feed);
                    break;
                case PollType.CLUB_ARENA_ONE_DAY_LEFT:
                    AddFeedToKudoData<KudoDataClubArenaOneDayLeft>(feed);
                    break;
                case PollType.CLUB_ARENA_END:
                    AddFeedToKudoData<KudoDataClubArenaEnd>(feed);
                    break;
                case PollType.HIDDEN_UNIVERSE_FINDER_GIFT:
                    AddFeedToKudoData<KudoDataFinderReceived>(feed);
                    break;
                case PollType.PROGRAMMED_KUDO:
                    AddFeedToKudoData<KudoDataProgrammedWin>(feed);
                    break;
                default:
                    return false;
            }

            return true;
        }

        private bool ParseLocalFeed(Blackboard feed)
        {
            if(feed == null) return false;

            var feedType = feed.GetVariable<MetaFeedUtils.LocalFeedEventType>("localFeedEventType")?.value ??
                MetaFeedUtils.LocalFeedEventType.NONE;

            switch (feedType)
            {
                case MetaFeedUtils.LocalFeedEventType.MAINTENANCE:
                case MetaFeedUtils.LocalFeedEventType.BOSSRAIDERS:
                    AddFeedToKudoData<KudoDataMaintenance>(feed);
                    break;
                case MetaFeedUtils.LocalFeedEventType.CLUBARENA_REVENGE:
                    AddFeedToKudoData<KudoDataClubArenaRevenge>(feed);
                    break;
                case MetaFeedUtils.LocalFeedEventType.CLUBARENA_REWARD:
                    AddFeedToKudoData<KudoDataClubArenaReward>(feed);
                    break;
                case MetaFeedUtils.LocalFeedEventType.CLUBARENA_START:
                    AddFeedToKudoData<KudoDataClubArenaMaintenance>(feed);
                    break;
                default:
                    return false;
            }
            Destroy(feed.gameObject);
            return true;
        }

        private void AddFeedToKudoData<T>(Blackboard feedInfo) where T : KudoData, new()
        {
            KudoData kudoData;

            var existKudoData = kudoDataList.FirstOrDefault(k => k.Is<T>());
            if(existKudoData != null)
            {
                kudoData = existKudoData;
            }
            else
            {
                T newKudoData = new T();

                if(!newKudoData.isUsable)
                    return;

                kudoDataList.Add(newKudoData);
                kudoData = newKudoData;
            }

            kudoData.AddFeedToList(feedInfo);
        }

        private void BI_tournament_break(Blackboard info)
        {
            var id = info.GetValue<string>("tournamentId");
            var round = info.GetValue<int>("serialWinCount") + 1;
            var rank = info.GetValue<int>("rank");
            var reward = info.GetValue<long>("winCredit");
            var prizeMultiplier = info.GetValue<double>("serialWinBonus");

            Analytics.CustomEvent("client_tournament", new Dictionary<string, object>
            {
                { "type", "break" },
                { "tournament_id", id },
                { "round", round },
                { "rank", rank },
                { "earn_coin", reward },
                { "prize_multiplier", prizeMultiplier }
            });
        }
    }
}
