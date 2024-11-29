using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Task.Actions
{

    [Category("★ BagelCode/Poll")]
    public class GetPollEventNameBB : ActionTask<Blackboard>
    {
        public BBParameter<string> valueA;
        [BlackboardOnly]
        public BBParameter<string> saveAs;

        protected override string info
        {
            get { return string.Format("{0} = {1}.ToString()", saveAs, valueA); }
        }

        protected override void OnExecute()
        {
            saveAs.value = "";

            var pollType = BlackboardUtils.FindVariable<PollType>(agent, valueA.value);

            switch (pollType.value)
            {
                case PollType.KUDO_RECEIVE:
                    saveAs.value = "POLL_KUDO_RECEIVE";
                    break;
                // case PollType.CHAT:
                //     saveAs.value = "POLL_USER_CHAT";
                //     break;
                // case PollType.CLUB_CHAT:
                //     saveAs.value = "POLL_CLUB_CHAT";
                //     break;
                case PollType.LEVEL_UP:
                    saveAs.value = "POLL_LEVEL_UP";
                    break;
                case PollType.TIER_UP:
                    saveAs.value = "POLL_TIER_UP";
                    break;
                case PollType.LIKE:
                    saveAs.value = "POLL_LIKE";
                    break;
                case PollType.WIN:
                    saveAs.value = "POLL_WIN";
                    break;
                case PollType.JACKPOT_WIN:
                    saveAs.value = "POLL_GAME_JACKPOT";
                    break;
                case PollType.META_JACKPOT_WIN:
                    saveAs.value = "POLL_META_JACKPOT";
                    break;
                case PollType.FRIEND_INVITE:
                    saveAs.value = "POLL_FRIEND_INVITE";
                    break;
                case PollType.FRIEND_CONNECT:
                    saveAs.value = "POLL_FRIEND_CONNECT";
                    break;
                case PollType.TOURNAMENT_START:
                    // saveAs.value = "TOURNAMENT_START";
                    break;
                case PollType.TOURNAMENT_END:
                    saveAs.value = "POLL_TOURNAMENT_END";
                    break;
                case PollType.TOURNAMENT_TOP_RANKER:
                    saveAs.value = "POLL_TOURNAMENT_TOP_RANKER";
                    break;
                case PollType.BONUS_TRIGGER:
                    saveAs.value = "POLL_BONUS_TRIGGER";
                    break;
                case PollType.CHALLENGE_MISSION_COMPLETE:
                    saveAs.value = "POLL_CHALLENGE_MISSION_COMPLETE";
                    break;
                //case PollType.COMMUNITY_GAME_TICKET_EARN:
                //    saveAs.value = "COMMUNITY_GAME_TICKET_EARN";
                //    break;
                case PollType.CLUB_CHALLENGE_MISSION_COMPLETE:
                    saveAs.value = "POLL_CLUB_CHALLENGE_MISSION_COMPLETE";
                    break;
                case PollType.CLUB_MEMBER_LOGIN:
                    saveAs.value = "POLL_CLUB_MEMBER_LOGIN";
                    break;
                case PollType.CLUB_COLEADER_CHANGE:
                    saveAs.value = "POLL_CLUB_COLOADER_CHANGE";
                    break;
                case PollType.CLUB_REQUEST:
                    saveAs.value = "POLL_CLUB_REQUEST";
                    break;
                case PollType.CLUB_REQUEST_ACCEPTED:
                    saveAs.value = "POLL_CLUB_ACCEPTED";
                    break;
                case PollType.CUSTOM_USER_IN_GAME_ACTION:
                    saveAs.value = "POLL_CUSTOM_USER_IN_GAME_ACTION";
                    break;
                //case PollType.SOCIAL_CREDIT:
                //    saveAs.value = "SOCIAL_CREDIT";
                //    break;
                //case PollType.CLUB_DEAL:
                //    saveAs.value = "CLUB_DEAL";
                //    break;
                //case PollType.CLUB_SHARE:
                //    saveAs.value = "CLUB_SHARE";
                //    break;
                //case PollType.CLUB_INVITE:
                //    saveAs.value = "CLUB_INVITE";
                //    break;
                //case PollType.FIRST_GLOBAL_CHAT:
                //    saveAs.value = "FIRST_GLOBAL_CHAT";
                //    break;
                //case PollType.BOSS_RAIDERS_ROUND_COMPLETE:
                //    saveAs.value = "BOSS_RAIDERS_ROUND_COMPLETE";
                //    break;
                //case PollType.BOSS_RAIDERS_END:
                //    saveAs.value = "BOSS_RAIDERS_END";
                //    break;
                //case PollType.CLUB_ARENA_RANKING_UP:
                //    saveAs.value = "CLUB_ARENA_RANKING_UP";
                //    break;
                //case PollType.CLUB_ARENA_ONE_DAY_LEFT:
                //    saveAs.value = "CLUB_ARENA_ONE_DAY_LEFT";
                //    break;
                //case PollType.CLUB_ARENA_END:
                //    saveAs.value = "CLUB_ARENA_END";
                //    break;
                //case PollType.HIDDEN_UNIVERSE_FINDER_GIFT:
                default:
                    saveAs.value = pollType.ToString();
                    break;
            }

            EndAction();
        }
    }
}
