using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public class LobbyBottomController : MonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType CHILDRENDEEP = ContextSearchingType.ChildrenDeepSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private bool isInit = false;
        private int safetyShopFlag = 2;
        private string contextID;

        private List<Blackboard> bottomIconList;

        private ContextElement mainIconArea;
        private ContextElement extendIconArea;
        private ContextElement buttonArea;
        private ContextElement leftIconArea;
        private ContextElement rightIconArea;
        private ContextElement badgeIcon;
        private ContextElement BonusButtonElement;

        public void Init()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            NewInitContext();
            NewInitClickEvents();

            bottomIconList = BlackboardUtils.FindVariable<List<Blackboard>>("/bottomIconList")?.value;
            if (bottomIconList.Count == 0) return;

            contextID = BiEventUtils.GenerateContextID();
            BlackboardUtils.SetOrCreateValue(bb, "contextID", contextID);

            InitContext();
            InitClickEvents();
            CreateSideLobbyIcons();
            CreateBottomLobbyIcons();
            UpdateBadge();

            isInit = true;
        }

        private void InitContext()
        {
            mainIconArea = ContextUtils.FindElement(root, "Horizontal Group 1", CHILDREN);
            extendIconArea = ContextUtils.FindElement(root, "Horizontal Group 2", CHILDREN);
            buttonArea = ContextUtils.FindElement(root, "Button Area", CHILDREN);

            leftIconArea = ContextUtils.FindElement(root, "Button Club Area", CHILDREN);
            rightIconArea = ContextUtils.FindElement(root, "Button Daily Spin Area", CHILDREN);

            badgeIcon = ContextUtils.FindElement(root, "Badge", CHILDREN);

            
        }

        private void NewInitContext()
        {
            BonusButtonElement = ContextUtils.FindElement(root, "Right/Button Area/Button Lobby Bonus", ContextSearchingType.FullNameSearch);
        }

        private void InitClickEvents()
        {
            MetaContextElementUtils.SimpleSetClickable(root, "Button Area/Button More",
                () =>
                {
                    anim.SetBool("Appear", true);
                    BIClientClickButtonLobbyBottom("lobby_bottom_area_opened", contextID);
                }, true, FULL);
            MetaContextElementUtils.SimpleSetClickable(root, "Button Area/Button Close",
                () =>
                {
                    anim.SetBool("Appear", false);
                    BIClientClickButtonLobbyBottom("lobby_bottom_area_closed", contextID);
                    UpdateBadge();
                }, true, FULL);
            
        }

        private void NewInitClickEvents()
        {
            MetaContextElementUtils.SetClickable(BonusButtonElement, gameObject, MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_ENTER_JACKPOT_RECORD, true, true);
        }

        private void CreateSideLobbyIcons()
        {
            MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Button Club", leftIconArea.transform);
            MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Button Daily Spin", rightIconArea.transform);
        }

        private void CreateBottomLobbyIcons()
        {
            MetaContextElementUtils.SetActive(buttonArea, bottomIconList.Count == 14);

            for (int i = 0; i < bottomIconList.Count; i++)
            {
                var type = bottomIconList[i].GetValue<BottomIconType>("type");
                CreateBottomLobbyIcon(type, i >= 7);
            }
        }

        private void CreateBottomLobbyIcon(BottomIconType type, bool isExtend)
        {
            var area = isExtend ? extendIconArea : mainIconArea;

            var assetName = GetLobbyIconAssetName(type);
            var obj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, assetName, area.transform);
            var objBB = obj?.GetComponent<Blackboard>();

            switch (type)
            {
                case BottomIconType.BOSS_RAIDERS:
                case BottomIconType.CLUB_ARENA:
                case BottomIconType.HOG:
                case BottomIconType.LUCKY_FIVE:
                case BottomIconType.COLLECTING_GAME:
                case BottomIconType.EPIC_PASS:
                case BottomIconType.GOLD_TOWER:
                    BlackboardUtils.SetOrCreateValue<BottomIconType>(objBB, "targetType", type);
                    BlackboardUtils.SetOrCreateValue<bool>(objBB, "ignoreSpeechBalloon", true);
                    break;

                case BottomIconType.BUILD_DREAM:
                case BottomIconType.VIP_LOUNGE:
                    if (VipLounge.VipLounge.Utils.IsEnded)
                    {
                        BlackboardUtils.SetOrCreateValue<BottomIconType>(objBB, "targetType", type);
                    }
                    else
                    {
                        if (type == BottomIconType.VIP_LOUNGE)
                        {
                            BlackboardUtils.SetOrCreateValue<BottomIconType>(objBB, "targetType", BottomIconType.BUILD_DREAM);
                        }
                        else
                        {
                            BlackboardUtils.SetOrCreateValue<BottomIconType>(objBB, "targetType", BottomIconType.VIP_LOUNGE);
                        }
                    }
                    BlackboardUtils.SetOrCreateValue<bool>(objBB, "ignoreSpeechBalloon", true);
                    break;

                case BottomIconType.CHALLENGE:
                    if (!IsUniqueChallenge())
                        BlackboardUtils.SetOrCreateValue<bool>(objBB, "ignoreEventChallenge", true);
                    break;
            }
        }

        private string GetLobbyIconAssetName(BottomIconType type)
        {
            switch (type)
            {
                case BottomIconType.BOSS_RAIDERS:
                case BottomIconType.CLUB_ARENA:
                case BottomIconType.HOG:
                case BottomIconType.VIP_LOUNGE:
                case BottomIconType.BUILD_DREAM:
                case BottomIconType.LUCKY_FIVE:
                case BottomIconType.COLLECTING_GAME:
                case BottomIconType.EPIC_PASS:
                case BottomIconType.GOLD_TOWER:
                    if (IsValidMeta(type))
                        return "Button Lobby Meta Icon";
                    else
                        return GetSafetyShop();
                case BottomIconType.CHALLENGE:
                    return "Button Lobby Challenge Anchor";
                case BottomIconType.EVENT_CHALLENGE:
                    if (IsUniqueChallenge())
                        return "Button Lobby Challenge Anchor";
                    else
                        return "Button Lobby Challenge Event Anchor";
                case BottomIconType.EPIC_ALBUM:
                    return "Button Lobby Epic Album";
                case BottomIconType.ONLINE_PLAYERS:
                    return "Button Lobby Online Players";
                case BottomIconType.COIN_SHOP:
                    return "Button Lobby Coin Shop";
                case BottomIconType.GEM_SHOP:
                    return "Button Lobby Gem Shop";
                case BottomIconType.DEAL_SHOP:
                    return "Button Lobby Shop Deal";
                case BottomIconType.WALL_OF_EPICS:
                    return "Button Lobby Wall of Epic";
                case BottomIconType.RANKING:
                    return "Button Lobby Ranking";
                case BottomIconType.VIP_DEAL:
                    return "Button Lobby VIP Deal";
                case BottomIconType.CHAT:
                    return "Button Lobby Chat";
                case BottomIconType.EARLY_ACCESS:
                    return "Button Lobby Early Access";
                case BottomIconType.FRIENDS:
                    return "Button Lobby Friend";
                case BottomIconType.TIME_BONUS:
                    return "Button Time Bonus";
                case BottomIconType.INBOX:
                    return "Button Lobby Inbox";
                case BottomIconType.NONE:
                    return GetSafetyShop();
                    // todo shk vip bottom icon
                default:
                    return "Button Lobby Empty";
            }
        }

        private bool IsUniqueChallenge()
        {
            var count = bottomIconList.Count(icon => icon.GetValue<BottomIconType>("type") == BottomIconType.CHALLENGE ||
                                        icon.GetValue<BottomIconType>("type") == BottomIconType.EVENT_CHALLENGE);

            return count == 1;
        }

        private bool IsValidMeta(BottomIconType type)
        {
            EventInfo eventInfo = null;
            switch (type)
            {
                case BottomIconType.BOSS_RAIDERS:
                {
                    eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo();
                    return eventInfo?.type == EventInfoType.BOSS_RAIDERS;
                }
                case BottomIconType.CLUB_ARENA:
                {
                    eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo();
                    return eventInfo?.type == EventInfoType.CLUB_ARENA;
                }
                case BottomIconType.LUCKY_FIVE:
                {
                    eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo();
                    return eventInfo?.type == EventInfoType.LUCKY_FIVE;
                }
                case BottomIconType.COLLECTING_GAME:
                {
                    eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo();
                    return eventInfo?.type == EventInfoType.COLLECTING_GAME;
                }
                case BottomIconType.EPIC_PASS:
                {
                    eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo();
                    return eventInfo?.type == EventInfoType.SEASON_PASS;
                }
                case BottomIconType.GOLD_TOWER:
                {
                    eventInfo = BlackboardQueryUtils.GetOtherMetaGameEventInfo();
                    return eventInfo != null;
                }
                case BottomIconType.HOG:
                {
                    return BlackboardQueryUtils.IsHiddenObjectsActive();
                }
                case BottomIconType.VIP_LOUNGE:
                {
                    return BlackboardQueryUtils.IsVipLoungeActive();
                }
                case BottomIconType.BUILD_DREAM:
                {
                    return BlackboardQueryUtils.IsVegasDreamsActive();
                }
            }

            return false;
        }

        private string GetSafetyShop()
        {
            string assetName;
            if (safetyShopFlag == 2)
            {
                if (bottomIconList.Any(icon => icon.GetValue<BottomIconType>("type") == BottomIconType.GEM_SHOP))
                    assetName = "Button Lobby Empty";
                else
                    assetName = "Button Lobby Gem Shop";
            }
            else if (safetyShopFlag == 1)
            {
                if (bottomIconList.Any(icon => icon.GetValue<BottomIconType>("type") == BottomIconType.COIN_SHOP))
                    assetName = "Button Lobby Empty";
                else
                    assetName = "Button Lobby Coin Shop";
            }
            else
            {
                if (bottomIconList.Any(icon => icon.GetValue<BottomIconType>("type") == BottomIconType.DEAL_SHOP))
                    assetName = "Button Lobby Empty";
                else
                    assetName = "Button Lobby Shop Deal";
            }
            safetyShopFlag -= 1;
            return assetName;
        }

        public void UpdateBadge()
        {
            var badgeListeners = extendIconArea.GetComponentsInChildren<LobbyBottomBadgeListener>(true);
            var badgeAvailable = badgeListeners.Any(badgeListener => badgeListener.IsBadgeAvailable());

            MetaContextElementUtils.SetActive(badgeIcon, badgeAvailable && bottomIconList.Count == 14);
        }

        private void BIClientClickButtonLobbyBottom(string buttonName, string contextId)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["button_name"] = buttonName;
            customData["context_id"] = contextId;
            Analytics.CustomEvent("client_click_button", customData);
        }

        #if UNITY_EDITOR
        [Button]
        private void TestRefresh()
        {
            UpdateBadge();
        }
        #endif
    }
}
