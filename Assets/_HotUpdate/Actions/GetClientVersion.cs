using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Utils")]
public class GetClientVersion : ActionTask 
{
    [BlackboardOnly]
    public BBParameter<string> saveAs;

    protected override string info
    {
        get { return "Get Client Version in Application Settings"; }
    }

    protected override void OnExecute()
    {
        saveAs.value = ApplicationSettings.Instance.clientVersion;
        EndAction();
        
    }
}

}
