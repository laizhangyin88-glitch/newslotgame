using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using System.Collections.Generic;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/NativeHelper")]
public class InitializeAdjustManager : ActionTask
{
    protected override string info
    {
        get
        {
            return "Initialize AdjustManager";
        }
    }

    protected override void OnExecute()
    {
        if (ApplicationSettings.LogTest())
            Debug.Log("Init AdjustManager");

//#if !UNITY_EDITOR
//    #if DEV
//        if (ApplicationSettings.LogTest())
//        {
//            var eventTokenDict = ProductSettings.Instance.GetAdjustTokens();

//            foreach (KeyValuePair<string, string> pair in eventTokenDict)
//            {
//                Debug.Log( string.Format("Adjust Tokens {0} : {1}", pair.Key, pair.Value) );
//            }
//        }
//    #endif

//        AdjustManager.Instance.Initialize(ProductSettings.Instance.GetAdjustTokens());
//#endif

        EndAction();
    }
}

}
