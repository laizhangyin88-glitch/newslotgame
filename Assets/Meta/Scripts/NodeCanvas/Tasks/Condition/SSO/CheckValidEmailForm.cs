using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/SSO")]
public class CheckValidEmailForm : ConditionTask
{
    public BBParameter<string> email;

    protected override string info { get { return "Check email is valid"; } }
    
    protected override bool OnCheck()
    {
        return BagelCode.RegexUtilities.IsValidEmail(email.value);
    }
}

}
