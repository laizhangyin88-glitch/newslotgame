using System;
using UnityEngine;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;

using static BagelCode.DevEditorSettingsGeneral.DebugButtonTypeGeneral;

namespace BagelCode
{
    public class DevEditorSettingsGeneral : DevEditorSettings<DevEditorSettingsGeneral.DebugButtonTypeGeneral>
    {
        protected override int Height() => HEIGHT;

        private const int HEIGHT = 7; // update menually

        private const string TEAM_SERVER_URL = "https://logic-team.slots1.bagelgames.com/api";
        private const string QA_SERVER_URL = "https://logic-qa.slots1.bagelgames.com/api";

        public enum DebugButtonTypeGeneral
        {
            UNKNOWN = 0,
            TOGGLE_PURCHASE,
            DEFAULT,
            SAVE,
            TEAM,
            OPEN_APP_SETTINGS,
            TOGGLE_SPIN,
            NEW_GUEST,
            CLEAR_PLAYER_PREFS,
            QA,
            TEST_BINGO,
            RESET_LEAGUE_POPUP_COOLTIME,
            RESET_LEADER_PUSH_UNLOCK,
            PRINT_BUNDLE_HASH,
            TEST_PURCHASE_CRASH,
            OPEN_VIDEO,
            OPEN_URL,
            PIN,
            ORIENTATION,
            PLAY_ADS,
            CHECK_ADS,
            CRASH_REPORT_TEST,
            CHANGE_DEVICEID,
            REQUEST_IDFA,
            QA_META_TEST,
            RESET_CLUB_METAGAME_POPUP_COOLTIME,
            RESET_CLUB_LEAGUE_RESULT_POPUP,
            CLEAR_IGNORE_LIST,
            ELIGIBLE_BET,
            META_ITEM,
            DEEP_LINK,
            SKIP_DELETE_ACCOUNT_CONFIRMATION,
            META_INTERRUPTING_SHOP,
            UNITY_CRASH_REPORT_TEST,
            IAM_PREVIEW,
        }

        protected override void SetButtons()
        {
            // Buttons here
            displayButtons = new DebugButtonTypeGeneral[HEIGHT, WIDTH]
            {
                { DEFAULT,                      TEAM,                       QA,                 NEW_GUEST,      CLEAR_PLAYER_PREFS},
                { TOGGLE_PURCHASE,              TEST_PURCHASE_CRASH,        TOGGLE_SPIN,        QA_META_TEST,   CRASH_REPORT_TEST},
                { OPEN_VIDEO,                   OPEN_URL,                   ORIENTATION,        PLAY_ADS,       CHECK_ADS},
                { SAVE,                         OPEN_APP_SETTINGS,          PRINT_BUNDLE_HASH,  PIN,            CHANGE_DEVICEID},
                { RESET_LEAGUE_POPUP_COOLTIME,  RESET_LEADER_PUSH_UNLOCK,   UNKNOWN,            REQUEST_IDFA,   RESET_CLUB_METAGAME_POPUP_COOLTIME},
                { CLEAR_IGNORE_LIST,            ELIGIBLE_BET,               META_ITEM,          DEEP_LINK,      RESET_CLUB_LEAGUE_RESULT_POPUP },
                { SKIP_DELETE_ACCOUNT_CONFIRMATION, META_INTERRUPTING_SHOP, UNITY_CRASH_REPORT_TEST,    UNKNOWN,    IAM_PREVIEW },
            };
        }

        protected override void SetButtonFunctions()
        {
            // Toggle
            // META_INTERRUPTING_SHOP
            elementNameDict.Add(META_INTERRUPTING_SHOP, "Button Interrupt Shop");
            buttonTextDict.Add(META_INTERRUPTING_SHOP, new string[] { "Interrupt\nShop(Off)", "Intterupt\nShop(On)" });
            onClickMethodDict.Add(META_INTERRUPTING_SHOP, () => OnToggle(META_INTERRUPTING_SHOP));
            playerPrefsKeyDict.Add(META_INTERRUPTING_SHOP, "META_INTERRUPTING_SHOP");

            // Debug Spin
            elementNameDict.Add(TOGGLE_SPIN, "Button Toggle Spin");
            buttonTextDict.Add(TOGGLE_SPIN, new string[] { "Real\nSpin", "Debug\nSpin" });
            onClickMethodDict.Add(TOGGLE_SPIN, () => OnToggle(TOGGLE_SPIN));
            playerPrefsKeyDict.Add(TOGGLE_SPIN, "DEBUG_SPIN");

            // Test Purchase
            elementNameDict.Add(TOGGLE_PURCHASE, "Button Toggle Purchase");
            buttonTextDict.Add(TOGGLE_PURCHASE, new string[] { "Real\nPurchase", "Dev\nPurchase" });
            onClickMethodDict.Add(TOGGLE_PURCHASE, () => OnToggle(TOGGLE_PURCHASE));
            playerPrefsKeyDict.Add(TOGGLE_PURCHASE, "DEBUG_PURCHASE");

            // Test Purchase Crash
            elementNameDict.Add(TEST_PURCHASE_CRASH, "Button Test Purchase Crash");
            buttonTextDict.Add(TEST_PURCHASE_CRASH, new string[] { "Purchase\nCrash(ON)", "Purchase\nCrash(OFF)" });
            onClickMethodDict.Add(TEST_PURCHASE_CRASH, () => OnToggle(TEST_PURCHASE_CRASH));
            playerPrefsKeyDict.Add(TEST_PURCHASE_CRASH, "DEBUG_PURCHASE_CRASH");

            // Eligible Bet
            elementNameDict.Add(ELIGIBLE_BET, "Button Eligible Bet");
            buttonTextDict.Add(ELIGIBLE_BET, new string[] { "Eligible\nDefault", "Eligible\nLP", "Eligible\nHOG" });
            onClickMethodDict.Add(ELIGIBLE_BET, () => OnToggle(ELIGIBLE_BET));
            playerPrefsKeyDict.Add(ELIGIBLE_BET, "DEBUG_ELIGIBLE_BET");

            // Fix Orientation
            elementNameDict.Add(ORIENTATION, "Button Orientation");
            buttonTextDict.Add(ORIENTATION, new string[] { "Not Implemented", "Fix Landscape", "Fix Portrait" });
            onClickMethodDict.Add(ORIENTATION, () => OnToggle(ORIENTATION));
            playerPrefsKeyDict.Add(ORIENTATION, "DEBUG_ORIENTATION");

            // Delete Account Skip Confirm Email
            elementNameDict.Add(SKIP_DELETE_ACCOUNT_CONFIRMATION, "Button Orientation");
            buttonTextDict.Add(SKIP_DELETE_ACCOUNT_CONFIRMATION, new string[] { "Skip Email Confirmation(Off)", "Skip Email Confirmation(On)" });
            onClickMethodDict.Add(SKIP_DELETE_ACCOUNT_CONFIRMATION, () => OnToggle(SKIP_DELETE_ACCOUNT_CONFIRMATION));
            playerPrefsKeyDict.Add(SKIP_DELETE_ACCOUNT_CONFIRMATION, "DEBUG_SKIP_DELETE_ACCOUNT_EMAIL_CONFIRMATION");

            // Meta Item todo shk..
            // 활성 시 active인 메타게임 메타아이템 종류별로 순차적으로 얻기 (스핀마다)
            // V3MetaSystem에서 세팅..
            elementNameDict.Add(META_ITEM, "Button Meta Item");
            buttonTextDict.Add(META_ITEM, new string[] { "Default Item", "Get Item"});
            onClickMethodDict.Add(META_ITEM, () => OnToggle(META_ITEM));
            playerPrefsKeyDict.Add(META_ITEM, "DEBUG_META_ITEM");

            // Default Server
            elementNameDict.Add(DEFAULT, "Button Default");
            buttonTextDict.Add(DEFAULT, new string[] { "Default" });
            onClickMethodDict.Add(DEFAULT, OnClickDefault);

            // Team Server
            elementNameDict.Add(TEAM, "Button Team");
            buttonTextDict.Add(TEAM, new string[] { "Team\nServer" });
            onClickMethodDict.Add(TEAM, OnClickEnterTeamServer);

            // QA Server
            elementNameDict.Add(QA, "Button QA");
            buttonTextDict.Add(QA, new string[] { "QA\nServer" });
            onClickMethodDict.Add(QA, OnClickEnterQAServer);

            // New Guest
            elementNameDict.Add(NEW_GUEST, "Button New Guest");
            buttonTextDict.Add(NEW_GUEST, new string[] { "New\nGuest" });
            onClickMethodDict.Add(NEW_GUEST, OnClickNewGuest);

            // Save
            elementNameDict.Add(SAVE, "Button Save");
            buttonTextDict.Add(SAVE, new string[] { "Save &\nReset" });
            onClickMethodDict.Add(SAVE, OnClickSave);

            // App Settings
            elementNameDict.Add(OPEN_APP_SETTINGS, "Button Open App Settings");
            buttonTextDict.Add(OPEN_APP_SETTINGS, new string[] { "Push(Off)", "Push(On)" });
            onClickMethodDict.Add(OPEN_APP_SETTINGS, OnClickOpenAppSettings);

            elementNameDict.Add(CLEAR_PLAYER_PREFS, "Button Clear PlayerPrefs");
            buttonTextDict.Add(CLEAR_PLAYER_PREFS, new string[] { "Clear\nPlayerPrefs" });
            onClickMethodDict.Add(CLEAR_PLAYER_PREFS, OnClickClearPlayerPrefs);

            elementNameDict.Add(PRINT_BUNDLE_HASH, "Button Print Asset Bundle Hash");
            buttonTextDict.Add(PRINT_BUNDLE_HASH, new string[] { "Print JGQ Hash" });
            onClickMethodDict.Add(PRINT_BUNDLE_HASH, OnClickPrintJgqHash);

            elementNameDict.Add(RESET_LEAGUE_POPUP_COOLTIME, "Button Reset Club League");
            buttonTextDict.Add(RESET_LEAGUE_POPUP_COOLTIME, new string[] { "Reset Club League" });
            onClickMethodDict.Add(RESET_LEAGUE_POPUP_COOLTIME, OnClickClubLeagueCooltimeReset);

            elementNameDict.Add(RESET_LEADER_PUSH_UNLOCK, "Button Reset Captain's Call Unlock");
            buttonTextDict.Add(RESET_LEADER_PUSH_UNLOCK, new string[] { "Reset Captain's Call Unlock" });
            onClickMethodDict.Add(RESET_LEADER_PUSH_UNLOCK, OnClickCaptainsCallUnlockReset);

            elementNameDict.Add(OPEN_VIDEO, "Button Open Video");
            clickEventDict.Add(OPEN_VIDEO, "OnOpenVideo");
            buttonTextDict.Add(OPEN_VIDEO, new string[] { "Video\nURL(input2)" });

            elementNameDict.Add(OPEN_URL, "Button Open URL");
            buttonTextDict.Add(OPEN_URL, new string[] { "Open\nURL(input2)" });
            onClickMethodDict.Add(OPEN_URL, OnClickOpenURL);

            elementNameDict.Add(PIN, "Button Pin");
            clickEventDict.Add(PIN, "OnPinButton");
            buttonTextDict.Add(PIN, new string[] { "Pin" });

            elementNameDict.Add(PLAY_ADS, "Button Play ADS");
            buttonTextDict.Add(PLAY_ADS, new string[] { "Play ADS" });
            onClickMethodDict.Add(PLAY_ADS, OnPlayADS);

            elementNameDict.Add(CHECK_ADS, "Button Check ADS");
            buttonTextDict.Add(CHECK_ADS, new string[] { "Check ADS" });
            onClickMethodDict.Add(CHECK_ADS, OnCheckADS);

            elementNameDict.Add(CRASH_REPORT_TEST, "Button Crash Report Test");
            buttonTextDict.Add(CRASH_REPORT_TEST, new string[] { "Crash Report Test" });
            onClickMethodDict.Add(CRASH_REPORT_TEST, OnCrashReportTestShoot);

            elementNameDict.Add(CHANGE_DEVICEID, "Button Change DeviceId");
            buttonTextDict.Add(CHANGE_DEVICEID, new string[] { "Change\nDeviceID" });
            onClickMethodDict.Add(CHANGE_DEVICEID, OnChangeDeviceID);

            elementNameDict.Add(REQUEST_IDFA, "Button Request IDFA");
            buttonTextDict.Add(REQUEST_IDFA, new string[] { "Request\nIDFA" });
            onClickMethodDict.Add(REQUEST_IDFA, OnRequestIDFA);

            elementNameDict.Add(QA_META_TEST, "Button QA Meta Test");
            buttonTextDict.Add(QA_META_TEST, new string[] { "QA\nMetaTest" });
            onClickMethodDict.Add(QA_META_TEST, OnQAMetaTest);

            elementNameDict.Add(DEEP_LINK, "DEEP LINK");
            buttonTextDict.Add(DEEP_LINK, new string[] { "DEEP\nLINK" });
            onClickMethodDict.Add(DEEP_LINK, OnClickDeepLink);

            elementNameDict.Add(RESET_CLUB_METAGAME_POPUP_COOLTIME, "Button Meta Popup Cool Time");
            buttonTextDict.Add(RESET_CLUB_METAGAME_POPUP_COOLTIME, new string[] { "Reset\nMeta Popup Cooltime" });
            onClickMethodDict.Add(RESET_CLUB_METAGAME_POPUP_COOLTIME, OnClickClubMetaGameCooltimeReset);

            elementNameDict.Add(RESET_CLUB_LEAGUE_RESULT_POPUP, "Button League Popup Cool Time");
            buttonTextDict.Add(RESET_CLUB_LEAGUE_RESULT_POPUP, new string[] { "Reset Popup\nClub League" });
            onClickMethodDict.Add(RESET_CLUB_LEAGUE_RESULT_POPUP, OnClickClubLeaguePopupReset);

            elementNameDict.Add(CLEAR_IGNORE_LIST, "Button Clear Ignore List");
            buttonTextDict.Add(CLEAR_IGNORE_LIST, new string[] { "Clear\nIgnore List" });
            onClickMethodDict.Add(CLEAR_IGNORE_LIST, OnClickClearIgnoreList);

            elementNameDict.Add(UNITY_CRASH_REPORT_TEST, "Button Unity Crash Report Test");
            buttonTextDict.Add(UNITY_CRASH_REPORT_TEST, new string[] { "Unity Crash Report Test" });
            onClickMethodDict.Add(UNITY_CRASH_REPORT_TEST, OnUnityCrashReportTestShoot);

            elementNameDict.Add(IAM_PREVIEW, "Button Iam Simulator");
            buttonTextDict.Add(IAM_PREVIEW, new string[] { "Open Iam\nSimulator" });
            onClickMethodDict.Add(IAM_PREVIEW, OpenIamSimulator);
        }

        private void OpenIamSimulator()
        {
            StartCoroutine(OpenIamSimulatorCoroutine());
        }

        private IEnumerator OpenIamSimulatorCoroutine()
        {
            GameObject loadingObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenLoadingPopupCoroutine(
                (GameObject popupObj) => loadingObj = popupObj));

            string bundle = "testsuite";
            string asset = "Popup Dev Button Scroller Extension Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;
            GameObject stageListObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                (SceneLoadOperation sceneLoadOperation) => stageListObj = sceneLoadOperation.GetScene()));

            stageListObj.AddComponent<PopupDevButtonScrollerIamPreview>();

            MetaPopupUtils.ClosePopup(loadingObj);

            MetaPopupUtils.OpenPopup(stageListObj);

            OnClickClose();
        }

        private void OnClickClearPlayerPrefs()
        {
            UnSubscribeBackButton();

            ClearPlayerPrefs();
            SendResetEvent();
        }

        private void OnClickEnterTeamServer()
        {
            UnSubscribeBackButton();

            PlayerPrefs.SetString("Dev_URL", TEAM_SERVER_URL);
            PlayerPrefs.SetInt("Is_Enable_Change_URL_Popup", 0);
            SendResetEvent();
        }

        private void OnClickEnterQAServer()
        {
            UnSubscribeBackButton();

            PlayerPrefs.SetString("Dev_URL", QA_SERVER_URL);
            PlayerPrefs.SetInt("Is_Enable_Change_URL_Popup", 0);
            SendResetEvent();
        }

        private void OnClickDefault()
        {
            UnSubscribeBackButton();

            PlayerPrefs.SetString("Dev_URL", string.Empty);
            PlayerPrefs.SetInt("Is_Enable_Change_URL_Popup", 0);
            SendResetEvent();
        }

        private void OnClickSave()
        {
            UnSubscribeBackButton();

            var input = currentBB.GetVariable<ContextElement>("_inputField");
            IContextText text = input.value as IContextText;

            PlayerPrefs.SetString("Dev_URL", text.GetText());
            PlayerPrefs.SetInt("Is_Enable_Change_URL_Popup", 0);
            SendResetEvent();
        }

        private void OnClickNewGuest()
        {
            UnSubscribeBackButton();

            ClearPlayerPrefs();

            var newID = BiEventUtils.GenerateContextID();
            PlayerPrefs.SetString("DEBUG_DEVICE_ID", newID);
            SendResetEvent();
        }

        private void OnClickOpenURL()
        {
            var input = currentBB.GetVariable<ContextElement>("_inputField2");
            IContextText text = input.value as IContextText;

#if UNITY_WEBGL && !UNITY_EDITOR
            NativeHelper.Instance.OpenUrl(text.GetText());
#else
            Application.OpenURL(text.GetText());
#endif
        }

        private void OnPlayADS()
        {
            var input = currentBB.GetVariable<ContextElement>("_inputField2");
            IContextText text = input.value as IContextText;

            string placementKey = text.GetText();

            if (BagelCode.VideoAdsController.Instance.IsVideoAdsAvailable(placementKey))
            {
                BagelCode.VideoAdsController.Instance.ShowRewardedVideo(placementKey, null);
            }
            else
            {
                Debug.LogError(string.Format("Video is not available.({0})", placementKey));
                BagelCode.VideoAdsController.Instance.LoadPlacement(placementKey);
            }
        }

        private void OnCheckADS()
        {
            BagelCode.VideoAdsController.Instance.IntegrationTest();
        }

        private void OnClickPrintJgqHash()
        {
            string hash = AssetBundleManager.Manifest.GetAssetBundleHash("jgq").ToString();
            string key = "jgq_hash";

            Debug.LogError(string.Format("prev hash: {0}", PlayerPrefs.GetString(key)));
            Debug.LogError(string.Format("curr hash: {0}", hash));

            PlayerPrefs.SetString(key, hash);
        }

        private void OnClickClubLeagueCooltimeReset()
        {
            PlayerPrefsUtils.SetInt64(ClubDefine.PLAYER_PREFS_LAST_CLUB_CHECKED_MS, 0L);
        }

        private void OnClickClubMetaGameCooltimeReset()
        {
            ClubUtils.ResetClubMetaGamePlayerPrefs();
        }

        private void OnClickClubLeaguePopupReset()
        {
            PlayerPrefsUtils.SetInt64(ClubDefine.PLAYER_PREFS_LEAGUE_RESULT_POPUP, 0L);
        }

        private void OnClickClearIgnoreList()
        {
            PlayerPrefs.SetString("ignore_list", "");
        }

        private void OnClickCaptainsCallUnlockReset()
        {
            long clubId = BlackboardUtils.FindVariable<long>("/me/clubId")?.value ?? 0L;
            ClubUtils.SetClubPlayerPrefs(ClubDefine.PLAYER_PREFS_UNLOCK_LEADER_PUSH_POPUP_SHOWN, clubId, 0);
        }

        private void OnClickOpenAppSettings()
        {
            string biContextID = BlackboardUtils.FindValue<string>(currentBB, "_biContextID");
            BlackboardQueryUtils.BI_Device_Push_Setting(biContextID);
            NativeHelper.Instance.OpenAppSettings();
            NativeHelper.Instance.TogglePush();

            UpdateText();
        }

        private void OnCrashReportTestShoot()
        {
            // throw new Exception("Hellow world. This is a crash report test.");
            NativeHelper.Instance.TestCrashlytics(string.Format("Test Crash({0}) : {1}", ApplicationSettings.GetPlatformName().ToUpper(), ApplicationSettings.Instance.clientVersion));
        }

        private void OnUnityCrashReportTestShoot()
        {
            // throw new System.Exception(string.Format("Test Crash({0}) : {1}", ApplicationSettings.GetPlatformName().ToUpper(), ApplicationSettings.Instance.clientVersion));
            OrientationUtils.Instance.PossibleChangeOrientation();
        }

        private void OnChangeDeviceID()
        {
            //Change Deviceid Popup
            var prefab = AssetBundleManager.LoadAsset<GameObject>("testsuite", "Change Deviceid Popup");
            var go = GameObject.Instantiate(prefab) as GameObject;
            go.name = "Change Deviceid Popup";
            go.transform.SetParent(PopupManager.Instance.transform, false);

            OnClickClose();
        }

        private void OnRequestIDFA()
        {
            Debug.LogError("IDFA OnRequestIDFA");

            if (IDFAHelper.Instance.IsEligibleToSeeIDFAConsentPopup())
            {
                IDFAHelper.Instance.RequestIDFA(String.Empty);
            }
            else
            {
                Debug.LogError("IDFA not support");
            }

            NativeHelper.Instance.GetReferrerUrl(
                (referrerUrl) =>
                {
                    ReferrerDetailInfo info = new ReferrerDetailInfo(referrerUrl);
                }
            );
        }

        private void OnQAMetaTest()
        {
            ClientModels.EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo();
            if (eventInfo == null)
                return;

            switch (eventInfo.type)
            {
                case ClientModels.EventInfoType.CLUB_ARENA:
                    {
                        var prefab = AssetBundleManager.LoadAsset<GameObject>("testsuite", "QA ClubArena Meta Test Popup");
                        var go = GameObject.Instantiate(prefab) as GameObject;
                        go.name = "QA ClubArena Meta Test Popup";
                        go.transform.SetParent(PopupManager.Instance.transform, false);
                    }
                    break;
            }
            OnClickClose();
        }

        private void OnClickDeepLink()
        {
            var input = currentBB.GetVariable<ContextElement>("_inputField2");
            IContextText text = input.value as IContextText;
            PendingActionManager.Instance.OpenUrl(text.GetText());
        }

        protected override void UpdateText()
        {
            DebugButtonTypeGeneral[] types = (DebugButtonTypeGeneral[])System.Enum.GetValues(typeof(DebugButtonTypeGeneral));
            for (int i = 0; i < types.Length; ++i)
            {
                DebugButtonTypeGeneral type = types[i];

                if (!elementNameDict.ContainsKey(type))
                    continue;

                ContextElement element = ContextUtils.FindElement(contentsElement, elementNameDict[type] + "/Text", ContextSearchingType.FullNameSearch);
                if (element is IContextText textElement && buttonTextDict.ContainsKey(type))
                {
                    if (type == OPEN_APP_SETTINGS)
                    {
                        bool state = NativeHelper.Instance.GetPushNotificationSubscribed();

                        string str = buttonTextDict[OPEN_APP_SETTINGS][1];
                        if (!state) str = buttonTextDict[OPEN_APP_SETTINGS][0];

                        textElement.SetText(str);
                    }
                    else
                    {
                        int state = buttonTextDict[type].Length == 1 ?
                            0 : PlayerPrefs.GetInt(playerPrefsKeyDict[type], 0);
                        textElement.SetText(buttonTextDict[type][state]);
                    }
                }
            }
        }
    }
}
