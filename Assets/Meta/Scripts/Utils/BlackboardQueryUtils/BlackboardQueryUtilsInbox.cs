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
    public static void RemoveInboxItem(int removeInboxID)
    {
        var inboxList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "inboxList");

        if(inboxList == null) return;
        
        for(int i=0; i<inboxList.value.Count; ++i)
        {
            Variable<int> id = BlackboardUtils.FindVariable<int>(inboxList.value[i], "id");
            
            if(id.value == removeInboxID)
            {
                GameObject.Destroy(inboxList.value[i].gameObject);
                inboxList.value.RemoveAt(i);
                break;
            }
        }
    }

    public static void RemoveInboxBannerItem(int removeInboxBannerID)
    {
        var inboxList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "inboxBannerList");

        if(inboxList == null) return;
        
        for(int i=0; i<inboxList.value.Count; ++i)
        {
            Variable<int> id = BlackboardUtils.FindVariable<int>(inboxList.value[i], "id");
            
            if(id.value == removeInboxBannerID)
            {
                GameObject.Destroy(inboxList.value[i].gameObject);
                inboxList.value.RemoveAt(i);
                break;
            }
        }
    }

    public static void RemoveInboxVegasBucksItem(int removeInboxBucksId)
    {
        Blackboard bucksInfoBB = GetUserBucksInfoBB();
        if (bucksInfoBB == null) return;

        var bucksGiftList = BlackboardUtils.FindVariable<List<Blackboard>>(bucksInfoBB, "giftList");

        if(bucksGiftList == null) return;
        
        for(int i=0; i<bucksGiftList.value.Count; ++i)
        {
            Variable<int> id = BlackboardUtils.FindVariable<int>(bucksGiftList.value[i], "giftId");
            
            if(id.value == removeInboxBucksId)
            {
                GameObject.Destroy(bucksGiftList.value[i].gameObject);
                bucksGiftList.value.RemoveAt(i);
                break;
            }
        }
    }

    public static List<Blackboard> GetBlackboardInboxInfoFromType(InboxTypes inboxType)
    {
        List<Blackboard> itemList = new List<Blackboard>();

        var inboxList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "inboxList");
        if(inboxList == null) return itemList;

        for(int i=0; i<inboxList.value.Count; ++i)
        {
            Variable<InboxTypes> type = BlackboardUtils.FindVariable<InboxTypes>(inboxList.value[i], "type");
            
            if(type.value == inboxType)
            {
                itemList.Add(inboxList.value[i]);
            }
        }

        return itemList;
    }

    public static void SetUsableInboxItemList(List<Blackboard> itemList)
    {
        if(itemList == null || itemList.Count < 1) return;

        // playerprefs.setvalue(id);
        for(int i=0; i<itemList.Count; ++i)
        {
            Variable<int> itemID = BlackboardUtils.FindVariable<int>(itemList[i], "id");
            
            PlayerPrefs.SetInt( string.Format("USABLE_INBOX_ITEM_{0}", itemID.value), 1);
        }
    }

    public static bool IsUsableInboxItem(int itemID)
    {
        return (PlayerPrefs.GetInt(string.Format("USABLE_INBOX_ITEM_{0}", itemID), 0) > 0) ? false : true;
    }

    public static void UpdateInboxItemList(List<InboxInfo> itemList)
    {
        if (itemList == null || itemList.Count < 1) return;

        BlackboardUtils.DestroyBlackboardList(MainBlackboard.Get(), "inboxList");
        BlackboardUtils.SetOrCreateList(MainBlackboard.Get(), "inboxList", itemList, ClientAPI2Blackboard.Serialize);
        
    }

    public static bool IsInboxBanner(Blackboard item)
    {
        var type = item.GetVariable<InboxBannerTypes>("inboxBannerType");

        if (type != null && type.value != InboxBannerTypes.UNKNOWN)
        {
            return true;
        }

        return false;
    }

    public static void SetWatchedInboxBannerItem(int itemID)
    {
        PlayerPrefs.SetInt(string.Format("WATCHED_INBOX_BANNER_ITEM_{0}", itemID), 1);
    }

    public static bool IsWatchedInboxBannerItem(int itemID)
    {
        return PlayerPrefs.GetInt(string.Format("WATCHED_INBOX_BANNER_ITEM_{0}", itemID), 0) > 0;
    }

    public static void RemoveWatchedInboxItem(List<Blackboard> itemList)
    {
        if (itemList == null || itemList.Count < 1) return;

        for (int i = itemList.Count-1; i >= 0; i--)
        {
            var id = itemList[i].GetVariable<int>("id");
            if (IsWatchedInboxBannerItem(id.value))
            {
                itemList.Remove(itemList[i]);
            }
        }
    }

    public static void UpdateCollectAllCredit()
    {
        var inboxList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "inboxList");
        if(inboxList == null) return;

        long allCredit = 0;
        
        for (int i = 0; i < inboxList.value.Count; i++)
        {
            allCredit += GetCreditFromInbox(inboxList.value[i]);
        }
        
        BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "collectAllCredit", allCredit);
    }

    public static long GetCreditFromInbox(Blackboard inbox)
    {
        var inboxType = inbox.GetValue<InboxTypes>("type");

        if (inboxType == InboxTypes.REWARD)
        {
            var isDelivery = inbox.GetVariable<bool>("isDailyDelivery");
            if(isDelivery == null || isDelivery.value ==  false)
            {
                var reward = inbox.GetVariable<Blackboard>("reward");
                if (reward != null)
                    return GetRewardCoin(reward.value, true);
            }
        }
        else if (inboxType == InboxTypes.GAME_COMPENSATION)
        {
            var credit = inbox.GetVariable<long>("credit");
            if (credit != null)
                return credit.value;
        }
        else if (inboxType == InboxTypes.FACEBOOK_FRIEND_CONNECT)
        {
            var credit = inbox.GetVariable<long>("credit");
            if (credit != null)
                return credit.value;
        }
        else if (inboxType == InboxTypes.FACEBOOK_SHARE)
        {
            var credit = inbox.GetVariable<long>("credit");
            if (credit != null)
                return credit.value;
        }
        else if (inboxType == InboxTypes.TOURNAMENT_WIN)
        {
            var credit = inbox.GetVariable<long>("actualWinCredit");
            if (credit != null)
                return credit.value;       
        }
        else if (inboxType == InboxTypes.SOCIAL_CREDIT)
        {
            var credit = inbox.GetVariable<long>("credit");
            if (credit != null)
                return FreebieLevelUtils.GetLevelMultiplierNumeratorValue(credit.value, FreebieLevelUtils.FreebieType.FRIENDS_DEAL_BONUS);
        }

        return 0;
    }

    public static int GetMaxInboxId()
    {
        var inboxList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "inboxList");

        int maxId = 0;
        for (int i = 0; i < inboxList.value.Count; i++)
        {
            int id = BlackboardUtils.FindValue<int>(inboxList.value[i], "id");
            if (id > maxId)
                maxId = id;
        }

        return maxId;
    }
    // Bucks Gift List
    public static List<Blackboard> GetBucksGiftList()
    {
        Blackboard userBucksInfoBB = GetUserBucksInfoBB();
        if (userBucksInfoBB == null)
            return null;
        return BlackboardUtils.FindVariable<List<Blackboard>>(userBucksInfoBB, "giftList")?.value ?? null;
    }
}

}
