using System.Collections.Generic;
using System.Collections;
using UnityEngine;

using static BagelCode.DevEditorSettingsHiddenObjects.DebugButtonTypeHiddenObjects;
using SlotMaker;

namespace BagelCode
{
    public class DevEditorSettingsHiddenObjects : DevEditorSettings<DevEditorSettingsHiddenObjects.DebugButtonTypeHiddenObjects>
    {
        public HiddenObjects.HiddenObjectsChapterData chapterData;

        protected override int Height() => HEIGHT;

        private const int HEIGHT = 4; // update menually

        public enum DebugButtonTypeHiddenObjects
        {
            UNKNOWN = 0,
            SHOW_HINT,
            SHOW_RECT,
            RESET_CUT_SCENE,
            FIRST_CHAPTER,
            ALL_OBJECTS,
            OPEN_STAGE_LIST,
            TOTAL_SCORE_ZERO,
            RESET_LEVEL_LOCK,
            ENABLE_ZOOM,
            CHAPTER_UNLOCK_EASY,

            // Hog Deal
            HOG_DEAL_JACKPOT,
            HOG_DEAL_CLEAR_BIG_WIN,
            RESET_LAST_UNLOCKED_CHAPTER,
        }

        protected override void SetButtons()
        {
            // Buttons here
            displayButtons = new DebugButtonTypeHiddenObjects[HEIGHT, WIDTH]
            {
                { RESET_CUT_SCENE, FIRST_CHAPTER, SHOW_HINT, SHOW_RECT, ALL_OBJECTS},
                { CHAPTER_UNLOCK_EASY, UNKNOWN, RESET_LEVEL_LOCK, UNKNOWN, UNKNOWN},
                { HOG_DEAL_JACKPOT,HOG_DEAL_CLEAR_BIG_WIN,UNKNOWN,UNKNOWN,UNKNOWN},
                { OPEN_STAGE_LIST, TOTAL_SCORE_ZERO, RESET_LAST_UNLOCKED_CHAPTER, ENABLE_ZOOM, UNKNOWN},
            };
        }

        protected override void SetButtonFunctions()
        {
            // Toggle
            // HOG Chapter Setting
            elementNameDict.Add(FIRST_CHAPTER, "Button HOG Chapter Setting");
            var chapterList = new List<string>() { "HOG Chapter Default" };
            chapterList.AddRange(chapterData.chapterSymbolList);
            buttonTextDict.Add(FIRST_CHAPTER, chapterList.ToArray());
            onClickMethodDict.Add(FIRST_CHAPTER, () => OnToggle(FIRST_CHAPTER));
            playerPrefsKeyDict.Add(FIRST_CHAPTER, "HOG_CHAPTER_SETTING");

            // Hint
            elementNameDict.Add(SHOW_HINT, "Button Show Hint");
            buttonTextDict.Add(SHOW_HINT, new string[] { "Show\nHint(Off)", "Show\nHint(On)" });
            onClickMethodDict.Add(SHOW_HINT, () => OnToggle(SHOW_HINT));
            playerPrefsKeyDict.Add(SHOW_HINT, HiddenObjects.HiddenObjects.Defines.PLAYER_PREFS_SHOW_HINT);

            // Rect
            elementNameDict.Add(SHOW_RECT, "Button Show Rect");
            buttonTextDict.Add(SHOW_RECT, new string[] { "Show\nRect(Off)", "Show\nRect(On)" });
            onClickMethodDict.Add(SHOW_RECT, () => OnToggle(SHOW_RECT));
            playerPrefsKeyDict.Add(SHOW_RECT, HiddenObjects.HiddenObjects.Defines.PLAYER_PREFS_SHOW_RECT);

            // Show All Objects
            elementNameDict.Add(ALL_OBJECTS, "Button Show All");
            buttonTextDict.Add(ALL_OBJECTS, new string[] { "Show\nAll(Off)", "Show\nAll(On)" });
            onClickMethodDict.Add(ALL_OBJECTS, () => OnToggle(ALL_OBJECTS));
            playerPrefsKeyDict.Add(ALL_OBJECTS, HiddenObjects.HiddenObjects.Defines.PLAYER_PREFS_SHOW_ALL);

            // Show All Objects
            elementNameDict.Add(TOTAL_SCORE_ZERO, "Button Total Score Zero");
            buttonTextDict.Add(TOTAL_SCORE_ZERO, new string[] { "Score0\n(Off)", "Score0\n(On)" });
            onClickMethodDict.Add(TOTAL_SCORE_ZERO, () => OnToggle(TOTAL_SCORE_ZERO));
            playerPrefsKeyDict.Add(TOTAL_SCORE_ZERO, HiddenObjects.HiddenObjects.Defines.PLAYER_PREFS_TOTAL_SCORE_ZERO);

            // Disable Zoom
            elementNameDict.Add(ENABLE_ZOOM, "Button Zoom Enable");
            buttonTextDict.Add(ENABLE_ZOOM, new string[] { "Zoom\n(On)", "Zoom\n(Off)" });
            onClickMethodDict.Add(ENABLE_ZOOM, () => OnToggle(ENABLE_ZOOM));
            playerPrefsKeyDict.Add(ENABLE_ZOOM, HiddenObjects.HiddenObjects.Defines.PLAYER_PREFS_ZOOM_ENABLE);

            // Unlock Easy
            elementNameDict.Add(CHAPTER_UNLOCK_EASY, "Button Chapter Unlock Easy");
            buttonTextDict.Add(CHAPTER_UNLOCK_EASY, new string[] { "Unlock\nEasy(Off)", "Unlock\nEasy(On)" });
            onClickMethodDict.Add(CHAPTER_UNLOCK_EASY, () => OnToggle(CHAPTER_UNLOCK_EASY));
            playerPrefsKeyDict.Add(CHAPTER_UNLOCK_EASY, HiddenObjects.HiddenObjects.Defines.PLAYER_PREFS_CHAPTER_UNLOCK_EASY);

            // Hog Deal
            // Jackpot
            elementNameDict.Add(HOG_DEAL_JACKPOT, "Button Hog Deal Jackpot");
            buttonTextDict.Add(HOG_DEAL_JACKPOT, new string[] { "HogDeal\nJackpot(Off)", "HogDeal\nJackpot(On)" });
            onClickMethodDict.Add(HOG_DEAL_JACKPOT, () => OnToggle(HOG_DEAL_JACKPOT));
            playerPrefsKeyDict.Add(HOG_DEAL_JACKPOT, HogDeal.Defines.PLAYER_PREFS_JACKPOT);

            //Big Win
            elementNameDict.Add(HOG_DEAL_CLEAR_BIG_WIN, "Button Hog Deal Big Win");
            var hogDealWinList = new List<string>() { "HogDeal\nWin", "Big", "Super Big", "Mega", "Super Mega", "Epic" };
            buttonTextDict.Add(HOG_DEAL_CLEAR_BIG_WIN, hogDealWinList.ToArray());
            onClickMethodDict.Add(HOG_DEAL_CLEAR_BIG_WIN, () => OnToggle(HOG_DEAL_CLEAR_BIG_WIN));
            playerPrefsKeyDict.Add(HOG_DEAL_CLEAR_BIG_WIN, HogDeal.Defines.PLAYER_PREFS_WIN);

            // Functions
            // Stage List Popup
            elementNameDict.Add(OPEN_STAGE_LIST, "Button Open Stage List");
            buttonTextDict.Add(OPEN_STAGE_LIST, new string[] { "Stage\nList" });
            onClickMethodDict.Add(OPEN_STAGE_LIST, OpenStageListPopup);

            // Reset Cut Scene
            elementNameDict.Add(RESET_CUT_SCENE, "Button Reset HOG CutScene");
            buttonTextDict.Add(RESET_CUT_SCENE, new string[] { "Reset\nCutScene" });
            onClickMethodDict.Add(RESET_CUT_SCENE, OnResetHogCutScene);

            // Reset Level Lock
            elementNameDict.Add(RESET_LEVEL_LOCK, "Button Reset HOG Level Lock");
            buttonTextDict.Add(RESET_LEVEL_LOCK, new string[] { "Reset\nLevel Lock" });
            onClickMethodDict.Add(RESET_LEVEL_LOCK, OnResetHogCutScene);

            // Reset Cut Scene
            elementNameDict.Add(RESET_LAST_UNLOCKED_CHAPTER, "Button Reset HOG Last Unlocked");
            buttonTextDict.Add(RESET_LAST_UNLOCKED_CHAPTER, new string[] { "Reset\nUnlocked Chapter" });
            onClickMethodDict.Add(RESET_LAST_UNLOCKED_CHAPTER, OnResetLevelLock);
        }

        private void OpenStageListPopup()
        {
            StartCoroutine(OpenStageListPopupCoroutine());
        }

        private IEnumerator OpenStageListPopupCoroutine()
        {
            var mainObj = HiddenObjects.HiddenObjects.Utils.MainScene;
            if (mainObj == null)
            {
                Debug.LogError("Enter hog first.");
                yield break;
            }

            GameObject loadingObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenLoadingPopupCoroutine(
                (GameObject popupObj) => loadingObj = popupObj));

            string bundle = "testsuite";
            string asset = "Popup Dev Button Scroller Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;
            GameObject stageListObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                (SceneLoadOperation sceneLoadOperation) => stageListObj = sceneLoadOperation.GetScene()));

            stageListObj.AddComponent<HiddenObjects.PopupDevButtonScrollerHiddenObjects>();

            MetaPopupUtils.ClosePopup(loadingObj);

            MetaPopupUtils.OpenPopup(stageListObj);

            OnClickClose();
        }

        private void OnResetHogCutScene()
        {
            var symbolList = HiddenObjects.HiddenObjects.Utils.ChapterData.chapterSymbolList;
            for (int i = 0; i < symbolList.Count; ++i)
            {
                string symbol = symbolList[i];
                for (int j = 0; j < HiddenObjects.HiddenObjects.Defines.STAGE_CELL_COUNT; ++j)
                {
                    string playerPrefsKey = string.Format(
                        HiddenObjects.HiddenObjects.Defines.PLAYER_PREFS_SHOWN_CUT_SCENE_FORMAT, symbol, j + 1);

                    PlayerPrefs.SetInt(playerPrefsKey, 0);
                }
            }
        }

        private void OnResetLevelLock()
        {
            PlayerPrefs.SetInt(HiddenObjects.HiddenObjects.Defines.PLAYER_PREFS_LAST_UNLOCKED_CHAPTER_INDEX, 0);
        }
    }
}
