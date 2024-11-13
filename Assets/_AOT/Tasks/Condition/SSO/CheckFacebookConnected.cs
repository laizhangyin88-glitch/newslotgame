using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Conditions
{

[Category("★ BagelCode/SSO")]
public class CheckFacebookConnected : ConditionTask
{
    protected override string info { get { return "Check if connected facebook."; } }
    
    protected override bool OnCheck()
    {
        return BagelCode.AccountUtils.IsFacebookConnected();
    }
}

}
