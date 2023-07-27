using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Name("Initialize Application Settings")]
[Category("★ BagelCode/Utils")]
public class InitApplicationSettings : ActionTask 
{
    protected override void OnExecute()
    {
#if USE_ASSETBUNDLE
        if (BlackboardUtils.FindVariable<bool>(null, "/values/misc/CLIENT_ASSET_DOWNLOAD_RETRY_ENABLED").value)
            ApplicationSettings.Instance.asyncLoadBundleTimeout = BlackboardUtils.FindVariable<int>(null, "/values/misc/CLIENT_ASSET_DOWNLOAD_RETRY_TIMEOUT_SEC").value;
        else 
            ApplicationSettings.Instance.asyncLoadBundleTimeoutEnabled = false;
#endif
        EndAction();
    }
}

}
