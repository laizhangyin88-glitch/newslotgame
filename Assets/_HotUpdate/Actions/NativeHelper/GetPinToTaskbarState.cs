using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/NativeHelper")]
    public class GetPinToTaskbarState : ActionTask
    {
        public BBParameter<bool> saveAsPinned;

        protected override string info
        {
            get
            {
                return string.Format("{0} = Get pin to taskbar state", saveAsPinned);
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

// #if UNITY_WSA && !UNITY_EDITOR
            NativeHelper.Instance.IsPinned(
                (isPinned) =>
                {
                    saveAsPinned.value = isPinned;
                    // Debug.LogError(string.Format("Pinned = {0}", isPinned));
                    EndAction();
                }
            );
// #else
//             EndAction();
// #endif

        }
    }

}
