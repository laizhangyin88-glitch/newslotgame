using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/BI")]
public class BI_client_click_setting : ActionTask<Blackboard>
{
    public BBParameter<string> types;
    public BBParameter<string> biContextID;

    public bool isSlot;

    protected override void OnExecute()
    {
        Dictionary<string, object> customData = new Dictionary<string, object>();
        if (string.IsNullOrEmpty(biContextID.value))
            biContextID.value = (isSlot) ? BiEventUtils.GetSlotEnterContextID() : BiEventUtils.GenerateContextID();
        customData["type"]       = types.value;
        customData["context_id"] = biContextID.value;
        Analytics.CustomEvent("client_click_setting", customData);

        EndAction();
    }
}
}
