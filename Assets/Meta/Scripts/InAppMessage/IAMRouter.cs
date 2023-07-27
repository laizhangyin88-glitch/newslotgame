using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{

public class IAMRouter : MonoWeakSingleton<IAMRouter>
{
    public Blackboard bb;

    protected Dictionary<InAppMessageTriggerType, List<Blackboard>> inAppMessageInfoDict;

    private const string PARENT_PATH = "Popup Manager/Area";
    private const string IAM_CALLBACK = "OnIAMCallback";
    private const string IAM_PURCHASE_COUNT_KEY = "IAM_PURCHASE_COUNT_{0}";

    private void Awake()
    {
        inAppMessageInfoDict = new Dictionary<InAppMessageTriggerType, List<Blackboard>>();
    }

    protected override void OnDestroy()
    {
        inAppMessageInfoDict.Clear();

        base.OnDestroy();
    }

    public void UpdateIAMInfo()
    {
        inAppMessageInfoDict.Clear();

        List<Blackboard> inAppMessageList = BlackboardUtils.GetOrCreateBlackboardList(MainBlackboard.Get(), "inAppMessageList");

        for (int i = 0; i < inAppMessageList.Count; ++i)
        {
            var triggerV2List = BlackboardUtils.GetOrCreateBlackboardList(inAppMessageList[i], "triggerV2List");

            for (int j = 0; j < triggerV2List.Count; ++j)
            {
                var type = triggerV2List[j].GetValue<InAppMessageTriggerType>("type");
                if (!inAppMessageInfoDict.ContainsKey(type))
                    inAppMessageInfoDict.Add(type, new List<Blackboard>());

                inAppMessageInfoDict[type].Add(inAppMessageList[i]);
            }
        }
    }

    public void SortTrigger(InAppMessageTriggerType targetTrigger, int gameID)
    {
        if( !inAppMessageInfoDict.ContainsKey(targetTrigger) ) return;

        var targetSortingList = inAppMessageInfoDict[targetTrigger];

        if(targetSortingList.Count < 2) return;

        targetSortingList.Sort(
            (source, dest) =>
            {
                int sourceGameID = source.GetValue<int>("promotedGameId");
                int destGameID = dest.GetValue<int>("promotedGameId");

                if(sourceGameID == gameID && destGameID != gameID)
                    return -1;
                else if(sourceGameID != gameID && destGameID == gameID)
                    return 1;
                // else if(sourceGameID == 0 && destGameID != 0)
                //     return -1;
                // else if(sourceGameID != 0 && destGameID == 0)
                //     return 1;

                var sourcePriority = source.GetValue<int>("priority");
                var destPriority = dest.GetValue<int>("priority");

                if(sourcePriority == destPriority) return 0;

                return sourcePriority > destPriority ? -1 : 1;
            }
        );

        // Logs
        // for(int i=0; i < inAppMessageInfoDict[targetTrigger].Count; ++i)
        // {
        //     Debug.LogError(
        //         string.Format("{0}({1}) : {2}"
        //             , inAppMessageInfoDict[targetTrigger][i].GetValue<int>("id")
        //             , inAppMessageInfoDict[targetTrigger][i].GetValue<int>("priority")
        //             , inAppMessageInfoDict[targetTrigger][i].GetValue<int>("promotedGameId")
        //         )
        //    );
        // }
    }

    public bool TriggerIAMByID(InAppMessageTriggerType triggerType, int iamId, GameObject caller, string contextID, bool usingCoolTime = false, bool usingExposureCount = false, bool refresh = false)
    {
        var inAppMessageList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "inAppMessageList");

        if (inAppMessageList != null && inAppMessageList.value != null)
        {
            for (int i = 0; i < inAppMessageList.value.Count; ++i)
            {
                int id = inAppMessageList.value[i].GetValue<int>("id");
                if(iamId == id)
                {
                    if(IsValidIAMbyID(inAppMessageList.value[i], usingCoolTime, usingExposureCount))
                    {
                        if (!refresh)
                        {
                            ShowIAM(inAppMessageList.value[i], caller, contextID, triggerType);
                        }
                        else
                        {
                            RefreshIAM(inAppMessageList.value[i], contextID, triggerType);
                        }
                        if (usingCoolTime)
                        {
                            SetCoolTime(inAppMessageList.value[i]);
                        }

                        if (usingExposureCount)
                        {
                            SetExposureCount(inAppMessageList.value[i]);
                        }

                        SetTriggerType(triggerType);
                        return true;
                    }
                }
            }
        }

       return false;
    }

    public bool TriggerIAM(InAppMessageTriggerType triggerType, GameObject caller, string contextID)
    {
        if (inAppMessageInfoDict.ContainsKey(triggerType))
        {
            for (int i = 0; i < inAppMessageInfoDict[triggerType].Count; ++i)
            {
                if (IsValidIAM(inAppMessageInfoDict[triggerType][i]) && IsVaildImages(inAppMessageInfoDict[triggerType][i]) && CheckTriggerData(inAppMessageInfoDict[triggerType][i], triggerType))
                {
                    ShowIAM(inAppMessageInfoDict[triggerType][i], caller, contextID, triggerType);
                    SetCoolTime(inAppMessageInfoDict[triggerType][i]);
                    SetExposureCount(inAppMessageInfoDict[triggerType][i]);
                    SetTriggerType(triggerType);
                    return true;
                }
            }
        }
        return false;
    }

    public bool CheckTriggerIAM(InAppMessageTriggerType triggerType)
    {
        if (inAppMessageInfoDict.ContainsKey(triggerType))
        {
            for (int i = 0; i < inAppMessageInfoDict[triggerType].Count; ++i)
            {
                if (IsValidIAM(inAppMessageInfoDict[triggerType][i]))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public Blackboard GetValidTriggerIAMInfo(InAppMessageTriggerType triggerType)
    {
        if (inAppMessageInfoDict.ContainsKey(triggerType))
        {
            for (int i = 0; i < inAppMessageInfoDict[triggerType].Count; ++i)
            {
                if (IsValidIAM(inAppMessageInfoDict[triggerType][i]))
                {
                    return inAppMessageInfoDict[triggerType][i];
                }
            }
        }

        return null;
    }

    private void SetCoolTime(Blackboard iamInfo)
    {
        string coolTimePrefsName = string.Format("IAMCoolTime:{0}", iamInfo.GetValue<int>("id"));
        PlayerPrefs.SetString(coolTimePrefsName, System.Convert.ToString(TimeUtils.GetCurrentTime()));
    }

    private bool IsCoolTime(Blackboard iamInfo)
    {
        int coolTime = iamInfo.GetVariable<int>("cooltimeSec").value;
        string coolTimePrefsName = string.Format("IAMCoolTime:{0}", iamInfo.GetValue<int>("id"));
        string lastTriggeredTime = PlayerPrefs.GetString(coolTimePrefsName, "0");
        if (lastTriggeredTime != "0")
        {
            if (TimeUtils.GetCurrentTime() - System.Convert.ToInt64(lastTriggeredTime) < coolTime)
            {
                return true;
            }
        }
        return false;
    }

    private void SetExposureCount(Blackboard iamInfo)
    {
        int maxExposureCount = iamInfo.GetVariable<int>("maxExposureCount").value;
        string exposureCountPrefsName = string.Format("IAMExposureCount:{0}", iamInfo.GetValue<int>("id"));
        if (maxExposureCount > 0)
        {
            PlayerPrefs.SetInt(exposureCountPrefsName, PlayerPrefs.GetInt(exposureCountPrefsName, 0) + 1);
        }
    }

    private bool IsOverExposureCount(Blackboard iamInfo)
    {
        int maxExposureCount = iamInfo.GetVariable<int>("maxExposureCount").value;
        string exposureCountPrefsName = string.Format("IAMExposureCount:{0}", iamInfo.GetValue<int>("id"));
        int currentExposureCount = PlayerPrefs.GetInt(exposureCountPrefsName, 0);
        if (maxExposureCount > 0 && currentExposureCount >= maxExposureCount)
        {
            return true;
        }

        return false;
    }

    private bool IsValidIAMDefault(Blackboard iamInfo, bool isTriggerById = false)
    {
        InAppMessageType messageType = iamInfo.GetValue<InAppMessageType>("type");

        switch(messageType)
        {
            case InAppMessageType.COIN_PURCHASE_POPUP:
                {
                    int iamID = BlackboardUtils.FindVariable<int>(iamInfo, "id").value;
                    int maxPurchaseCount = BlackboardUtils.FindVariable<int>(iamInfo, "maxPurchaseCount").value;

                    if(maxPurchaseCount > 0)
                    {
                        string saveCountKey = string.Format(IAM_PURCHASE_COUNT_KEY, iamID);
                        int iamPurchaseCount = PlayerPrefs.GetInt(saveCountKey, 0);

                        if(iamPurchaseCount >= maxPurchaseCount)
                        {
                            return false;
                        }
                    }

                    if (!isTriggerById)
                    {
                        var productInfoDict = BlackboardUtils.FindVariable<Blackboard>(iamInfo, "productInfoDict");
                        if (productInfoDict != null && productInfoDict.value.variables.Count > 0)
                        {
                            List<string> keys = productInfoDict.value.variables.Keys.ToList();

                            int invalidCount = 0;
                            foreach (string key in keys)
                            {
                                var productInfo = (Blackboard)productInfoDict.value.variables[key].value;
                                int purchaseLimit = productInfo.GetVariable<int>("purchaseLimit").value;
                                int purchasedCount = productInfo.GetVariable<int>("purchasedCount").value;

                                if (purchaseLimit > 0 && purchasedCount >= purchaseLimit)
                                {
                                    invalidCount++;
                                }
                            }

                            if (invalidCount == keys.Count)
                            {
                                return false;
                            }
                        }
                    }
                }
                break;
            case InAppMessageType.DAILY_BOOST_PURCHASE_POPUP:
                {
                    // Check DailyBoost
                    var dailyBoost = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "dailyBoost");
                    if(dailyBoost != null && dailyBoost.value.GetValue<bool>("isTerminated") != true)
                    {
                        if (isTriggerById)
                        {
                            bool stringError = false;
                            ErrorPopupInfo info = new ErrorPopupInfo();

                            info.type = ErrorPopupType.OK;
                            info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_DAILY_BOOST_ALREADY_EXIST", out stringError);
                            info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);

                            ErrorPopupHandler.Instance.OpenError(info);
                        }

                        return false;
                    }
                }
                break;
            case InAppMessageType.EARLY_ACCESS_PURCHASE_POPUP:
                {
                    if(BlackboardQueryUtils.IsEarlyAccessAvailable())
                    {
                        if (isTriggerById)
                        {
                            bool stringError = false;
                            ErrorPopupInfo info = new ErrorPopupInfo();

                            info.type = ErrorPopupType.OK;
                            info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_EARLY_ACCESS_ALREADY_EXIST", out stringError);
                            info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);

                            ErrorPopupHandler.Instance.OpenError(info);
                        }

                        return false;
                    }
                }
                break;
            case InAppMessageType.VIDEO_ADS_POPUP:
                {
                    var placement = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "videoAdsPlacementNames/challenge");

                    if(placement == null)
                        return false;
                    else
                        return VideoAdsController.Instance.IsVideoAdsAvailable(placement.value);
                }
                break;
            case InAppMessageType.EPIC_PASS_PURCHASE_POPUP:
                {
                    var eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.SEASON_PASS);

                    if(eventInfo != null)
                    {
#if USE_ASSETBUNDLE
                        // Check Asset.
                        var bundleName = BlackboardQueryUtils.GetMetaBundleName(eventInfo);
                        var bundle = AssetBundleManager.GetLoadedAssetBundle(bundleName);
                        if(bundle != null)
                            return true;
#else
                        return true;
#endif
                    }

                }
                break;
            case InAppMessageType.IDFA_POPUP:
                {
#if UNITY_IOS
                    if (IDFAHelper.Instance.IsEligibleToSeeIDFAConsentPopup() && !IDFAHelper.Instance.HasSeenIDFAConsentPopup())
                        return true;

                    return false;
#else
                    return false;
#endif
                }
                break;
            case InAppMessageType.TERMS_OF_USE_POPUP:
                {
                    var termsOfUseType = BlackboardUtils.FindVariable<TermsOfUseType>(MainBlackboard.Get(), "termsOfUseType");
                    if(termsOfUseType != null && termsOfUseType.value == TermsOfUseType.OLD_USER)
                        return true;
                    else
                        return false;
                }
                break;
            case InAppMessageType.PIN_TO_TASKBAR_POPUP:
                {
                    if( NativeHelper.Instance.IsPinningAllowed()
                        && false == BlackboardUtils.GetOrCreateVariable<bool>(MainBlackboard.Get(), "isWindowsPinned", true).value)
                        return true;
                    else
                        return false;
                }
                break;
            case InAppMessageType.INVISIBLE_ACTION_TRIGGER:
                {
                    var action = iamInfo.GetValue<Blackboard>("action");
                    if(action == null) return false;

                    ActionType actionType = action.GetValue<ActionType>("type");

                    if(ApplicationSettings.LogTest())
                        Debug.LogError( string.Format("action type = {0}", actionType));

                    switch(actionType)
                    {
                        case ActionType.PIN_TO_TASKBAR:
                            {
                                if( NativeHelper.Instance.IsPinningAllowed()
                                    && false == BlackboardUtils.GetOrCreateVariable<bool>(MainBlackboard.Get(), "isWindowsPinned", true).value)
                                    return true;
                                else
                                    return false;
                            }
                            break;
                        default:
                            return true;
                    }
                }
                break;
        }

        return true;
    }

    private bool IsValidIAM(Blackboard iamInfo)
    {
        // Check Playing Tutorial
        var playingTutorial = BlackboardUtils.GetOrCreateVariable<bool>(MainBlackboard.Get(), "/IsPlayingTutorial");
        if (playingTutorial != null && playingTutorial.value == true)
        {
            return false;
        }

        // Check CoolTime // Using Inhouse AD.
        if (IsCoolTime(iamInfo)) return false;

        // Check ExposureCount
        if (IsOverExposureCount(iamInfo)) return false;

        return IsValidIAMDefault(iamInfo);
    }

    public bool IsValidIAMasDeal(Blackboard iamInfo)
    {
        // Check Playing Tutorial
        var playingTutorial = BlackboardUtils.GetOrCreateVariable<bool>(MainBlackboard.Get(), "/IsPlayingTutorial");
        if (playingTutorial != null && playingTutorial.value == true)
        {
            return false;
        }

        // Check ExposureCount
        if (IsOverExposureCount(iamInfo)) return false;

        return IsValidIAMDefault(iamInfo);
    }

    private bool IsValidIAMbyID(Blackboard iamInfo, bool usingCoolTime = false, bool usingExposureCount = false)
    {
        // Check CoolTime
        if (usingCoolTime && IsCoolTime(iamInfo)) return false;

        // Check ExposureCount
        if (usingExposureCount && IsOverExposureCount(iamInfo)) return false;

        return IsValidIAMDefault(iamInfo, true);
    }

    private bool IsVaildImages(Blackboard iamInfo)
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        var componentList = BlackboardUtils.FindVariable<List<Blackboard>>(iamInfo, "componentList");

        if (componentList != null && componentList.value.Count > 0)
        {
            for (int i = 0; i < componentList.value.Count; ++i)
            {
                InAppMessageComponentType type = componentList.value[i].GetValue<InAppMessageComponentType>("type");

                if (type == InAppMessageComponentType.BACKGROUND_WEB_IMAGE)
                {
                    string imageUrl = componentList.value[i].GetValue<string>("imageUrl");

                    if(!WebImageDownloader.Instance.CheckCachedImage(imageUrl, CacheType.FileCache))
                    {
                        WebImageDownloader.Instance.LoadWebImage(
                            imageUrl,
                            CacheType.FileCache,
                            true,
                            null,
                            null,
                            null,
                            null
                        );

                        return false;
                    }
                }
            }
        }
#endif
        return true;
    }

    private bool CheckTriggerData(Blackboard iamInfo, InAppMessageTriggerType triggerType)
    {
        switch (triggerType)
        {
            case InAppMessageTriggerType.TIER_UP:
                return IAMTriggerCheck.TierCheck(iamInfo);

            default:
                return true;
        }
    }

    private bool CheckRestriction(Blackboard restriction)
    {
        if (restriction == null) return false;

        var userProperty = restriction.GetVariable<string>("userProperty");
        var operation = restriction.GetVariable<string>("operation");
        var toolValue = restriction.GetVariable<string>("value");


        if (userProperty == null || userProperty.value == null
            || operation == null || operation.value == null
            || toolValue == null || toolValue.value == null)
        {
            return false;
        }

        var meProperty = BlackboardUtils.FindVariable(MainBlackboard.Get(), "/me/"+userProperty.value);
        if (meProperty == null || meProperty.value == null) return false;

        if (meProperty.varType == typeof(int))
        {
            return FormatUtility.CompareOperator<int>(operation.value, Convert.ToInt32(meProperty.value), Convert.ToInt32(toolValue.value));
        }
        else if (meProperty.varType == typeof(long))
        {
            return FormatUtility.CompareOperator<long>(operation.value, Convert.ToInt64(meProperty.value), Convert.ToInt64(toolValue.value));
        }
        else if (meProperty.varType == typeof(double))
        {
            return FormatUtility.CompareOperator<double>(operation.value, Convert.ToDouble(meProperty.value), Convert.ToDouble(toolValue.value));
        }
        else if (meProperty.varType == typeof(string))
        {
            return FormatUtility.CompareOperator<string>(operation.value, Convert.ToString(meProperty.value), Convert.ToString(toolValue.value));
        }
        else if (meProperty.varType == typeof(bool))
        {
            return FormatUtility.CompareOperator<bool>(operation.value, Convert.ToBoolean(meProperty.value), Convert.ToBoolean(toolValue.value));
        }

        return false;
    }

    private void SetTriggerType(InAppMessageTriggerType triggerType)
    {
        var TriggerType = bb.GetVariable<InAppMessageTriggerType>("triggerType");
        TriggerType.value = triggerType;
    }

    private void ShowIAM(Blackboard iamInfo, GameObject caller, string contextID, InAppMessageTriggerType triggerType)
    {
        if(iamInfo == null) return;

        var iamType = iamInfo.GetValue<InAppMessageType>("type");
        if(iamType == InAppMessageType.INVISIBLE_ACTION_TRIGGER)
        {
            ShowActionIAM(iamInfo, caller, contextID, triggerType);
        }
        else
        {
            ShowDefaultIAM(iamInfo, caller, contextID, triggerType);
        }
    }

    private void ShowDefaultIAM(Blackboard iamInfo, GameObject caller, string contextID, InAppMessageTriggerType triggerType)
    {
        if(iamInfo == null) return;

        var lastTriggeredTime = BlackboardUtils.GetOrCreateVariable<long>(iamInfo, "lastTriggeredTime");
        lastTriggeredTime.SetValue(TimeUtils.GetCurrentTime());

        // LoadIAM.cs ////////////////////

        // Check Endtime
        long endTimestamp = IAMUtils.GetEndTimestamp(iamInfo);
        if (endTimestamp == -1L)
            return;

        // Make Deal Button
        if (endTimestamp > 0L)
            IAMUtils.MakeDeal(iamInfo);
        ///////////////////

        Transform parentTransform = GameObject.Find(PARENT_PATH).transform;
        GameObject iamObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "IAM Base", parentTransform);
        iamObject.name = "IAM Base";

        var iamController = iamObject.GetComponent<InAppMessage.InAppMessageBase>();
        iamController.LoadIAM(MetaStringDefine.LOBBY_BUNDLE_NAME, iamInfo, endTimestamp);
        ////////////////////////////////////////

        // Set information ////////////////////
        Blackboard iamBB = iamObject.GetComponent<Blackboard>();

        BlackboardUtils.SetOrCreateValue<GameObject>(iamBB, "iamCaller", gameObject);
        BlackboardUtils.SetOrCreateValue<Blackboard>(iamBB, "_iamInfo", iamInfo);
        BlackboardUtils.SetOrCreateValue<string>(iamBB, "_biContextID", contextID);
        BlackboardUtils.SetOrCreateValue<InAppMessageTriggerType>(iamBB, "triggerType", triggerType);
        ////////////////////////////////////////

        // open IAM
        PopupManager.Instance.Open(iamObject);
        BlackboardUtils.SetOrCreateValue<GameObject>(bb, "_iam", iamObject);

        MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData("RefreshDeal"));

        BlackboardUtils.SetOrCreateValue<GameObject>(bb, "caller", caller);
    }

    private void RefreshIAM(Blackboard iamInfo, string contextID, InAppMessageTriggerType triggerType)
    {
        var lastTriggeredTime = BlackboardUtils.GetOrCreateVariable<long>(iamInfo, "lastTriggeredTime");
        lastTriggeredTime.SetValue(TimeUtils.GetCurrentTime());

        // RefreshIAM.cs ////////////////////
        var iamObject = bb.GetVariable<GameObject>("_iam");
        if (iamObject == null)
            return;

        long endTimestamp = IAMUtils.GetEndTimestamp(iamInfo);
        if(endTimestamp == -1L)
            return;

        if (endTimestamp > 0L)
            IAMUtils.MakeDeal(iamInfo);

        IAMUtils.ClearComponents(iamObject.value);

        var iamController = iamObject.value.GetComponent<InAppMessage.InAppMessageBase>();
        iamController.LoadIAM(MetaStringDefine.LOBBY_BUNDLE_NAME, iamInfo, endTimestamp);
        ////////////////////////////////////////

        // Set information ////////////////////
        Blackboard iamBB = iamObject.value.GetComponent<Blackboard>();

        BlackboardUtils.SetOrCreateValue<Blackboard>(iamBB, "_iamInfo", iamInfo);
        BlackboardUtils.SetOrCreateValue<string>(iamBB, "_biContextID", contextID);
        BlackboardUtils.SetOrCreateValue<InAppMessageTriggerType>(iamBB, "triggerType", triggerType);
        ////////////////////////////////////////

        // GraphOwner agent = iamObject.value.GetComponent<GraphOwner>();
        // agent.SendEvent("OnRefreshCallback");
    }

    private void ShowActionIAM(Blackboard iamInfo, GameObject caller, string contextID, InAppMessageTriggerType triggerType)
    {
        if(iamInfo == null) return;

        var lastTriggeredTime = BlackboardUtils.GetOrCreateVariable<long>(iamInfo, "lastTriggeredTime");
        lastTriggeredTime.SetValue(TimeUtils.GetCurrentTime());

        // LoadIAM.cs ////////////////////

        // Check Endtime
        long endTimestamp = IAMUtils.GetEndTimestamp(iamInfo);
        if (endTimestamp == -1L)
            return;

        Transform parentTransform = GameObject.Find(PARENT_PATH).transform;
        GameObject iamObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "IAM Action Base", parentTransform);
        iamObject.name = "IAM Action Base";

        // var iamController = iamObject.GetComponent<InAppMessage.InAppMessagePopup>();
        // iamController.LoadIAM(MetaStringDefine.LOBBY_BUNDLE_NAME, iamInfo, endTimestamp);
        ////////////////////////////////////////

        // Set information ////////////////////
        Blackboard iamBB = iamObject.GetComponent<Blackboard>();

        BlackboardUtils.SetOrCreateValue<GameObject>(iamBB, "iamCaller", gameObject);
        BlackboardUtils.SetOrCreateValue<Blackboard>(iamBB, "_iamInfo", iamInfo);
        BlackboardUtils.SetOrCreateValue<Blackboard>(iamBB, "_iamComponentBB", iamInfo);
        BlackboardUtils.SetOrCreateValue<string>(iamBB, "_biContextID", contextID);
        BlackboardUtils.SetOrCreateValue<InAppMessageTriggerType>(iamBB, "triggerType", triggerType);
        ////////////////////////////////////////

        // open IAM
        PopupManager.Instance.Open(iamObject);
        BlackboardUtils.SetOrCreateValue<GameObject>(bb, "_iam", iamObject);

        // MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData("RefreshDeal"));

        BlackboardUtils.SetOrCreateValue<GameObject>(bb, "caller", caller);
    }

    // From BT
    public void OnCalleeCallback(GameObject caller)
    {
        if(caller != null)
            EventSender.SendEvent(caller, EventSender.ON_CUSTOM_EVENT, new EventData(IAM_CALLBACK));
    }
}

}
