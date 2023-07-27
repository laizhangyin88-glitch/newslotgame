using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.Json;

namespace BagelCode.Tasks.Condition
{

[Category("★ BagelCode/NativeHelper")]
public class CheckFacebookPermission : ConditionTask
{
    public BBParameter<string> permission;
    protected override string info
    {
        get
        { 
            return string.Format("Check facebook {0} permission", permission); 
        }
    }

    protected override bool OnCheck()
    {
#if !UNITY_EDITOR
        return SocialManager.Instance.HasPermission(permission.value);
#else
        return false;
#endif
    }
}

}
