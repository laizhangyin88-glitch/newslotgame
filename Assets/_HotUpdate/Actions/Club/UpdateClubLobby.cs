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

public class UpdateClubLobby : ActionTask<Blackboard>
{
    private bool isInit = false;

    private ContextElement badgeArea;
    private ContextElement coinArea;
    private ContextElement leagueEventTimerAreaElement;
    private EventTagController eventTagController;

    private GameObject badgeObject;
    private Animator badgeAnimator;

    private const string BADGE_ASSET_NAME = "Badge";

    protected override string info
    {
        get { return "Update Club Lobby Badge"; }
    }

    protected override void OnExecute()
    {
        InitProperty();

        if(BlackboardQueryUtils.IsExistNewFeed())
        {
            ShowNewBadge();
        }
        else
        {
            if(badgeObject != null)
                badgeObject.SetActive(false);
        }

        ShowCoin( BlackboardQueryUtils.IsExistNewFeed() );

        ShowLeagueTimer(BlackboardQueryUtils.GetClubLeagueEndTimestamp());

        EndAction();
    }

    private void InitProperty()
    {
        if(isInit) return;

        ContextElement agentElement = agent.gameObject.GetComponent<ContextElement>();

        badgeArea = ContextUtils.FindElement(agentElement, "Badge Area", ContextSearchingType.ChildrenSearch);
        coinArea = ContextUtils.FindElement(agentElement, "Effect Coin Collect Icon", ContextSearchingType.ChildrenSearch);

        leagueEventTimerAreaElement = ContextUtils.FindElement(agentElement, "League Timer Area", ContextSearchingType.ChildrenSearch);
        var eventTagElement = ContextUtils.FindElement(leagueEventTimerAreaElement, "Event Tag Purple", ContextSearchingType.ChildrenSearch);
        eventTagController = eventTagElement.GetComponent<EventTagController>();

        isInit = true;
    }

    private void MakeNewBadge()
    {
        if(badgeObject == null)
        {
            badgeObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, BADGE_ASSET_NAME, badgeArea.transform, "");
            ContextElement badgeElement = badgeObject.GetComponent<ContextElement>();
            badgeElement.UpdateContext(true);

            badgeAnimator = badgeObject.GetComponent<Animator>();
            MetaContextElementUtils.SimpleSetText(badgeElement, "Text", "N", ContextSearchingType.ChildrenSearch);
        }
    }

    private void ShowNewBadge()
    {
        MakeNewBadge();

        if(badgeObject != null)
        {
            badgeObject.SetActive(true);
            badgeAnimator.SetInteger("value", 1);
        }
    }

    private void ShowCoin(bool isShow)
    {
        coinArea.gameObject.SetActive(isShow);
    }

    private void ShowLeagueTimer(long clubLeagueEndtimestamp)
    {
        if(leagueEventTimerAreaElement == null || eventTagController == null) return;

        if(clubLeagueEndtimestamp > 0)
        {
            long currentTimestamp = TimeUtils.GetTimeStamp();
            DateTime targetDate = TimeUtils.ParseTimestampToDateTime(clubLeagueEndtimestamp);
            DateTime currentDate = TimeUtils.ParseTimestampToDateTime(currentTimestamp);
            TimeSpan leftTimespan = targetDate - currentDate;

            if(currentTimestamp < clubLeagueEndtimestamp && leftTimespan.TotalDays <= 1.0)
            {
                leagueEventTimerAreaElement.gameObject.SetActive(true);

                eventTagController.Initialize(
                    targetTime: clubLeagueEndtimestamp,
                    warningTime: 0L,
                    timeFormatKey: "TIME_FORMAT_HHMMSS_TOTALHOUR",
                    outputFormatKey: "CLUB_LEAGUE_END_TIMER_TITLE_OUTPUT_FORMAT",
                    warningFormatKey: null,
                    expireText: StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_LEAGUE_END_TIMER_EXPIRE_TEXT"),
                    useCommonTimer: true,
                    caller: null,
                    eventTextList: null,
                    textChangeDelay: 0f,
                    textChangeSpd: 0f,
                    defaultEventText: StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_LEAGUE_END_TIMER_TEXT")
                );
            }
            else
            {
                leagueEventTimerAreaElement.gameObject.SetActive(false);
            }
        }
        else
        {
            leagueEventTimerAreaElement.gameObject.SetActive(false);
        }

    }
}

}

