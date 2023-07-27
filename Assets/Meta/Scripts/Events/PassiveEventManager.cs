using System.Collections.Generic;
using ParadoxNotion;
using SlotMaker;
using BagelCode.ClientModels;
using BagelCode.Protobuf;
using UnityEngine;

namespace BagelCode
{

public class PassiveEventManager : SlotMaker.MonoWeakSingleton<PassiveEventManager>
{
    private List<int> activeEventIdList = new List<int>();
    private Dictionary<int, EventInfo>  activeEventDict = new Dictionary<int, EventInfo>();

    private List<int> inactiveEventIdList = new List<int>();
    private Dictionary<int, EventInfo> inactiveEventDict = new Dictionary<int, EventInfo>();
    private long oldestInactiveEventEndTimestamp = 0;

    private Dictionary<string, MessageDispatcher.EventDelegate> delegates = new Dictionary<string, MessageDispatcher.EventDelegate>();

    private long lastIDServerTimestamp = 0;
    private long lastInfoServerTimestamp = 0;

    private const string ON_PASSIVE_EVENT = "OnPassiveEvent";
    private const string START_PASSIVE_EVENT = "StartPassive";
    private const string REFRESH_PASSIVE_EVENT = "RefreshPassive";

    private const string ON_SYSTEM_EVENT = "OnSystemEvent";
    private const string ON_SYSTEM_RESET_EVENT = "SystemReset";

    private void Start()
    {
        delegates[ON_SYSTEM_RESET_EVENT] = OnSystemReset;

        MessageDispatcher.Register(ON_SYSTEM_EVENT, OnSystemEvent);
        BagelCode.Internal.BagelCodeHTTP.commonHandler += CallbackCommon;
    }

    protected override void OnDestroy()
    {
        MessageDispatcher.UnRegister(ON_SYSTEM_EVENT, OnSystemEvent);
        BagelCode.Internal.BagelCodeHTTP.commonHandler -= CallbackCommon;

        base.OnDestroy();
    }

    private void CallbackCommon(IResponse<Error, CommonResponse> response)
    {
        if(response != null)
            UpdateEventCommon(response.common);
    }

    public void Clear()
    {
        activeEventIdList.Clear();
        activeEventDict.Clear();

        inactiveEventIdList.Clear();
        inactiveEventDict.Clear();

        lastIDServerTimestamp = 0;
        lastInfoServerTimestamp = 0;
    }

    private void OnSystemEvent(EventData eventData)
    {
        MessageDispatcher.EventDelegate del;
        if (delegates.TryGetValue(eventData.name, out del))
            del.Invoke(eventData);
    }

    private void OnSystemReset(EventData eventData)
    {
        Clear();
    }

    public void UpdateEventCommon(CommonResponse commonResponse)
    {
        if(commonResponse == null) return;

        UpdateEventIDList(commonResponse.ongoingEventTimestamp, commonResponse.ongoingEventIdList);
    }

    private void UpdateEventIDList(long serverTimestamp, List<int> eventIDList)
    {
        if(serverTimestamp <= lastIDServerTimestamp) return;
        lastIDServerTimestamp = serverTimestamp;

        List<int> removeIDList  = new List<int>();

        // Remove Item Check.
        foreach(KeyValuePair<int, EventInfo> eventInfo in activeEventDict)
        {
            if(eventIDList == null || !eventIDList.Contains(eventInfo.Key))
            {
                removeIDList.Add(eventInfo.Key);
            }
        }

        // Remove Item.
        foreach(int removeID in removeIDList)
        {
            if(activeEventDict.ContainsKey(removeID))
            {
                EventInfo removeEventInfo = activeEventDict[removeID];

                AddInactiveEvent(removeID, removeEventInfo);

                activeEventDict.Remove(removeID);
                activeEventIdList.Remove(removeID);

                if(removeEventInfo != null)
                {
                    // Debug.LogError(string.Format("remove event : {0}", removeEventInfo.id));
                    // RemoveRefresh.
                    var e = new EventData<int>(REFRESH_PASSIVE_EVENT, removeEventInfo.id);
                    MessageDispatcher.Dispatch(ON_PASSIVE_EVENT, e);
                }
            }
        }

        if (oldestInactiveEventEndTimestamp != 0 && inactiveEventIdList.Count > 0)
        {
            while (TimeUtils.GetTimeStamp() - oldestInactiveEventEndTimestamp > TimeUtils.ONE_DAY_MS)
            {
                inactiveEventDict.Remove(inactiveEventIdList[0]);
                inactiveEventIdList.RemoveAt(0);

                if (inactiveEventIdList.Count > 0)
                {
                    oldestInactiveEventEndTimestamp = inactiveEventDict[inactiveEventIdList[0]].endTimestamp;
                }
                else
                {
                    oldestInactiveEventEndTimestamp = 0;
                    break;
                }
            }
        }


        if(eventIDList == null) return;

        // new Type Check..
        List<int> requestEventIDList = new List<int>();

        foreach(int newID in eventIDList)
        {
            if(!activeEventDict.ContainsKey(newID))
            {
                activeEventDict.Add(newID, null);
                requestEventIDList.Add(newID);
            }
            else if(activeEventDict[newID] == null)
            {
                requestEventIDList.Add(newID);
            }
        }

        // Request Item Info..
        if(requestEventIDList.Count > 0)
        {
            BagelCodeClientAPI.PassiveEventInfo( requestEventIDList,
            (response) =>
            {
                PassiveEventManager.Instance.UpdateEventInfoList(response.common.ongoingEventTimestamp, response.ongoingEventList);
            },
            (error) =>
            {
            });
        }
    }

    private void AddInactiveEvent(int id, EventInfo eventInfo)
    {
        if (eventInfo == null || eventInfo.endTimestamp == 0 || inactiveEventDict.ContainsKey(id))
            return;

        inactiveEventDict[id] = eventInfo;

        if (inactiveEventIdList.Count == 0)
        {
            inactiveEventIdList.Add(id);
        }
        else
        {
            // order by timestamp
            bool added = false;
            for (int i = 0; i < inactiveEventIdList.Count; i++)
            {
                if (inactiveEventDict[inactiveEventIdList[i]].endTimestamp >= inactiveEventDict[id].endTimestamp)
                {
                    inactiveEventIdList.Insert(i, id);
                    added = true;
                    break;
                }
            }

            if (!added)
                inactiveEventIdList.Add(id);
        }

        oldestInactiveEventEndTimestamp = inactiveEventDict[inactiveEventIdList[0]].endTimestamp;
    }

    public void UpdateEventInfoList(long serverTimestamp, List<EventInfo> eventInfoList)
    {
        if(eventInfoList == null) return;

        if(serverTimestamp <= lastInfoServerTimestamp) return;
        lastInfoServerTimestamp = serverTimestamp;

        foreach(EventInfo info in eventInfoList)
        {
            UpdateEventInfo(info);
        }
    }

    public void AddEventInfoList(long serverTimestamp, List<EventInfo> eventInfoList)
    {
        if(eventInfoList == null) return;

        if(serverTimestamp <= lastInfoServerTimestamp) return;
        lastInfoServerTimestamp = serverTimestamp;

        foreach(EventInfo info in eventInfoList)
        {
            if(!activeEventDict.ContainsKey(info.id))
            {
                activeEventDict.Add(info.id, null);
            }
            UpdateEventInfo(info);
        }
    }

    private void UpdateEventInfo(EventInfo info)
    {
        if(info == null) return;

        if(activeEventDict.ContainsKey(info.id))
        {
            activeEventIdList.Add(info.id);
            activeEventDict[info.id] = info;

            // Start Event.
            // Debug.LogError(string.Format("Start event : {0}", info.id));
            var e = new EventData<EventInfoType>(START_PASSIVE_EVENT, info.type);
            MessageDispatcher.Dispatch(ON_PASSIVE_EVENT, e);

        }
    }

    public int GetActiveEventID(EventInfoType evtType)
    {
        if(evtType == EventInfoType.UNKNOWN) return 0;

        foreach(KeyValuePair<int, EventInfo> eventInfo in activeEventDict)
        {
            if(eventInfo.Value == null) continue;

            if(eventInfo.Value.type == evtType && IsActive(eventInfo.Value))
            {
                return eventInfo.Key;
            }
        }

        return 0;
    }

    public EventInfo GetActiveEventInfo(EventInfoType evtType, bool allowInactive = false)
    {
        if(evtType == EventInfoType.UNKNOWN) return null;

        foreach(KeyValuePair<int, EventInfo> eventInfo in activeEventDict)
        {
            if(eventInfo.Value == null) continue;

            if(eventInfo.Value.type == evtType && IsActive(eventInfo.Value))
            {
                return eventInfo.Value;
            }
        }

        if (allowInactive)
        {
            foreach(KeyValuePair<int, EventInfo> eventInfo in inactiveEventDict)
            {
                if(eventInfo.Value == null) continue;

                if(eventInfo.Value.type == evtType)
                {
                    return eventInfo.Value;
                }
            }
        }

        return null;
    }

    public List<EventInfo> GetActiveEventInfoList(EventInfoType evtType)
    {
        List<EventInfo> eventInfoList = new List<EventInfo>();

        if(evtType == EventInfoType.UNKNOWN) return eventInfoList;

        foreach(KeyValuePair<int, EventInfo> eventInfo in activeEventDict)
        {
            if(eventInfo.Value == null) continue;

            if(eventInfo.Value.type == evtType && IsActive(eventInfo.Value))
            {
                eventInfoList.Add(eventInfo.Value);
            }
        }

        return eventInfoList;
    }

    public EventInfo GetEventInfoFromID(int eventID)
    {
        foreach(KeyValuePair<int, EventInfo> eventInfo in activeEventDict)
        {
            if(eventInfo.Value == null) continue;

            if(eventInfo.Value.id == eventID)
            {
                return eventInfo.Value;
            }
        }

        return null;
    }

    public EventInfo GetEventInfoFromID(int eventID, bool allowInactive)
    {
        foreach (KeyValuePair<int, EventInfo> eventInfo in activeEventDict)
        {
            if (eventInfo.Value == null) continue;

            if (eventInfo.Value.id == eventID)
            {
                return eventInfo.Value;
            }
        }

        if (allowInactive)
        {
            foreach (KeyValuePair<int, EventInfo> eventInfo in inactiveEventDict)
            {
                if (eventInfo.Value == null) continue;

                if (eventInfo.Value.id == eventID)
                {
                    return eventInfo.Value;
                }
            }
        }

        return null;
    }

    public double GetEventInfoViewMultiplier(EventInfo eventInfo)
    {
        if(eventInfo == null) return 1.0;
        if(IsActive(eventInfo))
        {
            switch(eventInfo.type)
            {
                default:
                    long multiplierNumerator = GetEventInfoMultiplierNumerator(eventInfo);
                    return NumberUtils.GetMultiplierFromNumerator(multiplierNumerator);
            }
        }

        return 1.0;
    }

    public long GetEventInfoViewPercent(EventInfo eventInfo)
    {
        if(eventInfo == null) return 100L;
        if(IsActive(eventInfo))
        {
            switch(eventInfo.type)
            {
                default:
                    long multiplierNumerator = GetEventInfoMultiplierNumerator(eventInfo);
                    return NumberUtils.GetPercentFromNumerator(multiplierNumerator);
            }
        }

        return 100L;
    }

    public long GetEventInfoViewAddPercent(EventInfo eventInfo)
    {
        if(eventInfo == null) return 100L;
        if(IsActive(eventInfo))
        {
            switch(eventInfo.type)
            {
                default:
                    long multiplierNumerator = GetEventInfoMultiplierNumerator(eventInfo);
                    return NumberUtils.GetAdditionalPercent(multiplierNumerator);
            }
        }

        return 100L;
    }

    public long GetEventInfoMultiplierNumerator(EventInfo eventInfo)
    {
        if(eventInfo == null) return NumberUtils.GetGlobalDenominator();
        if(IsActive(eventInfo))
        {
            switch(eventInfo.type)
            {
                case EventInfoType.TIER_UP_SHOP_EVENT_MULTIPLY:
                    {
                        EventDataTierUpShopMultiply constraintsInfo = eventInfo.constraints as EventDataTierUpShopMultiply;
                        if (constraintsInfo != null)
                        {
                            return constraintsInfo.multiplierNumerator;
                        }
                    }
                    break;
                case EventInfoType.EXP_MULTIPLY:
                    {
                        EventDataExpMultiply constraintsInfo = eventInfo.constraints as EventDataExpMultiply;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.multiplierNumerator;
                        }
                    }
                    break;
                case EventInfoType.EXP_MULTIPLY_EXTENDABLE:
                    {
                        EventDataExpMultiplyExtendable constraintsInfo = eventInfo.constraints as EventDataExpMultiplyExtendable;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.multiplierNumerator;
                        }
                    }
                    break;
                case EventInfoType.TIME_BONUS:
                    {
                        EventDataTimeBonus constraintsInfo = eventInfo.constraints as EventDataTimeBonus;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.multiplierNumerator;
                        }
                    }
                    break;
                case EventInfoType.CREDIT_MULTIPLIER_WHEEL:
                    {
                        EventDataCreditMultiplierWheel constraintsInfo = eventInfo.constraints as EventDataCreditMultiplierWheel;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.multiplierNumerator;
                        }
                    }
                    break;
                // case EventInfoType.PAYER_RP_MULTIPLY:
                //     break;
                case EventInfoType.DAILY_WHEEL_MULTIPLY:
                    {
                        EventDataDailyWheelMultiply constraintsInfo = eventInfo.constraints as EventDataDailyWheelMultiply;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.multiplierNumerator;
                        }
                    }
                    break;
                case EventInfoType.PIGGY_BANK_MULTIPLY:
                    {
                        EventDataPiggyBankMultiply constraintsInfo = eventInfo.constraints as EventDataPiggyBankMultiply;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.multiplierNumerator;
                        }
                    }
                    break;
                // case EventInfoType.PIGGY_BANK_SALE:
                //     break;
                case EventInfoType.CHALLENGE_CLAIM_MULTIPLY:
                    {
                        EventDataChallengeClaimMultiply constraintsInfo = eventInfo.constraints as EventDataChallengeClaimMultiply;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.multiplierNumerator;
                        }
                    }
                    break;
                // case EventInfoType.FREE_COIN_BOOSTER:
                //     break;
                // case EventInfoType.LUCKY_FIVE:
                //     break;
                // case EventInfoType.TIME_BONUS_COOLTIME:
                //     break;
                // case EventInfoType.LP_BOOST:
                //     break;
                case EventInfoType.CREDIT_MULTIPLIER_WHEEL_MULTIPLY:
                    {
                        EventDataCreditMultiplierWheelMultiply constraintsInfo = eventInfo.constraints as EventDataCreditMultiplierWheelMultiply;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.multiplierNumerator;
                        }
                    }
                    break;
                // case EventInfoType.COLLECTING_GAME:
                //     break;
                case EventInfoType.POG_BOOSTER_MULTIPLY:
                    {
                        EventDataPogBoosterMultiply constraintsInfo = eventInfo.constraints as EventDataPogBoosterMultiply;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.multiplierNumerator;
                        }
                    }
                    break;
                case EventInfoType.POG_BOOSTER_MIN_MULTIPLIER:
                    {
                        EventDataPogBoosterMinMultiplier constraintsInfo = eventInfo.constraints as EventDataPogBoosterMinMultiplier;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.multiplierNumerator;
                        }
                    }
                    break;
                case EventInfoType.BONUS_SALE:
                    {
                        EventDataBonusSale constraintsInfo = eventInfo.constraints as EventDataBonusSale;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.saleNumerator;
                        }
                    }
                    break;
                case EventInfoType.BONUS_MULTIPLY:
                {
                    EventDataBonusMultiply constraintsInfo = eventInfo.constraints as EventDataBonusMultiply;
                    if(constraintsInfo != null)
                    {
                        return constraintsInfo.multiplierNumerator;
                    }
                }
                    break;
                // case EventInfoType.POG_BOOSTER_FREE:
                //     break;
                // case EventInfoType.POG_BOOSTER_WITHOUT:
                //     break;
                // case EventInfoType.ALL_GAME_OPEN:
                //     break;
                // case EventInfoType.CRAZY_CLUB_CHALLENGE:
                //     break;
                // case EventInfoType.CRAZY_CHALLENGE:
                //     break;
                case EventInfoType.COIN_SHOP_EVENT_MULTIPLY:
                case EventInfoType.GEM_SHOP_EVENT_MULTIPLY:
                    {
                        EventDataShopMultiplyPassiveEvent constraintsInfo = eventInfo.constraints as EventDataShopMultiplyPassiveEvent;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.multiplierNumerator;
                        }
                    }
                    break;
                // case EventInfoType.BONUS_SALE:
                //     break;
                // case EventInfoType.BONUS_MULTIPLY:
                //     break;
                case EventInfoType.GEM_BAB_SHOP_EVENT_MULTIPLY:
                    {
                        EventDataShopMultiplyPassiveEvent constraintsInfo = eventInfo.constraints as EventDataShopMultiplyPassiveEvent;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.multiplierNumerator;
                        }
                    }
                    break;
                case EventInfoType.GEM_BOOSTER:
                    {
                        EventDataGemBooster constraintsInfo = eventInfo.constraints as EventDataGemBooster;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.multiplierNumerator;
                        }
                    }
                    break;
                // case EventInfoType.FREE_GEM_BOOSTER:
                case EventInfoType.GEM_BOOSTER_MULTIPLY:
                    {
                        EventDataGemBoosterMultiply constraintsInfo = eventInfo.constraints as EventDataGemBoosterMultiply;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.multiplierNumerator;
                        }
                    }
                    break;
                case EventInfoType.COIN_SHOP_DISCOUNT_EVENT_MULTIPLY:
                    {
                        EventDataDiscountMultiply constraintsInfo = eventInfo.constraints as EventDataDiscountMultiply;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.multiplierNumerator;
                        }
                    }
                    break;
                case EventInfoType.GEM_SHOP_DISCOUNT_EVENT_MULTIPLY:
                    {
                        EventDataDiscountMultiply constraintsInfo = eventInfo.constraints as EventDataDiscountMultiply;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.multiplierNumerator;
                        }
                    }
                    break;
                case EventInfoType.POG_DISCOUNT_EVENT_MULTIPLY:
                    {
                        EventDataDiscountMultiply constraintsInfo = eventInfo.constraints as EventDataDiscountMultiply;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.multiplierNumerator;
                        }
                    }
                    break;
                case EventInfoType.GEM_BAB_SHOP_DISCOUNT_EVENT_MULTIPLY:
                    {
                        EventDataDiscountMultiply constraintsInfo = eventInfo.constraints as EventDataDiscountMultiply;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.multiplierNumerator;
                        }
                    }
                    break;
                case EventInfoType.GEM_BAB_PROMOTION_SHOP_EVENT_MULTIPLY:
                    {
                        EventDataShopMultiplyPassiveEvent constraintsInfo = eventInfo.constraints as EventDataShopMultiplyPassiveEvent;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.multiplierNumerator;
                        }
                    }
                    break;
                case EventInfoType.GEM_BAB_PROMOTION_SHOP_DISCOUNT_EVENT_MULTIPLY:
                    {
                        EventDataDiscountMultiply constraintsInfo = eventInfo.constraints as EventDataDiscountMultiply;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.multiplierNumerator;
                        }
                    }
                    break;
                case EventInfoType.COLLECTING_GAME_CHEST_DROP_RATE_MULTIPLY:
                    {
                        EventDataDropRateMultiply constraintsInfo = eventInfo.constraints as EventDataDropRateMultiply;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.multiplierNumerator;
                        }
                    }
                    break;
                case EventInfoType.VOUCHER_SHOP_EVENT_MULTIPLY:
                    {
                        EventDataVoucherShopMultiply constraintsInfo = eventInfo.constraints as EventDataVoucherShopMultiply;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.multiplierNumerator;
                        }
                    }
                    break;
                case EventInfoType.GEM_JACKPOT_REWARD_MULTIPLY:
                    {
                        EventDataJackpotRewardMultiply constraintsInfo = eventInfo.constraints as EventDataJackpotRewardMultiply;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.multiplierNumerator;
                        }
                    }
                    break;
                case EventInfoType.GEM_JACKPOT_SPIN_GEM_SALE:
                    {
                        EventDataJackpotSpinGemSale constraintsInfo = eventInfo.constraints as EventDataJackpotSpinGemSale;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.saleNumerator;
                        }
                    }
                    break;
                case EventInfoType.DAILY_WHEEL_MULTIPLY_FREEBIE:
                    {
                        EventDataDailyWheelMultiplyFreebie constraintsInfo = eventInfo.constraints as EventDataDailyWheelMultiplyFreebie;
                        if (constraintsInfo != null)
                        {
                            return constraintsInfo.multiplierNumerator;
                        }
                    }
                    break;
                default:
#if DEV
                    Debug.LogError(string.Format("Add Passive Event Type. {0}", eventInfo.type));
#endif
                    break;
            }
        }

        return NumberUtils.GetGlobalDenominator();
    }

    public EventInfo GetBonusEventInfo(int gameId)
    {
        List<EventInfo> bonusSaleEventInfoList = GetActiveEventInfoList(EventInfoType.BONUS_SALE);

        if (bonusSaleEventInfoList != null && bonusSaleEventInfoList.Count > 0)
        {
            for (int i = 0; i < bonusSaleEventInfoList.Count; i++)
            {
                if (gameId == ((EventDataBonusSale) bonusSaleEventInfoList[i].constraints).gameId)
                    return bonusSaleEventInfoList[i];
            }
        }

        List<EventInfo> bonusMultiplyEventInfoList = GetActiveEventInfoList(EventInfoType.BONUS_MULTIPLY);

        if (bonusMultiplyEventInfoList != null && bonusMultiplyEventInfoList.Count > 0)
        {
            for (int i = 0; i < bonusMultiplyEventInfoList.Count; i++)
            {
                if (gameId == ((EventDataBonusMultiply) bonusMultiplyEventInfoList[i].constraints).gameId)
                    return bonusMultiplyEventInfoList[i];
            }
        }

        return null;
    }

    public int GetEventInfoIndex(EventInfo eventInfo)
    {
        if(eventInfo == null) return 0;
        if(IsActive(eventInfo))
        {
            switch(eventInfo.type)
            {
                case EventInfoType.PIGGY_BANK_SALE:
                    {
                        EventDataPiggyBankSale constraintsInfo = eventInfo.constraints as EventDataPiggyBankSale;
                        if(constraintsInfo != null)
                        {
                            return (int)constraintsInfo.productIndex;
                        }
                    }
                    break;
                 case EventInfoType.POG_BOOSTER_WITHOUT:
                    {
                        EventDataPogBoosterWithout constraintsInfo = eventInfo.constraints as EventDataPogBoosterWithout;
                        if(constraintsInfo != null)
                        {
                            return constraintsInfo.minMultiplierIndex;
                        }
                    }
                    break;
            }
        }

        return 0;
    }

    public bool IsActive(EventInfo eventInfo)
    {
        if(eventInfo == null) return false;
        if(eventInfo.endTimestamp == 0) return true;
        if(eventInfo.endTimestamp > TimeUtils.GetTimeStamp()) return true;

        return false;
    }
}

}
