using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

    [Category("★ Extend/LastFreeGame")]
    public class CheckLastFreeGame : ConditionTask<ContextElement>
    {

        protected override string info => $"当前是断线重连";

        protected override bool OnCheck()
        {
            return LastFreeGameManager.Instance.isLastGameSpin;
        }
    }

}
