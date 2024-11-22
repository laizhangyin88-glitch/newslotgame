using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Task.Action
{

[Category("★ BagelCode/Feature Lock")]
public class GetFeatureLockStatus : ActionTask
{
    public BBParameter<LockedFeatureType> featureType;

    [BlackboardOnly]
    public BBParameter<bool> isLocked;

    public BBParameter<int> unlockLevel;

    protected override string info
    {
        get {return string.Format("{0} = Get Feature Lock Status({1})", isLocked, featureType);}
    }

    protected override void OnExecute()
    {
        isLocked.value = BlackboardQueryUtils.IsLockedFeature(featureType.value);
        unlockLevel.value = 0;

        if(isLocked.value)
        {
            unlockLevel.value = BlackboardQueryUtils.GetFeatureMinLevel(featureType.value);
        }
        
        EndAction();
    }
}

}
