using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/NativeHelper")]
public class GetFreeDiskSpace : ActionTask
{
    protected override string info { get { return string.Format("GetFreeDiskSpace"); } }

    protected override void OnExecute()
    {
        Variable<bool> enableStorageCheck = BlackboardUtils.FindVariable<bool>(null, "/values/misc/ENABLE_CLIENT_STORAGE_CHECK");

        if (enableStorageCheck != null && enableStorageCheck.value)
        {
            Variable<int> storageCheckMinMB = BlackboardUtils.FindVariable<int>(null, "/values/misc/CLIENT_STORAGE_CHECK_MIN_MB");
            long freeDiskSpace = NativeHelper.Instance.GetFreeDiskSpace();
            
            if (freeDiskSpace < storageCheckMinMB.value)
            {
                BICustomEvents.StorageCheck(freeDiskSpace, storageCheckMinMB.value);

                bool isError = false;

                ErrorPopupInfo info = new ErrorPopupInfo();

                info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_NEED_MORE_STORAGE", storageCheckMinMB.value, out isError);

#if UNITY_ANDROID
                info.type = ErrorPopupType.OK;
                
                info.useXButton = false;
                info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OK", out isError);
                info.buttonAutoClose1 = false;
                
                info.callback1 = delegate
                {
                    Application.Quit();
                };
#else
                info.type = ErrorPopupType.TextOnly;
#endif

                ErrorPopupHandler.Instance.OpenError(info);
            }
            else
            {
                EndAction(); 
            }
        }
        else
        {
            EndAction();    
        }  
    }
}

}
 
