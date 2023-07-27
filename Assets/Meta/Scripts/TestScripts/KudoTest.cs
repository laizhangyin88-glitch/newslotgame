using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using BagelCode.ClientModels;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;

namespace BagelCode
{
    public enum MaintenanceType
    {
        FIVE_MINUTES,
        THREE_MINUTES,
        ONE_MINUTE,
        THIRTY_SECONDS,
        FIFTEEN_SECONDS
    }

    public class KudoTest : MonoBehaviour
    {
#if UNITY_EDITOR
        public static int testIndex = 0;

        private const int SEAT_COUNT = 5;

        #region General

        [TabGroup("General"), Button]
        public void TestFunc()
        {
            for (int i = 0; i < 10; ++i)
            {
                Debug.LogError(Mathf.Pow(10, i + 1).ToString("N0"));
            }
        }

        [TabGroup("General"), Button]
        public void TestMetsEvent(string testString)
        {
            MessageDispatcher.Dispatch("OnMetaUIEvent", new EventData(testString));
        }

        [TabGroup("General"), Button]
        public void TestError(Error errorType)
        {
            var error = new BagelCodeHTTPError
            {
                url = "Test Error",
                responseCode = 0,
                error = "error",
                errorCode = errorType
            };

            GlobalErrorHandler.GlobalError(error);
        }

        [TabGroup("General"), Button]
        public void MaintenanceAlert(MaintenanceType maintenanceType)
        {
            switch (maintenanceType)
            {
                case MaintenanceType.FIVE_MINUTES:
                    Maintenance("5 minutes");
                    break;
                case MaintenanceType.THREE_MINUTES:
                    Maintenance("3 minutes");
                    break;
                case MaintenanceType.ONE_MINUTE:
                    Maintenance("1 minute");
                    break;
                case MaintenanceType.THIRTY_SECONDS:
                    Maintenance("30 seconds");
                    break;
                case MaintenanceType.FIFTEEN_SECONDS:
                    Maintenance("15 seconds");
                    break;
                default:
                    break;
            }
        }

        [TabGroup("General"), Button]
        public void TestURI(string uri)
        {
            if (string.IsNullOrEmpty(uri)) return;

            PendingActionManager.Instance.OpenUrl(uri);
            ++testIndex;
        }

        [TabGroup("General"), Button]
        public void TestRegex()
        {
            string pattern = "\\<.*?\\>";
            string text = "<b><color=#000000>name</color><b>";
            Debug.Log(text);

            if (Regex.IsMatch(text, pattern))
            {
                text = Regex.Replace(text, pattern, "", RegexOptions.None);
                Debug.Log(text);
            }
        }

        #endregion

        #region Kudo

        private void SendKudoEvent(PollType type, Blackboard bb)
        {
            string typeName = type.ToString();

            //EventSender.SendGlobalEvent(
            //    MetaEventDefine.ON_LONG_POLL_EVENT,
            //    new EventData<Blackboard>(typeName, bb));


            var e = new EventData<Blackboard>(type.ToString(), bb);
            MessageDispatcher.Dispatch(MetaEventDefine.ON_LONG_POLL_EVENT, e);

            ++testIndex;

            StartCoroutine(DestroyTestBB(bb));
        }

        private IEnumerator DestroyTestBB(Blackboard bb)
        {
            yield return new WaitForSeconds(1f);
            GameObject.Destroy(bb.gameObject);
        }

        [TabGroup("Kudo"), Button]
        public void JackpotLikeEvent()
        {
            PollType type = PollType.KUDO_RECEIVE;

            Blackboard newBB = BlackboardUtils.CreateBlackboard(string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", type);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("test Id {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "name", string.Format("test name {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<int>(newBB, "tier", UnityEngine.Random.Range(0, 21));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "profileUrl", "");
            BlackboardUtils.SetOrCreateValue<KudoLikeType>(newBB, "kudoLikeType", KudoLikeType.JACKPOT_WIN);

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void GiftLikeEvent()
        {
            PollType type = PollType.KUDO_RECEIVE;

            Blackboard newBB = BlackboardUtils.CreateBlackboard(string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", type);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("test Id {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "name", string.Format("test name {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<int>(newBB, "tier", UnityEngine.Random.Range(0, 21));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "profileUrl", "");
            BlackboardUtils.SetOrCreateValue<KudoLikeType>(newBB, "kudoLikeType", KudoLikeType.META_GAME_SHARE);

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void SocialClubDealCoin()
        {
            PollType type = PollType.CLUB_DEAL;

            Blackboard newBB = BlackboardUtils.CreateBlackboard(string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", type);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("test Id {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "name", string.Format("test name {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<int>(newBB, "tier", UnityEngine.Random.Range(0, 21));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "profileUrl", "");
            BlackboardUtils.SetOrCreateValue<long>(newBB, "earnCredit", 100000L);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "clubSymbol", "Club Symbol 05");
            BlackboardUtils.SetOrCreateValue<int>(newBB, "clubLeagueTier", UnityEngine.Random.Range(1, 8));
            BlackboardUtils.SetOrCreateValue<int>(newBB, "kudoId", 123);

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void ReceiveClubShareItem()
        {
            PollType type = PollType.CLUB_SHARE;

            Blackboard newBB = BlackboardUtils.CreateBlackboard(string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", type);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("test Id {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "name", string.Format("test name {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "profileUrl", "");
            BlackboardUtils.SetOrCreateValue<int>(newBB, "tier", UnityEngine.Random.Range(0, 21));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "clubSymbol", "Club Symbol 03");
            BlackboardUtils.SetOrCreateValue<int>(newBB, "clubLeagueTier", UnityEngine.Random.Range(1, 8));
            BlackboardUtils.SetOrCreateValue<EventInfoType>(newBB, "eventType", EventInfoType.COLLECTING_GAME);
            BlackboardUtils.SetOrCreateValue<int>(newBB, "eventPresetId", 1);
            BlackboardUtils.SetOrCreateValue<int>(newBB, "kudoId", 123);

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void KudoSocialPurchaseCoin()
        {
            PollType type = PollType.SOCIAL_CREDIT;

            Blackboard newBB = BlackboardUtils.CreateBlackboard(string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", type);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("test Id {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "name", string.Format("test name {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<int>(newBB, "tier", UnityEngine.Random.Range(0, 21));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "profileUrl", "");
            BlackboardUtils.SetOrCreateValue<long>(newBB, "earnCredit", 100000L);
            BlackboardUtils.SetOrCreateValue<int>(newBB, "kudoId", 123);

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void ClubPromote()
        {
            PollType type = PollType.CLUB_COLEADER_CHANGE;

            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
            Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", type);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("test Id {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<bool>(newBB, "promote", true);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "clubSymbol", "Club Symbol 01");
            BlackboardUtils.SetOrCreateValue<int>(newBB, "clubLeagueTier", UnityEngine.Random.Range(1, 8));

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void ClubDemote()
        {
            PollType type = PollType.CLUB_COLEADER_CHANGE;

            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
            Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", type);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("test Id {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<bool>(newBB, "promote", false);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "clubSymbol", "Club Symbol 02");
            BlackboardUtils.SetOrCreateValue<int>(newBB, "clubLeagueTier", UnityEngine.Random.Range(1, 8));

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void ClubJoinRequest()
        {
            PollType type = PollType.CLUB_REQUEST;

            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
            Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", type);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("test Id {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "profileUrl", "");
            BlackboardUtils.SetOrCreateValue<int>(newBB, "tier", UnityEngine.Random.Range(0, 21));

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void ClubJoinAccept()
        {
            PollType type = PollType.CLUB_REQUEST_ACCEPTED;

            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
            Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", type);
            BlackboardUtils.SetOrCreateValue<long>(newBB, "clubId", 0L);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "clubSymbol", "Club Symbol 02");
            BlackboardUtils.SetOrCreateValue<int>(newBB, "clubLeagueTier", UnityEngine.Random.Range(1, 8));

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void FriendInvite()
        {
            PollType type = PollType.FRIEND_INVITE;

            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
            Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", type);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("test Id {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "name", string.Format("test name {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<int>(newBB, "tier", UnityEngine.Random.Range(0, 21));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "profileUrl", "");
            BlackboardUtils.SetOrCreateValue<string>(newBB, "profileHighResolutionUrl", "");
            BlackboardUtils.SetOrCreateValue<int>(newBB, "gameId", 1);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "betZoneId", "150");
            BlackboardUtils.SetOrCreateValue<string>(newBB, "roomId", "rm:50:150:1");
            BlackboardUtils.SetOrCreateValue<InviteType>(newBB, "inviteType", InviteType.FRIEND);

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void ClubInvite()
        {
            PollType type = PollType.CLUB_INVITE;

            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
            Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", type);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("test Id {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "name", string.Format("test name {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<int>(newBB, "clubLeagueTier", UnityEngine.Random.Range(1, 8));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "clubName", string.Format("Test Club {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "clubSymbol", "Club Symbol 02");
            BlackboardUtils.SetOrCreateValue<InviteType>(newBB, "inviteType", InviteType.CLUB);

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void GameJackpotGrand()
        {
            PollType type = PollType.JACKPOT_WIN;

            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
            Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", type);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("test Id {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "name", string.Format("test name {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<int>(newBB, "tier", UnityEngine.Random.Range(0, 21));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "profileUrl", "");
            BlackboardUtils.SetOrCreateValue<long>(newBB, "winCredit", 100000000);
            BlackboardUtils.SetOrCreateValue<int>(newBB, "gameId", 1);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "betZoneId", "150");
            BlackboardUtils.SetOrCreateValue<JackpotKudoType>(newBB, "jackpotKudoType", JackpotKudoType.GRAND);
            BlackboardUtils.SetOrCreateValue<int>(newBB, "kudoId", 123);

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void GameJackpotMega()
        {
            PollType type = PollType.JACKPOT_WIN;

            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
            Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", PollType.JACKPOT_WIN);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("test Id {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "name", string.Format("test name {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<int>(newBB, "tier", UnityEngine.Random.Range(0, 21));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "profileUrl", "");
            BlackboardUtils.SetOrCreateValue<long>(newBB, "winCredit", 99990000);
            BlackboardUtils.SetOrCreateValue<int>(newBB, "gameId", 1);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "betZoneId", "150");
            BlackboardUtils.SetOrCreateValue<JackpotKudoType>(newBB, "jackpotKudoType", JackpotKudoType.MEGA);
            BlackboardUtils.SetOrCreateValue<int>(newBB, "kudoId", 123);

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void GameJackpotMajor()
        {
            PollType type = PollType.JACKPOT_WIN;

            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
            Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", PollType.JACKPOT_WIN);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("test Id {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "name", string.Format("test name {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<int>(newBB, "tier", UnityEngine.Random.Range(0, 21));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "profileUrl", "");
            BlackboardUtils.SetOrCreateValue<long>(newBB, "winCredit", 8880000);
            BlackboardUtils.SetOrCreateValue<int>(newBB, "gameId", 1);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "betZoneId", "150");
            BlackboardUtils.SetOrCreateValue<JackpotKudoType>(newBB, "jackpotKudoType", JackpotKudoType.MAJOR);
            BlackboardUtils.SetOrCreateValue<int>(newBB, "kudoId", 123);

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void MetaJackpotDailyBonusWheel()
        {
            PollType type = PollType.META_JACKPOT_WIN;

            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
            Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", type);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("test Id {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "name", string.Format("test name {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<int>(newBB, "tier", UnityEngine.Random.Range(0, 21));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "profileUrl", "");
            BlackboardUtils.SetOrCreateValue<long>(newBB, "winCredit", 1987540);
            BlackboardUtils.SetOrCreateValue<MetaJackpotType>(newBB, "type", MetaJackpotType.DAILY_BONUS);
            BlackboardUtils.SetOrCreateValue<JackpotKudoType>(newBB, "jackpotKudoType", JackpotKudoType.MINI);
            BlackboardUtils.SetOrCreateValue<int>(newBB, "kudoId", 123);

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void MetaJackpotMegaWheel()
        {
            PollType type = PollType.META_JACKPOT_WIN;

            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
            Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", type);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("test Id {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "name", string.Format("test name {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<int>(newBB, "tier", UnityEngine.Random.Range(0, 21));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "profileUrl", "");
            BlackboardUtils.SetOrCreateValue<long>(newBB, "winCredit", 1987540);
            BlackboardUtils.SetOrCreateValue<MetaJackpotType>(newBB, "type", MetaJackpotType.DAILY_MEGA_WHEEL);
            BlackboardUtils.SetOrCreateValue<JackpotKudoType>(newBB, "jackpotKudoType", JackpotKudoType.MINI);
            BlackboardUtils.SetOrCreateValue<int>(newBB, "kudoId", 123);

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void MetaJackpotGemJackpot()
        {
            PollType type = PollType.META_JACKPOT_WIN;

            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
            Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", type);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("test Id {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "name", string.Format("test name {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<int>(newBB, "tier", UnityEngine.Random.Range(0, 21));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "profileUrl", "");
            BlackboardUtils.SetOrCreateValue<long>(newBB, "winCredit", 1987540);
            BlackboardUtils.SetOrCreateValue<MetaJackpotType>(newBB, "type", MetaJackpotType.GEM_JACKPOT);
            BlackboardUtils.SetOrCreateValue<JackpotKudoType>(newBB, "jackpotKudoType", JackpotKudoType.MINI);
            BlackboardUtils.SetOrCreateValue<int>(newBB, "kudoId", 123);

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void GameJackpotMinor()
        {
            PollType type = PollType.JACKPOT_WIN;

            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
            Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", type);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("test Id {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "name", string.Format("test name {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<int>(newBB, "tier", UnityEngine.Random.Range(0, 21));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "profileUrl", "");
            BlackboardUtils.SetOrCreateValue<long>(newBB, "winCredit", 200000);
            BlackboardUtils.SetOrCreateValue<int>(newBB, "gameId", 1);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "betZoneId", "150");
            BlackboardUtils.SetOrCreateValue<JackpotKudoType>(newBB, "jackpotKudoType", JackpotKudoType.MINOR);
            BlackboardUtils.SetOrCreateValue<int>(newBB, "kudoId", 123);

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void GameJackpotMini()
        {
            PollType type = PollType.JACKPOT_WIN;

            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
            Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", PollType.JACKPOT_WIN);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("test Id {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "name", string.Format("test name {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<int>(newBB, "tier", UnityEngine.Random.Range(0, 21));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "profileUrl", "");
            BlackboardUtils.SetOrCreateValue<long>(newBB, "winCredit", 10000);
            BlackboardUtils.SetOrCreateValue<int>(newBB, "gameId", 1);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "betZoneId", "150");
            BlackboardUtils.SetOrCreateValue<JackpotKudoType>(newBB, "jackpotKudoType", JackpotKudoType.MINI);
            BlackboardUtils.SetOrCreateValue<int>(newBB, "kudoId", 123);

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void TournamentWin()
        {
            PollType type = PollType.TOURNAMENT_TOP_RANKER;

            Blackboard newBB = BlackboardUtils.CreateBlackboard(string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", type);
            BlackboardUtils.SetOrCreateValue<long>(newBB, "winCredit", 1000000);
            BlackboardUtils.SetOrCreateValue<int>(newBB, "kudoId", 123);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("test Id {0}", testIndex));

            var profileBB = BlackboardUtils.GetOrCreateBlackboard(newBB, "profile");

            UserProfile newProfile = new UserProfile();
            newProfile.userId = "test_id";
            newProfile.name = "Test Win";
            newProfile.profileUrl = "";
            newProfile.tier = UnityEngine.Random.Range(0, 21);

            ClientAPI2Blackboard.Serialize(profileBB, newProfile);

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void TournamentEnd()
        {
            PollType type = PollType.TOURNAMENT_END;

            Blackboard newBB = BlackboardUtils.CreateBlackboard(string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", type);
            BlackboardUtils.SetOrCreateValue<int>(newBB, "rank", 1);
            BlackboardUtils.SetOrCreateValue<long>(newBB, "winCredit", 1000000);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "tournamentId", "test_id");
            BlackboardUtils.SetOrCreateValue<int>(newBB, "serialWinCount", 1);
            BlackboardUtils.SetOrCreateValue<double>(newBB, "serialWinBonus", 2.0);

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void Maintenance(string text)
        {
            // var mogBB = (Blackboard)BlackboardUtils.CreateBlackboard(string.Format("Test_{0}", testIndex));
            // BlackboardUtils.SetOrCreateValue<PollType>(mogBB, "__event__", PollType.NOTICE);
            // BlackboardUtils.SetOrCreateValue<long>(mogBB, "id", 0L);
            // BlackboardUtils.SetOrCreateValue<bool>(mogBB, "isHighlighted", true);
            // BlackboardUtils.SetOrCreateValue<string>(mogBB, "message", string.Format("Our app will undergo maintenance in {0}.<br>Sorry for the interruption. We will be back soon!", text));
            // BlackboardUtils.AddToBlackboardList(MainBlackboard.Get(), "feedEventList", mogBB);
            MetaFeedUtils.SendMaintenanceFeed(text);

            testIndex++;
            // var mog = BlackboardUtils.FindVariable<long>(null, "/common/MOG");
            // long currentTimestamp = TimeUtils.GetTimeStamp();
            // mog.value = currentTimestamp + time + 1000L; // 1000 waitng interval
        }

        [TabGroup("Kudo"), Button]
        public void FirstGlobalChat()
        {
            PollType type = PollType.FIRST_GLOBAL_CHAT;

            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
            Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", type);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("test Id {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "name", string.Format("test name {0}", testIndex));
            BlackboardUtils.SetOrCreateValue<int>(newBB, "tier", UnityEngine.Random.Range(0, 21));
            BlackboardUtils.SetOrCreateValue<string>(newBB, "profileUrl", "");
            BlackboardUtils.SetOrCreateValue<int>(newBB, "kudoId", 123);

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void BossRaidersRound()
        {
            PollType type = PollType.BOSS_RAIDERS_ROUND_COMPLETE;

            Blackboard newBB = BlackboardUtils.CreateBlackboard(string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", type);
            BlackboardUtils.SetOrCreateValue<bool>(newBB, "metaStart", false);
            BlackboardUtils.AddToBlackboardList(MainBlackboard.Get(), "feedEventList", newBB);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "clubSymbol", "Club Symbol 05");

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void BossRaidersEnd()
        {
            PollType type = PollType.BOSS_RAIDERS_END;

            Blackboard newBB = BlackboardUtils.CreateBlackboard(string.Format("Test_{0}", testIndex)) as Blackboard;
            // temp
            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", type);
            BlackboardUtils.SetOrCreateValue<int>(newBB, "kudoId", 123);
            BlackboardUtils.AddToBlackboardList(MainBlackboard.Get(), "feedEventList", newBB);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "clubSymbol", "Club Symbol 05");

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void ClubArenaRankingUp()
        {
            PollType type = PollType.CLUB_ARENA_RANKING_UP;
            Blackboard newBB = BlackboardUtils.CreateBlackboard(string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", type);
            BlackboardUtils.SetOrCreateValue<int>(newBB, "kudoId", 123);
            BlackboardUtils.AddToBlackboardList(MainBlackboard.Get(), "feedEventList", newBB);

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void ClubArenaOneDayLeft()
        {
            PollType type = PollType.CLUB_ARENA_ONE_DAY_LEFT;
            Blackboard newBB = BlackboardUtils.CreateBlackboard(string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", type);
            BlackboardUtils.SetOrCreateValue<int>(newBB, "kudoId", 123);
            BlackboardUtils.AddToBlackboardList(MainBlackboard.Get(), "feedEventList", newBB);

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void ClubArenaEnd()
        {
            PollType type = PollType.CLUB_ARENA_END;
            Blackboard newBB = BlackboardUtils.CreateBlackboard(string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", type);
            BlackboardUtils.SetOrCreateValue<int>(newBB, "kudoId", 123);
            BlackboardUtils.AddToBlackboardList(MainBlackboard.Get(), "feedEventList", newBB);

            SendKudoEvent(type, newBB);
        }

        [TabGroup("Kudo"), Button]
        public void ClubArenaRevengeLocalPush()
        {
            MetaFeedUtils.SendClubArenaRevenge(StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_ARENA_KUDO_REVENGE_TEXT"));
            ++testIndex;
        }

        [TabGroup("Kudo"), Button]
        public void ClubArenaRewardLocalPush()
        {
            MetaFeedUtils.SendClubArenaReward();
            ++testIndex;
        }

        [TabGroup("Kudo"), Button]
        public void ClubArenaStartLocalPush()
        {
            MetaFeedUtils.SendClubArenaStart();
            ++testIndex;
        }

        [TabGroup("Kudo"), Button]
        public void ProgrammedKudoWin()
        {
            PollType type = PollType.PROGRAMMED_KUDO;

            Blackboard newBB = BlackboardUtils.CreateBlackboard(string.Format("Test_{0}", testIndex)) as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", type);
            BlackboardUtils.SetOrCreateValue<WinType>(newBB, "winType", WinType.EPIC);
            var profileUrlList = new List<string>();
            profileUrlList.Add("https://cdn.bagelgames.com/common/images/profile_default_v2/gdw_03.png");
            profileUrlList.Add("https://cdn.bagelgames.com/common/images/profile_default_v2/lps_04.png");
            profileUrlList.Add("https://cdn.bagelgames.com/common/images/profile_default_v2/lgs_01.png");
            BlackboardUtils.SetOrCreateValue<List<string>>(newBB, "profileUrlList", profileUrlList);
            BlackboardUtils.SetOrCreateValue<string>(newBB, "id", BlackboardQueryUtils.GetMyUserId());
            BlackboardUtils.SetOrCreateValue<int>(newBB, "likeCount", 545);

            SendKudoEvent(type, newBB);
            ++testIndex;
        }
        #endregion

        #region Seat

        [TabGroup("Seat"), Button]
        public void SeatEnter()
        {
            BlackboardUtils.SetOrCreateValue<bool>(MainBlackboard.Get(), "seatEditorMode", true);

            Room newRoom = new Room();

            newRoom.roomId = BlackboardUtils.FindVariable<string>(ContentBlackboard.Get(), "room/roomId").value;

            var countryList = CountryUtils.GetCountryCodeList();

            for (int i = 0; i < SEAT_COUNT; ++i)
            {
                UserInfoRoom userInfo = new UserInfoRoom();

                userInfo.userId = string.Format("TestUserID{0}", i);
                userInfo.name = string.Format("Test User {0}", i);
                // public string profileUrl = "";
                userInfo.credit = (long)UnityEngine.Random.Range(100, 2000000000);
                userInfo.tier = UnityEngine.Random.Range(0, 21);
                userInfo.reportCount = 0;

                var randIndex = UnityEngine.Random.Range(0, countryList.Count - 1);
                userInfo.countrySelected = countryList[randIndex];

                userInfo.clubId = UnityEngine.Random.Range(0, 2);
                userInfo.clubLeagueTier = UnityEngine.Random.Range(1, 14);

                newRoom.players.Add(userInfo);
            }

            Blackboard roomBB = BlackboardUtils.GetOrCreateBlackboard(ContentBlackboard.Get(), "room") as Blackboard;
            ClientAPI2Blackboard.Serialize(roomBB, newRoom);
            BlackboardQueryUtils.UpdateSeat(newRoom);
        }

        [TabGroup("Seat"), Button]
        public void SeatLeave()
        {
            BlackboardUtils.SetOrCreateValue<bool>(MainBlackboard.Get(), "seatEditorMode", false);

            // Room newRoom = new Room();

            // newRoom.roomId = BlackboardUtils.FindVariable<string>(ContentBlackboard.Get(), "room/roomId").value;

            // Blackboard roomBB = BlackboardUtils.GetOrCreateBlackboard(ContentBlackboard.Get(), "room") as Blackboard;
            // ClientAPI2Blackboard.Serialize(roomBB, newRoom);
            // BlackboardQueryUtils.UpdateSeat(newRoom);
        }

        //
        // POLL_WIN
        // POLL_LIKE

        // UNKNOWN = -1,
        // KUDO_RECEIVE = 1,
        // CHAT = 2,
        // LEVEL_UP = 4,
        // TIER_UP = 5,
        // LIKE = 6,
        // WIN = 7,
        // JACKPOT_WIN = 19,
        // META_JACKPOT_WIN = 20,
        // DAILY_BONUS_JACKPOT_WIN = 21,
        // FRIEND_INVITE = 22,
        // FRIEND_CONNECT = 23,
        // NOTICE = 24,
        // ACTION_NOTICE = 25,
        // ACTION_IMAGE = 26,
        // HEADS_UP_FINISH_GAME = 30,
        // HEADS_UP_CHAMPION = 31,
        // HEADS_UP_END = 32,
        // BONUS_TRIGGER = 33,
        // TOURNAMENT_START = 34,
        // TOURNAMENT_END = 35,
        // TOURNAMENT_TOP_RANKER = 36

        [TabGroup("Seat"), Button]
        public void SeatLevelUp()
        {
            List<Blackboard> dataList = new List<Blackboard>();

            for (int i = 0; i < SEAT_COUNT; ++i)
            {
                var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
                Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

                BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", PollType.LEVEL_UP);
                BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("TestUserID{0}", i));
                BlackboardUtils.SetOrCreateValue<int>(newBB, "level", UnityEngine.Random.Range(2, 200));

                dataList.Add(newBB);

                ++testIndex;
            }

            StartCoroutine(SendEventCoroutine("POLL_LEVEL_UP", dataList));
        }

        [TabGroup("Seat"), Button]
        public void SeatTierUp()
        {
            List<Blackboard> dataList = new List<Blackboard>();

            for (int i = 0; i < SEAT_COUNT; ++i)
            {
                var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
                Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

                BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", PollType.TIER_UP);
                BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("TestUserID{0}", i));
                BlackboardUtils.SetOrCreateValue<int>(newBB, "tier", UnityEngine.Random.Range(0, 21));

                dataList.Add(newBB);

                ++testIndex;
            }

            StartCoroutine(SendEventCoroutine("POLL_TIER_UP", dataList));
        }

        [TabGroup("Seat"), Button]
        public void SeatBigWin()
        {
            List<Blackboard> dataList = new List<Blackboard>();

            for (int i = 0; i < SEAT_COUNT; ++i)
            {
                var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
                Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

                BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", PollType.WIN);
                BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("TestUserID{0}", i));
                BlackboardUtils.SetOrCreateValue<WinType>(newBB, "winType", WinType.BIG);
                BlackboardUtils.SetOrCreateValue<long>(newBB, "winCredit", UnityEngine.Random.Range(20000, 1000000));

                dataList.Add(newBB);

                ++testIndex;
            }

            StartCoroutine(SendEventCoroutine("POLL_WIN", dataList));
        }

        [TabGroup("Seat"), Button]
        public void SeatSuperBigWin()
        {
            List<Blackboard> dataList = new List<Blackboard>();

            for (int i = 0; i < SEAT_COUNT; ++i)
            {
                var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
                Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

                BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", PollType.WIN);
                BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("TestUserID{0}", i));
                BlackboardUtils.SetOrCreateValue<WinType>(newBB, "winType", WinType.SUPER_BIG);
                BlackboardUtils.SetOrCreateValue<long>(newBB, "winCredit", UnityEngine.Random.Range(20000, 1000000));

                dataList.Add(newBB);

                ++testIndex;
            }

            StartCoroutine(SendEventCoroutine("POLL_WIN", dataList));
        }

        [TabGroup("Seat"), Button]
        public void SeatMegaWin()
        {
            List<Blackboard> dataList = new List<Blackboard>();

            for (int i = 0; i < SEAT_COUNT; ++i)
            {
                var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
                Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

                BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", PollType.WIN);
                BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("TestUserID{0}", i));
                BlackboardUtils.SetOrCreateValue<WinType>(newBB, "winType", WinType.MEGA);
                BlackboardUtils.SetOrCreateValue<long>(newBB, "winCredit", UnityEngine.Random.Range(20000, 1000000));

                dataList.Add(newBB);

                ++testIndex;
            }

            StartCoroutine(SendEventCoroutine("POLL_WIN", dataList));
        }

        [TabGroup("Seat"), Button]
        public void SeatSuperMegaWin()
        {
            List<Blackboard> dataList = new List<Blackboard>();

            for (int i = 0; i < SEAT_COUNT; ++i)
            {
                var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
                Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

                BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", PollType.WIN);
                BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("TestUserID{0}", i));
                BlackboardUtils.SetOrCreateValue<WinType>(newBB, "winType", WinType.SUPER_MEGA);
                BlackboardUtils.SetOrCreateValue<long>(newBB, "winCredit", UnityEngine.Random.Range(20000, 1000000));

                dataList.Add(newBB);

                ++testIndex;
            }

            StartCoroutine(SendEventCoroutine("POLL_WIN", dataList));
        }

        [TabGroup("Seat"), Button]
        public void SeatEpicWin()
        {
            List<Blackboard> dataList = new List<Blackboard>();

            for (int i = 0; i < SEAT_COUNT; ++i)
            {
                var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
                Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

                BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", PollType.WIN);
                BlackboardUtils.SetOrCreateValue<string>(newBB, "userId", string.Format("TestUserID{0}", i));
                BlackboardUtils.SetOrCreateValue<WinType>(newBB, "winType", WinType.EPIC);
                BlackboardUtils.SetOrCreateValue<long>(newBB, "winCredit", UnityEngine.Random.Range(20000, 1000000));

                dataList.Add(newBB);

                ++testIndex;
            }

            StartCoroutine(SendEventCoroutine("POLL_WIN", dataList));
        }

        private IEnumerator SendEventCoroutine(string evtName, List<Blackboard> dataList)
        {
            for (int i = 0; i < dataList.Count; ++i)
            {
                // GraphOwner.SendGlobalEvent<Blackboard>(evtName, dataList[i]);
                var e = new EventData<Blackboard>(evtName, dataList[i]);
                MessageDispatcher.Dispatch(MetaEventDefine.ON_LONG_POLL_EVENT, e);

                yield return new WaitForSeconds(0.1f);
            }
        }

        [TabGroup("Seat"), Button]
        public void SeatLikeToMe()
        {
            // from -> target.
            string meID = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "me/userId").value;
            // POLL_LIKE
            // PollDataLike

            List<Blackboard> dataList = new List<Blackboard>();

            for (int i = 0; i < SEAT_COUNT; ++i)
            {
                var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
                Blackboard pollBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

                BlackboardUtils.SetOrCreateValue<PollType>(pollBB, "__event__", PollType.LIKE);
                BlackboardUtils.SetOrCreateValue<string>(pollBB, "userId", string.Format("TestUserID{0}", i));
                BlackboardUtils.SetOrCreateValue<string>(pollBB, "targetUserId", meID);

                dataList.Add(pollBB);

                ++testIndex;
            }

            StartCoroutine(SendEventCoroutine("POLL_LIKE", dataList));
        }

        [TabGroup("Seat"), Button]
        public void SeatLikeToUser()
        {
            // from -> target.
            string meID = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "me/userId").value;
            // POLL_LIKE
            // PollDataLike

            List<Blackboard> dataList = new List<Blackboard>();

            for (int i = 0; i < SEAT_COUNT; ++i)
            {
                var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");

                Blackboard pollBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

                BlackboardUtils.SetOrCreateValue<PollType>(pollBB, "__event__", PollType.LIKE);
                BlackboardUtils.SetOrCreateValue<string>(pollBB, "userId", meID);
                BlackboardUtils.SetOrCreateValue<string>(pollBB, "targetUserId", string.Format("TestUserID{0}", i));

                dataList.Add(pollBB);

                ++testIndex;
            }

            StartCoroutine(SendEventCoroutine("POLL_LIKE", dataList));
        }

        [TabGroup("Seat"), Button]
        public void SeatLikeUserToUser()
        {
            // from -> target.
            string meID = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "me/userId").value;
            // POLL_LIKE
            // PollDataLike

            List<Blackboard> dataList = new List<Blackboard>();

            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");

            for (int i = 0; i < SEAT_COUNT; ++i)
            {
                for (int j = 0; j < 6; ++j)
                {
                    if (i != j)
                    {
                        Blackboard pollBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

                        BlackboardUtils.SetOrCreateValue<PollType>(pollBB, "__event__", PollType.LIKE);

                        BlackboardUtils.SetOrCreateValue<string>(pollBB, "userId", string.Format("TestUserID{0}", i));

                        BlackboardUtils.SetOrCreateValue<string>(pollBB, "targetUserId", string.Format("TestUserID{0}", j));

                        dataList.Add(pollBB);

                        ++testIndex;
                    }
                }
            }

            StartCoroutine(SendEventCoroutine("POLL_LIKE", dataList));
        }

        #endregion

        #region Challange

        [TabGroup("Challenge"), Button]
        public void DailyMissionAlarmTest()
        {
            List<string> eventAlarmTextList = new List<string>();
            eventAlarmTextList.Add("Dummy Alarm Test");

            GraphOwner.SendGlobalEvent<List<string>>("DailyMissionAlarm", eventAlarmTextList);
        }

        [TabGroup("Challenge"), Button]
        public void ChallengeMissionComplete(ChallengeType challengeType, int challengeProgress, bool challengeDone, bool challengeClaimed)
        {
            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
            Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

            // Challenge Reward ///
            RewardInfoValueCredit challengeRewardCreditInfo = new RewardInfoValueCredit();
            challengeRewardCreditInfo.credit = 120000;
            RewardInfo challengeRewardInfo = new RewardInfo();
            challengeRewardInfo.type = RewardType.CREDIT;
            challengeRewardInfo.value = challengeRewardCreditInfo;
            ///////////////////////

            // RewardResult ///
            RewardResultCredit rewardResultCredit = new RewardResultCredit();
            rewardResultCredit.credit = 1200;

            RewardResult rewardResultInfo = new RewardResult();
            rewardResultInfo.rewardType = RewardType.CREDIT;
            rewardResultInfo.rewardResult = rewardResultCredit;
            ////////////

            // RewardInfo ///
            RewardInfoValueCredit rewardCreditInfo = new RewardInfoValueCredit();
            rewardCreditInfo.credit = 1200;
            RewardInfo rewardInfo = new RewardInfo();
            rewardInfo.type = RewardType.CREDIT;
            rewardInfo.value = rewardCreditInfo;
            /////////////////

            // Mission DetailInfo ///
            // ChallengeMissionValueEnterNthRoundTournamnet detailInfo = new ChallengeMissionValueEnterNthRoundTournamnet();
            // detailInfo.round = 1;
            ChallengeMissionValueAchieveTotalWinCreditByBigWinTargeted detailInfo = new ChallengeMissionValueAchieveTotalWinCreditByBigWinTargeted();
            detailInfo.gameId = 1;
            detailInfo.winType = "BIG";
            /////////////////////////

            // Mission //
            ChallengeMissionV1 missionInfo = new ChallengeMissionV1();
            missionInfo.missionType = ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_TARGETED;
            missionInfo.info = detailInfo;
            missionInfo.completeCount = 10L;
            missionInfo.reward = rewardInfo;
            missionInfo.progress = 10L;
            missionInfo.done = true;
            /////////////

            // Simple Info ///
            ChallengeInfoSimple simpleChallengeInfo = new ChallengeInfoSimple();
            simpleChallengeInfo.challengeType = challengeType;
            simpleChallengeInfo.challengeProgress = challengeProgress;
            simpleChallengeInfo.done = challengeDone;
            simpleChallengeInfo.claimed = challengeClaimed;
            List<RewardInfo> rewardList = new List<RewardInfo>();
            rewardList.Add(challengeRewardInfo);
            simpleChallengeInfo.rewardList = rewardList;
            //////////////////

            PollDataChallengeMissionCompleteV1 challengeMissionComplete = new PollDataChallengeMissionCompleteV1();

            challengeMissionComplete.rewardResult = rewardResultInfo;
            challengeMissionComplete.mission = missionInfo;
            challengeMissionComplete.challenge = simpleChallengeInfo;

            PollV1 poll = new PollV1();
            poll.__event__ = PollType.CHALLENGE_MISSION_COMPLETE;
            poll.data = challengeMissionComplete;
            poll.id = testIndex;

            ClientAPI2Blackboard.Serialize(newBB, poll);

            BlackboardUtils.AddToBlackboardList(MainBlackboard.Get(), "challengeEventList", newBB);

            List<Blackboard> dataList = new List<Blackboard>();
            dataList.Add(newBB);
            StartCoroutine(SendEventCoroutine("POLL_CHALLENGE_MISSION_COMPLETE", dataList));

            ++testIndex;
        }

        [TabGroup("Challenge"), Button]
        public void ChallengeComplete(ChallengeType challengeType, int challengeProgress, bool challengeDone, bool challengeClaimed)
        {
            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
            Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

            // Challenge Reward ///
            RewardInfoValueCredit challengeRewardCreditInfo = new RewardInfoValueCredit();
            challengeRewardCreditInfo.credit = 120000;
            RewardInfo challengeRewardInfo = new RewardInfo();
            challengeRewardInfo.type = RewardType.CREDIT;
            challengeRewardInfo.value = challengeRewardCreditInfo;
            ///////////////////////

            // RewardResult ///
            RewardResultCredit rewardResultCredit = new RewardResultCredit();
            rewardResultCredit.credit = 1200;

            RewardResult rewardResultInfo = new RewardResult();
            rewardResultInfo.rewardType = RewardType.CREDIT;
            rewardResultInfo.rewardResult = rewardResultCredit;
            ////////////

            // RewardInfo ///
            RewardInfoValueCredit rewardCreditInfo = new RewardInfoValueCredit();
            rewardCreditInfo.credit = 1200;
            RewardInfo rewardInfo = new RewardInfo();
            rewardInfo.type = RewardType.CREDIT;
            rewardInfo.value = rewardCreditInfo;
            /////////////////

            // Mission DetailInfo ///
            ChallengeMissionValueAchieveTotalWinCreditByBigWinAny detailInfo = new ChallengeMissionValueAchieveTotalWinCreditByBigWinAny();
            detailInfo.winType = "BIG";
            /////////////////////////

            // Mission //
            ChallengeMissionV1 missionInfo = new ChallengeMissionV1();
            missionInfo.missionType = ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_ANY;
            missionInfo.info = detailInfo;
            missionInfo.completeCount = 10L;
            missionInfo.reward = rewardInfo;
            missionInfo.progress = 10L;
            missionInfo.done = true;
            /////////////

            // Simple Info ///
            ChallengeInfoSimple simpleChallengeInfo = new ChallengeInfoSimple();
            simpleChallengeInfo.challengeType = challengeType;
            simpleChallengeInfo.challengeProgress = challengeProgress;
            simpleChallengeInfo.done = challengeDone;
            simpleChallengeInfo.claimed = challengeClaimed;
            List<RewardInfo> rewardList = new List<RewardInfo>();
            rewardList.Add(challengeRewardInfo);
            simpleChallengeInfo.rewardList = rewardList;
            //////////////////

            PollDataChallengeMissionCompleteV1 challengeMissionComplete = new PollDataChallengeMissionCompleteV1();

            challengeMissionComplete.rewardResult = rewardResultInfo;
            challengeMissionComplete.mission = missionInfo;
            challengeMissionComplete.challenge = simpleChallengeInfo;

            PollV1 poll = new PollV1();
            poll.__event__ = PollType.CHALLENGE_MISSION_COMPLETE;
            poll.data = challengeMissionComplete;
            poll.id = testIndex;

            ClientAPI2Blackboard.Serialize(newBB, poll);

            BlackboardUtils.AddToBlackboardList(MainBlackboard.Get(), "challengeEventList", newBB);

            var e = new EventData<Blackboard>("OnStartCompleteChallengePopup", newBB);
            MessageDispatcher.Dispatch("OnMetaUIEvent", e);

            // List<Blackboard> dataList = new List<Blackboard>();
            // dataList.Add(newBB);
            // StartCoroutine( SendEventCoroutine("POLL_CHALLENGE_MISSION_COMPLETE", dataList) );

            ++testIndex;
        }

        [TabGroup("Challenge"), Button]
        public void ClubChallengeMissionComplete(int challengeProgress, int clubChallengeStage, ClubChallengeType clubChallengeType)
        {
            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "TestFeed");
            Blackboard newBB = BlackboardUtils.GetOrCreateBlackboard(bb, string.Format("Test_{0}", testIndex)) as Blackboard;

            // Challenge Reward ///
            RewardInfoValueCredit challengeRewardCreditInfo = new RewardInfoValueCredit();
            challengeRewardCreditInfo.credit = 142000;
            RewardInfo challengeRewardInfo = new RewardInfo();
            challengeRewardInfo.type = RewardType.CREDIT;
            challengeRewardInfo.value = challengeRewardCreditInfo;
            ///////////////////////

            // RewardResult ///
            RewardResultCredit rewardResultCredit = new RewardResultCredit();
            rewardResultCredit.credit = 94000;

            RewardResult rewardResultInfo = new RewardResult();
            rewardResultInfo.rewardType = RewardType.CREDIT;
            rewardResultInfo.rewardResult = rewardResultCredit;
            ////////////

            // RewardInfo ///
            RewardInfoValueCredit rewardCreditInfo = new RewardInfoValueCredit();
            rewardCreditInfo.credit = 120000;
            RewardInfo rewardInfo = new RewardInfo();
            rewardInfo.type = RewardType.CREDIT;
            rewardInfo.value = rewardCreditInfo;
            /////////////////

            // Mission DetailInfo ///
            // ChallengeMissionValueEnterNthRoundTournamnet detailInfo = new ChallengeMissionValueEnterNthRoundTournamnet();
            // detailInfo.round = 1;
            ClubChallengeMissionValueAchieveTotalWinCreditByBigWinTargeted detailInfo = new ClubChallengeMissionValueAchieveTotalWinCreditByBigWinTargeted();
            detailInfo.gameId = 1;
            detailInfo.winType = "BIG";
            /////////////////////////

            // Mission //
            ClubChallengeMission missionInfo = new ClubChallengeMission();
            missionInfo.missionType = ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_TARGETED;
            missionInfo.info = detailInfo;
            missionInfo.completeCount = 10L;
            missionInfo.reward = rewardInfo;
            missionInfo.progress = 100L;
            missionInfo.done = true;
            missionInfo.leaguePoint = 0;
            /////////////

            // Simple Info ///
            ClubChallengeInfoSimple simpleChallengeInfo = new ClubChallengeInfoSimple();
            simpleChallengeInfo.challengeProgress = challengeProgress;
            simpleChallengeInfo.maxChallengeProgress = 4;
            simpleChallengeInfo.reward = challengeRewardInfo;
            simpleChallengeInfo.stage = clubChallengeStage;
            simpleChallengeInfo.type = clubChallengeType;
            simpleChallengeInfo.maxStage = 4;
            List<RewardInfo> rewardList = new List<RewardInfo>();
            rewardList.Add(challengeRewardInfo);
            simpleChallengeInfo.rewardList = rewardList;
            //////////////////

            PollDataClubChallengeMissionComplete challengeMissionComplete = new PollDataClubChallengeMissionComplete();

            challengeMissionComplete.rewardResult = rewardResultInfo;
            challengeMissionComplete.mission = missionInfo;
            challengeMissionComplete.challenge = simpleChallengeInfo;

            PollV1 poll = new PollV1();
            poll.__event__ = PollType.CLUB_CHALLENGE_MISSION_COMPLETE;
            poll.data = challengeMissionComplete;
            poll.id = testIndex;

            ClientAPI2Blackboard.Serialize(newBB, poll);

            BlackboardUtils.AddToBlackboardList(MainBlackboard.Get(), "challengeEventList", newBB);

            List<Blackboard> dataList = new List<Blackboard>();
            dataList.Add(newBB);
            StartCoroutine(SendEventCoroutine("POLL_CLUB_CHALLENGE_MISSION_COMPLETE", dataList));

            // var e = new EventData<Blackboard>("OnStartCompleteClubChallengePopup", newBB);
            // MessageDispatcher.Dispatch("OnMetaUIEvent", e);

            ++testIndex;
        }

        #endregion

        #region Push

        [TabGroup("Push"), Button]
        public void TestPush(ActionType testActionType, string testActionData)
        {
            DeepLinkData testData = new DeepLinkData();

            testData.type = string.Format("test name {0}", testIndex);
            testData.ts = TimeUtils.GetTimeStamp();
            testData.comment = string.Format("Test Comment {0}", testIndex);
            testData.templateId = string.Format("Test Temp ID {0}", testIndex);

            testData.action = new BagelCode.ClientModels.Action();

            testData.action.type = testActionType;

            switch (testActionType)
            {
                case ActionType.COUPON_REDEEM:
                    {
                        var redeemData = new BagelCode.ClientModels.ActionDataCouponRedeem();
                        redeemData.code = testActionData;
                        testData.action.data = redeemData;
                    }
                    break;
                case ActionType.SNS_INVITE:
                    {
                        var redeemData = new ActionDataSnsInvite();
                        redeemData.inviterUserId = "f6b3c6a7-f6c8-5a36-b433-1508f300d309";
                        testData.action.data = redeemData;
                    }
                    break;
                case ActionType.FACEBOOK_INVITE:
                    {
                        var redeemData = new ActionDataFbMessageInvite();
                        redeemData.inviterUserId = "f6b3c6a7-f6c8-5a36-b433-1508f300d309";
                        testData.action.data = redeemData;
                    }
                    break;
            }

            PendingActionManager.Instance.TestPushPendingAction(testData);
        }

        [TabGroup("Push"), Button]
        public void TestPushDirectData(string testActionData)
        {
            PendingActionManager.Instance.PushPendingAction(testActionData);
        }

        [TabGroup("Push"), Button]
        public void BossRaidersLocalPush()
        {
            MetaFeedUtils.SendBossRaidersFeed(StringTableUtils.GetString(StringTable.StringTableType.Global, "BOSS_RAIDERS_KUDO_META_GAME_START"));
            ++testIndex;
        }

        #endregion

        // public double revenue;
        // public double lifetimeSpend;

        // public void TestRevenue()
        // {
        //     AdjustManager.Instance.SendConversionValueEvent(revenue, lifetimeSpend);
        // }
#endif
    }
}
