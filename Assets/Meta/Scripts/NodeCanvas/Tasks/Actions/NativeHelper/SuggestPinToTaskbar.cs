using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/NativeHelper")]
    public class SuggestPinToTaskbar : ActionTask
    {
        public BBParameter<bool> saveAsPinned;

        protected override string info
        {
            get
            {
                return string.Format("{0} = Suggest pin to taskbar", saveAsPinned);
            }
        }

        protected override void OnExecute()
        {
            saveAsPinned.value = false;

            if( !NativeHelper.Instance.IsPinningAllowed() )
            {
                EndAction();
                return;
            }

            NativeHelper.Instance.SetPin(
            (isPinned) =>
            {
                saveAsPinned.value = isPinned;
                BlackboardUtils.SetOrCreateValue<bool>(MainBlackboard.Get(), "isWindowsPinned", isPinned);
                EndAction();
            }
        );

        }
    }

}
