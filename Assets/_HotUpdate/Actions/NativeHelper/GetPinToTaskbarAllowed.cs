using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/NativeHelper")]
    public class GetPinToTaskbarAllowed : ActionTask
    {
        public BBParameter<bool> saveAsPinAllowed;

        protected override string info
        {
            get
            {
                return string.Format("{0} = Get pin to taskbar allowed", saveAsPinAllowed);
            }
        }

        protected override void OnExecute()
        {
            saveAsPinAllowed.value = NativeHelper.Instance.IsPinningAllowed();

            EndAction();
        }
    }

}
