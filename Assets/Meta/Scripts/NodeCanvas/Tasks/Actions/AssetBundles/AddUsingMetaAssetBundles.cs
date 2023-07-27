using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions.AssetBundle
{

[Category("★ BagelCode/AssetBundle")]
public class AddUsingMetaAssetBundles : ActionTask<Blackboard>
{
    public BBParameter<List<string>> bundleNames;

    protected override void OnExecute()
    {
        BlackboardQueryUtils.AddUsingMetaAssetBundles(bundleNames.value);

        EndAction();
    }
}

}
