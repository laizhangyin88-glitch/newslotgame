using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Tutorial")]
    public class InitTutorialArrow : ActionTask
    {
        public BBParameter<Transform> pivot;

        protected override string info
        {
            get { return "Initialize Tutorial Arrow Position"; }
        }

        protected override void OnExecute()
        {
            var buttonTransform = MetaGameAppearTransformManager.Instance.GetTransform("LobbyTimeBonusButton");
            if (buttonTransform != null)
            {
                pivot.value.transform.position = buttonTransform.position;
            }
            
            EndAction();
        }
    }
}