using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public partial class ClubContentsController
    {
        private string metaGameName = "";
        private int metaGameThemeId = 0;
        private EventInfoType metaEventType = EventInfoType.UNKNOWN;

        private bool CreateBottomMetaIcon(string assetName, EventInfo eventInfo)
        {
            bool isCreate = false;
            if (metaButtonAreaElement.transform.childCount > 0)
                Destroy(metaButtonAreaElement.transform.GetChild(0).gameObject);

            if (!string.IsNullOrEmpty(assetName))
            {
                GameObject metaIconObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, assetName, metaButtonAreaElement.transform, null, "Meta Button");
                SetMetaGameName(eventInfo);
                if (metaIconObject != null)
                {
                    isCreate = true;
                    metaEventType = eventInfo.type;
                    metaIconObject.GetComponent<PIDButton>().onClick.AddListener(() => SendClubEvent("OnTabMetaGame"));
                }
            }
            return isCreate;
        }

        private void SendClubEvent(string eventName)
        {
            if (!string.IsNullOrEmpty(eventName))
                MetaContextElementUtils.SendEvent(root, eventName, null, null);
        }

        private void SendClubMetaUIEvent(string eventName)
        {
            MessageDispatcher.Dispatch("OnMetaUIEvent", new ParadoxNotion.EventData(eventName));
        }

        private bool CheckActiveClubMetaGame(EventInfoType eventType)
        {
            switch (eventType)
            {
                case EventInfoType.BOSS_RAIDERS:
                case EventInfoType.CLUB_ARENA:
                    return true;
                default:
                    return false;
            }
        }

        private string GetAssetNameClubMetaGame(EventInfo eventInfo)
        {
            string assetName = "";
            switch (eventInfo.type)
            {
                case EventInfoType.BOSS_RAIDERS:
                    assetName = string.Format("Club Meta Button Boss Raiders_{0}", ((EventDataBossRaiders)eventInfo.constraints).themeId);
                    break;
                case EventInfoType.CLUB_ARENA:
                    assetName = "Club Meta Button Club Arena";
                    break;
                default:
                    break;
            }
            return assetName;
        }

        private void SetMetaGameName(EventInfo eventInfo)
        {
            switch (eventInfo.type)
            {
                case EventInfoType.BOSS_RAIDERS:
                    metaGameName = StringTableUtils.GetString(StringTable.StringTableType.Global, string.Format("LOBBY_EVENT_NAME_BOSS_RAIDERS_{0}", ((EventDataBossRaiders)eventInfo.constraints).themeId));
                    break;
                case EventInfoType.CLUB_ARENA:
                    metaGameName = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_ARENA_NAME");
                    break;
                default:
                    metaGameName = "";
                    break;
            }
        }

        public void OnTabMetaGame()
        {
            if (EnterClubMetaGame() == false)
            {
                // Enter metagame faild to popup
                OpenCommonPopupAsync();
                tabMetaButtonElement.gameObject.SetActive(false);
            }
        }

        private bool EnterClubMetaGame()
        {
            EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo();
            if (eventInfo != null && CheckActiveClubMetaGame(eventInfo.type))
            {
                metaGameThemeId = eventInfo.type == EventInfoType.BOSS_RAIDERS ? ((EventDataBossRaiders)eventInfo.constraints).themeId : 0;
                string bundleName = BlackboardQueryUtils.GetMetaBundleName(eventInfo.type, false, metaGameThemeId);
                string loadingAssetName = BlackboardQueryUtils.GetMetaLoadingAssetName(eventInfo);

                SceneInfo sceneInfo = AssetBundleManager.LoadAsset<SceneInfoObject>(bundleName, loadingAssetName).GetSceneInfo();
                Transform parent = MetaPopupUtils.PopupManagerAreaTransform;
                GameObject loadingScene = SceneManager.LoadScene(parent, sceneInfo);
                if (loadingScene == null)
                {
                    Destroy(loadingScene);
                    return false;
                }

                Blackboard loadingBB = loadingScene.GetComponent<Blackboard>();
                if (loadingBB == null)
                {
                    Destroy(loadingScene);
                    return false;
                }

                string biContextId = GetContextId();

                BlackboardUtils.SetOrCreateValue<string>(loadingBB, "enterType", "club_meta");
                BlackboardUtils.SetOrCreateValue<string>(loadingBB, "_biContextID", biContextId);
                BlackboardUtils.SetOrCreateValue<bool>(loadingBB, "isEnter", true);

                PopupManager.Instance.Open(loadingScene);
                SendClubMetaUIEvent("OnEnterMetaGameFromClub");
                BI_ClientClickMetaGameIcon(eventInfo.type);
                return true;
            }

            return false;
        }

        private void OpenCommonPopupAsync()
        {
            if (!string.IsNullOrEmpty(metaGameName))
            {
                MetaPopupUtils.OpenPopupAsync(this,
                    MetaStringDefine.LOBBY_BUNDLE_NAME,
                    "Popup Common Ok Scene",
                    MetaPopupUtils.PopupManagerAreaTransform,
                    (s) =>
                    {
                        GameObject popupObject = s.GetScene();
                        Blackboard popupBB = popupObject.GetComponent<Blackboard>();

                        popupBB.SetValue("owner", gameObject.transform);
                        popupBB.SetValue("title", StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CLUB_METAGAME_OVER_TEXT", metaGameName));
                        popupBB.SetValue("buttonYesText", StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY"));
                        popupBB.SetValue("autoCloseYes", true);
                        popupBB.SetValue("autoCloseX", true);

                        PopupManager.Instance.Open(popupObject);
                        popupObject.gameObject.SetActive(true);
                        BI_ClientMetaGameOverPopup();
                    });
            }
        }

        private void BI_ClientClickMetaGameIcon(EventInfoType eventInfoType)
        {
            if (CheckActiveClubMetaGame(eventInfoType))
            {
                // Send BI
                switch (eventInfoType)
                {
                    case EventInfoType.BOSS_RAIDERS:
                        BossRaidersUtils.BIClientClickBossRaidersIcon(GetContextId(), "club_meta");
                        break;
                    case EventInfoType.CLUB_ARENA:
                        ClubArenaUtils.BIClientClickClubArenaIcon(GetContextId(), "club_meta");
                        break;
                }

            }
        }

        private void BI_ClientMetaGameOverPopup()
        {
            switch (metaEventType)
            {
                case EventInfoType.BOSS_RAIDERS:
                    BIClientClubBossRaidersResultPopup();
                    break;
                case EventInfoType.CLUB_ARENA:
                    BIClientClubArenaResultPopup();
                    break;
            }
        }

        private void BIClientClubBossRaidersResultPopup()
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["type"] = "within_club";
            customData["theme_id"] = metaGameThemeId;
            Analytics.CustomEvent("client_club_boss_raiders_result_popup", customData);
        }

        private void BIClientClubArenaResultPopup()
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["type"] = "within_club";
            Analytics.CustomEvent("client_club_arena_result_popup", customData);
        }
    }
}