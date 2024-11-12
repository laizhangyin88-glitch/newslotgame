using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Utils")]
public class GetClientVersionNumber : ActionTask 
{
    [BlackboardOnly]
    public BBParameter<int> saveAs;

    protected override string info
    {
        get { return "Get Client Version Number in Application Settings"; }
    }

    protected override void OnExecute()
    {
        saveAs.value = ApplicationSettings.GetClientVersionNumber();
        EndAction();
        
    }
}

}
