using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode;
using System.Collections.Generic;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]
    public class GambleTake : ActionTask
    {
        protected override void OnExecute()
        {
            MetaSystem.GambleTake(EndAction, null);
        }
    }
}
