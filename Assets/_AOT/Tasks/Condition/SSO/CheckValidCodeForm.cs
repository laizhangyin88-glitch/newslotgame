using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/SSO")]
public class CheckValidCodeForm : ConditionTask
{
    public BBParameter<string> code;

    protected override string info { get { return "Check code is valid"; } }
    
    protected override bool OnCheck()
    {
        if (code.value == null) return false;
        
        return (code.value.Length == 5) && BagelCode.RegexUtilities.IsValidCode(code.value);
    }
}

}
