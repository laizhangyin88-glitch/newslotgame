using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Utils")]
public class GetProductVersion : ActionTask 
{
    [BlackboardOnly]
    public BBParameter<string> saveAs;

    protected override string info
    {
        get { return "Get Product Version in Application Settings"; }
    }

    protected override void OnExecute()
    {
        saveAs.value = ProductSettings.Instance.productVersion;
        EndAction();
        
    }
}

}
