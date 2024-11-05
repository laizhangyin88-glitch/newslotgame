using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/NativeHelper")]
public class InitializePurchaseManager : ActionTask
{
    protected override string info
    {
        get
        {
            return "Initialize PurchaseManager";
        }
    }

    protected override void OnExecute()
    {
        if (ApplicationSettings.LogTest())
            Debug.Log("Init PurchaseManager");

//#if !UNITY_EDITOR
//        PurchaseManager.Instance.Initialize();
//#endif

        EndAction();
    }
}

}
