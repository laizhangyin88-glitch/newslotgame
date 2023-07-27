using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Task.Actions
{

    [Category("★ BagelCode/Tournament")]
    public class StartTournamentFeed : ActionTask<Blackboard>
    {
        protected override string info
        {
            get { return "Start Tournament"; }
        }

        protected override void OnExecute()
        {
            Blackboard newBB = BlackboardUtils.CreateBlackboard("StartTournament") as Blackboard;

            BlackboardUtils.SetOrCreateValue<PollType>(newBB, "__event__", PollType.TOURNAMENT_START);
            BlackboardUtils.SetOrCreateValue<int>(newBB, "periodMin", BlackboardQueryUtils.GetTournamentPeriodMin());

            string eventName = PollType.TOURNAMENT_START.ToString();
            EventSender.SendGlobalEvent(
                MetaEventDefine.ON_LONG_POLL_EVENT,
                new ParadoxNotion.EventData<Blackboard>(eventName, newBB));

            EndAction();
        }
    }
}