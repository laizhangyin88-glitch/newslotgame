using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{
    public static partial class BlackboardQueryUtils
    {
        private const string IAM_KEY = "IAMTimer:{0}";
        private const string IN_APP_MESSAGE_LIST = "inAppMessageList";

        public static Blackboard GetIAMBlackboard(int iamId)
        {
            var inAppMessageList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), IN_APP_MESSAGE_LIST);
            for (int i = 0; i < inAppMessageList.value.Count; ++i)
            {
                int id = inAppMessageList.value[i].GetValue<int>("id");
                if (iamId == id)
                {
                    return inAppMessageList.value[i];
                }
            }
            return null;
        }

        public static void SetIAMUserTimer(Blackboard iamInfo)
        {
            string key = string.Format(IAM_KEY, iamInfo.GetValue<int>("id"));

            var isUserTimer = BlackboardUtils.FindVariable<bool>(iamInfo, "useUserTimer");
            var userTimerMin = BlackboardUtils.FindVariable<int>(iamInfo, "userTimerMin");
            long userStartTimestamp = System.Convert.ToInt64(PlayerPrefs.GetString(key, "0"));

            if (isUserTimer.value && userTimerMin.value != 0 && userStartTimestamp == 0L)
            {
                PlayerPrefs.SetString(key, TimeUtils.GetTimeStamp().ToString());
            }
        }

        public static List<Blackboard> GetIAMListFromType(InAppMessageType findtype)
        {
            List<Blackboard> findList = new List<Blackboard>();

            var inAppMessageList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), IN_APP_MESSAGE_LIST);

            if (inAppMessageList != null && inAppMessageList.value != null)
            {
                for (int i = 0; i < inAppMessageList.value.Count; ++i)
                {
                    var iamType = BlackboardUtils.FindVariable<InAppMessageType>(inAppMessageList.value[i], "type");

                    if (iamType.value == findtype)
                    {
                        long currentTimestamp = BagelCode.TimeUtils.GetTimeStamp();
                        long endTimestamp = GetTimestamp(inAppMessageList.value[i]);

                        if (endTimestamp == 0 || endTimestamp > currentTimestamp)
                            findList.Add(inAppMessageList.value[i]);
                    }
                }
            }

            return findList;
        }

        public static Dictionary<int, Blackboard> GetBonusIAMDict()
        {
            Dictionary<int, Blackboard> findDict = new Dictionary<int, Blackboard>();

            var inAppMessageList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), IN_APP_MESSAGE_LIST);

            if (inAppMessageList != null && inAppMessageList.value != null)
            {
                for (int i = 0; i < inAppMessageList.value.Count; ++i)
                {
                    var IAMType = BlackboardUtils.FindVariable<InAppMessageType>(inAppMessageList.value[i], "type");

                    if (IAMType.value == InAppMessageType.SUPER_BONUS_PURCHASE_POPUP ||
                       IAMType.value == InAppMessageType.BUY_A_BONUS_PURCHASE_POPUP ||
                       IAMType.value == InAppMessageType.INSTANT_BONUS_PURCHASE_POPUP)
                    {
                        long currentTimestamp = BagelCode.TimeUtils.GetTimeStamp();
                        long endTimestamp = GetTimestamp(inAppMessageList.value[i]);

                        if (endTimestamp == 0 || endTimestamp > currentTimestamp)
                        {
                            int gameId = inAppMessageList.value[i].GetValue<int>("gameId");

                            if (findDict.ContainsKey(gameId))
                            {
                                InAppMessageType existIAMType = findDict[gameId].GetValue<InAppMessageType>("type");

                                if (existIAMType < IAMType.value)
                                    findDict[gameId] = inAppMessageList.value[i];
                            }
                            else
                            {
                                findDict.Add(gameId, inAppMessageList.value[i]);
                            }
                        }
                    }
                }
            }

            return findDict;
        }

        private static long GetTimestamp(Blackboard iamInfoBB)
        {
            long currentTimestamp = BagelCode.TimeUtils.GetTimeStamp();

            InAppMessageType IAMtype = BlackboardUtils.FindVariable<InAppMessageType>(iamInfoBB, "type").value;
            long endTimestamp = BlackboardUtils.FindVariable<long>(iamInfoBB, "endTimestamp").value;
            bool useUserTimer = BlackboardUtils.FindVariable<bool>(iamInfoBB, "useUserTimer").value;
            int userTimerMin = BlackboardUtils.FindVariable<int>(iamInfoBB, "userTimerMin").value;
            int userTimerRegenMin = BlackboardUtils.FindVariable<int>(iamInfoBB, "userTimerRegenMin").value;

            string id = BlackboardUtils.FindVariable<int>(iamInfoBB, "id").value.ToString();
            string iamKey = string.Format(IAM_KEY, id);
            string userStartTimeText = PlayerPrefs.GetString(iamKey, "0");
            long userStartTimestamp = System.Convert.ToInt64(userStartTimeText);
            long userEndTimestamp = userStartTimestamp + (System.Convert.ToInt64(userTimerMin) * 60000L);

            if (useUserTimer && userTimerMin > 0)
            {
                // Regen time min.
                if (userTimerRegenMin > 0 && userEndTimestamp > 0 && currentTimestamp >= userEndTimestamp)
                {
                    long regenCoolTimestamp = System.Convert.ToInt64(userTimerMin + userTimerRegenMin) * 60000L;

                    if (userStartTimestamp + regenCoolTimestamp < currentTimestamp)
                    {
                        userStartTimeText = currentTimestamp.ToString();
                        PlayerPrefs.SetString(iamKey, userStartTimeText);

                        userStartTimestamp = System.Convert.ToInt64(userStartTimeText);
                        userEndTimestamp = userStartTimestamp + (System.Convert.ToInt64(userTimerMin) * 60000L);
                    }
                }

                if (endTimestamp == 0L)
                {
                    endTimestamp = userEndTimestamp;
                }
                else
                {
                    if (userEndTimestamp < endTimestamp)
                    {
                        endTimestamp = userEndTimestamp;
                    }
                }
            }

            return endTimestamp;
        }

        public static void LoadInAppMessageWebImages(CacheType cacheType = CacheType.FileCache, bool priority = false)
        {
            List<string> imageUrlList = GetInAppMessageWebImageUrlList();

            for (int i = 0; i < imageUrlList.Count; i++)
            {
                WebImageDownloadManager.Instance.LoadWebImage(imageUrlList[i], cacheType, priority);
            }
        }

        public static void UpdateInAppMessageList(List<InAppMessageInfo> inAppMessageList)
        {
            BlackboardUtils.SetOrCreateList(MainBlackboard.Get(), IN_APP_MESSAGE_LIST, inAppMessageList, ClientAPI2Blackboard.Serialize);
        }

        public static List<Blackboard> GetInAppMessageList()
        {
            return BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(),
                IN_APP_MESSAGE_LIST)?.value ?? new List<Blackboard>();
        }

        public static void AddInAppMessage(InAppMessageInfo iamInfo)
        {
            var newInfoBB = BlackboardUtils.CreateBlackboard("inAppMessageInfo");
            ClientAPI2Blackboard.Serialize(newInfoBB, iamInfo);
            BlackboardUtils.AddToBlackboardList(MainBlackboard.Get(), IN_APP_MESSAGE_LIST, newInfoBB);
        }

        public static void RemoveIAMBlackboardById(int iamId)
        {
            var inAppMessageList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), IN_APP_MESSAGE_LIST);
            for (int i = 0; i < inAppMessageList.value.Count; ++i)
            {
                int id = inAppMessageList.value[i].GetValue<int>("id");
                if (iamId == id)
                {
                    GameObject.Destroy(inAppMessageList.value[i].gameObject);
                    inAppMessageList.value.RemoveAt(i);
                    return;
                }
            }
        }

        public static List<string> GetInAppMessageWebImageUrlList()
        {
            List<string> imageUrlList = new List<string>();

            var iamInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "inAppMessageList");

            if (iamInfoList.value != null)
            {
                for (int i = 0; i < iamInfoList.value.Count; ++i)
                {
                    var iamType = BlackboardUtils.FindVariable<InAppMessageType>(iamInfoList.value[i], "type");
                    if (iamType.value == InAppMessageType.BUY_A_BONUS_PURCHASE_POPUP)
                        continue;

                    var componentList = BlackboardUtils.FindVariable<List<Blackboard>>(iamInfoList.value[i], "componentList");

                    if (componentList.value != null)
                    {
                        for (int j = 0; j < componentList.value.Count; ++j)
                        {
                            InAppMessageComponentType componentType = componentList.value[j].GetValue<InAppMessageComponentType>("type");

                            if (componentType == InAppMessageComponentType.BACKGROUND_WEB_IMAGE)
                            {
                                string url = componentList.value[j].GetValue<string>("imageUrl");

                                if (!string.IsNullOrEmpty(url))
                                {
                                    imageUrlList.Add(url);
                                }
                            }
                        }
                    }
                }
            }

            return imageUrlList;
        }

        public static long GetWinXNumeratorFromTicketedBonusID(int ticketedBonusID)
        {
            var winxNumeratorDict = BlackboardUtils.FindVariable<Dictionary<int, long>>(MainBlackboard.Get(), "winxNumeratorDict");
            if (winxNumeratorDict == null || winxNumeratorDict.value == null) return NumberUtils.GetGlobalDenominator();

            foreach (var info in winxNumeratorDict.value)
            {
                if (info.Key == ticketedBonusID)
                    return info.Value;
            }

            return NumberUtils.GetGlobalDenominator();
        }

        public static bool GetEnableBucksPurchase(Blackboard product)
        {
            if (product != null)
            {
                var isSubscription = BlackboardUtils.FindVariable<bool>(product, "isSubscription");
                if (isSubscription != null && isSubscription.value == false)
                    return BlackboardUtils.FindVariable<bool>(product, "enableBucksPurchase")?.value ?? false;
            }
            return false;
        }
    }
}
