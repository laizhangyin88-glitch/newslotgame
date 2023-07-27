using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/NativeHelper")]
public class LogoutFacebook : ActionTask
{
    protected override string info { get { return "Logout Facebook"; } }
    
    protected override void OnExecute()
    {
        SocialManager.Instance.LogoutFB();
        EndAction();
    }
}

}
