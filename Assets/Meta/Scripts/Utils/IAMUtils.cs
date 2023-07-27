using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BagelCode.ClientModels;
using BagelCode.InAppMessage;
using NodeCanvas.Framework;
using UnityEngine;
using SlotMaker;
using System.Collections;
using System.Linq;

namespace BagelCode
{
    public static class IAMUtils
    {
        public const string ON_IAM_CALLBACK_EVENT = "OnIAMCallback";

        private static string IAM_KEY = "IAMTimer:{0}";
        private static string IAM_DEAL_KEY = "IAMDeal_ID";
        private static string IAM_PURCHASE_COUNT_KEY = "IAM_PURCHASE_COUNT_{0}";

        private static long gemValueForCoin = 0;
        private static long gemValueForCointDiscountNumerator = 0;

        public static System.Action onVideoAdsCallback = null;

        private const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        public static GameObject GetIamCaller()
        {
            var iamManagerBB = IAMRouter.Instance.GetComponent<Blackboard>();
            return iamManagerBB.GetVariable<GameObject>("caller")?.value;
        }

        public static IEnumerator OpenPopupCoroutine(MonoBehaviour owner, ActionOpenPopupType type)
        {
            if (owner == null) yield break;

            string contextId = BlackboardUtils.FindVariable<string>("_biContextID")?.value;

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset;
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            switch (type)
            {
                case ActionOpenPopupType.SHOP:
                case ActionOpenPopupType.GEM_SHOP:
                    {
                        asset = "Shop Scene";
                        GameObject popupObj = null;
                        yield return owner.StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, parent,
                            (GameObject obj) => popupObj = obj));

                        var popupBB = popupObj.GetComponent<Blackboard>();

                        if (type == ActionOpenPopupType.GEM_SHOP)
                            BlackboardUtils.SetOrCreateValue(popupBB, "initTabIndex", 1);

                        BlackboardUtils.SetOrCreateValue(popupBB, "_biContextID", contextId);
                        BlackboardUtils.SetOrCreateValue(popupBB, "_isAvailableBucks", true);
                        MetaObjectUtils.SetCalleeCaller(popupObj, owner.gameObject);
                        MetaPopupUtils.OpenPopup(popupObj);
                    }
                    break;
                case ActionOpenPopupType.VIP_CLUB:
                    {
                        string email = PlayerPrefs.GetString("VALIDATE_EMAIL", "");
                        if (string.IsNullOrEmpty(email))
                        {
                            asset = "Popup Account Join Scene";

                            // Generate VIP funnel info
                            var funnelType = Tasks.Actions.BI.BI_client_vip_club_funnel.BIVIPClubFunnelType.OPEN_POPUP;
                            PlayerPrefs.SetInt("VIP_CLUB_FUNNEL_TYPE", (int)funnelType);
                            PlayerPrefs.SetString("VIP_CLUB_FUNNEL_CONTEXT_ID", BiEventUtils.GenerateContextID());
                        }
                        else
                        {
                            asset = "Popup Account Email Join Code Scene";
                        }
                        GameObject popupObj = null;
                        yield return owner.StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, parent,
                            (GameObject obj) => popupObj = obj));

                        MetaObjectUtils.SetCalleeCaller(popupObj, owner.gameObject);
                        MetaPopupUtils.OpenPopup(popupObj);
                    }
                    break;
                case ActionOpenPopupType.PIGGY_BANK:
                    {
                        yield return owner.StartCoroutine(OpenPopupPiggyBankCoroutine(owner, contextId));
                    }
                    break;
                case ActionOpenPopupType.STATUS_MATCH:
                    {
                        asset = "Popup Status Match Application Form Scene";
                        GameObject popupObj = null;
                        yield return owner.StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, parent,
                            (GameObject obj) => popupObj = obj));

                        MetaObjectUtils.SetCalleeCaller(popupObj, owner.gameObject);
                        MetaPopupUtils.OpenPopup(popupObj);
                    }
                    break;
                case ActionOpenPopupType.VIP_REWARDS:
                    {
                        asset = "VIP Rewards Scene";
                        GameObject popupObj = null;
                        yield return owner.StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, parent,
                            (GameObject obj) => popupObj = obj));

                        MetaPopupUtils.OpenPopup(popupObj);

                        var vipDealInfo = BlackboardQueryUtils.GetActiveVipDealInfo();
                        if(vipDealInfo != null)
                        {
                            int vipDealId = BlackboardUtils.FindVariable<int>(vipDealInfo, "vipDealInfoId")?.value ?? 0;
                            PlayerPrefs.SetInt("VIP_DEAL_INFO_ID", vipDealId);
                        }
                    }
                    break;
                case ActionOpenPopupType.CHALLENGE:
                case ActionOpenPopupType.PERSONAL_EVENT_CHALLENGE:
                case ActionOpenPopupType.CLUB_CHALLENGE:
                case ActionOpenPopupType.CLUB_EVENT_CHALLENGE:
                    {
                        bool isLockedFeature = BlackboardQueryUtils.IsLockedFeature(LockedFeatureType.CHALLENGE);
                        if (!isLockedFeature)
                        {
                            asset = "Popup Challenge Scene";
                            GameObject popupObj = null;
                            yield return owner.StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, parent,
                                (GameObject obj) => popupObj = obj));

                            var popupBB = popupObj.GetComponent<Blackboard>();
                            MetaChallengeType challengeTabType = MetaChallengeType.NONE;
                            if (type == ActionOpenPopupType.CLUB_CHALLENGE)
                                challengeTabType = MetaChallengeType.CLUB;
                            else if (type == ActionOpenPopupType.PERSONAL_EVENT_CHALLENGE)
                                challengeTabType = MetaChallengeType.EVENT_PERSONAL;
                            else if (type == ActionOpenPopupType.CLUB_EVENT_CHALLENGE)
                                challengeTabType = MetaChallengeType.EVENT_CLUB;
                            BlackboardUtils.SetOrCreateValue(popupBB, "forceSelectTabType", challengeTabType);

                            MetaObjectUtils.SetCalleeCaller(popupObj, owner.gameObject);
                            MetaPopupUtils.OpenPopup(popupObj);
                        }
                    }
                    break;
                case ActionOpenPopupType.SETTINGS:
                    {
                        asset = "Popup Settings Scene";
                        GameObject popupObj = null;
                        yield return owner.StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, parent,
                            (GameObject obj) => popupObj = obj));

                        MetaObjectUtils.SetCalleeCaller(popupObj, owner.gameObject);
                        MetaPopupUtils.OpenPopup(popupObj);
                    }
                    break;
                case ActionOpenPopupType.LEADER_PUSH:
                    {
                        asset = "Popup Captain's Call Push Scene";
                        GameObject popupObj = null;
                        yield return owner.StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, parent,
                            (GameObject obj) => popupObj = obj));

                        var popupBB = popupObj.GetComponent<Blackboard>();
                        BlackboardUtils.SetOrCreateValue(popupBB, "fromIam", true);
                        MetaObjectUtils.SetCalleeCaller(popupObj, owner.gameObject);
                        MetaPopupUtils.OpenPopup(popupObj);
                    }
                    break;
                case ActionOpenPopupType.INVITE_INSTALL:
                    {
                        string platformName = ApplicationSettings.GetPlatformName().ToUpper();
                        bool isValidPlatform =
                            platformName == MetaStringDefine.PLATFORM_ANDROID ||
                            platformName == MetaStringDefine.PLATFORM_IOS;

                        if (isValidPlatform)
                        {
                            asset = "Popup Invite Link Need More Coin Scene";
                            GameObject popupObj = null;
                            yield return owner.StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, parent,
                                (GameObject obj) => popupObj = obj));

                            var popupBB = popupObj.GetComponent<Blackboard>();
                            BlackboardUtils.SetOrCreateValue(popupBB, "_biContextID", contextId);
                            MetaObjectUtils.SetCalleeCaller(popupObj, owner.gameObject);
                            MetaPopupUtils.OpenPopup(popupObj);
                        }
                    }
                    break;
                case ActionOpenPopupType.DEAL_IAM_OR_SHOP:
                    {
                        AEUtils.SendAE("client_click_button",
                            ("button_name", "level_up_dash_get_iam"),  // todo level dash 외 요인으로 호출 시 수정
                            ("context_id", contextId));

                        Blackboard iamRouterBB = IAMRouter.Instance.bb;
                        var caller = BlackboardUtils.FindVariable<GameObject>(iamRouterBB, "caller")?.value;
                        bool triggered = MetaPopupUtils.OpenDealIam(caller,
                            InAppMessageTriggerType.ANY_PURCHASE_CLICK_GET_BUTTON,
                            false);

                        if (!triggered)
                        {
                            // Open Shop
                            MetaPopupUtils.OpenShop(caller, contextId);
                        }
                    }
                    break;
                case ActionOpenPopupType.VIP_EMAIL_CONNECT:
                    {
                        var funnelType = Tasks.Actions.BI.BI_client_vip_club_funnel.BIVIPClubFunnelType.JOIN_VIP_VIA_ACTION_POPUP;
                        PlayerPrefs.SetInt("VIP_CLUB_FUNNEL_TYPE", (int)funnelType);
                        PlayerPrefs.SetString("VIP_CLUB_FUNNEL_CONTEXT_ID", BiEventUtils.GenerateContextID());

                        asset = "Popup Account Email Join Scene";

                        GameObject popupObj = null;
                        yield return owner.StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, parent,
                            (GameObject obj) => popupObj = obj));

                        MetaObjectUtils.SetCalleeCaller(popupObj, owner.gameObject);
                        MetaPopupUtils.OpenPopup(popupObj);
                    }
                    break;
            }
        }

        private static IEnumerator OpenPopupPiggyBankCoroutine(MonoBehaviour owner, string contextId)
        {
            long piggyCredit = BlackboardUtils.FindVariable<long>("/me/piggyCredit")?.value ?? 0L;
            int tier = BlackboardUtils.FindVariable<int>("/me/tier")?.value ?? 0;
            long tierMultiplierNumerator = TierUtils.GetTableValue(tier, TierUtils.GetTierMultiplierNumeratorTable());
            double tierMultiplier = NumberUtils.GetMultiplierFromNumerator(tierMultiplierNumerator);

            EventInfo pogEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.PIGGY_BANK_SALE);
            Blackboard pogProductBB = null;
            if (pogEventInfo != null)
            {
                int productIndex = PassiveEventManager.Instance.GetEventInfoIndex(pogEventInfo);
                pogProductBB = BlackboardQueryUtils.GetPotOfGoldProduct(productIndex);
            }
            else
            {
                pogProductBB = BlackboardQueryUtils.GetPotOfGoldProduct(0);
            }

            var itemInfoBB = BlackboardQueryUtils.GetItemFromProduct(pogProductBB, ItemType.PIGGY_BANK);
            long minCredit = BlackboardUtils.FindVariable<long>(itemInfoBB, "minCredit")?.value ?? 0L;
            long maxCredit = BlackboardUtils.FindVariable<long>(itemInfoBB, "maxCredit")?.value ?? 0L;

            // Calc piggy level
            long resultPiggyCredit = NumberUtils.GetMultiplierNumeratorValue(piggyCredit, tierMultiplierNumerator);

            long proportionNumerator = resultPiggyCredit -= minCredit;
            long range = maxCredit - minCredit;
            proportionNumerator = NumberUtils.GetDevideNumeratorValue(resultPiggyCredit, range);

            List<double> thresholdList = BlackboardUtils.FindVariable<List<double>>("/values/misc/PIGGY_BANK_INFO_APPEAR_SECTION_LIST")?.value ?? new List<double>();
            double proportion = NumberUtils.GetMultiplierFromNumerator(proportionNumerator);

            int piggyLevel = thresholdList.Count(t => proportion >= t);

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Pot Of Gold Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;
            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);

            var popupBB = popupObj.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(popupBB, "_biContextID", contextId);
            BlackboardUtils.SetOrCreateValue(popupBB, "_piggyCredit", resultPiggyCredit);
            BlackboardUtils.SetOrCreateValue(popupBB, "_proportion", proportion);
            BlackboardUtils.SetOrCreateValue(popupBB, "_tier", tier);
            BlackboardUtils.SetOrCreateValue(popupBB, "_multiplier", tierMultiplier);
            BlackboardUtils.SetOrCreateValue(popupBB, "_level", piggyLevel);
            MetaObjectUtils.SetCalleeCaller(popupObj, owner.gameObject);
            MetaPopupUtils.OpenPopup(popupObj);

            GSManager.Instance.GetHandler("UI_Potofgold");

            yield break;
        }

        public static List<GameObject> MakeComponents(InAppMessageBase owner)
        {
            if (owner == null || owner.iamInfo == null) return new List<GameObject>();

            List<Blackboard> componentList = BlackboardUtils.FindVariable<List<Blackboard>>(owner.iamInfo, "componentList").value;
            InAppMessageType iamType = BlackboardUtils.FindVariable<InAppMessageType>(owner.iamInfo, "type").value;

            if (componentList == null || componentList.Count == 0) return new List<GameObject>();

            GameObject iamObj = owner.gameObject;
            Blackboard iamBB = owner.GetComponent<Blackboard>();

            bool interactable = !owner.isPreview;

            List<GameObject> componentObjList = new List<GameObject>();

            for (int i = 0; i < componentList.Count; ++i)
            {
                Blackboard componentInfoBB = componentList[i];
                GameObject componentObj = null;

                InAppMessageComponentType componentType = componentInfoBB.GetValue<InAppMessageComponentType>("type");
                switch (componentType)
                {
                    case InAppMessageComponentType.BACKGROUND_WEB_IMAGE:
                        {
                            string imageUrl = componentInfoBB.GetValue<string>("imageUrl");

                            if (!string.IsNullOrEmpty(imageUrl))
                            {
                                BlackboardUtils.SetOrCreateValue<string>(iamBB, "_biBackgroundURL", imageUrl);
                            }

                            if (WebImageDownloader.Instance.CheckCachedImage(imageUrl, CacheType.FileCache))
                            {
                                string assetName = "IAM Background Image";
                                componentObj = MakeComponentObject(assetName, owner, componentInfoBB, owner.parent, true);

                                IContextImage imageElement = componentObj.GetComponent<IContextImage>();
                                if (imageElement != null)
                                {
                                    WebImageDownloader.Instance.LoadWebImage(
                                        imageUrl,
                                        CacheType.FileCache,
                                        true,
                                        null,
                                        (Sprite img) => imageElement.SetSprite(img));
                                }
                            }
                            else
                            {
                                MakeLoadingObject(owner);

                                string assetName = "IAM Background Image With Downloading";
                                componentObj = MakeComponentObject(assetName, owner, componentInfoBB, owner.parent, true);

                                Blackboard componentBB = componentObj.GetComponent<Blackboard>();
                                componentBB.SetValue("imageURL", imageUrl);
                                componentBB.SetValue("loadingObj", owner.loadingObj);
                            }
                        }
                        break;
                    case InAppMessageComponentType.TEXT:
                        {
                            string resultText = componentInfoBB.GetValue<string>("text");
                            bool showOutline = componentInfoBB.GetValue<bool>("showOutline");

                            string assetName = showOutline ? "IAM Central Outline Text" : "IAM Central Text";
                            componentObj = MakeComponentObject(assetName, owner, componentInfoBB);
                            componentObj.name = "IAM Central Text " + i.ToString();

                            resultText = InterpolatedTextUtils.ParseCommonText(resultText);

                            var product = BlackboardUtils.FindVariable<Blackboard>(componentInfoBB, "product");
                            if (product != null)
                            {
                                resultText = ParseIAMText(resultText, product.value, owner.iamInfo);
                                // resultText = ParseIAMTextOfProduct(resultText, product.value);
                                // resultText = ParseIAMTextByIAMType(resultText, product.value, owner.iamInfo);
                            }

                            ContextTextMeshProUGUI textElement = componentObj.GetComponent<ContextTextMeshProUGUI>();

                            textElement.SetText(resultText);
                        }
                        break;
                    case InAppMessageComponentType.TIMER:
                        {
                            // preview timer always displayed
                            if (interactable && owner.endTimestamp <= 0L) continue;

                            string assetName = "IAM Timer";
                            componentObj = MakeComponentObject(assetName, owner, componentInfoBB);

                            componentObj.GetComponent<Blackboard>().SetValue("endTimestamp", owner.endTimestamp);

                            Transform timerAreaTransform = componentObj.transform.Find("Timer Area");
                            GameObject timerObj = MetaObjectUtils.MakePrefab(owner.bundleName, "Remaining Timer", timerAreaTransform);

                            MetaObjectUtils.MakePrefab(owner.bundleName, "Text", timerObj.transform);
                        }
                        break;
                    case InAppMessageComponentType.SLOT_THUMBNAIL:
                        {
                            int gameId = componentInfoBB.GetValue<int>("gameId");
                            if (gameId == 0) continue;

                            string assetName = "IAM Thumbnail";
                            componentObj = MakeComponentObject(assetName, owner, componentInfoBB);

                            Blackboard gameInfo = BlackboardQueryUtils.GetGameInfo(gameId);
                            string gameTitle = gameInfo.GetValue<string>("gameTitle");
                            string thumbnailString = StringTableUtils.GetString(GLOBAL, "SLOT_THUMBNAIL_NORMAL", gameTitle);

                            LoadThumbnailPrefabSync(thumbnailString, componentObj.transform);
                        }
                        break;
                    case InAppMessageComponentType.SCROLL_TEXT_BOX:
                        {
                            string text = componentInfoBB.GetValue<string>("text");

                            string assetName = "IAM Scroll View Text";
                            componentObj = MakeComponentObject(assetName, owner, componentInfoBB, owner.scrollAnchor, true);

                            ContextCompositor componentRoot = componentObj.GetComponent<ContextCompositor>();
                            componentRoot.UpdateContext(true);
                            ContextTextMeshProUGUI textContext = (ContextTextMeshProUGUI)componentRoot.Find("Text");

                            textContext.SetText(text);
                        }
                        break;
                    case InAppMessageComponentType.CLICK_ACTION:
                        {
                            string assetName = iamType == InAppMessageType.SURVEY_POPUP ? "IAM Pressable Button" : "IAM Button";
                            componentObj = MakeComponentObject(assetName, owner, componentInfoBB);

                            if (interactable)
                                AddListenerOnButton(componentObj, owner.gameObject, componentInfoBB, iamType);
                        }
                        break;
                    case InAppMessageComponentType.BUY_BUTTON:
                        {
                            if (!interactable ||
                                IsValidBuyButton(iamType, owner.iamInfo, componentInfoBB))
                            {
                                string assetName = "IAM Buy Button";
                                componentObj = MakeComponentObject(assetName, owner, componentInfoBB);

                                if (interactable)
                                    AddListenerOnButton(componentObj, owner.gameObject, componentInfoBB, iamType);

                                double productPrice = 0.0;

                                var product = BlackboardUtils.FindVariable<Blackboard>(componentInfoBB, "product")?.value;
                                if (product != null)
                                    productPrice = BlackboardUtils.FindVariable<double>(product, "price")?.value ?? 0.0;

                                string priceText = StringTableUtils.GetString(StringTable.StringTableType.Global, "IAM_BUY_BUTTON", productPrice);
                                componentObj.transform.GetChild(0).GetChild(1).GetComponent<ContextTextMeshProUGUI>().SetText(priceText);
                            }
                        }
                        break;
                    case InAppMessageComponentType.BUTTON:
                        {
                            bool useAnimation = componentInfoBB.GetValue<bool>("useAnimation");
                            string assetName = useAnimation ? "IAM Button Green Animation" : "IAM Button Green";
                            componentObj = MakeComponentObject(assetName, owner, componentInfoBB);

                            if (interactable)
                                AddListenerOnButton(componentObj, owner.gameObject, componentInfoBB, iamType);

                            ContextElement buttonElement = componentObj.GetComponent<ContextElement>();
                            buttonElement.UpdateContext(true);

                            // Parse Button Text
                            string resultText = componentInfoBB.GetValue<string>("text");
                            resultText = InterpolatedTextUtils.ParseCommonText(resultText);

                            var product = BlackboardUtils.FindVariable<Blackboard>(componentInfoBB, "action/product");
                            if (product != null)
                            {
                                resultText = ParseIAMText(resultText, product.value, owner.iamInfo);
                                // resultText = ParseIAMTextOfProduct(resultText, product.value);
                                // resultText = ParseIAMTextByIAMType(resultText, product.value, owner.iamInfo);
                            }

                            MetaContextElementUtils.SimpleSetText(buttonElement, "Text", resultText);

                            // Parse Tag Text
                            bool isTagged = componentInfoBB.GetValue<bool>("isTagged");
                            MetaContextElementUtils.SimpleSetActive(buttonElement, "Tag", isTagged);
                            if (isTagged)
                            {
                                string tagText = componentInfoBB.GetValue<string>("tag");
                                tagText = InterpolatedTextUtils.ParseCommonText(tagText);
                                if (product != null)
                                {
                                    resultText = ParseIAMText(resultText, product.value, owner.iamInfo);
                                }

                                MetaContextElementUtils.SimpleSetText(buttonElement, "Tag/Text", tagText, ContextSearchingType.FullNameSearch);
                            }
                        }
                        break;
                    case InAppMessageComponentType.TIER_ICON:
                        {
                            string assetName = "IAM Tier Icon";
                            componentObj = MakeComponentObject(assetName, owner, componentInfoBB);

                            Blackboard tierIconBB = componentObj.GetComponent<Blackboard>();
                            tierIconBB.SetValue("isRefresh", true);
                            tierIconBB.SetValue("multiplierType", TierMultiplierTableType.CoinMultiplier);
                        }
                        break;
                    case InAppMessageComponentType.SINGLE_BONUS_BUY_BUTTON:
                        {
                            string assetName = "IAM Bonus";
                            componentObj = MakeComponentObject(assetName, owner, componentInfoBB);

                            var action = componentInfoBB.GetValue<Blackboard>("action");
                            ProductUtils.MakeBonusProduct(action, "super_bonus");

                            componentObj.GetComponent<IAMBonusController>().Init(owner.gameObject, componentInfoBB, false, interactable);
                        }
                        break;
                    case InAppMessageComponentType.SINGLE_RANGE_BONUS_BUY_BUTTON:
                        {
                            string assetName = "IAM Bonus Auto Balance";
                            componentObj = MakeComponentObject(assetName, owner, componentInfoBB);

                            componentObj.GetComponent<IAMBonusAutoBalanceController>().Init(owner.gameObject, componentInfoBB, owner.iamInfo, interactable);
                            break;
                        }
                    case InAppMessageComponentType.RANGE_BONUS_BUY_BUTTON:
                        {
                            string assetName = "IAM Bonus Range";
                            componentObj = MakeComponentObject(assetName, owner, componentInfoBB);

                            componentObj.GetComponent<IAMBonusRangeController>().Init(owner.gameObject, componentInfoBB, interactable);
                        }
                        break;
                    case InAppMessageComponentType.BONUS_EVENT_BADGE:
                        {
                            int gameId = owner.iamInfo.GetValue<int>("gameId");
                            EventInfo bonusEventInfo = PassiveEventManager.Instance.GetBonusEventInfo(gameId);
                            if (bonusEventInfo != null)
                            {
                                bool isBonus = bonusEventInfo.type == EventInfoType.BONUS_SALE;
                                string assetName = isBonus ? "IAM Badge Sale" : "IAM Badge Multiply";
                                string key = isBonus ? "IAM_BADGE_SALE_TEXT" : "IAM_BADGE_MULTIPLY_TEXT";

                                componentObj = MakeComponentObject(assetName, owner, componentInfoBB);
                                ContextElement textElement = componentObj.transform.GetChild(0).GetComponent<ContextElement>();

                                long saleNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(bonusEventInfo);
                                string text = StringTableUtils.GetString(GLOBAL, key, saleNumerator);
                                MetaContextElementUtils.SetText(textElement, text);
                            }
                        }
                        break;
                    case InAppMessageComponentType.LEVEL_MULTIPLIER_TEXT:
                        {
                            bool showOutline = componentInfoBB.GetValue<bool>("showOutline");
                            string assetName = showOutline ? "IAM Central Outline Text" : "IAM Central Text";
                            componentObj = MakeComponentObject(assetName, owner, componentInfoBB);

                            bool showLevelMultiplierText = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "showLevelMultiplierText")?.value ?? false;
                            string resultText = showLevelMultiplierText ? componentInfoBB.GetValue<string>("showLevelMultiplierOnText") : componentInfoBB.GetValue<string>("showLevelMultiplierOffText");
                            resultText = InterpolatedTextUtils.ParseCommonText(resultText);

                            var product = BlackboardUtils.FindVariable<Blackboard>(componentInfoBB, "product");
                            if (product != null)
                            {
                                resultText = ParseIAMText(resultText, product.value, owner.iamInfo);
                            }

                            componentObj.name = "IAM Central Text " + i.ToString();

                            // use meta context element utils instead?
                            ContextTextMeshProUGUI textElement = componentObj.GetComponent<ContextTextMeshProUGUI>();
                            textElement.SetText(resultText);
                        }
                        break;
                    case InAppMessageComponentType.LEVEL_UP_DASH_MISSION_LEFT_TIME:
                        {
                            long levelDashEndTimestamp = LevelUpDash.LevelUpDash.Utils.EndTimestamp;
                            if (interactable && levelDashEndTimestamp <= 0L)
                                continue;

                            componentObj = MakeComponentObject("IAM Timer", owner, componentInfoBB);

                            Transform timerAreaTransform = componentObj.transform.Find("Timer Area");
                            GameObject timer = MetaObjectUtils.MakePrefab(owner.bundleName, "Remaining Timer", timerAreaTransform);

                            MetaObjectUtils.MakePrefab(owner.bundleName, "Text", timer.transform);

                            componentObj.GetComponent<Blackboard>().SetValue("endTimestamp", levelDashEndTimestamp);
                        }
                        break;
                    case InAppMessageComponentType.RED_RIBBON_TAG:
                        {
                            int size = componentInfoBB.GetValue<int>("tagSize");

                            string asset = (size == 0) ? "IAM Tag Label Small" :
                                (size == 1) ? "IAM Tag Label Normal" : "IAM Tag Label Big";

                            componentObj = MakeComponentObject(asset, owner, componentInfoBB);

                            ContextElement componentElement = componentObj.GetComponent<ContextElement>();
                            componentElement.UpdateContext(true);

                            string resultText = componentInfoBB.GetValue<string>("text");
                            resultText = InterpolatedTextUtils.ParseCommonText(resultText);

                            var product = BlackboardUtils.FindVariable<Blackboard>(componentInfoBB, "product");
                            if (product != null)
                            {
                                resultText = ParseIAMText(resultText, product.value, owner.iamInfo);
                            }

                            MetaContextElementUtils.SimpleSetText(componentElement, "Text", resultText);
                        }
                        break;
                    default:
                        {
                            Debug.LogError("IAMUtils.MakeComponent error. " + componentType + " is undefined <InAppMessageComponentType>.");
                        }
                        break;
                }

                if (componentObj != null)
                    componentObjList.Add(componentObj);
            }

            return componentObjList;
        }

        private static void MakeLoadingObject(InAppMessageBase owner)
        {
            if(owner.loadingObj == null)
                owner.loadingObj = MetaObjectUtils.MakePrefab(owner.bundleName, "IAM Loading Full", owner.loadingArea);
        }

        private static GameObject MakeComponentObject(string assetName, InAppMessageBase owner, Blackboard componentInfoBB, Transform parent = null, bool lockTransform = false)
        {
            if (parent == null) parent = owner.parent;
            var componentObj = MetaObjectUtils.MakePrefab(owner.bundleName, assetName, parent);

            if (!lockTransform && componentObj != null)
            {
                ParsePosition(componentObj.transform, componentInfoBB);
                ParseRect(componentObj.transform, componentInfoBB);
                ParseRotation(componentObj.transform, componentInfoBB);
            }

            return componentObj;
        }

        public static string GetBundleName(bool combineApplicationType, string bundleName)
        {
            return combineApplicationType ? ApplicationSettings.MakeApplicationBundleName(bundleName) : bundleName;
        }

        public static long GetEndTimestamp(Blackboard iamInfoBB)
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

            if (endTimestamp == 0L)
            {
                return 0L;
            }
            else if (endTimestamp > 0 && currentTimestamp < endTimestamp)
            {
                if (IAMtype == InAppMessageType.COIN_PURCHASE_POPUP)
                {
                    int maxPurchaseCount = BlackboardUtils.FindVariable<int>(iamInfoBB, "maxPurchaseCount").value;

                    if (maxPurchaseCount > 0)
                    {
                        string saveCountKey = string.Format(IAM_PURCHASE_COUNT_KEY, id);
                        int iamPurchaseCount = PlayerPrefs.GetInt(saveCountKey, 0);

                        if (iamPurchaseCount >= maxPurchaseCount)
                        {
                            return -1L;
                        }
                    }
                }

                return endTimestamp;
            }
            else
            {
                if (IAMtype == InAppMessageType.INSTANT_BONUS_PURCHASE_POPUP || IAMtype == InAppMessageType.BUY_A_BONUS_PURCHASE_POPUP || IAMtype == InAppMessageType.SUPER_BONUS_PURCHASE_POPUP)
                    return 0L;
            }

            return -1L;
        }

        public static void MakeDeal(Blackboard iamInfoBB)
        {
            bool useDeal = BlackboardUtils.FindVariable<bool>(iamInfoBB, "useDeal").value;
            string id = BlackboardUtils.FindVariable<int>(iamInfoBB, "id").value.ToString();

            if (useDeal)
            {
                PlayerPrefs.SetString(IAM_DEAL_KEY, id);
            }
            else
            {
                string playerPrefsID = PlayerPrefs.GetString(IAM_DEAL_KEY, "0");
                if (playerPrefsID.Equals(id))
                {
                    PlayerPrefs.SetString(IAM_DEAL_KEY, "0");
                }

                var iamInfo = BlackboardQueryUtils.GetIAMBlackboard(Int32.Parse(playerPrefsID));
                if (iamInfo != null)
                {
                    var nextSequentialIamId = iamInfo.GetVariable<int>("nextSequentialIamId");
                    if (nextSequentialIamId != null)
                    {
                        if (nextSequentialIamId.value == Int32.Parse(id))
                        {
                            PlayerPrefs.SetString(IAM_DEAL_KEY, "0");
                        }
                    }
                }
            }
        }

        private static void LoadThumbnailPrefabSync(string assetName, Transform parent)
        {
            var prefab = AssetBundleManager.LoadAsset<GameObject>("slotthumb1", assetName);
            GameObject go = GameObject.Instantiate(prefab) as GameObject;

            if (parent != null)
                go.transform.SetParent(parent, false);
        }

        private static string ParseIAMText(string text, Blackboard product, Blackboard iamInfo)
        {
            text = InterpolatedTextUtils.ParseTextOfProduct(text, product);
            text = ParseIAMTextByIAMType(text, product, iamInfo);
            text = InterpolatedTextUtils.ParseEconomyMultiplier(text);

            text = InterpolatedTextUtils.ParseCustomText(text);

            return text;
        }

        private static string ParseIAMTextByIAMType(string text, Blackboard product, Blackboard iamInfo)
        {
            if (product == null) return text;
            if (iamInfo == null) return text;

            InAppMessageType type = BlackboardUtils.FindVariable<InAppMessageType>(iamInfo, "type").value;

            switch (type)
            {
                case InAppMessageType.COIN_PURCHASE_POPUP:
                    {
                        if (text.Contains("quantity_counter"))
                        {
                            int id = product.GetValue<int>("id");
                            Blackboard productInfoDict = iamInfo.GetVariable<Blackboard>("productInfoDict").value;
                            Blackboard productInfo = productInfoDict.GetVariable<Blackboard>(id.ToString()).value;
                            int purchaseLimit = productInfo.GetVariable<int>("purchaseLimit").value;
                            int purchasedCount = productInfo.GetVariable<int>("purchasedCount").value;

                            string quantityCounter = StringTableUtils.GetString(StringTable.StringTableType.Global, "IAM_QUANTITY_COUNTER", purchaseLimit - purchasedCount < 0 ? 0 : purchaseLimit - purchasedCount, purchaseLimit);
                            text = Regex.Replace(text, "\\{quantity_counter\\}", quantityCounter, RegexOptions.None);
                        }
                    }
                    break;
                default:
                    break;
            }

            return text;
        }

        private static string ParseItemPatternText(string text, string pattern, Blackboard product, Blackboard item, long value)
        {
            if (Regex.IsMatch(text, pattern))
            {
                string patternString = Regex.Match(text, pattern).Value;
                patternString = patternString.Replace("{", "").Replace("}", "");
                string[] formats = patternString.Split('|');

                bool isSimpleFormat = false;

                for (int i = 1; i < formats.Length; ++i)
                {
                    if ("simple" == formats[i].ToLower())
                        isSimpleFormat = true;
                    else
                        value = ApplyItemFormat(value, formats[i], product, item);
                }

                string formattedText = isSimpleFormat ? FormatUtility.SimpleNumberFormat(value) : FormatUtility.CommaNumberFormat(value);
                text = Regex.Replace(text, pattern, formattedText, RegexOptions.None);
            }

            return text;
        }

        private static string ParseRewardPatternText(string text, string pattern, Blackboard product, Blackboard reward, long value)
        {
            if (Regex.IsMatch(text, pattern))
            {
                string patternString = Regex.Match(text, pattern).Value;
                patternString = patternString.Replace("{", "").Replace("}", "");
                string[] formats = patternString.Split('|');

                bool isSimpleFormat = false;

                for (int i = 1; i < formats.Length; ++i)
                {
                    if ("simple" == formats[i].ToLower())
                        isSimpleFormat = true;
                    else
                        value = ApplyRewardFormat(value, formats[i], product, reward);
                }

                string formattedText = isSimpleFormat ? FormatUtility.SimpleNumberFormat(value) : FormatUtility.CommaNumberFormat(value);
                text = Regex.Replace(text, pattern, formattedText, RegexOptions.None);
            }

            return text;
        }

        private static long ApplyItemFormat(long value, string formatType, Blackboard product, Blackboard item)
        {
            switch (formatType)
            {
                case "event":
                    {
                        if (product.GetVariable<long>("eventMultiplierNumerator") != null)
                        {
                            long eventMultiplierNumerator = product.GetValue<long>("eventMultiplierNumerator");
                            value = NumberUtils.GetMultiplierNumeratorValue(value, eventMultiplierNumerator);
                        }
                    }
                    break;
                case "tier":
                    {
                        value = TierUtils.GetTierFractionCoin(value, TierUtils.GetMeTier());
                    }
                    break;
                case "additional":
                    {
                        if (item.GetVariable<long>("additionalCreditMultiplierNumerator") != null)
                        {
                            long additionalCreditMultiplierNumerator = item.GetValue<long>("additionalCreditMultiplierNumerator");
                            value = NumberUtils.GetAdditionalMultiplierNumeratorValue(value, additionalCreditMultiplierNumerator);
                        }
                    }
                    break;
                case "day":
                    {
                        if (item.GetVariable<int>("totalDayCount") != null)
                        {
                            int totalDayCount = item.GetValue<int>("totalDayCount");
                            value = value * (long)totalDayCount;
                        }
                    }
                    break;
            }

            return value;
        }

        private static long ApplyRewardFormat(long value, string formatType, Blackboard product, Blackboard rewardInfo)
        {
            if (rewardInfo == null) return value;

            // TODO : Want you new type format? try try..
            // switch (formatType)
            // {
            //     case "a":
            //         {
            //         }
            //         break;
            //     case "b":
            //         {
            //         }
            //         break;
            // }

            return value;
        }

        private static void ParsePosition(Transform componentTrasform, Blackboard componentBB)
        {
            if (componentTrasform == null || componentBB == null) return;

            var posXVar = BlackboardUtils.FindVariable<int>(componentBB, "posX");
            var posYVar = BlackboardUtils.FindVariable<int>(componentBB, "posY");
            if (posXVar == null || posYVar == null) return;

            RectTransform componentRectTransform = componentTrasform.GetComponent<RectTransform>();
            if (componentRectTransform != null)
            {
                componentRectTransform.anchorMin = new Vector2(0, 1f);
                componentRectTransform.anchorMax = new Vector2(0, 1f);

                componentRectTransform.anchoredPosition = new Vector2(posXVar.value, -posYVar.value);
            }
        }

        private static void ParseRotation(Transform componentTrasform, Blackboard componentBB)
        {
            if (componentTrasform == null || componentBB == null) return;
            
            var rotVar = BlackboardUtils.FindVariable<int>(componentBB, "rotation");
            if (rotVar == null) return;

            componentTrasform.rotation = Quaternion.Euler(0, 0, (float)rotVar.value);
        }

        private static void ParseRect(Transform buttonTransform, Blackboard rectBB)
        {
            if (buttonTransform == null || rectBB == null) return;

            var widthVar = BlackboardUtils.FindVariable<int>(rectBB, "width");
            var heightVar = BlackboardUtils.FindVariable<int>(rectBB, "height");
            if (widthVar == null || heightVar == null) return;

            RectTransform buttonRectTransform = buttonTransform.GetComponent<RectTransform>();
            if(buttonRectTransform != null)
                buttonRectTransform.sizeDelta = new Vector2(widthVar.value, heightVar.value);
        }

        public static void ClearComponents(GameObject IAMObject)
        {
            int anchorCount = IAMObject.transform.childCount;
            for (int i = 0; i < anchorCount; i++)
            {
                Transform anchor = IAMObject.transform.GetChild(i);
                int childCount = anchor.childCount;

                for (int j = childCount - 1; j >= 0; j--)
                {
                    GameObject.Destroy(anchor.GetChild(j).gameObject);
                }
            }
        }

        public static void MakeCloseButton(string bundleName, GameObject IAMObject, Blackboard iamInfo)
        {
            Transform parentTransform = IAMObject.transform.Find("Button Close Anchor").transform;
            parentTransform.SetAsLastSibling();
            var closeButton = MetaObjectUtils.MakePrefab(bundleName, "Button Close Timer", parentTransform);
            var closeButtonTimer = closeButton.GetComponent<CloseButtonTimer>();

            var locktimeSec = BlackboardUtils.FindVariable<int>(iamInfo, "locktimeSec").value;
            if (locktimeSec > 0)
                closeButtonTimer.StartTimer((float)locktimeSec);
            else
                closeButtonTimer.StopTimer();
        }

        public static void AddListenerOnButton(GameObject buttonObject, GameObject IAMObject, Blackboard component, InAppMessageType iamType)
        {
            ContextElement buttonElement = buttonObject.GetComponent<ContextElement>();
            if (buttonElement != null)
            {
                Blackboard buttonBB = buttonObject.AddComponent<Blackboard>();
                BlackboardUtils.SetOrCreateValue<Blackboard>(buttonBB, "componentBB", component);

                GraphOwner owner = IAMObject.GetComponent<GraphOwner>();
                IContextClickable clickableElement = buttonElement as IContextClickable;
                clickableElement.AddListenerOnClick((ContextElement sender) => { owner.SendEvent<ContextElement>("OnClickButton", sender); });

#if UNITY_WEBGL && !UNITY_EDITOR
                 var hash = component.GetVariable<Blackboard>("action").value.GetVariable<string>("hash");
                 var userId = BlackboardUtils.FindVariable<string>(null, "/me/userId");

                 if (iamType == InAppMessageType.SURVEY_POPUP)
                 {
                     IContextPressable pressableElement = buttonElement as IContextPressable;
                     pressableElement.AddListenerOnPress((ContextElement sender) =>
                     {
                         NativeHelper.Instance.DoSurvey(hash.value, userId.value, () => {});
                     } );
                 }
#endif
            }
        }

        public static List<string> GetImageUrlList(Blackboard iamInfo)
        {
            List<string> imageUrlList = new List<string>();

            if (iamInfo != null)
            {
                var componentList = BlackboardUtils.FindVariable<List<Blackboard>>(iamInfo, "componentList");

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

            return imageUrlList;
        }

        private static bool IsValidBuyButton(InAppMessageType iamType, Blackboard iamInfo, Blackboard componentBB)
        {
            switch (iamType)
            {
                case InAppMessageType.COIN_PURCHASE_POPUP:
                    {
                        Blackboard productInfoDict = iamInfo.GetValue<Blackboard>("productInfoDict");
                        int productId = componentBB.GetValue<Blackboard>("product").GetValue<int>("id");

                        Blackboard productInfo = productInfoDict.GetValue<Blackboard>(productId.ToString());
                        int purchaseLimit = productInfo.GetValue<int>("purchaseLimit");
                        int purchasedCount = productInfo.GetValue<int>("purchasedCount");

                        if (purchaseLimit != 0 && (purchaseLimit > 0 && purchasedCount >= purchaseLimit))
                            return false;
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        public static long GetGemValueForCoin()
        {
            if (gemValueForCoin == 0)
                gemValueForCoin = BlackboardUtils.FindVariable<long>(null, "/values/misc/GEM_VALUE_FOR_COIN").value;

            return gemValueForCoin;
        }

        public static long GetGemValueForCointDiscountNumerator()
        {
            if (gemValueForCointDiscountNumerator == 0)
                gemValueForCointDiscountNumerator = BlackboardUtils.FindVariable<long>(null, "/values/misc/GEM_VALUE_FOR_COIN_DISCOUNT_NUMERATOR").value;

            return gemValueForCointDiscountNumerator;
        }

        public static long CalculateGem(long bet, long extraBet, long winxNumerator)
        {
            long valueForCoinDiscount = GetGemValueForCoin() * GetGemValueForCointDiscountNumerator() / NumberUtils.GetGlobalDenominator();
            long gem = (bet + extraBet) * winxNumerator / NumberUtils.GetGlobalDenominator() / LevelUtils.GetLevelMultiplierNumeratorValue(valueForCoinDiscount, "coin");

            return RemoveUnitOfDigit(gem);
        }

        public static long CalculateBet(long baseBet, long bet, long extraBet, long rawBaseBet, long winxNumerator = 0L)
        {
            if (winxNumerator == 0L)
                winxNumerator = NumberUtils.GetGlobalDenominator();

            if (rawBaseBet == 0L) // bug
                rawBaseBet = bet;

            long numeratedValue = NumberUtils.GetMultiplierNumeratorValue(bet, winxNumerator);
            long betScale = Mathf.FloorToInt(numeratedValue / rawBaseBet);
            long resultBet = rawBaseBet * betScale;

            if (extraBet > 0L)
            {
                long validMultiplier = (long)((double)resultBet / (double)baseBet * NumberUtils.GetGlobalDenominator());
                long multiplierdExtra = NumberUtils.GetMultiplierNumeratorValue(extraBet, validMultiplier);
                return resultBet + multiplierdExtra;
            }
            else
                return resultBet;
        }

        public static long CalculateBetLevelMultiplier(long bet, long extraBet, long rawBaseBet, long winxNumerator = 0L)
        {
            if (winxNumerator == 0L)
                winxNumerator = NumberUtils.GetGlobalDenominator();
            if (rawBaseBet == 0L) // bug
                rawBaseBet = bet;

            long betScale = Mathf.FloorToInt(NumberUtils.GetMultiplierNumeratorValue(LevelUtils.GetLevelMultiplierNumeratorValue(bet, "ticketedBonus"), winxNumerator) / rawBaseBet);
            long multiplierdBet = rawBaseBet * betScale;

            if (extraBet > 0L)
            {
                long validMultiplier = (long)((double)multiplierdBet / (double)bet * NumberUtils.GetGlobalDenominator());
                long multiplierdExtra = NumberUtils.GetMultiplierNumeratorValue(extraBet, validMultiplier);
                return multiplierdBet + multiplierdExtra;
            }
            else
                return multiplierdBet;
        }

        public static long RemoveUnitOfDigit(long number)
        {
            if (number >= 100)
                number = number - (number % 10);

            return number;
        }

        public static List<BonusInfo> GetBonusInfoList(long minGem, long maxGem, long winxNumerator, int gameId)
        {
            if (!BlackboardQueryUtils.IsIngame()) return new List<BonusInfo>();

            List<BonusInfo> bonusInfoList = new List<BonusInfo>();

            List<long> betList = BlackboardUtils.FindValue<List<long>>("./betList");
            List<int> forcedExtraBetRatioIndexList = BlackboardUtils.FindValue<List<int>>("./forcedExtraBetRatioIndexList");
            List<Blackboard> extraBetRatioList = BlackboardUtils.FindValue<List<Blackboard>>("./game/extraBetRatioList");
            bool useForcedExtraBetRatio = BlackboardUtils.FindValue<bool>("./useForcedExtraBetRatio");
            int defaultBabExtraBetIndex = BlackboardUtils.FindValue<int>("./defaultBabExtraBetIndex");
            for (int i = 0; i < betList.Count; i++)
            {
                Blackboard extraBetRatio = extraBetRatioList[useForcedExtraBetRatio ? forcedExtraBetRatioIndexList[i] : defaultBabExtraBetIndex];
                int numerator = extraBetRatio.GetValue<int>("numerator");
                int denominator = extraBetRatio.GetValue<int>("denominator");

                long extraBet = betList[i] * numerator / denominator;
                long gem = CalculateGem(betList[i], extraBet, winxNumerator);

                BonusInfo bonusInfo = new BonusInfo(betList[i], extraBet, gem);
                bonusInfoList.Add(bonusInfo);
            }

            EventInfo bonusEventInfo = PassiveEventManager.Instance.GetBonusEventInfo(gameId);
            if (bonusEventInfo != null && bonusEventInfo.type == EventInfoType.BONUS_SALE)
                maxGem = maxGem * NumberUtils.GetGlobalDenominator() / (100 - PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(bonusEventInfo));

            for (int i = bonusInfoList.Count - 1; i >= 0; i--)
            {
                if (bonusInfoList[i].gem < minGem || bonusInfoList[i].gem > maxGem)
                    bonusInfoList.RemoveAt(i);
            }

            return bonusInfoList;
        }

        public static IEnumerator IAMInfoRequestCoroutine(int iamId, System.Action<Blackboard, InAppMessageTriggerType> onResponse)
        {
            bool isDone = false;
            var triggerType = InAppMessageTriggerType.UNKNOWN;
            Blackboard bb = null;

            BagelCodeClientAPI.InAppMessageInfoRequest(iamId,
                (response) =>
                {

                    if (response.inAppMessage != null)
                    {
                        BlackboardQueryUtils.RemoveIAMBlackboardById(iamId);

                        BlackboardQueryUtils.AddInAppMessage(response.inAppMessage);

                        IAMRouter.Instance.UpdateIAMInfo();

                        if (response.inAppMessage.triggerV2List.Count > 0)
                            triggerType = response.inAppMessage.triggerV2List[0].type;

                        bb = BlackboardQueryUtils.GetIAMBlackboard(iamId);
                    }

                    isDone = true;
                },
                (error) =>
                {
                    Debug.LogWarning("IAMUtils.InAppMessageInfoRequest is Failre. iamId: " + iamId);

                    isDone = true;
                });

            yield return new WaitUntil(() => isDone);
            onResponse?.Invoke(bb, triggerType);
        }

        public static IEnumerator PreloadIAMImagesCoroutine(int iamId, System.Action<bool> onLoadComplete)
        {
            bool isDone = false;
            bool isSuccess = true;
            int requestImageCount = 0;

            var iamInfo = BlackboardQueryUtils.GetIAMBlackboard(iamId);
            List<string> imageUrlList = GetImageUrlList(iamInfo);

            if (imageUrlList.Count > 0)
            {
                requestImageCount = imageUrlList.Count;

                for (int i = 0; i < imageUrlList.Count; ++i)
                {
                    WebImageDownloader.Instance.LoadWebImage(
                        imageUrlList[i],
                        CacheType.FileCache,
                        true,
                        ImageDownloaded,
                        null,
                        null,
                        ImageDownloadFailed
                    );
                }
            }
            else
            {
                isDone = true;
            }

            void ImageDownloaded(string url)
            {
                --requestImageCount;

                if (requestImageCount <= 0)
                    isDone = true;
            }

            void ImageDownloadFailed(WebImageDownloader.WebImageDownloadError error)
            {
                isSuccess = false;

                --requestImageCount;

                if (requestImageCount <= 0)
                    isDone = true;
            }

            yield return new WaitUntil(() => isDone);
            onLoadComplete?.Invoke(isSuccess);
        }

        public static bool OnVideoAdsEnd(System.Action callback)
        {
            if (BagelCode.IAMRouter.Instance.TriggerIAM(InAppMessageTriggerType.CLOSE_VIDEO_ADS, null, null))
            {
                onVideoAdsCallback = callback;
                return true;
            }

            return false;
        }

        public class BonusInfo
        {
            public long bet;
            public long extraBet;
            public long gem;

            public BonusInfo(long bet, long extraBet, long gem)
            {
                this.bet = bet;
                this.extraBet = extraBet;
                this.gem = gem;
            }
        }
    }
}
