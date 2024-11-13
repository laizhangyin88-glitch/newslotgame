using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class LoadFSM : ActionTask
{
    public BBParameter<string> assetName;
    public BBParameter<string> bundleName;          // "./game/gameTitle"

    public BBParameter<FSM> saveAs;

    protected override void OnExecute()
    {
        var bundle = BlackboardUtils.FindVariable<string>(null, bundleName.value);
        var fsm    = AssetBundleManager.LoadAsset<FSM>(bundle.value, assetName.value);
        saveAs.value = fsm;
        EndAction();
    }
}

}
