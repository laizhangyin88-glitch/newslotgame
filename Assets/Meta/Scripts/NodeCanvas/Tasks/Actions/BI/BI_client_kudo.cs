using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_kudo : ActionTask<Blackboard>
{
    public BBParameter<string> type;
    public BBParameter<string> targetUserID;
    public BBParameter<int> kudoID;
    public BBParameter<string> kudoType;

    protected override string info 
    { 
        get 
        { 
            return string.Format("BI Kudo {0}, {1}, {2}, {3}", type, targetUserID, kudoID, kudoType);
        } 
    }

    protected override void OnExecute()
    {
        Dictionary<string, object> customData = new Dictionary<string, object>();

        // type: string ("trigger", "click"),
        // target_user_id: string,
        // kudo_id: number,
        // kudo_type: string ("kudo_jackpot", "kudo_tournament", "kudo_receive"),
        
        customData["type"] = type.value;
        customData["target_user_id"] = targetUserID.value;
        customData["kudo_id"] = kudoID.value;
        customData["kudo_type"] = kudoType.value;
        customData["slot_enter_context_id"] = BiEventUtils.GetSlotEnterContextID();

        Analytics.CustomEvent("client_kudo", customData);

        EndAction();
    }

}

}
