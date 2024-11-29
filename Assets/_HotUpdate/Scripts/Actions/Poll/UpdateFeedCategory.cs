using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Task.Actions
{

    [Category("★ BagelCode/Poll")]
    public class UpdateFeedCategory : ActionTask<Blackboard>
    {
        public BBParameter<List<Blackboard>> feedEventList;

        public BBParameter<List<Blackboard>> kudoJackpotLikeList;
        public BBParameter<List<Blackboard>> kudoGiftLikeList;
        public BBParameter<List<Blackboard>> gameJackpotWinList;
        public BBParameter<List<Blackboard>> metaJackpotWinList;
        public BBParameter<List<Blackboard>> friendInviteList;
        public BBParameter<List<Blackboard>> tournamentWinList;
        public BBParameter<List<Blackboard>> clubAuthorityList;
        public BBParameter<List<Blackboard>> clubJoinRequestList;
        public BBParameter<List<Blackboard>> clubJoinAcceptList;
        public BBParameter<List<Blackboard>> socialPurchaseCoinList;
        public BBParameter<List<Blackboard>> socialClubDealList;
        public BBParameter<List<Blackboard>> clubReceiveGiftList;
        public BBParameter<List<Blackboard>> clubInviteList;
        public BBParameter<List<Blackboard>> firstGlobalChatList;
        public BBParameter<List<Blackboard>> maintenanceList;
        public BBParameter<List<Blackboard>> bossRaidersRoundList;
        public BBParameter<List<Blackboard>> bossRaidersEndList;


        protected override string info
        {
            get { return string.Format("Update Feed Category({0})", feedEventList); }
        }

        protected override void OnExecute()
        {
            if(feedEventList.value != null && feedEventList.value.Count > 0)
            {
                for(int i=0; i<feedEventList.value.Count; ++i)
                {
                    bool isUpdate = UpdatePollEvents(feedEventList.value[i]);

                    if(!isUpdate)
                        isUpdate = UpdateLocalFeedEvents(feedEventList.value[i]);
                }

                feedEventList.value.Clear();
            }

            EndAction();
        }

        private bool UpdatePollEvents(Blackboard info)
        {
            var feedType = BlackboardUtils.FindVariable<PollType>(info, "__event__");
            if(feedType == null) return false;

            bool isControllType = true;

            switch(feedType.value)
            {
                case PollType.KUDO_RECEIVE:
                    SortKudoReceiveList(info);
                    break;
                // case PollType.CHAT:
                //     break;
                // case PollType.LEVEL_UP:
                //     break;
                // case PollType.TIER_UP:
                //     break;
                // case PollType.LIKE:
                //     break;
                // case PollType.WIN:
                //     break;
                case PollType.JACKPOT_WIN:
                    gameJackpotWinList.value.Add(info);
                    break;
                case PollType.META_JACKPOT_WIN:
                    metaJackpotWinList.value.Add(info);
                    break;
                case PollType.FRIEND_INVITE:
                    friendInviteList.value.Add(info);
                    break;
                // case PollType.FRIEND_CONNECT:
                //     break;
                // case PollType.BONUS_TRIGGER:
                //     break;
                // case PollType.TOURNAMENT_START:
                //     break;
                case PollType.TOURNAMENT_END:
                    BI_tournament_break(info);
                    break;
                case PollType.TOURNAMENT_TOP_RANKER:
                    tournamentWinList.value.Add(info);
                    break;
                // case PollType.CHALLENGE_MISSION_COMPLETE:
                //     break;
                // case PollType.COMMUNITY_GAME_TICKET_EARN:
                //     break;
                // case PollType.CLUB_CHAT:
                //     break;
                // case PollType.CLUB_CHALLENGE_MISSION_COMPLETE:
                //     break;
                // case PollType.CLUB_MEMBER_LOGIN:
                //     break;
                case PollType.CLUB_COLEADER_CHANGE:
                    clubAuthorityList.value.Add(info);
                    break;
                case PollType.CLUB_REQUEST:
                    clubJoinRequestList.value.Add(info);
                    break;
                case PollType.CLUB_REQUEST_ACCEPTED:
                    clubJoinAcceptList.value.Add(info);
                    break;
                // case PollType.CUSTOM_USER_IN_GAME_ACTION:
                //     break;
                case PollType.SOCIAL_CREDIT:
                    socialPurchaseCoinList.value.Add(info);
                    break;
                case PollType.CLUB_DEAL:
                    socialClubDealList.value.Add(info);
                    break;
                case PollType.CLUB_SHARE:
                    clubReceiveGiftList.value.Add(info);
                    break;
                case PollType.CLUB_INVITE:
                    clubInviteList.value.Add(info);
                    break;
                case PollType.FIRST_GLOBAL_CHAT:
                    firstGlobalChatList.value.Add(info);
                    break;
                case PollType.BOSS_RAIDERS_ROUND_COMPLETE:
                    bossRaidersRoundList.value.Add(info);
                    break;
                case PollType.BOSS_RAIDERS_END:
                    bossRaidersEndList.value.Add(info);
                    break;
                default:
                    isControllType = false;
                    break;
            }

            return isControllType;
        }

        private bool UpdateLocalFeedEvents(Blackboard info)
        {
            var localFeedType = BlackboardUtils.FindVariable<MetaFeedUtils.LocalFeedEventType>(info, "localFeedEventType");
            if(localFeedType == null) return false;

            bool isControllType = true;

            switch(localFeedType.value)
            {
                case MetaFeedUtils.LocalFeedEventType.MAINTENANCE:
                case MetaFeedUtils.LocalFeedEventType.BOSSRAIDERS:
                case MetaFeedUtils.LocalFeedEventType.CLUBARENA_REVENGE:
                    maintenanceList.value.Add(info);
                    break;
                default:
                    isControllType = false;
                    break;
            }

            return isControllType;
        }

        private void SortKudoReceiveList(Blackboard info)
        {
            var likeType = info.GetValue<KudoLikeType>("kudoLikeType");

            switch(likeType)
            {
                case KudoLikeType.JACKPOT_WIN:
                case KudoLikeType.META_JACKPOT_WIN:
                case KudoLikeType.TOURNAMENT_TOP_RANKER:
                    kudoJackpotLikeList.value.Add(info);
                    break;
                case KudoLikeType.SOCIAL_CREDIT:
                case KudoLikeType.CLUB_DEAL:
                case KudoLikeType.META_GAME_SHARE:
                    kudoGiftLikeList.value.Add(info);
                    break;
                default:
                    kudoJackpotLikeList.value.Add(info);
                    break;
            }
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
