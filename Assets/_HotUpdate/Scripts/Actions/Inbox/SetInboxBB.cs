using System;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using BagelCode.ClientModels;
using System.Collections.Generic;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Inbox")]
    public class SetInboxBB : ActionTask
    {
        public BBParameter<Blackboard>  inboxInfoBB;

        [BlackboardOnly]
        public BBParameter<string>  saveButtonText;

        [BlackboardOnly]
        public BBParameter<string>  saveIconText;

        [BlackboardOnly]
        public BBParameter<string>  saveMessage;

        public BBParameter<int>     saveGameID;
        public BBParameter<int>    bonusTag;
        public BBParameter<string>  saveExpireText;

        public BBParameter<bool>    isInboxBanner;

        public BBParameter<IconType> iconType;
        public BBParameter<string>  imageUrl;
        public BBParameter<InboxBannerTypes> inboxBannerType;
        public BBParameter<InboxTypes> inboxType;

        public BBParameter<Color> defaultColor;
        public BBParameter<Color> defaultGradientColor;
        public BBParameter<Color> warningColor;
        public BBParameter<Color> warningGradientColor;

        public BBParameter<int> saveAsEventID;
        public BBParameter<long> saveAsEventNumerator;
        
        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;
        private Variable<string>            title;
        private Variable<string>            message;

        private Dictionary<string, string>  stringFormatDict = new Dictionary<string, string>();

        private const string EXPIRE_EMPTY = " ";

        private bool isInit = false;
        private ContextElement rootElement;

        private ContextElement messageElement;
        private ContextElement yellowButtonElement;
        private ContextElement greenButtonElement;

        private ContextElement cellBaseElement;
        private ContextElement cellGradientElement;

        public enum InboxButtonType
        {
            GREEN = 0,
            YELLOW = 1,
            UNKNOWN,
        };
        
        private InboxButtonType buttonType = InboxButtonType.YELLOW;

        protected override string info
        {
            get { return "Set Inbox BB"; }
        }

        public enum IconType
        {
            CREDIT,
            RP,
            DAILY_BOOST,
            EMPTY,
            PURCHASE_COUPON,
            FACEBOOK,
            EMAIL,
            FACEBOOK_FRIEND_CONNECT,
            FACEBOOK_SHARE,
            PURCHASE_RECOVER_CREDIT,
            PURCHASE_RECOVER_RP,
            GAME_THUMBNAIL,
            TIER_UPGRADE,
            DAILY_BONUS_WHEEL_SPIN,
            TOURNAMENT_WIN,
            EXP_MULTIPLY,
            RANDOM,
            NEWS,
            WEB_IMAGE,
            INSTANT_BONUS,
            BUY_A_BONUS,
            SUPER_BONUS,
            DEFAULT,
            GEM,
            MEGA_WHEEL_SPIN
        }
        
        private void InitProperty()
        {
            if(isInit) return;

            rootElement = agent.gameObject.GetComponent<ContextElement>();

            messageElement = ContextUtils.FindElement(rootElement, "Text Message", ContextSearchingType.ChildrenSearch);
            yellowButtonElement = ContextUtils.FindElement(rootElement, "Button Accept", ContextSearchingType.ChildrenSearch);
            greenButtonElement = ContextUtils.FindElement(rootElement, "Button Redeem", ContextSearchingType.ChildrenSearch);

            cellBaseElement = ContextUtils.FindElement(rootElement, "Cell Base", ContextSearchingType.ChildrenSearch);
            cellGradientElement = ContextUtils.FindElement(cellBaseElement, "Cell Gradient", ContextSearchingType.ChildrenSearch);

            isInit = true;
        }

        protected override void OnExecute()
        {
            InitProperty();

            Variable<long> coin;

            var owner = agent.gameObject.GetComponent<GraphOwner>();
            inboxInfoBB.value.AddVariable("graphOwner", owner);

            title = BlackboardUtils.FindVariable<string>(inboxInfoBB.value, "title");
            message = BlackboardUtils.FindVariable<string>(inboxInfoBB.value, "message");
            saveGameID.value = -1;
            bonusTag.value = -1;
            saveExpireText.value = GetExpireText();
            isInboxBanner.value = BlackboardQueryUtils.IsInboxBanner(inboxInfoBB.value);
            iconType.value = IconType.EMPTY;
            imageUrl.value = GetImageUrl();

            saveAsEventID.value = 0;
            saveAsEventNumerator.value = NumberUtils.GetGlobalDenominator();

            buttonType = InboxButtonType.YELLOW;

            MetaContextElementUtils.SetColor(cellBaseElement, defaultColor.value);
            MetaContextElementUtils.SetColor(cellGradientElement, defaultGradientColor.value);
            
            if (!isInboxBanner.value)
            {
                Variable<InboxTypes> inboxType = BlackboardUtils.FindVariable<InboxTypes>(inboxInfoBB.value, "type");
    
                switch(inboxType.value)
                {
                    case InboxTypes.REWARD:
                        UpdateRewardEvent();
                        saveMessage.value = GetRewardMessage();
                        saveButtonText.value = GetRewardButtonText();
                        iconType.value = GetRewardIconType();
                        break;
                    case InboxTypes.MESSAGE:
                        saveMessage.value = GetTextMessage();
                        saveButtonText.value = StringTableUtils.GetString(tableType, "INBOX_ITEM_BUTTON_OK");
                        iconType.value = IconType.EMPTY;
                        break;
                    case InboxTypes.MESSAGE_WARNING:
                        saveMessage.value = GetTextMessage();
                        saveButtonText.value = StringTableUtils.GetString(tableType, "INBOX_ITEM_BUTTON_OK");
                        iconType.value = IconType.EMPTY;

                        MetaContextElementUtils.SetColor(cellBaseElement, warningColor.value);
                        MetaContextElementUtils.SetColor(cellGradientElement, warningGradientColor.value);
                        break;
                    case InboxTypes.GAME_COMPENSATION:
                        saveGameID.value = inboxInfoBB.value.GetValue<int>("gameId");
                        coin = BlackboardUtils.FindVariable<long>(inboxInfoBB.value, "credit");
                        saveMessage.value = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_COMPENSATION", title.value, message.value, coin.value);
                        saveButtonText.value = StringTableUtils.GetString(tableType, "INBOX_ITEM_BUTTON_ACCEPT");
                        iconType.value = IconType.GAME_THUMBNAIL;
                        break;
                    case InboxTypes.FACEBOOK_FRIEND_CONNECT:
                        coin = BlackboardUtils.FindVariable<long>(inboxInfoBB.value, "credit");
                        Variable<string> userName = BlackboardUtils.FindVariable<string>(inboxInfoBB.value, "userName");
                        saveMessage.value = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_FACEBOOK_FRIEND", userName.value, coin.value);
                        saveButtonText.value = StringTableUtils.GetString(tableType, "INBOX_ITEM_BUTTON_CLAIM");
                        iconType.value = IconType.FACEBOOK_FRIEND_CONNECT;
                        break;
                    case InboxTypes.FACEBOOK_SHARE:
                        coin = BlackboardUtils.FindVariable<long>(inboxInfoBB.value, "rp");
                        saveMessage.value = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_FACEBOOK_SHARE", coin.value);
                        saveButtonText.value = StringTableUtils.GetString(tableType, "INBOX_ITEM_BUTTON_CLAIM");
                        iconType.value = IconType.FACEBOOK_SHARE;
                        break;
                    case InboxTypes.TOURNAMENT_WIN:
                        saveMessage.value = GetTournamentMessage();
                        saveButtonText.value = StringTableUtils.GetString(tableType, "INBOX_ITEM_BUTTON_ACCEPT");
                        iconType.value = IconType.TOURNAMENT_WIN;
                        saveIconText.value = BlackboardUtils.FindVariable<int>(inboxInfoBB.value, "rank").value.ToString();
                        break;
                    case InboxTypes.TICKETED_BONUS_TICKET:
                    case InboxTypes.TICKETED_BONUS_TICKET_FOR_BOOST:
                        saveGameID.value = inboxInfoBB.value.GetValue<int>("gameId");
                        saveMessage.value = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_MESSAGE", title.value, message.value);
                        saveButtonText.value = StringTableUtils.GetString(tableType, "INBOX_ITEM_BUTTON_PLAY");
                        iconType.value = IconType.GAME_THUMBNAIL;
                        break;
                    case InboxTypes.SOCIAL_CREDIT:
                        {
                            // Convert Data
                            coin = BlackboardUtils.FindVariable<long>(inboxInfoBB.value, "credit");
                            var applyerTierMultiplier = BlackboardUtils.FindVariable<bool>(inboxInfoBB.value, "applyTierMultiplier");

                            // TODO ?
                            if (applyerTierMultiplier.value)
                            {
                                var tier = TierUtils.GetMeTier();
                                var tierMultiplier = TierUtils.GetTierMultiplier(tier);
                                saveMessage.value = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_COIN_WITH_TIER_MULTIPLIER", title.value, message.value, coin.value, tierMultiplier, tier);
                            }
                            else
                            {
                                saveMessage.value = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_COIN", title.value, message.value, coin.value);
                            }

                            saveMessage.value = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_COIN", title.value, message.value, coin.value);
                            saveButtonText.value = StringTableUtils.GetString(tableType, "INBOX_ITEM_BUTTON_ACCEPT");
                            iconType.value = IconType.WEB_IMAGE;
                        }
                        break;
                    default:
                        saveMessage.value = "";
                        saveButtonText.value = "";
                        iconType.value = IconType.EMPTY;
                        break;
                }

                this.inboxType.value = inboxType.value;
            }
            else
            {
                Variable<InboxBannerTypes> inboxBannerType = BlackboardUtils.FindVariable<InboxBannerTypes>(inboxInfoBB.value, "inboxBannerType");

                stringFormatDict.Clear();
                stringFormatDict["name"] = BlackboardUtils.FindVariable<string>(null, "/me/name").value;

                var gameID = BlackboardUtils.GetOrCreateVariable<int>(inboxInfoBB.value, "gameId");
                if(gameID != null && gameID.value > 0)
                {
                    saveGameID.value = gameID.value;
                    stringFormatDict["game_name"] = BlackboardQueryUtils.GetMetaGameTitle(saveGameID.value);
                    iconType.value = IconType.GAME_THUMBNAIL;
                }
                else
                {
                    var url = BlackboardUtils.GetOrCreateVariable<string>(inboxInfoBB.value, "imageUrl");

                    if (url != null && !String.IsNullOrEmpty(url.value))
                        iconType.value = IconType.WEB_IMAGE;
                }

                switch (inboxBannerType.value)
                {
                    case InboxBannerTypes.NEWS:
                        {
                            saveMessage.value = ConvertStringFormat(GetTextMessage());
                            saveButtonText.value = StringTableUtils.GetString(tableType, "INBOX_ITEM_BUTTON_READ");

                            if(iconType.value == IconType.EMPTY)
                                iconType.value = IconType.NEWS;
                        }
                        break;
                    case InboxBannerTypes.SALES:
                        {
                            saveMessage.value = ConvertStringFormat(GetTextMessage());
                            saveButtonText.value = StringTableUtils.GetString(tableType, "INBOX_ITEM_BUTTON_SHOW_ME");
                            
                            if(iconType.value == IconType.EMPTY)
                                iconType.value = IconType.DEFAULT;

                            buttonType = InboxButtonType.GREEN;
                        }
                        break;
                    default:
                        saveMessage.value = "";
                        saveButtonText.value = "";
                        iconType.value = IconType.EMPTY;
                        break;
                }
                
                this.inboxBannerType.value = inboxBannerType.value;
            }

            MetaContextElementUtils.SetText(messageElement, saveMessage.value);

            switch(buttonType)
            {
                case InboxButtonType.GREEN:
                    yellowButtonElement.gameObject.SetActive(false);
                    greenButtonElement.gameObject.SetActive(true);
                    MetaContextElementUtils.SimpleSetText(greenButtonElement, "Text", saveButtonText.value);
                    MetaContextElementUtils.SetClickable(
                        greenButtonElement,
                        "OnAccept",
                        rootElement,
                        null
                    );
                    break;
                case InboxButtonType.YELLOW:
                    yellowButtonElement.gameObject.SetActive(true);
                    greenButtonElement.gameObject.SetActive(false);
                    MetaContextElementUtils.SimpleSetText(yellowButtonElement, "Text", saveButtonText.value);
                    MetaContextElementUtils.SetClickable(
                        yellowButtonElement,
                        "OnAccept",
                        rootElement,
                        null
                    );
                    break;
            }

            EndAction();
        }

        private string ConvertTierStyleText(int tier)
        {
            int tierGroup = TierUtils.GetTierGroup(tier);
            string tierText = StringTableUtils.GetString(tableType, string.Format("TIER_{0}", tier));
            string tierStyleText = StringTableUtils.GetString(tableType, string.Format("TIER_STYLE_{0}", tierGroup), tierText);
            return tierStyleText;
        }

        private string ConvertVipStyleText(long rp)
        {
            string rpText = FormatUtility.CommaNumberFormat(rp);
            rpText = "<style=vip>" + rpText + "</style>";
            return rpText;
        }

        private string ConvertCoinStyleText(long credit)
        {
            string coinText = FormatUtility.CommaNumberFormat(credit);
            coinText = "<style=coin>" + coinText + "</style>";
            return coinText;
        }

        private string ConvertStringFormat(string str)
        {
            foreach(var item in stringFormatDict)
            {
                string stringFormat = string.Format("{{{0}}}", item.Key);
                if (str.Contains(stringFormat))
                {
                    str = str.Replace(stringFormat, item.Value);
                }
            }
            return str;
        }

        private string ConvertRankToOrdinal(int rank)
        {
            switch(rank)
            {
                case 1:
                    return "1st";
                case 2:
                    return "2nd";
                case 3:
                    return "3rd";
                default:
                    return string.Format("{0}th", rank);
            }
        }

        private string GetTextMessage()
        {
            return StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_MESSAGE", title.value, message.value);
        }

        private string GetTournamentMessage()
        {
            var rank = BlackboardUtils.FindVariable<int>(inboxInfoBB.value, "rank");
            var round = BlackboardUtils.FindVariable<int>(inboxInfoBB.value, "serialWinCount");
            var actualCredit = BlackboardUtils.FindVariable<long>(inboxInfoBB.value, "actualWinCredit");
            var serialWinBonus = BlackboardUtils.FindVariable<double>(inboxInfoBB.value, "serialWinBonus");

            return StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_TOURNAMENT_WIN", ConvertRankToOrdinal(rank.value), (round.value + 1), serialWinBonus.value, actualCredit.value);
        }

        private string GetRewardMessage()
        {
            stringFormatDict.Clear();
            stringFormatDict["name"] = BlackboardUtils.FindVariable<string>(null, "/me/name").value;

            Variable<RewardType> rewardType = BlackboardUtils.FindVariable<RewardType>(inboxInfoBB.value, "reward/type");
            Variable<RewardCauseType> rewardCauseType = BlackboardUtils.FindVariable<RewardCauseType>(inboxInfoBB.value, "rewardCauseType");

            string rewardMessage = "";

            switch(rewardType.value)
            {
                case RewardType.CREDIT:
                case RewardType.CREDIT_WITH_MULTIPLIER:
                    {
                        Variable<long> coin = BlackboardUtils.FindVariable<long>(inboxInfoBB.value, "reward/credit");
                        var applyerTierMultiplier = BlackboardUtils.FindVariable<bool>(inboxInfoBB.value, "reward/applyTierMultiplier");
                        if (rewardCauseType.value == RewardCauseType.PURCHASE_RECOVER)
                        {
                            rewardMessage = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_PURCHASE_COIN", title.value, message.value, coin.value);
                        }
                        else
                        {
                            if(applyerTierMultiplier.value)
                            {
                                var tier = TierUtils.GetMeTier();
                                var tierMultiplier = TierUtils.GetTierMultiplier(tier);
                                rewardMessage = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_COIN_WITH_TIER_MULTIPLIER", title.value, message.value, coin.value, tierMultiplier, tier);
                            }
                            else
                            {
                                rewardMessage = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_COIN", title.value, message.value, coin.value);
                            }
                        }
                    }
                    break;
                case RewardType.RP:
                    {
                        Variable<long> rp = BlackboardUtils.FindVariable<long>(inboxInfoBB.value, "reward/rp");
                        if (rewardCauseType.value == RewardCauseType.PURCHASE_RECOVER)
                        {
                            rewardMessage = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_PURCHASE_RP", title.value, message.value, rp.value);
                        }
                        else
                        {
                            rewardMessage = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_RP", title.value, message.value, rp.value);
                        }
                    }
                    break;
                case RewardType.DAILY_BOOST:
                    {
                        var tier = TierUtils.GetMeTier();
                        var baseCoins = BlackboardUtils.FindVariable<long>(inboxInfoBB.value, "reward/baseCreditPerDay");
                        long totalCoins = TierUtils.GetTierFractionCoin(baseCoins.value, tier);
                        totalCoins = LevelUtils.GetLevelMultiplierDailyBoostCoinNumeratorValue(totalCoins, BlackboardUtils.FindValue<Blackboard>(inboxInfoBB.value, "reward"), "dailyBoost");
                        var totalDayCount = BlackboardUtils.FindVariable<int>(inboxInfoBB.value, "reward/totalDayCount");

                        stringFormatDict["credit"] = ConvertCoinStyleText(totalCoins);
                        stringFormatDict["day"] = totalDayCount.value.ToString();

                        rewardMessage = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_MESSAGE", title.value, message.value);
                    }
                    break;
                case RewardType.TIER_UPGRADE:
                    {
                        var targetTier = BlackboardUtils.FindVariable<int>(inboxInfoBB.value, "reward/targetTier");

                        stringFormatDict["tier"] = ConvertTierStyleText(targetTier.value);

                        rewardMessage = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_MESSAGE", title.value, message.value);
                    }
                    break;
                case RewardType.DAILY_BONUS_WHEEL_SPIN:
                    {
                        var spinCount = BlackboardUtils.FindVariable<int>(inboxInfoBB.value, "reward/spinCount");

                        stringFormatDict["spin_count"] = spinCount.value.ToString();

                        rewardMessage = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_MESSAGE", title.value, message.value);
                    }
                    break;
                case RewardType.MEGA_WHEEL_SPIN:
                    {
                        var spinCount = BlackboardUtils.FindVariable<int>(inboxInfoBB.value, "reward/spinCount");

                        stringFormatDict["spin_count"] = spinCount.ToString();

                        rewardMessage = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_MESSAGE", title.value, message.value);
                    }
                    break;
                case RewardType.GAME_SPIN:
                    {
                        saveGameID.value = BlackboardUtils.FindVariable<int>(inboxInfoBB.value, "reward/gameId").value;
                        var spinCount = BlackboardUtils.FindVariable<int>(inboxInfoBB.value, "reward/spinCount");
                        var bet = BlackboardUtils.FindVariable<long>(inboxInfoBB.value, "reward/bet");
                        var totalBet = BlackboardUtils.FindVariable<long>(inboxInfoBB.value, "reward/totalBet");

                        string gameTitleFormat = string.Format("GAME_TITLE_{0}", saveGameID.value.ToString());
                        string gameTitle = StringTableUtils.GetString(tableType, gameTitleFormat);

                        stringFormatDict["bet"] = ConvertCoinStyleText(bet.value);
                        stringFormatDict["total_bet"] = ConvertCoinStyleText(totalBet.value);
                        stringFormatDict["spin_count"] = spinCount.value.ToString();
                        stringFormatDict["game_name"] = gameTitle;

                        rewardMessage = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_MESSAGE", title.value, message.value);
                    }
                    break;
                case RewardType.GAME_DEAL:
                    {
                        saveGameID.value = BlackboardUtils.FindVariable<int>(inboxInfoBB.value, "reward/gameId").value;
                        var dealCount = BlackboardUtils.FindVariable<int>(inboxInfoBB.value, "reward/count");
                        var betPerHand = BlackboardUtils.FindVariable<long>(inboxInfoBB.value, "reward/betPerHand");
                        var handCount = BlackboardUtils.FindVariable<int>(inboxInfoBB.value, "reward/handCount");

                        string gameTitleFormat = string.Format("GAME_TITLE_{0}", saveGameID.value.ToString());
                        string gameTitle = StringTableUtils.GetString(tableType, gameTitleFormat);

                        stringFormatDict["bet_per_hand"] = ConvertCoinStyleText(betPerHand.value);
                        stringFormatDict["deal_count"] = dealCount.value.ToString();
                        stringFormatDict["hand_count"] = handCount.value.ToString();
                        stringFormatDict["game_name"] = gameTitle;

                        rewardMessage = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_MESSAGE", title.value, message.value);
                    }
                    break;
                case RewardType.PURCHASE_COUPON:
                    {
                        var tier = TierUtils.GetMeTier();
                        var item = BlackboardQueryUtils.GetItemFromProduct(BlackboardUtils.FindVariable<Blackboard>(inboxInfoBB.value, "reward/product").value, ItemType.CREDIT);
                        var baseCoins = item.GetValue<long>("baseCredit");
                        var tierMultipliedCoins = TierUtils.GetTierFractionCoin(baseCoins, tier);
                        tierMultipliedCoins = LevelUtils.GetLevelMultiplierNumeratorValue(tierMultipliedCoins, "coin");
                        var eventMultiplierNumerator = BlackboardUtils.FindVariable<long>(inboxInfoBB.value, "reward/product/eventMultiplierNumerator").value;

                        if(saveAsEventID.value != 0)
                            eventMultiplierNumerator += saveAsEventNumerator.value - NumberUtils.GetGlobalDenominator();

                        stringFormatDict["price"] = string.Format("${0}", BlackboardUtils.FindVariable<double>(inboxInfoBB.value, "reward/product/price").value.ToString());
                        stringFormatDict["credit"] = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_PURCHASE_COUPON_COIN", NumberUtils.GetMultiplierNumeratorValue(tierMultipliedCoins, eventMultiplierNumerator));
                        stringFormatDict["rp"] = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_PURCHASE_COUPON_RP", item.GetValue<long>("rp"));
                        
                        rewardMessage = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_MESSAGE", title.value, message.value);
                        break;
                    }
                case RewardType.EXP_MULTIPLY:
                case RewardType.EXP_MULTIPLY_EXTENDABLE:
                    rewardMessage = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_MESSAGE", title.value, message.value);
                    break;
                case RewardType.RANDOM:
                    rewardMessage = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_MESSAGE", title.value, message.value);
                    break;
                case RewardType.TICKETED_BONUS_TICKET:
                case RewardType.TICKETED_BONUS_TICKET_FOR_BOOST:
                    {
                        bonusTag.value = (int)BlackboardUtils.FindVariable<BonusTag>(inboxInfoBB.value, "reward/tag").value;
                        saveGameID.value = BlackboardUtils.FindVariable<int>(inboxInfoBB.value, "reward/gameId").value;
                        var baseBet = BlackboardUtils.FindVariable<long>(inboxInfoBB.value, "reward/baseBet");
                        var extraBet = BlackboardUtils.FindVariable<long>(inboxInfoBB.value, "reward/extraBet");

                        string gameTitleFormat = string.Format("GAME_TITLE_{0}", saveGameID.value.ToString());
                        string gameTitle = StringTableUtils.GetString(tableType, gameTitleFormat);

                        stringFormatDict["bet"] = ConvertCoinStyleText(baseBet.value + extraBet.value);
                        stringFormatDict["game_name"] = gameTitle;

                        rewardMessage = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_MESSAGE", title.value, message.value);
                    }
                    break;
                case RewardType.SCRATCHER:
                    {
                        var scratcherName = BlackboardUtils.FindVariable<ScratcherName>(inboxInfoBB.value, "reward/scratcherName");
                        stringFormatDict["meta_game"] = BlackboardQueryUtils.GetScratcherName(scratcherName.value);

                        var groupId = inboxInfoBB.value.GetVariable<string>("groupId");
                        if(groupId != null)
                        {
                            var groupFirstInfo = inboxInfoBB.value.GetValue<Blackboard>("groupFirstInfo");
                            int groupCount = groupFirstInfo.GetValue<int>("groupCount");
                            stringFormatDict["group_count"] = StringTableUtils.GetString(tableType, "INBOX_ITEM_SCRATCHER_GROUP", groupCount);
                        }

                        rewardMessage = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_MESSAGE", title.value, message.value);

                        break;
                    }
                case RewardType.GEM:
                {
                    Variable<long> gem = BlackboardUtils.FindVariable<long>(inboxInfoBB.value, "reward/gem");
                    rewardMessage = StringTableUtils.GetString(tableType, "INBOX_ITEM_TEXT_GEM", title.value, message.value, gem.value);   
                    break;
                }
                default:
                    break;
            }

            return ConvertStringFormat(rewardMessage);
        }

        private string GetRewardButtonText()
        {
            Variable<RewardType> rewardType = BlackboardUtils.FindVariable<RewardType>(inboxInfoBB.value, "reward/type");
            Variable<RewardCauseType> rewardCauseType = BlackboardUtils.FindVariable<RewardCauseType>(inboxInfoBB.value, "rewardCauseType");

            if (rewardCauseType.value == RewardCauseType.PURCHASE_RECOVER) {
                return StringTableUtils.GetString(tableType, "INBOX_ITEM_BUTTON_CLAIM");
            }
            switch(rewardType.value)
            {
                case RewardType.CREDIT:
                case RewardType.CREDIT_WITH_MULTIPLIER:
                    return StringTableUtils.GetString(tableType, "INBOX_ITEM_BUTTON_ACCEPT");
                case RewardType.RP:
                    return StringTableUtils.GetString(tableType, "INBOX_ITEM_BUTTON_ACCEPT");
                case RewardType.DAILY_BOOST:
                    return StringTableUtils.GetString(tableType, "INBOX_ITEM_BUTTON_BEGIN");
                case RewardType.TIER_UPGRADE:
                    return StringTableUtils.GetString(tableType, "INBOX_ITEM_BUTTON_ACCEPT");
                case RewardType.DAILY_BONUS_WHEEL_SPIN:
                case RewardType.MEGA_WHEEL_SPIN:
                    return StringTableUtils.GetString(tableType, "INBOX_ITEM_BUTTON_ACCEPT");
                case RewardType.GAME_SPIN:
                case RewardType.GAME_DEAL:
                case RewardType.TICKETED_BONUS_TICKET:
                case RewardType.TICKETED_BONUS_TICKET_FOR_BOOST:
                    return StringTableUtils.GetString(tableType, "INBOX_ITEM_BUTTON_PLAY");
                case RewardType.PURCHASE_COUPON:
                    return StringTableUtils.GetString(tableType, "INBOX_ITEM_BUTTON_REDEEM");
                case RewardType.EXP_MULTIPLY:
                case RewardType.EXP_MULTIPLY_EXTENDABLE:
                    return StringTableUtils.GetString(tableType, "INBOX_ITEM_BUTTON_BEGIN");
                case RewardType.RANDOM:
                    return StringTableUtils.GetString(tableType, "INBOX_ITEM_BUTTON_ACCEPT");
                case RewardType.SCRATCHER:
                    return StringTableUtils.GetString(tableType, "INBOX_ITEM_BUTTON_SCRATCH");
                case RewardType.GEM:
                    return StringTableUtils.GetString(tableType, "INBOX_ITEM_BUTTON_ACCEPT");
                    
                default:
                    break;
            }

            return StringTableUtils.GetString(tableType, "INBOX_ITEM_BUTTON_OK");
        }

        private IconType GetRewardIconType()
        {
            if( !string.IsNullOrEmpty(imageUrl.value) )
                return IconType.WEB_IMAGE;

            Variable<RewardType> rewardType = BlackboardUtils.FindVariable<RewardType>(inboxInfoBB.value, "reward/type");
            Variable<RewardCauseType> rewardCauseType = BlackboardUtils.FindVariable<RewardCauseType>(inboxInfoBB.value, "rewardCauseType");
            
            bool isRecover = rewardCauseType.value == RewardCauseType.PURCHASE_RECOVER;

            switch(rewardType.value)
            {
                case RewardType.CREDIT:
                case RewardType.CREDIT_WITH_MULTIPLIER:
                {
                    if (isRecover)
                        return IconType.PURCHASE_RECOVER_CREDIT;
                    return IconType.CREDIT;
                }
                case RewardType.RP:
                {
                    if (isRecover)
                        return IconType.PURCHASE_RECOVER_RP;
                    return IconType.RP;
                }
                case RewardType.DAILY_BOOST:
                    return IconType.DAILY_BOOST;
                case RewardType.TIER_UPGRADE:
                    return IconType.TIER_UPGRADE;
                case RewardType.DAILY_BONUS_WHEEL_SPIN:
                    return IconType.DAILY_BONUS_WHEEL_SPIN;
                case RewardType.MEGA_WHEEL_SPIN:
                    return IconType.MEGA_WHEEL_SPIN;
                case RewardType.GAME_SPIN:
                case RewardType.GAME_DEAL:
                case RewardType.TICKETED_BONUS_TICKET:
                case RewardType.TICKETED_BONUS_TICKET_FOR_BOOST:
                    return IconType.GAME_THUMBNAIL;
                case RewardType.PURCHASE_COUPON:
                    return IconType.PURCHASE_COUPON;
                case RewardType.EXP_MULTIPLY:
                case RewardType.EXP_MULTIPLY_EXTENDABLE:
                    return IconType.EXP_MULTIPLY;
                case RewardType.RANDOM:
                    return IconType.RANDOM;
                case RewardType.SCRATCHER:
                    return IconType.WEB_IMAGE;
                case RewardType.GEM:
                    return IconType.GEM;
            }

            return IconType.EMPTY;
        }

        private void UpdateRewardEvent()
        {
            Variable<RewardType> rewardType = BlackboardUtils.FindVariable<RewardType>(inboxInfoBB.value, "reward/type");

            EventInfo eventInfo = null;

            switch(rewardType.value)
            {
                case RewardType.PURCHASE_COUPON:
                    eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.VOUCHER_SHOP_EVENT_MULTIPLY);
                    break;
            }

            if(eventInfo != null)
            {
                saveAsEventID.value = eventInfo.id;
                saveAsEventNumerator.value = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo);
            }
        }

        private string GetExpireText()
        {
            long expireTimestamp = inboxInfoBB.value.GetValue<long>("expireTimestamp");

            if(expireTimestamp > 0)
                return StringTableUtils.GetString(tableType, "INBOX_ITEM_EXPIRE_DATE", expireTimestamp - MetaSystem.GetTimeStamp());

            return EXPIRE_EMPTY;
        }

        private string GetImageUrl()
        {
            if (!isInboxBanner.value)
            {
                Variable<InboxTypes> inboxType = BlackboardUtils.FindVariable<InboxTypes>(inboxInfoBB.value, "type");
                if (inboxType.value == InboxTypes.REWARD)
                {
                    var rewardImageURL = BlackboardUtils.FindVariable<string>(inboxInfoBB.value, "imageUrl");
                    if(rewardImageURL != null && !string.IsNullOrEmpty(rewardImageURL.value))
                    {
                        return rewardImageURL.value;
                    }
                    
                    Variable<RewardType> rewardType = BlackboardUtils.FindVariable<RewardType>(inboxInfoBB.value, "reward/type");
                    if (rewardType.value == RewardType.SCRATCHER)
                    {
                        return BlackboardUtils.FindValue<string>(inboxInfoBB.value, "reward/inboxImageUrl");
                    }
                }
                else if (inboxType.value == InboxTypes.SOCIAL_CREDIT)
                {
                    return inboxInfoBB.value.GetValue<string>("profileUrl");
                }
            }
            else
            {
                return inboxInfoBB.value.GetValue<string>("imageUrl");
            }

            return "";
        }
    }
}
