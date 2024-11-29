using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions
{

    [Category("✫ Blackboard")]
    public class CheckLibVersion : ConditionTask
    {
        protected override string info => $"Application.isEditor";

        protected override bool OnCheck()
        {
            return Application.isEditor;
        }
    }
}
