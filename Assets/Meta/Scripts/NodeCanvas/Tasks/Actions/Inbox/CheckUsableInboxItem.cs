using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Inbox")]
public class CheckUsableInboxItem : ConditionTask
{
    public BBParameter<int> itemID;

    protected override string info
    {
        get { return string.Format("Check Usableinbox item {0}", itemID); }
    }

    protected override bool OnCheck() 
    {
        return BlackboardQueryUtils.IsUsableInboxItem(itemID.value);
    }
}

}
