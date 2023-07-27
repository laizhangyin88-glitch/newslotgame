using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Services;
using SlotMaker;
using SlotMaker.Cards;

namespace BagelCode
{

public class VideoPokerCommonWinSoundView : MonoBehaviour
{
    protected int grade = -1;

    public List<int> bonusIds;
    public List<CoinSoundViewPlayerInfo> sounds;

    public static readonly string ON_WIN_EVENT = "OnWinEvent";
    public static readonly string ON_TOTAL_WIN_EVENT = "TotalWin";
    public static readonly string ON_TOTAL_WIN_LINE_EVENT = "TotalWinLine";

    public static readonly string ON_CREDIT_EVENT = "OnCreditEvent";
    public static readonly string ON_UPDATE_TURN_CREDIT_EVENT = "UpdateTurnCredit";
    public static readonly string ON_UPDATED_TURN_CREDIT_EVENT = "UpdatedTurnCredit";

    protected virtual void OnEnable()
    {
        MessageDispatcher.Register(ON_WIN_EVENT, OnWinEvent);
        MessageDispatcher.Register(ON_CREDIT_EVENT, OnCreditEvent);
    }

    protected virtual void OnDisable()
    {
        MessageDispatcher.UnRegister(ON_WIN_EVENT, OnWinEvent);
        MessageDispatcher.UnRegister(ON_CREDIT_EVENT, OnCreditEvent);
    }

    protected virtual void OnWinEvent(EventData receivedEvent)
    {
        if (receivedEvent.name.Equals(ON_TOTAL_WIN_EVENT, StringComparison.Ordinal) ||
            receivedEvent.name.Equals(ON_TOTAL_WIN_LINE_EVENT, StringComparison.Ordinal))
        {
            OnTotalWin((List<PokerWin>)receivedEvent.value);
        }
    }

    protected virtual void OnCreditEvent(EventData receivedEvent)
    {
        if (receivedEvent.name.Equals(ON_UPDATE_TURN_CREDIT_EVENT, StringComparison.Ordinal))
        {
            OnUpdateTurnCredit((bool)receivedEvent.value);
        }
        else if (receivedEvent.name.Equals(ON_UPDATED_TURN_CREDIT_EVENT, StringComparison.Ordinal))
        {
            OnUpdatedTurnCredit();
        }
    }

    protected long GetTotalEarnCredit(List<PokerWin> winList)
    {
        long totalEarnCredit = 0L;
        foreach (var win in winList)
        {
            totalEarnCredit += win.earnCredit;
        }
        return totalEarnCredit;
    }

    private bool CheckIgnoreWinEnd (int grade)
    {
        var playerInfo = GetCurrentSoundSet();
        if (playerInfo.ignoreWinEnd == null || playerInfo.ignoreWinEnd.Count == 0) 
            return false;
        else 
            return playerInfo.ignoreWinEnd[grade];
    }

    private int FindBonus(int bonusId)
    {
        for (int i = 0; i < bonusIds.Count; ++i)
        {
            if (bonusId == bonusIds[i])
                return i;
        }

        return -1;
    }

    private CoinSoundViewPlayerInfo GetBonusSound(int bonusId)
    {
        int index = FindBonus(bonusId);
        return (index >= 0) ? sounds[index + 1] : sounds[0];
    }

    private CoinSoundViewPlayerInfo GetCurrentSoundSet()
    {
        var bonus = BlackboardUtils.FindVariable<Blackboard>(null, "./bonus");
        if (bonus == null)
            return sounds[0];

        int bonusId = bonus.value.GetValue<int>("bonusId");
        return GetBonusSound(bonusId);
    }

    private void OnTotalWin(List<PokerWin> winList)
    {
        var playerInfo = GetCurrentSoundSet();
        var betCredit  = BlackboardUtils.FindValue<long>(null, "./turn/totalBetCredit");
        var earnCredit = GetTotalEarnCredit(winList);
        var gradeCount = playerInfo.grades.Count;

        grade = -1;
        for (int i = 0; i < gradeCount; ++i)
        {
            if (earnCredit < betCredit * playerInfo.grades[i])
            {
                GSManager.Instance.GetHandler(playerInfo.winSounds[i]).Play();
                grade = i;

                break;
            }
        }
    }

    private void OnTotalWinLine(List<PokerWin> winList)
    {
        OnTotalWin(winList);
    }

    protected virtual void OnUpdateTurnCredit(bool forceUpdate)
    {
        if (forceUpdate)
            OnUpdatedTurnCredit();
    }

    protected virtual void OnUpdatedTurnCredit()
    {
        if (grade >= 0)
        {
            if (!CheckIgnoreWinEnd(grade))
            {
                var playerInfo = GetCurrentSoundSet();
                GSManager.Instance.GetHandler(playerInfo.winSounds[grade]).Stop();
                GSManager.Instance.GetHandler(playerInfo.winEndSounds[grade]).Play();
            }
            grade = -1;
        }
    }
}

}
