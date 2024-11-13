using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Conditions
{

[Category("★ BagelCode/SSO")]
public class CheckHasAccount : ConditionTask
{
    protected override string info { get { return "I have account"; } }
    
    protected override bool OnCheck()
    {
        return BagelCode.AccountUtils.HasAccount();
    }
}

}
