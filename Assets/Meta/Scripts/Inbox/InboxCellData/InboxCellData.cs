using System.Collections;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public abstract class InboxCellData
    {
        public Blackboard inboxInfo;

        public object AcceptEventData = null;

        public string text;
        public string buttonText;
        public IconType iconType;

        public InboxCellController cell;
        protected ContextElement ownerRootElement;
        protected Blackboard ownerBlackboard;

        public bool isBanner;
        public InboxTypes inboxType;
        public InboxBannerTypes bannerType;

        public int gameId = -1;
        public int disappearType = 0;
        public BonusTag bonusTag = BonusTag.UNKNOWN;
        public string iconUrl = string.Empty;
        public bool isAcceptNext = false;
        public bool useWarningColor = false;
        public bool isFirstReward = true;
        public InboxCellController.InboxButtonType buttonType = InboxCellController.InboxButtonType.ACCEPT;

        public string title;
        public string message;

        protected const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;
        protected const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        protected const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        public enum IconType
        {
            NONE = 0,
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
            MEGA_WHEEL_SPIN,
            SPIN_BOOST,
            HOG_DEAL,
            FINDER,
            VIP_INVITE,
            VIP_EPIC_LOUNGE,
            VIP_LOUNGE_OPEN_TICKET,
            VIP_LOUNGE_JACKPOT,
            BOSSRAIDERS_DEAL,
            DEPOT,
            WILD_PUZZLE,
            LEVEL_UP_EXP_BOOST,
            BUCKS_GIFT,
        }

        //

        public virtual void InboxItemCheckType() { }
        public virtual IEnumerator InboxItemCheckExtraCoroutine() { yield break; }
        public virtual IEnumerator InboxItemCheckViewAdCoroutine() { yield break; }

        public static InboxCellData Create(InboxCellController _ownerCellController, Blackboard _inboxInfo)
        {
            bool _isBanner = BlackboardQueryUtils.IsInboxBanner(_inboxInfo);
            if (!_isBanner)
            {
                InboxTypes inboxType = _inboxInfo.GetValue<InboxTypes>("type");
                switch (inboxType)
                {
                    case InboxTypes.REWARD:
                        return new InboxCellDataReward(_ownerCellController, _inboxInfo);
                    case InboxTypes.MESSAGE:
                        return new InboxCellDataMessage(_ownerCellController, _inboxInfo);
                    case InboxTypes.MESSAGE_WARNING:
                        return new InboxCellDataMessageWarning(_ownerCellController, _inboxInfo);
                    case InboxTypes.GAME_COMPENSATION:
                        return new InboxCellDataGameCompensation(_ownerCellController, _inboxInfo);
                    case InboxTypes.FACEBOOK_FRIEND_CONNECT:
                        return new InboxCellDataFaceBookFriendConnect(_ownerCellController, _inboxInfo);
                    case InboxTypes.FACEBOOK_SHARE:
                        return new InboxCellDataFaceBookShare(_ownerCellController, _inboxInfo);
                    case InboxTypes.TOURNAMENT_WIN:
                        return new InboxCellDataTournamentWin(_ownerCellController, _inboxInfo);
                    case InboxTypes.TICKETED_BONUS_TICKET:
                        return new InboxCellDataTicketedBonusTicket(_ownerCellController, _inboxInfo);
                    case InboxTypes.TICKETED_BONUS_TICKET_FOR_BOOST:
                        return new InboxCellDataTicketedBonusTicketForBoost(_ownerCellController, _inboxInfo);
                    case InboxTypes.SOCIAL_CREDIT:
                        return new InboxCellDataSocialCredit(_ownerCellController, _inboxInfo);
                    case InboxTypes.SPIN_DEAL:
                        return new InboxCellDataSpinDeal(_ownerCellController, _inboxInfo);
                    case InboxTypes.HOG_DEAL:
                        return new InboxCellDataHogDeal(_ownerCellController, _inboxInfo);
                    case InboxTypes.SPIN_DEAL_V2:
                        return new InboxCellDataSpinDeal(_ownerCellController, _inboxInfo);
                    case InboxTypes.BOSS_RAIDERS_DEAL:
                        return new InboxCellDataBossRaidersDeal(_ownerCellController, _inboxInfo);
                    default:
                        Debug.LogError("InboxCellData.Create Failure: " + inboxType + " is undefined inbox type.");
                        return null;
                }
            }
            else
            {
                InboxBannerTypes bannerType = _inboxInfo.GetValue<InboxBannerTypes>("inboxBannerType");
                switch (bannerType)
                {
                    case InboxBannerTypes.SALES:
                        return new InboxCellDataSales(_ownerCellController, _inboxInfo);
                    case InboxBannerTypes.NEWS:
                        return new InboxCellDataNews(_ownerCellController, _inboxInfo);
                    case InboxBannerTypes.SALES_META:
                        return new InboxCellDataSalesMeta(_ownerCellController, _inboxInfo);
                    default:
                        Debug.LogError("InboxCellData.Create Failure: " + bannerType + " is undefined banner type.");
                        return null;
                }
            }
        }

        public virtual IEnumerator OnAcceptSuccessCoroutine()
        {
            InboxRectController callerRect = cell.rectController;
            yield return callerRect.StartCoroutine(MetaCommonRewardUtils.CheckTierUpCoroutine(callerRect.gameObject));
        }

        public virtual void OnRemoveInboxItem() { }

        public IEnumerator InboxCellCheckGameCoroutine()
        {
            if (gameId == -1) yield break;

            Blackboard slotInfoBB = BlackboardQueryUtils.GetEarlyAccessSlotInfo(gameId);
            if (slotInfoBB is null)
                slotInfoBB = BlackboardQueryUtils.GetSlotInfoBB(gameId);

            int slotInfoState = slotInfoBB.GetVariable<int>("flags/status")?.value ?? -1;
            if (slotInfoState != 3) yield break;

            GameObject commonPopup = null;
            yield return MetaPopupUtils.SimpleOpenPopupCoroutine(cell, "Popup Common OK Scene",
                (sceneLoadOperation) => commonPopup = sceneLoadOperation.GetScene());

            string title = StringTableUtils.GetString(StringTable.StringTableType.Global, "INBOX_ITEM_UNDER_CONSTRUCTION");
            string yesButtonText = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OK");
            string onCalleeCallback = "OnCalleeCallback";

            MetaPopupUtils.SetCommonPopupData(commonPopup, cell.transform, title, "", onCalleeCallback,
                yesButtonText, "", "", onCalleeCallback, onCalleeCallback, true, false, true, false, true);

            MetaPopupUtils.OpenPopup(commonPopup);

            yield return new WaitUntilTrigger(new EventTrigger(cell, onCalleeCallback));

            InboxController.IsAcceptable = true;
        }

        protected InboxCellData(InboxCellController _ownerCellController, Blackboard _inboxInfo)
        {
            cell = _ownerCellController;
            inboxInfo = _inboxInfo;

            ownerRootElement = cell.GetComponent<ContextElement>();
            ownerBlackboard = cell.GetComponent<Blackboard>();

            title = inboxInfo.GetValue<string>("title");
            message = inboxInfo.GetValue<string>("message");

            isBanner = BlackboardQueryUtils.IsInboxBanner(inboxInfo);
            if (isBanner)
                bannerType = inboxInfo.GetValue<InboxBannerTypes>("inboxBannerType");
            else
                inboxType = inboxInfo.GetValue<InboxTypes>("type");

            text = GetText();
            buttonText = GetButtonText();
            iconType = GetIconType();
        }

        // GetInternalText + common tag format
        protected string GetText()
        {
            string text = GetInternalText();

            if( text.Contains("{") )
            {
                // post process
                text = InterpolatedTextUtils.ParseCommonText(text);
                text = InterpolatedTextUtils.ParseEconomyMultiplier(text);
                text = InterpolatedTextUtils.ParseCustomText(text);
            }

            return text;
        }

        protected virtual string GetInternalText()
        {
            return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_TEXT_MESSAGE", title, message);
        }

        protected virtual string GetButtonText()
        {
            return StringTableUtils.GetString(GLOBAL, "INBOX_ITEM_BUTTON_OK");
        }

        protected virtual IconType GetIconType()
        {
            return IconType.EMPTY;
        }
    }
}
