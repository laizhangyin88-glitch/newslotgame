using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Tutorial")]
    public class CheckTutorialWelcome : ConditionTask
    {
        protected override string info
        {
            get { return string.Format("Check Tutorial Welcome"); }
        }

        protected override bool OnCheck() 
        {
            var level = BlackboardUtils.FindVariable<int>(null, "/me/level");
            var requiredExp = BlackboardUtils.FindVariable<long>(null, "/me/requiredExp");
            var requiredExpMax = BlackboardUtils.FindVariable<long>(null, "/me/requiredExpMax");
            var tutorialEnabled = BlackboardUtils.FindVariable<bool>(null, "/values/misc/TUTORIAL_ENABLED");

            if (    tutorialEnabled != null
                 && tutorialEnabled.value
                 && level.value == 1
                 && requiredExp.value == requiredExpMax.value
                 && PlayerPrefs.GetInt("tutorial_welcome_triggered", 0) == 0
                )
            {
                return true;
            }

            return false;
        }
    }
}
