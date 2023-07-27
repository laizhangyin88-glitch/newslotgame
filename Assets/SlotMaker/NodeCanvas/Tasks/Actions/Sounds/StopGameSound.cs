using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Sounds")]
public class StopGameSound : ActionTask
{
    public BBParameter<GameSound> gameSound;

    protected override string info { get { return "Stop " + gameSound; } }

    protected override void OnExecute()
    {
        // if (gameSound != null &&
        if(gameSound.value != null)
            gameSound.value.Stop();

        EndAction();
    }
}

}
