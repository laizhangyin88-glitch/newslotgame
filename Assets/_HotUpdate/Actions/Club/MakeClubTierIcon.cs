using System;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Club")]

public class MakeClubTierIcon : ActionTask<Blackboard>
{
    public BBParameter<string>  clubTierValue;
    public BBParameter<Transform> parent;
    public BBParameter<string> parentName;

    public BBParameter<GameObject> saveAs;

    protected override string info
    {
        get { return "Make Club Tier Icon"; }
    }

    protected override void OnExecute()
    {
        var clubTier = BlackboardUtils.FindVariable<int>(agent, clubTierValue.value);

        if(saveAs.value != null)
            GameObject.Destroy(saveAs.value);

        saveAs.value = null;

        if(clubTier != null)
        {
            // int clubTierGroup = ClubUtils.GetClubTierGroup(clubTier.value);
            saveAs.value = MetaIconUtils.MakeClubTierIconObject(clubTier.value, parent.value, parentName.value);
        }

        EndAction();
    }
}

}
