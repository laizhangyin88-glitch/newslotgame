using System;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Club")]

public class GetClubFeedRequestShareItemBB : ActionTask<Blackboard>
{
    public BBParameter<int> cellIndex;
    public BBParameter<List<GameObject>> bgList; // 0, 1
    public BBParameter<string>  clubFeedInfoValue;
    public BBParameter<string> mgBundleName;
    public BBParameter<EventInfoType> mgEventType;

    public BBParameter<int> saveAsItemID;
    public BBParameter<ContextElement> saveAsLikeGaugeElement;

    ContextElement rootElement;
    ContextElement userNameElement;
    ContextElement leftTimeAgoElement;
    ContextElement textMessageElement;
    ContextElement gaugeElement;
    ContextElement gaugeTextElement;
    ContextElement myHaveTextElement;
    ContextElement tierAnimationElement;
    ContextElement imageElement;
    ContextElement giftEnableElement;
    ContextElement giftButtonElement;
    ContextElement giftButtonTextElement;
    ContextElement giftDisableElement;
    ContextElement giftDisableTextElement;
    ContextElement itemAreaElement;
    ContextElement itemElement;

    private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

    private GameObject itemObj = null;
    private bool isInit = false;

    protected override string info
    {
        get { return "Get Club Feed Request Share Item Info BB"; }
    }

    protected override void OnExecute()
    {
        InitProperty();
        UpdateVariables();

        EndAction();
    }

    private void InitProperty()
    {
        if(isInit) return;

        rootElement = agent.gameObject.GetComponent<ContextElement>();

        userNameElement = ContextUtils.FindElement(rootElement, "Text Name", ContextSearchingType.ChildrenSearch);
        leftTimeAgoElement = ContextUtils.FindElement(rootElement, "Text Time", ContextSearchingType.ChildrenSearch);
        textMessageElement = ContextUtils.FindElement(rootElement, "Text Context", ContextSearchingType.ChildrenSearch);
        gaugeElement = ContextUtils.FindElement(rootElement, "Gauge", ContextSearchingType.ChildrenSearch);
        gaugeTextElement = ContextUtils.FindElement(rootElement, "Gauge/Text", ContextSearchingType.FullNameSearch);
        myHaveTextElement = ContextUtils.FindElement(rootElement, "Gift Enabled/Text You Have Amount", ContextSearchingType.FullNameSearch);

        tierAnimationElement =ContextUtils.FindElement(rootElement, "Profile Picture", ContextSearchingType.ChildrenSearch);
        imageElement = ContextUtils.FindElement(rootElement, "Profile Picture/Image", ContextSearchingType.FullNameSearch);

        giftEnableElement = ContextUtils.FindElement(rootElement, "Gift Enabled", ContextSearchingType.ChildrenSearch);
        giftButtonElement = ContextUtils.FindElement(giftEnableElement, "Button Gift", ContextSearchingType.ChildrenSearch);
        giftButtonTextElement = ContextUtils.FindElement(giftButtonElement, "Text", ContextSearchingType.ChildrenSearch);

        giftDisableElement = ContextUtils.FindElement(rootElement, "Gift Disabled", ContextSearchingType.ChildrenSearch);
        giftDisableTextElement = ContextUtils.FindElement(giftDisableElement, "Text Disabled", ContextSearchingType.ChildrenSearch);

        itemAreaElement = ContextUtils.FindElement(rootElement, "Item Area", ContextSearchingType.ChildrenSearch);

        MetaContextElementUtils.SetText(textMessageElement, StringTableUtils.GetString(tableType, "CLUB_NEWS_FEED_REQUEST_ITEM_TEXT"));

        itemObj = MetaObjectUtils.MakePrefab(mgBundleName.value, "Share Item", itemAreaElement.transform, null, null);
        itemElement = itemObj.GetComponent<ContextElement>();
        itemElement.UpdateContext(false);

        MetaContextElementUtils.SetText(giftButtonTextElement, StringTableUtils.GetString(tableType, "CLUB_NEWS_FEED_BUTTON_GIFT"));
        MetaContextElementUtils.SetClickable(
            giftButtonElement,
            "OnGift",
            false,
            false,
            SendEvent,
            ownerSystem
        );

        isInit = true;
    }

    private void UpdateVariables()
    {
        var clubFeedInfo = BlackboardUtils.FindVariable<Blackboard>(agent, clubFeedInfoValue.value);

        if(clubFeedInfo != null)
        {
            var feedUserID  = clubFeedInfo.value.GetValue<string>("userId");
            var like        = clubFeedInfo.value.GetValue<int>("like");
            var userName    = clubFeedInfo.value.GetValue<string>("name");
            var profileUrl  = clubFeedInfo.value.GetValue<string>("profileUrl");
            var tier        = clubFeedInfo.value.GetValue<int>("tier");
            var eventID     = clubFeedInfo.value.GetValue<int>("eventId");
            
            int capacity    = 0;
            int possessions = 0;
            int requirement = 0;

            saveAsItemID.value = 0;

            var meID = BlackboardUtils.FindVariable<string>(null, "/me/userId");
            int tierGroup = TierUtils.GetTierGroup(tier);

            bool isMeFeed = feedUserID == meID.value;

            // link / capacity
            // possesssions / requirement 
            switch(mgEventType.value)
            {
                case EventInfoType.COLLECTING_GAME:
                    {
                        capacity    = clubFeedInfo.value.GetValue<int>("capacity");
                        possessions = clubFeedInfo.value.GetValue<int>("possessions");
                        requirement = clubFeedInfo.value.GetValue<int>("requirement");

                        var pieceID = clubFeedInfo.value.GetValue<int>("pieceId");
                        var rarity  = clubFeedInfo.value.GetValue<ScratcherPieceRarity>("rarity");

                        saveAsItemID.value = pieceID;

                        if(rarity == ScratcherPieceRarity.UNKNOWN)
                            rarity = ScratcherPieceRarity.ONE;
                        ContextElement starElement = ContextUtils.FindElement(itemElement, "Star", ContextSearchingType.ChildrenSearch);
                        MetaContextElementUtils.SetSprite(starElement, Scratcher.CollectingGameCustomData.Instance.collectingGameAssets.starAssets[(int)rarity - 1]);

                        ContextElement ItemImageElement = ContextUtils.FindElement(itemElement, "Image", ContextSearchingType.ChildrenSearch);

                        Sprite itemAsset = BlackboardQueryUtils.GetAssetOfItem(pieceID);
                        MetaContextElementUtils.SetSprite(ItemImageElement, itemAsset);
                    }
                    break;
            }

            bool isEnableFeed = false;
            if(like >= capacity)
            {
                // Disable
                // Received All
                isEnableFeed = false;
                MetaContextElementUtils.SetText(giftDisableTextElement, StringTableUtils.GetString(tableType, "CLUB_NEWS_FEED_REQUEST_STATE_TEXT_00"));
            }
            else
            {
                if(isMeFeed)
                {
                    // Disable
                    // My Request
                    isEnableFeed = false;
                    MetaContextElementUtils.SetText(giftDisableTextElement, StringTableUtils.GetString(tableType, "CLUB_NEWS_FEED_REQUEST_STATE_TEXT_01"));
                }
                else
                {
                    isEnableFeed = true;

                    // Unity 2019.2.21f1 Interaction bug..
                    MetaContextElementUtils.SetBooleanProperty(giftButtonElement, false);
                    ///////
                    
                    MetaContextElementUtils.SetBooleanProperty(giftButtonElement, possessions > 0);
                }
                
            }

            giftEnableElement.gameObject.SetActive(isEnableFeed);
            giftDisableElement.gameObject.SetActive(!isEnableFeed);

            // Profile
            MetaContextElementUtils.SetWebImage(imageElement, profileUrl, CacheType.MemCache, true, null);
            MetaContextElementUtils.SetIntProperty(tierAnimationElement, tierGroup);
            MetaContextElementUtils.SetText(userNameElement, StringTableUtils.GetString(tableType, "CLUB_NEWS_FEED_NAME", userName));

            var createdTimestamp = BlackboardUtils.GetOrCreateVariable<long>(clubFeedInfo.value, "createdTimestamp").value;
            MetaContextElementUtils.SetText(leftTimeAgoElement, ClubUtils.GetClubFeedLeftTimeText( TimeUtils.GetTimeStamp() - createdTimestamp ));

            MetaContextElementUtils.SetText(gaugeTextElement, StringTableUtils.GetString(tableType, "CLUB_NEWS_FEED_REQUEST_GAUGE_PROGRESS_TEXT", like, capacity));
            MetaContextElementUtils.SetFloatProperty(gaugeElement, (float)like/(float)capacity);

            MetaContextElementUtils.SetText(myHaveTextElement, StringTableUtils.GetString(tableType, "CLUB_NEWS_FEED_REQUEST_HAVE_PROGRESS_TEXT", possessions, requirement));

            saveAsLikeGaugeElement.value = gaugeElement;

            UpdateBG();
        }
    }

    private void UpdateBG()
    {
        int cellStyle = cellIndex.value%2;

        for(int i=0; i<bgList.value.Count; ++i)
        {
            bgList.value[i].SetActive(i==cellStyle);
        }
    }
}

}
