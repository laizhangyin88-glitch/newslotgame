using System.Collections;using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Conditions
{

[Category("★ BagelCode/SSO")]
public class CheckEmailConnected : ConditionTask
{
    protected override string info { get { return "Check if connected email."; } }
    
    protected override bool OnCheck()
    {
        return BagelCode.AccountUtils.IsEmailConnected();
    }
}

}
