using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions.AssetBundle
{

[Category("★ BagelCode/AssetBundle")]
public class AddUsingMetaAssetBundle : ActionTask<Blackboard>
{
    public BBParameter<string> bundleName;

    protected override void OnExecute()
    {
        BlackboardQueryUtils.AddUsingMetaAssetBundle(bundleName.value);

        EndAction();
    }
}

}
