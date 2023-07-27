using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
    public class MetaFeedUtils
    {
        public enum LocalFeedEventType
        {
            NONE = 0,
            MAINTENANCE,
            BOSSRAIDERS,
            CLUBARENA_REVENGE,
            CLUBARENA_REWARD,
            CLUBARENA_START,
        }

        public static void SendMaintenanceFeed(string message)
        {
            var mogBB = (Blackboard)BlackboardUtils.CreateBlackboard("Poll");
            BlackboardUtils.SetOrCreateValue<LocalFeedEventType>(mogBB, "localFeedEventType", LocalFeedEventType.MAINTENANCE);
            BlackboardUtils.SetOrCreateValue<long>(mogBB, "id", 0L);
            BlackboardUtils.SetOrCreateValue<bool>(mogBB, "isHighlighted", true);
            BlackboardUtils.SetOrCreateValue<string>(mogBB, "message", string.Format("Our app will undergo maintenance in {0}.<br>Sorry for the interruption. We will be back soon!", message));
            // BlackboardUtils.AddToBlackboardList(agent, "data", mogBB);
            MetaFeedUtils.SendLocalFeed(mogBB);
        }

        public static void SendBossRaidersFeed(string message)
        {
            var mogBB = (Blackboard)BlackboardUtils.CreateBlackboard("Poll");
            BlackboardUtils.SetOrCreateValue<LocalFeedEventType>(mogBB, "localFeedEventType", LocalFeedEventType.BOSSRAIDERS);
            BlackboardUtils.SetOrCreateValue<long>(mogBB, "id", 0L);
            BlackboardUtils.SetOrCreateValue<bool>(mogBB, "isHighlighted", false);
            BlackboardUtils.SetOrCreateValue<string>(mogBB, "message", message);
            MetaFeedUtils.SendLocalFeed(mogBB);
        }

        public static void SendClubArenaRevenge(string message)
        {
            var mogBB = (Blackboard)BlackboardUtils.CreateBlackboard("Poll");
            BlackboardUtils.SetOrCreateValue<LocalFeedEventType>(mogBB, "localFeedEventType", LocalFeedEventType.CLUBARENA_REVENGE);
            BlackboardUtils.SetOrCreateValue<string>(mogBB, "message", message);
            MetaFeedUtils.SendLocalFeed(mogBB);
        }

        public static void SendClubArenaReward()
        {
            var mogBB = (Blackboard)BlackboardUtils.CreateBlackboard("Poll");
            BlackboardUtils.SetOrCreateValue<LocalFeedEventType>(mogBB, "localFeedEventType", LocalFeedEventType.CLUBARENA_REWARD);
            MetaFeedUtils.SendLocalFeed(mogBB);
        }

        public static void SendClubArenaStart()
        {
            var mogBB = (Blackboard)BlackboardUtils.CreateBlackboard("Poll");
            BlackboardUtils.SetOrCreateValue<LocalFeedEventType>(mogBB, "localFeedEventType", LocalFeedEventType.CLUBARENA_START);
            MetaFeedUtils.SendLocalFeed(mogBB);
        }

        //

        private static void SendLocalFeed(Blackboard bb)
        {
            var feedType = bb.GetVariable<LocalFeedEventType>("localFeedEventType")?.value;
            EventSender.SendGlobalEvent(
                MetaEventDefine.ON_LOCAL_FEED_EVENT,
                new ParadoxNotion.EventData<Blackboard>(feedType.Value.ToString(), bb));
        }
    }
}
