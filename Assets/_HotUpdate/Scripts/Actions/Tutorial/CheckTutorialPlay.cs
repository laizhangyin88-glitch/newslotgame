using BagelCode.ClientModels;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Tutorial")]
    public class CheckTutorialPlay : ConditionTask
    {
        protected override string info
        {
            get { return string.Format("Trigger Tutorial Play"); }
        }

        protected override bool OnCheck() 
        {
            var tutorialEnabled = BlackboardUtils.FindVariable<bool>(null, "/values/misc/TUTORIAL_ENABLED");
            var tutorialInfo = BlackboardUtils.FindVariable<Blackboard>(null, "/tutorialInfo");

            if (tutorialEnabled != null && tutorialEnabled.value && tutorialInfo != null)
            {
                int stage = tutorialInfo.value.GetValue<int>("stage");
                if (stage == 0 || stage == 1 || stage == 2)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
