using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using BagelCode.ClientModels;

namespace BagelCode.VegasDreams
{
    public static class VegasDreams
    {
        public static class Utils
        {
            public static GameObject MainScene
            {
                get
                {
                    if (mainScene == null)
                        mainScene = GameObject.Find("Main Canvas/Area/Vegas Dreams");
                    return mainScene;
                }
            }
            private static GameObject mainScene = null;

            public static Blackboard MainSceneBlackboard
            {
                get
                {
                    if (mainSceneBlackboard == null)
                        mainSceneBlackboard = MainScene?.GetComponent<Blackboard>();
                    return mainSceneBlackboard;
                }
            }
            private static Blackboard mainSceneBlackboard = null;

            public static Blackboard VegasDreamsInfo
            {
                get
                {
                    if (vegasDreamsInfo == null)
                    {
                        var info = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "buildDreamInfo");
                        vegasDreamsInfo = (Blackboard)info;
                    }

                    return vegasDreamsInfo;
                }
            }
            private static Blackboard vegasDreamsInfo;

            public static bool IsEnded => TimeUtils.GetTimeStamp() > SeasonEndTimestamp;
            public static bool IsValidFreeDepot => TimeUtils.GetTimeStamp() > FreeDepotLastCollectTimestamp + FreeDepotCooltime;

            #region Build Dream Info
            public static int SeasonId => BlackboardUtils.FindVariable<int>(VegasDreamsInfo, "seasonId")?.value ?? 0;
            public static int SeasonThemeId => BlackboardUtils.FindVariable<int>(VegasDreamsInfo, "seasonThemeId")?.value ?? 0;
            public static int TotalDepotCount => BlackboardUtils.FindVariable<int>(VegasDreamsInfo, "totalDepotCount")?.value ?? 0;
            public static long VipLoungeBenefitEndTimestamp => BlackboardUtils.FindVariable<long>(VegasDreamsInfo, "vipLoungeBenefitEndTimestamp")?.value ?? 0;
            public static long SeasonEndTimestamp => BlackboardUtils.FindVariable<long>(VegasDreamsInfo, "seasonEndTimestamp")?.value ?? 0;
            #endregion

            public static string SeasonName => BlackboardUtils.FindVariable<string>(MainSceneBlackboard, "buildDreamSeason/name")?.value ?? "";

            public static long FreeDepotLastCollectTimestamp => BlackboardUtils.FindVariable<long>(MainSceneBlackboard, "freeDepotLastCollectTimestamp")?.value ?? 0;

            public static long FreeDepotCooltime => BlackboardUtils.FindVariable<long>(MainSceneBlackboard, "buildDreamConstants/FREE_DEPOT_COOLTIME_MILLISEC")?.value ?? 0;
            public static long DailyChestCooltime => BlackboardUtils.FindVariable<long>(MainSceneBlackboard, "buildDreamConstants/DAILY_CHEST_COOLTIME_MILLISEC")?.value ?? 0;
            public static long SeasonFinalRewardCredit => BlackboardUtils.FindVariable<long>(MainSceneBlackboard, "buildDreamConstants/SEASON_FINAL_REWARD_CREDIT")?.value ?? 0;
            public static long SeasonFinalRewardGem => BlackboardUtils.FindVariable<long>(MainSceneBlackboard, "buildDreamConstants/SEASON_FINAL_REWARD_GEM")?.value ?? 0;
            public static long FinalRewardCredit => BlackboardUtils.FindVariable<long>(MainSceneBlackboard, "finalRewardCredit")?.value ?? 0;
            public static long FinalRewardGem => BlackboardUtils.FindVariable<long>(MainSceneBlackboard, "finalRewardGem")?.value ?? 0;
            public static long WildDepotExp => BlackboardUtils.FindVariable<long>(MainSceneBlackboard, "buildDreamConstants/WILD_DEPOT_EXP")?.value ?? 0;
            public static int WildPuzzleCount => BlackboardUtils.FindVariable<int>(MainSceneBlackboard, "wildPuzzleCount")?.value ?? 0;
            public static int WildPuzzleCountMax => BlackboardUtils.FindVariable<int>(MainSceneBlackboard, "buildDreamConstants/WILD_PUZZLE_COUNT_MAX")?.value ?? 0;
            public static int BuildingMaxLevel => BlackboardUtils.FindVariable<int>(MainSceneBlackboard, "buildDreamConstants/BUILDING_LEVEL_MAX")?.value ?? 0;
            public static int GurusBuildingIndex => BlackboardUtils.FindVariable<int>(MainSceneBlackboard, "buildDreamConstants/GURUS_BUILDING_INDEX")?.value ?? 0;

            public static int NewGurusRank => BlackboardUtils.FindVariable<int>(MainSceneBlackboard, "newGurusRankPercentile/rank")?.value ?? 0;
            public static int GurusRank => BlackboardUtils.FindVariable<int>(MainSceneBlackboard, "gurusRankPercentile/rank")?.value ?? 0;
            public static int GurusPercentile => BlackboardUtils.FindVariable<int>(MainSceneBlackboard, "gurusRankPercentile/percentile")?.value ?? 0;

            public static bool DepotBundleShopActive => BlackboardUtils.FindVariable<bool>(MainSceneBlackboard, "depotBundleShopActive")?.value ?? false;

            public static Blackboard Depot => BlackboardUtils.FindVariable<Blackboard>(MainSceneBlackboard, "depot")?.value;
            public static Blackboard UsedDepot => BlackboardUtils.FindVariable<Blackboard>(MainSceneBlackboard, "usedDepot")?.value;
            public static Blackboard EarnedDepot => BlackboardUtils.FindVariable<Blackboard>(MainSceneBlackboard, "earnedDepot")?.value;
            public static Blackboard GurusBuilding => BlackboardUtils.FindVariable<Blackboard>(MainSceneBlackboard, "gurusBuilding")?.value;
            public static Blackboard GurusBuildingPreset => BlackboardUtils.FindVariable<Blackboard>(MainSceneBlackboard, "gurusBuildingPreset")?.value;
            public static Blackboard CreatedGurusBuilding => BlackboardUtils.FindVariable<Blackboard>(MainSceneBlackboard, "createdGurusBuilding")?.value;
            public static Blackboard NewGurusBuildingPreset => BlackboardUtils.FindVariable<Blackboard>(MainSceneBlackboard, "newGurusBuildingPreset")?.value;

            public static List<Blackboard> ExhibitionSeasonList => BlackboardUtils.FindVariable<List<Blackboard>>(MainSceneBlackboard, "exhibitionSeasonList")?.value;
            public static List<Blackboard> DailyChestList => BlackboardUtils.FindVariable<List<Blackboard>>(MainSceneBlackboard, "dailyChestList")?.value;
            public static List<Blackboard> BuildingList => BlackboardUtils.FindVariable<List<Blackboard>>(MainSceneBlackboard, "buildingList")?.value;
            public static List<Blackboard> BuildingPresetList => BlackboardUtils.FindVariable<List<Blackboard>>(MainSceneBlackboard, "buildingPresetList")?.value;
            public static List<Blackboard> SeasonBuildingPresetList => BlackboardUtils.FindVariable<List<Blackboard>>(MainSceneBlackboard, "buildDreamSeason/preset/buildingPresetList")?.value;
            public static List<Blackboard> DepotOpenResultList => BlackboardUtils.FindVariable<List<Blackboard>>(MainSceneBlackboard, "depotOpenResultList")?.value;
            public static List<Blackboard> GurusFinalRewardRankList => BlackboardUtils.FindVariable<List<Blackboard>>(MainSceneBlackboard, "gurusFinalRewardPreset/rewardByRankList")?.value;
            public static List<Blackboard> GurusFinalRewardPercentileList => BlackboardUtils.FindVariable<List<Blackboard>>(MainSceneBlackboard, "gurusFinalRewardPreset/rewardByPercentileList")?.value;

            public static string EVENT_NAME => StringTableUtils.GetString(StringTable.StringTableType.Global, "LOBBY_EVENT_NAME_VEGAS_DREAMS");

            public static long SpinEarnDepot
            {
                get
                {
                    return VegasDreamsInfo.GetValue<int>("earnedDepotCount") > 0 ? 1 : -1L;
                }
            }

            public static string GetContentsSeasonBundle()
            {
                return $"mgvegasdreamscontents{SeasonThemeId}";
            }

            public static string GetObjectSeasonBundle()
            {
                return $"mgvegasdreamstheme{SeasonThemeId}obj";
            }

            public static List<int> GetBuildingObjectIndex(int index)
            {
                var presetList = SeasonBuildingPresetList;
                if (presetList == null) return null;
                return presetList[index].GetValue<List<int>>("objectIndexList");
            }

            public static string GetBuildingName(int index)
            {
                var presetList = SeasonBuildingPresetList;
                
                if (presetList == null) return "UNKNOWN";
                return presetList[index].GetValue<string>("name");
            }

            public static int GetBuildingLevel(int index)
            {
                var buildingList = BuildingList;

                foreach (var building in buildingList)
                {
                    if (building.GetValue<int>("buildingIndex") == index + 1)
                    {
                        return building.GetValue<int>("level");
                    }
                }

                return -1;
            }
            
            public static long GetBuildingExp(int index)
            {
                var buildingList = BuildingList;

                foreach (var building in buildingList)
                {
                    if (building.GetValue<int>("buildingIndex") == index + 1)
                    {
                        return building.GetValue<long>("exp");
                    }
                }

                return -1L;
            }

            public static BuildingRankType GetBuildingRank(int index)
            {
                var presetList = BuildingPresetList;
                
                if (presetList == null) return BuildingRankType.UNKNOWN;
                return presetList[index].GetValue<BuildingRankType>("rank");
            }

            public static long GetBuildingPresetData(int index, int level, string key)
            {
                var presetList = BuildingPresetList;
                List<Blackboard> buildingLevelList = null;

                foreach (var preset in presetList)
                {
                    if (preset.GetValue<int>("buildingIndex") == index + 1)
                    {
                        buildingLevelList = preset.GetValue<List<Blackboard>>("levelList");
                    }
                }

                foreach (var building in buildingLevelList)
                {
                    if (building.GetValue<int>("level") == level)
                    {
                        return building.GetValue<long>(key);
                    }
                }

                return -1L;
            }
        }

        public static class Defines
        {
            // Sounds
            public const string VEGAS_DREAMS_LOADING = "Meta_VegasDreams_Loading";
            public const string DEPOT_OBTAIN = "Meta_VegasDreams_Depot_Obtain";
            public const string MAIN_BGM = "Meta_VegasDreams_MainBGM";
            public const string EXP_UP = "Meta_VegasDreams_Exp_Up";
            public const string FULL_EXP = "Meta_VegasDreams_Full_Exp";
            public const string GET_STAR = "Meta_VegasDreams_Get_Star";
            public const string LEVEL_UP = "Meta_VegasDreams_Level_Up_Effect";
            public const string HAMMER_EFFECT = "Meta_VegasDreams_Hammer_Effect";
            public const string GET_WILD_DEPOT = "Meta_VegasDreams_Get_Wild_Depot";
            public const string GET_FREE_DEPOT = "Meta_VegasDreams_Get_Free_Depot";
            public const string OPEN_DEPOT = "Meta_VegasDreams_Open_Depot";
            public const string GET_TOTAL_REWARD = "Meta_VegasDreams_Get_Total_Reward";
            public const string FINAL_PRIZE = "Meta_VegasDreams_Get_Final_Prize";

            public const int INFORMATION_PAGE_COUNT = 5;
            public const int MAX_BUILDING_COUNT = 9;
            public const int MAX_BUILDING_LEVEL = 5;

            public const float FREE_DEPOT_AUTO_CLOSE_SECONDS = 2f;
            
            // Bundles
            public const string COMMON_BUNDLE = "mgvegasdreamscommon";
            public const string CONTENTS_BUNDLE = "mgvegasdreamscontents";
            
            //
            public const string PLAYER_PREFS_IS_FIRST_ENTER = "VEGAS_DREAMS_IS_FIRST_ENTER";
            public const string PLAYER_PREFS_DEBUG_EARN_BADGE = "VIP_LOUNGE_DEBUG_EARN_BADGE";
            public const string PLAYER_PREFS_DEBUG_STATIC_BADGE = "VIP_LOUNGE_DEBUG_STATIC_BADGE";

            // Main
            public const string DAILY_CHEST = "DailyChest";
            public const string EXHIBITION = "Exhibition";
        }

        public static class Events
        {
            public const string ON_CLICK_VEGAS_DREAMS_META_ICON = "OnClickVegasDreamsMetaIcon";
            public const string ON_ENTER_VEGAS_DREAMS = "OnEnterVegasDreams";
            public const string ON_ENTER_VIP_LOUNGE = "OnEnterVipLounge";
            public const string ON_CLOSE_WELCOME_POPUP = "OnCloseWelcomePopup";

            // OnMetaUIEvent
            public const string ON_UPDATE_VIP_LOUNGE_INFO = "OnUpdateVipLoungeInfo";
            public const string ON_UPDATE_VEGAS_DREAMS_INFO = "OnUpdateVegasDreamsInfo";

            // Main Scene
            public const string ON_CLOSE = "OnClose";
            public const string ON_COLLLECT = "OnCollect";
            public const string ON_CLICK_INFORMATION = "OnClickInformation";
            public const string ON_CLICK_EXHIBITION = "OnClickExhibition";
            public const string ON_CLICK_LEFT_ARROW = "OnClickLeftArrow";
            public const string ON_CLICK_RIGHT_ARROW = "OnClickRightArrow";
            public const string ON_CLICK_FREE_DEPOT = "OnClickFreeDepot";
            public const string ON_CLICK_GET_DEPOT = "OnClickGetDepot";
            public const string ON_CLICK_WILD_PUZZLE = "OnClickWildPuzzle";
            public const string ON_CLICK_OPEN_DEPOT = "OnClickOpenDepot";
            public const string ON_CLICK_OPEN_DEPOT_ALL = "OnClickOpenDepotAll";
            public const string ON_CLICK_GURUS_RANKING_REWARD = "OnClickGurusRankingReward";
            public const string ON_CLICK_COLLECT_DAILY_CHEST = "OnClickCollectDailyChest";
            public const string ON_UPDATE_BUILDING = "OnUpdateBuilding";
            public const string ON_UPDATE_DAILY_CHEST = "OnUpdateDailyChest";
            public const string ON_BACK_BUTTON = "OnBackButton";
            public const string ON_OK_BUTTON = "OnOKButton";
            public const string ON_RETURN = "OnReturn";
            public const string ON_VEGAS_DREAMS_END_CALLBACK = "OnVegasDreamsEndCallback";

            // Update
            public const string ON_UPDATE_DEPOT = "OnUpdateDepot";
            public const string ON_UPDATE_WILD_PUZZLE_COUNT = "OnUpdateWildPuzzleCount";
            public const string ON_UPDATE_GURUS_BUILDING = "OnUpdateGurusBuilding";
            public const string ON_UDATE_GURUS_RANKING = "OnUpdateGurusRanking";
            public const string ON_UDATE_EXIHIBITION_SEASON_LIST = "OnUpdateExhibitionSeasonList";

            // Information
            public const string ON_SELECT_LEFT_INFO = "OnSelectLeftInfo";
            public const string ON_SELECT_RIGHT_INFO = "OnSelectRightInfo";

            // Depot Open
            public const string ON_SKIP = "OnSkip";
            public const string ON_COLLISION_TOTAL_REWARD = "OnCollisionTotalReward";

            // Wild Puzzle Select
            public const string ON_CLICK_WILD_PUZZLE_BUILDING = "OnClickWildPuzzleBuilding";
            public const string ON_Collect_WILD_PUZZLE_BUILDING = "OnCollectWildPuzzleBuilding";

            // Daily Chest Popup
            public const string ON_CLICK_COLLECT_ALL_DAILY_CHEST = "OnClickCollectAllDailyChest";
            public const string ON_CLICK_COLLECT_DAILY_CHEST_ON_GIFT_POPUP = "OnClickCollectDailyChestOnGiftPopup";

            // Gurus
            public const string ON_OPEN_GURUS_BUILDING = "OnOpenGurusBuilding";

            // Exhibition
            public const string ON_CLICK_EXHIBITION_DROPDOWN_SEASON = "OnClickExhibitionDropdownSeason";
        }
    }
}
