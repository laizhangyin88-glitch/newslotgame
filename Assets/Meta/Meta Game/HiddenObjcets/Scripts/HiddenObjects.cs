using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using System.Collections.Generic;

namespace BagelCode.HiddenObjects
{
    public static class HiddenObjects
    {
        public static class Utils
        {
            public static GameObject MainScene
            {
                get
                {
                    if (mainScene == null)
                        mainScene = GameObject.Find("Main Canvas/Area/Hidden Objects Main");
                    return mainScene;
                }
            }
            private static GameObject mainScene = null;

            public static int MaxFinder => BlackboardUtils.FindVariable<int>("/hiddenUniverseInfo/maxFinder")?.value ?? 0;
            public static int Finder => BlackboardUtils.FindVariable<int>("/hiddenUniverseInfo/finder")?.value ?? 0;
            public static int LastStageNeedFinder => BlackboardUtils.FindVariable<int>("/hiddenUniverseInfo/requiredFinderForLastStage")?.value ?? 0;

            public static string EVENT_NAME => StringTableUtils.GetString(StringTable.StringTableType.Global, "LOBBY_EVENT_NAME_HIDDEN_OBEJCTS");

            public static void AddFinderCount(int addFinder, bool isBroadcast = true)
            {
                if (addFinder == 0) return;

                int finder = Finder;
                BlackboardUtils.GetOrCreateVariable<int>(null, "/hiddenUniverseInfo/finder").value = finder + addFinder;

                if(isBroadcast)
                    EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, Events.ON_UPDATE_FINDER_COUNT);
            }

            public static void UpdateFinderCount(int finder = -1, int maxFinder = -1) // -1: ignore
            {
                if(finder != -1)
                {
                    BlackboardUtils.GetOrCreateVariable<int>(null, "/hiddenUniverseInfo/finder").value = finder;
                }

                if(maxFinder != -1)
                {
                    BlackboardUtils.GetOrCreateVariable<int>(null, "/hiddenUniverseInfo/maxFinder").value = maxFinder;
                }

                if (finder != -1 || maxFinder != -1)
                    EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, Events.ON_UPDATE_FINDER_COUNT);
            }

            public static int NeedStarForStageUnlock => BlackboardUtils.GetOrCreateVariable<int>(HiddenObjectsInfo, "neededStarForStageUnlock").value;
            public static int NeedStarForChapterUnlock => BlackboardUtils.GetOrCreateVariable<int>(HiddenObjectsInfo, "neededStarForChapterUnlock").value;

            private static Blackboard hiddenObjectsInfo = null;
            public static Blackboard HiddenObjectsInfo
            {
                get
                {
                    if (hiddenObjectsInfo == null)
                    {
                        var info = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "hiddenObjectsInfo");
                        hiddenObjectsInfo = (Blackboard)info;
                    }

                    return hiddenObjectsInfo;
                }
            }

            private static HiddenObjectsChapterData chapterData = null;
            public static HiddenObjectsChapterData ChapterData
            {
                get
                {
                    if (chapterData == null && HiddenObjectsCustomChapterData.Instance != null)
                        chapterData = HiddenObjectsCustomChapterData.Instance.data;
                    return chapterData;
                }
            }

            private static Dictionary<(string,int),List<CutSceneData>> cutSceneDataDict = null;
            public static Dictionary<(string, int), List<CutSceneData>> CutSceneDataDict
            {
                get
                {
                    if(cutSceneDataDict == null)
                    {
                        var list = HiddenObjectsCustomCutSceneData.Instance.data.cutSceneDataList;
                        cutSceneDataDict = new Dictionary<(string, int), List<CutSceneData>>();
                        foreach (var item in list)
                        {
                            if (!cutSceneDataDict.ContainsKey((item.symbol, item.stage)))
                            {
                                cutSceneDataDict.Add((item.symbol, item.stage), new List<CutSceneData>() { item });
                            }
                            else
                            {
                                cutSceneDataDict[(item.symbol, item.stage)].Add(item);
                            }
                        }
                    }

                    return cutSceneDataDict;
                }
            }

            public static bool IsValidChapterIndex(int chapter, out List<string> hiddenUniverseSymbolList)
            {
                hiddenUniverseSymbolList = hiddenObjectsInfo.GetValue<List<string>>("hiddenUniverseSymbolList");
                if (!hiddenUniverseSymbolList.IsValidIndex(chapter - 1)) return false;

#if !BUILD_RROD
                int targetChapter = PlayerPrefs.GetInt("HOG_CHAPTER_SETTING");
                if(targetChapter > 0)
                {
                    if (ChapterData.chapterSymbolList.IsValidIndex(targetChapter - 1))
                        hiddenUniverseSymbolList[0] = ChapterData.chapterSymbolList[targetChapter - 1];
                }
#endif

                string symbol = hiddenUniverseSymbolList[chapter - 1];
                return ChapterData.chapterSymbolList.Contains(symbol);
            }

            public static string ChapterNumberToSymbol(int chapter)
            {
                if (IsValidChapterIndex(chapter, out List<string> chapterSymbolList))
                    return chapterSymbolList[chapter - 1];

                return "";
            }

            public static int ChapterSymbolToNumber(string symbol)
            {
                var symbloList = hiddenObjectsInfo.GetValue<List<string>>("hiddenUniverseSymbolList");
                return symbloList.IndexOf(symbol);
            }

            public static string GetStageAssetName(string chapter, int stage)
            {
                return string.Format("{0} Stage {1}", chapter, stage);
            }

            public static string GetStageAssetName(int chapter, int stage)
            {
                if (IsValidChapterIndex(chapter, out List<string> chapterSymbolList))
                    return string.Format("{0} Stage {1}", chapterSymbolList[chapter - 1], stage);

                return "";
            }

            public static string GetStageBundleName(string chapter, int stage)
            {
                return string.Format("mghiddenobjects{0}st{1}", chapter, stage).ToLower();
            }

            public static string GetStageBundleName(int chapter, int stage)
            {
                if (IsValidChapterIndex(chapter, out List<string> chapterSymbolList))
                    return string.Format("mghiddenobjects{0}st{1}", chapterSymbolList[chapter - 1], stage).ToLower();

                return "";
            }

            public static bool GetCutSceneData(int chapter, int stage, out List<CutSceneData> sceneDataList) // stage 0 = main story
            {
                string chapterSymbol = ChapterNumberToSymbol(chapter);

                if(CutSceneDataDict.TryGetValue((chapterSymbol, stage), out List<CutSceneData> _sceneDataList))
                {
                    sceneDataList = _sceneDataList;
                    return true;
                }

                sceneDataList = null;
                return false;
            }

            public static bool IsStoryEnabled(int chapter, int stage)
            {
                string symbol = ChapterNumberToSymbol(chapter);
                string playerPrefsKey = string.Format(
                    Defines.PLAYER_PREFS_SHOWN_CUT_SCENE_FORMAT, symbol, stage);

                bool shown = PlayerPrefs.GetInt(playerPrefsKey, 0) == 1;

                GetCutSceneData(chapter, stage, out List<CutSceneData> sceneDataList);
                return !shown && sceneDataList != null;
            }
        }

        public static class Defines
        {
            // Main
            public const int STAGE_CELL_COUNT = 5;
            public const int CHAPTER_CELL_COUNT = 4;

            // In Game
            public const int MAX_STAR_COUNT = 5;

            // Player Prefs
            public const string PLAYER_PREFS_IS_FIRST_ENTER = "HIDDEN_OBJECTS_IS_FIRST_ENTER";

            public const string PLAYER_PREFS_SHOWN_CUT_SCENE_FORMAT = "SHOWN_CUT_SCENE_{0}"; // symbol
            public const string PLAYER_PREFS_LAST_UNLOCKED_CHAPTER_INDEX = "LAST_UNLOCKED_CHAPTER_INDEX";

            public const string PLAYER_PREFS_SHOW_HINT = "HOG_SHOW_HINT";
            public const string PLAYER_PREFS_SHOW_RECT = "HOG_SHOW_RECT";
            public const string PLAYER_PREFS_SHOW_ALL = "HOG_SHOW_ALL";
            public const string PLAYER_PREFS_TOTAL_SCORE_ZERO = "HOG_TOTAL_SCORE_ZERO";
            public const string PLAYER_PREFS_ZOOM_ENABLE = "HOG_ZOOM_ENABLE";
            public const string PLAYER_PREFS_CHAPTER_UNLOCK_EASY = "HOG_CHAPTER_UNLOCK_EASY";

            // Bundle
            public const string COMMON_BUNDLE = "mghiddenobjectscommon";
            public const string CONTENTS_BUNDLE = "mghiddenobjectscontents";

            // Sounds
            // ㄴCommon
            public const string SOUNDS_START = "Meta_HOG_Start";
            public const string SOUNDS_FINDER_OBTAIN1 = "Meta_HOG_Finder_Obtain1";
            public const string SOUNDS_FINDER_OBTAIN2 = "Meta_HOG_Finder_Obtain2";
            // ㄴContents
            public const string SOUNDS_BACKGROUND = "Meta_HOG_Background";
            public const string SOUNDS_NEW_STAGE_UNLOCK = "Meta_HOG_Stage_Unlock";
            public const string SOUNDS_NEW_CHAPTER_UNLOCK = "Meta_HOG_Chapter_Unlock";
            public const string SOUNDS_COLLECT_FINDER = "Meta_HOG_Collect_Finder";
            public const string SOUNDS_SELECT_CHAPTER = "Meta_HOG_Select_Chapter";
            public const string SOUNDS_CONSUME_FINDER = "Meta_HOG_Consume_Finder";

            public const string SOUNDS_STAGE_READY = "Meta_HOG_Stage_Ready";
            public const string SOUNDS_STAGE_PENALTY = "Meta_HOG_Stage_Penalty";
            public const string SOUNDS_OBJECT_NOCOMBO = "Meta_HOG_Object_Nocombo";
            public const string SOUNDS_OBJECT_COMBO_FORMAT = "Meta_HOG_Object_Combo{0}";
            public const string SOUNDS_HINT = "Meta_HOG_Hint";
            public const string SOUNDS_OBJECT_DISAPPEAR = "Meta_HOG_Object_Disappear";
            public const string SOUNDS_HURRY_UP1 = "Meta_HOG_Hurry_Up1";
            public const string SOUNDS_HURRY_UP2 = "Meta_HOG_Hurry_Up2";

            public const string SOUNDS_SCORE_ADDITION = "Meta_HOG_Score_Addition";
            public const string SOUNDS_STAR_ADDITION = "Meta_HOG_Star_Addition";
            public const string SOUNDS_GEM_EARNED = "Meta_HOG_Collect_Gem_Earned";
            public const string SOUNDS_STAGE_PERFECT_CLEAR = "Meta_HOG_Stage_PerfectClear";
            public const string SOUNDS_STAGE_CLEAR = "Meta_HOG_Stage_Clear";
            public const string SOUNDS_STAGE_TIME_UP = "Meta_HOG_Stage_Timeup";
        }

        public static class Events
        {
            // General
            public const string ON_SUCCESS = "OnSuccess";
            public const string ON_FAIL = "OnFail";

            // MetaUIEvent (Global)
            public const string ON_UPDATE_FINDER_COUNT = "OnUpdateFinderCount";
            public const string ON_ENTER_FINDER_SHOP = "OnEnterFinderShop";
            public const string ON_EXIT_FINDER_SHOP = "OnExitFinderShop";
            public const string ON_FINISH_CUT_SCENE = "OnFinishCutScene";
            public const string ON_OPEN_LOADING = "OnOpenLoading";
            public const string ON_FINDER_CONSUMED = "OnFinderConsumed";
            public const string ON_START_STAGE_UNLOCK_ANIM = "OnStartStageUnlockAnim";
            public const string ON_FINISH_STAGE_UNLOCK_ANIM = "OnFinishStageUnlockAnim";
            public const string ON_START_CHAPTER_UNLOCK_ANIM = "OnStartChapterUnlockAnim";
            public const string ON_FINISH_CHAPTER_UNLOCK_ANIM = "OnFinishChapterUnlockAnim";

            // Loading
            public const string ON_ENTER_HIDDEN_OBJECTS = "OnEnterHiddenObjects";
            public const string ON_RETURN_TO_LOBBY = "OnReturnToLobby";
            public const string RETURN_TO_IN_GAME = "ReturnToInGame";

            // Main Scene
            public const string ON_CLOSE = "OnClose";
            public const string ON_CANCEL_PURCHASE = "OnCancelPurchase";
            public const string ON_SUCCESS_PURCHASE = "OnSuccessPurchase";
            public const string ON_CLICK_INFORMATION = "OnClickInformation";
            public const string ON_CLICK_FINDER_REQUEST = "OnClickFinderRequest";
            public const string ON_CLICK_FINDER_SHOP = "OnClickFinderShop";
            public const string ON_CLICK_FINDER_BONUS = "OnClickFinderBonus";
            public const string ON_BACK_BUTTON = "OnBackButton";
            public const string ON_OK_BUTTON = "OnOKButton";
            public const string ON_RETURN = "OnReturn";
            public const string ON_COLLECT_CHAPTER_REWARD = "OnCollectChapterReward";

            public const string ON_CLICK_STAGE = "OnClickStage";
            public const string ON_CLICK_CHAPTER = "OnClickChapter";

            public const string ON_END_FINDER_REQUEST_COOLTIME = "OnEndFinderRequestCooltime";
            public const string ON_END_FINDER_BONUS_COOLTIME = "OnEndFinderBonusCooltime";

            public const string ON_NOT_ENOUGH_FINDER = "OnNotEnoughFinder";
            public const string ON_ENOUGH_FINDER = "OnEnoughFinder";

            // In Game
            public const string PLAY_IN_GAME_BGM = "PlayInGameBGM";
            public const string ON_SEARCH = "OnSearch";
            public const string ON_CLEAR = "OnClear";

            public const string ON_PAUSE = "OnPause";
            public const string ON_RESUME = "OnResume";
            public const string ON_QUIT = "OnQuit";

            public const string ON_USE_HINT = "OnUseHint";
            public const string ON_TRIGGER_PENALTY = "OnTriggerPenalty";
            public const string ON_FIND_OBJECT_DESTROYED = "OnFindObjectDestroyed";
            public const string ON_ARRIVE_PRIZE = "OnArrivePrize";

            public const string ON_FINISH_STAGE = "OnFinishStage";

            public const string ON_SKIP = "OnSkip";

            public const string ON_BACK_TO_MAIN = "OnBackToMain";
            public const string ON_PLAY_AGAIN = "OnPlayAgain";

            // Cut Scene
            public const string ON_CLICK = "OnClick";
            public const string PLAY_CUT_SCENE = "PlayCutScene";
            public const string CANCEL_CUT_SCENE = "CancelCutScene";

            public const string ON_CHANGE_CHAPTER = "OnChangeChapter";
            public const string CHECK_CUT_SCENE = "CheckCutScene";
        }
    }
}
