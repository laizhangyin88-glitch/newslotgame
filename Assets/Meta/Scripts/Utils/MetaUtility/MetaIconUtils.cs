using UnityEngine;
using System;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class MetaIconUtils
    {
        private static string CLUB_DEFAULT_SYMBOL = "Club Symbol 01";
        private static string CLUB_DEFAULT_TIER_ICON = "Club Tier Icon 01";
        private static string CLUB_DEFAULT_TIER_ICON_SMALL = "Club Tier Icon Small 01";
        private static string CLUB_DEFAULT_TIER_SMALL_BADGE = "Club Tier Icon Small Badge 01";

        public const string SLOT_THUMBNAIL_BIG = "SLOT_THUMBNAIL_BIG";
        public const string SLOT_THUMBNAIL_SMALL = "SLOT_THUMBNAIL_SMALL";

        public static GameObject MakeChallengeImageIcon(ChallengeType challengeType, Transform root, string parentName)
        {
            string assetName = "";

            switch (challengeType)
            {
                case ChallengeType.DAILY:
                    assetName = "Image Challenge Daily";
                    break;
                case ChallengeType.EXPERT:
                    assetName = "Image Challenge Expert";
                    break;
                case ChallengeType.MASTER:
                    assetName = "Image Challenge Master";
                    break;
                case ChallengeType.EVENT:
                    assetName = "Image Challenge Event Personal";
                    break;
            }

            return MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, assetName, root, parentName);
        }

        public static GameObject MakeChallengeMissionIconObject(Blackboard missionInfo, int gameID, Transform root, string parentName)
        {
            if (missionInfo == null) return null;

            ChallengeMissionType missionType = BlackboardUtils.FindVariable<ChallengeMissionType>(missionInfo, "missionType").value;

            string assetName = "";

            switch (missionType)
            {
                case ChallengeMissionType.WIN_ANY:
                    assetName = "Challenge Icon Spin Count";
                    break;
                case ChallengeMissionType.SPIN_ANY:
                    if (gameID == -1) assetName = "Challenge Icon Spin";
                    break;
                case ChallengeMissionType.WIN_BIG_WIN_ANY:
                    {
                        string winType = BlackboardUtils.FindVariable<string>(missionInfo, "winType").value;
                        assetName = string.Format("Challenge Icon Get {0} Win", BlackboardQueryUtils.GetWinText(winType));
                    }
                    break;
                case ChallengeMissionType.ENTER_FREE_SPIN_ANY:
                    if (gameID == -1) assetName = "Challenge Icon Bonus Game";
                    break;
                // case ChallengeMissionType.SPIN_WITH_MAX_BET_ANY:
                //     assetName = "Challenge Icon Max Bet";
                //     break;
                case ChallengeMissionType.WIN_CREDIT_MORE_THAN_ANY:
                    assetName = "Challenge Icon Spin Win";
                    break;
                case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_ANY:
                    assetName = "Challenge Icon Spin Win";
                    break;
                case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_ANY:
                    {
                        string winType = BlackboardUtils.FindVariable<string>(missionInfo, "winType").value;
                        assetName = string.Format("Challenge Icon Total {0} Win", BlackboardQueryUtils.GetWinText(winType));
                    }
                    break;
                case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_ANY:
                    if (gameID == -1) assetName = "Challenge Icon Bonus Game";
                    break;
                // case ChallengeMissionType.ADD_FRIENDS:
                //     assetName = "Challenge Icon Add Friend";
                //     break;
                // case ChallengeMissionType.SEND_GIFTS:
                //     assetName = "Challenge Icon Send Coin";
                //     break;
                // case ChallengeMissionType.COLLECT_GIFTS:
                //     assetName = "Challenge Icon Get Coin";
                //     break;
                // case ChallengeMissionType.SEND_LIKES:
                //     assetName = "Challenge Icon Like";
                //     break;
                // case ChallengeMissionType.RECEIVE_LIKES:
                //     assetName = "Challenge Icon Like";
                //     break;
                case ChallengeMissionType.COLLECT_TIMEBONUS:
                    assetName = "Challenge Icon Collect Time Bonus";
                    break;
                case ChallengeMissionType.COLLECT_LUCKY_SPINS:
                    assetName = "Challenge Icon Lucky Spin";
                    break;
                // case ChallengeMissionType.ENTER_NTH_ROUND_TOURNAMENT:
                //     assetName = "Challenge Icon Get Round";
                //     break;
                case ChallengeMissionType.PURCHASE_ANY:
                    assetName = "Challenge Icon Purchase";
                    break;
                case ChallengeMissionType.PURCHASE_PRICE_MORE_THAN:
                    assetName = "Challenge Icon Purchase";
                    break;
                case ChallengeMissionType.ACHIEVE_TOTAL_PURCHASE_PRICE:
                    assetName = "Challenge Icon Purchase";
                    break;
                case ChallengeMissionType.WATCH_VIDEO_ADS:
                    assetName = "Challenge Icon Ad";
                    break;
                case ChallengeMissionType.CONNECT_FACEBOOK:
                    assetName = "Challenge Icon Facebook";
                    break;
                case ChallengeMissionType.ADD_FRIENDS:
                    assetName = "Challenge Icon Friend";
                    break;
                case ChallengeMissionType.JOIN_CLUB:
                    assetName = "Challenge Icon Epic Club";
                    break;
                case ChallengeMissionType.CONNECT_EMAIL:
                    assetName = "Challenge Icon Email";
                    break;
                case ChallengeMissionType.VIP_CLUB:
                    assetName = "Challenge Icon Vip Club";
                    break;
                case ChallengeMissionType.USE_GEM_MORE_THAN:
                    assetName = "Challenge Icon Gem";
                    break;
                case ChallengeMissionType.ACHIEVE_TOTAL_WAGER_CREDIT_ANY:
                    assetName = "Challenge Icon Bet";
                    break;
                case ChallengeMissionType.ACHIEVE_TOTAL_WAGER_CREDIT_ANY_WITH_BET_LIMIT:
                    assetName = "Challenge Icon Bet";
                    break;
                case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_ANY_WITH_BET_LIMIT:
                    assetName = "Challenge Icon Spin Win";
                    break;
                case ChallengeMissionType.SPIN_WITH_BET_LIMIT:
                    assetName = "Challenge Icon Spin";
                    break;
                case ChallengeMissionType.HIDDEN_UNIVERSE_COLLECT_FINDERS:
                    assetName = "Challenge Icon Hidden Universe";
                    break;
                case ChallengeMissionType.GEM_JACKPOT_SPIN:
                    assetName = "Challenge Icon Gold Tower";
                    break;
                case ChallengeMissionType.LUCKY_FIVE_COLLECT_CARDS:
                case ChallengeMissionType.LUCKY_FIVE_COLLECT_WILD_CARDS:
                    assetName = "Challenge Icon Lucky 5";
                    break;
                case ChallengeMissionType.COLLECTING_GAME_COLLECT_CHESTS:
                case ChallengeMissionType.COLLECTING_GAME_SCRATCH_SCRATCHER:
                    {
                        int collectingGameId = BlackboardUtils.GetOrCreateVariable<int>(missionInfo, "collectingGameId")?.value ?? -1;
                        if (collectingGameId > -1)
                        {
                            string gameName = TextDecoUtils.EnumTypeToText<MetaGameType>(
                                collectingGameId, TextDecoUtils.TextFormat.PASCAL_CASE, " ");

                            assetName = "Challenge Icon Scratcher " + gameName;
                        }
                    }
                    break;
                // use thumbnail types.
                // case ChallengeMissionType.WIN_BIG_WIN_TARGETED:
                //     assetName = "Challenge Icon Spin Win";
                //     break;
                // case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_TARGETED:
                //     assetName = "Challenge Icon Spin Win";
                //     break;
            }


            GameObject resultObj = null;
            if (!string.IsNullOrEmpty(assetName))
            {
                resultObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME,
                    assetName, root, parentName);
            }

            return resultObj;
        }

        public static GameObject MakeClubChallengeMissionIconObject(Blackboard missionInfo, int gameID, Transform root, string parentName)
        {
            if (missionInfo == null) return null;

            ClubChallengeMissionType missionType = BlackboardUtils.FindVariable<ClubChallengeMissionType>(missionInfo, "missionType").value;

            string assetName = "";

            switch (missionType)
            {
                case ClubChallengeMissionType.WIN_ANY:
                    assetName = "Challenge Icon Spin Count";
                    break;
                case ClubChallengeMissionType.SPIN_ANY:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_ANY:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_FREE_SPIN_ANY:
                    if (gameID == -1) assetName = "Challenge Icon Spin";
                    break;
                case ClubChallengeMissionType.WIN_BIG_WIN_ANY:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_BIG_WIN_ANY:
                    {
                        string winType = BlackboardUtils.FindVariable<string>(missionInfo, "winType").value;
                        assetName = string.Format("Challenge Icon Get {0} Win", BlackboardQueryUtils.GetWinText(winType));
                    }
                    break;
                case ClubChallengeMissionType.ENTER_FREE_SPIN_ANY:
                    if (gameID == -1) assetName = "Challenge Icon Bonus Game";
                    break;
                case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_ANY:
                    assetName = "Challenge Icon Spin Win";
                    break;
                case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_ANY:
                    {
                        string winType = BlackboardUtils.FindVariable<string>(missionInfo, "winType").value;
                        assetName = string.Format("Challenge Icon Total {0} Win", BlackboardQueryUtils.GetWinText(winType));
                    }
                    break;
                case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_ANY:
                    if (gameID == -1) assetName = "Challenge Icon Bonus Game";
                    break;
            }

            return MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, assetName, root, parentName);
        }

        public static GameObject MakeCommonRewardIconObject(RewardType rewardType, Transform root, string parentName)
        {
            string assetName = "";

            switch (rewardType)
            {
                case RewardType.CREDIT:
                case RewardType.CREDIT_WITH_MULTIPLIER:
                    assetName = "Icon Coin";
                    break;
                case RewardType.RP:
                    assetName = "Icon Vip Point";
                    break;
                case RewardType.DAILY_BONUS_WHEEL_SPIN:
                    assetName = "Icon Wheel";
                    break;
                case RewardType.GAME_SPIN:
                case RewardType.GAME_DEAL:
                case RewardType.GAME_PLAY:
                    assetName = "Icon Free Spin";
                    break;
                case RewardType.DAILY_BOOST:
                    assetName = "Icon Daily Boost";
                    break;
            }

            return MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, assetName, root, parentName);
        }

        public static GameObject MakeCommonRewardImageObject(RewardType rewardType, Transform root, string parentName, Blackboard rewardInfo, RewardCheckScene checkScene = RewardCheckScene.DEFAULT)
        {
            string assetName = MetaCommonRewardUtils.GetRewardImageAssetName(rewardType, rewardInfo, checkScene);
            return MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, assetName, root, parentName);
        }

        public static GameObject MakeJackpotIconObject(JackpotAssetType jackpotAssetType, Transform root, string parentName)
        {
            string assetName = "";

            if (jackpotAssetType == JackpotAssetType.UNKNOWN || jackpotAssetType == JackpotAssetType.NONE)
                return null;
            else
                assetName = jackpotAssetType.ToString();

            if (!string.IsNullOrEmpty(assetName))
            {
                bool isError = false;
                // SLOT_JACKPOT_ICON
                assetName = StringTableUtils.GetString(StringTable.StringTableType.Global, "SLOT_JACKPOT_ICON", out isError, assetName);
            }

            return MetaObjectUtils.MakePrefab("slotthumb1", assetName, root, parentName);
        }

        public static GameObject MakeSlotThumbnailIconObject(int gameID, Transform root, string parentName)
        {
            string gameTitle = BlackboardQueryUtils.GetGameTitle(gameID);

            if (string.IsNullOrEmpty(gameTitle)) return null;

            return MakeSlotThumbnailIconObjectFromGameTitle(gameTitle, root, parentName);
        }

        // Thumbnail. not slot big/small
        public static GameObject MakeSlotThumbnailIconObjectFromGameTitle(string gameTitle, Transform root, string parentName)
        {
            bool error = false;
            string assetName = StringTableUtils.GetString(StringTable.StringTableType.Global, "SLOT_THUMBNAIL_NORMAL", out error, gameTitle);

            GameObject go = MakeSlotThumbnailIconObject(assetName, root, parentName);

            if (go == null)
            {
                assetName = StringTableUtils.GetString(StringTable.StringTableType.Global, "SLOT_THUMBNAIL_NORMAL", out error, "DEFAULT");
                go = MakeSlotThumbnailIconObject(assetName, root, parentName);
            }

            return go;
        }

        public static GameObject MakeSlotThumbnailEAIconObjectFromGameTitle(string gameTitle, Transform root, string parentName)
        {
            bool error = false;
            string assetName = StringTableUtils.GetString(StringTable.StringTableType.Global, "SLOT_THUMBNAIL_EA", out error, gameTitle);

            GameObject go = MakeSlotThumbnailIconObject(assetName, root, parentName);

            if (go == null)
            {
                return MakeSlotThumbnailIconObjectFromGameTitle(gameTitle, root, parentName);
            }

            return go;
        }

        // Thumbnail. not slot big / small
        public static GameObject MakeSlotThumbnailIconObject(string assetName, Transform root, string parentName)
        {
            string bundleName = "slotthumb1";
            return MetaObjectUtils.MakePrefab(bundleName, assetName, root, parentName);
        }

        // Using for lobby SlotList.
        public static GameObject MakeSlotImageObjectFromGameTitle(string gameTitle, bool isLong, bool useBG, Transform root, string parentName)
        {
            string stringImageKey = isLong ? SLOT_THUMBNAIL_BIG : SLOT_THUMBNAIL_SMALL;
            string assetName = StringTableUtils.GetString(StringTable.StringTableType.Global, stringImageKey, gameTitle);

            return MakeSlotImageIcon(assetName, gameTitle, useBG, root, parentName);
        }

        private static GameObject MakeSlotImageIcon(string assetName, string gameTitle, bool useBG, Transform root, string parentName)
        {
          //  string builtInAssetName = ProductSettings.Instance.GetThumbnailName(assetName);
            string builtInAssetName = null;
            GameObject go = null;
            if( string.IsNullOrEmpty(builtInAssetName) )
            {
                // Make Web Image Icon
                // AssetName to url.
                string imageURL = BlackboardQueryUtils.GetGameWebImageURL(assetName);
                go = MakeWebImageIconObject(imageURL, useBG, root, parentName);
#if DEV
                var controller = go.GetComponent<WebImageController>();
                controller.SetDebugText(gameTitle);
#endif
            }
            else
            {
                string bundleName = "slotthumb1";
                go = MetaObjectUtils.MakePrefab(bundleName, builtInAssetName, root, parentName);
            }

            return go;
        }

        private static GameObject MakeWebImageIconObject(string url, bool useBG, Transform root, string parentName)
        {
            string bundleName = "lobby0";
            string assetName = "Web Image Icon";

            var go = MetaObjectUtils.MakePrefab(bundleName, assetName, root, parentName);

            var controller = go.GetComponent<WebImageController>();
            controller.SetWebImage(url, useBG);

            return go;
        }

        public static GameObject MakeAnimSlotImageObjectFromGameTitle(string gameTitle, bool isLong, bool useBG, Transform root, string parentName)
        {
            string stringImageKey = isLong ? SLOT_THUMBNAIL_BIG : SLOT_THUMBNAIL_SMALL;
            string assetName = StringTableUtils.GetString(StringTable.StringTableType.Global, stringImageKey, gameTitle);
            assetName += " Anim";
            string bundleName = ProductSettings.Instance.GetAnimationThumbnailAssetBundleName(assetName);

            if(string.IsNullOrEmpty(bundleName)) return null;

            // Make DLC Icon
            var go = MetaObjectUtils.MakePrefab("lobby0", "Slot Image DLC", root, parentName);

            var controller = go.GetComponent<SlotImageDLCController>();
            controller.SetSlotImage(bundleName, assetName, useBG);
#if DEV
            controller.SetDebugText(gameTitle);
#endif

            return go;
        }

        public static GameObject MakeClubSymbolIconObject(string assetName, Transform root, string parentName)
        {
            GameObject go = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, assetName, root, parentName);

            if (go == null)
                go = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, CLUB_DEFAULT_SYMBOL, root, parentName);

            return go;
        }

        public static GameObject MakeClubTierIconObject(int clubTierValue, Transform root, string parentName)
        {
            string assetName = string.Format("Club Tier Icon {0:00}", clubTierValue + 1);
            GameObject go = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, assetName, root, parentName);

            if (go == null)
                go = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, CLUB_DEFAULT_TIER_ICON, root, parentName);

            return go;
        }

        public static GameObject MakeClubTierIconSmallObject(int clubTierValue, Transform root, string parentName)
        {
            string assetName = string.Format("Club Tier Icon Small {0:00}", clubTierValue + 1);
            GameObject go = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, assetName, root, parentName);

            if (go == null)
                go = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, CLUB_DEFAULT_TIER_ICON_SMALL, root, parentName);

            return go;
        }

        public static GameObject MakeClubTierSmallBadgeObject(int clubTierValue, Transform root, string parentName)
        {
            string assetName = string.Format("Club Tier Icon Small Badge {0:00}", ClubUtils.GetClubTierGroup(clubTierValue) + 1);
            GameObject go = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, assetName, root, parentName);

            if (go == null)
                go = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, CLUB_DEFAULT_TIER_SMALL_BADGE, root, parentName);

            return go;
        }

        public static GameObject MakeDailyListCellObject(int day, Transform root, string parentName)
        {
            string objectName = string.Format("Daily List Cell {0}", day);
            GameObject go = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Popup Daily Delivery Cell", root, parentName, objectName);

            ContextElement dayTextElement = ContextUtils.FindElement(go.GetComponent<ContextElement>(), "Day Text", ContextSearchingType.ChildrenSearch);
            int dayText = day + 1;
            MetaContextElementUtils.SetText(dayTextElement, dayText.ToString());

            return go;
        }

        public static GameObject MakeInboxIconObjectFromName(string assetName, Transform root, string parentName)
        {
            GameObject go = MakeInboxIconObject(assetName, root, parentName);

            if (go == null)
            {
                assetName = InboxUtils.GetInboxIconPrefabName(InboxCellData.IconType.DEFAULT);
                go = MakeInboxIconObject(assetName, root, parentName);
            }

            return go;
        }

        public static GameObject MakeInboxIconObject(string assetName, Transform root, string parentName)
        {
            return MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, assetName, root, parentName);
        }
    }
}
