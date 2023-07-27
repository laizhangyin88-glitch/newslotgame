using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode.ClientModels;
using System.Collections;

namespace BagelCode
{
    public class ChallengeMissionCellController : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;

        private ContextElement buttonPlayAreaElement;

        private const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        public void InitProperty()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            buttonPlayAreaElement = ContextUtils.FindElement(root, "Button Play Area", CHILDREN);
        }

        public void UpdateMissionButton()
        {
            var missionType = BlackboardUtils.FindVariable<ChallengeMissionType>(bb, "_missionType")?.value ?? ChallengeMissionType.UNKNOWN;

            switch(missionType)
            {
                case ChallengeMissionType.CONNECT_FACEBOOK:
                    SetButtonState("POPUP_CHALLENGE_MISSION_CONNTEXT_BUTTON", "OnFacebookConnect");
                    break;
                case ChallengeMissionType.CONNECT_EMAIL:
                    SetButtonState("POPUP_CHALLENGE_MISSION_CONNTEXT_BUTTON", "OnEmailConnect");
                    break;
                case ChallengeMissionType.JOIN_CLUB:
                    SetButtonState("POPUP_CHALLENGE_MISSION_JOIN_CLUB_BUTTON", "OnJoinClub");
                    break;
                case ChallengeMissionType.PURCHASE_ANY:
                    SetButtonState("POPUP_CHALLENGE_MISSION_SHOP_BUTTON", "OnRandomShop");
                    break;
                case ChallengeMissionType.WATCH_VIDEO_ADS:
                    SetButtonState("POPUP_CHALLENGE_MISSION_ADS_BUTTON", "OnVideoAds");
                    break;
                case ChallengeMissionType.VIP_CLUB:
                    SetButtonState("POPUP_CHALLENGE_MISSION_JOIN_BUTTON", "OnVIPConnect");
                    break;
                case ChallengeMissionType.ADD_FRIENDS:
                    {
                        if (BlackboardQueryUtils.GetChallengeFriendRecommendationList().Count > 0)
                        {
                            SetButtonState("POPUP_CHALLENGE_MISSION_ADD_FRIENDS_BUTTON", "OnAddFriends");
                        }
                        else
                        {
                            SetButtonDeactive();
                        }
                    }
                    break;
                default: // todo
                    UpdateWinAnyMissionButton();
                    break;
            }
        }

        private void UpdateWinAnyMissionButton()
        {
            int gameId = BlackboardUtils.FindVariable<int>(bb, "_gameID")?.value ?? -1;

            int nowGameId = -1;

            bool isInGame = BlackboardQueryUtils.IsIngame();
            if (isInGame)
            {
                nowGameId = BlackboardUtils.FindVariable<int>(
                    MainBlackboard.Get(), "enterGameInfo/gameId")?.value ?? -1;
            }

            if (gameId == -1)
            {
                int level = BlackboardUtils.FindVariable<int>(bb, "_level")?.value ?? 0;
                int tier = BlackboardUtils.FindVariable<int>(bb, "_tier")?.value ?? 0;
                gameId = BlackboardQueryUtils.GetRandomGameID(nowGameId, level, tier);
            }

            if (gameId > 0)
            {
                var gameInfo = BlackboardQueryUtils.GetGameInfo(gameId);
                BlackboardUtils.SetOrCreateValue(bb, "_gameInfo", gameInfo);

                var slotInfo = BlackboardQueryUtils.GetSlotInfoBB(gameId, out _);
                BlackboardUtils.SetOrCreateValue(bb, "_slotInfo", slotInfo);

                // thumbnail
                bool isSlotThumbnail = BlackboardUtils.FindVariable<bool>(bb, "_isSlotThumbnail")?.value ?? false;
                if (isSlotThumbnail)
                {
                    string gameTitle = "Default";
                    int thumbNailGameID = BlackboardUtils.FindVariable<int>(bb, "_thumbNailGameID")?.value ?? 0;
                    if (thumbNailGameID > 0)
                        gameTitle = BlackboardUtils.FindVariable<string>(gameInfo, "gameTitle")?.value;

                    string bundle = "slotthumb1";
                    string asset = StringTableUtils.GetString(GLOBAL, "SLOT_THUMBNAIL_NORMAL", gameTitle);
                    Transform parent = root.transform.Find("Anchor/Mission/Thumbnail Area");

                    var prefab = AssetBundleManager.LoadAsset<GameObject>(bundle, asset);
                    if (prefab != null)
                    {
                        GameObject go = GameObject.Instantiate(prefab) as GameObject;
                        go.name = asset;
                        go.transform.SetParent(parent, false);
                        BlackboardUtils.SetOrCreateValue(bb, "_missionIconObj", go);
                    }
                }
            }

            bool usePlayButton = BlackboardUtils.FindVariable<bool>(bb, "_usePlayButton")?.value ?? false;
            if (usePlayButton && gameId > -1)
            {
                string eventName = "OnPlay";
                if (isInGame)
                {
                    int thumbNailGameID = BlackboardUtils.FindVariable<int>(bb, "_thumbNailGameID")?.value ?? 0;
                    if (gameId == nowGameId || thumbNailGameID < 0) eventName = "OnClose";
                }

                SetButtonState("POPUP_CHALLENGE_MISSION_PLAY_BUTTON", eventName);
            }
            else
            {
                SetButtonDeactive();
            }
        }

        private void SetButtonDeactive()
        {
            MetaContextElementUtils.SetActive(buttonPlayAreaElement, false);
        }

        private void SetButtonState(string key, string eventName = "", string args = "")
        {
            MetaContextElementUtils.SetActive(buttonPlayAreaElement, true);

            MetaContextElementUtils.SimpleSetTextGlobal(buttonPlayAreaElement,
                "Button Play/Text", key, FULL, args);

            MetaContextElementUtils.SimpleSetClickable(buttonPlayAreaElement,
                "Button Play", gameObject, EventSender.ON_CUSTOM_EVENT, eventName, false, true, CHILDREN);
        }
    }
}
