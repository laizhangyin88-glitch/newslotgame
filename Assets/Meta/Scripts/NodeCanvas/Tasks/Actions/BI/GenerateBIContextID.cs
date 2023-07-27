using UnityEngine;
using System.Text;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class GenerateBIContextID : ActionTask
{
    public BBParameter<string> saveAsContextID;

    protected override void OnExecute()
    {
        saveAsContextID.value = BiEventUtils.GenerateContextID();
        
        EndAction();
    }
}

}
