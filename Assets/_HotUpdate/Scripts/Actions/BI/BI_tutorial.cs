using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{
    [Category("★ BagelCode/BI")]
    public class BI_tutorial : ActionTask
    {
        public enum TutorialType
        {
            WELCOME,
            PLAY,
            LEVEL_UP,
            MYSTERY_GIFT,
            CHALLENGE,
            TOURNAMENT,
            CLUB,
            EARLY_ACCESS
        }

        public enum Type
        {
            TRIGGER,
            CLICK,
            CLOSE
        }

        public BBParameter<TutorialType> tutorialType;
        public BBParameter<Type> type;

        protected override void OnExecute()
        {
            if (!string.IsNullOrEmpty(tutorialType?.value.ToString()))
            {
                Analytics.CustomEvent("client_tutorial_progress", new Dictionary<string, object>
            {
                { "tutorial_type", tutorialType.value.ToString().ToLower() },
                { "type", type.value.ToString().ToLower() },
            });
            }

            EndAction();
        }
    }
}