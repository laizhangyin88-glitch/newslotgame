using BagelCode.ClientModels;
using System.Linq;
using System.Collections.Generic;

namespace BagelCode
{
    public static partial class BlackboardQueryUtils
    {
        public static bool IsAvailablePassiveEventTimestamp(EventInfo eventInfo)
        {
            if (eventInfo == null) return false;

            return TimeUtils.IsAvailableTimestamp(eventInfo.startTimestamp, eventInfo.endTimestamp);
        }

        public static bool IsAvailablePassiveEvent(EventInfo eventInfo)
        {
            if (eventInfo == null) return false;

            return IsAvailablePassiveEventTimestamp(eventInfo);
        }

        public static List<int> GetEventIdList(EventInfoType type)
        {
            var eventInfoList = PassiveEventManager.Instance.GetActiveEventInfoList(type);
            var list = new List<int>();

            if (eventInfoList != null)
                list = eventInfoList.Select(i => i.id).ToList();

            return list;
        }
    }
}