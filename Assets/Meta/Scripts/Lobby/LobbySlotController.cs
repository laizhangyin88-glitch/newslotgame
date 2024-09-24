using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;
using ParadoxNotion;
using ParadoxNotion.Services;
using NodeCanvas.Framework;
using UnityEngine.UI;
using Newtonsoft.Json;
using SimpleJSON;

namespace BagelCode
{
    public class LobbySlotController : MonoBehaviour
    {
        public Transform anchor;
        public Transform thumbnailArea;
        public Transform topArea;
        public Transform badgeArea;
        public Transform lockArea;
        public Transform tagArea;
        public Toggle isCollect;

        public bool isLong;

        // To do. convert private
        public GameObject thumbObject;
        public GameObject jackpotBoardObject;
        public GameObject subObject;
        public GameObject lockObject;
        public GameObject lockWithOutButtonObject;
        public GameObject badgeObject;
        public GameObject slotTagObject;
        public GameObject slotTagEventObject;
        public GameObject backgroundObject;
        public GameObject selected;
        public GameObject randomEff;

        // Set Inspector Values
        public List<string> tagString;
        public List<string> tagAssetNames;
        public List<string> statusAssetNames;

        private ContextElement selfContextElement;
        private ContextElement buttonElement;
        private EnterGameInfoBehaviour enterGameInfo;

        private bool isInit = false;
        private bool isSelect = false;

        public int gameID;
        public string gameTitle;
        public int levelRestriction;
        public GameUnlockStatus unlockStatus;
        public int gameSpinCount;
        public int statusIndex;
        public int tagIndex;
        public int meLevel;
        public bool isAnimatedSlotImage;

        public Blackboard jackpotInfo;
        public List<Blackboard> jackpotList;
        public JackpotAssetType jackpotAssetType;

        public Blackboard slotInfoBB;
        public Blackboard gameInfoBB;
        public Blackboard bonusIAMBB;

        public List<int> collectList;

        private const string LOCK_ANI_PARAMETER_NAME = "Appear";

        private Dictionary<string, GameObject> slotThumbDict = new Dictionary<string, GameObject>();

        private Coroutine randomEffCoroutine;

        private void Awake()
        {
            isCollect.onValueChanged.AddListener(OnCollectChange);
        }

        private void OnEnable()
        {
            if (gameInfoBB != null)
            {
                SetGameSpin();
            }
            randomEffCoroutine = StartCoroutine(SetRandomEff());
        }

        private IEnumerator SetRandomEff()
        {
            while (true)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(0, 10f));
                randomEff.SetActive(false);
                randomEff.SetActive(true);
            }
        }

        private void OnDisable()
        {
            if (randomEffCoroutine != null)
                StopCoroutine(randomEffCoroutine);
        }

        public void UpdateSlotInfo(Blackboard newSlotInfoBB, Blackboard newGameInfoBB, Blackboard buyABonusIAMBB, bool isUnlock = false)
        {
            InitContext();
            DestroyPrevObjects();

            slotInfoBB = newSlotInfoBB;
            gameInfoBB = newGameInfoBB;
            bonusIAMBB = buyABonusIAMBB;

            SetButtonInteractable(false);

            if(gameInfoBB != null)
            {
                SetCommonValues(slotInfoBB, gameInfoBB);
                SetStatus(slotInfoBB);
                SetTag(bonusIAMBB);
                SetGameSpin();
                SetSlotImage(isUnlock);
            }
        }

        public void OnUnlockSlot()
        {
            if(gameInfoBB != null)
            {
                var newLockStatus = gameInfoBB.GetValue<GameUnlockStatus>("unlockStatus");

                if(unlockStatus != newLockStatus)
                {
                    UpdateSlotInfo(slotInfoBB, gameInfoBB, bonusIAMBB, true);
                }
            }
        }

        private void SetButtonInteractable(bool isInteractable)
        {
            MetaContextElementUtils.SetBooleanProperty(buttonElement, isInteractable);
        }

        private void InitContext()
        {
            backgroundObject.SetActive(false);

            if(isInit) return;

            selfContextElement = gameObject.GetComponent<ContextElement>();
            selfContextElement.UpdateContext(false);

            buttonElement = ContextUtils.FindElement(selfContextElement, "Anchor", ContextSearchingType.ChildrenSearch);

            enterGameInfo = gameObject.GetComponent<EnterGameInfoBehaviour>();
            enterGameInfo.gameId = 0;

            isInit = true;
        }

        private void DestroyPrevObjects()
        {
            // GameObject.Destroy(thumbObject);
            // GameObject.Destroy(jackpotBoardObject);
            // GameObject.Destroy(lockObject);
            if(thumbObject != null) thumbObject.SetActive(false);
            if(jackpotBoardObject != null) jackpotBoardObject.SetActive(false);
            if(lockObject != null) lockObject.SetActive(false);
            if(lockWithOutButtonObject != null) lockWithOutButtonObject.SetActive(false);

            GameObject.Destroy(subObject);
            GameObject.Destroy(badgeObject);
            GameObject.Destroy(slotTagObject);
            GameObject.Destroy(slotTagEventObject);
        }

        private void SetCommonValues(Blackboard slotInfoBB, Blackboard gameInfoBB)
        {
            gameID = gameInfoBB.GetValue<int>("gameId");
            gameTitle = gameInfoBB.GetValue<string>("gameTitle");
            levelRestriction = gameInfoBB.GetValue<int>("minLevel");
            unlockStatus = gameInfoBB.GetValue<GameUnlockStatus>("unlockStatus");

            statusIndex = BlackboardUtils.FindVariable<int>(slotInfoBB, "flags/status").value;
            tagIndex = BlackboardUtils.FindVariable<int>(slotInfoBB, "flags/tag").value;
            isAnimatedSlotImage = BlackboardUtils.FindVariable<bool>(slotInfoBB, "flags/isAnimated").value;

            gameSpinCount = BlackboardQueryUtils.GetGameSpinTotalCount(gameID);

            meLevel = BlackboardUtils.FindVariable<int>(null, "/me/level").value;

            enterGameInfo.gameId = 0;
            enterGameInfo.slotStatus = 0;
            collectList = MainBlackboard.Get().GetValue<List<int>>("collectList");
            isCollect.isOn = collectList.Contains(gameID);
        }

        private void SetSlotImage(bool isRefresh)
        {
            string stringImageKey = isLong ? MetaIconUtils.SLOT_THUMBNAIL_BIG : MetaIconUtils.SLOT_THUMBNAIL_SMALL;
            string assetName = StringTableUtils.GetString(StringTable.StringTableType.Global, stringImageKey, gameTitle);

            // Refresh Unlock anim obj
            if(isRefresh && IsAvailableAnimationSlotImage())
            {
                if(thumbObject != null)
                {
                    Destroy(thumbObject);
                    thumbObject = null;
                }

                if(slotThumbDict.ContainsKey(assetName))
                    slotThumbDict.Remove(assetName);
            }

            // Load from pool
            if( LoadSlotImageFromPool(assetName) ) return;

            // Load from asset
            if( MakeAnimationSlotImageFromAsset(assetName) ) return;

            if( MakeSlotImage(assetName) ) return;

            // slot image not exsit from asset. show empty slot image.
            backgroundObject.SetActive(true);
        }

        private bool LoadSlotImageFromPool(string assetName)
        {
            if(slotThumbDict.ContainsKey(assetName))
            {
                thumbObject = slotThumbDict[assetName];
                if(thumbObject != null)
                {
                    thumbObject.SetActive(true);
                    return true;
                }
            }

            return false;
        }

        private bool MakeAnimationSlotImageFromAsset(string assetName)
        {
            if(!IsAvailableAnimationSlotImage()) return false;

            thumbObject = MetaIconUtils.MakeAnimSlotImageObjectFromGameTitle(gameTitle, isLong, false, thumbnailArea, "");
            if(thumbObject == null) return false;

            slotThumbDict[assetName] = thumbObject;

            return true;
        }

        private bool MakeSlotImage(string assetName)
        {
            thumbObject = MetaIconUtils.MakeSlotImageObjectFromGameTitle(gameTitle, isLong, false, thumbnailArea, "");
            if(thumbObject == null) return false;

            slotThumbDict[assetName] = thumbObject;
            return true;
        }

        private bool IsAvailableAnimationSlotImage()
        {
            if(!isLong) return false;
            if(!isAnimatedSlotImage) return false;
            if(string.IsNullOrEmpty(gameTitle)) return false;

            // 0 Default,1 Comming soon,2 Comming soon,3 Under construction,4 Update to play,5 Early Access
            if(statusIndex == 0 || statusIndex == 5)
            {
                if(unlockStatus == GameUnlockStatus.UNLOCKED || unlockStatus == GameUnlockStatus.TEMP_UNLOCKED || meLevel >= levelRestriction)
                {
                    return true;
                }
            }

            return false;
        }

        private void SetStatus(Blackboard slotInfoBB)
        {
            // 0 Default,1 Comming soon,2 Comming soon,3 Under construction,4 Update to play,5 Early Access
            if(statusIndex == 0 || statusIndex == 5)
            {
                enterGameInfo.gameId = gameID;
                enterGameInfo.slotStatus = statusIndex;

                if(unlockStatus == GameUnlockStatus.UNLOCKED || unlockStatus == GameUnlockStatus.TEMP_UNLOCKED || meLevel >= levelRestriction)
                {
                    SetButtonInteractable(true);
                    SetJackpot(slotInfoBB);
                }
                else
                {
                    bool withoutUnlockButton = false;
                    var gemShopBB = BlackboardQueryUtils.GetShopBB(ShopType.GEM);
                    long slotUnlockCost = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "values/misc/UNLOCK_GAME_GEM_COST").value;

                    GameObject currentLockObj = null;

                    if(gemShopBB == null)
                    {
                        if(lockWithOutButtonObject == null)
                            lockWithOutButtonObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Slot Lock Without Button", lockArea, "");
                        lockWithOutButtonObject.SetActive(true);
                        currentLockObj = lockWithOutButtonObject;
                        withoutUnlockButton = true;
                    }
                    else
                    {
                        if(slotUnlockCost <= 0)
                        {
                            if(lockWithOutButtonObject == null)
                                lockWithOutButtonObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Slot Lock Without Button", lockArea, "");
                            lockWithOutButtonObject.SetActive(true);
                            currentLockObj = lockWithOutButtonObject;
                            withoutUnlockButton = true;
                        }
                        else
                        {
                            if(lockObject == null)
                                lockObject = MetaObjectUtils.MakeScene(MetaStringDefine.LOBBY_BUNDLE_NAME, "Slot Lock Scene", lockArea, "");
                            lockObject.SetActive(true);
                            currentLockObj = lockObject;
                        }
                    }

                    LobbySlotControllerUnlock unlockController = currentLockObj.GetComponent<LobbySlotControllerUnlock>();
                    if(unlockController == null)
                        unlockController = currentLockObj.AddComponent<LobbySlotControllerUnlock>();

                    unlockController.InitUnlock(gameID, levelRestriction, withoutUnlockButton);
                }
            }
            else
            {
                // Lock
                // Common Soon, Under Construction, need update
                subObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, statusAssetNames[statusIndex], lockArea, "");
                ContextElement subElement = subObject.GetComponent<ContextElement>();
                subElement.UpdateContext(true);
                if(statusIndex == 1 || statusIndex == 2)
                {
                    // Common Soon
                    MetaContextElementUtils.SimpleSetText(subElement, "Text", slotInfoBB.GetValue<string>("comingSoonText"), ContextSearchingType.ChildrenSearch);
                }
                else if(statusIndex == 4)
                {
                    // Update
                     IContextClickable clickableElement = subElement as IContextClickable;
                    if (clickableElement != null)
                    {
                        clickableElement.RemoveAllListener();
                        clickableElement.AddListenerOnClick( (ContextElement sender) => { OpenPopupAppUpdate(); } );
                    }

                    if(unlockStatus == GameUnlockStatus.LOCKED && meLevel < levelRestriction)
                    {
                        // Debug.LogError(gameID);
                        if(lockWithOutButtonObject == null)
                            lockWithOutButtonObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Slot Lock Without Button", lockArea, "");
                        lockWithOutButtonObject.SetActive(true);
                        ContextElement lockElement = lockWithOutButtonObject.GetComponent<ContextElement>();
                        lockElement.UpdateContext(true);
                        MetaContextElementUtils.SimpleSetText(lockElement, "Text", levelRestriction.ToString(), ContextSearchingType.ChildrenSearch);
                    }
                }
            }
        }

        private void SetTag(Blackboard bonusIAMBB)
        {
            // 0 Default, 1 New,2 Featured,3 Popular,4 Empty,5 Early Access,6 Sale,7 Instant bonus,8 Buy A Bonus, 9 Super Bonus, 10 Early Access
            if((statusIndex == 0 || statusIndex == 5) && (unlockStatus == GameUnlockStatus.UNLOCKED || unlockStatus == GameUnlockStatus.TEMP_UNLOCKED))
            {
                if(statusIndex == 5)
                    tagIndex = 10;

                if(tagIndex < tagAssetNames.Count && !string.IsNullOrEmpty(tagAssetNames[tagIndex]))
                {
                    long productEventMultiplier = 0L;

                    if (tagIndex == 7 && bonusIAMBB != null && bonusIAMBB.GetValue<InAppMessageType>("type") == InAppMessageType.INSTANT_BONUS_PURCHASE_POPUP && bonusIAMBB.GetValue<bool>("isSale"))
                    {
                        var salePercentage = bonusIAMBB.GetValue<int>("salePercentage");
                        var endTimestamp = bonusIAMBB.GetValue<long>("endTimestamp");

                        MakeEventTag(salePercentage, endTimestamp, true, "Slot Tag Instant Bonus");
                    }
                    else if (tagIndex == 7 && bonusIAMBB != null && bonusIAMBB.GetValue<InAppMessageType>("type") == InAppMessageType.INSTANT_BONUS_PURCHASE_POPUP && isProductEventMultiplier(bonusIAMBB, out productEventMultiplier))
                    {
                        var endTimestamp = bonusIAMBB.GetValue<long>("endTimestamp");
                        MakeEventTag(productEventMultiplier, endTimestamp, false, "Slot Tag Instant Bonus");
                    }
                    else if ((tagIndex == 8 && bonusIAMBB != null && bonusIAMBB.GetValue<InAppMessageType>("type") == InAppMessageType.BUY_A_BONUS_PURCHASE_POPUP) ||
                             (tagIndex == 9 && bonusIAMBB != null && bonusIAMBB.GetValue<InAppMessageType>("type") == InAppMessageType.SUPER_BONUS_PURCHASE_POPUP))
                    {
                        int gameId = bonusIAMBB.GetValue<int>("gameId");
                        string assetName = tagIndex == 8 ? "Slot Tag Buy A Bonus" : "Slot Tag Super Bonus";

                        var bonusEventInfo = PassiveEventManager.Instance.GetBonusEventInfo(gameId);
                        if (bonusEventInfo != null)
                        {
                            if (bonusEventInfo.type == EventInfoType.BONUS_SALE)
                            {
                                long numerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(bonusEventInfo);
                                MakeEventTag(numerator, bonusEventInfo.endTimestamp, true, assetName);
                            }
                            else if (bonusEventInfo.type == EventInfoType.BONUS_MULTIPLY)
                            {
                                long numerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(bonusEventInfo);

                                if(BlackboardQueryUtils.IsBuyABonusEventPercentText())
                                {
                                    long viewAddPercent = NumberUtils.GetAdditionalPercent(numerator);
                                    string eventText = StringTableUtils.GetString(StringTable.StringTableType.Global, "SLOT_EVENT_TAG_ADD_BET_PERCENT", viewAddPercent);

                                    MakeEventPercentTextTag(eventText, bonusEventInfo.endTimestamp, assetName);
                                }
                                else
                                {
                                    MakeEventTag(numerator, bonusEventInfo.endTimestamp, false, assetName);
                                }
                            }
                            else
                            {
                                MakeTag();
                            }
                        }
                        else
                        {
                            MakeTag();
                        }
                    }
                    else
                    {
                        MakeTag();
                    }
                }
            }
        }

        private void MakeTag()
        {
            slotTagObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, tagAssetNames[tagIndex], tagArea, "");

            var slotTagElement = slotTagObject.GetComponent<ContextElement>();
            slotTagElement.UpdateContext(true);

            string tagText = StringTableUtils.GetString(StringTable.StringTableType.Global, tagString[tagIndex]);
            MetaContextElementUtils.SimpleSetText(slotTagElement, "Text Event Tag", tagText, ContextSearchingType.ChildrenSearch);
        }

        private void MakeEventTag(long numerator, long endTimestamp, bool isSale, string assetName)
        {
            string prefabName = (endTimestamp != 0) ? "Event Tag" : "Event Tag Without Timer";

            slotTagObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, assetName, tagArea, "");
            slotTagEventObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, prefabName, slotTagObject.transform, "Event Tag Area");

            var slotTagElement = slotTagObject.GetComponent<ContextElement>();
            slotTagElement.UpdateContext(true);

            var slotTagEventElement = slotTagEventObject.GetComponent<ContextElement>();

            string stringKey = isSale ? "SLOT_EVENT_TAG_SALE" : "SLOT_EVENT_TAG_MULTIPLY";
            var argument = isSale ? numerator : NumberUtils.GetMultiplierFromNumerator(numerator);

            string eventText = StringTableUtils.GetString(StringTable.StringTableType.Global, stringKey, argument);
            MetaContextElementUtils.SimpleSetText(slotTagEventElement, "Text", eventText, ContextSearchingType.ChildrenSearch);

            var remainingElement = ContextUtils.FindElement(slotTagEventElement, "Remaining Timer", ContextSearchingType.ChildrenSearch);
            if (remainingElement != null)
                MetaContextElementUtils.SetCommonRemainingTimer(remainingElement, endTimestamp, 0, "TIME_FORMAT_HHMMSS_TOTALHOUR", null, null, "Ended", true, null);
        }

        private void MakeEventPercentTextTag(string eventText, long endTimestamp, string assetName)
        {
            slotTagObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, assetName, tagArea, "");
            slotTagEventObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Event Tag", slotTagObject.transform, "Event Tag Area");

            var slotTagElement = slotTagObject.GetComponent<ContextElement>();
            slotTagElement.UpdateContext(true);

            var slotTagEventElement = slotTagEventObject.GetComponent<ContextElement>();
            MetaContextElementUtils.SimpleSetText(slotTagEventElement, "Text", eventText, ContextSearchingType.ChildrenSearch);

            var remainingElement = ContextUtils.FindElement(slotTagEventElement, "Remaining Timer", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetCommonRemainingTimer(remainingElement, endTimestamp, 0, "TIME_FORMAT_HHMMSS_TOTALHOUR", null, null, "Ended", true, null);
        }

        private void SetGameSpin()
        {
            if(gameSpinCount > 0 && (statusIndex == 0 || statusIndex == 5) )
            {
                var isEnableGameSpin = BlackboardUtils.FindVariable<bool>(null, "/values/misc/ENABLE_GAME_SPIN");

                if(isEnableGameSpin.value)
                {
                    badgeObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Badge", badgeArea, "");
                    var badgeElement = badgeObject.GetComponent<ContextElement>();
                    badgeElement.UpdateContext(true);

                    MetaContextElementUtils.SimpleSetText(badgeElement, "Text", gameSpinCount.ToString(), ContextSearchingType.ChildrenSearch);
                    MetaContextElementUtils.SetIntProperty(badgeElement, gameSpinCount);
                }
            }
        }

        private void SetJackpot(Blackboard slotInfoBB)
        {
            var jackpotInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/jackpotInfoForLobbyList");

            jackpotInfo = BlackboardQueryUtils.GetJackpotBlackboardForLobby(jackpotInfoList.value, gameID);
            if(jackpotInfo != null)
            {
                jackpotList = BlackboardQueryUtils.GetJackpotListForLobby(jackpotInfoList.value, gameID);
                jackpotAssetType = jackpotInfo.GetValue<JackpotAssetType>("jackpotAssetType");

                if(jackpotAssetType != JackpotAssetType.NONE && jackpotAssetType != JackpotAssetType.UNKNOWN)
                {
                    if(jackpotBoardObject == null)
                    {
                        jackpotBoardObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Slot Jackpot Board", topArea, "");
                    }

                    jackpotBoardObject.SetActive(true);

                    var jackpotBoard = jackpotBoardObject.GetComponent<LobbySlotControllerJackpotBoard>();
                    jackpotBoard.SetJackpot(slotInfoBB, jackpotInfo, jackpotList);
                }
            }
        }

        private void OpenPopupAppUpdate()
        {
            string appDownloadURL = BlackboardUtils.FindVariable<string>(null, "/appDownloadUrl").value;

            var rewardCoins = BlackboardUtils.FindVariable<long>(null, "/values/misc/VERSION_UPDATE_REWARD_CREDIT");

            ErrorPopupInfo info = new ErrorPopupInfo();

            info.type = ErrorPopupType.OkWithTitle;
            info.title = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_DEPRECATED_VERSION_ERROR_WITH_TITLE");
            info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_DEPRECATED_VERSION_ERROR_WITH_REWARD", rewardCoins.value);

            info.useXButton = true;

            info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OK");
            info.buttonAutoClose1 = false;

            info.callback1 = delegate
            {
                Application.OpenURL(appDownloadURL);
            };

            info.callbackX = delegate
            {
            };

            ErrorPopupHandler.Instance.OpenError(info);
        }

        private bool isProductEventMultiplier(Blackboard bonusIAMBB, out long productEventMultiplier)
        {
            productEventMultiplier = 0L;
            List<Blackboard> componentList = BlackboardUtils.FindVariable<List<Blackboard>>(bonusIAMBB, "componentList").value;

            foreach(var component in componentList)
            {
                var eventMultiplier = BlackboardUtils.FindVariable<long>(component, "action/product/eventMultiplierNumerator");
                if (eventMultiplier != null && eventMultiplier.value > 100)
                {
                    productEventMultiplier = (long)eventMultiplier.value;
                    return true;
                }
            }

            return false;
        }

        public void Select(bool isSelect)
        {
            this.isSelect = isSelect;
            selected.SetActive(isSelect);
        }

        public void EnterGame()
        {
            if (isSelect)
            {
                gameObject.GetComponent<GameSoundPlayer>().PlayGameSound("UI_Button_Normal");
                enterGameInfo.SetEnterGameInfo();
                gameObject.GetComponent<SendEvent>().SendNow("ClickedSlot");
            }
        }

        private void OnCollectChange(bool collect)
        {
            if (!collect)
            {
                if (collectList.Contains(gameID))
                {
                    collectList.Remove(gameID);
                    SendUserCache();
                }

            }
            else
            {
                if (!collectList.Contains(gameID))
                {
                    collectList.Add(gameID);
                    SendUserCache();
                }
            }

        }

        private void SendUserCache()
        {
            var userCache = MainBlackboard.Get().GetValue<JSONNode>("userCache");
            userCache["userCollect"] = JsonConvert.SerializeObject(collectList);
            var tempNode = JSONNode.Parse("{}");
            tempNode.Add("user_cache", userCache);
            NetManager.Instance.SendMsg(RPCName.updateUserCache, tempNode);
        }
    }
}
