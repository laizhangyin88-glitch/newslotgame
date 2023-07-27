using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/Contents")]
    public class ClearContentData : ActionTask
    {
        protected override void OnExecute()
        {
            var bb = ContentBlackboard.Get();
            BlackboardUtils.ClearBlackboard(bb);

            bb.AddVariable("betCredit", 0L);
            bb.AddVariable("extraBetCredit", 0L);
            bb.AddVariable("totalBetCredit", 0L);
            bb.AddVariable("maxBetCredit", 0L);
            bb.AddVariable("betIndex", 0);
            bb.AddVariable("extraBetRatioIndex", 0);
            bb.AddVariable("maxBetIndex", 0);
            bb.AddVariable("autoSpin", false);
            bb.AddVariable("isGameSpin", false);

            MetaSystem.ClearContentData();
            
            StringTable.Content().variables.Clear();

            while (GSManager.Instance.handlers.Count > 1)
            {
                GSManager.Instance.handlers.RemoveAt(GSManager.Instance.handlers.Count - 1);
            }

            EndAction();
        }
    }
}
