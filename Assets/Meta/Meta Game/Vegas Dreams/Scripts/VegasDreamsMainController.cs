using System.Collections;
using UnityEngine;
using SlotMaker;
using ParadoxNotion;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using System.Collections.Generic;

using static BagelCode.VegasDreams.VegasDreams.Defines;
using System;
using System.Linq;

namespace BagelCode.VegasDreams
{
    public class VegasDreamsMainController : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private bool isInit = false;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private GameObject currentObject;
        private GameObject dailyChest;
        private GameObject gurusObject;

        private Dictionary<BuildingRankType, GameObject> gaugeObjects = new Dictionary<BuildingRankType, GameObject>();

        private ContextElement buildingAreaElement;
        private ContextElement gurusAreaElement;
        private ContextElement gurusObjectAreaElement;
        private ContextElement objectAreaElement;
        private ContextElement gaugeAreaElement;
        private ContextElement chestAreaElement;
        private ContextElement dotsAreaElement;
        private ContextElement wildPuzzleBadgeAreaElement;

        private ContextElement leftChestAreaElement;
        private ContextElement leftRewardCoinAreaElement;
        private ContextElement leftGurusRankingAreaElement;

        private ContextElement titlePrizeAreaElement;
        private ContextElement titleGurusAreaElement;
        private ContextElement titleGurusLockedElement;

        private ContextElement backgroundAreaElement;
        private ContextElement gurusBackgroundAreaElement;

        private Animator infoSpeechAnimator;
        private Coroutine infoSpeechBalloonEnumerator = null;

        private Animator dropdownButtonAnimator;
        private Animator dropDownCloseButtonAnimator;

        private Dictionary<string, (Coroutine, Animator)> balloonSpeech = new Dictionary<string, (Coroutine, Animator)>();

        private const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        private string contextId;

        private int index = 0;

        public void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            contextId = BlackboardUtils.GetOrCreateVariable<string>(bb, "contextID")?.value;
            VegasDreamsAnalytics.build_dream_building_information(contextId);
            root.UpdateContext(false);

            MetaObjectUtils.MakePrefab(CONTENTS_BUNDLE, "Vegas Dreams Contents Sounds", transform);

            StartBGM();
            InitSkeleton();
            InitDropdownButtons();
            UpdateMainCenter();
            UpdateDepotCount();
            UpdateWildPuzzleCount();

            // Play Anim
            anim.SetBool("Active", true);

            // First Enter?
            bool isFirstEnter = PlayerPrefs.GetInt(PLAYER_PREFS_IS_FIRST_ENTER, 1) == 1;
            if (isFirstEnter)
            {
                PlayerPrefs.SetInt(PLAYER_PREFS_IS_FIRST_ENTER, 0);
                OpenInformationPopup(true);
            }

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            RegisterHandleEventType(GestureManager.ON_GESTURE_EVENT);
            RegisterHandleEventType(MetaEventDefine.ON_CONTENT_EVENT);
            Register(VegasDreams.Events.ON_UDATE_EXIHIBITION_SEASON_LIST, InitDropdownButtons);

#if DEV
            GestureManager.Instance.EnableGestureHandler(GestureManager.GestureHandlerType.SWIPE, true);
#else
            GestureManager.Instance.EnableGestureHandler(GestureManager.GestureHandlerType.SWIPE, false);
#endif

            Register(GestureManager.ON_GESTURE_EVENT, GestureManager.GestureType.SWIPE_RIGHT.ToString(), (eventData) => {
                    index = (int)Mathf.Repeat(--index, 9);
                    EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLICK_LEFT_ARROW);
                });
            Register(GestureManager.ON_GESTURE_EVENT, GestureManager.GestureType.SWIPE_LEFT.ToString(), (eventData) => {
                    index = (int)Mathf.Repeat(++index, 9);
                    EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLICK_RIGHT_ARROW);
                });

            isInit = true;
        }

        private void InitSkeleton()
        {
            // Background
            backgroundAreaElement = ContextUtils.FindElement(root, "Background", CHILDREN);
            gurusBackgroundAreaElement = ContextUtils.FindElement(root, "Gurus Background", CHILDREN);

            // Title
            titlePrizeAreaElement = ContextUtils.FindElement(root, "Title", CHILDREN);
            titleGurusAreaElement = ContextUtils.FindElement(root, "Gurus Title", CHILDREN);
            titleGurusLockedElement = ContextUtils.FindElement(root, "Gurus Title/Text Gurus Locked", FULL);

            var titleNameText = ContextUtils.FindElement(root, "Title/Title Text", FULL);
            MetaContextElementUtils.SetTextGlobal(titleNameText, "VEGAS_DREAMS_MAIN_TITLE_NAME_KEY", VegasDreams.Utils.SeasonName);

            var titleCoinText = ContextUtils.FindElement(root, "Title/Coin Text", FULL);
            UpdateFinalRewardText(titleCoinText);

            // Speech Balloon
            var exhibitionSpeechBalloon = ContextUtils.FindElement(root, "Button Menu Area/Dropdown Menu/Exhibition Hall Speech Balloon", FULL);
            var dailyChestSpeechBalloon = ContextUtils.FindElement(root, "Chest Area/Locked Speech Balloon", FULL);

            balloonSpeech[EXHIBITION] = (null, exhibitionSpeechBalloon.GetComponent<Animator>());
            balloonSpeech[DAILY_CHEST] = (null, dailyChestSpeechBalloon.GetComponent<Animator>());

            buildingAreaElement = ContextUtils.FindElement(root, "Building Area", CHILDREN);
            gurusAreaElement = ContextUtils.FindElement(root, "Gurus Area", CHILDREN);
            gurusObjectAreaElement = ContextUtils.FindElement(root, "Gurus Area/Object Area", FULL);
            objectAreaElement = ContextUtils.FindElement(root, "Building Area/Object Area", FULL);
            gaugeAreaElement = ContextUtils.FindElement(root, "Building Area/Gauge Area", FULL);
            chestAreaElement = ContextUtils.FindElement(root, "Chest Area/Chest Area", FULL);

            // Center Dots Area
            dotsAreaElement = ContextUtils.FindElement(root, "Building Area/Dots Area", FULL);
            
            for (int i = 1; i <= MAX_BUILDING_COUNT; i++)
            {
                MetaObjectUtils.MakePrefab(VegasDreams.Defines.CONTENTS_BUNDLE, "Vegas Dreams Dot", dotsAreaElement.transform, null, $"Dots {i:00}");
            }

            dotsAreaElement.UpdateContext(false);

            // Left Area
            leftChestAreaElement = ContextUtils.FindElement(root, "Chest Area", CHILDREN);
            leftRewardCoinAreaElement = ContextUtils.FindElement(root, "Reward Coin Area", CHILDREN);
            leftGurusRankingAreaElement = ContextUtils.FindElement(root, "Gurus Ranking Area", CHILDREN);
            
            var chestButton = ContextUtils.FindElement(root, "Chest Area/Base", FULL);
            MetaContextElementUtils.SetClickable(chestButton, () =>
            {
                var chestList = VegasDreams.Utils.DailyChestList;

                bool flag = false;

                foreach (var chest in chestList)
                {
                    var buildingIndex = chest.GetValue<int>("buildingIndex");

                    if (buildingIndex == index + 1)
                    {
                        var lastCollectTimestamp = chest.GetValue<long>("lastCollectTimestamp");
                        var nextCollectTimestamp = lastCollectTimestamp + VegasDreams.Utils.DailyChestCooltime;

                        if (TimeUtils.GetTimeStamp() > nextCollectTimestamp)
                        {
                            EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLICK_COLLECT_DAILY_CHEST);
                        }

                        flag = true;
                        break;
                    }
                }

                if (!flag)
                {
                    ActiveBadgeSpeechBalloon(DAILY_CHEST);
                }
            });


            var rewardButton = ContextUtils.FindElement(root, "Gurus Ranking Area/Gurus Ranking/Button Reward", FULL);
            MetaContextElementUtils.SetClickable(rewardButton, () =>
            {
                EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLICK_GURUS_RANKING_REWARD);
            });

            // Gurus Dots
            var gurusDotsAreaElement = ContextUtils.FindElement(root, "Gurus Area/Dots Area", FULL);
            
            for (int i = 1; i <= MAX_BUILDING_COUNT; i++)
            {
                MetaObjectUtils.MakePrefab(VegasDreams.Defines.CONTENTS_BUNDLE, "Vegas Dreams Dot", gurusDotsAreaElement.transform, null, $"Dots {i:00}");
            }

            gurusDotsAreaElement.UpdateContext(false);
            MetaContextElementUtils.SetIntProperty(gurusDotsAreaElement, 8); // always show gurus toggle

            // Left, Right Arrows
            var buildingLeftElement = ContextUtils.FindElement(root, "Building Area/Arrow Area Left/Button Arrow", FULL);
            var buildingRightElement = ContextUtils.FindElement(root, "Building Area/Arrow Area Right/Button Arrow", FULL);
            var gurusLeftElement = ContextUtils.FindElement(root, "Gurus Area/Arrow Area Left/Button Arrow", FULL);
            var gurusRightElement = ContextUtils.FindElement(root, "Gurus Area/Arrow Area Right/Button Arrow", FULL);

            MetaContextElementUtils.SetClickable(buildingLeftElement, 
                () => 
                {
                    index = (int)Mathf.Repeat(--index, 9);
                    EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLICK_LEFT_ARROW);
                });
            MetaContextElementUtils.SetClickable(buildingRightElement, 
                () => 
                {
                    index = (int)Mathf.Repeat(++index, 9);
                    EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLICK_RIGHT_ARROW);
                });
            MetaContextElementUtils.SetClickable(gurusLeftElement, 
                () => 
                {
                    index = (int)Mathf.Repeat(--index, 9);
                    EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLICK_LEFT_ARROW);
                });
            MetaContextElementUtils.SetClickable(gurusRightElement, 
                () => 
                {
                    index = (int)Mathf.Repeat(++index, 9);
                    EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLICK_RIGHT_ARROW);
                });

            //Wild Puzzle Icon
            var wildPuzzleButton = ContextUtils.FindElement(root, "Wild Puzzle Icon", FULL);
            MetaContextElementUtils.SetClickable(wildPuzzleButton,
                () => 
                {
                    VegasDreamsAnalytics.click_button_vds("WILD_DEPOT", contextId);
                    EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLICK_WILD_PUZZLE);
                });

            // Free Depot Button
            var freeDepotButtonElement = ContextUtils.FindElement(root, "Button Free Depot", CHILDREN);
            MetaContextElementUtils.SetClickable(freeDepotButtonElement,
                () => 
                {
                    VegasDreamsAnalytics.click_button_vds("FREE_DEPOT", contextId);
                    EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLICK_FREE_DEPOT);
                });

            // Get Depot Button
            var getDepotButtonElement = ContextUtils.FindElement(root, "Button Get Depot", FULL);
            MetaContextElementUtils.SetClickable(getDepotButtonElement,
                () => 
                {
                    VegasDreamsAnalytics.click_button_vds("GET_DEPOT", contextId);
                    EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLICK_GET_DEPOT);
                });

            MetaContextElementUtils.SetActive(getDepotButtonElement, VegasDreams.Utils.DepotBundleShopActive);

            // Dropdown Button
            var dropDownButtonElement = ContextUtils.FindElement(root, "Button Menu Area/Button Vegas Dreams Menu", FULL);
            MetaContextElementUtils.SetClickable(dropDownButtonElement,
                () => {
                    VegasDreamsAnalytics.click_button_vds("MENU", contextId);
                    dropdownButtonAnimator = ContextUtils.FindElement(root, "Button Menu Area/Dropdown Menu", FULL).GetComponent<Animator>();
                    dropdownButtonAnimator.SetBool("Active", !dropdownButtonAnimator.GetBool("Active"));

                    dropDownCloseButtonAnimator = ContextUtils.FindElement(root, "Button Menu Area/Button Vegas Dreams Menu", FULL).GetComponent<Animator>();
                    dropDownCloseButtonAnimator.SetBool("Active", !dropDownCloseButtonAnimator.GetBool("Active"));
                });

            // Close Button
            var closeButtonElement = ContextUtils.FindElement(root, "Button Close", CHILDREN);
            MetaContextElementUtils.SetClickable(closeButtonElement,
                () => EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLOSE));

            // Back Button Subscribe
            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(),
                () => EventSender.SendEvent(gameObject, VegasDreams.Events.ON_BACK_BUTTON));

            // Make Right Side Depot Badges
            MakeSideDepotBadge();
            MakeCenterGauge();

            // Make Wild Puzzle Badge
            wildPuzzleBadgeAreaElement = ContextUtils.FindElement(root, $"Wild Puzzle Icon/Badge Area", FULL);
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Badge";
            Transform parent = wildPuzzleBadgeAreaElement.transform;

            var badgeObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);

            wildPuzzleBadgeAreaElement.UpdateContext(false);
        }

#region Update in BT
        public void UpdateBuildingCreditReward()
        {
            if (index == 8) return; // When gurus mode, don't need to update building credit reward

            var level = VegasDreams.Utils.GetBuildingLevel(index);
            for (int i = 1; i <= VegasDreams.Defines.MAX_BUILDING_LEVEL; i++)
            {
                if (i <= level)
                    MetaContextElementUtils.SimpleSetText(root, $"Reward Coin Area/Coin Reward Text Lv{i}", "COLLECTED", FULL);
                else
                    MetaContextElementUtils.SimpleSetTextGlobal(root, $"Reward Coin Area/Coin Reward Text Lv{i}", "COMMA_STYLE_COIN", FULL, VegasDreams.Utils.GetBuildingPresetData(index, i, "levelUpReward"));
            }
        }
        
        public void UpdateDepotCount()
        {
            UpdateSideDepotContext();
            UpdateFreeDepotButton();
        }

        public void UpdateGurusObjects()
        {
            Destroy(gurusObject);

            var titleCoinText = ContextUtils.FindElement(root, "Title/Coin Text", FULL);
            UpdateFinalRewardText(titleCoinText);

            
            MetaContextElementUtils.SetActive(titleGurusLockedElement, VegasDreams.Utils.GurusBuilding == null || PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.BUILD_DREAM_SEASON) == null);
            if (PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.BUILD_DREAM_SEASON) == null)
            {
                MetaContextElementUtils.SimpleSetActive(root, "Gurus Ranking Area/Gurus Ranking/Gurus Ranking Board/Cell Group", false, FULL);
                MetaContextElementUtils.SimpleSetActive(root, "Gurus Ranking Area/Gurus Ranking/Gurus Ranking Board/Locked", true, FULL);
            }
            else
            {
                MetaContextElementUtils.SimpleSetActive(root, "Gurus Ranking Area/Gurus Ranking/Gurus Ranking Board/Cell Group", VegasDreams.Utils.GurusBuilding != null, FULL);
                MetaContextElementUtils.SimpleSetActive(root, "Gurus Ranking Area/Gurus Ranking/Gurus Ranking Board/Locked", VegasDreams.Utils.GurusBuilding == null, FULL);
            }

            if (VegasDreams.Utils.GurusBuilding == null) // Lock state
            {
                gurusObject = MetaObjectUtils.MakePrefab(VegasDreams.Utils.GetContentsSeasonBundle(), 
                    $"Vegas Dreams Theme {VegasDreams.Utils.SeasonThemeId} Gurus Building Locked",
                    gurusObjectAreaElement.transform
                );

            }
            else // Unlock
            {
                gurusObject = MetaObjectUtils.MakePrefab(VegasDreams.Utils.GetContentsSeasonBundle(), 
                    $"Vegas Dreams Theme {VegasDreams.Utils.SeasonThemeId} Gurus Building Place",
                    gurusObjectAreaElement.transform
                );
            }
        }
#endregion

        private void UpdateSideDepotContext()
        {
            foreach (DepotType depot in Enum.GetValues(typeof(DepotType)))
            {
                if (depot == DepotType.UNKNOWN) continue;
                UpdateSideDepotTypeContext(depot);
            }

            // Collect All Depot
            var collectAllButton = ContextUtils.FindElement(root, "Depot List/Button Collect All", FULL);
            collectAllButton.GetComponent<PIDButton>().interactable = VegasDreams.Utils.TotalDepotCount > 0;
            MetaContextElementUtils.SetClickable(collectAllButton, 
                () => 
                {
                    VegasDreamsAnalytics.click_button_vds("DEPOTS_COLLECT_ALL", contextId);
                    EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLICK_OPEN_DEPOT_ALL);
                });
        }

        private void UpdateSideDepotTypeContext(DepotType type)
        {
            string typeName = type.ToString();

            var count = VegasDreams.Utils.Depot.GetValue<int>(typeName.ToLower());
            bool isValidDepot = count > 0;

            // Depot Idle Animation
            var depotListAnimator = ContextUtils.FindElement(root, "Depot List", CHILDREN).GetComponent<Animator>();
            depotListAnimator.SetBool(typeName, isValidDepot);

            // Collect Type Depot
            var depotTypeButton = ContextUtils.FindElement(root, $"Depot List/{typeName}", FULL);
            depotTypeButton.GetComponent<PIDButton>().interactable = isValidDepot;
            MetaContextElementUtils.SetClickable(depotTypeButton, () => 
            {
                VegasDreamsAnalytics.click_button_vds($"{typeName}_DEPOT", contextId);
                var eventData = new EventData<int>(VegasDreams.Events.ON_CLICK_OPEN_DEPOT, (int)type);
                EventSender.SendEvent(gameObject, eventData);
            });

            var badgeAreaElement = ContextUtils.FindElement(root, $"Depot List/{typeName}/Badge Area", FULL);

            // Badge Update
            var badgeElement = ContextUtils.FindElement(badgeAreaElement, $"Badge", CHILDREN);
            MetaContextElementUtils.SetIntProperty(badgeElement, count);

            var badgeTextElement = ContextUtils.FindElement(badgeAreaElement, $"Badge/Text", FULL);
            MetaContextElementUtils.SetText(badgeTextElement, count.ToString());
        }

        private void MakeSideDepotBadge()
        {
            foreach (DepotType depot in Enum.GetValues(typeof(DepotType)))
            {
                if (depot == DepotType.UNKNOWN) continue;
                MakeSideDepotTypeBadge(depot);
            }
        }

        private void MakeSideDepotTypeBadge(DepotType type)
        {
            string typeName = type.ToString();

            var badgeAreaElement = ContextUtils.FindElement(root, $"Depot List/{typeName}/Badge Area", FULL);
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Badge";
            Transform parent = badgeAreaElement.transform;

            var badgeObj = MetaObjectUtils.MakePrefab(bundle, asset, parent);

            badgeAreaElement.UpdateContext(false);
        }

        private void UpdateFreeDepotButton()
        {
            var freeDepotElement = ContextUtils.FindElement(root, "Button Free Depot", CHILDREN);
            freeDepotElement.GetComponent<PIDButton>().interactable = VegasDreams.Utils.IsValidFreeDepot;

            var freeDepotTimerElement = ContextUtils.FindElement(root, "Button Free Depot/Timer", FULL);
            MetaContextElementUtils.SetActive(freeDepotTimerElement, !VegasDreams.Utils.IsValidFreeDepot);

            var freeDepotTimerTextElement = ContextUtils.FindElement(root, "Button Free Depot/Timer/Text", FULL);
            var timer = freeDepotTimerTextElement.GetComponent<RemainingTimerController>();
            timer.Init(freeDepotTimerTextElement, "TIME_FORMAT_HHMMSS", "TEXT_NORMAL", "", "Ended", true, UpdateFreeDepotButton);
            timer.StartTimer(VegasDreams.Utils.FreeDepotLastCollectTimestamp + VegasDreams.Utils.FreeDepotCooltime, 0);
        }

        public void UpdateWildPuzzleCount()
        {
            var count = VegasDreams.Utils.WildPuzzleCount;
            var max = VegasDreams.Utils.WildPuzzleCountMax;

            var wildPuzzleProgressElement = ContextUtils.FindElement(root, "Wild Puzzle Icon/Progress Bar", FULL);
            MetaContextElementUtils.SetFloatProperty(wildPuzzleProgressElement, (float)count / max);
            
            var wildPuzzleProgressTextElement = ContextUtils.FindElement(root, "Wild Puzzle Icon/Progress Bar/Text Progress Bar", FULL);
            if (count == max)
                MetaContextElementUtils.SetText(wildPuzzleProgressTextElement, "MAX");
            else
                MetaContextElementUtils.SetTextGlobal(wildPuzzleProgressTextElement, "A_PER_B", count, max);

            var badgeElement = ContextUtils.FindElement(wildPuzzleBadgeAreaElement, $"Badge", CHILDREN);
            MetaContextElementUtils.SetIntProperty(badgeElement, count == max ? 1 : 0);
        }

        public void UpdateMainCenter()
        {
            MetaContextElementUtils.SetActive(buildingAreaElement, index != 8);
            MetaContextElementUtils.SetActive(leftChestAreaElement, index != 8);
            MetaContextElementUtils.SetActive(leftRewardCoinAreaElement, index != 8);
            MetaContextElementUtils.SetActive(titlePrizeAreaElement, index != 8);
            MetaContextElementUtils.SetActive(backgroundAreaElement, index != 8);

            MetaContextElementUtils.SetActive(gurusAreaElement, index == 8);
            MetaContextElementUtils.SetActive(gurusBackgroundAreaElement, index == 8);
            MetaContextElementUtils.SetActive(leftGurusRankingAreaElement, index == 8);
            MetaContextElementUtils.SetActive(titleGurusAreaElement, index == 8);
            MetaContextElementUtils.SetActive(titleGurusLockedElement, VegasDreams.Utils.GurusBuilding == null || VegasDreams.Utils.IsEnded);

            MetaContextElementUtils.SetIntProperty(dotsAreaElement, index);

            if (index == 8) // GURUS MODE
            {
                UpdateGurusObjects();
            }
            else // BUILDING
            {
                MakeCenterObject();
                MakeDailyChest();
                UpdateCenterGauge();
                UpdateBuildingCreditReward();

                MetaContextElementUtils.SimpleSetText(root, "Building Area/Text Building Name", VegasDreams.Utils.GetBuildingName(index), FULL);

                var eventData = new EventData<int>(VegasDreams.Events.ON_UPDATE_BUILDING, index);
                EventSender.SendGlobalEvent(eventData);
            }
        }

        private void MakeCenterObject()
        {
            Destroy(currentObject);

            currentObject = MetaObjectUtils.MakePrefab(
                VegasDreams.Utils.GetObjectSeasonBundle(),
                $"Theme {VegasDreams.Utils.SeasonThemeId} Building {index+1}",
                objectAreaElement.transform
            );

            var bb = currentObject.GetComponent<Blackboard>();
            bb.SetValue("index", index);
        }

        private void MakeDailyChest()
        {
            Destroy(dailyChest);

            dailyChest = MetaObjectUtils.MakePrefab(
                VegasDreams.Defines.CONTENTS_BUNDLE,
                $"Chest {VegasDreams.Utils.GetBuildingRank(index)}",
                chestAreaElement.transform
            );
            dailyChest.GetComponent<Blackboard>().SetValue("index", index);
        }

        private void MakeCenterGauge()
        {
            foreach (BuildingRankType rank in Enum.GetValues(typeof(BuildingRankType)))
            {
                if (rank == BuildingRankType.UNKNOWN) continue;
                gaugeObjects[rank] = MetaObjectUtils.MakePrefab(
                    VegasDreams.Defines.CONTENTS_BUNDLE,
                    $"Gauge Rarity {rank}",
                    gaugeAreaElement.transform
                );

                gaugeObjects[rank].SetActive(false);
            }
        }

        private void UpdateCenterGauge()
        {
            foreach (var gaugeObject in gaugeObjects)
            {
                gaugeObject.Value.SetActive(false);
            }

            var rank = VegasDreams.Utils.GetBuildingRank(index);
            var obj = gaugeObjects[rank];
            
            obj.GetComponent<Blackboard>().SetValue("index", index);
            obj.SetActive(true);
        }

        private void InitDropdownButtons()
        {                    
            // Rules Dropdown Button
            var rulesButtonElement = ContextUtils.FindElement(root, "Button Menu Area/Dropdown Menu/Button Rules", FULL);
            MetaContextElementUtils.SetClickable(rulesButtonElement,
                () => 
                {
                    EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLICK_INFORMATION);
                    dropdownButtonAnimator.SetBool("Active", false);
                    dropDownCloseButtonAnimator.SetBool("Active", false);
                });
                
            // Gurus Dropdown Button
            var gurusButtonElement = ContextUtils.FindElement(root, "Button Menu Area/Dropdown Menu/Button Gurus", FULL);
            MetaContextElementUtils.SetClickable(gurusButtonElement,
                () => 
                {
                    if (index != 8)
                    {
                        MoveToGurus();
                    }
                    dropdownButtonAnimator.SetBool("Active", false);
                    dropDownCloseButtonAnimator.SetBool("Active", false);
                });

            MetaContextElementUtils.SimpleSetActive(root, "Button Menu Area/Dropdown Menu/Button Exhibition Hall/Locked State", VegasDreams.Utils.ExhibitionSeasonList.Count == 0, FULL);
            var exhibitionButtonElement = ContextUtils.FindElement(root, "Button Menu Area/Dropdown Menu/Button Exhibition Hall", FULL);
            MetaContextElementUtils.SetClickable(exhibitionButtonElement,
                () => 
                {
                    if (VegasDreams.Utils.ExhibitionSeasonList.Count == 0)
                    {
                        // speech balloon loigc
                        ActiveBadgeSpeechBalloon(EXHIBITION);
                    }
                    else
                    {
                        // exhibition popup
                        EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLICK_EXHIBITION);
                        dropdownButtonAnimator.SetBool("Active", false);
                        dropDownCloseButtonAnimator.SetBool("Active", false);
                    }
                });
        }

        public void MoveToGurus()
        {
            index = 8;
            UpdateMainCenter();
        }

        public void OpenInformationPopup(bool isAuto = false)
        {
            string bundle = CONTENTS_BUNDLE;

            string asset = "Popup Vegas Dreams Information Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);

            var popupBB = popupObj.GetComponent<Blackboard>();

            BlackboardUtils.SetOrCreateValue(popupBB, "contextId", contextId);

            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
            MetaPopupUtils.OpenPopup(popupObj);
        }

        public IEnumerator OpenFreeDepotPopupCoroutine()
        {
            var isSuccess = false;
            BagelCodeClientAPI.RequestVegasDreamCollectFreeDepot(
                (response) =>
                {
                    ClientAPI2Blackboard.Serialize(bb, response);
                    BlackboardQueryUtils.UpdateVegasDreamsInfo(response.buildDreamInfo);
                    
                    EventSender.SendEvent(gameObject, VegasDreams.Events.ON_UPDATE_DEPOT);
                    isSuccess = true;
                },
                (error) =>
                {
                    switch (error.errorCode)
                    {
                        case Error.BUILD_DREAM_NOT_ACTIVE_ERROR:
                            VegasDreamsErrorHandler.OpenVegasDreamsEndPopup(() => EventSender.SendEvent(gameObject, VegasDreams.Events.ON_VEGAS_DREAMS_END_CALLBACK));
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                }
            );

            yield return new WaitUntil(() => isSuccess);

            string contextId = "";
            string placementKey = BlackboardUtils.FindVariable<string>("/videoAdsPlacementNames/buildDream")?.value ?? "";
            var inhouseAdsEnabled = BlackboardUtils.FindVariable<bool>(null, "/values/misc/INHOUSE_ADS_ENABLED");
            if (inhouseAdsEnabled != null && inhouseAdsEnabled.value == true)
            {
                // loading scene create -> settings(+BI) -> wait trigger -> close popup scene
                // Open Loading Popup
                GameObject loadingObj = MetaPopupUtils.OpenLoadingPopup();
                MetaSystem.BackupUserSyncInfo();
                System.GC.Collect();
                // BIClientVideoAd("click", "inhouse", "", GetBIContextId());

                if (IAMRouter.Instance.TriggerIAM(InAppMessageTriggerType.INHOUSE_ADS_FOR_FREE_DEPOT, gameObject, contextId))
                {
                    // OnIAMCallback
                    var iamCallbacklTrigger = new EventTrigger(gameObject, "OnIAMCallback");
                    yield return new WaitUntilTrigger(iamCallbacklTrigger);
                    // BIClientVideoAd("complete", "inhouse", placementKey, GetBIContextId());
                }
                else
                {
                    // BIClientVideoAd("complete", "no_video", placementKey, GetBIContextId());
                }
                MetaPopupUtils.ClosePopup(loadingObj);
            }
            else
            {
                var videoAdsEnabled = BlackboardUtils.FindVariable<bool>(null, "/values/misc/VIDEO_ADS_ENABLED");
                if (videoAdsEnabled != null && videoAdsEnabled.value == true && VideoAdsController.Instance.IsVideoAdsAvailable(placementKey))
                {
                    yield return new WaitForSeconds(0.2f);
                    System.GC.Collect();
                    // BIClientVideoAd("click", "ironsource", placementKey, GetBIContextId());

                    VideoAdsController.Instance.ShowRewardedVideo(placementKey,
                    () =>
                    {
                        Debug.Log("OnVideoAdsRewarded");
                    });
#if !UNITY_EDITOR
                    var videoCallbacklTrigger = new EventTrigger(gameObject, "OnVideoAdsRewarded");
                    yield return new WaitUntilTrigger(videoCallbacklTrigger);
#endif
                    // BIClientVideoAd("complete", "ironsource", placementKey, GetBIContextId());
                }
                else
                {
                    // BIClientVideoAd("complete", "no_video", placementKey, GetBIContextId());
                }
            }

            string bundle = CONTENTS_BUNDLE;

            string asset = "Popup Vegas Dreams Get Free Depot Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);

            var popupBB = popupObj.GetComponent<Blackboard>();

            foreach (DepotType depot in Enum.GetValues(typeof(DepotType)))
            {
                if (depot == DepotType.UNKNOWN) continue;
                string typeName = depot.ToString();
                var depotTypeButton = ContextUtils.FindElement(root, $"Depot List/{typeName}", FULL);
                BlackboardUtils.SetOrCreateValue<Transform>(popupBB, typeName, depotTypeButton.transform);
            }

            BlackboardUtils.SetOrCreateValue(popupBB, "contextId", contextId);

            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
            MetaPopupUtils.OpenPopup(popupObj);
        }

        public IEnumerator OpenBundleShopPopupCoroutine()
        {
            GameObject loadingObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenLoadingPopupCoroutine(
                (GameObject popupObj) => loadingObj = popupObj));

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Shop Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject shopObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                (SceneLoadOperation sceneLoadOperation) => shopObj = sceneLoadOperation.GetScene()));

            var shopBB = shopObj.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(shopBB, "_biContextID", contextId);
            BlackboardUtils.SetOrCreateValue(shopBB, "coinShopType", ShopType.COIN_WITH_BUILD_DREAM);
            BlackboardUtils.SetOrCreateValue(shopBB, "gemShopType", ShopType.GEM_WITH_BUILD_DREAM);
            BlackboardUtils.SetOrCreateValue(shopBB, "isNotVIPLounge", true);

            MetaPopupUtils.OpenPopup(shopObj);

            MetaObjectUtils.SetCalleeCaller(shopObj, gameObject);

            MetaPopupUtils.ClosePopup(loadingObj);

            var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);

            EventSender.SendEvent(gameObject, VegasDreams.Events.ON_UPDATE_DEPOT);
        }

        public void OpenDepotOpenPopup(int type)
        {
            BagelCodeClientAPI.RequestVegasDreamOpenDepot((DepotType)type,
                (response) =>
                {
                    ClientAPI2Blackboard.Serialize(bb, response);
                    BlackboardQueryUtils.UpdateVegasDreamsInfo(response.buildDreamInfo);
                    BlackboardQueryUtils.UpdateGurusRanking(response.newGurusRankPercentile);

                    string bundle = CONTENTS_BUNDLE;

                    string asset = "Popup Vegas Dreams Depot Open Scene";
                    Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

                    var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);

                    var popupBB = popupObj.GetComponent<Blackboard>();

                    BlackboardUtils.SetOrCreateValue(popupBB, "contextId", contextId);

                    MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
                    MetaPopupUtils.OpenPopup(popupObj);
                    EventSender.SendEvent(gameObject, VegasDreams.Events.ON_UPDATE_DEPOT);
                },
                (error) =>
                {
                    switch (error.errorCode)
                    {
                        case Error.BUILD_DREAM_NOT_ACTIVE_ERROR:
                            VegasDreamsErrorHandler.OpenVegasDreamsEndPopup(() => EventSender.SendEvent(gameObject, VegasDreams.Events.ON_VEGAS_DREAMS_END_CALLBACK));
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                }
            );
        }

        public void OpenDepotOpenPopupFromCollectAll()
        {
            BagelCodeClientAPI.RequestVegasDreamOpenDepotAll(
                (response) =>
                {
                    ClientAPI2Blackboard.Serialize(bb, response);
                    BlackboardQueryUtils.UpdateVegasDreamsInfo(response.buildDreamInfo);
                    BlackboardQueryUtils.UpdateGurusRanking(response.newGurusRankPercentile);

                    string bundle = CONTENTS_BUNDLE;

                    string asset = "Popup Vegas Dreams Depot Open Scene";
                    Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

                    var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);

                    var popupBB = popupObj.GetComponent<Blackboard>();

                    BlackboardUtils.SetOrCreateValue(popupBB, "contextId", contextId);

                    MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
                    MetaPopupUtils.OpenPopup(popupObj);
                    EventSender.SendEvent(gameObject, VegasDreams.Events.ON_UPDATE_DEPOT);
                },
                (error) =>
                {
                    switch (error.errorCode)
                    {
                        case Error.BUILD_DREAM_NOT_ACTIVE_ERROR:
                            VegasDreamsErrorHandler.OpenVegasDreamsEndPopup(() => EventSender.SendEvent(gameObject, VegasDreams.Events.ON_VEGAS_DREAMS_END_CALLBACK));
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                }
            );
        }

        public void OpenDepotOpenPopupFromWildPuzzle(int targetBuildingIndex)
        {
            BagelCodeClientAPI.RequestVegasDreamCollectWildPuzzle(targetBuildingIndex,
                (response) =>
                {
                    ClientAPI2Blackboard.Serialize(bb, response);
                    BlackboardQueryUtils.UpdateVegasDreamsInfo(response.buildDreamInfo);
                    BlackboardQueryUtils.UpdateGurusRanking(response.newGurusRankPercentile);

                    string bundle = CONTENTS_BUNDLE;

                    string asset = "Popup Vegas Dreams Depot Open Scene";
                    Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

                    var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);

                    var popupBB = popupObj.GetComponent<Blackboard>();

                    BlackboardUtils.SetOrCreateValue(popupBB, "contextId", contextId);
                    popupBB.SetValue("fromWildPuzzle", true);

                    MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
                    MetaPopupUtils.OpenPopup(popupObj);
                    EventSender.SendEvent(gameObject, VegasDreams.Events.ON_UPDATE_WILD_PUZZLE_COUNT);
                },
                (error) =>
                {
                    switch (error.errorCode)
                    {
                        case Error.BUILD_DREAM_NOT_ACTIVE_ERROR:
                            VegasDreamsErrorHandler.OpenVegasDreamsEndPopup(() => EventSender.SendEvent(gameObject, VegasDreams.Events.ON_VEGAS_DREAMS_END_CALLBACK));
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                }
            );
        }

        public void OpenDepotOpenPopupFromWildPuzzleGurus()
        {
            OpenDepotOpenPopupFromWildPuzzle(VegasDreams.Utils.GurusBuildingIndex);
        }

        public void OpenDepotOpenResultPopup()
        {
            string bundle = VegasDreams.Defines.CONTENTS_BUNDLE;

            string asset = "Popup Vegas Dreams Depot Open Result Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);

            var popupBB = popupObj.GetComponent<Blackboard>();

            BlackboardUtils.SetOrCreateValue(popupBB, "contextId", contextId);

            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
            MetaPopupUtils.OpenPopup(popupObj);
        }

        public void OpenWildPuzzlePopup()
        {
            string bundle = CONTENTS_BUNDLE;

            string asset = "Popup Vegas Dreams Wild Puzzle Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);

            var popupBB = popupObj.GetComponent<Blackboard>();

            BlackboardUtils.SetOrCreateValue(popupBB, "contextId", contextId);

            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
            MetaPopupUtils.OpenPopup(popupObj);
        }

        public void OpenWildPuzzleSelectPopup()
        {
            string bundle = CONTENTS_BUNDLE;

            string asset = "Popup Vegas Dreams Wild Puzzle Add Exp Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);

            var popupBB = popupObj.GetComponent<Blackboard>();

            BlackboardUtils.SetOrCreateValue(popupBB, "contextId", contextId);

            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
            MetaPopupUtils.OpenPopup(popupObj);
        }

        public void OpenDailyChestCollectPopup()
        {
            string bundle = CONTENTS_BUNDLE;

            string asset = "Popup Vegas Dreams Daily Chest Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);

            var popupBB = popupObj.GetComponent<Blackboard>();

            BlackboardUtils.SetOrCreateValue(popupBB, "contextId", contextId);

            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
            MetaPopupUtils.OpenPopup(popupObj);
        }

        public void OpenFinalPrizePopup()
        {
            string bundle = CONTENTS_BUNDLE;

            string asset = "Popup Vegas Dreams Final Prize Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);

            var popupBB = popupObj.GetComponent<Blackboard>();

            BlackboardUtils.SetOrCreateValue(popupBB, "contextId", contextId);

            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
            MetaPopupUtils.OpenPopup(popupObj);
        }

        public void OpenGurusOpenPopup()
        {
            string bundle = VegasDreams.Utils.GetContentsSeasonBundle();
            
            string asset = $"Popup Vegas Dreams Theme {VegasDreams.Utils.SeasonThemeId} Gurus Open Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);

            var popupBB = popupObj.GetComponent<Blackboard>();

            BlackboardUtils.SetOrCreateValue(popupBB, "contextId", contextId);

            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
            MetaPopupUtils.OpenPopup(popupObj);
        }

        public void OpenExihibitonPopup()
        {
            string bundle = VegasDreams.Defines.CONTENTS_BUNDLE;
            
            string asset = "Popup Vegas Dreams Exhibition Hall Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);

            var popupBB = popupObj.GetComponent<Blackboard>();

            BlackboardUtils.SetOrCreateValue(popupBB, "contextId", contextId);

            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
            MetaPopupUtils.OpenPopup(popupObj);
        }

        public void OpenGurusRankingRewardPopup()
        {
            string bundle = CONTENTS_BUNDLE;

            string asset = "Popup Vegas Dreams Gurus Ranking Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);

            var popupBB = popupObj.GetComponent<Blackboard>();

            BlackboardUtils.SetOrCreateValue(popupBB, "contextId", contextId);

            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
            MetaPopupUtils.OpenPopup(popupObj);
        }

        public void OpenProfilePopup(string userId)
        {
            var loadingObj = MetaPopupUtils.OpenLoadingPopup();
            
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Profile Popup Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var profileObj = MetaObjectUtils.MakeScene(bundle, asset, parent);

            var profileBB = profileObj.GetComponent<Blackboard>();

            MetaObjectUtils.SetCalleeCaller(profileObj, gameObject);
            // BlackboardUtils.SetOrCreateValue(profileBB, "bi_fromType", "hidden_universe_clear");
            BlackboardUtils.SetOrCreateValue(profileBB, "_userId", userId);
            BlackboardUtils.SetOrCreateValue(profileBB, "isInRoom", false);
            BlackboardUtils.SetOrCreateValue(profileBB, "bi_fromType", "metaGame");

            if (userId == BlackboardQueryUtils.GetMyUserId())
                profileBB.SetValue("isMe", true);

            MetaPopupUtils.OpenPopup(profileObj);
            MetaPopupUtils.ClosePopup(loadingObj);
        }

        public IEnumerator CloseCoroutine()
        {
            // Make Loading Scene
            string bundle = COMMON_BUNDLE;
            string asset = "Vegas Dreams Loading Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject loadingObj = null;
            yield return StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(
                bundle, asset, parent, (GameObject sceneObj) => loadingObj = sceneObj));

            MetaPopupUtils.OpenPopup(loadingObj);

            Blackboard loadingBB = loadingObj.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(loadingBB, "isEnter", false);
            BlackboardUtils.SetOrCreateValue(loadingBB, "isMetaInGame", bb.GetVariable<bool>("isMetaInGame")?.value ?? false);
            var prevOrientation = BlackboardUtils.FindVariable<Orientation>(bb, "prevOrientation");
            if (prevOrientation != null)
                BlackboardUtils.SetOrCreateValue(loadingBB, "prevOrientation", prevOrientation.value);

            StopBGM();
        }

        public IEnumerator OnBackButtonCoroutine()
        {
            var okButtonTrigger = new EventTrigger(gameObject, VegasDreams.Events.ON_OK_BUTTON);

            // OK Popup
            yield return StartCoroutine(OpenCommonMessagePopupCoroutine("VIP_LOUNGE_POPUP_LEAVE", true));

            EventSender.SendEvent(gameObject,
                okButtonTrigger.IsTrigger ?
                VegasDreams.Events.ON_CLOSE :
                VegasDreams.Events.ON_RETURN);

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame(); // wait for exit action in fsm
        }

        public void Close(bool instantly)
        {
            GestureManager.Instance.DisableGestureHandler(GestureManager.GestureHandlerType.SWIPE);
            MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
            if (instantly)
            {
                Destroy(gameObject);
            }
            else
            {
                anim.SetTrigger("Close");
            }
        }

        //

        private void StartBGM()
        {
            // Play BGM
            GSManager.Instance.GetHandler(MAIN_BGM).Play();
        }

        private void StopBGM()
        {
            // Stop BGM
            GSManager.Instance.GetHandler(MAIN_BGM).Stop();
        }

        
        private IEnumerator OpenCommonMessagePopupCoroutine(string messageKey, bool isBack = false)
        {
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject messageObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenOKPopupCoroutine(parent,
                (GameObject popupObj) => messageObj = popupObj));

            string message = StringTableUtils.GetString(GLOBAL, messageKey);
            string ok = StringTableUtils.GetString(GLOBAL, "BUTTON_OKAY");

            string okEvent = isBack ? VegasDreams.Events.ON_OK_BUTTON : "";

            MetaPopupUtils.SetCommonPopupData(messageObj, transform, message, "", okEvent, ok, "", "", "", "",
                true, false, true, true, true);

            MetaObjectUtils.SetCalleeCaller(messageObj, gameObject);

            MetaPopupUtils.OpenPopup(messageObj);

            var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);
        }

        private void CloseBadgesSpeechBallon()
        {
            foreach (var cell in balloonSpeech)
            {
                var (cellCoroutine, cellAnimator) = cell.Value;
                
                if (cellCoroutine != null)
                {
                    cellAnimator.SetBool("IsActive", false);
                    StopCoroutine(cellCoroutine);
                }
            }
        }

        public void ActiveBadgeSpeechBalloon(string key)
        {
            CloseBadgesSpeechBallon();

            var (coroutine, animator) = balloonSpeech[key];

            coroutine = StartCoroutine(nameof(SpeechBalloonCoroutine), animator);
            balloonSpeech[key] = (coroutine, animator);
        }

        private IEnumerator SpeechBalloonCoroutine(Animator animator)
        {
            animator.SetBool("IsActive", true);
            yield return new WaitForSeconds(3.0f);
            animator.SetBool("IsActive", false);
        }

        private void UpdateFinalRewardText(ContextElement titleCoinText)
        {
            if (VegasDreams.Utils.GurusBuilding == null)
            {
                if (VegasDreams.Utils.SeasonFinalRewardCredit > 0 && VegasDreams.Utils.SeasonFinalRewardGem > 0)
                {
                    MetaContextElementUtils.SetTextGlobal(titleCoinText, "VEGAS_DREAMS_GURUS_FINAL_REWARD_COIN_GEM", VegasDreams.Utils.SeasonFinalRewardCredit, VegasDreams.Utils.SeasonFinalRewardGem);
                }
                else if (VegasDreams.Utils.SeasonFinalRewardCredit > 0)
                {
                    MetaContextElementUtils.SetTextGlobal(titleCoinText, "COMMA_STYLE_COIN", VegasDreams.Utils.SeasonFinalRewardCredit);
                }
                else
                {
                    MetaContextElementUtils.SetTextGlobal(titleCoinText, "COMMA_STYLE_GEM", VegasDreams.Utils.SeasonFinalRewardGem);
                }
            }
            else
            {
                MetaContextElementUtils.SetText(titleCoinText, "COLLECTED");
            }
        }

        // TODO
//         private void BIClientVideoAd(string action, string typeOfAd, string placementKey, string biContextId)
//         {
//             if (typeOfAd != "inhouse" && typeOfAd != "no_video")
//             {
// #if UNITY_WSA
//                 typeOfAd = "vungle";
// #else
//                 typeOfAd = "ironsource";
// #endif
//             }

//             Dictionary<string, object> customData = new Dictionary<string, object>();
//             customData["action"] = action;
//             customData["type_of_reward"] = "depot";
//             customData["placement"] = placementKey;
//             customData["type_of_ad"] = typeOfAd;
//             customData["context_id"] = biContextId;

//             Analytics.CustomEvent("client_video_ad", customData);
//         }
    }
}
