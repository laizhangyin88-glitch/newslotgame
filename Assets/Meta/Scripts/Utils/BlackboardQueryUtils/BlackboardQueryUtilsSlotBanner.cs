using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{
    public static partial class BlackboardQueryUtils
    {
        private const string SLOT_BANNER_GROUP_LIST = "slotBannerGroupList";
        private const string SLOT_BANNER_LIST = "slotBannerList";

        public static void UpdateSlotBannerList(List<SlotBannerGroup> slotBannerGroupList)
        {
            BlackboardUtils.SetOrCreateList(MainBlackboard.Get(), SLOT_BANNER_GROUP_LIST, slotBannerGroupList, ClientAPI2Blackboard.Serialize);
        }

        public static List<Blackboard> GetValidSlotBannerNoticeList(Blackboard bannerGroupInfo)
        {
            var slotBannerList = bannerGroupInfo.GetValue<List<Blackboard>>(SLOT_BANNER_LIST);
            // ATTENTION: It is ALWAYS return 0 index banners, other cases are not expected!!
            if (slotBannerList.Count == 0)
                return new List<Blackboard>();

            var result = new List<Blackboard>();
            var noticeList = slotBannerList[0].GetValue<List<Blackboard>>(NOTICE_LIST);
            int count = noticeList.Count;
            long currTimestamp = BagelCode.TimeUtils.GetTimeStamp();

            for (int i = 0; i < count; ++i)
            {
                var notice = noticeList[i];
                string uid = notice.GetValue<string>("id");
                long endTimestamp = notice.GetValue<long>("endTimestamp");
                var constraints = notice.GetValue<Blackboard>("constraints");

                string slbCooltimeKey = string.Format("SLBLastTriggeredTime:{0}", uid);
                long lastTriggerTimestamp = PlayerPrefsUtils.GetInt64(slbCooltimeKey, 0L);

                bool useUserTimer = constraints.GetValue<bool>("useUserTimer");
                if (useUserTimer)
                {
                    long userCoolTime = (long)constraints.GetValue<int>("userTimerMin") * 60000L;
                    
                    string slbKey = string.Format("SLBLastTriggeredTimeByUser:{0}", uid);
                    long lastTriggerTimestampByUser = PlayerPrefsUtils.GetOrCreateInt64(slbKey, currTimestamp);

                    if (endTimestamp == 0L || (lastTriggerTimestampByUser + userCoolTime) < endTimestamp)
                        endTimestamp = lastTriggerTimestampByUser + userCoolTime;
                }

                var slbGlobalCoolTime = (long)constraints.GetValue<int>("cooltimeSec") * 1000L;

                if ((lastTriggerTimestamp + slbGlobalCoolTime) < currTimestamp && (endTimestamp == 0L || currTimestamp < endTimestamp))
                {
                    BlackboardUtils.SetOrCreateValue<long>(notice, "clientTimestamp", endTimestamp);
                    result.Add(notice);

                    PlayerPrefsUtils.SetInt64(slbCooltimeKey, currTimestamp);
                }
            }

            return result;
        }

        public static List<Blackboard> GetSlotBannerGroupList()
        {
            return BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(),
                SLOT_BANNER_GROUP_LIST)?.value ?? new List<Blackboard>();
        }

        public static List<string> GetSlotBannerImageList()
        {
            List<string> imageUrlList = new List<string>();

            var slotBannerGroupList = MainBlackboard.Get().GetValue<List<Blackboard>>(SLOT_BANNER_GROUP_LIST);
            for (int i = 0; i < slotBannerGroupList.Count; ++i)
            {
                var slotBannerList = slotBannerGroupList[i].GetValue<List<Blackboard>>(SLOT_BANNER_LIST);
                for (int j = 0; j < slotBannerList.Count; ++j)
                {
                    var noticeList = slotBannerList[j].GetValue<List<Blackboard>>(NOTICE_LIST);
                    for (int k = 0; k < noticeList.Count; ++k)
                    {
                        imageUrlList.Add( noticeList[k].GetValue<string>("imageUrl") );
                    }
                }
            }

            return imageUrlList;
        }
    }
}
