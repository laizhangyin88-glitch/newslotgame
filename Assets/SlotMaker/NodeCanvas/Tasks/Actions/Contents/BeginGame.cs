using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.Contents;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/Contents")]
    public class BeginGame : ActionTask
    {
        protected override void OnExecute()
        {
            var mb = MainBlackboard.Get();
            BlackboardUtils.SetOrCreateValue(mb, "inGame", true);

            var cb = ContentBlackboard.Get();
            BlackboardUtils.SetOrCreateValue(cb, "autoSpin", false);

            var game = cb.GetValue<Blackboard>("game");

            long betCredit = cb.GetValue<List<long>>("betList")[0];
            BlackboardUtils.SetOrCreateValue(cb, "betCredit", betCredit);

            int extraBetRatioIndex = BlackboardUtils.FindValue<int>("./extraBetRatioIndex");
            var extraBetRatioList = BlackboardUtils.FindValue<List<Blackboard>>("./game/extraBetRatioList");
            var extraBetNumerator = extraBetRatioList[extraBetRatioIndex].GetValue<int>("numerator");
            var extraBetDenominator = extraBetRatioList[extraBetRatioIndex].GetValue<int>("denominator");
            BlackboardUtils.SetOrCreateValue(cb, "extraBetCredit", (betCredit * extraBetNumerator / extraBetDenominator));

            string guid = Guid.NewGuid().ToString();
            long initialCredit = BlackboardUtils.FindVariable<long>(null, "/me/credit").value;
            long timestamp = MetaSystem.GetTimeStamp();

            int gameVersion = ContentsVersionManager.Instance.GetCurrentGameVersion();

            BlackboardUtils.SetOrCreateValue(game, "uid", guid);
            BlackboardUtils.SetOrCreateValue(game, "spentCredit", 0L);
            BlackboardUtils.SetOrCreateValue(game, "earnCredit", 0L);
            BlackboardUtils.SetOrCreateValue(game, "beginCredit", initialCredit);
            BlackboardUtils.SetOrCreateValue(game, "turnCount", 0);
            BlackboardUtils.SetOrCreateValue(game, "spinCount", 0);
            BlackboardUtils.SetOrCreateValue(game, "beginTime", timestamp);
            BlackboardUtils.SetOrCreateValue(game, "gameVersion", gameVersion);

            ContentEvent.BeginGame(game);

            EndAction();
        }
    }
}
