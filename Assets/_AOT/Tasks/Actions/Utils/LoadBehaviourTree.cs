using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.BehaviourTrees;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class LoadBehaviourTree : ActionTask
{
    public BBParameter<string> assetName;
    public BBParameter<string> bundleName;          // "./game/gameTitle"

    public BBParameter<BehaviourTree> saveAs;

    protected override void OnExecute()
    {
        var bundle = BlackboardUtils.FindVariable<string>(null, bundleName.name);
        var fsm    = AssetBundleManager.LoadAsset<BehaviourTree>(bundle.value, assetName.value);
        saveAs.value = fsm;
        EndAction();
    }
}

}
