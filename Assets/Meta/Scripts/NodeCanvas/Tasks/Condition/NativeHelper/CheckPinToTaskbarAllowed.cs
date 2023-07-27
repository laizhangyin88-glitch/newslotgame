using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Condition
{
    [Category("★ BagelCode/NativeHelper")]
    public class CheckPinToTaskbarAllowed : ConditionTask
    {
        protected override bool OnCheck()
        {
            return NativeHelper.Instance.IsPinningAllowed();
        }
    }
}
