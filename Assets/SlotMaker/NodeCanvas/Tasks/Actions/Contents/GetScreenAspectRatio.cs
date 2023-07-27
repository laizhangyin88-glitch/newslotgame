using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
[Category("★ BagelCode/Contents")]
[Description("Save value as Screen.width/Screen.height")]
public class GetScreenAspectRatio : ActionTask 
{
    public BBParameter<float> saveAs;

    protected override void OnExecute()
    {
        saveAs.value = (float)Screen.width / (float)Screen.height;
        EndAction();
    }
}

}
