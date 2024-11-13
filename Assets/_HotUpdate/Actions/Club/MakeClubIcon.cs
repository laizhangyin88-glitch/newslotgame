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

public class MakeClubIcon : ActionTask<Blackboard>
{
    public BBParameter<string>  symbolNameValue;
    public BBParameter<Transform> parent;
    public BBParameter<string> parentName;

    public BBParameter<GameObject> saveAs;

    protected override string info
    {
        get { return "Make Club Icon"; }
    }

    protected override void OnExecute()
    {
        var symbolName = BlackboardUtils.FindVariable<string>(agent, symbolNameValue.value);

        if(saveAs.value != null)
            GameObject.Destroy(saveAs.value);

        saveAs.value = null;

        if(symbolName != null && !string.IsNullOrEmpty(symbolName.value))
        {
            saveAs.value = MetaIconUtils.MakeClubSymbolIconObject(symbolName.value, parent.value, parentName.value);
        }

        EndAction();
    }
}

}
